using System.Text.RegularExpressions;
using TheOracle2.DataClasses;
using TheOracle2.GameObjects;
using TheOracle2.UserContent;
namespace TheOracle2.OracleRoller;
/// <summary>
/// Represents an oracle roll result composed of one or more oracle rolls.
/// </summary>
public class OracleResult : List<OracleRoll>
{
    /// <summary>
    /// Create oracle rolls from several oracles at once.
    /// </summary>
    /// TODO: allowDuplicateRolls should be a function of the table - being able to set it at the oracle result level just complicates things.
    public OracleResult(EFContext dbContext, Random random, IEnumerable<Oracle> oracles, bool allowDuplicateRolls = false) : this(dbContext, random, allowDuplicateRolls)
    {
        var oracleList = oracles.ToList();
        for (int i = 0; i < oracles.Count(); i++)
        {
            var oracle = oracleList[i];
            if (!AllowDuplicateRolls && UsedRolls(oracle) != null && UsedRolls(oracle).Any())
            {
                // TODO: this should use proper row comparison instead, probably?
                Add(new OracleRoll(dbContext, random, oracle, UsedRolls(oracle)));
                continue;
            }
            Add(new OracleRoll(dbContext, random, oracle));
        }
    }
    /// <summary>
    /// Create one or more rolls from a single oracle
    /// </summary>
    public OracleResult(EFContext dbContext, Random random, Oracle oracle, int amount = 1, bool allowDuplicateRolls = false) : this(dbContext, random, allowDuplicateRolls)
    {
        for (int i = 0; i < amount; i++)
        {
            if (!AllowDuplicateRolls && i > 0 && UsedRolls(oracle).Count > 0)
            {
                Add(new OracleRoll(dbContext, random, oracle, UsedRolls(oracle)));
                continue;
            }
            Add(new OracleRoll(dbContext, random, oracle));
        }
    }
    /// <summary>
    /// Create an empty oracle roll group.
    /// </summary>
    public OracleResult(EFContext dbContext, Random random, bool allowDuplicateRolls = false)
    {
        DbContext = dbContext;
        Random = random;
        AllowDuplicateRolls = allowDuplicateRolls;
    }
    public List<Oracle> Oracles => this.Select(oracleRoll => oracleRoll.Oracle).ToList();
    // TODO: actually implement stuff that uses this
    public bool AllowDuplicateRolls { get; set; }
    private List<int> UsedRolls(Oracle oracle)
    {
        return this.Where(roll => roll.Oracle == oracle).Select(roll => roll.Value).ToList();
    }
    public EFContext DbContext { get; set; }
    public Random Random { get; set; }
}
