using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheOracle2.DataClasses;
using TheOracle2.OracleRoller;

namespace TheOracle2.UserContent;

// TODO: remove this and save it for another PR

/// <summary>
/// For rendering the oracle as a table embed.
/// </summary>
internal class DiscordOracleTableEntity : IDiscordEntity
{
    public Oracle Oracle { get; set; }

    private string RowString(RollableTableRow row)
    {
        string rangeString = $"{row.Floor}-{row.Ceiling}";
        if (row.Floor == row.Ceiling)
        {
            rangeString = row.Floor.ToString();
        }
        return $"`{rangeString,6}` {row}";
    }

    public string TableString(RollableTable table)
    {
        var fakeRows = table.Select(row => RowString(row));
        // could store these in a 2d array instead, hmm
        // then MD tables could be converted by
        return string.Join("\n", fakeRows);
    }

    // Image handling - at row level?
}
