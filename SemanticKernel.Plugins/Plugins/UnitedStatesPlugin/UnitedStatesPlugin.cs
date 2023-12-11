using Microsoft.SemanticKernel;
using SemanticKernel.Plugins.Models;
using System.ComponentModel;
using System.Net.Http.Json;

namespace SemanticKernel.Plugins.Plugins.UnitedStatesPlugin
{
    public class UnitedStatesPlugin
    {
        [KernelFunction, Description("Get the United States population for a specific year")]
        public async Task<string> GetPopulation([Description("The year")] int year)
        {
            string request = "https://datausa.io/api/data?drilldowns=Nation&measures=Population";
            HttpClient client = new HttpClient();
            var result = await client.GetFromJsonAsync<UnitedStatesResult>(request);
            var populationData = result.data.FirstOrDefault(x => x.Year == year.ToString());
            string response = $"The population number in the United States in {year} was {populationData.Population}";
            return response;
        }

        [KernelFunction, Description("Get the United States population who identifies as male for a specific year")]
        public async Task<string> GetMalePopulation([Description("The year")] int year)
        {
            string request = "https://datausa.io/api/data?drilldowns=Year,Gender&measures=Total+Population";
            HttpClient client = new HttpClient();
            var result = await client.GetFromJsonAsync<GenderResult>(request);
            var populationData = result.data.FirstOrDefault(x => x.Year == year.ToString() && x.Gender == "Male");
            string response = $"The population number in the United States in {year} who identifies as male was {populationData.TotalPopulation}";
            return response;
        }
    }
}
