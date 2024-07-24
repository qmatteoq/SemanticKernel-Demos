# Semantic Kernel samples

This repository includes a series of samples to demonstrate the usage of the various Semantic Kernel features in a .NET console application.
These samples are used as a companion for some of the presentations I delivered on this topic and for my blog posts on [The Developer's Cantina](https://www.developerscantina.com/).

## Table of Contents
- [Semantic Kernel samples](#semantic-kernel-samples)
  - [Table of Contents](#table-of-contents)
  - [Prerequisites](#prerequisites)
  - [Samples](#samples)
    - [KernelMemory](#kernelmemory)
    - [SemanticKernel.Basic](#semantickernelbasic)
    - [SemanticKernel.PromptFunction](#semantickernelpromptfunction)
    - [SemanticKernel.NativeFunction](#semantickernelnativefunction)
    - [SemanticKernel.OpenAIPlugin](#semantickernelopenaiplugin)
    - [SemanticKernel.FunctionCalling](#semantickernelfunctioncalling)
    - [SemanticKernel.FunctionStepwisePlanner](#semantickernelfunctionstepwiseplanner)
    - [SemanticKernel.HandlebarsPlanner](#semantickernelhandlebarsplanner)
    - [SemanticKernel.BingPlugin](#semantickernelbingplugin)
    - [SemanticKernel.GraphPlugin](#semantickernelgraphplugin)
    - [SemanticKernel.KernelMemoryPlugin](#semantickernelkernelmemoryplugin)
    - [SemanticKernel.ChatMemory](#semantickernelchatmemory)
    - [SemanticKernel.PlannerTicket](#semantickernelplannerticket)
    - [SemanticKernel.Plugins](#semantickernelplugins)
    - [SemanticKernel.Prompty](#semantickernelprompty)
  - [License](#license)
  
## Prerequisites
You will need a valid AI service to run the samples. By default, all the samples are configured to use [Azure OpenAI](https://azure.microsoft.com/products/ai-services/openai-service) and they will look in your configuration file for the following section:

```json
{
  "AzureOpenAI": {
    "Endpoint": "<url-of-the-endpoint>",
    "DeploymentName": "<deployment-name>",
    "ApiKey": "<api-key>"
  }
}
```

In code, every sample will look for the `AzureOpenAI` section in the configuration file and will use the settings to setup the kernel using one of the initialization methods with the AzureOpenAI suffix, as in the following sample:

```csharp
string apiKey = configuration["AzureOpenAI:ApiKey"];
string deploymentName = configuration["AzureOpenAI:DeploymentName"];
string endpoint = configuration["AzureOpenAI:Endpoint"];

var kernel = Kernel.CreateBuilder()
    .AddAzureOpenAIChatCompletion(deploymentName, endpoint, apiKey)
    .Build();
```

If you prefer, you can use also [OpenAI](https://openai.com/api/). In this case, you will need to add your custom settings in the configuration file and, then, in code, replace all the references to `AzureOpenAI` with `OpenAI` in the kernel setup. This is, for example, how the previous initialization code will look like with OpenAI:

```csharp
string apiKey = configuration["OpenAI:ApiKey"];
string modelId = configuration["OpenAI:ModelId"];

var kernel = Kernel.CreateBuilder()
    .AddOpenAIChatCompletion(modelId, apiKey)
    .Build();
```


## Samples
### KernelMemory
This project shows in action Kernel Memory, a library which makes it easier to implement "Chat with your data" scenarios in your applications. The sample project is a web application based on Blazor and it enables you to load some documents and then query them using natural language.

### SemanticKernel.Basic
This project shows the basic usage of the Semantic Kernel library. It demonstrates how to create a prompt, execute it, and display the response from the LLM.

### SemanticKernel.PromptFunction
This project shows how you can move a prompt outside code into a library, then load it into a function so that you can use it in your application. The library is stored in the SemanticKernel.Plugins project.

### SemanticKernel.NativeFunction
This project shows how you can create a native function, which is a type of plugin that Semantic Kernel can use and which is based on native code. In this example, the plugin (which is defined in the SemanticKernel.Plugins project) is called UnitedStatesPlugin and it uses a public and free REST API to retrieve information about the US population.

### SemanticKernel.OpenAIPlugin
This project has the same goal as the previous sample, but the plugin is hosted on the cloud and it's published using the OpenAI plugin format. This allows a plugin to be reused across multiple AI platforms.

### SemanticKernel.FunctionCalling
This project shows how you can create AI agents using Semantic Kernel. When you build an AI agent, you define a task to perform and then you provide a series of tools (plugins) that the agent can use to accomplish the task. In this example, you define a task to generate a mail which contains information about the United States population and Semantic Kernel will automatically generate a plan that involves calling the plugins built in the previous samples to perform the operations.

### SemanticKernel.FunctionStepwisePlanner
This project shows how you can use a planner, instead of function calling, to build an AI agent. This example is included for historical reasons, since planners are on the way of deprecation. In the past, they offered an extra boost to generate a proper plan to perform a task, since in some scenarios function calling wasn't very effective. With the latest improvements in function calling from OpenAI, however, the need for a planner is less and less frequent. Function calling is more efficient and it's the recommended way to build AI agents.

### SemanticKernel.HandlebarsPlanner
This project shows the usage of another type of planner, which leverages the Handlebar syntax to generate a plan, since it's easier to be interpreted by an LLM. However, this planner is also set for deprecation.

### SemanticKernel.BingPlugin
This project shows the usage of one of the plugins built by the Semantic Kernel team, which enables Semantic Kernel to perform web searches. Thanks to this plugin, the LLM will rely also on the web to generate a response, instead of using only the training data that was used to create the LLM.

### SemanticKernel.GraphPlugin
This project shows the usage of another plugin built by the Semantic Kernel team, which enables Semantic Kernel to perform operations with the Microsoft Graph. Thanks to this plugin, the LLM will be able to leverage generative AI with the Microsoft 365 ecosystem.

### SemanticKernel.KernelMemoryPlugin
This project shows the combined usage of Semantic Kernel and Kernel Memory. The plugin enables Semantic Kernel to chat with your data, by leveraging the Kernel Memory service to retrieve the information stored in a vector database. Thanks to this plugin, you can create complex scenarios that involve plugins and memory.

### SemanticKernel.ChatMemory
This project shows another example of combined usage of Semantic Kernel and Kernel memory. In this case, the plugin enables a "continuous chat" experience with your data, so that you can ask multiple questions and retain the context. This scenario isn't supported by Kernel Memory itself, but it becomes possible thanks to the flexibility of Semantic Kernel.

### SemanticKernel.PlannerTicket
This project is another example of using Semantic Kernel to create AI agents. In this case, the agent is able to get information about support tickets and share them into a mail, by combining the usage of plugins and prompt functions.

### SemanticKernel.Plugins
This project contains the plugins used in the previous samples. It's a shared library that is referenced by the other projects.

### SemanticKernel.Prompty
This project is an example of using Semantic Kernel with Prompty, a new solution to standardize prompts and its execution into a single asset that we can use to improve the management of our prompts in our applications.

## License
This repository is licensed under the MIT License. See the LICENSE file for more information.




