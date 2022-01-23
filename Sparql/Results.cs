using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace WikiDataHistory.Sparql
{
    public partial class Results
    {
        [JsonPropertyName("bindings")]
        public Binding[] Bindings { get; set; }
    }
}
