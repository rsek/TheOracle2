namespace TheOracle2.GameObjects;

public abstract class ProgressTrack : IProgressTrack
{
  protected internal ProgressTrack(Embed embed)
  {
    Rank = IProgressTrack.ParseEmbedRank(embed);
    Ticks = IProgressTrack.ParseEmbedTicks(embed);
    Title = embed.Title;
    Description = embed.Description;
  }
  protected internal ProgressTrack(ChallengeRank rank, int ticks = 0, string title = "", string description = "")
  {
    Rank = rank;
    Ticks = ticks;
    Title = title;
    Description = description;
  }
  public ChallengeRank Rank { get; set; }
  private int _ticks;
  public int Ticks { get => _ticks; set => _ticks = Math.Max(0, Math.Min(value, ITrack.MaxTicks)); }
  public RankData RankData => IProgressTrack.RankInfo[Rank];
  public int Score => ITrack.GetScore(Ticks);
  public abstract string EmbedCategory { get; }
  public string Title { get; set; }
  public string Description { get; set; }
  public ProgressRoll Roll(Random random)
  {
    return new ProgressRoll(random, Score, Title);
  }
  public virtual EmbedBuilder ToEmbed()
  {
    return IProgressTrack.MakeEmbed(EmbedCategory, Rank, Title, Ticks);
  }
  public virtual ButtonBuilder ClearButton()
  {
    return IProgressTrack
      .ClearButton(RankData.MarkTrack)
        .WithDisabled(Ticks == 0);
  }
  public virtual SelectMenuOptionBuilder ClearOption()
  {
    return IProgressTrack.ClearOption(RankData.MarkTrack);
  }
  public virtual ButtonBuilder MarkButton()
  {
    return IProgressTrack
      .MarkButton(RankData.MarkTrack)
        .WithDisabled(Score >= ITrack.TrackSize);
  }
  public virtual SelectMenuOptionBuilder MarkOption()
  {
    return IProgressTrack.MarkOption(RankData.MarkTrack);
  }
  public virtual ButtonBuilder ResolveButton()
  {
    return IProgressTrack
      .ResolveButton(Score);
  }
  public virtual SelectMenuOptionBuilder ResolveOption()
  {
    return IProgressTrack.ResolveOption(Score);
  }
  public virtual ButtonBuilder RecommitButton()
  {
    return IProgressTrack
      .RecommitButton(Ticks, Rank);
  }

  public virtual SelectMenuOptionBuilder RecommitOption()
  {
    return IProgressTrack.RecommitOption(Ticks, Rank);
  }
  public virtual ComponentBuilder MakeComponents()
  {
    return new ComponentBuilder()
      .WithButton(ClearButton())
      .WithButton(MarkButton())
      .WithButton(ResolveButton());
  }
}