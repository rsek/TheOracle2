namespace TheOracle2.GameObjects;
public class SceneChallenge : Clock, IProgressTrack
{
  public SceneChallenge(Embed embed) : base(embedField: embed.Fields.FirstOrDefault(field => field.Name == "Clock"))
  {
    Title = embed.Title;
    EmbedField progressField = embed.Fields.FirstOrDefault(field => field.Name.StartsWith("Progress [", StringComparison.InvariantCulture));
    Ticks = IProgressTrack.EmojiToTicks(progressField.Value);
  }
  public SceneChallenge(ClockSize segments = (ClockSize)6, int filledSegments = 0, int ticks = 0, string title = "") : base(segments, filledSegments, title)
  {
    Ticks = ticks;
  }
  public ChallengeRank Rank { get; set; } = ChallengeRank.Formidable;

  public RankData RankInfo { get => IProgressTrack.RankInfo[Rank]; }

  public int Ticks { get; set; }
  public int Score => (int)(Ticks / 4);
  public override string EmbedCategory { get; } = "Scene Challenge";
  public string ProgressScoreString { get => $"{Score}/10"; }
  public override EmbedBuilder ToEmbed()
  {
    return base.ToEmbed()
    .AddField(IProgressTrack.ToEmbedField(ProgressScoreString, Ticks))
    .AddField(ToEmbedField())
    .WithDescription("")
    ;
  }
  public virtual ButtonBuilder ClearButton()
  {
    return IProgressTrack
      .ClearButton(RankInfo.MarkTrack)
        .WithDisabled(Ticks == 0);
  }
  public ButtonBuilder ResolveButton()
  {
    return IProgressTrack
      .ResolveButton(Score)
      .WithLabel("Resolve challenge")
    ;
  }
  public ButtonBuilder MarkButton()
  {
    return IProgressTrack
      .MarkButton(RankInfo.MarkTrack)
      .WithDisabled(Score >= 10 || IsFull)
    ;
  }

  public override ComponentBuilder MakeComponents()
  {
    ComponentBuilder components = new ComponentBuilder()
      .WithButton(ClearButton())
      .WithButton(MarkButton())
      .WithButton(ResolveButton())
      ;
    foreach (ActionRowBuilder row in
        base.MakeComponents().ActionRows)
    {
      foreach (ButtonBuilder button in row.Components)
      {
        components = components.WithButton(button);
      }
    }
    return components;
  }
}