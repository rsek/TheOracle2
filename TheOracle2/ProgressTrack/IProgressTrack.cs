using TheOracle2;
namespace TheOracle2.GameObjects;

/// <summary>
/// Interface for all ranked progress tracks.
/// </summary>
public interface IProgressTrack : ITrack
{
  public ChallengeRank Rank { get; set; }
  public string Title { get; set; }
  public string Description { get; set; }
  public ComponentBuilder MakeComponents();
  public string EmbedCategory { get; }
  public EmbedBuilder ToEmbed();
  public static EmbedBuilder MakeEmbed(string embedCategory, ChallengeRank rank, string title, int ticks, string description = "")
  {
    return new EmbedBuilder()
    .WithAuthor(embedCategory)
    .WithTitle(title)
    .WithDescription(description)
    .WithFields(
      ToRankField(rank),
      ToProgressBarField(ticks)
        .WithIsInline(true)
      );
  }
  public static EmbedFieldBuilder ToRankField(ChallengeRank rank)
  {
    return new EmbedFieldBuilder()
        .WithName("Rank")
        .WithValue(rank.ToString())
        .WithIsInline(true);
  }
  public static ChallengeRank ParseEmbedRank(Embed embed)
  {
    EmbedField rankField = embed.Fields.FirstOrDefault(field => field.Name == "Rank");
    return Enum.Parse<ChallengeRank>(rankField.Value);
  }
  public static int ParseEmbedTicks(Embed embed)
  {
    EmbedField progressField = embed.Fields.FirstOrDefault(field => field.Name.StartsWith("Track ["));
    return EmojiToTicks(progressField.Value);
  }

  public static IProgressTrack FromEmbed(Embed embed)
  {
    switch (embed.Author.ToString())
    {
      case "Progress Track":
        return new GenericTrack(embed);
      case "Scene Challenge":
        return new SceneChallenge(embed);
      //     case "Combat Track":
      //       break;
      //     case "Expedition Track":
      //       break;
      //     case "Vow Track":
      //       break;
      //     case "Connection Track":
      //       break;
      default:
        break;
    }
    throw new Exception("Unable to parse embed into progress track.");
  }


  public static ButtonBuilder ResolveButton(int score)
  {
    return new ButtonBuilder()
      .WithLabel("Roll progress")
      .WithStyle(ButtonStyle.Success)
      .WithCustomId($"progress-roll:{score}")
      .WithEmote(new Emoji("🎲"))
    ;
  }

  public static SelectMenuOptionBuilder ResolveOption(int score)
  {
    return new SelectMenuOptionBuilder()
      .WithLabel("Roll progress")
      .WithValue($"progress-roll:{score}")
      .WithEmote(new Emoji("🎲"))
      ;
  }
  public static ButtonBuilder MarkButton(int addTicks)
  {
    return new ButtonBuilder()
      .WithLabel("Mark progress")
      .WithStyle(ButtonStyle.Primary)
      .WithEmote(Emote.Parse(BarEmoji[Math.Min(4, addTicks)]))
      .WithCustomId($"progress-mark:{addTicks}")
    ;
  }
  public static SelectMenuOptionBuilder MarkOption(int addTicks)
  {
    return new SelectMenuOptionBuilder()
      .WithLabel($"Mark {TickString(addTicks)} progress")
      .WithEmote(Emote.Parse(BarEmoji[Math.Min(4, addTicks)]))
      .WithValue($"progress-mark:{addTicks}")
      ;
  }
  public static ButtonBuilder ClearButton(int subtractTicks)
  {
    return new ButtonBuilder()
      .WithLabel("Clear progress")
      .WithStyle(ButtonStyle.Danger)
      .WithCustomId($"progress-clear:{subtractTicks}")
    .WithEmote(Emote.Parse(BarEmoji[0]))
    ;
  }
  public static SelectMenuOptionBuilder ClearOption(int subtractTicks)
  {
    return new SelectMenuOptionBuilder()
      .WithLabel($"Clear {TickString(subtractTicks)} progress")
      .WithEmote(Emote.Parse(BarEmoji[0]))
      .WithValue($"progress-clear:{subtractTicks}")
      ;
  }
  public static ButtonBuilder RecommitButton(int currentTicks, ChallengeRank currentRank)
  {
    return new ButtonBuilder()
      .WithLabel("Recommit")
      .WithStyle(ButtonStyle.Secondary)
      .WithCustomId($"progress-recommit:{currentTicks},{currentRank}")
      .WithEmote(new Emoji("🔄"))
    ;
  }
  public static SelectMenuOptionBuilder RecommitOption(int currentTicks, ChallengeRank currentRank)
  {
    return new SelectMenuOptionBuilder()
      .WithLabel("Recommit")
      .WithValue($"progress-recommit:{currentTicks},{currentRank}")
      .WithEmote(new Emoji("🔄"))
    ;
  }
  public static readonly Dictionary<ChallengeRank, RankData> RankInfo = new()
  {
    {
      ChallengeRank.Troublesome,
      new RankData(rank: ChallengeRank.Troublesome, markTrack: 12, markLegacy: 1, suffer: 1)
    },
    {
      ChallengeRank.Dangerous,
      new RankData(rank: ChallengeRank.Dangerous, markTrack: 8, markLegacy: 2, suffer: 2)
    },
    {
      ChallengeRank.Formidable,
      new RankData(rank: ChallengeRank.Formidable, markTrack: 4, markLegacy: 4, suffer: 2)
    },
    {
      ChallengeRank.Extreme,
      new RankData(rank: ChallengeRank.Extreme, markTrack: 2, markLegacy: 8, suffer: 3)
    },
    {
      ChallengeRank.Epic,
      new RankData(rank: ChallengeRank.Epic, markTrack: 1, markLegacy: 12, suffer: 3)
    }
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
    public EmbedBuilder ToAlertEmbed(string moveName = "placeholder")
    {
      string rankString = OldTrack.Rank == NewTrack.Rank ? NewTrack.Rank.ToString() : $"~~{OldTrack.Rank}~~ {NewTrack.Rank}";
      return new EmbedBuilder()
        .WithAuthor(moveName)
        .WithTitle("Recommit")
        .WithDescription($"You recommit to *{OldTrack.Title}*.")
        .AddField("Challenge Dice", $"{ChallengeDie1.Value}, {ChallengeDie2.Value}", true)
        .AddField("Progress Boxes Cleared", $"{BoxesToClear}", true)
        .AddField($"Track ~~[{OldTrack.Score}/10]~~ {NewTrack.Score}/10]", TicksToEmoji(NewTrack.Ticks))
        .AddField("Rank", rankString, true)
        ;
    }
  }
}