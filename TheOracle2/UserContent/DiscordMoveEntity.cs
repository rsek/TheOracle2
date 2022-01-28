using TheOracle2.DataClasses;

namespace TheOracle2.UserContent
{
    internal class DiscordMoveEntity : IDiscordEntity
    {
        public DiscordMoveEntity(Move move, bool ephemeral = false)
        {
            Move = move;
            IsEphemeral = ephemeral;
        }

        public bool IsEphemeral { get; set; }
        public Move Move { get; }

        public SelectMenuOptionBuilder ReferenceOption()
        {
            string append = "…";
            int maxChars = SelectMenuOptionBuilder.MaxDescriptionLength;
            SelectMenuOptionBuilder option = new();
            string moveTrigger = Move.TriggerText ?? Move.Text;
            string triggerString = moveTrigger.Length <= maxChars ? moveTrigger : moveTrigger[0..(maxChars - 1)] + append;
            option.WithLabel(Move.Name);
            option.WithEmote(GetEmoji());
            option.WithValue(Move.Name);
            option.WithDescription(triggerString);
            return option;
        }

        // commented out until there's a sensible way to implement ephemeral-reveal. probably inessential anyways.
        public MessageComponent GetComponents()
        {
            // if (IsEphemeral)
            // {
            //   ButtonBuilder button = new ButtonBuilder()
            //   .WithLabel("Reveal to channel")
            //   .WithCustomId("ephemeral-reveal")
            //   .WithEmote(new Emoji("👁"))
            //   .WithStyle(ButtonStyle.Secondary)
            //   ;
            //   return new ComponentBuilder().WithButton(button).Build();
            // }
            return null;
        }

        public async Task<IMessage> GetDiscordMessage(IInteractionContext context)
        {
            return null;
        }

        public Embed[] GetEmbeds()
        {
            return new Embed[] {new EmbedBuilder()
                .WithAuthor(Move.Category)
                .WithTitle(Move.Name)
                .WithDescription(Move.Text).Build() };
        }
        public IEmote GetEmoji()
        {
            if (Emoji.ContainsKey(Move.Name)) { return Emoji[Move.Name]; }
            if (Emoji.ContainsKey(Move.Category)) { return Emoji[Move.Category]; }
            return new Emoji("📖");
        }
        public static readonly Dictionary<string, IEmote> Emoji = new Dictionary<string, IEmote>(){
    {"Quest", new Emoji("✴️")},
    {"Combat", new Emoji("⚔️")},
    {"Recovery", new Emoji("❤️‍🩹 ")},
    {"Suffer", new Emoji("🩸")},
      {"Lose Momentum", new Emoji("⌛️")},
      {"Endure Harm", new Emoji("🩸")},
      {"Endure Stress", new Emoji("💔")},
      {"Companion Takes a Hit", new Emoji("🩸")},
      {"Sacrifice Resources", new Emoji("💸")},
      {"Withstand Damage", new Emoji("⚙️")},
    {"Connection", new Emoji("🪢")},
    {"Threshold", new Emoji("🚪")},
      {"Face Death", new Emoji("💀")},
      {"Face Desolation", new Emoji("🖤")},
      {"Overcome Destruction", new Emoji("💥")},
    {"Exploration", new Emoji("🧭")},
    {"Legacy", new Emoji("🔖")},
    {"Adventure", new Emoji("🌐")},
      {"Compel", new Emoji("⚖️")},
      {"Gather Information", new Emoji("🧩")},
      {"Check Your Gear", new Emoji("🎒")},
      {"Aid Your Ally", new Emoji("👥")},
    {"Fate", new Emoji("🔮")},
      {"Ask the Oracle", new Emoji("🔮")}
  };
    }
}
