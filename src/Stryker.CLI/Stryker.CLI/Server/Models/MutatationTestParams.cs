using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Stryker.CLI.Server.Models;

public class MutatationTestParams
{
    public const string CommandName = "mutationTest";

    [JsonPropertyName("files")]
    public IEnumerable<string> Files { get; set; }
}
