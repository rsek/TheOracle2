﻿using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheOracle2.UserContent;

namespace TheOracle2.Commands
{
    public class AddContentCommand : ISlashCommand
    {
        public SocketSlashCommand Context { get; set; }

        [OracleSlashCommand("add-content")]
        public async Task Register(EFContext efContext)
        {
            int id = Convert.ToInt32(Context.Data.Options.FirstOrDefault().Value);

            ulong guildId;
            if (Context.Channel is IGuildChannel guildChannel)
            {
                guildId = guildChannel.GuildId;
            }
            else
            {
                guildId = Context.Channel.Id;
            }

            var guild = OracleGuild.GetGuild(guildId, efContext);
            var items = string.Join("\n", guild.Moves.Select(gi => $"{gi.Id} - {gi.Name}"));

            await Context.RespondAsync(items, ephemeral: true);

            //guild.GameItems.Add(new GameItem() { GameItemId = id });
            //await efContext.SaveChangesAsync().ConfigureAwait(false);
            //await Context.RespondAsync(":thumbsup:", ephemeral: true);
        }

        public IList<SlashCommandBuilder> GetCommandBuilders()
        {
            var command = new SlashCommandBuilder()
                .WithName("add-content")
                .WithDescription("Register a game item this discord server")
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("val")
                    .WithDescription("Registers the ID as a content item on this server")
                    .WithRequired(true)
                    .WithType(ApplicationCommandOptionType.Integer)
            );

            return new List<SlashCommandBuilder>() { command };
        }
    }
}
