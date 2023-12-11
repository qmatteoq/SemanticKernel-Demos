using System.Text.Json.Serialization;

namespace SemanticKernel.Plugins.Models
{
    public class GenderResult
    {
        public GenderResults[] data { get; set; }
        public Source[] source { get; set; }
    }

    public class GenderResults
    {
        public int IDYear { get; set; }
        public string Year { get; set; }
        public int IDGender { get; set; }
        public string Gender { get; set; }

        [JsonPropertyName("Total Population")]
        public int TotalPopulation { get; set; }
    }

}
