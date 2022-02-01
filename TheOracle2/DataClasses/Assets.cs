using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using TheOracle2.UserContent;

namespace TheOracle2.DataClasses;


public record Asset
{
    [JsonIgnore]
    [Key]
    public string Id { get; set; }
    public string Name { get; set; }

    // [JsonIgnore]
    // public virtual IList<OracleGuild> OracleGuilds { get; set; }

    public virtual IList<Ability> Abilities { get; set; }

    public virtual IList<string> Aliases { get; set; }

    [JsonProperty("Asset Type")]
    public string AssetType { get; set; }

    [JsonProperty("Condition Meter")]
    public virtual ConditionMeter ConditionMeter { get; set; }

    public virtual Counter Counter { get; set; }

    public string Description { get; set; }

    public IList<string> Input { get; set; }

    public bool Modules { get; set; }


    public virtual Select Select { get; set; }

    public virtual Track Track { get; set; }

    public override string ToString()
    {
        return Name;
    }
    [OnDeserialized]
    internal void OnDeserializedMethod(StreamingContext context)
    {
        Id = Name;
    }
}

public record Ability
{
    [JsonIgnore]
    public string Id { get; set; }

    [JsonProperty("Alter Moves")]
    public virtual IList<AlterMove> AlterMoves { get; set; }

    [JsonProperty("Alter Properties")]
    public virtual AlterProperties AlterProperties { get; set; }

    [JsonProperty("Counter")]
    public virtual Counter Counter { get; set; }

    [JsonProperty("Enabled")]
    public bool Enabled { get; set; }

    [JsonProperty("Input")]
    public IList<string> Input { get; set; }

    public virtual AssetMove Move { get; set; }

    [JsonProperty("Text")]
    public string Text { get; set; }
}

public record AlterProperties
{
    [JsonIgnore]
    public int Id { get; set; }
    [JsonProperty("Condition Meter")]
    public virtual ConditionMeter ConditionMeter { get; set; }

    [JsonProperty("Track")]
    public virtual Track Track { get; set; }
}

public record AssetRoot
{
    [JsonIgnore]
    public int Id { get; set; }
    [JsonProperty("Assets")]
    public virtual IList<Asset> Assets { get; set; }

    [JsonProperty("Name")]
    public string Name { get; set; }

    [JsonProperty("Source")]
    public virtual Source Source { get; set; }

    [JsonProperty("Tags")]
    public IList<string> Tags { get; set; }
}

public class AssetTrigger : MoveTrigger
{
    public string Asset { get; set; }
}

public class AssetMove : Move
{
    public string Asset { get; set; }
}

public record ConditionMeter
{
    [JsonIgnore]
    public int Id { get; set; }
    public string Name { get; set; }
    public int Max { get; set; }

    public IList<string> Conditions { get; set; }

    [JsonProperty("Starts At")]
    public int? StartsAt { get; set; }
}

public record Counter
{
    [JsonIgnore]
    public int Id { get; set; }
    [JsonProperty("Name")]
    public string Name { get; set; }

    [JsonProperty("Starts At")]
    public int StartsAt { get; set; }

    [JsonProperty("Max")]
    public int Max { get; set; }
}

public record Track
{
    [JsonIgnore]
    public int Id { get; set; }
    public string Name { get; set; }

    [JsonProperty("Starts At")]
    public int StartsAt { get; set; }
    public int Value { get; set; }
}

public record AlterMove
{
    [JsonIgnore]
    public int Id { get; set; }
    [JsonProperty("Any Move")]
    public bool AnyMove { get; set; }
    public string Name { get; set; }
    public virtual IList<AssetTrigger> Triggers { get; set; }
}

public record Select
{
    [JsonIgnore]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public IList<string> Options { get; set; }
}
