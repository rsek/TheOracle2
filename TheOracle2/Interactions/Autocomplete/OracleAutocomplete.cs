using System.Text.RegularExpressions;
using Discord.Interactions;
using Microsoft.Extensions.Logging;
using TheOracle2.DataClasses;
using TheOracle2.UserContent;

namespace TheOracle2.Commands;

public class OracleAutocomplete : AutocompleteHandler
{
    private static readonly Dictionary<string, Task<AutocompletionResult>> dict = new Dictionary<string, Task<AutocompletionResult>>();

    private Task<AutocompletionResult> emptyOraclesResult
    {
        get
        {
            var defaultKeys = new List<string> {
                "Core / Action",
                "Core / Theme",
                "Core / Descriptor",
                "Core / Focus",
                "Move / Pay the Price",
                "Space / Space Sighting / Terminus",
                "Space / Space Sighting / Outlands",
                "Space / Space Sighting / Expanse"
            };

            if (dict.TryGetValue("initialOracles", out Task<AutocompletionResult> result))
            {
                return result;
            }
            var list = DbContext.OracleTables.Where(table =>
                defaultKeys.Contains(table.Path)
            ).AsEnumerable()
                .Select(table => new AutocompleteResult(table.Path, table.Path))
                .OrderBy(x => //Todo this is really lazy ordering, but so is the rest of this getter's code.
                    x.Name == "Pay the Price" ? 1 :
                    x.Name.Contains("Space Sighting") ? 3 :
                    2)
                .Take(SelectMenuBuilder.MaxOptionCount);
            result = Task.FromResult(AutocompletionResult.FromSuccess(list));
            dict.Add("initialOracles", result);
            return result;
        }
    }

    public EFContext DbContext { get; set; }
    public ILogger<OracleAutocomplete> logger { get; set; }

    public override Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
    {
        try
        {
            List<AutocompleteResult> successList = new List<AutocompleteResult>();

            var userText = autocompleteInteraction.Data.Current.Value as string;

            if (string.IsNullOrEmpty(userText))
            {
                return emptyOraclesResult;
            }
            /// appears to be pointing at info - weird!
            successList = DbContext.OracleTables
            .Where(table => Regex.IsMatch(table.Path, $@"\b(?i){userText}"))
            .Select(table => new AutocompleteResult(table.Path, table.Path)).ToList();
            return Task.FromResult(AutocompletionResult.FromSuccess(successList.Take(SelectMenuBuilder.MaxOptionCount)));
        }
        catch (Exception ex)
        {
            return Task.FromResult(AutocompletionResult.FromError(ex));
        }
    }
    protected override string GetLogString(IInteractionContext context) => $"Accessing DB from {context.Guild}-{context.Channel}";
}
