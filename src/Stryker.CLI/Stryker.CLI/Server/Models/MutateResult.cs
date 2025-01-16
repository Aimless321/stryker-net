using System.Collections.Generic;
using Newtonsoft.Json;
using Stryker.Abstractions.Reporting;

namespace Stryker.CLI.Server.Models;

public class MutationTestResult
{
    [JsonProperty("files")]
    public IDictionary<string, ISourceFile> Files { get; init; } = new Dictionary<string, ISourceFile>();

}
