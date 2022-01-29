using System.ComponentModel;

namespace TheOracle2.DataClasses;
public class AddTemplate
{
    [JsonProperty("Object type")]
    public string ObjectType { get; set; }
}
