namespace TheOracle2.GameObjects;

public abstract class Clock : IClock
{
  protected Clock(Embed embed)
  {
    var values = IClock.ParseClock(embed);
    Title = embed.Title;
    Description = embed.Description;
    Filled = values.Item1;
    Segments = values.Item2;
  }
  protected Clock(ClockSize segments = (ClockSize)6, int filledSegments = 0, string title = "", string description = "")
  {
    if (filledSegments < 0 || filledSegments > ((int)segments))
    {
      throw new ArgumentOutOfRangeException(nameof(filledSegments), "filledSegments can't exceed segments");
    }
    Title = title;
    Segments = (int)segments;
    Filled = filledSegments;
    Description = description;
  }
  public int Segments { get; }
  public int Filled { get; set; }
  public string Title { get; set; }
  public string Description { get; set; }
  public abstract string EmbedCategory { get; }
  public bool IsFull => Filled >= Segments;

  public virtual EmbedBuilder ToEmbed()
  {
    var embed = new EmbedBuilder()
      .WithAuthor(EmbedCategory)
      .WithTitle(Title)
      .WithDescription(Description)
    ;
    return IClock.AddClockTemplate(embed, Segments, Filled);
  }
  public EmbedBuilder AlertEmbed()
  {
    return IClock.AlertEmbedTemplate(Segments, Filled, FillMessage);
  }
  public virtual string FillMessage { get; set; } = "";
  public virtual ComponentBuilder MakeComponents()
  {
    return new ComponentBuilder()
    .WithButton(IClock.AdvanceButton().WithDisabled(IsFull))
    .WithButton(IClock.ResetButton().WithDisabled(Filled == 0));
  }
}