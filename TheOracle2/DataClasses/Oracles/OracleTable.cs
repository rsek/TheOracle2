using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using TheOracle2.OracleRoller;
using TheOracle2.UserContent;
namespace TheOracle2.DataClasses;

public class OracleTable : List<OracleTableRow>
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
    /// The table this table is embedded in, if any.
    /// </summary>
    [JsonIgnore]
    public virtual OracleTable EmbeddedIn { get; set; }

    /// <summary>
    /// Find the first row whose dice range includes the provided number..
    /// </summary>
    public virtual OracleTableRow Lookup(int roll)
    {
        Console.WriteLine($"Received lookup input: {roll}");
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
    internal virtual void BuildAncestry(Oracle parent)
    {
        Metadata = parent;
        Path = parent.Path;
        Category = parent.Category;
        BuildDescendants();
    }
    internal virtual void BuildAncestry(OracleTableRow parent)
    {
        Path = parent.Path;
        EmbeddedIn = parent.RowOf;
        Metadata = parent.Metadata;
        Category = parent.Category;
        BuildDescendants();
    }
    internal virtual void BuildDescendants()
    {
        Category.TablesWithin.Add(this);
        foreach (var row in this)
        {
            row.BuildAncestry(this);
        }
        Console.WriteLine($"{this.Path}: Built {this.Count} rows.");
    }
}
