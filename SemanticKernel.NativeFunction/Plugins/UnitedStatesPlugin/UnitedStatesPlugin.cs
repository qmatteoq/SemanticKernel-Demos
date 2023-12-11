using Microsoft.SemanticKernel;
using SemanticKernel.NativeFunction.Models;
using System.ComponentModel;
using System.Net.Http.Json;

namespace SemanticKernel.NativeFunction.Plugins.UnitedStatesPlugin
{
    public class UnitedStatesPlugin
    {
        [KernelFunction, Description("Get the United States population for a specific year")]
        public async Task<string> GetPopulation([Description("The year")]int year)
        {
            string request = "https://datausa.io/api/data?drilldowns=Nation&measures=Population";
            HttpClient client = new HttpClient();
            var result = await client.GetFromJsonAsync<UnitedStatesResult>(request);
            var populationData = result.data.FirstOrDefault(x => x.Year == year.ToString());
            string response = $"The population number in the United States in {year} was {populationData.Population}";
            return response;
        }
    }
}
