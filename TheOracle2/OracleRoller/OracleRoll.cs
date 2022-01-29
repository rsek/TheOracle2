using System.Text.RegularExpressions;
using TheOracle2.DataClasses;
using TheOracle2.GameObjects;
using TheOracle2.UserContent;

namespace TheOracle2.OracleRoller;

public class OracleRoll : Die
{
    public OracleRoll(EFContext dbContext, Random random, string oracleId, int? roll = null, int[] secondaryRolls = null) : this(dbContext, random, dbContext.Oracles.Find(oracleId), roll)
    {
    }
    public OracleRoll(EFContext dbContext, Random random, Oracle oracle, int? roll = null, int[] secondaryRolls = null) : this(dbContext, random, roll)
    {
        if (oracle.Table == null || !oracle.Table.Any())
        {
            throw new Exception("Oracle must have a table to roll on; if it contains Oracles of its own, check the Tables of those instead.");
        }
        Oracle = oracle;
        if (Row.MultipleRolls != null)
        {
            Rolls = new OracleResult(DbContext, Random, Oracle, Row.MultipleRolls.Amount, Row.MultipleRolls.AllowDuplicates);
        }
        if (Row.OracleRolls?.Any() == true)
        {
            var referencedOracles = DbContext.Oracles.Where(oracle => Row.OracleRolls.Contains(oracle.Id));

            Rolls = new OracleResult(DbContext, Random, referencedOracles, false);
        }
    }
    /// <summary>
    /// blacklist rows by result string
    /// </summary>
    public OracleRoll(EFContext dbContext, Random random, Oracle oracle, IEnumerable<string> rowBlacklist) : this(dbContext, random, oracle, oracle.Table.Where(row => !rowBlacklist.Contains(row.Result)))
    {
    }
    /// <summary>
    /// blacklist rows by roll - if a row includes the value, it's excluded
    /// </summary>
    public OracleRoll(EFContext dbContext, Random random, Oracle oracle, IEnumerable<int> rollBlacklist) : this(dbContext, random, oracle, oracle.Table.Where(row => !rollBlacklist.Any(blacklistedNumber => row.RollIsInRange(blacklistedNumber))))
    {
    }
    private OracleRoll(EFContext dbContext, Random random, Oracle oracle, IEnumerable<RollableTableRow> whitelistedRows) : this(dbContext, random, oracle)
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
    private List<int> BuildRollWhitelist(IEnumerable<RollableTableRow> rowWhitelist)
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
    public Oracle Oracle { get; set; }
    public RollableTableRow Row => this.Oracle.Table.Lookup(Value);

    /// <summary>
    /// For multiple roll results as with "Roll Twice" or "Action + Theme" results
    /// </summary>
    public OracleResult Rolls { get; set; }
}
