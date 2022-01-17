namespace TheOracle2.GameObjects;

public class ConnectionTrack : ProgressTrack
{
  public ConnectionTrack(ChallengeRank rank, int ticks = 0, string title = "", string description = "") : base(rank, ticks, title, description) { }
  public ConnectionTrack(Embed embed) : base(embed) { }
  public ConnectionTrack(Embed embed, int ticks) : base(embed, ticks) { }

  public override string MarkAlertTitle => "Develop Your Relationship";
  public override bool CanRecommit => true;
  public override string ResolveMoveName => "Forge a Bond";
  public override string TrackCategory => "Connection";
  public override Tuple<MoveNumber, string>[] MoveReferences => new Tuple<MoveNumber, string>[] {
    new(MoveNumber.MakeAConnection,
    "When you search out a new relationship or give focus to an existing relationship (not an ally or companion)…"),
    new(MoveNumber.DevelopYourRelationship,
    "When you reinforce your relationship with a connection…"),
    new(MoveNumber.TestYourRelationship,
    "When your relationship with a connection is tested through conflict, betrayal, or circumstance…"),
    new(MoveNumber.ForgeABond,
    "When your relationship with a connection is ready to evolve…") };
}