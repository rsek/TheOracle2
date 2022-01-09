namespace TheOracle2;

public class RankData
{
  public RankData(ChallengeRank rank, int markTrack, int markLegacy, int? suffer = null)
  {
    Name = rank.ToString();
    Value = (int)rank;
    MarkTrack = markTrack;
    MarkLegacy = markLegacy;
    Suffer = suffer;
  }
  public string Name { get; set; }
  public int Value { get; set; }
  public int MarkTrack { get; set; }
  public int MarkLegacy { get; set; }
  public int? Suffer { get; set; }
  public ChallengeRank ToEnumValue()
  {
    return (ChallengeRank)Value;
  }
}
public class Rank : Dictionary<ChallengeRank, RankData>
{
  public Rank()
  {
    Add((ChallengeRank)0, new RankData(rank: (ChallengeRank)0, markTrack: 0, markLegacy: 0, suffer: 0));
    Add((ChallengeRank)1, new RankData(rank: (ChallengeRank)1, markTrack: 12, markLegacy: 1, suffer: 1));
    Add((ChallengeRank)2, new RankData(rank: (ChallengeRank)2, markTrack: 8, markLegacy: 2, suffer: 2));
    Add((ChallengeRank)3, new RankData(rank: (ChallengeRank)3, markTrack: 4, markLegacy: 4, suffer: 2));
    Add((ChallengeRank)4, new RankData(rank: (ChallengeRank)4, markTrack: 2, markLegacy: 8, suffer: 3));
    Add((ChallengeRank)5, new RankData(rank: (ChallengeRank)5, markTrack: 1, markLegacy: 12, suffer: 3));
  }
}

public enum ChallengeRank
{
  None = 0, // for Legacy Tracks, Troublesome quests granting less reward, etc
  Troublesome = 1,
  Dangerous = 2,
  Formidable = 3,
  Extreme = 4,
  Epic = 5
}