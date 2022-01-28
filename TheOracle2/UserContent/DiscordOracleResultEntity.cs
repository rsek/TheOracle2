using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheOracle2.DataClasses;
using TheOracle2.OracleRoller;

namespace TheOracle2.UserContent;

internal class DiscordOracleResultEntity : IDiscordEntity
{
    // TODO:
    // should probably be initialized with an OracleResult
    // needs a way to check embeds for existing rolls
    // and reference them against roll Max, repeating, etc

    // e.g. AddRollsToEmbed?
    //
    // function to infer what the add'l rolls should be:
    // * same category
    // * used with key
    public DiscordOracleResultEntity(Oracle oracle, EFContext dbContext, Random random)
    {
        // var RollerFactory = new TableRollerFactory(dbContext, random);

        // var rollResult = RollerFactory.GetRoller(oracle).Build();

        // ob = new DiscordOracleBuilder(rollResult).Build();
    }

    public DiscordOracleResultEntity(string oracleQuery, EFContext dbContext, Random random)
    {
        // var RollerFactory = new TableRollerFactory(dbContext, random);

        // var rollResult = RollerFactory.GetRoller(oracleQuery).Build();

        // ob = new DiscordOracleBuilder(rollResult).Build();
    }

    public bool IsEphemeral { get; set; } = false;

    public Oracle Data { get; set; }

    public OracleRolls OracleResult { get; set; }

    public EmbedFieldBuilder OracleRollField(OracleRoll oracleRoll)
    {
        var fieldNameRoll = oracleRoll.Value.ToString();
        var fieldValue = oracleRoll.Row.ToString();
        if (oracleRoll.Rolls != null && oracleRoll.Rolls?.Count > 0)
        {
            // to a foreign table: " ⏵ "
            string resultSeparator = " ⏵ ";
            // for rolls that don't (or can't, like action+theme) allow duplicates
            string resultJoiner = " + ";
            // to same table (e.g. 'roll twice'): " > "?? ": "???
            if (oracleRoll.Row.Table != null)
            {
                resultSeparator = " > ";
            }
            // to table embedded in row: " / "
            if (oracleRoll.Row.MultipleRolls != null)
            {
                resultSeparator = " / ";
            }
            if (oracleRoll.Rolls.AllowDuplicateRolls)
            {
                resultJoiner = ", ";
            }
            fieldNameRoll += resultSeparator + string.Join(
                resultJoiner,
                oracleRoll.Rolls.Select(item => item.Value)
            );
            fieldValue += resultSeparator + string.Join(
                resultJoiner,
                oracleRoll.Rolls
            );
        }
        var fieldName = oracleRoll.Row.DisplayPath + $" [{fieldNameRoll}]";
        return new EmbedFieldBuilder()
            .WithName(fieldName)
            .WithValue(fieldValue)
            ;
    }
    public EmbedBuilder ToEmbed()
    {
        var fields = OracleResult
            .Select(oracleRoll =>
                OracleRollField(oracleRoll)
                    .WithIsInline(true));
        var embed = new EmbedBuilder()
            .WithFields(fields);
        return embed;
        // TODO: split over multiple embeds if it exceeds the number of fields?
        // should either have a size limit of 25 to accomodate embeds, or else have some logic to account for multiple embeds
    }
    public MessageComponent GetComponents()
    {
        return null;
    }

    public async Task<IMessage> GetDiscordMessage(IInteractionContext context)
    {
        return null;
    }

    public Embed[] GetEmbeds()
    {
        return null;
    }
}
