using Discord.WebSocket;
using TheOracle2.UserContent;
using Discord.Interactions;
using System.Text.RegularExpressions;
namespace TheOracle2.GameObjects;

/// <summary>
/// Interface for widgets that include a select menu with move references.
/// </summary>
public interface IMoveRef : IWidget
{
  public Tuple<MoveNumber, string>[] MoveReferences { get; }
  public SelectMenuBuilder MoveRefMenu();

  /// <summary>
  /// Generates menu options representing move references for an IMoveRef.
  /// </summary>
  /// <param name="moveRefParent">The IMoveRef to build a list for.</param>
  /// <param name="prefix">A prefix to add to the Value of the menu options</param>
  public static List<SelectMenuOptionBuilder> MenuOptions(IMoveRef moveRefParent, string prefix = "")
  {
    List<SelectMenuOptionBuilder> options = new();
    foreach ((MoveNumber move, string moveTrigger) in moveRefParent.MoveReferences)
    {
      int id = (int)move;
      string append = "…";
      string label = Regex.Replace(move.ToString(), "([A-Z])", " $1");
      string[] labelWords = label.Split(" ").Select(word => (word == "An" || word == "A" || word == "The") ? word.ToLowerInvariant() : word) as string[];
      label = string.Join(" ", labelWords);
      int maxChars = SelectMenuOptionBuilder.MaxDescriptionLength;
      string triggerString = moveTrigger.Length <= maxChars ? moveTrigger : moveTrigger[0..(maxChars - 1)] + append;
      // string casedMove = move.ToLowerInvariant().Replace(" ", "_");
      options.Add(new SelectMenuOptionBuilder(label: label, value: $"{prefix}{id}", description: triggerString, emote: new Emoji("📖")));
    }
    return options;
  }
  /// <summary>
  /// Builds a menu of move references from an IMoveRef.
  /// </summary>
  public static SelectMenuBuilder MenuBase(IMoveRef moveRefParent)
  {
    SelectMenuBuilder menu =
     new SelectMenuBuilder()
       .WithPlaceholder("Reference moves...")
       .WithCustomId("move-ref-menu")
       .WithOptions(MenuOptions(moveRefParent));
    return menu;
  }
}