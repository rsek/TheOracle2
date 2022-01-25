using System.Text.RegularExpressions;
using TheOracle2.DataClasses;
using TheOracle2.GameObjects;
using TheOracle2.UserContent;

namespace TheOracle2.OracleRoller;

public class OracleRoll : Die
{
    public OracleRoll(EFContext dbContext, Random random, Oracle oracle, int? roll = null) : base(random, 100, roll)
    {
        if (oracle.Table == null || !oracle.Table.Any())
        {
            throw new Exception("Oracle must have a table to roll on; if it contains Oracles of its own, check the Tables of those instead.");
        }
        DbContext = dbContext;
        Random = random;
        Oracle = oracle;
        // TODO: initialize multiple roll/other oracle table results

    }
    /// <summary>
    /// blacklist rows by result string
    /// </summary>
    public OracleRoll(EFContext dbContext, Random random, Oracle oracle, IEnumerable<string> rowBlacklist) : this(dbContext, random, oracle)
    {
        var blacklistedRows = oracle.Table.Where(row => !rowBlacklist.Contains(row.Result));
        var filteredNumbers = Enumerable.Range(1, 100).Where(roll => blacklistedRows.Any(row => row.RollIsInRange(roll))).ToArray();
        Value = filteredNumbers[random.Next(0, filteredNumbers.Length)];

        // TODO: initialize multiple roll/other oracle table results
    }
    /// <summary>
    /// blacklist rows by roll - if a row includes the value, it's excluded
    /// </summary>
    public OracleRoll(EFContext dbContext, Random random, Oracle oracle, IEnumerable<int> rowBlacklist) : this(dbContext, random, oracle)
    {
        var blacklistedRows = oracle.Table.Where(row => !rowBlacklist.Any(blacklistedNumber => row.RollIsInRange(blacklistedNumber)));
        var filteredNumbers = Enumerable.Range(1, 100).Where(roll => blacklistedRows.Any(row => row.RollIsInRange(roll))).ToArray();
        Value = filteredNumbers[random.Next(0, filteredNumbers.Length)];
        // TODO: initialize multiple roll/other oracle table results
    }
    public EFContext DbContext { get; set; }
    public Random Random { get; set; }
    public Oracle Oracle { get; set; }
    public RollableTableRow Row => this.Oracle.Table.Lookup(Value);
    public EmbedFieldBuilder ToField()
    {
        return Row.ToField(Value);
    }
    // <summary>For multiple roll results as with "Roll Twice" or "Action + Theme" results</summary>
    public OracleRolls Rolls { get; set; }

    public static OracleRoll FromField(EFContext dbContext, Random random, EmbedField field)
    {
        var oraclePathPattern = @"^(.*) \[([0-9]+[,\+\/⏵0-9 ]*)\]$";
        // .*? to handle multiple numbers later, still needs to be split tho
        var oraclePathMatches = Regex.Match(field.Name, oraclePathPattern).Groups;
        var oraclePath = oraclePathMatches[1].Value;
        var rollString = oraclePathMatches[2].Value;
        if (rollString.Contains(' '))
        {
            // single roll, nothing fancy: [99]
            // multiple rolls on same table:
            // '+' separates results that don't allow duplicates
            // ',' separates results that do
            // roll twice: [99: 56 + 12]
            // roll 3 times, allow duplicates: [99: 56, 12, 31]

            // to in-row Table: [99 / 56]
            // to another table: [99 ⏵ 56]
            // to multiple tables, e.g. Action + Theme: [99 ⏵ 56 + 12]
            // the same separators should be used in rendering the field's value, too
            throw new NotImplementedException("handler for multiple-roll oracle fields NYI");
        }
        if (!int.TryParse(rollString, out var dieValue))
        {
            throw new Exception($"Unable to parse roll from {rollString}");
        }
        var oracle = dbContext.Oracles.Find(oraclePath);
        var roll = new OracleRoll(dbContext, random, oracle, dieValue);
        return roll;
    }
}
