using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheOracle2.DataClasses;
using TheOracle2.OracleRoller;
namespace TheOracle2.UserContent;

/// <summary>
///
/// </summary>
internal class DiscordOracleTableEntity : IDiscordEntity
{
    public EFContext DbContext { get; set; }
    public OracleTable Table { get; set; }
    public Random Random { get; set; }
    public DiscordOracleTableEntity(EFContext dbContext, Random random, OracleTable table)
    {
        DbContext = dbContext;
        Random = random;
        Table = table;
    }
    public static SelectMenuOptionBuilder ToMenuOption(OracleTable table)
    {
        var option = new SelectMenuOptionBuilder()
            .WithLabel($"{table.Path}")
            .WithValue(DiscordOracleTableEntity.ToMenuOptionValue(table));
        return option;
    }
    public SelectMenuOptionBuilder ToMenuOption()
    {
        var option = new SelectMenuOptionBuilder()
            .WithLabel($"{Table.Path}")
            .WithValue(ToMenuOptionValue());
        return option;
    }
    public string ToMenuOptionValue()
    {
        return ToMenuOptionValue(Table);
    }
    public static string ToMenuOptionValue(OracleTable table)
    {
        int remainingRolls = 1;
        if (table.Metadata.Usage != null)
        {
            remainingRolls = table.Metadata.Usage.Repeatable ? -1 : table.Metadata.Usage.MaxRolls;
        }
        return $"{remainingRolls},{table.Path}";
    }
    public static SelectMenuBuilder DecrementOracleOptions(SelectMenuBuilder menuBuilder, IEnumerable<string> values)
    {
        foreach (var value in values)
        {
            menuBuilder = DecrementOracleOption(menuBuilder, value);
        }
        return menuBuilder;
    }
    public static SelectMenuBuilder DecrementOracleOptions(SelectMenuBuilder menuBuilder, IEnumerable<Oracle> oracles)
    {
        return DecrementOracleOptions(menuBuilder, oracles.Select(oracle => oracle.Path));
    }

    public static SelectMenuBuilder DecrementOracleOptions(SelectMenuBuilder menuBuilder, DiscordOracleResultEntity result)
    {
        return DecrementOracleOptions(menuBuilder, result.Select(item => item.OracleRoll.Oracle));
    }
    public static SelectMenuBuilder DecrementOracleOption(SelectMenuBuilder menuBuilder, Oracle oracle)
    {
        return DecrementOracleOption(menuBuilder, oracle.Path);
    }
    public static SelectMenuBuilder DecrementOracleOption(SelectMenuBuilder menuBuilder, string oracleId)
    {
        var targetIndex = menuBuilder.Options.FindIndex(option => option.Value.StartsWith(oracleId) || option.Label == oracleId);
        if (targetIndex == -1)
        {
            throw new ArgumentException($"The oracle could not be found in the selectmenu: {oracleId}");
        }
        var targetOption = menuBuilder.Options[targetIndex];
        var rollsLeftString = targetOption.Value.Split(",")[0];
        if (!int.TryParse(rollsLeftString, out var rollsLeft)) { throw new ArgumentException($"Unable to parse remaining rolls from {targetOption.Value}"); }
        if (rollsLeft <= -1)
        {
            // a value of -1 represents an oracle that can be added indefinitely - no changes are needed.
            // magic numbers aren't great, but given the restrictions of custom IDs they're the simplest way to handle this
            return menuBuilder;
        }
        if (rollsLeft > 0)
        {
            // if there's at least one roll left, decrement and update the option's value
            rollsLeft--;
            var newValue = $"{rollsLeft},{oracleId}";
            menuBuilder.Options[targetIndex] = menuBuilder.Options[targetIndex].WithValue(newValue);
        }
        if (rollsLeft == 0)
        {
            // no more rolls left, so remove the menu option
            menuBuilder.Options.RemoveAt(targetIndex);
        }
        return menuBuilder;
    }
    public static SelectMenuBuilder DecrementOracleOption(SelectMenuComponent menu, string value)
    {
        return DecrementOracleOption(menu.ToBuilder(), value);
    }
    public Embed[] GetEmbeds()
    {
        return null;
    }
    public MessageComponent GetComponents()
    {
        return null;
    }
    public Task<IMessage> GetDiscordMessage(IInteractionContext context)
    {
        return null;
    }
    public bool IsEphemeral { get; set; } = false;
}
