namespace TheOracle2.GameObjects;

public abstract class ProgressTrack : IProgressTrack
{
  public ProgressTrack(ChallengeRank rank, int ticks = 0, string title = "", string description = "")
  {
    Rank = rank;
    Ticks = ticks;
    Title = title;
    Description = description;
  }
  public ChallengeRank Rank { get; set; }
  public int Ticks { get; set; }
  public string Title { get; set; }
  public string Description { get; set; }
  public int Score => (int)(Ticks / 4);
  public virtual string ProgressTypeString => "Progress Track";

  public string ProgressScoreString { get => $"{Score}/10"; }

  public virtual EmbedBuilder ToEmbed()
  {
    return new EmbedBuilder()
    .WithAuthor($"{ProgressTypeString}: {Rank}")
    .AddField($"Progress [{ProgressScoreString}]", IProgressTrack.TicksToEmoji(Ticks));
  }

}