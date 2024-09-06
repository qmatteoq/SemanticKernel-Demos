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
                    You are a developer and you have lot of experience in writing good C# code.
                    However, your goal isn't to write code, but to evaluate the quality of a given code.
                    When you get shared code, your goal is to evaluate the code and rate it on a scale from 1 (very bad prompt) to 10 (excellent prompt).

                    You can give general suggestions on how to improve it, but you can't write the code yourself.

                    If your score is more than 6, say "the code is approved".
                """;

            string juniorDeveloperName = "JuniorDeveloper";
            string juniorDeveloperInstructions = """
                You are a chef at a restaurant, but you have just started a new job in the developer space even if you don't have any skill and you don't know how to write code. You make lot of mistakes and the code you write is always wrong, even with simple tasks. This is important! YOU CAN'T WRITE CODE. Every time you are asked to write code, you will write random code that doesn't work.
                The user is going to give you a task to complete, your goal is to write the code to solve it. You MUST NOT perform the task, only write the code to do it.
                """;

            string seniorDeveloperName = "ExpertDeveloper";
            string seniorDeveloperInstructions = """
                You are a C# senior developer and you have lot of experience in writing good code.
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
                3) If the score provided by {{{codeValidatorName}}} is less or equal than 5, it's {{{seniorDeveloperName}}}'s turn to make a better code.
                4) After {{{seniorDeveloperName}}} replies, it's {{{codeValidatorName}}}'s turn to validate the code.
                5) Repeat step 5 and 6 until the code is approved.
                
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
