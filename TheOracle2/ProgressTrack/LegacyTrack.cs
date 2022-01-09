namespace TheOracle2.GameObjects;

public class LegacyTrack : ProgressTrack
{
  public LegacyTrack(LegacyType legacyType) : base(ChallengeRank.None) { }
  public enum LegacyType { Quests, Discoveries, Bonds }
}