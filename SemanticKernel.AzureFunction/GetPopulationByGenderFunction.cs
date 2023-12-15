using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using SemanticKernel.AzureFunction.Models;
using System.Net;
using System.Net.Http.Json;

namespace SemanticKernel.AzureFunction
{
    public class GetPopulationByGenderFunction
    {
        private readonly ILogger<GetPopulationByGenderFunction> _logger;

        public GetPopulationByGenderFunction(ILogger<GetPopulationByGenderFunction> log)
        {
            _logger = log;
        }

        [Function("GetPopulationByGender")]
        [OpenApiOperation(operationId: "GetPopulationByGender", tags: new[] { "year" }, Description = "Get the United States population for a specific year who identifies themselves with a specific gender")]
        [OpenApiParameter(name: "year", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The year")]
        [OpenApiParameter(name: "gender", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The gender")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(UnitedStatesResponse), Description = "The population number by gender")]
        public async Task<HttpResponseData> Run(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequestData req, [FromQuery] string year, [FromQuery] string gender)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string request = "https://datausa.io/api/data?drilldowns=Year,Gender&measures=Total+Population";
            HttpClient client = new HttpClient();
            var result = await client.GetFromJsonAsync<GenderResult>(request);
            var populationData = result.data.FirstOrDefault(x => x.Year == year.ToString() && x.Gender.ToLower() == gender);

            var jsonResponse = new UnitedStatesResponse
            {
                Gender = gender,
                TotalNumber = populationData.TotalPopulation,
                Year = year
            };

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(jsonResponse);

            return response;
        }
    }
}
