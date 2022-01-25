using System.Linq;
namespace TheOracle2.DataClasses;


public class RollableTable : List<RollableTableRow>
{
    [JsonIgnore]
    public string Id { get; set; }
    [JsonIgnore]
    public string DisplayPath { get; set; }

    /// <summary>
    /// Find the first row whose dice range includes a number.
    /// </summary>
    public RollableTableRow Lookup(int roll)
    {
        return Find(row => row.RollIsInRange(roll));
    }
    /// <summary>
    /// Find the first row with the same result string.
    /// </summary>
    public RollableTableRow LookupResult(string result)
    {
        return Find(row => row.Result == result);
    }
    public override string ToString()
    {
        return string.Join("\n", this.Select(row => row.ToString()));
    }
    public RollableTableRow Roll(Random random)
    {
        var roll = random.Next(1, 101);
        return Lookup(roll);
    }
}
