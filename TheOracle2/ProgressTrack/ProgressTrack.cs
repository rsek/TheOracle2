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

  public RankData RankInfo { get => IProgressTrack.RankInfo[Rank]; }
  public string Title { get; set; }
  public string Description { get; set; }
  public int Score => (int)(Ticks / 4);
  public virtual string EmbedCategory => "Progress Track";
  public string ProgressScoreString { get => $"{Score}/10"; }
  public virtual EmbedBuilder ToEmbed()
  {
    return IProgressTrack.ToEmbed(EmbedCategory, Rank, Title, ProgressScoreString, Ticks);
  }
  public virtual ButtonBuilder ClearButton()
  {
    return IProgressTrack
      .ClearButton(RankInfo.MarkTrack)
        .WithDisabled(Ticks == 0);
  }
  public virtual ButtonBuilder MarkButton()
  {
    return IProgressTrack
      .MarkButton(RankInfo.MarkTrack)
        .WithDisabled(Score >= 10);
  }
  public virtual ButtonBuilder ResolveButton()
  {
    return IProgressTrack
      .ResolveButton(Score);
  }
  public virtual ButtonBuilder RecommitButton()
  {
    return IProgressTrack
      .RecommitButton(Ticks, Rank);
  }
  public virtual ComponentBuilder MakeComponents()
  {
    return new ComponentBuilder()
      .WithButton(ClearButton())
      .WithButton(MarkButton())
      .WithButton(ResolveButton());
  }
}