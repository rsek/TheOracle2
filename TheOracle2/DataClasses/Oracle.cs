using System.Linq;

namespace TheOracle2.DataClasses;

public class Oracle
{
    [JsonProperty("_path")]
    public string Path { get; set; }
    public string Id { get; set; }
    public string Name { get; set; }

    [JsonProperty("Display name")]
    public string DisplayName { get; set; }

    public virtual IList<string> Aliases { get; set; }

    public string Description { get; set; }

    public virtual Source Source { get; set; }

    public virtual OracleUsage Usage { get; set; }

    public virtual IList<OracleCategory> Categories { get; set; }

    public virtual IList<Oracle> Oracles { get; set; }
    public virtual RollableTable Table { get; set; }
}
