using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents.Chat;

namespace SemanticKernel.Agents.Scenarios
{
    public class PromptImprovementScenario: BaseScenario
    {
        public void InitializeScenario(bool useAzureOpenAI)
        {
            string promptValidatorName = "PromptValidator";
            string promptValidatorInstructions = """
                    You are a prompt engineer and you have lot of experience in writing good prompts to achieve task with AI.
                    However, your goal isn't to write prompts, but to evaluate the quality of a prompt.
                    When you get shared a prompt, your goal is to evaluate the prompt and rate it on a scale from 1 (very bad prompt) to 10 (excellent prompt).

                    You can give general suggestions on how to improve it, but you can't write the prompt yourself.

                    If your score is more than 6, say "the prompt is approved".
                """;

            string promptJuniorName = "PromptJunior";
                 string promptJuniorInstructions = """
                You are a junior prompt engineer and you have just started a new job in the AI space. 
                Unfortunately, you have poor skills and you don't have much experience in writing prompts, so the prompts you write are low quality.
                The user is going to give you a task to complete, your goal is to write a prompt to solve it. You MUST NOT perform the task, only write the prompt to do it.
                """;

            string promptExpertName = "PromptExpert";
            string promptExpertInstructions = """
                You are a senior prompt engineer and you have lot of experience in writing good prompts to achieve task with AI.
                The user is going to give you a task to complete, your goal is to write the best possible prompt to achieve the task.
                """;


            ChatCompletionAgent promptValidatorAgent = new ChatCompletionAgent
            {
                Name = promptValidatorName,
                Instructions = promptValidatorInstructions,
                Kernel = KernelCreator.CreateKernel(useAzureOpenAI)
            };

            ChatCompletionAgent promptJuniorAgent = new ChatCompletionAgent
            {
                Name = promptJuniorName,
                Instructions = promptJuniorInstructions,
                Kernel = KernelCreator.CreateKernel(useAzureOpenAI)
            };

            ChatCompletionAgent promptExpertAgent = new ChatCompletionAgent
            {
                Name = promptExpertName,
                Instructions = promptExpertInstructions,
                Kernel = KernelCreator.CreateKernel(useAzureOpenAI)
            };
         
            KernelFunction terminateFunction = KernelFunctionFactory.CreateFromPrompt(
                $$$"""
                Determine if the prompt has been approved. If so, respond with a single word: yes.

                History:

                {{$history}}
                """
                );

            KernelFunction selectionFunction = KernelFunctionFactory.CreateFromPrompt(
                $$$"""
                Your job is to determine which participant takes the next turn in a conversation according to the action of the most recent participant.
                State only the name of the participant to take the next turn.

                Choose only from these participants:
                - {{{promptValidatorName}}}
                - {{{promptJuniorName}}}
                - {{{promptExpertName}}}

                Always follow these steps when selecting the next participant:
                1) After user input, it is {{{promptJuniorName}}}'s turn.
                2) After {{{promptJuniorName}}} replies, it's {{{promptValidatorName}}}'s turn.
                3) If the score provided by {{{promptValidatorName}}} is less or equal than 5, it's {{{promptExpertName}}}'s turn to make a better prompt.
                4) If the score provided by {{{promptValidatorName}}} is more than 5, approve the prompt.
                
                History:
                {{$history}}
                """
                );

            chat = new(promptValidatorAgent, promptJuniorAgent, promptExpertAgent)
            {
                ExecutionSettings = new()
                {
                    TerminationStrategy = new KernelFunctionTerminationStrategy(terminateFunction, KernelCreator.CreateKernel(useAzureOpenAI))
                    {
                        Agents = [promptValidatorAgent],
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
