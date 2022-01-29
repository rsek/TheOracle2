using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TheOracle2.DataClasses;
using TheOracle2.OracleRoller;
namespace TheOracle2.UserContent;

/// <summary>
/// Renders a single OracleRoll as an embed field.
/// </summary>
internal class DiscordOracleRollEntity : IDiscordEntityField
{
    public EFContext DbContext { get; set; }
    public Random Random { get; set; }
    public OracleRoll OracleRoll { get; set; }

    /// <summary>
    /// Creates a roll entity with a new OracleRoll from a single oracle.
    /// </summary>
    public DiscordOracleRollEntity(EFContext dbContext, Random random, Oracle oracle)
    {
        DbContext = dbContext;
        OracleRoll = new OracleRoll(dbContext, random, oracle);
    }
    /// <summary>
    /// Creates a roll entity with a new OracleRoll from a single oracle, filtered against a blacklist of integers (rows containing them will be excluded).
    /// </summary>
    public DiscordOracleRollEntity(EFContext dbContext, Random random, Oracle oracle, IEnumerable<int> blacklistRowsWith) : this(dbContext, random)
    {
        OracleRoll = new OracleRoll(dbContext, random, oracle, blacklistRowsWith);
    }
    public DiscordOracleRollEntity(OracleRoll oracleRoll)
    {
        OracleRoll = oracleRoll;
        DbContext = OracleRoll.DbContext;
    }
    public DiscordOracleRollEntity(EFContext dbContext, Random random, EmbedField embedField) : this(dbContext, random, embedField.Name)
    {
    }
    public DiscordOracleRollEntity(EFContext dbContext, Random random, string oracleString) : this(dbContext, random)
    {
        OracleRoll = ParseOracleRoll(oracleString).GetAwaiter().GetResult();
    }
    private DiscordOracleRollEntity(EFContext dbContext, Random random)
    {
        DbContext = dbContext;
        Random = random;
    }
    const string OracleRollPattern = @"^(.*) \[([0-9])+(.*)\]$";
    const string IntegerPattern = "([0-9]+)";

    public static int GetFirstInteger(string stringWithIntegers)
    {
        string capture = Regex.Match(stringWithIntegers, IntegerPattern).Captures.FirstOrDefault()?.ToString();
        if (!int.TryParse(capture, out var number)) { throw new ArgumentException($"Could not parse an integer from {stringWithIntegers}"); }
        return number;
    }
    public static bool IsOracleRoll(string oracleString)
    {
        return Regex.IsMatch(oracleString, OracleRollPattern);
    }
    public static bool IsOracleRoll(EmbedField field)
    {
        return IsOracleRoll(field.Name);
    }
    private async Task<Oracle> GetOracle(string oracleId)
    {
        return await DbContext.Oracles.FindAsync(oracleId);
    }
    private async Task<OracleRoll> ParseOracleRoll(string oracleRollString)
    {
        if (!IsOracleRoll(oracleRollString))
        {
            // attempt initialization from sparse string if it appears to be an ID
            if (oracleRollString.Contains(" / "))
            {
                return new OracleRoll(DbContext, Random, oracleRollString);
            }
            throw new ArgumentException($"Unable to parse field name as oracle roll: {oracleRollString}");
        }
        var oracleRegex = Regex.Match(oracleRollString, OracleRollPattern);
        var oracleId = oracleRegex.Captures[0].ToString();
        var primaryRollString = oracleRegex.Captures[1].ToString();
        if (!int.TryParse(primaryRollString, out int primaryRoll))
        {
            throw new ArgumentException($"Unable to parse primary die roll from string: {oracleRollString}");
        }
        var secondaryDieString = oracleRegex.Captures[2].ToString();
        var oracle = await GetOracle(oracleId);
        if (oracle == null)
        {
            throw new ArgumentException($"Unknown Oracle ID: {oracleId}");
        }
        var oracleRoll = new OracleRoll(DbContext, Random, oracle, primaryRoll);

        if (!string.IsNullOrEmpty(secondaryDieString))
        {
            if (!oracleRoll.Rolls.Any())
            {
                throw new ArgumentException($"Roll string appears to , but this row doesn't include any additional rolls from MultipleRolls or OracleRolls. String: {oracleRollString}");
            }
            if (!Regex.IsMatch(secondaryDieString, IntegerPattern))
            {
                throw new ArgumentException($"Unable to parse secondary rolls from substring: {secondaryDieString} (original string: {oracleRollString})");
            }
            // TODO: handle nested rolls
            // first get enumerable containing the integers
            var presetRolls = Regex.Matches(secondaryDieString, IntegerPattern)
                .Skip(1) // skip first match since it's just the whole string
                .Select(match =>
                {
                    var substring = match.Captures.Single().ToString();
                    if (!int.TryParse(substring, out int roll))
                    {
                        throw new ArgumentException($"Unable to parse a secondary roll from substring: {substring} (original string: {oracleRollString})");
                    }
                    return roll;
                }).ToList()
                ;
            if (oracleRoll.Rolls?.Any() == true)
            {
                var crawler = CrawlAndPresetRolls(oracleRoll.Rolls, presetRolls);
                oracleRoll.Rolls = crawler.Item1;
            }
        }
        return oracleRoll;
    }

    private Tuple<OracleResult, IEnumerable<int>> CrawlAndPresetRolls(OracleResult oracleResult, IEnumerable<int> presetValues)
    {
        var modifiedResult = oracleResult;
        var presets = presetValues.ToList();

        if (oracleResult.Any() && presetValues.Any())
        {
            modifiedResult.ForEach(oracleRoll =>
           {
               oracleRoll.Value = presets.FirstOrDefault();
               presets.RemoveAt(0);
               if (oracleRoll.Rolls != null && oracleRoll.Rolls?.Any() == true)
               {
                   var childCrawler = CrawlAndPresetRolls(oracleRoll.Rolls, presets);
                   oracleRoll.Rolls = childCrawler.Item1;
                   presets = childCrawler.Item2.ToList();
               }
           });
        }


        return new Tuple<OracleResult, IEnumerable<int>>(modifiedResult, presets);
    }
    private async Task<OracleRoll> ParseOracleRoll(EmbedField field)
    {
        var oracleString = field.Name;
        return await ParseOracleRoll(oracleString);
        // TODO: have it check the results against the actual strings? hmm...
    }


    /// <summary>
    /// String used to separate results that originate from a different table, e.g. "Stellar Object ⏵ A burning yellow star."
    /// </summary>
    const string ForeignTableSeparator = " ⏵ ";
    /// <summary>
    /// String used to separate results that originate from the same table, generally from a MultipleRolls result.
    /// e.g "Roll Twice > result A + result B"
    /// </summary>
    /// TODO: use something else?
    const string SameTableSeparator = " > ";
    /// <summary>
    /// String used to separate results that originate from a Table embedded in the Row.
    /// </summary>
    /// TODO: switch to " > "? or just use the same as ForeignTableSeparator?
    const string SubtableSeparator = " / ";
    /// <summary>
    /// String used to join subresults that forbid duplicates (or simply can't produce them, on account of being different tables).
    /// e.g. "Action + Theme ⏵ Attack + Planet"
    /// </summary>
    const string NonDuplicateJoiner = " + ";
    /// <summary>
    /// String used to join subresults where duplicates are allowed.
    /// TODO: account for ironsworn-style "roll twice and make it worse" results
    const string DuplicateJoiner = ", ";
    public EmbedFieldBuilder ToEmbedField()
    {
        var fieldNameRoll = OracleRoll.Value.ToString();
        var fieldValue = OracleRoll.Row.ToString();
        if (OracleRoll.Rolls != null && OracleRoll.Rolls?.Count > 0)
        {
            string resultSeparator = ForeignTableSeparator;
            // for rolls that don't (or can't, like action+theme) allow duplicates
            string resultJoiner = NonDuplicateJoiner;
            // to same table (e.g. 'roll twice'): " > "?? ": "???
            if (OracleRoll.Row.Table != null)
            {
                resultSeparator = SameTableSeparator;
            }
            // to table embedded in row: " / "
            if (OracleRoll.Row.MultipleRolls != null)
            {
                resultSeparator = SubtableSeparator;
            }
            if (OracleRoll.Rolls.AllowDuplicateRolls)
            {
                resultJoiner = DuplicateJoiner;
            }
            fieldNameRoll += resultSeparator + string.Join(
                resultJoiner,
                OracleRoll.Rolls.Select(item => item.Value)
            );
            fieldValue += resultSeparator + string.Join(
                resultJoiner,
                OracleRoll.Rolls
            );
        }
        var fieldName = OracleRoll.Row.DisplayPath + $" [{fieldNameRoll}]";
        return new EmbedFieldBuilder()
            .WithName(fieldName)
            .WithValue(fieldValue)
            .WithIsInline(true)
            ;
    }
}
