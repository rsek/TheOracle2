namespace TheOracle2;
namespace TheOracle2.IronswornRoller;

public class ProgressRoll : IronswornRoll
{
  /// <summary>
  /// Makes a Progress Roll.
  /// </summary>
  /// <param name="progressScore">The player character's progress score.</param>
  /// <param name="text">A user-provided text string.</param>
  /// <param name="challengeDieValue1">A preset value for the first challenge die.</param>
  /// <param name="challengeDieValue2">A preset value for the second challenge die.</param>
  public ProgressRoll(Random random, int progressScore, string text = "", int? challengeDieValue1 = null, int? challengeDieValue2 = null) : base(random, text, challengeDieValue1, challengeDieValue2)
  {
    ProgressScore = progressScore;
  }

  /// <inheritdoc/>
  public override string RollTypeLabel { get => IronswornRollResources.ProgressRoll; }
  public override string ScoreLabel { get => IronswornRollResources.ProgressScore; }
  public int ProgressScore { get; set; }

  /// <inheritdoc/>
  public override int RawScore { get => ProgressScore; }
}