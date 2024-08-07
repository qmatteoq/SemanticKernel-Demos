using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents.Chat;

namespace SemanticKernel.Agents.Scenarios
{
    public class PromptImprovementScenario: BaseScenario
    {
        public void InitializeScenario()
        {
            string promptValidatorName = "PromptValidator";
            string promptValidatorInstructions = """
        You are a prompt engineer and you have lot of experience in writing good prompts to achieve task with AI.
        However, your goal isn't to write prompts, but to evaluate the quality of a prompt and suggest improvements.
        When you get shared a prompt, you must perform two tasks:

        1) Evaluate the prompt and rate it on a scale from 1 (very bad prompt) to 10 (excellent prompt).
        2) Provide suggestions on how to improve the prompt.

        After you have improved the prompt, share it and say "the prompt is approved".
    """;

            string promptExpertName = "PromptExpert";
            string promptExpertInstructions = """
                You are a prompt engineer and you have lot of experience in writing good prompts to achieve task with AI.
                The user is going to give you a task to complete, your goal is to write the best possible prompt to achieve the task.
                """;

           
            ChatCompletionAgent promptValidatorAgent = new ChatCompletionAgent
            {
                Name = promptValidatorName,
                Instructions = promptValidatorInstructions,
                Kernel = KernelCreator.CreateKernel()
            };

            ChatCompletionAgent promptExpertAgent = new ChatCompletionAgent
            {
                Name = promptExpertName,
                Instructions = promptExpertInstructions,
                Kernel = KernelCreator.CreateKernel()
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
                - {{{promptExpertName}}}

                Always follow these two steps when selecting the next participant:
                1) After user input, it is {{{promptExpertName}}}'s turn.
                2) After {{{promptExpertName}}} replies, it's {{{promptValidatorName}}}'s turn.
                
                Once {{{promptValidatorName}}} has refined and approved the prompt, end the conversation.

                History:
                {{$history}}
                """
                );

            chat = new(promptValidatorAgent, promptExpertAgent)
            {
                ExecutionSettings = new()
                {
                    TerminationStrategy = new KernelFunctionTerminationStrategy(terminateFunction, KernelCreator.CreateKernel())
                    {
                        Agents = [promptValidatorAgent],
                        ResultParser = (result) => result.GetValue<string>()?.Contains("yes", StringComparison.OrdinalIgnoreCase) ?? false,
                        HistoryVariableName = "history",
                        MaximumIterations = 10
                    },
                    SelectionStrategy = new KernelFunctionSelectionStrategy(selectionFunction, KernelCreator.CreateKernel())
                    {
                        AgentsVariableName = "agents",
                        HistoryVariableName = "history"
                    }
                }
            };
        }
    }
}
