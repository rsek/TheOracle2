using System.Linq;

namespace TheOracle2.DataClasses;

public class Oracle
{
    [JsonProperty("_path")]
    public string Path { get; set; }
    public string Id { get; set; }
    public string Name { get; set; }

    [JsonProperty("Display name")]
    public string DisplayName { get; set; }

    public virtual IList<string> Aliases { get; set; }

    public string Description { get; set; }

    public virtual Source Source { get; set; }

    public virtual OracleUsage Usage { get; set; }

    public virtual IList<OracleCategory> Categories { get; set; }

    public virtual IList<Oracle> Oracles { get; set; }

    public virtual RollableTable Table { get; set; }

    public EmbedBuilder ToEmbed()
    {
        EmbedBuilder embed = new EmbedBuilder()
            // .WithAuthor(PathTo)
            .WithTitle(DisplayName ?? Name);
        // if (Source != null)
        // {
        //     embed.WithFooter(Source.ToString());
        // }
        if (Table != null)
        {
            embed.WithDescription(Table.ToString());
        }
        if (Oracles != null)
        {
            foreach (Oracle oracle in Oracles)
            {
                if (oracle.Table != null && oracle.Table.Any())
                {
                    embed.AddField(oracle.Name, oracle.Table.ToString());
                }
            }
        }
        return embed;
    }

}
