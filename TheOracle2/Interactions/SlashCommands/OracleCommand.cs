using Discord.Interactions;
using Discord.WebSocket;
using TheOracle2.Commands;
using TheOracle2.DataClasses;
using TheOracle2.OracleRoller;
using TheOracle2.UserContent;

namespace TheOracle2;

public class OracleCommand : InteractionModuleBase
{
    public OracleCommand(EFContext dbContext, Random random)
    {
        DbContext = dbContext;
        Random = random;
    }

    public EFContext DbContext { get; }
    public Random Random { get; }

    [SlashCommand("oracle", "Roll on an oracle table. To ask a yes/no question, use /ask.")]
    public async Task RollOracle(
        [Autocomplete(typeof(OracleAutocomplete))] string oracle,
        [Autocomplete(typeof(OracleAutocomplete))] string oracle2 = "",
        [Autocomplete(typeof(OracleAutocomplete))] string oracle3 = "",
        [Autocomplete(typeof(OracleAutocomplete))] string oracle4 = "",
        [Autocomplete(typeof(OracleAutocomplete))] string oracle5 = "",
        [Autocomplete(typeof(OracleAutocomplete))] string oracle6 = "",
        [Autocomplete(typeof(OracleAutocomplete))] string oracle7 = "",
        [Autocomplete(typeof(OracleAutocomplete))] string oracle8 = "",
        [Autocomplete(typeof(OracleAutocomplete))] string oracle9 = "",
        [Autocomplete(typeof(OracleAutocomplete))] string oracle10 = "",
        [Autocomplete(typeof(OracleAutocomplete))] string oracle11 = "",
        [Autocomplete(typeof(OracleAutocomplete))] string oracle12 = "",
        [Autocomplete(typeof(OracleAutocomplete))] string oracle13 = "",
        [Autocomplete(typeof(OracleAutocomplete))] string oracle14 = "",
        [Autocomplete(typeof(OracleAutocomplete))] string oracle15 = "",
        [Autocomplete(typeof(OracleAutocomplete))] string oracle16 = "",
        [Autocomplete(typeof(OracleAutocomplete))] string oracle17 = "",
        [Autocomplete(typeof(OracleAutocomplete))] string oracle18 = "",
        [Autocomplete(typeof(OracleAutocomplete))] string oracle19 = "",
        [Autocomplete(typeof(OracleAutocomplete))] string oracle20 = "",
        [Autocomplete(typeof(OracleAutocomplete))] string oracle21 = "",
        [Autocomplete(typeof(OracleAutocomplete))] string oracle22 = "",
        [Autocomplete(typeof(OracleAutocomplete))] string oracle23 = "",
        [Autocomplete(typeof(OracleAutocomplete))] string oracle24 = "",
        [Autocomplete(typeof(OracleAutocomplete))] string oracle25 = ""
        )
    {
        await DeferAsync();
        var oracleStrings = new List<string>() {
            oracle, oracle2, oracle3, oracle4, oracle5, oracle6, oracle7, oracle8, oracle9, oracle10, oracle11, oracle12, oracle13, oracle14, oracle15, oracle16, oracle17, oracle18, oracle19, oracle20, oracle21, oracle22, oracle23, oracle24, oracle25,
        }.Where(item => !string.IsNullOrEmpty(item));

        // TODO:
        var oracleData = await Task.FromResult<IQueryable<Oracle>>(DbContext.Oracles.Where(item => oracleStrings.Contains(item.Id)));
        // var oracleData = await DbContext.Oracles.FindAsync(oracle);
        var oracleList = new List<Oracle>();
        foreach (var oracleIds in oracleStrings)
        {
            oracleList.Add(oracleData.FirstOrDefault(item => item.Id == oracleIds));
        }

        var results = new OracleRolls(DbContext, Random, oracleList);
        var embed = results.ToEmbed();

        await this.FollowupAsync(
                embed: embed.Build()
            ).ConfigureAwait(false);
    }
}

// public class OracleComponents : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
// {
//     public TableRollerFactory RollerFactory { get; }

//     public OracleComponents(EFContext dbContext, Random random)
//     {
//         DbContext = dbContext;
//         RollerFactory = new TableRollerFactory(dbContext, random);
//     }

//     public EFContext DbContext { get; }

//     [ComponentInteraction("add-oracle-select")]
//     public async Task FollowUp(string[] values)
//     {
//         var builder = Context.Interaction.Message.Embeds.FirstOrDefault().ToEmbedBuilder();
//         foreach (var value in values)
//         {
//             var roll = RollerFactory.GetRoller(value).Build();

//             builder = DiscordOracleBuilder.AddFieldsToBuilder(roll, builder);
//         }

//         await Context.Interaction.UpdateAsync(msg =>
//         {
//             msg.Embeds = new Embed[] { builder.Build() };
//         }).ConfigureAwait(false);
//     }

//     [ComponentInteraction("tables-oracle-*")]
//     public async Task SelectedTableRoll(string oracleId, string[] values)
//     {
//         int.TryParse(values.FirstOrDefault(), out int Id);

//         var table = DbContext.Tables.Find(Id);

//         var rollResult = RollerFactory.GetRoller(table).Build();

//         var ob = new DiscordOracleBuilder(rollResult).Build();

//         await Context.Interaction.UpdateAsync(msg =>
//         {
//             msg.Embed = ob.EmbedBuilder.Build();
//             msg.Components = ob.ComponentBuilder.Build();
//             msg.Content = "";
//         }).ConfigureAwait(false);
//     }
// }
