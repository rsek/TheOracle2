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
internal class DiscordOracleEntity : IDiscordEntity
{
    public EFContext DbContext { get; set; }
    public Oracle Oracle { get; set; }
    public Random Random { get; set; }
    public DiscordOracleEntity(EFContext dbContext, Random random, Oracle oracle)
    {
        DbContext = dbContext;
        Random = random;
        Oracle = oracle;
    }

    public static SelectMenuOptionBuilder ToMenuOption(Oracle oracle)
    {
        var option = new SelectMenuOptionBuilder()
            .WithLabel($"{oracle.Id}")
            .WithValue(DiscordOracleEntity.ToMenuOptionValue(oracle));
        return option;
    }
    public SelectMenuOptionBuilder ToMenuOption()
    {
        var option = new SelectMenuOptionBuilder()
            .WithLabel($"{Oracle.Id}")
            .WithValue(ToMenuOptionValue());
        return option;
    }
    public string ToMenuOptionValue()
    {
        return ToMenuOptionValue(Oracle);
    }
    public static string ToMenuOptionValue(Oracle oracle)
    {
        int remainingRolls = 1;
        if (oracle.Usage != null)
        {
            remainingRolls = oracle.Usage.Repeatable ? -1 : oracle.Usage.MaxRolls;
        }
        return $"{remainingRolls},{oracle.Id}";
    }
    public static SelectMenuBuilder DecrementOracleOption(SelectMenuBuilder menuBuilder, string value)
    {
        var rollsLeftString = value.Split(",")[0];
        if (!int.TryParse(rollsLeftString, out var rollsLeft))
        {
            throw new ArgumentException($"Couldn't parse the remaining rolls from the value: {value}");
        }
        if (rollsLeft <= -1)
        {
            // a value of -1 represents an oracle that can be added indefinitely - no changes are needed.
            // magic numbers aren't great, but given the restrictions of custom IDs they're the simplest way to handle this
            return menuBuilder;
        }
        var targetOption = menuBuilder.Options.Find(option => option.Value == value || option.Label == value.Split(",")[1]);
        if (targetOption == null)
        {
            throw new ArgumentException($"The provided value could not be found in the selectmenu: {value}");
        }
        //
        var oracleId = value.Split(",").Last();
        // var menuBuilder = menu.ToBuilder();
        var targetIndex = menuBuilder.Options.FindIndex(item => item.Value == value);

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
