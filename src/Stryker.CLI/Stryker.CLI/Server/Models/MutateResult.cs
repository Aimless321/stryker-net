using System.Collections.Generic;
using Newtonsoft.Json;
using Stryker.Abstractions;
using Stryker.Abstractions.Reporting;

namespace Stryker.CLI.Server.Models;

public class MutationTestResult
{
    [JsonProperty("files")]
    public IDictionary<string, MutantResultFile> Files { get; init; } = new Dictionary<string, MutantResultFile>();

}

public class MutantResultFile
{
    [JsonProperty("mutants")]
    public IEnumerable<IJsonMutant> Mutants { get; init; }
}
