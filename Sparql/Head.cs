using System.Text.Json.Serialization;

namespace WikiDataHistory.Sparql
{
    public partial class Head
    {
        [JsonPropertyName("vars")]
        public string[] Vars { get; set; }
    }
}
