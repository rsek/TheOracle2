using System.ComponentModel.DataAnnotations;

namespace TheOracle2.DataClasses;
public class Requires
{
    [JsonIgnore]
    public int Id { get; set; }
    public virtual IList<string> Paths { get; set; }
    public virtual IList<string> Results { get; set; }
    // TODO: method for testing whether an oracle contains a given result and/or retrieving it
    // TODO: method for testing whether an embed contains a given resul
}
