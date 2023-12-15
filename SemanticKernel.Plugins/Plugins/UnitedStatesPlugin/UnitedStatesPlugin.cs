using Microsoft.SemanticKernel;
using SemanticKernel.Plugins.Models;
using System.ComponentModel;
using System.Net.Http.Json;

namespace SemanticKernel.Plugins.Plugins.UnitedStatesPlugin
{
    public class UnitedStatesPlugin
    {
        [KernelFunction, Description("Get the United States population for a specific year")]
        public async Task<UnitedStatesResponse> GetPopulation([Description("The year")] string year)
        {
            string request = "https://datausa.io/api/data?drilldowns=Nation&measures=Population";
            HttpClient client = new HttpClient();
            var result = await client.GetFromJsonAsync<UnitedStatesResult>(request);
            var populationData = result.data.FirstOrDefault(x => x.Year == year);

            var response = new UnitedStatesResponse
            {
                Gender = null,
                TotalNumber = populationData.Population,
                Year = year
            };

            return response;
        }

        [KernelFunction, Description("Get the United States population who identifies with a specific gender in a given year")]
        public async Task<UnitedStatesResponse> GetPopulationByGender([Description("The year")] string year, [Description("The gender")]string gender)
        {
            string request = "https://datausa.io/api/data?drilldowns=Year,Gender&measures=Total+Population";
            HttpClient client = new HttpClient();
            var result = await client.GetFromJsonAsync<GenderResult>(request);
            var populationData = result.data.FirstOrDefault(x => x.Year == year && x.Gender.ToLower() == gender);

            var response = new UnitedStatesResponse
            {
                Gender = gender,
                TotalNumber = populationData.TotalPopulation,
                Year = year
            };

            return response;
        }
    }
}
