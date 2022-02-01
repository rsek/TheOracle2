using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using TheOracle2.OracleRoller;
using TheOracle2.UserContent;
namespace TheOracle2.DataClasses;

public class OracleTable : List<OracleTableRow>
{
    [JsonIgnore]
    [Key]
    public string Id { get; set; }
    /// <summary>
    /// A string representation of the path to this object. This is generated when building the DBset, not pulled from the JSON.
    /// </summary>
    [JsonIgnore]
    public string Path { get; set; }
    /// <summary>
    /// The nearest Oracle ancestor of this object.
    /// </summary>
    [JsonIgnore]
    public virtual OracleInfo OracleInfo { get; set; }
    [JsonIgnore]
    public virtual string OracleInfoId { get; set; }
    /// <summary>
    /// The nearest OracleCategory ancestor of this object.
    /// </summary>
    [JsonIgnore]
    public virtual OracleCategory Category { get; set; }
    [JsonIgnore]
    public virtual string CategoryId { get; set; }
    /// <summary>
    /// The table this table is embedded in, if any.
    /// </summary>
    [JsonIgnore]
    public virtual OracleTableRow EmbeddedIn { get; set; }
    [JsonIgnore]
    public virtual string RowId { get; set; }

    /// <summary>
    /// Find the first row whose dice range includes the provided number..
    /// </summary>
    public virtual OracleTableRow Lookup(int roll)
    {
        // Console.WriteLine($"Received lookup input: {roll}");
        return Find(row => row.RollIsInRange(roll));
    }
    /// <summary>
    /// Find the first row with the same result string.
    /// </summary>
    public virtual OracleTableRow LookupResult(string result)
    {
        return Find(row => row.Result == result);
    }
    // public override string ToString() => Path;
    public virtual string ToAscii()
    {
        return string.Join("\n", this.Select(row => row.ToString()));
    }
}
