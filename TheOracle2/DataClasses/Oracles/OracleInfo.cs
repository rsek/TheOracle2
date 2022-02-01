using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;

namespace TheOracle2.DataClasses;

public class OracleInfo
{
    [JsonIgnore]
    [Key]
    public string Id { get; set; }

    /// <summary>
    /// A string representation of the path to this object.
    /// </summary>
    [JsonProperty("_path")]
    public string Path { get; set; }
    public string Name { get; set; }

    [JsonProperty("Display name")]
    public string DisplayName { get; set; }
    public virtual IList<string> Aliases { get; set; }
    public string Description { get; set; }
    public virtual Source Source { get; set; }
    public virtual OracleUsage Usage { get; set; }
    public virtual IList<OracleInfo> Oracles { get; set; }
    public virtual OracleTable Table { get; set; }
    public virtual string TableId { get; set; }
    /// <summary>
    /// The nearest Oracle ancestor of this object, if any.
    /// </summary>
    [JsonIgnore]
    public virtual OracleInfo MemberOf { get; set; }
    public virtual string MemberOfId { get; set; }
    /// <summary>
    /// The nearest OracleCategory ancestor of this object.
    /// </summary>
    [JsonIgnore]
    public virtual OracleCategory Category { get; set; }
    public string CategoryId { get; set; }
    public override string ToString() => Path;
}
