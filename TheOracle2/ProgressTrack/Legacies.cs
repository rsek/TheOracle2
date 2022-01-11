namespace TheOracle2.GameObjects;

public class Legacies
{
  public Legacies(int questTicks = 0, int bondTicks = 0, int discoveryTicks = 0)
  {
    Quests = new LegacyTrack(Legacy.Quests, questTicks);
    Bonds = new LegacyTrack(Legacy.Bonds, bondTicks);
    Discoveries = new LegacyTrack(Legacy.Discoveries, discoveryTicks);
  }
  public LegacyTrack Quests { get; set; }
  public LegacyTrack Bonds { get; set; }
  public LegacyTrack Discoveries { get; set; }
  public int Xp => Quests.Xp + Bonds.Xp + Discoveries.Xp;
  public EmbedBuilder ToEmbed()
  {
    return new EmbedBuilder()
      .WithTitle("Legacies")
      .AddField("XP Earned", Xp.ToString(), true)
      .AddField(Quests.ToEmbedField())
      .AddField(Bonds.ToEmbedField())
      .AddField(Discoveries.ToEmbedField())
    ;
  }
  public ComponentBuilder MakeComponents()
  {
    ComponentBuilder components = new ComponentBuilder().WithSelectMenu(new SelectMenuBuilder()
      .AddOption(Quests.MarkRewardOption())
      .AddOption(Bonds.MarkRewardOption())
      .AddOption(Discoveries.MarkRewardOption())
    );
    return components;
  }
}