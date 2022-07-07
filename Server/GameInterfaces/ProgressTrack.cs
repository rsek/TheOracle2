﻿using Server.Data;
using Server.DiceRoller;
using Server.Interactions.Helpers;
using TheOracle2;
using TheOracle2.GameObjects;

namespace Server.GameInterfaces
{
    public class ProgressTrack : ITrack
    {
        private readonly IMoveRepository moves;
        private IEmoteRepository Emotes { get; }
        private readonly Random Random;

        public ProgressTrack(Random random, ChallengeRank rank, IEmoteRepository emotes, IMoveRepository moves, string? title = null, string? desc = null, int score = 0)
        {
            Random = random;
            Emotes = emotes;
            this.moves = moves;

            TrackData = new TrackData
            {
                Rank = rank,
                Ticks = score * BoxSize,
                Title = title,
                Description = desc
            };
        }

        public TrackData TrackData { get; set; }
        public int BoxSize { get; set; } = 4;
        public bool IsEphemeral { get; set; } = false;
        public string TrackDisplayName { get; set; } = "Track";
        public int TrackSize { get; set; } = 10;
        public int Ticks { get => TrackData.Ticks; set => TrackData.Ticks = value; }

        public ComponentBuilder? GetComponents()
        {
            return new ComponentBuilder().WithRows(GetActionRows());
        }

        public EmbedBuilder? GetEmbed()
        {
            var builder = new EmbedBuilder();
            builder.WithAuthor("Progress Track")
                .WithTitle(TrackData.Title)
                .WithDescription(TrackData.Description)
                .AddField("Rank", TrackData.Rank)
                .AddField(this.AsDiscordField(Emotes));

            return builder;
        }

        public int GetScore()
        {
            int rawScore = Ticks / BoxSize;
            return Math.Min(rawScore, TrackSize);
        }

        public IActionRoll Roll()
        {
            return new ProgressRollRandom(Random, GetScore(), $"Progress Roll for {TrackData.Description}");
        }

        internal List<ActionRowBuilder> GetActionRows()
        {
            var myList = new List<ActionRowBuilder>();
            var actionRow1 = new ActionRowBuilder();
            ActionRowBuilder? actionRow2 = null;

            actionRow1.WithSelectMenu(new SelectMenuBuilder()
                .WithCustomId("progress-main")
                .WithPlaceholder("Manage tracker...")
                .AddOption("Mark Progress", $"track-increase-{TrackData.Id}", emote: Emotes.MarkProgress)
                .AddOption("Resolve Progress", $"track-roll-{TrackData.Id}", emote: Emotes.Roll));

            myList.Add(actionRow1);

            var movesToFind = new string[] { "Swear an Iron Vow", "Reach a Milestone", "Fulfill Your Vow", "Forsake Your Vow" };
            var vowMoves = moves.GetMoves().Where(m => movesToFind.Any(s => s.Contains(m.Name, StringComparison.OrdinalIgnoreCase)));
            var referenceSelectBuilder = new SelectMenuBuilder().WithCustomId("progress-references").WithPlaceholder("Reference Moves...");
            foreach (var move in vowMoves)
            {
                if (actionRow2 == null) actionRow2 = new ActionRowBuilder();

                referenceSelectBuilder.AddOption(move.MoveAsSelectOption(Emotes));
            }

            if (actionRow2 != null) myList.Add(actionRow2.WithSelectMenu(referenceSelectBuilder));
            return myList;
        }
    }
}
