using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Microsoft.EntityFrameworkCore;

namespace TheOracle2.DataClasses;

[Index(nameof(TableId), nameof(Floor), nameof(Ceiling), IsUnique = true, Name = "Index_TableRow")]
public class OracleTableRow
{
    // [JsonIgnore]
    // [Key]
    public string Id { get; set; }
    /// <summary>
    /// A string representation of the path to this object. This is generated when building the DBset, not pulled from the JSON.
    /// </summary>
    [JsonIgnore]
    public string Path { get; set; }
    // [JsonIgnore]
    // public int Id { get; set; }
    /// <summary>
    /// The nearest Oracle ancestor of this object.
    /// </summary>
    [JsonIgnore]
    public virtual OracleInfo OracleInfo { get; set; }
    [JsonIgnore]
    public string OracleInfoId { get; set; }
    /// <summary>
    /// The nearest OracleCategory ancestor of this object.
    /// </summary>
    [JsonIgnore]
    public virtual OracleCategory Category { get; set; }
    [JsonIgnore]
    public string CategoryId { get; set; }
    /// <summary>
    /// The nearest RollableTable ancestor of this object.
    /// </summary>
    [JsonIgnore]
    public virtual OracleTable RowOf { get; set; }
    [JsonIgnore]
    public string TableId { get; set; }
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
    [JsonProperty("Table")]
    public virtual OracleTable Subtable { get; set; }
    public virtual string SubtableId { get; set; }

    // [JsonProperty("Add template")]
    // public virtual AddTemplate AddTemplate { get; set; }

    public string Image { get; set; }
    // [JsonConstructor]
    // public OracleTableRow()
    // {
    //     Id = $"{Path} [{Ceiling}]";
    // }

}
