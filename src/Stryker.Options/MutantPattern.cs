using System;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Stryker.Abstractions.ProjectComponents;
using Stryker.Abstractions.Reporting;

namespace Stryker.Configuration;

public sealed class MutantPattern : IMutantPattern
{
    private static readonly Regex MutantPathRegex = new(@"^(.+?):(\d+):(\d+)-(\d+):(\d+)$", RegexOptions.Compiled);

    public MutantPattern(string path, ILocation location)
    {
        Path = path;
        Location = location;
    }

    /// <summary>
    /// The file path.
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// The location of the mutant.
    /// </summary>
    public ILocation Location { get; }

    public bool Equals(IMutantPattern other) => throw new NotImplementedException();

    /// <summary>
    /// Parses a given mutant pattern string.
    /// Format: (!)&lt;filePath&gt;:&lt;startLine&gt;-&lt;startColumn&gt;:&lt;endLine&gt;-&lt;endColumn&gt;*
    /// </summary>
    /// <param name="pattern">The pattern to parse.</param>
    /// <returns>The <see cref="MutantPattern"/></returns>
    public static MutantPattern Parse(string pattern)
    {
        var match = MutantPathRegex.Match(pattern);
        if (match.Success)
        {
            var filePath = match.Groups[1].Value;
            var startLine = int.Parse(match.Groups[2].Value);
            var startColumn = int.Parse(match.Groups[3].Value);
            var endLine = int.Parse(match.Groups[4].Value);
            var endColumn = int.Parse(match.Groups[5].Value);

            if (startLine > endLine || (startLine == endLine && startColumn > endColumn))
            {
                throw new ArgumentException("Invalid range: start must not be greater than end.");
            }

            return new MutantPattern(filePath,
                new Location(new Position(startLine, startColumn), new Position(endLine, endColumn)));
        }

        throw new ArgumentException("Invalid mutant pattern.");
    }

    public bool IsMatch(string filePath, FileLinePositionSpan location) =>
        filePath == Path && location.StartLinePosition.Line + 1 == Location.Start.Line &&
        location.StartLinePosition.Character + 1 == Location.Start.Column &&
        location.EndLinePosition.Line + 1 == Location.End.Line &&
        location.EndLinePosition.Character + 1 == Location.End.Column;
}
