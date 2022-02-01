using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TheOracle2.DataClasses;
using TheOracle2.OracleRoller;
namespace TheOracle2.UserContent;
internal class DiscordOracleResultEntity : List<DiscordOracleRollEntity>, IDiscordEntity
{
    /// <summary>
    /// Initialize with a new OracleResult from multiple oracles.
    /// </summary>
    public DiscordOracleResultEntity(EFContext dbContext, Random random, IEnumerable<OracleTable> tables) : this(new OracleResult(dbContext, random, tables))
    {
    }
    /// <summary>
    /// Initialize with a new OracleResult from multiple oracles.
    /// </summary>
    public DiscordOracleResultEntity(EFContext dbContext, Random random, IEnumerable<string> tableIds) : this(dbContext, random, dbContext.OracleTables.Where(table => tableIds.Contains(table.Path)))
    {
    }
    /// <summary>
    /// Initialize with a new OracleResult containing one or more results from a single oracle.
    /// </summary>
    // public DiscordOracleResultEntity(EFContext dbContext, Random random, string oracleId, int amount = 1) : this(new OracleResult(dbContext, random, oracleId, amount))
    // {
    // }
    /// <summary>
    /// Initialize with a new OracleResult containing one or more results from a single oracle.
    /// </summary>
    public DiscordOracleResultEntity(EFContext dbContext, Random random, OracleTable table, int amount = 1) : this(new OracleResult(dbContext, random, table, amount))
    {
    }
    /// <summary>
    /// Initialize from an existing OracleResult.
    /// </summary>
    private DiscordOracleResultEntity(OracleResult oracleResult)
    {
        DbContext = oracleResult.DbContext;
        Random = oracleResult.Random;
        OracleResult = oracleResult;
        AddRange(OracleResult.ConvertAll(roll => new DiscordOracleRollEntity(roll)));
    }
    public EFContext DbContext { get; set; }
    public Random Random { get; set; }
    public bool IsEphemeral { get; set; } = false;
    public OracleResult OracleResult { get; set; }
    public static EmbedBuilder AddRoll(EFContext dbContext, Random random, EmbedBuilder embed, OracleTable table)
    {
        /// iterate over fields
        var siblingRolls = embed.Fields.Where(field => field.Name.StartsWith(table.Path));
        DiscordOracleRollEntity entity;
        if (!siblingRolls.Any())
        {
            // no existing rolls from this table to conflict with - just add it
            entity = new DiscordOracleRollEntity(dbContext, random, table);
            return embed.AddField(entity.ToEmbedField());
        }
        var blacklistRowsWith = siblingRolls.Select(field => DiscordOracleRollEntity.GetFirstInteger(field.Name));
        entity = new DiscordOracleRollEntity(dbContext, random, table, blacklistRowsWith);
        return embed.AddField(entity.ToEmbedField());
    }
    public EmbedBuilder ToEmbed()
    {
        var fields = ConvertAll(oracleRollEntity =>
                oracleRollEntity.ToEmbedField()
                    );
        var embed = new EmbedBuilder()
            .WithTitle("Oracle results")
            .WithFields(fields);
        return embed;
        // TODO: split over multiple embeds if it exceeds the number of fields?
        // should either have a size limit of 25 to accomodate embeds, or else have some logic to account for multiple embeds
    }
    public List<OracleTable> GetSiblingTables()
    {
        // TODO: filter for stuff what's already been rolled against
        return OracleResult.Tables.SelectMany(table => table.Category.TablesWithin()).Distinct().ToList();
    }
    public MessageComponent GetComponents()
    {
        var components = new ComponentBuilder();
        var siblingEntities = GetSiblingTables().ConvertAll(table => new DiscordOracleTableEntity(DbContext, Random, table));
        var menu = new SelectMenuBuilder()
            .WithCustomId("oracle-add-menu")
            .WithMaxValues(1)
            .WithMinValues(0)
            .WithPlaceholder("Add an oracle roll...")
        ;
        siblingEntities.ForEach(item => menu.AddOption(item.ToMenuOption()));
        menu.Options = menu.Options.Distinct().ToList();

        menu = DiscordOracleTableEntity.DecrementOracleOptions(menu, this);
        components.WithSelectMenu(menu);
        return components.Build();
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
