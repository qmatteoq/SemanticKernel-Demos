using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using SemanticKernel.AzureFunction.Models;
using UnitedStatesDataFunction.Models;

namespace SemanticKernel.AzureFunction
{
    public class GetPopulationFunction
    {
        private readonly ILogger<GetPopulationFunction> _logger;

        public GetPopulationFunction(ILogger<GetPopulationFunction> log)
        {
            _logger = log;
        }

        [Function("GetPopulation")]
        [OpenApiOperation(operationId: "GetPopulation", tags: new[] { "year" }, Description = "Get the United States population for a specific year")]
        [OpenApiParameter(name: "year", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The year")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(UnitedStatesResponse), Description = "The population number")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequestData req, [FromQuery] string year)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string request = "https://datausa.io/api/data?drilldowns=Nation&measures=Population";
            HttpClient client = new HttpClient();
            var result = await client.GetFromJsonAsync<UnitedStatesResult>(request);
            var populationData = result.data.FirstOrDefault(x => x.Year == year);

            var jsonResponse = new UnitedStatesResponse
            {
                Gender = null,
                TotalNumber = populationData.Population,
                Year = year
            };

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(jsonResponse);

            return response;
        }
    }
}

