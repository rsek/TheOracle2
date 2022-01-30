using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace TheOracle2.DataClasses;
public class OracleTableRow
{
    /// <summary>
    /// A string representation of the path to this object. This is generated when building the DBset, not pulled from the JSON.
    /// </summary>
    [JsonIgnore]
    [Key]
    public string Path { get; set; }
    /// <summary>
    /// The nearest Oracle ancestor of this object.
    /// </summary>
    [JsonIgnore]
    public virtual Oracle Metadata { get; set; }
    /// <summary>
    /// The nearest OracleCategory ancestor of this object.
    /// </summary>
    [JsonIgnore]
    public virtual OracleCategory Category { get; set; }
    /// <summary>
    /// The nearest RollableTable ancestor of this object.
    /// </summary>
    [JsonIgnore]
    public virtual OracleTable RowOf { get; set; }
    public virtual bool RollIsInRange(int roll)
    {
        if (roll >= Floor && roll <= Ceiling) { return true; }
        return false;
    }
    public override string ToString()
    {
        if (Summary != null)
        {
            return $"{Result} ({Summary})";
        }
        return Result;
    }
    public int Floor { get; set; }
    public int Ceiling { get; set; }
    // public int Weight => (Ceiling - Floor) + 1;
    public string Result { get; set; }
    public string Summary { get; set; }

    [JsonProperty("Game objects")]
    public virtual IList<GameObject> GameObjects { get; set; }

    [JsonProperty("Multiple rolls")]
    public virtual MultipleRolls MultipleRolls { get; set; }

    [JsonProperty("Oracle rolls")]
    public virtual IList<string> OracleRolls { get; set; }

    // TODO: function to get oracle rolls

    public virtual Suggestions Suggestions { get; set; }


    [DefaultValue(null)]
    public int Amount { get; set; }

    [JsonProperty("Part of speech")]
    public virtual IList<string> PartOfSpeech { get; set; }

    public virtual IList<string> Assets { get; set; }

    public virtual OracleTable Table { get; set; }

    // [JsonProperty("Add template")]
    // public virtual AddTemplate AddTemplate { get; set; }

    public string Image { get; set; }
    internal virtual void BuildAncestry(OracleTable parent)
    {
        RowOf = parent;
        Path = parent.Path + $" / {parent.IndexOf(this)}";
        Category = parent.Category;
        Metadata = parent.Metadata;
        if (Table?.Count > 0) { Table?.BuildAncestry(this); }
    }
}
