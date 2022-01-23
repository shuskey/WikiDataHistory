using System.Text.Json.Serialization;

namespace WikiDataHistory.Sparql
{
    public partial class Binding
    {
        [JsonPropertyName("year")]
        public Entity Year { get; set; }

        [JsonPropertyName("linkCount")]
        public Entity LinkCount { get; set; }

        [JsonPropertyName("item")]
        public Entity Item { get; set; }

        [JsonPropertyName("itemLabel")]
        public Label ItemLabel { get; set; }

        [JsonPropertyName("picture")]
        public Label Picture { get; set; }

        [JsonPropertyName("wikiLink")]
        public Label WikiLink { get; set; }

        [JsonPropertyName("description")]
        public Label Description { get; set; }

        [JsonPropertyName("aliases")]
        public Label Aliases { get; set; }

        [JsonPropertyName("locations")]
        public Label Locations { get; set; }

        [JsonPropertyName("countries")]
        public Label Countries { get; set; }

        [JsonPropertyName("pointInTime")]
        public Label PointInTime { get; set; }

        [JsonPropertyName("eventStartDate")]
        public Label EventStartDate { get; set; }

        [JsonPropertyName("eventEndDate")]
        public Label EventEndDate { get; set; }
    }
}
