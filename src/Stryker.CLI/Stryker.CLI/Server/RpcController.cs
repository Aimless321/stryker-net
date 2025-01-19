using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using StreamJsonRpc;
using Stryker.Abstractions.Exceptions;
using Stryker.Abstractions.Options;
using Stryker.Abstractions.ProjectComponents;
using Stryker.CLI.Logging;
using Stryker.CLI.Server.Models;
using Stryker.Core;
using Stryker.Core.Initialisation;
using Stryker.Core.ProjectComponents;
using Stryker.Core.ProjectComponents.TestProjects;
using Stryker.Core.Reporters.Json;
using Stryker.LanguageServer.Models;

namespace Stryker.CLI.Server;

#nullable enable

public class RpcController
{
    private readonly IFileSystem _fileSystem;
    private readonly IStrykerInputs _inputs;
    private readonly IConfigBuilder _configBuilder;
    private readonly IProjectOrchestrator _projectOrchestrator;
    private readonly IStrykerRunner _stryker;

    public RpcController(
        IFileSystem fileSystem,
        IStrykerInputs inputs,
        IConfigBuilder configBuilder,
        IProjectOrchestrator projectOrchestrator,
        IStrykerRunner stryker)
    {
        _fileSystem = fileSystem;
        _inputs = inputs;
        _configBuilder = configBuilder;
        _projectOrchestrator = projectOrchestrator;
        _stryker = stryker;
    }

    public ConfigureParams Params { get; private set; }
    public IStrykerOptions Options { get; private set; }

    [JsonRpcMethod(ConfigureParams.CommandName, UseSingleObjectParameterDeserialization = true)]
    public ConfigureResult Configure(ConfigureParams @params)
    {
        Params = @params;

        _configBuilder.Build(_inputs, [], null, null);
        _inputs.OutputPathInput.SuppliedInput ??= Path.Combine("StrykerOutput", DateTime.Now.ToString("yyyy-MM-dd.HH-mm-ss"));
        Directory.CreateDirectory(_inputs.OutputPathInput.SuppliedInput);

        return new ConfigureResult
        {
            Version = "0"
        };
    }

    [JsonRpcMethod(DiscoverParams.CommandName, UseSingleObjectParameterDeserialization = true)]
    public DiscoverResult Discover(DiscoverParams @params)
    {
        var mutationTestProcesses = _projectOrchestrator.MutateProjects(Options, null).ToList();

        var rootComponent = AddRootFolderIfMultiProject(mutationTestProcesses.Select(x => x.Input.SourceProjectInfo.ProjectContents).ToList(), Options);
        var combinedTestProjectsInfo = mutationTestProcesses.Select(mtp => mtp.Input.TestProjectsInfo).Aggregate((a, b) => (TestProjectsInfo?)a + (TestProjectsInfo?)b);

        var report = JsonReport.Build(Options, rootComponent, combinedTestProjectsInfo);

        var mutants = report.Files.Values
            .SelectMany(file => file.Mutants)
            .Select(mutant => new DiscoveredMutant
            {
                Id = mutant.Id,
                Description = mutant.Description,
                MutatorName = mutant.MutatorName,
                Location = mutant.Location,
                Replacement = mutant.Replacement
            })
            .ToList();

        return new DiscoverResult
        {
            Mutants = mutants
        };
    }

    [JsonRpcMethod(MutatationTestParams.CommandName, UseSingleObjectParameterDeserialization = true)]
    public MutationTestResult MutationTest(MutatationTestParams @params)
    {
        var filePatterns = @params.Files;
        if (filePatterns != null && filePatterns.Any())
        {
            // TODO: Implement proper mutant filtering
            _inputs.MutateInput.SuppliedInput = filePatterns.Select(fp => fp.EndsWith("/") ? $"{fp}**/*" : fp).ToList();
        }

        new LoggingInitializer().SetupLogOptions(_inputs, _fileSystem);
        _stryker.RunMutationTest(_inputs, ApplicationLogging.LoggerFactory, _projectOrchestrator);

        return new MutationTestResult();
    }

    /// <summary>
    /// In the case of multiple projects we wrap them inside a wrapper root component. Otherwise the only project root will be the root component.
    /// </summary>
    /// <param name="projectComponents">A list of all project root components</param>
    /// <param name="options">The current stryker options</param>
    /// <returns>The root folder component</returns>
    private IReadOnlyProjectComponent? AddRootFolderIfMultiProject(IEnumerable<IReadOnlyProjectComponent> projectComponents, IStrykerOptions options)
    {
        if (!projectComponents.Any())
        {
            throw new NoTestProjectsException();
        }

        if (projectComponents.Count() > 1)
        {
            var rootComponent = new Solution
            {
                FullPath = options.ProjectPath // in case of a solution run the basePath will be where the solution file is
            };
            rootComponent.AddRange(projectComponents.Cast<IProjectComponent>());
            return rootComponent;
        }

        return projectComponents.FirstOrDefault();
    }
}
