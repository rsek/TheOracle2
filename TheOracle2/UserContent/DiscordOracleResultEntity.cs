using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TheOracle2.DataClasses;
using TheOracle2.OracleRoller;
namespace TheOracle2.UserContent;
internal class DiscordOracleResultEntity : IDiscordEntity
{
    // TODO:
    // should probably be initialized with an OracleResult
    // needs a way to check embeds for existing rolls
    // and reference them against roll Max, repeating, etc
    // e.g. AddRollsToEmbed?
    //
    // function to infer what the add'l rolls should be:
    // * same category
    // * used with key

    public DiscordOracleResultEntity(EFContext dbContext, Random random, string oracleId) : this(dbContext, random)
    {
        OracleResult = new OracleResult(DbContext, Random)
        {
            new OracleRoll(DbContext, Random, oracleId)
        };
    }
    public DiscordOracleResultEntity(OracleResult oracleResult) : this(oracleResult.DbContext, oracleResult.Random)
    {
        OracleResult = oracleResult;
    }
    private DiscordOracleResultEntity(EFContext dbContext, Random random)
    {
        DbContext = dbContext;
        Random = random;
    }
    public EFContext DbContext { get; set; }
    public Random Random { get; set; }
    public bool IsEphemeral { get; set; } = false;
    public OracleResult OracleResult { get; set; }

    public List<DiscordOracleRollEntity> GetRolls() => OracleResult.Select(oracleRoll =>
           new DiscordOracleRollEntity(oracleRoll)
        ).ToList();

    public static EmbedBuilder AddRoll(EFContext dbContext, Random random, EmbedBuilder embed, Oracle oracle)
    {
        /// iterate over fields
        var siblingRolls = embed.Fields.Where(field => field.Name.StartsWith(oracle.Id));
        DiscordOracleRollEntity entity;
        if (!siblingRolls.Any())
        {
            // no existing rolls from this table to conflict with - just add it
            entity = new DiscordOracleRollEntity(dbContext, random, oracle);
            return embed.AddField(entity.ToEmbedField());
        }
        var blacklistRowsWith = siblingRolls.Select(field => DiscordOracleRollEntity.GetFirstInteger(field.Name));
        entity = new DiscordOracleRollEntity(dbContext, random, oracle, blacklistRowsWith);
        return embed.AddField(entity.ToEmbedField());
    }
    public EmbedBuilder ToEmbed()
    {
        var fields = GetRolls()
            .Select(oracleRollEntity =>
                oracleRollEntity
                    .ToEmbedField()
                    );
        var embed = new EmbedBuilder()
            .WithTitle("Oracle results")
            .WithFields(fields);
        return embed;
        // TODO: split over multiple embeds if it exceeds the number of fields?
        // should either have a size limit of 25 to accomodate embeds, or else have some logic to account for multiple embeds
    }
    public static List<Oracle> GetSiblingOracles(EFContext dbContext, IEnumerable<string> oracleIds)
    {
        // core oracles included by default
        var paths = new List<string>() { "Core" };
        // add items that share the same parent category
        paths.AddRange(oracleIds.Select(item => PathOneLevelUp(item)));

        paths = paths.Distinct().ToList();
        var siblingOracles = dbContext.OracleCategories.Where(oracleCat =>
            paths.Any(path => path == oracleCat.Id) &&
            oracleCat.Oracles != null &&
            oracleCat.Oracles.Any()
        ).SelectMany(oracleCat => oracleCat.Oracles)
            .ToList();
        // var siblingOracles = dbContext.Oracles.Select(item => new string(item.Id)).ToList();
        // siblingOracles = siblingOracles.Where(item => paths.Any(path => item.StartsWith(path))).ToList();
        return siblingOracles;
    }
    public List<Oracle> GetSiblingOracles()
    {
        var oracleIds = OracleResult.Select(roll => !string.IsNullOrEmpty(roll.Oracle.SubtableOf) ? roll.Oracle.SubtableOf : roll.Oracle.Id);
        Console.WriteLine(string.Join(", ", oracleIds));
        return GetSiblingOracles(DbContext, oracleIds);
    }
    public static string PathOneLevelUp(string oracleId)
    {
        const string pathSeparator = " / ";
        var pathParts = oracleId.Split(pathSeparator);
        var pathOneLevelUp = string.Join(pathSeparator, pathParts.SkipLast(1));
        return pathOneLevelUp;
    }
    private List<Oracle> GetSiblingOracles(string oracleId)
    {
        var pathOneLevelUp = PathOneLevelUp(oracleId);
        var siblingOracles = DbContext.Oracles.Where(item => item.Id.StartsWith(pathOneLevelUp)).ToList();
        // TODO: skip up one level if it's a e.g. regional subtable (SelectBy??)
        // TODO: 'usewith' oracles - separate method?
        return siblingOracles;
    }
    private List<Oracle> GetSiblingOracles(Oracle oracle)
    {
        return GetSiblingOracles(!string.IsNullOrEmpty(oracle.SubtableOf) ? oracle.SubtableOf : oracle.Id);
    }
    public MessageComponent GetComponents()
    {
        var components = new ComponentBuilder();
        var siblingEntities = GetSiblingOracles().ConvertAll(oracle => new DiscordOracleEntity(DbContext, Random, oracle));
        var menu = new SelectMenuBuilder()
            .WithCustomId("oracle-add-menu")
            .WithMaxValues(1)
            .WithMinValues(0)
            .WithPlaceholder("Add an oracle roll...")
        ;
        siblingEntities.ForEach(item => menu.AddOption(item.ToMenuOption()));
        // iterate pre-existing rolls thru the menu option decrementer
        OracleResult.ForEach(oracleResult =>
        {
            var valueString = DiscordOracleEntity.ToMenuOptionValue(oracleResult.Oracle);
            menu = DiscordOracleEntity.DecrementOracleOption(menu, valueString);
        });
        components.WithSelectMenu(menu);
        return components.Build();
        // placeholder menu w/ action + theme, etc
    }
    public async Task<IMessage> GetDiscordMessage(IInteractionContext context)
    {
        return null;
    }
    public Embed[] GetEmbeds()
    {
        return new Embed[] { ToEmbed().Build() };
    }
}
