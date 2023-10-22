using System.IO;
using System.Reflection;
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
            var binDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var rootDirectory = Path.GetFullPath(Path.Combine(binDirectory, ".."));
            var result = File.ReadAllText(rootDirectory + "/manifest/ai-plugin.json");
            var json = result.Replace("{url}", currentDomain);
            return new OkObjectResult(json);
        }
    }
}
