using Discord;
namespace TheOracle2.GameObjects;

/// <summary>
/// The three legacy tracks that grant the player XP. Unlike other tracks, they have no Challenge Rank of their own. They can be advanced indefinitely, though their Score remains capped at 10 and they earn XP at half speed.
/// </summary>
public class LegacyTrack : ITrack
{
  public LegacyTrack(EmbedField embedField)
  {
    Tuple<Legacy, int> data = ParseLegacy(embedField);
    Legacy = data.Item1;
    Ticks = data.Item2;
  }
  public LegacyTrack(Legacy legacy, int ticks = 0)
  {
    Legacy = legacy;
    Ticks = ticks;
  }
  public Legacy Legacy { get; }
  public string Title => $"{Legacy} Legacy";
  public int Ticks { get; set; }
  public int Score => ITrack.GetScore(Ticks);

  /// <summary>
  /// The number of ticks to have XP calculated at half rate.
  /// </summary>

  private const int XpPerBox = 2;

  private const int XpPerBoxPast10 = 1;
  private int XpFullRate => Score * XpPerBox;
  private int RemainderTicks => Math.Max(0, Ticks - ITrack.MaxTicks);
  private int RemainderBoxes => RemainderTicks / ITrack.BoxSize;
  private int XpHalfRate => RemainderBoxes * XpPerBoxPast10;
  private string PlusValue => Ticks switch
  {
    <= ITrack.MaxTicks => "",
    <= ITrack.MaxTicks * 2 => $"+{ITrack.TrackSize}",
    > ITrack.MaxTicks * 2 => $"+{ITrack.TrackSize} ×{(int)(Ticks / ITrack.MaxTicks)}"
  };

  public int Xp => XpFullRate + XpHalfRate;
  public ProgressRoll Roll(Random random)
  {
    return new ProgressRoll(random, Score, Title);
  }
  private string TrackTitle => Ticks switch
  {
    <= ITrack.MaxTicks => $"{Title} [{Score}/{ITrack.TrackSize}]",
    > ITrack.MaxTicks => $"{Title} [{(Ticks % ITrack.MaxTicks) / ITrack.BoxSize}/{ITrack.TrackSize}] {PlusValue}"
  };
  public string EmojiTrack => Ticks switch
  {
    <= ITrack.MaxTicks => ITrack.TicksToEmojiBar(Ticks),
    > ITrack.MaxTicks => ITrack.TicksToEmojiBar(Ticks % ITrack.MaxTicks)
  };
  public EmbedFieldBuilder ToEmbedField()
  {
    return new EmbedFieldBuilder()
            .WithName(TrackTitle)
            .WithValue(EmojiTrack);
  }
  public static int RewardTicks(ChallengeRank rewardRank)
  {
    return IProgressTrack.RankInfo[rewardRank].MarkLegacy;
  }
  public void MarkReward(ChallengeRank rewardRank)
  {
    Ticks += RewardTicks(rewardRank);
  }
  public void MarkReward(int addTicks)
  {
    Ticks += addTicks;
  }

  public SelectMenuOptionBuilder MarkRewardOption()
  {
    return new SelectMenuOptionBuilder()
    .WithLabel($"Mark {Title} ()")
    .WithValue($"legacy-mark:{Legacy}")
    .WithEmote(Emoji[Legacy])
    ;
  }
  public SelectMenuOptionBuilder MarkRewardOption(ChallengeRank rank)
  {
    var ticks = RewardTicks(rank);
    var emojiString = string.Concat(ITrack.TicksToEmoji(ticks).Select(emoji => emoji.ToString()));
    return new SelectMenuOptionBuilder()
    .WithLabel($"{Legacy}: {rank}")
    .WithDescription($"Mark {ITrack.TickString(ticks)}")
    .WithValue($"legacy-mark:{Legacy},{ticks}")
    .WithEmote(Emoji[Legacy])
    ;
  }
  public SelectMenuOptionBuilder MarkRewardOption(int ticks)
  {
    return new SelectMenuOptionBuilder()
    .WithLabel($"Mark {Title}")
    .WithValue($"legacy-mark:{Legacy},{ticks}")
    .WithEmote(Emoji[Legacy])
    ;
  }
  public SelectMenuOptionBuilder ClearOption(int ticks)
  {
    return new SelectMenuOptionBuilder()
    .WithLabel($"Clear {Title}: {ITrack.TickString(ticks)}")
    .WithValue($"legacy-clear:{Legacy},{ticks}")
    .WithEmote(new Emoji("❌"))
    ;
  }

  // TODO: clear tick option/button. should probably only present - different number of buttons = less likely to be blindly clicked thru. should also be the DANGER colour.
  public static ButtonBuilder MarkRewardButton(Legacy legacy, ChallengeRank rank)
  {
    return new ButtonBuilder()
      .WithCustomId($"legacy-mark:{legacy},{RewardTicks(rank)}")
      .WithLabel($"{rank} ({IProgressTrack.TickString(IProgressTrack.RankInfo[rank].MarkLegacy)})")
      .WithStyle(ButtonStyle.Primary)
    ;
  }
  public static ComponentBuilder MarkRewardButtons(Legacy legacy)
  {
    ComponentBuilder components = new();
    foreach (ChallengeRank rank in Enum.GetValues(typeof(ChallengeRank)))
    {
      components.WithButton(MarkRewardButton(legacy, rank));
    }
    return components;
  }
  public static Tuple<Legacy, int> ParseLegacy(EmbedField embedField)
  {
    int ticks = ITrack.EmojiStringToTicks(embedField.Value);
    int extraBoxes = 0;
    string legacyString = "";

    if (embedField.Name.Contains("+") && embedField.Name.Contains("×"))
    {
      int multiplier = int.Parse(embedField.Name.Split("×")[1]);
      extraBoxes = ITrack.TrackSize * multiplier;
    }
    if (embedField.Name.Contains("+") && !embedField.Name.Contains("×"))
    {
      extraBoxes = ITrack.TrackSize;
    }
    ticks += extraBoxes * ITrack.BoxSize;

    foreach (Legacy legacy in Enum.GetValues(typeof(Legacy)))
    {
      if (embedField.Name.StartsWith(legacy.ToString()))
      {
        legacyString = legacy.ToString();
      }
    }
    return new Tuple<Legacy, int>(Enum.Parse<Legacy>(legacyString), ticks);
  }
  public static readonly Dictionary<Legacy, Emoji> Emoji = new()
  {
    { Legacy.Quests, new Emoji("✴️") },
    { Legacy.Bonds, new Emoji("🪢") },
    { Legacy.Discoveries, new Emoji("🧭") }
  };
}
public enum Legacy { Quests, Bonds, Discoveries }