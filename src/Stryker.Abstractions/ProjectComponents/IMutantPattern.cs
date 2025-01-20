using Microsoft.CodeAnalysis;
using Stryker.Abstractions.Reporting;

namespace Stryker.Abstractions.ProjectComponents;

public interface IMutantPattern
{
    string Path { get; }
    ILocation Location { get; }

    bool IsMatch(string filePath, FileLinePositionSpan location);
}
