using Discord.Interactions;
using Discord.WebSocket;
using TheOracle2.GameObjects;
namespace TheOracle2;

[Group("track", "Creates an interactive progress track for vows, bonds, expeditions, combat, and scene challenges.")]
public class ProgressTrackerCommand : InteractionModuleBase
{
  private readonly Random Random;
  public ProgressTrackerCommand(Random random)
  {
    Random = random;
  }

  [SlashCommand("generic", "Create a generic progress track")]
  public async Task BuildProgressTrack(
    [Summary(description: "A title for the progress track.")]
    string title,
    [Summary(description: "The challenge rank of the progress track.")]
    ChallengeRank rank,
    [Summary(description: "An optional description.")]
    string description="",
    [Summary(description: "A score to pre-set the track, if desired.")][MinValue(0)][MaxValue(10)]
    int score = 0
  )
  {
    GenericTrack track = new(rank: rank, ticks: score * ITrack.BoxSize, title: title, description: description);
    await RespondAsync(embed: track.ToEmbed().Build(), components: track.MakeComponents().Build());
  }
  [SlashCommand("scene-challenge", "Create a scene challenge for extended non-combat scenes against threats or other characters (p. 235)")]
  public async Task BuildSceneChallenge(
    [Summary(description: "The scene challenge's objective.")]
    string title,
    [Summary(description: "The number of clock segments. Default = 6, severe disadvantage = 4, strong advantage = 8.")]
    SceneChallengeClockSize segments=SceneChallengeClockSize.Six,
    [Summary(description: "An optional description.")]
    string description = "",
    [Summary(description: "A score to pre-set the track, if desired.")] [MinValue(0)] [MaxValue(10)]
    int score = 0)

  {
    // intentionally the same as /clock scene-challenge
    // because it has both a clock and a progress track.
    SceneChallenge sceneChallenge = new(segments: segments, filledSegments: 0, ticks: score * ITrack.BoxSize, title: title, description: description);
    EmbedBuilder embed = sceneChallenge.ToEmbed();
    ComponentBuilder components = sceneChallenge.MakeComponents();
    await RespondAsync(
      embed: embed.Build(),
      components: components.Build()

      );
  }
  [SlashCommand("expedition", "Create an expedition progress track for the Undertake an Expedition move.")]
  public async Task BuildExpeditionTrack(
  [Summary(description: "The expedition's name.")]
    string title,
  [Summary(description: "The challenge rank of the progress track.")]
    ChallengeRank rank,
  [Summary(description: "An optional description.")]
    string description="",
  [Summary(description: "A score to pre-set the track, if desired.")]
  [MinValue(0)][MaxValue(10)]
    int score = 0
    )
  {
    ExpeditionTrack track = new(rank: rank, ticks: score * ITrack.BoxSize, title: title, description: description);
    await RespondAsync(embed: track.ToEmbed().Build(), components: track.MakeComponents().Build());
  }
  [SlashCommand("vow", "Create a vow progress track for the Swear an Iron Vow move.")]
  public async Task BuildVowTrack(
    [Summary(description: "The vow's objective.")]
    string title,
    [Summary(description: "The challenge rank of the progress track.")]
    ChallengeRank rank,
    [Summary(description: "An optional description.")]
    string description="",
    [Summary(description: "A score to pre-set the track, if desired.")]
    [MinValue(0)][MaxValue(10)]
    int score = 0
  )
  {
    VowTrack track = new(rank: rank, ticks: score * ITrack.BoxSize, title: title, description: description);
    await RespondAsync(embed: track.ToEmbed().Build(), components: track.MakeComponents().Build());
  }
  public async Task BuildCombatTrack(
  [Summary(description: "The combat objective.")]
    string title,
  [Summary(description: "The challenge rank of the progress track.")]
    ChallengeRank rank,
  [Summary(description: "An optional description.")]
    string description="",
  [Summary(description: "A score to pre-set the track, if desired.")]
    [MinValue(0)][MaxValue(10)]
    int score = 0
)
  {
    CombatTrack track = new(rank: rank, ticks: score * ITrack.BoxSize, title: title, description: description);
    await RespondAsync(embed: track.ToEmbed().Build(), components: track.MakeComponents().Build());
  }
}

