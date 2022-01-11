using TheOracle2;
namespace TheOracle2.GameObjects;

/// <summary>
/// Interface inherited by all ranked and unranked tracks.
/// </summary>

public interface ITrack
{
  public int Ticks { get; set; }
  public int Score { get; }
  public ProgressRoll Roll(Random random);
  protected static int GetScore(int ticks)
  {
    int rawScore = (int)(ticks / 4);
    return Math.Max(0, Math.Min(rawScore, 10));
  }
  protected static int EmojiToTicks(string emojiBar)
  {
    emojiBar = emojiBar.Replace("\u200C", "");
    int index = 0;
    foreach (string emoji in BarEmoji)
    {
      emojiBar = emojiBar.Replace(emoji, index.ToString());
      index++;
    }
    IEnumerable<int> values = emojiBar.Split(" ").Select(item => int.Parse(item));
    return values.Sum();
  }
  protected static string TickString(int ticks)
  {
    if (ticks >= 3)
    {
      int boxes = ticks / 4;
      int remainder = ticks % 4;
      string result = boxes.ToString() + " " + (boxes > 1 ? "boxes" : "box");
      if (remainder > 0)
      {
        result += $" and {remainder} ticks";
      }
      return result;
    }
    return $"{ticks} ticks";
  }
  protected static string TicksToEmoji(int ticks)
  {
    int score = ticks / 4;
    int remainder = ticks % 4;
    string fill = new('4', score);
    string finalTickMark = (remainder == 0) ? string.Empty : remainder.ToString();
    fill = (fill + finalTickMark).PadRight(10, '0');
    var boxValues = fill.ToCharArray().Select(item => item.ToString());
    var emojiStrings = boxValues.Select(box => BarEmoji[int.Parse(box)]);
    fill = String.Join(" ", emojiStrings);
    fill += "\u200C"; //special hidden character for mobile formatting small emojis
    return fill;
  }

  protected static readonly List<string> BarEmoji = new()
  {
    "<:progress0:880599822468534374>",
    "<:progress1:880599822736965702>",
    "<:progress2:880599822724390922>",
    "<:progress3:880599822736957470>",
    "<:progress4:880599822820864060>"
  };

  protected static EmbedFieldBuilder ToProgressBarField(int ticks = 0, string name = "Track")
  {
    int score = (int)(ticks / 4);
    return new EmbedFieldBuilder()
      .WithName($"{name} [{score}/10]")
      .WithValue(TicksToEmoji(ticks));
  }
}