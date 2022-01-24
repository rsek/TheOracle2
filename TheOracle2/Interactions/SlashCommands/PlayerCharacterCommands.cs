﻿using Discord.Interactions;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using TheOracle2.Commands;
using TheOracle2.GameObjects;
using TheOracle2.UserContent;

namespace TheOracle2;

[Group("player-character", "Create and manage player characters.")]
public class EditPlayerPaths : InteractionModuleBase
{
    public EditPlayerPaths(EFContext dbContext)
    {
        DbContext = dbContext;
    }

    public EFContext DbContext { get; }

    public GuildPlayer GuildPlayer => GuildPlayer.AddIfMissing(Context, DbContext);

    public override void AfterExecute(ICommandInfo command)
    {
        DbContext.SaveChanges();
    }


    [SlashCommand("create", "Create a player character.")]
    public async Task BuildPlayerCard(string name, [MaxValue(4)][MinValue(1)] int edge, [MaxValue(4)][MinValue(1)] int heart, [MaxValue(4)][MinValue(1)] int iron, [MaxValue(4)][MinValue(1)] int shadow, [MaxValue(4)][MinValue(1)] int wits)
    {
        await DeferAsync();
        var pcData = new PlayerCharacter(Context as SocketInteractionContext, name, edge, heart, iron, shadow, wits);
        DbContext.PlayerCharacters.Add(pcData);
        await DbContext.SaveChangesAsync();

        // AfterExecute does SaveChanges, but the PC has to be saved to the DB to get an Id.
        GuildPlayer.LastUsedPcId = pcData.Id;

        var pcEntity = new PlayerCharacterEntity(pcData);
        await FollowupAsync(embeds: pcEntity.GetEmbeds(), components: pcEntity.GetComponents()).ConfigureAwait(false);
        return;
    }
    [SlashCommand("impacts", "Add or remove a player character's Impacts (p. 46).")]
    public async Task SetImpact(
        [Autocomplete(typeof(CharacterAutocomplete))]
        string character,
        AddRemoveOptions action,
        [Autocomplete(typeof(ImpactAutocomplete))]
        string impact
    )
    {
        if (!int.TryParse(character, out var id)) return;
        var pc = await DbContext.PlayerCharacters.FindAsync(id);
        if (pc == null) { throw new Exception($"PC not found: {character}"); }

        if (pc.Impacts == null)
        {
            pc.Impacts = new List<string>();
        }

        var pcHasImpact = pc.Impacts?.Contains(impact, StringComparer.InvariantCultureIgnoreCase) ?? false;
        var response = "";

        switch (action)
        {
            case AddRemoveOptions.Remove:
                if (pcHasImpact)
                {
                    pc.Impacts = pc.Impacts.Where(item => !string.Equals(item, impact, StringComparison.OrdinalIgnoreCase)) as List<string>;
                    response += $"**{impact}** was removed. ";
                }
                break;
            case AddRemoveOptions.Add:
                if (!pcHasImpact)
                {
                    pc.Impacts.Add(impact);
                    response += $"**{impact}** was added. ";
                }
                break;
        }
        if (pc.Momentum > pc.MomentumMax) { pc.Momentum = pc.MomentumMax; }
        var impactListString = (pc.ImpactCount > 0 ? string.Join(", ", pc.Impacts) : "*none*") + ".";
        response += $"{pc.Name}'s current impacts: {impactListString} (Momentum Max {pc.MomentumMax}, Momentum Reset {pc.MomentumReset})\n\n";
        response += "Impacts will update next time you trigger an interaction on that character card.";

        var pcEntity = new PlayerCharacterEntity(pc);
        var components = new ComponentBuilder()
        .WithButton(await pcEntity.GetJumpButton(Context))
        ;
        await RespondAsync(response, ephemeral: true, components: components.Build()).ConfigureAwait(true);
        //await pc.RedrawCard(Context.Client);
    }

    [SlashCommand("earned-xp", "Set a player character's earned xp.")]
    public async Task SetEarnedXp([Autocomplete(typeof(CharacterAutocomplete))] string character,
                                [Summary(description: "The total amount of Xp this character has earned")][MinValue(0)] int earnedXp)
    {
        if (!int.TryParse(character, out var Id)) return;
        var pc = DbContext.PlayerCharacters.Find(Id);
        pc.XpGained = earnedXp;
        await RespondAsync($"{pc.Name}'s XP was set to **{earnedXp}**. Their card will be updated the next time you click a button on the card.", ephemeral: true).ConfigureAwait(false);
        return;
    }

    [SlashCommand("delete", "Delete a player character, removing them from the database and search results.")]
    public async Task DeleteCharacter([Autocomplete(typeof(CharacterAutocomplete))] string character)
    {
        if (!int.TryParse(character, out var id)) return;
        var pc = await DbContext.PlayerCharacters.FindAsync(id);

        if (pc.DiscordGuildId != Context.Guild.Id || (pc.UserId != Context.User.Id && Context.Guild.OwnerId != Context.User.Id))
        {
            await RespondAsync($"You are not allowed to delete this player character.", ephemeral: true);
        }

        await RespondAsync($"Are you sure you want to delete {pc.Name}?\nMomentum: {pc.Momentum}, xp: {pc.XpGained}\nPlayer id: {pc.Id}, last known message id: {pc.MessageId}",
            components: new ComponentBuilder()
            .WithButton(GenericComponents.CancelButton())
            .WithButton("Delete", $"delete-player-{pc.Id}", style: ButtonStyle.Danger)
            .Build());
    }
}

public enum AddRemoveOptions
{
    Remove,
    Add
}
