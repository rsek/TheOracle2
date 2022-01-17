using Discord.WebSocket;
using TheOracle2.UserContent;
using Discord.Interactions;

namespace TheOracle2;

/// <summary>
/// Components that access the game DB, used by multiple slash commands.
/// </summary>

public class DbComponents : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
{
  public DbComponents(EFContext dbContext)
  {
    DbContext = dbContext;
  }
  public EFContext DbContext { get; }

  [ComponentInteraction("move-ref-menu")]
  public async Task MoveReferenceMenu(string[] values)
  {
    string idString = values.FirstOrDefault();
    if (!int.TryParse(idString, out int Id))
    {
      throw new Exception($"Unable to parse {idString} into id");
    }
    var db = DbContext.Moves;
    var move = db.Find(Id);
    var moveItems = new DiscordMoveEntity(move, true);
    await Context.Interaction.UpdateAsync(msg =>
    {
      msg.Components = msg.Components;
    });
    await FollowupAsync(embeds: moveItems.GetEmbeds(), components: moveItems.GetComponents(), ephemeral: true).ConfigureAwait(false);
  }
}
