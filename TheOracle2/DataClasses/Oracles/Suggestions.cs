using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TheOracle2.DataClasses;
public class Suggestions
{
    [JsonIgnore]
    public int Id { get; set; }
    public virtual IList<GameObject> GameObjects { get; set; }
    /// <summary>
    /// Hints/suggestions for related oracle rolls. Should not be rolled automatically.
    /// </summary>
    public virtual IList<string> OracleRolls { get; set; }
}
