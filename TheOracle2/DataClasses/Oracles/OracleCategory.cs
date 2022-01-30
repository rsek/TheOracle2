using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
namespace TheOracle2.DataClasses;
public class OracleCategory
{
    /// <summary>
    /// A string representation of the path to this object.
    /// </summary>
    [JsonProperty("_path")]
    [Key]
    public string Path { get; set; }
    public string Name { get; set; }
    /// <summary>
    /// The nearest OracleCategory ancestor of this object, if any.
    /// </summary>
    [JsonIgnore]
    public virtual OracleCategory Category { get; set; }

    [JsonProperty("Display name")]
    public string DisplayName { get; set; }
    public virtual IList<string> Aliases { get; set; }
    public virtual Source Source { get; set; }
    public string Description { get; set; }
    public virtual IList<Oracle> Oracles { get; set; }
    public virtual IList<OracleCategory> Categories { get; set; }

    [JsonProperty("Sample names")]
    public virtual IList<string> SampleNames { get; set; }
    /// <summary>
    /// A list of tables that feature this Category in their Category field. In other words, it contains all oracles within the category *except* for those within a child subcategory.
    /// </summary>
    [JsonIgnore]
    public virtual IList<OracleTable> TablesWithin { get; set; } = new List<OracleTable>();
    internal void BuildAncestry(OracleCategory parent)
    {
        Category = parent;
        BuildDescendants();
    }
    internal void BuildDescendants()
    {
        if (Oracles?.Count > 0)
        {
            foreach (var oracle in Oracles)
            {
                oracle.BuildAncestry(this);
            }
        }
        if (Categories?.Count > 0)
        {
            foreach (var oracleCat in Categories)
            {
                oracleCat.BuildAncestry(this);
            }
        }
    }
    [OnDeserialized]
    internal void OnDeserializedMethod(StreamingContext context)
    {
        // this should only necessary on top-level categories - the rest should recurse throughout the entire oracle tree
        // TODO: figure out a smarter way of determining whether this is a top-level category?
        if (Path == Name)
        {
            BuildDescendants();
        }
    }
    public override string ToString() => Path;
}
