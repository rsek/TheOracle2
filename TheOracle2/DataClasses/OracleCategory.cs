namespace TheOracle2.DataClasses;
public class OracleCategory
{
    public string Name { get; set; }

    [JsonProperty("Display name")]
    public string DisplayName { get; set; }
    public virtual IList<string> Aliases { get; set; }
    public virtual Source Source { get; set; }
    public string Description { get; set; }
    public virtual IList<Oracle> Oracles { get; set; }
    public virtual IList<OracleCategory> Categories { get; set; }

    [JsonProperty("Sample names")]
    public virtual IList<string> SampleNames { get; set; }

    [JsonProperty("_path")]
    public string Path { get; set; }
    public string Id { get; set; }
}
