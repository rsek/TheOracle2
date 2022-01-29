using Discord.Interactions;
using Discord.WebSocket;
using TheOracle2.Commands;
using TheOracle2.DiscordHelpers;
using TheOracle2.UserContent;

namespace TheOracle2;

public class OracleComponents : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
{
    public OracleComponents(EFContext dbContext, Random random)
    {
        DbContext = dbContext;
        Random = random;
    }
    public EFContext DbContext { get; }
    public Random Random { get; }
    public SocketMessage Message => Context.Interaction.Message;
    public SocketMessageComponent Interaction => Context.Interaction;

    [ComponentInteraction("oracle-add-menu")]
    public async Task OracleRollMenu(string[] values)
    {

        // TODO: handling for multiple embeds - currently it just defaults to the first.

        // generally these should be 1 roll per action, but it can handle

        var oracles = values.Select(item => item.Split(",").Last()).ToList();
        var customId = Interaction.Data.CustomId;

        var rollEntities = oracles.ConvertAll(oracle => new DiscordOracleRollEntity(DbContext, Random, oracle));

        ComponentBuilder oldComponents = ComponentBuilder.FromComponents(Message.Components);
        var menu = oldComponents.GetComponentById(customId) as SelectMenuComponent;
        var menuBuilder = menu.ToBuilder();
        var embeds = Message.Embeds.ToList();
        var targetEmbed = embeds[0].ToEmbedBuilder();
        rollEntities.ForEach(rollEntity =>
            targetEmbed
            .AddField(rollEntity.ToEmbedField()));
        values.ToList().ForEach(value =>
        {
            menuBuilder = DiscordOracleEntity.DecrementOracleOption(menuBuilder, value);
        });

        if (menuBuilder.Options?.Count > 0)
        { oldComponents = oldComponents.ReplaceComponentById(customId, menuBuilder.Build()); }
        if (menuBuilder.Options.Count == 0)
        {
            oldComponents.RemoveComponentById(customId);
        }

        embeds[0] = targetEmbed.Build();

        await Interaction.UpdateAsync(msg =>
        {
            msg.Embeds = embeds.ToArray();
            msg.Components = oldComponents.Build();

        }).ConfigureAwait(false);
        return;
        // TODO: remove 'expended' rolls from components after using them. this might mean some kind of "rolls remaining" element in the values?
    }

    // // TODO: component to handle picking a specific result from a row and including it as a fake oracle roll.
    // // [ComponentInteraction("oracle-row-menu")]

    // [ComponentInteraction("oracle-roll:*,*,[*]")]
    // public async Task AddOracleRoll(string remainingRollsString, string oracleId, string rollBlacklistString)
    // {
    //     if (!int.TryParse(remainingRollsString, out var remainingRolls))
    //     {
    //         throw new ArgumentException($"Unable to parse remaining rolls from customId: {Context.Interaction.Data.CustomId}");
    //     }

    //     var rollsFromSameOracle = Message.Embeds.FirstOrDefault().Fields.Where(field => field.Name.StartsWith(oracleId));

    //     if (remainingRolls > 0) {

    //         remainingRolls--;
    //     }



    //     if (remainingRolls == 0)
    //     {
    //         // if it's been reduced to 0, remove by customId
    //     }
    //     // NYI: if it's -1, it can be used indefinitely

    // }
}
