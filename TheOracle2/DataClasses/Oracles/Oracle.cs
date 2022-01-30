using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;

namespace TheOracle2.DataClasses;

public class Oracle
{
    [JsonIgnore]
    public int Id { get; set; }
    /// <summary>
    /// A string representation of the path to this object.
    /// </summary>
    [JsonProperty("_path")]
    public string Path { get; set; }
    /// <summary>
    /// The nearest Oracle ancestor of this object, if any.
    /// </summary>
    [JsonIgnore]
    public virtual Oracle MemberOf { get; set; }
    /// <summary>
    /// The nearest OracleCategory ancestor of this object.
    /// </summary>
    [JsonIgnore]
    public virtual OracleCategory Category { get; set; }
    public string Name { get; set; }

    [JsonProperty("Display name")]
    public string DisplayName { get; set; }

    public virtual IList<string> Aliases { get; set; }
    public string Description { get; set; }
    public virtual Source Source { get; set; }
    public virtual OracleUsage Usage { get; set; }
    public virtual IList<Oracle> Oracles { get; set; }
    public virtual OracleTable Table { get; set; }
    internal void BuildAncestry(Oracle parent)
    {
        MemberOf = parent;
        Category = parent.Category;
        BuildDescendants();
    }
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
        if (Table?.Count > 0)
        {
            Table.BuildAncestry(this);
        }
    }
    public override string ToString() => Path;
}
