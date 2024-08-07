using SemanticKernel.Agents.Scenarios;

Console.WriteLine("Which scenario would you like to run?");
Console.WriteLine("1) Travel Scenario");
Console.WriteLine("2) Prompt Improvement Scenario");
Console.WriteLine("Type the number of the scenario you want to run and press Enter");
string response = Console.ReadLine();
switch (response)
{
   case "1":
        TravelScenario travelScenario = new TravelScenario();
        travelScenario.InitializeScenario();
        await travelScenario.ExecuteScenario("I live in Como, Italy and I would like to visit Paris. I'm on a budget, I want to travel by train and I would like to stay for maximum 3 days. Please craft a trip plan for me");
        break;
    case "2":
        PromptImprovementScenario promptScenario = new PromptImprovementScenario();
        promptScenario.InitializeScenario();
        await promptScenario.ExecuteScenario("Generate a job description for a Software Engineer in a company specialized in Microsoft techologies");
        break;
    default:
        Console.WriteLine("Invalid option");
        break;
}
