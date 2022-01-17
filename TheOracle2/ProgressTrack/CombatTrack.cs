namespace TheOracle2.GameObjects;

public class CombatTrack : ProgressTrack
{
  public CombatTrack(Embed embed) : base(embed) { }
  public CombatTrack(Embed embed, int ticks) : base(embed, ticks) { }
  public CombatTrack(ChallengeRank rank, int ticks = 0, string title = "", string description = "") : base(rank, ticks, title, description)
  { }
  public override string TrackCategory => "Combat Objective";
  public override bool CanRecommit => false;
  public override string ResolveMoveName => "Take Decisive Action";
  public override Tuple<MoveNumber, string>[] MoveReferences => new Tuple<MoveNumber, string>[] {
    new(MoveNumber.EnterTheFray,
      "When you initiate combat or are forced into a fight…"),
    new(MoveNumber.GainGround,
      "When you are in control and take action in a fight to reinforce your position or move toward an objective…"),
    new(MoveNumber.ReactUnderFire,
      "When you are in a bad spot and take action in a fight to avoid danger or overcome an obstacle…"),
    new(MoveNumber.Strike,
      "When you are in control and assault a foe at close quarters, or when you attack at a distance…"),
    new(MoveNumber.Clash,
      "When you are in a bad spot and fight back against a foe at close quarters, or when you exchange fire at a distance…"),
    new(MoveNumber.TakeDecisiveAction,
      "When you seize an objective in a fight…"),
    new(MoveNumber.FaceDefeat,
      "When you abandon or are deprived of an objective…"),
    new(MoveNumber.Battle,
      "When you fight a battle and it happens in a blur…"),};
}