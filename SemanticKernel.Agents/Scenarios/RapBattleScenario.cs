using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents.Chat;

namespace SemanticKernel.Agents.Scenarios
{
    public class RapBattleScenario : BaseScenario
    {
        public override void InitializeScenario(bool useAzureOpenAI)
        {            
            string rapMCName = "RapMCName";
            string rapMCInstructions = "You are a rap MC and your role is to review the rap lyrics in a rap battle and give it a score. Participants in the content will be given a topic and they will need to create a hip hop version of it. You can use the Advanced RAG plugin to get the information you need about the given topic. You're going to give to the each rap lyrics a score between 1 and 10. You must score them separately. The rapper who gets the higher score wins. You can search for information or rate the lyrics. You aren't allowed to write lyrics on your own and join the rap battle.";

            string eminemName = "Eminem";
            string eminemInstructions = "You are a rapper and you rap in the stlye of Eminem. You are participating to a rap battle. You will be given a topic and you will need to create the lyrics and rap about it.";

            string jayZName = "JayZ";
            string jayZInstructions = "You are a rapper and you rap in the stlye of Jay-Z. You are participating to a rap battle. You will be given a topic and you will need to create the lyrics and rap about it.";


            ChatCompletionAgent rapMCAgent = new ChatCompletionAgent
            {
                Name = rapMCName,
                Instructions = rapMCInstructions,
                Kernel = KernelCreator.CreateKernel(useAzureOpenAI)
            };


            ChatCompletionAgent eminemAgent = new ChatCompletionAgent
            {
                Name = eminemName,
                Instructions = eminemInstructions,
                Kernel = KernelCreator.CreateKernel(useAzureOpenAI)
            };

            ChatCompletionAgent jayZAgent = new ChatCompletionAgent
            {
                Name = jayZName,
                Instructions = jayZInstructions,
                Kernel = KernelCreator.CreateKernel(useAzureOpenAI)
            };


            KernelFunction terminateFunction = KernelFunctionFactory.CreateFromPrompt(
                $$$"""
                    A rap battle is completed once all the participants have created lyrics for the given topic, a score is given and a winner is determined.
                    Determine if the rap battle is completed.  If so, respond with a single word: yes.
        
                    History:
                    {{$history}}
                """
            );

            KernelFunction selectionFunction = KernelFunctionFactory.CreateFromPrompt(
                $$$"""
                    Your job is to determine which participant takes the next turn in a conversation according to the action of the most recent participant.
                    State only the name of the participant to take the next turn.

                    Choose only from these participants:
                    - {{{rapMCName}}}
                    - {{{eminemName}}}
                    - {{{jayZName}}}

                    Always follow these steps when selecting the next participant:
                    1) After user input, it is {{{rapMCName}}}'s turn to get information about the given topic.
                    2) After {{{rapMCName}}} replies, it's {{{eminemName}}}'s turn to create rap lyrics based on the results returned by {{{rapMCName}}}.
                    3) After {{{eminemName}}} replies, it's {{{jayZName}}}'s turn to create rap lyrics based on the results returned by {{{rapMCName}}}.
                    4) After {{{jayZName}}} replies, it's {{{rapMCName}}}'s turn to review the rap lyrics and give it a score.
                    5) {{{rapMCName}}} will declare the winner based on who got the higher score.
       
                    History:
                    {{$history}}
                    """
                );

        chat = new(rapMCAgent, eminemAgent, jayZAgent)
        {
            ExecutionSettings = new()
            {
                TerminationStrategy = new KernelFunctionTerminationStrategy(terminateFunction, KernelCreator.CreateKernel(useAzureOpenAI))
                {
                    Agents = [rapMCAgent],
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
