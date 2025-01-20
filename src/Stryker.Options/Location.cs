using Stryker.Abstractions.Reporting;

namespace Stryker.Configuration;

public class Location : ILocation
{
    public IPosition End { get; init; }
    public IPosition Start { get; init; }

    public Location(IPosition start, IPosition end)
    {
        Start = start;
        End = end;
    }
}
