using Stryker.Abstractions.Reporting;

namespace Stryker.Configuration;

public class Position : IPosition
{
    public int Column { get; set; }
    public int Line { get; set; }

    public Position(int line, int column)
    {
        Line = line;
        Column = column;
    }
}
