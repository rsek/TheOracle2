namespace TheOracle2;

/// <summary>
/// Interface for entities that are represented by a single embed field.
/// </summary>
internal interface IDiscordEntityField
{
    public EmbedFieldBuilder ToEmbedField();
}
