using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Chat;
using Microsoft.SemanticKernel.ChatCompletion;
using SemanticKernel.Agents;

string travelManagerName = "TravelManager";
string travelManagerInstructions = """
        You are a travel manager and your goal is to validate a given trip plan.
        If the plan is good, recap the entire plan into a Markdown table and say "the plan is approved".
        If not, write a paragraph to explain why it's not good and then provide an improved plan.
    """;

string travelAgentName = "TravelAgent";
string travelAgentInstructions = """
    You are a travel agent and you help users who wants to make a trip to visit a city. 
    The goal is to create a plan to visit a city based on the user preferences and budget.
    You don't have expertise on travel plans, so you can only suggest hotels, restaurants and places to see. You can't suggest travelling options like flights or trains.
    You're laser focused on the goal at hand. 
    Once you have generated a plan, don't ask the user for feedback or further suggestions. Stick with it.
    Don't waste time with chit chat. Don't say goodbye and don't wish the user a good trip.
    """;


string flightExpertName = "FlightExpert";
string flightExpertInstructions = """
        Your are an expert in flight travel and you are specialized in organizing flight trips by identifying the best flight options for your clients.
        Your goal is to create a flight plan to reach a city based on the user prefences and budget.
        You don't have experience on any other travel options, so you can only suggest flight options.
        You're laser focused on the goal at hand. You can provide plans only about flights. Do not include plans around lodging, meals or sightseeing.
        Once you have generated a flight plan, don't ask the user for feedback or further suggestions. Stick with it.
        Don't waste time with chit chat. Don't say goodbye and don't wish the user a good trip.
    """;

string trainExpertName = "TrainExpert";
string trainExpertInstructions = """
        Your are an expert in train travel and you are specialized in organizing train trips by identifying the best train options for your clients.
        Your goal is to create a train plan to reach a city based on the user prefences and budget.
        You don't have experience on any other travel options, so you can only suggest train options.
        You're laser focused on the goal at hand. You can provide plans only about trains. Do not include plans around lodging, meals or sightseeing.
        Once you have generated a train plan, don't ask the user for feedback or further suggestions. Stick with it.
        Don't waste time with chit chat. Don't say goodbye and don't wish the user a good trip.
    """;

//string busAgentName = "BusExpert";
//string busExpert = """
//    You are a bus expert and you are specialized in organizing bus trips by identifying the best bus options for your clients. 
//    Your expertise is in finding the best bus options for your clients based on their preferences and budget. 
//    Your goal is to help the travel manager to find the best travel options based on bus transportation. 
//    Propose ONLY travel plans based on bus transportation. Don't suggest any other transporation mode on your own.
//""";

//string trainAgentName = "TrainExpert";
//string trainExpert = """
//    You are a train expert and you are specialized in organizing train trips by identifying the best train options for your clients.
//    Your expertise is in finding the best train options for your clients based on their preferences and budget. Your goal is to help the travel manager to find the best travel options based on traing transportation.
//    Propose ONLY travel plans based on train transportation. Don't suggest any other transporation mode on your own.
//    """;


//string flightAgentName = "FlightExpert";
//string flightExpert = """
//    You are a flight expert and you are an specialized in organizing flight trips by identifying the best flight options for your clients.
//    Your expertise is in finding the best flight options for your clients based on their preferences and budget.
//    Your goal is to help the travel agent to find the best travel options based on flight transportation.
//    Every time you have finished created a flight plan, say "This is the way".
//    """;

#pragma warning disable SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

ChatCompletionAgent travelAgent = new ChatCompletionAgent
{
    Name = travelAgentName,
    Instructions = travelAgentInstructions,
    Kernel = KernelCreator.CreateKernel()
};

ChatCompletionAgent travelManager = new ChatCompletionAgent
{
    Name = travelManagerName,
    Instructions = travelManagerInstructions,
    Kernel = KernelCreator.CreateKernel()
};

ChatCompletionAgent trainAgent = new ChatCompletionAgent
{
    Name = trainExpertName,
    Instructions = trainExpertInstructions,
    Kernel = KernelCreator.CreateKernel()
};

//ChatCompletionAgent busAgent = new ChatCompletionAgent
//{
//    Name = busAgentName,
//    Instructions = busExpert,
//    Kernel = KernelCreator.CreateKernel()
//};

//ChatCompletionAgent trainAgent = new ChatCompletionAgent
//{
//    Name = trainAgentName,
//    Instructions = trainExpert,
//    Kernel = KernelCreator.CreateKernel()
//};

ChatCompletionAgent flightAgent = new ChatCompletionAgent
{
    Name = flightExpertName,
    Instructions = flightExpertInstructions,
    Kernel = KernelCreator.CreateKernel()
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
    2) After {{{travelAgentName}}} replies, determine which is the travel preference for the trip based on the user input:
    - If the user prefers to travel by train, it's {{{trainExpertName}}}'s turn.
    - If the user prefers to travel by flight, it's {{{flightExpertName}}}'s turn.
    
    3) Finally, it's {{{travelManagerName}}}'s turn to review and approve the plan.
    4) If the plan is approved, the conversation ends.
    5) If the plan isn't approved, it's {{{travelAgent}}}'s turn again.

    History:
    {{$history}}
    """
    );

AgentGroupChat chat = new(travelManager, travelAgent, flightAgent, trainAgent)
{
    ExecutionSettings = new()
    {
        TerminationStrategy = new KernelFunctionTerminationStrategy(terminateFunction, KernelCreator.CreateKernel())
        {
            Agents = [travelManager],
            ResultParser = (result) => result.GetValue<string>()?.Contains("The plan is approved", StringComparison.OrdinalIgnoreCase) ?? false,
            HistoryVariableName = "history",
            MaximumIterations = 10
        },
        SelectionStrategy = new KernelFunctionSelectionStrategy(selectionFunction, KernelCreator.CreateKernel())
        {
            ResultParser = (result) => result.GetValue<string>() ?? travelAgentName,
            AgentsVariableName = "agents",
            HistoryVariableName = "history"
        }
    }
};

var prompt = "I live in Como, Italy and I would like to visit Paris. I'm on a budget, I want to travel by train and I would like to stay for maximum 3 days. Please craft a trip plan for me.";

chat.AddChatMessage(new ChatMessageContent(AuthorRole.User, prompt));
await foreach (var content in chat.InvokeAsync())
{
#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    Console.WriteLine($"# {content.Role} - {content.AuthorName ?? "*"}: '{content.Content}'");
    Console.WriteLine();
    await Task.Delay(TimeSpan.FromSeconds(180));
#pragma warning restore SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
}

Console.WriteLine($"# IS COMPLETE: {chat.IsComplete}");

#pragma warning restore SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

