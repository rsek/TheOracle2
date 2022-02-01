using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
namespace TheOracle2.DataClasses;
public class OracleCategory
{

    [Key]
    [JsonProperty("_path")]
    public string Id { get; set; }
    /// <summary>
    /// A string representation of the path to this object.
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// The nearest OracleCategory ancestor of this object, if any.
    /// </summary>
    [JsonIgnore]
    public virtual OracleCategory Category { get; set; }
    [JsonIgnore]
    public virtual string CategoryId { get; set; }

    [JsonProperty("Display name")]
    public string DisplayName { get; set; }
    public virtual IList<string> Aliases { get; set; }
    public virtual Source Source { get; set; }
    public string Description { get; set; }
    public virtual IList<OracleInfo> Oracles { get; set; }
    public virtual IList<OracleCategory> Categories { get; set; }

    [JsonProperty("Sample names")]
    public virtual IList<string> SampleNames { get; set; }
    /// <summary>
    /// A list of tables that feature this Category in their Category field. In other words, it contains all oracles within the category *except* for those within a child subcategory.
    /// </summary>
    public List<OracleTable> TablesWithin()
    {
        var tableList = new List<OracleTable>();
        if (Oracles != null)
        {
            tableList.AddRange(Oracles.SelectMany(oracle => CrawlTables(oracle)));
        }
        return tableList;
    }

    private List<OracleTable> CrawlTables(OracleInfo oracle)
    {
        var tableList = new List<OracleTable>();
        if (oracle.Table != null)
        {
            tableList.Add(oracle.Table);
        }
        if (oracle.Oracles != null)
        {
            var recursiveList = oracle.Oracles.SelectMany(suboracle => CrawlTables(suboracle));
            tableList.AddRange(recursiveList);
        }
        return tableList;
    }
    public override string ToString() => Id;
}
