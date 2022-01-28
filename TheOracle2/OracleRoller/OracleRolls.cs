using System.Text.RegularExpressions;
using TheOracle2.DataClasses;
using TheOracle2.GameObjects;
using TheOracle2.UserContent;

namespace TheOracle2.OracleRoller;

/// <summary>
/// A list of OracleRolls with additional methods for embed generation. Also serves as a base for game objects like Starships and NPCs.
/// </summary>
public class OracleRolls : List<OracleRoll>
{
    /// <summary>
    /// Create oracle rolls from several oracles at once.
    /// </summary>
    public OracleRolls(EFContext dbContext, Random random, IEnumerable<Oracle> oracles, bool allowDuplicateRolls = false) : this(dbContext, random, allowDuplicateRolls)
    {
        var oracleList = oracles.ToList();
        for (int i = 0; i < oracles.Count(); i++)
        {
            var oracle = oracleList[i];
            if (!AllowDuplicateRolls && UsedRolls != null && UsedRolls.Any())
            {
                // TODO: this should use proper row comparison instead, probably?
                Add(new OracleRoll(dbContext, random, oracle, UsedRolls));
                continue;
            }
            Add(new OracleRoll(dbContext, random, oracle));
        }
    }

    /// <summary>
    /// Create one or more rolls from a single oracle
    /// </summary>
    public OracleRolls(EFContext dbContext, Random random, Oracle oracle, int amount = 1, bool allowDuplicateRolls = false) : this(dbContext, random, allowDuplicateRolls)
    {
        for (int i = 0; i < amount; i++)
        {
            if (!AllowDuplicateRolls && UsedRolls.Any())
            {
                Add(new OracleRoll(dbContext, random, oracle, UsedRolls));
                continue;
            }
            Add(new OracleRoll(dbContext, random, oracle));
        }
    }
    /// <summary>
    /// Create an empty oracle roll group.
    /// </summary>
    public OracleRolls(EFContext dbContext, Random random, bool allowDuplicateRolls = false)
    {
        DbContext = dbContext;
        Random = random;
        AllowDuplicateRolls = allowDuplicateRolls;
    }
    public List<Oracle> Oracles => this.Select(oracleRoll => oracleRoll.Oracle).ToList();

    // TODO: actually implement stuff that uses this
    public bool AllowDuplicateRolls { get; set; }

    private int[] UsedRolls => this.Select(oracleroll => oracleroll.Value) as int[];
    public EFContext DbContext { get; set; }
    public Random Random { get; set; }

}
