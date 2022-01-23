using System.Text.Json.Serialization;

namespace WikiDataHistory.Sparql
{
    public partial class Entity
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }
        
        [JsonPropertyName("value")]
        public string Value { get; set; }
    }
}
