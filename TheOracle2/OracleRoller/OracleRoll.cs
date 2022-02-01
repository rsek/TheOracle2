using System.Text.RegularExpressions;
using TheOracle2.DataClasses;
using TheOracle2.GameObjects;
using TheOracle2.UserContent;

namespace TheOracle2.OracleRoller;

/// <summary>
/// Represents a single oracle roll, absent of context. In most cases you shouldn't construct this directly - instead, use OracleResult.
/// </summary>
public class OracleRoll : Die
{
    public OracleRoll(EFContext dbContext, Random random, string tableId, int? roll = null, IEnumerable<int> secondaryRolls = null) : this(dbContext, random, dbContext.OracleTables.Find(tableId), roll)
    {
    }
    public OracleRoll(EFContext dbContext, Random random, OracleTable table, int? roll = null, IEnumerable<int> secondaryRolls = null) : this(dbContext, random, roll)
    {
        Table = table;
        if (Row.MultipleRolls != null)
        {
            Rolls = new OracleResult(DbContext, Random, Table, Row.MultipleRolls.Amount);
        }
        if (Row.OracleRolls?.Any() == true)
        {
            // TODO: have the DB generate these references instead of trying to parse them.
            var referencedTables = DbContext.OracleTables.Where(oracle => Row.OracleRolls.Contains(oracle.Path));
            Rolls = new OracleResult(DbContext, Random, referencedTables);
        }
    }
    /// <summary>
    /// blacklist rows by result string
    /// </summary>
    public OracleRoll(EFContext dbContext, Random random, OracleTable table, IEnumerable<string> rowBlacklist) : this(dbContext, random, table, table.Where(row => !rowBlacklist.Contains(row.Result)))
    {
    }
    /// <summary>
    /// blacklist rows by roll - if a row includes the value, it's excluded
    /// </summary>
    public OracleRoll(EFContext dbContext, Random random, OracleTable table, IEnumerable<int> rollBlacklist) : this(dbContext, random, table, table.Where(row => !rollBlacklist.Any(blacklistedNumber => row.RollIsInRange(blacklistedNumber))))
    {
    }
    private OracleRoll(EFContext dbContext, Random random, OracleTable table, IEnumerable<OracleTableRow> whitelistedRows) : this(dbContext, random, table)
    {
        var filteredNumbers = BuildRollWhitelist(whitelistedRows);
        if (filteredNumbers?.Count == 0)
        {
            /// all results blacklisted - fallback to rolling normally
            return;
        }
        if (filteredNumbers?.Count == 1)
        {
            Value = filteredNumbers[0];
            return;
        }
        Value = RollFromWhitelist(filteredNumbers);
    }
    private OracleRoll(EFContext dbContext, Random random, int? roll = null) : base(random, 100, roll)
    {
        DbContext = dbContext;
        Rolls = new OracleResult(DbContext, Random);
    }
    /// <summary>
    /// Generates a list of permissable d100 rolls from a provided list of permissable rows.
    /// </summary>
    private List<int> BuildRollWhitelist(IEnumerable<OracleTableRow> rowWhitelist)
    {
        var rowList = rowWhitelist.ToList();
        List<int> filteredNumbers = new();
        for (int i = 0; i < rowWhitelist.Count(); i++)
        {
            var row = rowList[i];
            var rowRange = Enumerable.Range(row.Floor, row.Ceiling);
            filteredNumbers.AddRange(rowRange);
        }
        return filteredNumbers;
    }
    private int RollFromWhitelist(IEnumerable<int> whitelist)
    {
        return whitelist.ToList()[Random.Next(0, whitelist.Count())];
    }
    public EFContext DbContext { get; set; }
    public Random Random { get; set; }
    /// <summary>
    /// The originating oracle of this roll (in other words, the last oracle ancestor of OracleRoll.Table).
    /// </summary>
    public OracleInfo OracleInfo => Table.OracleInfo;
    /// <summary>
    /// The originating table of this roll.
    /// </summary>
    public OracleTable Table { get; set; }
    /// <summary>
    /// The row data for this roll.
    /// </summary>
    public OracleTableRow Row => Table.Lookup(Value);
    /// <summary>
    /// For multiple roll results as with "Roll Twice" or "Action + Theme" results
    /// </summary>
    public OracleResult Rolls { get; set; }
    public override string ToString()
    {
        // TODO: construct strings for multiple roll result.
        // if (Rolls.Count > 0) {
        //     return Row.ToString();
        // }
        return Row.ToString();
    }
}
