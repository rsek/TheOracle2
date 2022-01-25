using System.Text.RegularExpressions;
using TheOracle2.DataClassesNext;
using TheOracle2.GameObjects;
using TheOracle2.UserContent;

namespace TheOracle2.OracleRoller;

public class OracleRolls : List<OracleRoll>
{
    /// <summary>
    /// Create oracle rolls from several oracles at once.
    /// </summary>
    public OracleRolls(EFContext dbContext, Random random, IEnumerable<Oracle> oracles, bool allowDuplicateRolls = false) : this(dbContext, random, allowDuplicateRolls) { }

    /// <summary>
    /// Create multiple oracle rolls from a single oracle.
    /// </summary>
    public OracleRolls(EFContext dbContext, Random random, Oracle oracle, int amount = 2, bool allowDuplicateRolls = false) : this(dbContext, random, allowDuplicateRolls) { }

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
    public bool AllowDuplicateRolls { get; set; }
    public EFContext DbContext { get; set; }
    public Random Random { get; set; }

    // how should this be accepting inputs or filtering? does it *need* to? hmm.
    public EmbedBuilder ToEmbed()
    {
        var embed = new EmbedBuilder().WithFields(this.Select(oracleRoll => oracleRoll.ToField()));
        return embed;
    }

    // should either have a size limit of 25 to accomodate embeds, or else have some logic to account for multiple embeds
}
