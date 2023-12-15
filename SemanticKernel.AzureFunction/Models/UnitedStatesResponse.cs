using System.Text.Json.Serialization;

namespace SemanticKernel.AzureFunction.Models
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
