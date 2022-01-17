using TheOracle2.UserContent;
namespace TheOracle2.GameObjects;
using System.Text.Json;
public class VowTrack : ProgressTrack
{
  public VowTrack(Embed embed) : base(embed) { }
  public VowTrack(Embed embed, int ticks) : base(embed, ticks) { }
  public VowTrack(ChallengeRank rank, int ticks = 0, string title = "", string description = "") : base(rank, ticks, title, description) { }
  public override string TrackCategory => "Vow";
  public override string ResolveMoveName => "Fulfill Your Vow";
  public override string MarkAlertTitle => "Reach a Milestone";
  public override bool CanRecommit => true;
  public override Tuple<MoveNumber, string>[] MoveReferences => new Tuple<MoveNumber, string>[] {
    new(MoveNumber.SwearAnIronVow,
    "When you swear upon iron to complete a quest…"),
    new(MoveNumber.ReachAMilestone,
    "When you make headway in your quest…"),
    new(MoveNumber.FulfillYourVow,
    "When you reach the end of your quest…"),
    new(MoveNumber.ForsakeYourVow,
    "When you renounce your quest, betray your promise, or the goal is lost to you…") };
}