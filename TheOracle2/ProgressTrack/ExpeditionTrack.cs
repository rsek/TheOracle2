namespace TheOracle2.GameObjects;

public class ExpeditionTrack : ProgressTrack
{

  public ExpeditionTrack(Embed embed) : base(embed) { }
  public ExpeditionTrack(Embed embed, int ticks) : base(embed, ticks) { }
  public ExpeditionTrack(ChallengeRank rank, int ticks = 0, string title = "", string description = "") : base(rank, ticks, title, description) { }
  public override string TrackCategory => "Expedition";
  public override string MarkAlertTitle => "Undertake an Expedition";
  public override string ResolveMoveName => "Finish an Expedition";
  public override bool CanRecommit => true;
  public override Tuple<MoveNumber, string>[] MoveReferences => new Tuple<MoveNumber, string>[] {
    new(MoveNumber.UndertakeAnExpedition,
      "When you trailblaze a route through perilous space, journey over hazardous terrain, or survey a mysterious site…"),
    new(MoveNumber.ExploreAWaypoint,
      "When you divert from an expedition to examine a notable location…"),
    new(MoveNumber.MakeADiscovery,
      "When your exploration of a waypoint uncovers something wondrous…"),
    new(MoveNumber.ConfrontChaos,
      "When your exploration of a waypoint uncovers something dreadful…"),
    new(MoveNumber.FinishAnExpedition,
      "When your expedition comes to an end…"),
    new(MoveNumber.SetACourse,
      "When you follow a known route through perilous space, across hazardous terrain, or within a mysterious site…") };
}