using System.Text.RegularExpressions;
using TheOracle2.DataClasses;
using TheOracle2.GameObjects;
using TheOracle2.UserContent;
namespace TheOracle2.OracleRoller;
/// <summary>
/// Represents an oracle result composed of one or more oracle rolls.
/// </summary>
public class OracleResult : List<OracleRoll>
{
    /// <summary>
    /// Create oracle rolls from several oracle tables at once.
    /// </summary>
    // public OracleResult(EFContext dbContext, Random random, IEnumerable<string> tableIds) : this(dbContext, random, dbContext.OracleTables.Where(table => tableIds.Contains(table.Path))) { }
    /// <summary>
    /// Create oracle rolls from several oracle tables at once.
    /// </summary>
    public OracleResult(EFContext dbContext, Random random, IEnumerable<OracleTable> tables) : this(dbContext, random)
    {
        var tableList = tables.ToList();
        for (int i = 0; i < tables.Count(); i++)
        {
            var table = tableList[i];
            var oracle = table.OracleInfo;
            if (oracle.Usage?.AllowDuplicateRolls != true && UsedRolls(table) != null && UsedRolls(table).Any())
            {
                // TODO: this should use proper row comparison instead, probably?
                Add(new OracleRoll(DbContext, Random, table, UsedRolls(table)));
                continue;
            }
            Add(new OracleRoll(DbContext, Random, table));
        }
    }
    /// <summary>
    /// Create one or more oracle rolls from a single oracle (by its path/ID)
    /// </summary>
    // public OracleResult(EFContext dbContext, Random random, string tableId, int amount = 1) : this(dbContext, random, dbContext.OracleTables.Find(tableId), amount) { }
    /// <summary>
    /// Create one or more oracle rolls from a single oracle
    /// </summary>
    public OracleResult(EFContext dbContext, Random random, OracleTable table, int amount = 1) : this(dbContext, random)
    {
        for (int i = 0; i < amount; i++)
        {
            if (table.OracleInfo?.Usage?.AllowDuplicateRolls != true && i > 0 && UsedRolls(table)?.Count > 0)
            {
                Add(new OracleRoll(DbContext, Random, table, UsedRolls(table)));
                continue;
            }
            Add(new OracleRoll(DbContext, Random, table));
        }
    }
    /// <summary>
    /// Create an empty oracle roll group.
    /// </summary>
    public OracleResult(EFContext dbContext, Random random)
    {
        DbContext = dbContext;
        Random = random;
    }
    // TODO: actually implement stuff that uses this
    public List<OracleTable> Tables => this.Select(oracleRoll => oracleRoll.Table).ToList();
    private List<int> UsedRolls(OracleTable table)
    {
        return this.Where(roll => roll.Table == table).Select(roll => roll.Value).ToList();
    }
    public EFContext DbContext { get; set; }
    public Random Random { get; set; }
}
