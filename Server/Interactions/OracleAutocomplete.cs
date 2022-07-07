﻿using Discord.Interactions;
using Server.Data;
using TheOracle2.Data;

namespace TheOracle2.Commands;

public class OracleAutocomplete : AutocompleteHandler
{
    private IOracleRepository OracleRepo;

    public OracleAutocomplete(IOracleRepository oracles)
    {
        OracleRepo = oracles;
    }

    private Task<AutocompletionResult> emptyOraclesResult
    {
        get
        {
            var oracles = OracleRepo.GetOracles().Where(o => o.Name == "Pay the Price" || o.Category.Contains("Action", StringComparison.OrdinalIgnoreCase)).AsEnumerable();
            var list = oracles
                .SelectMany(x => GetOracleAutocompleteResults(x))
                .OrderBy(x =>
                    x.Name == "Pay the Price" ? 1 :
                    2)
                .Take(SelectMenuBuilder.MaxOptionCount);

            return Task.FromResult(AutocompletionResult.FromSuccess(list));
        }
    }

    public override Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
    {
        try
        {
            List<AutocompleteResult> successList = new List<AutocompleteResult>();

            var value = autocompleteInteraction.Data.Current.Value as string;

            if (String.IsNullOrWhiteSpace(value))
            {
                return emptyOraclesResult;
            }

            //Match names and aliases first
           successList.AddRange(OracleRepo.GetOracles()
                        .Where(x => x.Name.Contains(value, StringComparison.OrdinalIgnoreCase)
                            || x.Parent?.Name.Contains(value, StringComparison.OrdinalIgnoreCase) == true
                            || x.Parent?.Aliases?.Any(s => s.Contains(value, StringComparison.OrdinalIgnoreCase)) == true
                            || x.Aliases?.Any(s => s.Contains(value, StringComparison.OrdinalIgnoreCase)) == true)
                        .SelectMany(x => GetOracleAutocompleteResults(x)));

            return Task.FromResult(AutocompletionResult.FromSuccess(successList.Take(SelectMenuBuilder.MaxOptionCount)));
        }
        catch (Exception ex)
        {
            return Task.FromResult(AutocompletionResult.FromError(ex));
        }
    }

    private IEnumerable<AutocompleteResult> GetOracleAutocompleteResults(Oracle oracle)
    {
        var list = new List<AutocompleteResult>();
        if (oracle.Oracles?.Count > 0)
        {
            if (oracle.Table?.Count > 0) list.Add(new AutocompleteResult(GetOracleDisplayName(oracle), oracle.Id));
            foreach (var t in oracle.Oracles)
            {
                list.Add(new AutocompleteResult(GetOracleDisplayName(oracle, t), t.Id));
            }
        }
        else
        {
            list.Add(new AutocompleteResult(GetOracleDisplayName(oracle), oracle.Id));
        }
        return list;
    }

    private string GetOracleDisplayName(Oracle oracle, Oracle t = null)
    {
        //return (t != null) ? t.Id : oracle.Id;

        string name = oracle.Name;
        if (oracle.Parent != null) name = $"{oracle.Parent.Name} - {name}";
        if (t != null) name += $" - {t.Name}";

        return name;
    }
}
