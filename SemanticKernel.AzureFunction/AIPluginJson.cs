using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace SemanticKernel.AzureFunction
{
    public class AIPluginJson
    {
        [FunctionName("GetAIPluginJson")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = ".well-known/ai-plugin.json")] HttpRequest req)
        {
            var currentDomain = $"{req.Scheme}://{req.Host.Value}";
            var result = File.ReadAllText(@"D:\src\events\Codemotion2023\SemanticKernel\SemanticKernel.AzureFunction\manifest\ai-plugin.json");
            var json = result.Replace("{url}", currentDomain);
            return new OkObjectResult(json);
        }
    }
}
