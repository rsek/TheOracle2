using Discord;
namespace TheOracle2.GameObjects;

/// <summary>
/// The three legacy tracks that grant the player XP. Unlike other tracks, they have no Challenge Rank of their own. They can be advanced indefinitely, though their Score remains capped at 10 and they earn XP at half speed.
/// </summary>
public class LegacyTrack : ITrack
{
  public LegacyTrack(Legacy legacy, int ticks = 0)
  {
    Legacy = legacy;
    Ticks = ticks;
  }
  public Legacy Legacy { get; }
  public string Title => $"{Legacy} Legacy";
  public int Ticks { get; set; }
  public int Score => ITrack.GetScore(Ticks);
  public int Xp
  {
    get
    {
      // The first 10 boxes of progress grant 2 xp apiece.
      var fullRateXp = Score * 2;
      return (Ticks) switch
      {
        <= 40 => fullRateXp,
        // Past the 10th box, XP is earned at half speed: 1 xp per box.
        > 40 => fullRateXp + (int)((Ticks - 40) / 4),
      };
    }
  }
  public ProgressRoll Roll(Random random)
  {
    return new ProgressRoll(random, Score, Title);
  }
  public EmbedFieldBuilder ToEmbedField()
  {
    return Ticks switch
    {
      >= 40 => new EmbedFieldBuilder()
        .WithName($"{Title} [{Score}/10]")
        .WithValue(ITrack.TicksToEmoji(Ticks)),
      < 40 => new EmbedFieldBuilder()
        .WithName($"{Title} [{Score}/10] +{(int)(Ticks / 4) * 10}")
        .WithValue(ITrack.TicksToEmoji(Ticks % 40))
    };
  }
  public void MarkReward(ChallengeRank rewardRank)
  {
    Ticks += IProgressTrack.RankInfo[rewardRank].MarkLegacy;
  }

  public SelectMenuOptionBuilder MarkRewardOption()
  {
    return new SelectMenuOptionBuilder()
    .WithLabel($"Mark {Title}")
    .WithValue($"legacy-mark:{Legacy}")
    .WithEmote(Emoji[Legacy])
    ;
  }

  // TODO: erase tick option/button. should probably only present - different number of buttons = less likely to be blindly clicked thru. should also be the DANGER colour.
  public ButtonBuilder MarkRewardButton(ChallengeRank rewardRank)
  {
    return new ButtonBuilder()
      .WithLabel($"{rewardRank} ({IProgressTrack.TickString(IProgressTrack.RankInfo[rewardRank].MarkLegacy)})")
      .WithStyle(ButtonStyle.Primary)
      .WithCustomId($"legacy-mark:{Legacy},{rewardRank}")
    ;
  }
  public static readonly Dictionary<Legacy, Emoji> Emoji = new()
  {
    { Legacy.Quests, new Emoji("✴️") },
    { Legacy.Bonds, new Emoji("🪢") },
    { Legacy.Discoveries, new Emoji("🧭") }
  };
}
public enum Legacy { Quests, Bonds, Discoveries }