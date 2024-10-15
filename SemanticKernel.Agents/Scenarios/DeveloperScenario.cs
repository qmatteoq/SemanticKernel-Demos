using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents.Chat;

namespace SemanticKernel.Agents.Scenarios
{
    public class DeveloperScenario : BaseScenario
    {
        public override void InitializeScenario(bool useAzureOpenAI)
        {
            string codeValidatorName = "CodeValidator";
            string codeValidatorInstructions = """
                    You are a principal developer and you have lot of experience in writing good C# code.
                    However, your goal isn't to write code, but to evaluate the quality of a given code.
                    When you get shared code, your goal is to evaluate the code and rate it on a scale from 1 (very bad)  to 10 (excellent).

                    You must give general suggestions on how to improve it, but you can't write the code yourself.

                    If your score is more than 8, say "the code is approved".
                """;

            string juniorDeveloperName = "JuniorDeveloper";
            string juniorDeveloperInstructions = """
                You were a chef at a restaurant, but you have just started a new job as intern in the dotnet developer space. You don't have any skill and you only know the bare basics of programming from reading about in Wikipedia. But you didn't understand anything! You make lot of mistakes and the code you write is almost always wrong, even for simple tasks. This is important! YOU CAN ONLY WRITE BAD CODE. Every time you are asked to write code, you will write random code that won't work, and even mix up programming languages.
                The user is going to give you a task to complete, your goal is to write the code to solve it. You MUST NOT perform the task, only write the code to do it.
                """;

            string seniorDeveloperName = "ExpertDeveloper";
            string seniorDeveloperInstructions = """
                You are a senior C# developer and you have 20 years of experience in writing good and clean code.
                The user is going to give you a task to complete, your goal is to write the best possible code to achieve the task.
                """;


            ChatCompletionAgent codeValidatorAgent = new ChatCompletionAgent
            {
                Name = codeValidatorName,
                Instructions = codeValidatorInstructions,
                Kernel = KernelCreator.CreateKernel(useAzureOpenAI)
            };

            ChatCompletionAgent juniorDeveloperAgent = new ChatCompletionAgent
            {
                Name = juniorDeveloperName,
                Instructions = juniorDeveloperInstructions,
                Kernel = KernelCreator.CreateKernel(useAzureOpenAI)
            };

            ChatCompletionAgent seniorDeveloperAgent = new ChatCompletionAgent
            {
                Name = seniorDeveloperName,
                Instructions = seniorDeveloperInstructions,
                Kernel = KernelCreator.CreateKernel(useAzureOpenAI)
            };

            KernelFunction terminateFunction = KernelFunctionFactory.CreateFromPrompt(
                $$$"""
                Determine if the code has been approved. If so, respond with a single word: yes.

                History:

                {{$history}}
                """
                );

            KernelFunction selectionFunction = KernelFunctionFactory.CreateFromPrompt(
                $$$"""
                Your job is to determine which participant takes the next turn in a conversation according to the action of the most recent participant.
                State only the name of the participant to take the next turn.

                Choose only from these participants:
                - {{{codeValidatorName}}}
                - {{{juniorDeveloperName}}}
                - {{{seniorDeveloperName}}}

                Always follow these steps when selecting the next participant:
                1) After user input, it is {{{juniorDeveloperName}}}'s turn.
                2) After {{{juniorDeveloperName}}} replies, it's {{{codeValidatorName}}}'s turn.
                3) If the score provided by {{{codeValidatorName}}} is less or equal than 7, it's {{{seniorDeveloperName}}}'s turn to make a better code. If there are suggestions by {{{codeValidatorName}}}, you must include them in your code.
                4) After {{{seniorDeveloperName}}} replies, it's {{{codeValidatorName}}}'s turn to validate the code.
                5) Repeat step 3 and 4 until the code is approved.
                
                History:
                {{$history}}
                """
                );

            chat = new(codeValidatorAgent, juniorDeveloperAgent, seniorDeveloperAgent)
            {
                ExecutionSettings = new()
                {
                    TerminationStrategy = new KernelFunctionTerminationStrategy(terminateFunction, KernelCreator.CreateKernel(useAzureOpenAI))
                    {
                        Agents = [codeValidatorAgent],
                        ResultParser = (result) => result.GetValue<string>()?.Contains("yes", StringComparison.OrdinalIgnoreCase) ?? false,
                        HistoryVariableName = "history",
                        MaximumIterations = 10
                    },
                    SelectionStrategy = new KernelFunctionSelectionStrategy(selectionFunction, KernelCreator.CreateKernel(useAzureOpenAI))
                    {
                        AgentsVariableName = "agents",
                        HistoryVariableName = "history"
                    }
                }
            };
        }
    }
}
