using System.Text.Json.Serialization;

namespace WikiDataHistory.Sparql
{
    public partial class Label
    {

        [JsonPropertyName("xml:lang")]
        public string XmlLang { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }
    }
}
