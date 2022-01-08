namespace TheOracle2.GameObjects;

public interface IProgressTrack
{
  public ChallengeRank Rank { get; set; }
  public int Ticks { get; set; }
  public int Score => (int)(Ticks / 4);
  public static IProgressTrack FromEmbed(Embed embed)
  {
    EmbedField progressField = embed.Fields.Where(field => field.Name.StartsWith("Progress [")).FirstOrDefault();
    int ticks = EmojiToTicks(progressField.Value);

    // switch from author field - split at ":" and get the first

  }
  public string ProgressTypeString { get; }

  public string ProgressScoreString {get;}


  public static int EmojiToTicks(string emojiBar)
  {
    // might be better handled by extracting it from a customid, tbh
    emojiBar = emojiBar.Replace("\u200C", "");
    int index = 0;
    foreach (string emoji in BarEmoji)
    {
      emojiBar = emojiBar.Replace(emoji, index.ToString());
      index++;
    }
    IEnumerable<int> values = emojiBar.Split(" ").Cast<int>();
    return values.Sum();
  }

  public static string TicksToEmoji(int ticks)
  {
    int score = ticks / 4;
    int remainder = ticks % 4;
    string fill = new('4', score);
    string finalTickMark = (remainder == 0) ? string.Empty : remainder.ToString();
    fill = (fill + finalTickMark).PadRight(10, '0');
    var boxValues = fill.ToCharArray().Cast<int>();
    var emojiStrings = boxValues.Select<int, string>(box => BarEmoji[box]);
    fill = String.Join(" ", emojiStrings);
    fill += "\u200C"; //special hidden character for mobile formatting small emojis
    return fill;
  }

  public static readonly List<string> BarEmoji = new()
  {
    "<:progress0:880599822468534374>",
    "<:progress1:880599822736965702>",
    "<:progress2:880599822724390922>",
    "<:progress3:880599822736957470>",
    "<:progress4:880599822820864060>"
  };
}