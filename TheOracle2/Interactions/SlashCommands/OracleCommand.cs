using Discord.Interactions;
using Discord.WebSocket;
using TheOracle2.Commands;
using TheOracle2.DiscordHelpers;
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
    public async Task RollOracle([Autocomplete(typeof(OracleAutocomplete))] string oracle)
    {
        var resultEntity = new DiscordOracleResultEntity(DbContext, Random, oracle);

        await RespondAsync(embeds: resultEntity.GetEmbeds(), ephemeral: resultEntity.IsEphemeral, components: resultEntity.GetComponents());
    }
}
