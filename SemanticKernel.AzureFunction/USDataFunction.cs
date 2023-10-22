using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using UnitedStatesDataFunction.Models;

namespace UnitedStatesDataFunction
{
    public class USDataFunction
    {
        private readonly ILogger<USDataFunction> _logger;

        public USDataFunction(ILogger<USDataFunction> log)
        {
            _logger = log;
        }

        [FunctionName("GetPopulation")]
        [OpenApiOperation(operationId: "GetPopulation", tags: new[] { "year" }, Description = "Get the United States population for a specific year")]
        [OpenApiParameter(name: "year", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The year")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The population number")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string year = req.Query["year"];

            string request = "https://datausa.io/api/data?drilldowns=Nation&measures=Population";
            HttpClient client = new HttpClient();
            var result = await client.GetFromJsonAsync<UnitedStatesResult>(request);
            var populationData = result.data.FirstOrDefault(x => x.Year == year);

            string response = $"The population number in the United States in {year} was {populationData.Population}";
            return new OkObjectResult(response);
        }
    }
}

