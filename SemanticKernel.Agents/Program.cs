using SemanticKernel.Agents.Scenarios;

Console.WriteLine("Which scenario would you like to run?");
Console.WriteLine("1) Travel Scenario");
Console.WriteLine("2) Prompt Improvement Scenario");
Console.WriteLine("Type the number of the scenario you want to run and press Enter");
string response = Console.ReadLine();
string prompt = string.Empty;
switch (response)
{
   case "1":
        TravelScenario travelScenario = new TravelScenario();
        travelScenario.InitializeScenario(true);

        Console.WriteLine("Write your request about the trip plan:");
        prompt = Console.ReadLine();
        await travelScenario.ExecuteScenario(prompt);
        break;
    case "2":
        PromptImprovementScenario promptScenario = new PromptImprovementScenario();
        promptScenario.InitializeScenario(true);

        Console.WriteLine("Write your task:");
        prompt = Console.ReadLine();
        await promptScenario.ExecuteScenario(prompt);
        break;
    default:
        Console.WriteLine("Invalid option");
        break;
}
