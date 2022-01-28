namespace TheOracle2.DataClasses;

public class Special
{
    [JsonIgnore]
    public int Id { get; set; }

    public string Name { get; set; }
    public int Value { get; set; }
}
