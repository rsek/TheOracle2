﻿using System.ComponentModel.DataAnnotations.Schema;
using TheOracle2.DataClasses;
using TheOracle2.UserContent;

namespace OracleData;

[System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.5.2.0 (Newtonsoft.Json v12.0.0.0)")]
public record Ability
{
    [JsonIgnore]
    public int Id { get; set; }

    [JsonPropertyName("Alter Moves")]
    public IList<AlterMove> AlterMoves { get; set; }

    [JsonPropertyName("Alter Properties")]
    public AlterProperties AlterProperties { get; set; }

    [JsonPropertyName("Counter")]
    public Counter Counter { get; set; }

    [JsonPropertyName("Enabled")]
    public bool Enabled { get; set; }

    [JsonPropertyName("Input")]
    //[NotMapped]
    public System.Collections.Generic.IList<string> Input { get; set; }

    public Move Move { get; set; }

    [JsonPropertyName("Text")]
    public string Text { get; set; }
}

[System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.5.2.0 (Newtonsoft.Json v12.0.0.0)")]
public record AlterProperties
{
    [JsonIgnore]
    public int Id { get; set; }
    [JsonPropertyName("Condition Meter")]
    public ConditionMeter ConditionMeter { get; set; }

    [JsonPropertyName("Track")]
    public Track Track { get; set; }
}

[System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.5.2.0 (Newtonsoft.Json v12.0.0.0)")]
public record Asset
{
    [JsonIgnore]
    public int Id { get; set; }

    [JsonIgnore]
    public IList<OracleGuild> OracleGuilds { get; set; } = new List<OracleGuild>();

    [JsonPropertyName("Abilities")]
    public System.Collections.Generic.IList<Ability> Abilities { get; set; }

    [JsonPropertyName("Aliases")]
    public System.Collections.Generic.IList<string> Aliases { get; set; }

    [JsonPropertyName("Category")]
    public string Category { get; set; }

    [JsonPropertyName("Condition Meter")]
    public ConditionMeter ConditionMeter { get; set; }

    [JsonPropertyName("Counter")]
    public Counter Counter { get; set; }

    [JsonPropertyName("Description")]
    public string Description { get; set; }

    [JsonPropertyName("Input")]
    [NotMapped]
    public System.Collections.Generic.IList<string> Input { get; set; }

    [JsonPropertyName("Name")]
    public string Name { get; set; }

    [JsonPropertyName("Select")]
    public Select Select { get; set; }

    [JsonPropertyName("Track")]
    public Track Track { get; set; }
}

public record ConditionMeter
{
    [JsonIgnore]
    public int Id { get; set; }
    public string Name { get; set; }
    public int Max { get; set; }

    [JsonPropertyName("Starts At")]
    public int StartsAt { get; set; }
}

[System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.5.2.0 (Newtonsoft.Json v12.0.0.0)")]
public record Counter
{
    [JsonIgnore]
    public int Id { get; set; }
    [JsonPropertyName("Name")]
    public string Name { get; set; }

    [JsonPropertyName("Starts At")]
    public int StartsAt { get; set; }

    [JsonPropertyName("Max")]
    public int Max { get; set; }
}

[System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.5.2.0 (Newtonsoft.Json v12.0.0.0)")]
public record Source
{
    [JsonIgnore]
    public int Id { get; set; }
    [JsonPropertyName("Name")]
    public string Name { get; set; }

    [JsonPropertyName("Page")]
    public string Page { get; set; }

    [JsonPropertyName("Date")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string Date { get; set; }
}

[System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.5.2.0 (Newtonsoft.Json v12.0.0.0)")]
public record Track
{
    [JsonIgnore]
    public int Id { get; set; }
    [JsonPropertyName("Name")]
    public string Name { get; set; }

    [JsonPropertyName("Starts At")]
    public int StartsAt { get; set; }

    [JsonPropertyName("Value")]
    public int Value { get; set; }
}

[System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.5.2.0 (Newtonsoft.Json v12.0.0.0)")]
public record AssetRoot
{
    [JsonIgnore]
    public int Id { get; set; }
    [JsonPropertyName("Assets")]
    public System.Collections.Generic.IList<Asset> Assets { get; set; }

    [JsonPropertyName("Name")]
    public string Name { get; set; }

    [JsonPropertyName("Source")]
    public Source Source { get; set; }

    //Todo Get rsek to convert these to tags with IDs?
    [JsonPropertyName("Tags")]
    public System.Collections.Generic.IList<string> Tags { get; set; }
}

public record AlterMove
{
    [JsonIgnore]
    public int Id { get; set; }
    [JsonPropertyName("Any Move")]
    public bool AnyMove { get; set; }
    public string Name { get; set; }
    public IList<Trigger> Triggers { get; set; }
}

public record Select
{
    [JsonIgnore]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public IList<string> Options { get; set; }
}