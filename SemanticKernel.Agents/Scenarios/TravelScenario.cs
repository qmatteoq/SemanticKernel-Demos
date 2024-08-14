using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents.Chat;
using Microsoft.SemanticKernel.ChatCompletion;

namespace SemanticKernel.Agents.Scenarios
{
    public class TravelScenario: BaseScenario
    {

        const string travelManagerName = "TravelManager";
        const string travelAgentName = "TravelAgent";
        const string flightExpertName = "FlightExpert";
        const string trainExpertName = "TrainExpert";

        private AgentGroupChat chat;

        public void InitializeScenario(bool useAzureOpenAI)
        {
            string travelManagerInstructions = """
    You are a travel manager and your goal is to validate a given trip plan. 
    You must make sure that the plan includes all the necessary details: transportation, lodging, meals and sightseeing. 
    If one of these details is missing, the plan is not good.
    If the plan is good, recap the entire plan into a Markdown table and say "the plan is approved".
    If not, write a paragraph to explain why it's not good and then provide an improved plan.
    """;

            string travelAgentInstructions = """
    You are a travel agent and you help users who wants to make a trip to visit a city. 
    The goal is to create a plan to visit a city based on the user preferences and budget.
    You don't have expertise on travel plans, so you can only suggest hotels, restaurants and places to see. You can't suggest travelling options like flights or trains.
    You're laser focused on the goal at hand. 
    Once you have generated a plan, don't ask the user for feedback or further suggestions. Stick with it.
    Don't waste time with chit chat. Don't say goodbye and don't wish the user a good trip.
    """;


            string flightExpertInstructions = """
        Your are an expert in flight travel and you are specialized in organizing flight trips by identifying the best flight options for your clients.
        Your goal is to create a flight plan to reach a city based on the user prefences and budget.
        You don't have experience on any other travel options, so you can only suggest flight options.
        You're laser focused on the goal at hand. You can provide plans only about flights. Do not include plans around lodging, meals or sightseeing.
        Once you have generated a flight plan, don't ask the user for feedback or further suggestions. Stick with it.
        Don't waste time with chit chat. Don't say goodbye and don't wish the user a good trip.
    """;

            string trainExpertInstructions = """
        Your are an expert in train travel and you are specialized in organizing train trips by identifying the best train options for your clients.
        Your goal is to create a train plan to reach a city based on the user prefences and budget.
        You don't have experience on any other travel options, so you can only suggest train options.
        You're laser focused on the goal at hand. You can provide plans only about trains. Do not include plans around lodging, meals or sightseeing.
        Once you have generated a train plan, don't ask the user for feedback or further suggestions. Stick with it.
        Don't waste time with chit chat. Don't say goodbye and don't wish the user a good trip.
    """;

            ChatCompletionAgent travelAgent = new ChatCompletionAgent
            {
                Name = travelAgentName,
                Instructions = travelAgentInstructions,
                Kernel = KernelCreator.CreateKernel(useAzureOpenAI)
            };

            ChatCompletionAgent travelManager = new ChatCompletionAgent
            {
                Name = travelManagerName,
                Instructions = travelManagerInstructions,
                Kernel = KernelCreator.CreateKernel(useAzureOpenAI)
            };

            ChatCompletionAgent trainAgent = new ChatCompletionAgent
            {
                Name = trainExpertName,
                Instructions = trainExpertInstructions,
                Kernel = KernelCreator.CreateKernel(useAzureOpenAI)
            };

            ChatCompletionAgent flightAgent = new ChatCompletionAgent
            {
                Name = flightExpertName,
                Instructions = flightExpertInstructions,
                Kernel = KernelCreator.CreateKernel(useAzureOpenAI)
            };

            KernelFunction terminateFunction = KernelFunctionFactory.CreateFromPrompt(
                $$$"""
                Determine if the travel plan has been approved by {{{travelManagerName}}}. If so, respond with a single word: yes.

                History:

                {{$history}}
                """
                );



            KernelFunction selectionFunction = KernelFunctionFactory.CreateFromPrompt(
                $$$"""
                Your job is to determine which participant takes the next turn in a conversation according to the action of the most recent participant.
                State only the name of the participant to take the next turn.

                Choose only from these participants:
                - {{{travelManagerName}}}
                - {{{travelAgentName}}}
                - {{{flightExpertName}}}
                - {{{trainExpertName}}}

                Always follow these four when selecting the next participant:
                1) After user input, it is {{{travelAgentName}}}'s turn.
                2) After {{{travelAgentName}}} replies, it's {{{flightExpertName}}}'s turn to generate a flight plan for the given trip.
                - If the user prefers to travel by train, it's {{{trainExpertName}}}'s turn.
                - If the user prefers to travel by flight, it's {{{flightExpertName}}}'s turn.
    
                3) Finally, it's {{{travelManagerName}}}'s turn to review and approve the plan.
                4) If the plan is approved, the conversation ends.
                5) If the plan isn't approved, it's {{{travelAgent}}}'s turn again.

                History:
                {{$history}}
                """
                );

            chat = new(travelManager, travelAgent, flightAgent, trainAgent)
            {
                ExecutionSettings = new()
                {
                    TerminationStrategy = new KernelFunctionTerminationStrategy(terminateFunction, KernelCreator.CreateKernel(useAzureOpenAI))
                    {
                        Agents = [travelManager],
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
