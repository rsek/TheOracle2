namespace TheOracle2.GameObjects;
public class GenericTrack : ProgressTrack
{
  public GenericTrack(Embed embed) : base(embed) { }
  public GenericTrack(Embed embed, int ticks) : base(embed, ticks) { }
  public GenericTrack(ChallengeRank rank, int ticks = 0, string title = "", string description = "") : base(rank, ticks, title, description)
  { }
  public override string EmbedCategory => "Progress Track";
  public override bool CanRecommit => false;
  public override string TrackCategory => "Progress Track";
  public override Tuple<MoveNumber, string>[] MoveReferences => Array.Empty<Tuple<MoveNumber, string>>();
}