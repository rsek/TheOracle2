using System.ComponentModel.DataAnnotations;
namespace TheOracle2.DataClasses;
public class GameObject
{
    [JsonIgnore]
    public int Id { get; set; }
    [JsonProperty("Object type")]
    public string ObjectType { get; set; }
    public virtual IList<Requires> Requires { get; set; }
}
