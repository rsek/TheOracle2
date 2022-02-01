using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using TheOracle2.GameObjects;

namespace TheOracle2.DataClasses;

public abstract class EncounterBase : IRanked
{
    [JsonIgnore]
    [Key]
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public virtual ChallengeRank Rank { get; set; }

    [OnDeserialized]
    protected virtual void Initialize()
    {
        Id = Name;
    }
}


public class Encounter : EncounterBase
{
    public string Nature { get; set; }
    public virtual IList<string> Drives { get; set; }
    public virtual IList<string> Features { get; set; }
    public virtual IList<string> Tactics { get; set; }
    [JsonProperty("Quest Starter")]
    public string QuestStarter { get; set; }
    public virtual Source Source { get; set; }
    public string Summary { get; set; }
    [JsonProperty("Variants", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public virtual IList<EncounterVariant> Variants { get; set; }
    [OnDeserialized]
    protected override void Initialize()
    {
        base.Initialize();
        if (Variants?.Any() == true)
        {
            foreach (var variant in Variants)
            {
                variant.BaseEncounter = this;
            }
        }
    }
}

public class EncounterVariant : EncounterBase
{
    [JsonIgnore]
    public virtual Encounter BaseEncounter { get; set; }
}


public class EncountersRoot
{
    public virtual IList<Encounter> Encounters { get; set; }
    public string Name { get; set; }
    public virtual Source Source { get; set; }
}
