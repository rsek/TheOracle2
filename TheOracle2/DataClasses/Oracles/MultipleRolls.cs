using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TheOracle2.DataClasses;
public class MultipleRolls
{
    [JsonIgnore]
    public int Id { get; set; }

    [DefaultValue(2)]
    public int Amount { get; set; }

    [JsonProperty("Allow duplicates")]
    [DefaultValue(false)]
    public bool AllowDuplicates { get; set; }
}
