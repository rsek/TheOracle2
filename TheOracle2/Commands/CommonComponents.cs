using Discord.Interactions;
using Discord.WebSocket;
using TheOracle2.GameObjects;
namespace TheOracle2;


/// <summary>
/// Progress and clock message components that are used by multiple slash commands.
/// </summary>
public class CommonComponents : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
{
  private readonly Random Random;
  public CommonComponents(Random random)
  {
    Random = random;
  }
  [ComponentInteraction("progress-mark:*,*")]
  public async Task MarkProgress(string tickString, string currentString
  )
  {
    var ticksAdded = int.Parse(tickString);
    var ticksOld = int.Parse(currentString);
    var ticksNew = ticksOld + ticksAdded;
    var interaction = Context.Interaction as SocketMessageComponent;
    var progressTrack = IProgressTrack.FromEmbed(interaction.Message.Embeds.FirstOrDefault(), ticksNew);
    await interaction.UpdateAsync(msg =>
    {
      msg.Components = progressTrack.MakeComponents().Build();
      msg.Embed = progressTrack.ToEmbed().Build();
    });
  }
  [ComponentInteraction("progress-clear:*,*")]
  public async Task ClearProgress(string tickString, string currentString
  )
  {
    var ticksSubtracted = int.Parse(tickString);
    var ticksOld = int.Parse(currentString);
    var ticksNew = ticksOld - ticksSubtracted;
    var interaction = Context.Interaction as SocketMessageComponent;
    var progressTrack = IProgressTrack.FromEmbed(interaction.Message.Embeds.FirstOrDefault(), ticksNew);
    await interaction.UpdateAsync(msg =>
    {
      msg.Components = progressTrack.MakeComponents().Build();
      msg.Embed = progressTrack.ToEmbed().Build();
    });
  }
  [ComponentInteraction("progress-recommit:*,*")]
  public async Task RecommitProgress(string currentTicksString, string rankString)
  {
    var interaction = Context.Interaction as SocketMessageComponent;
    var currentTicks = int.Parse(currentTicksString);
    var rank = Enum.Parse<ChallengeRank>(rankString);
    var progressTrack = IProgressTrack.FromEmbed(interaction.Message.Embeds.FirstOrDefault(), currentTicks);
    IProgressTrack.Recommit recommit = new(Random, progressTrack.ToEmbed().Build());
    await interaction.UpdateAsync(msg =>
    {
      msg.Embed = recommit.NewTrack.ToEmbed().Build();
      msg.Components = recommit.NewTrack.MakeComponents().Build();
    });
    await interaction.FollowupAsync(embed: recommit.ToAlertEmbed().Build());
  }
  [ComponentInteraction("progress-roll:*")]
  public async Task RollProgress(string ticksString)
  {
    // TODO: doesn't need to touch the embed since progress is embedded in the button; a component refresh should be enough.
    var ticks = int.Parse(ticksString);
    var score = ITrack.GetScore(ticks);
    var interaction = Context.Interaction as SocketMessageComponent;
    var progressTrack = IProgressTrack.FromEmbed(interaction.Message.Embeds.FirstOrDefault(), ticks);
    await interaction.RespondAsync(embed: progressTrack.Roll(Random).ToEmbed().Build());
  }
  [ComponentInteraction("progress-menu:*,*")]
  public async Task ProgressMenu(string rankString, string currentTicks, string[] values)
  {
    string option = values.FirstOrDefault();
    var interaction = Context.Interaction as SocketMessageComponent;
    // IProgressTrack track = IProgressTrack.FromEmbed(interaction.Message.Embeds.FirstOrDefault(), ticks);
    string operation = option.Split(":")[0];
    string operationArg = option.Split(":")[1];
    switch (operation)
    {
      case "progress-clear":
        await ClearProgress(operationArg, currentTicks);
        return;
      case "progress-mark":
        await MarkProgress(operationArg, currentTicks);
        return;
      case "progress-roll":
        int score = int.Parse(currentTicks) / 4;
        await RollProgress(score.ToString());
        return;
      case "progress-recommit":
        await RecommitProgress(currentTicks, rankString);
        return;
    }
    // do an after interaction function to automatically resend the components?
    // await interaction.UpdateAsync(msg =>
    // {
    //   msg.Components = track.MakeComponents().Build();
    //   msg.Embed = track.ToEmbed().Build();
    // });
    return;
  }
  [ComponentInteraction("clock-reset")]
  public async Task ResetClock()
  {
    var interaction = Context.Interaction as SocketMessageComponent;
    var clock = IClock.FromEmbed(interaction.Message.Embeds.FirstOrDefault());
    await interaction.UpdateAsync(msg =>
  {
    clock.Filled = 0;
    msg.Components = clock.MakeComponents().Build();
    msg.Embed = clock.ToEmbed().Build();
  });
  }
  [ComponentInteraction("clock-advance")]
  public async Task AdvanceClock()
  {
    var interaction = Context.Interaction as SocketMessageComponent;
    var clock = IClock.FromEmbed(interaction.Message.Embeds.FirstOrDefault());
    await interaction.UpdateAsync(msg =>
    {
      clock.Filled++;
      msg.Components = clock.MakeComponents().Build();
      msg.Embed = clock.ToEmbed().Build();
    });
    await interaction.FollowupAsync(embed: clock.AlertEmbed().Build());
  }

  // TODO: refactor in same style as progress menu
  [ComponentInteraction("clock-menu:*,*")]
  public async Task ClockMenu(string filledString, string segmentsString, string[] values)
  {
    string optionValue = values.FirstOrDefault();
    var interaction = Context.Interaction as SocketMessageComponent;
    var clock = IClock.FromEmbed(interaction.Message.Embeds.FirstOrDefault());
    var fractionString = $"{clock.Filled}/{clock.Segments}";
    if (int.TryParse(optionValue.Replace("clock-advance:", ""), out int odds))
    {
      OracleAnswer answer = new(Random, odds, $"Does the clock *{clock.Title}* advance?");
      EmbedBuilder answerEmbed = answer.ToEmbed();
      if (answer.IsYes)
      {
        clock.Filled += answer.IsMatch ? 2 : 1;
        if (answer.IsMatch)
        {
          answerEmbed.WithFooter("You rolled a match! Envision how this situation or project gains dramatic support or inertia.");
        }
        string append = answer.IsMatch ? $"The clock advances **twice** to {fractionString}." : $"The clock advances to {fractionString}.";
        answerEmbed.Description += "\n" + append;
        answerEmbed = answerEmbed.WithThumbnailUrl(IClock.Images[clock.Segments][clock.Filled]);
      }
      if (!answer.IsYes)
      {
        if (answer.IsMatch)
        {
          answerEmbed = answerEmbed.WithFooter("You rolled a match! Envision a surprising turn of events which pits new factors or forces against the clock.");
        }
        answerEmbed.Description += "\n" + $"The clock remains at {fractionString}";
      }
      answerEmbed = answerEmbed
        .WithThumbnailUrl(IClock.Images[clock.Segments][clock.Filled])
        .WithColor(IClock.ColorRamp[clock.Segments][clock.Filled]);
      await interaction.UpdateAsync(msg =>
      {
        msg.Components = clock.MakeComponents().Build();
        msg.Embed = clock.ToEmbed().Build();
      });
      await interaction.FollowupAsync(embed: answerEmbed.Build());
      return;
    }
    switch (optionValue)
    {
      case "clock-reset":
        clock.Filled = 0;
        break;
      case "clock-advance":
        clock.Filled++;
        break;
      default:
        throw new ArgumentOutOfRangeException(nameof(optionValue), "Could not parse integer or valid string from select menu option value.");
    }
    await interaction.UpdateAsync(msg =>
    {
      msg.Components = clock.MakeComponents().Build();
      msg.Embed = clock.ToEmbed().Build();
    });
    if (optionValue != "clock-reset")
    {
      await interaction.FollowupAsync(embed: clock.AlertEmbed().Build());
    }
    return;
  }
  [ComponentInteraction("scene-challenge-menu:*,*,*,*")]
  public async Task SceneChallengeMenu(string ticksString, string rankString, string filledString, string segmentsString, string[] values)
  {
    string optionValue = values.FirstOrDefault();
    if (optionValue.StartsWith("progress"))
    {
      await ProgressMenu(rankString, ticksString, values);
      return;
    }
    if (optionValue.StartsWith("clock"))
    {
      await ClockMenu(filledString, segmentsString, values);
      return;
    }
  }
}
