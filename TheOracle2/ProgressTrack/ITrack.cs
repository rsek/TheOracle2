using TheOracle2;
namespace TheOracle2.GameObjects;

/// <summary>
/// Interface inherited by all ranked and unranked tracks.
/// </summary>
public interface ITrack
{
  public const int BoxSize = 4;
  public const int TrackSize = 10;
  public const int MaxTicks = BoxSize * TrackSize;
  public int Ticks { get; set; }
  public int Score { get; }
  public ProgressRoll Roll(Random random);
  public static int GetScore(int ticks)
  {
    int rawScore = ticks / BoxSize;
    return Math.Max(0, Math.Min(rawScore, TrackSize));
  }
  public static string TickString(int ticks)
  {

    string tickAutoPlural = "tick";
    if (ticks >= 3)
    {
      int boxes = ticks / BoxSize;
      int remainder = ticks % BoxSize;
      string result = boxes.ToString() + " " + (boxes > 1 ? "boxes" : "box");
      if (remainder > 0)
      {
        if (remainder > 1) { tickAutoPlural += "s"; }
        result += $" and {remainder} {tickAutoPlural}";
      }
      return result;
    }
    if (ticks > 1) { tickAutoPlural += "s"; }
    return $"{ticks} {tickAutoPlural}";
  }
  protected static int EmojiStringToTicks(string emojiString)
  {
    emojiString = emojiString.Replace("\u200C", "");
    IEnumerable<int> integers = emojiString.Split(" ").Select(item => BarEmoji
      .IndexOf(
        Emote.Parse(item)
      ));
    return integers.Sum();
  }
  public static List<Emote> TicksToEmoji(int ticks)
  {
    List<Emote> emojis = new();
    var counter = ticks;
    while (counter > 0)
    {
      int increment = Math.Min(BoxSize, counter);
      emojis.Add(BarEmoji[increment]);
      counter -= increment;
    }
    return emojis;
  }
  public static string TicksToEmojiBar(int ticks)
  {
    List<Emote> emoji = TicksToEmoji(ticks);
    while (emoji.Count < TrackSize)
    {
      emoji.Add(BarEmoji[0]);
    }
    string result = string.Join(" ", emoji);
    result += "\u200C"; //special hidden character for mobile formatting small emojis
    return result;
  }

  protected static readonly List<Emote> BarEmoji = new()
  {
    Emote.Parse("<:progress0:880599822468534374>"),
    Emote.Parse("<:progress1:880599822736965702>"),
    Emote.Parse("<:progress2:880599822724390922>"),
    Emote.Parse("<:progress3:880599822736957470>"),
    Emote.Parse("<:progress4:880599822820864060>")
  };

  protected static EmbedFieldBuilder ToProgressBarField(int ticks = 0, string name = "Track")
  {
    int score = ticks / BoxSize;
    return new EmbedFieldBuilder()
      .WithName($"{name} [{score}/{TrackSize}]")
      .WithValue(TicksToEmojiBar(ticks));
  }
}