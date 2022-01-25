using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TheOracle2.DataClassesNext;
public class Suggestions
{

    [JsonIgnore]
    public int Id { get; set; }
    public virtual IList<GameObject> GameObjects { get; set; }
    public virtual IList<string> OracleRolls { get; set; }
}
