using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SemanticKernel.Plugins.Models
{
    public class UnitedStatesResponse
    {
        [JsonPropertyName("year")]
        public string Year { get; set; }

        [JsonPropertyName("totalNumber")]
        public int TotalNumber { get; set; }

        [JsonPropertyName("gender")]
        public string? Gender { get; set; }
    }
}
