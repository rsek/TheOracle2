using TheOracle2;
namespace TheOracle2.GameObjects;

public interface IProgressTrack
{
  public ChallengeRank Rank { get; set; }
  public string Title { get; set; }
  public string Description { get; set; }
  public int Ticks { get; set; }
  public int Score => (int)(Ticks / 4);
  public ComponentBuilder MakeComponents();
  public string EmbedCategory { get; }
  public string ProgressScoreString { get; }
  public static EmbedBuilder ToEmbed(string embedCategory, ChallengeRank rank, string title, string progressScoreString, int ticks)
  {
    return new EmbedBuilder()
    .WithAuthor($"{embedCategory}: {rank}")
    .WithTitle(title)
    .WithFields(ToEmbedField(progressScoreString, ticks));
  }
  public static EmbedFieldBuilder ToEmbedField(string progressScoreString, int ticks)
  {
    return new EmbedFieldBuilder()
      .WithName($"Progress [{progressScoreString}]")
      .WithValue(TicksToEmoji(ticks))
      .WithIsInline(true);
  }
  public static IProgressTrack FromEmbed(Embed embed)
  {
    EmbedField progressField = embed.Fields.FirstOrDefault(field => field.Name.StartsWith("Progress ["));
    int ticks = EmojiToTicks(progressField.Value);
    string[] toParse = embed.Author.ToString().Split(": ");
    var progressType = toParse[0];
    switch (progressType)
    {
      // case "Legacy Track":
      //   if (Enum.TryParse<LegacyTrack.LegacyType>(toParse[1], out LegacyTrack.LegacyType legacyType))
      //     return new LegacyTrack(legacyType);
      //   break;
      case "Scene Challenge":
        return new SceneChallenge(embed);
      default:
        break;
    }

    // if (Enum.TryParse<ChallengeRank>(toParse[1], out ChallengeRank rank))
    // {
    //   switch (progressType)
    //   {
    //     case "Combat Track":
    //       break;
    //     case "Expedition Track":
    //       break;
    //     case "Vow Track":
    //       break;
    //     case "Connection Track":
    //       break;
    //     default:
    //       break;
    //   }
    // }
    throw new Exception("Unable to parse embed into progress track.");
  }
  public static int EmojiToTicks(string emojiBar)
  {
    // might be better handled by extracting it from a customid, tbh
    emojiBar = emojiBar.Replace("\u200C", "");
    int index = 0;
    foreach (string emoji in BarEmoji)
    {
      emojiBar = emojiBar.Replace(emoji, index.ToString());
      index++;
    }
    IEnumerable<int> values = emojiBar.Split(" ").Cast<int>();
    return values.Sum();
  }
  public static string TicksToEmoji(int ticks)
  {
    int score = ticks / 4;
    int remainder = ticks % 4;
    string fill = new('4', score);
    string finalTickMark = (remainder == 0) ? string.Empty : remainder.ToString();
    fill = (fill + finalTickMark).PadRight(10, '0');
    var boxValues = fill.ToCharArray().Cast<int>();
    var emojiStrings = boxValues.Select<int, string>(box => BarEmoji[box]);
    fill = String.Join(" ", emojiStrings);
    fill += "\u200C"; //special hidden character for mobile formatting small emojis
    return fill;
  }
  public static ButtonBuilder ResolveButton(int score)
  {
    return new ButtonBuilder()
      .WithLabel("Roll progress")
      .WithStyle(ButtonStyle.Success)
      .WithEmote(new Emoji("🎲"))
      .WithCustomId($"progress-resolve:{score}")
    ;
  }
  public static ButtonBuilder MarkButton(int addTicks)
  {
    return new ButtonBuilder()
      .WithLabel("Mark progress")
      .WithStyle(ButtonStyle.Primary)
      .WithEmote(new Emoji(BarEmoji[4]))
      .WithCustomId($"progress-mark:{addTicks}")
    ;
  }
  public static ButtonBuilder ClearButton(int subtractTicks)
  {
    return new ButtonBuilder()
      .WithLabel("Clear progress")
      .WithStyle(ButtonStyle.Danger)
      .WithEmote(new Emoji(BarEmoji[0]))
      .WithCustomId($"progress-clear:{subtractTicks}")
    ;
  }
  public static ButtonBuilder RecommitButton(int currentTicks, ChallengeRank currentRank)
  {
    return new ButtonBuilder()
      .WithLabel("Recommit")
      .WithStyle(ButtonStyle.Secondary)
      .WithEmote(new Emoji("🔄"))
      .WithCustomId($"progress-recommit:{currentTicks},{(int)currentRank}")
    ;
  }
  public static readonly Rank RankInfo = new();
  public static readonly List<string> BarEmoji = new()
  {
    "<:progress0:880599822468534374>",
    "<:progress1:880599822736965702>",
    "<:progress2:880599822724390922>",
    "<:progress3:880599822736957470>",
    "<:progress4:880599822820864060>"
  };
  public class Recommit
  {
    public Recommit(Random random, Embed progressEmbed)
    {
      OldTrack = FromEmbed(progressEmbed);
      NewTrack = FromEmbed(progressEmbed);
      Random = random;
      ChallengeDie1 = new Die(Random, 10);
      ChallengeDie2 = new Die(Random, 10);
      int ticksToClear = BoxesToClear * 4;
      NewTrack.Ticks = Math.Min(0, NewTrack.Ticks - ticksToClear);
      NewTrack.Rank = (ChallengeRank)Math.Min(
        ((int)OldTrack.Rank) + 1, 5);
    }
    public IProgressTrack OldTrack { get; }
    public IProgressTrack NewTrack { get; set; }
    public Random Random { get; }
    public Die ChallengeDie1 { get; }
    public Die ChallengeDie2 { get; }

    public int BoxesToClear => Math.Min(ChallengeDie1.Value, ChallengeDie2.Value);
    public EmbedBuilder ToAlertEmbed(string moveName)
    {
      string rankString = OldTrack.Rank == NewTrack.Rank ? NewTrack.Rank.ToString() : $"~~{OldTrack.Rank}~~ {NewTrack.Rank}";
      return new EmbedBuilder()
        .WithAuthor(moveName)
        .WithTitle("Recommit")
        .WithDescription($"You recommit to *{OldTrack.Title}*.")
        .AddField("Challenge Dice", $"{ChallengeDie1.Value}, {ChallengeDie2.Value}", true)
        .AddField("Boxes Cleared", $"{BoxesToClear}", true)
        .AddField($"Progress ~~[{OldTrack.Score}/10]~~ {NewTrack.Score}/10]", TicksToEmoji(NewTrack.Ticks))
        .AddField("Rank", rankString, true)
        ;
    }
  }
}