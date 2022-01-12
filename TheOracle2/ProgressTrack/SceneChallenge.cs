namespace TheOracle2.GameObjects;
public class SceneChallenge : ProgressTrack, IClock
{
  public SceneChallenge(Embed embed) : base(embed)
  {
    Tuple<int, int> clockData = IClock.ParseClock(embed);
    Filled = clockData.Item1;
    Segments = clockData.Item2;
  }
  public SceneChallenge(ClockSize segments = (ClockSize)6, int filledSegments = 0, int ticks = 0, string title = "", string description = "", ChallengeRank rank = ChallengeRank.Formidable) : base(rank, ticks, title, description)
  {
    Filled = filledSegments;
    Segments = (int)segments;
  }
  public override string EmbedCategory { get; } = "Scene Challenge";
  public string FillMessage { get; set; } = "When the tension clock is filled, time is up. You must resolve the encounter by making a progress roll.";
  public int Segments { get; }
  public int Filled { get; set; }
  public bool IsFull => Filled >= Segments;
  public override EmbedBuilder ToEmbed()
  {
    return IClock.AddClockTemplate(base.ToEmbed(), Segments, Filled);
    ;
  }
  public EmbedBuilder AlertEmbed()
  {
    return IClock.AlertEmbedTemplate(Segments, Filled, FillMessage)
    .WithAuthor($"{EmbedCategory}: {Title}");
  }
  public override ButtonBuilder ResolveButton()
  {
    return base
      .ResolveButton()
      .WithLabel("Resolve challenge")
    ;
  }
  public override ButtonBuilder MarkButton()
  {
    return base
      .MarkButton()
      .WithDisabled(Score >= ITrack.TrackSize || IsFull)
    ;
  }
  public SelectMenuBuilder MakeSelectMenu()
  {
    SelectMenuBuilder menu = new SelectMenuBuilder()
    .WithCustomId("scene-challenge-menu")
    .WithPlaceholder("Manage scene challenge...")
    .WithMaxValues(1)
    .WithMinValues(0)
    ;
    if (!IsFull)
    {
      if (Score < ITrack.TrackSize) { menu = menu.AddOption(IProgressTrack.MarkOption(RankData.MarkTrack)); }
      menu = menu.AddOption(IClock.AdvanceOption());
    }
    menu = menu.AddOption(IProgressTrack.ResolveOption(Score));
    if (Ticks > 0) { menu = menu.AddOption(IProgressTrack.ClearOption(RankData.MarkTrack)); }
    if (Filled > 0) { menu = menu.AddOption(IClock.ResetOption()); }
    return menu;
  }
  public override ComponentBuilder MakeComponents()
  {
    return new ComponentBuilder()
      .WithSelectMenu(MakeSelectMenu())
      ;
  }
}