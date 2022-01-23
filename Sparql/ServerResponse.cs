using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace WikiDataHistory.Sparql
{
    partial class ServerResponse
    {
        [JsonPropertyName("head")]
        public Head Head { get; set; }

        [JsonPropertyName("results")]
        public Results Results { get; set; }
    }
}
