using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TheOracle2.DataClassesNext;
public class OracleUsage
{
    [JsonIgnore]
    public int Id { get; set; }
    public virtual IList<Requires> Requires { get; set; }
    public virtual Suggestions Suggestions { get; set; }

    [DefaultValue(false)]
    public bool Repeatable { get; set; }

    [DefaultValue(false)]
    public bool Initial { get; set; }

    [JsonProperty("Max rolls")]
    [DefaultValue(1)]
    public int MaxRolls { get; set; }


    [JsonProperty("Min rolls")]
    [DefaultValue(1)]
    public int MinRolls { get; set; }


    [JsonProperty("Allow duplicate rolls")]
    [DefaultValue(false)]
    public bool AllowDuplicateRolls { get; set; }
};
