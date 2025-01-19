using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StreamJsonRpc;
using Stryker.Abstractions;
using Stryker.Abstractions.ProjectComponents;
using Stryker.Abstractions.Reporting;
using Stryker.CLI.Server.Models;
using Stryker.Core.Reporters.Json.SourceFiles;

namespace Stryker.CLI.Server;

public class RpcReporter : IReporter
{
    private readonly JsonRpc _rpc;

    public RpcReporter(JsonRpc rpc)
    {
        _rpc = rpc;
    }

    public void OnMutantsCreated(IReadOnlyProjectComponent reportComponent, ITestProjectsInfo testProjectsInfo)
    {
    }

    public void OnStartMutantTestRun(IEnumerable<IReadOnlyMutant> mutantsToBeTested)
    {
    }

    public void OnMutantTested(IReadOnlyMutant result)
    {
        var path = "";
        foreach (var file in result.Parent.GetAllFiles())
        {
            if (file.Mutants.Contains(result))
            {
                path = file.RelativePath;
                break;
            }
        }

        var mutants = new MutantResultFile()
        {
            Mutants = [new JsonMutant(result)]
        };

        _ = HandleMutantTestResultAsync(new MutationTestResult()
        {
            Files = new Dictionary<string, MutantResultFile>() { { path, mutants } }
        });
    }

    public void OnAllMutantsTested(IReadOnlyProjectComponent reportComponent, ITestProjectsInfo testProjectsInfo)
    {
    }

    private async Task HandleMutantTestResultAsync(MutationTestResult result)
    {
        try
        {
            await _rpc.NotifyWithParameterObjectAsync("reportMutationTestProgress", result);
        }
        catch (Exception ex)
        {
            // Handle or log the exception appropriately
            Console.WriteLine($"Error during mutant test: {ex.Message}");
        }
    }
}
