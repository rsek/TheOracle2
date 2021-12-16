﻿using OracleData;

namespace TheOracle2.DataClasses;

//Todo: convert the classes in this file to records that should be.

public class AddTemplate
{
    [JsonIgnore]
    public int Id { get; set; }

    public Attributes Attributes { get; set; }

    [JsonPropertyName("Template type")]
    public string Templatetype { get; set; }
}

public partial class Attributes
{
    [JsonIgnore]
    public int Id { get; set; }

    [JsonPropertyName("Derelict Type")]
    public string DerelictType { get; set; }

    [JsonConverter(typeof(SingleOrArrayConverter<string>))]
    public IList<string> Location { get; set; }
}

public partial class ChanceTable
{
    [JsonIgnore]
    public int Id { get; set; }

    [JsonPropertyName("Add template")]
    public AddTemplate Addtemplate { get; set; }

    public IList<string> Assets { get; set; }
    public int Chance { get; set; }
    public string Description { get; set; }
    public string Details { get; set; }

    [JsonPropertyName("Game object")]
    public GameObject Gameobject { get; set; }

    [JsonPropertyName("Multiple rolls")]
    public MultipleRolls Multiplerolls { get; set; }

    public List<Oracle> Oracles { get; set; }
    public List<Suggest> Suggest { get; set; }
    public string Thumbnail { get; set; }
    public int Value { get; set; }
}

public partial class GameObject
{
    [JsonIgnore]
    public int Id { get; set; }

    public int Amount { get; set; }
    public Attributes Attributes { get; set; }

    [JsonPropertyName("Object type")]
    public string Objecttype { get; set; }
}

public class Inherit
{
    [JsonIgnore]
    public int Id { get; set; }

    public string Category { get; set; }
    public IList<string> Exclude { get; set; }
    public IList<string> Name { get; set; }
    public Requires Requires { get; set; }
}

public class MultipleRolls
{
    [JsonIgnore]
    public int Id { get; set; }

    [JsonPropertyName("Allow duplicates")]
    public bool Allowduplicates { get; set; }

    public int Amount { get; set; }
}

public partial class Oracle
{
    [JsonIgnore]
    public int Id { get; set; }

    public IList<string> Aliases { get; set; }

    [JsonPropertyName("Allow duplicate rolls")]
    public bool AllowDuplicateRolls { get; set; }

    public string Category { get; set; }
    public string Description { get; set; }

    [JsonPropertyName("Display name")]
    public string DisplayName { get; set; }

    public bool Initial { get; set; }

    [JsonPropertyName("Max rolls")]
    public int Maxrolls { get; set; }

    [JsonPropertyName("Min rolls")]
    public int Minrolls { get; set; }

    public string Name { get; set; }

    [JsonPropertyName("Oracle type")]
    public string OracleType { get; set; }

    public bool Repeatable { get; set; }
    public Requires Requires { get; set; }

    [JsonPropertyName("Select table by")]
    public string SelectTableBy { get; set; }

    public string Subgroup { get; set; }
    public List<ChanceTable> Table { get; set; }
    public List<Tables> Tables { get; set; }

    [JsonPropertyName("Use with")]
    public List<UseWith> UseWith { get; set; }

    [JsonPropertyName("Part of speech")]
    public IList<string> PartOfSpeech { get; set; }

    [JsonPropertyName("Content tags")]
    public IList<string> ContentTags { get; set; }

    public string Group { get; set; }
}

public class OracleInfo
{
    public OracleInfo()
    {
    }

    [JsonIgnore]
    public int Id { get; set; }

    public IList<string> Aliases { get; set; }
    public string Description { get; set; }

    [JsonPropertyName("Display name")]
    public string DisplayName { get; set; }

    public List<Inherit> Inherits { get; set; }
    public string Name { get; set; }
    public List<Oracle> Oracles { get; set; }
    public Source Source { get; set; }
    public List<Subcategory> Subcategories { get; set; }
    public IList<string> Tags { get; set; }
}

public partial class Requires
{
    [JsonIgnore]
    public int Id { get; set; }

    [JsonPropertyName("Derelict Type")]
    public IList<string> DerelictType { get; set; }

    public IList<string> Environment { get; set; }
    public IList<string> Life { get; set; }
    public IList<string> Location { get; set; }

    [JsonPropertyName("Planetary Class")]
    public IList<string> PlanetaryClass { get; set; }

    public IList<string> Region { get; set; }
    public IList<string> Scale { get; set; }

    [JsonPropertyName("Starship Type")]
    public IList<string> StarshipType { get; set; }

    [JsonPropertyName("Theme Type")]
    public IList<string> ThemeType { get; set; }

    public IList<string> Type { get; set; }

    public IList<string> Zone { get; set; }
}

public class Rootobject
{
    [JsonIgnore]
    public int Id { get; set; }

    public List<OracleInfo> OracleInfos { get; set; }
}

public class Subcategory
{
    [JsonIgnore]
    public int Id { get; set; }

    public IList<string> Aliases { get; set; }
    public string Category { get; set; }
    public string Description { get; set; }

    [JsonPropertyName("Display name")]
    public string Displayname { get; set; }

    public List<Inherit> Inherits { get; set; }
    public string Name { get; set; }
    public List<Oracle> Oracles { get; set; }
    public Requires Requires { get; set; }

    [JsonPropertyName("Sample Names")]
    public IList<string> SampleNames { get; set; }

    public Source Source { get; set; }
    public string Thumbnail { get; set; }
}

public class Suggest
{
    [JsonIgnore]
    public int Id { get; set; }

    [JsonPropertyName("Game object")]
    public GameObject Gameobject { get; set; }

    public List<Oracle> Oracles { get; set; }
}

public class Tables
{
    [JsonIgnore]
    public int Id { get; set; }

    public IList<string> Aliases { get; set; }

    [JsonPropertyName("Display name")]
    public string Displayname { get; set; }

    public string Name { get; set; }
    public Requires Requires { get; set; }
    public List<ChanceTable> Table { get; set; }
}

public class UseWith
{
    [JsonIgnore]
    public int Id { get; set; }

    public string Category { get; set; }
    public string Name { get; set; }
    public string Group { get; set; }
}
