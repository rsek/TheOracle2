using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TheOracle2.DataClassesNext;
public class RollableTableRow
{
    [JsonIgnore]
    public string ParentPath { get; set; }
    [JsonIgnore]
    private string DisplayPath => ParentPath.Replace(" / Table", "");
    public EmbedFieldBuilder ToField(int? rollValue = null)
    {
        var name = DisplayPath;
        var value = ToResultString();
        if (rollValue != null)
        {
            name += $" [{rollValue}]";
        }
        return new EmbedFieldBuilder().WithName(name).WithValue(value);
    }
    public bool RollIsInRange(int roll)
    {
        if (roll >= Floor && roll <= Ceiling) { return true; }
        return false;
    }
    public override string ToString()
    {
        return $"`{ToRangeString().PadLeft(7)}` {ToResultString()}";
    }
    public string ToResultString()
    {
        if (Summary != null)
        {
            return $"{Result} ({Summary})";
        }
        return Result;
    }

    public string ToRangeString()
    {
        if (Floor == Ceiling) { return Floor.ToString(); }
        return $"{Floor}-{Ceiling}";
    }
    public int Floor { get; set; }
    public int Ceiling { get; set; }

    public string Result { get; set; }

    public string Summary { get; set; }

    [JsonProperty("Game objects")]
    public virtual IList<GameObject> GameObjects { get; set; }

    [JsonProperty("Multiple rolls")]
    public virtual MultipleRolls MultipleRolls { get; set; }

    [JsonProperty("Oracle rolls")]
    public IList<string> OracleRolls { get; set; }

    // TODO: function to get oracle rolls

    public virtual Suggestions Suggestions { get; set; }


    [DefaultValue(null)]
    public int Amount { get; set; }

    [JsonProperty("Part of speech")]
    public IList<string> PartOfSpeech { get; set; }

    public IList<string> Assets { get; set; }

    public virtual RollableTable Table { get; set; }

    // [JsonProperty("Add template")]
    // public virtual AddTemplate AddTemplate { get; set; }

    public string Image { get; set; }
}
