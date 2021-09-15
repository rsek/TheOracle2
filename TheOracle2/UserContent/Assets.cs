﻿//----------------------
// <auto-generated>
//     Generated using the NJsonSchema v10.5.2.0 (Newtonsoft.Json v12.0.0.0) (http://NJsonSchema.org)
// </auto-generated>
//----------------------

using System.Collections.Generic;
using TheOracle2.UserContent;

namespace OracleData
{
#pragma warning disable // Disable all warnings

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.5.2.0 (Newtonsoft.Json v12.0.0.0)")]
    public record Ability
    {
        [Newtonsoft.Json.JsonProperty("Alter Properties", Required = Newtonsoft.Json.Required.Default)]
        public AlterProperties Alter_Properties { get; set; }

        [Newtonsoft.Json.JsonProperty("Enabled")]
        public bool Enabled { get; set; }

        [Newtonsoft.Json.JsonProperty("Fields")]
        public System.Collections.Generic.ICollection<string> Fields { get; set; }

        [Newtonsoft.Json.JsonProperty("Text")]
        public string Text { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties = new System.Collections.Generic.Dictionary<string, object>();

        [Newtonsoft.Json.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties; }
            set { _additionalProperties = value; }
        }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.5.2.0 (Newtonsoft.Json v12.0.0.0)")]
    public record AlterProperties
    {
        [Newtonsoft.Json.JsonProperty("Track")]
        public Track Track { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties = new System.Collections.Generic.Dictionary<string, object>();

        [Newtonsoft.Json.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties; }
            set { _additionalProperties = value; }
        }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.5.2.0 (Newtonsoft.Json v12.0.0.0)")]
    public record Asset
    {
        public int AssetID { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public ICollection<OracleGuild> OracleGuilds { get; set; } = new List<OracleGuild>();

        [Newtonsoft.Json.JsonProperty("Abilities")]
        public System.Collections.Generic.ICollection<Ability> Abilities { get; set; }

        [Newtonsoft.Json.JsonProperty("Category")]
        public string Category { get; set; }

        [Newtonsoft.Json.JsonProperty("Counter")]
        public Counter Counter { get; set; }

        [Newtonsoft.Json.JsonProperty("Description")]
        public string Description { get; set; }

        //[Newtonsoft.Json.JsonProperty("Fields")]
        //public System.Collections.Generic.ICollection<string> Fields { get; set; }

        [Newtonsoft.Json.JsonProperty("Name")]
        public string Name { get; set; }

        [Newtonsoft.Json.JsonProperty("Track")]
        public Track Track { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties = new System.Collections.Generic.Dictionary<string, object>();

        [Newtonsoft.Json.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties; }
            set { _additionalProperties = value; }
        }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.5.2.0 (Newtonsoft.Json v12.0.0.0)")]
    public record Counter
    {
        [Newtonsoft.Json.JsonProperty("Name")]
        public string Name { get; set; }

        [Newtonsoft.Json.JsonProperty("Starts At")]
        public int Starts_At { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties = new System.Collections.Generic.Dictionary<string, object>();

        [Newtonsoft.Json.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties; }
            set { _additionalProperties = value; }
        }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.5.2.0 (Newtonsoft.Json v12.0.0.0)")]
    public record Source
    {
        [Newtonsoft.Json.JsonProperty("Name")]
        public string Name { get; set; }

        [Newtonsoft.Json.JsonProperty("Page")]
        public string Page { get; set; }

        [Newtonsoft.Json.JsonProperty("Date", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Date { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties = new System.Collections.Generic.Dictionary<string, object>();

        [Newtonsoft.Json.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties; }
            set { _additionalProperties = value; }
        }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.5.2.0 (Newtonsoft.Json v12.0.0.0)")]
    public record Track
    {
        [Newtonsoft.Json.JsonProperty("Name")]
        public string Name { get; set; }

        [Newtonsoft.Json.JsonProperty("Starts At", Required = Newtonsoft.Json.Required.Always)]
        public int Starts_At { get; set; }

        [Newtonsoft.Json.JsonProperty("Value", Required = Newtonsoft.Json.Required.Always)]
        public int Value { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties = new System.Collections.Generic.Dictionary<string, object>();

        [Newtonsoft.Json.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties; }
            set { _additionalProperties = value; }
        }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.5.2.0 (Newtonsoft.Json v12.0.0.0)")]
    public record AssetRoot
    {
        [Newtonsoft.Json.JsonProperty("Assets")]
        public System.Collections.Generic.ICollection<Asset> Assets { get; set; }

        [Newtonsoft.Json.JsonProperty("Name")]
        public string Name { get; set; }

        [Newtonsoft.Json.JsonProperty("Source")]
        public Source Source { get; set; }

        [Newtonsoft.Json.JsonProperty("Tags")]
        public System.Collections.Generic.ICollection<string> Tags { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties = new System.Collections.Generic.Dictionary<string, object>();

        [Newtonsoft.Json.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties; }
            set { _additionalProperties = value; }
        }


    }
}