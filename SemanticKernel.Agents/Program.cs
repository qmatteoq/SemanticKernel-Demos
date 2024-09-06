using SemanticKernel.Agents.Scenarios;

Console.WriteLine("Which scenario would you like to run?");
Console.WriteLine("1) Travel agency");
Console.WriteLine("2) Prompt Improvement");
Console.WriteLine("3) Developer");
Console.WriteLine("4) Rap battle");
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
    case "3":
        DeveloperScenario developerScenario = new DeveloperScenario();
        developerScenario.InitializeScenario(false);

        Console.WriteLine("Write your task:");
        prompt = Console.ReadLine();
        await developerScenario.ExecuteScenario(prompt);
        break;
    case "4":

        RapBattleScenario rapBattleScenario = new RapBattleScenario();
        rapBattleScenario.InitializeScenario(true);

        Console.WriteLine("Write your topic:");
        prompt = Console.ReadLine();
        await rapBattleScenario.ExecuteScenario(prompt);
        break;
    default:
        Console.WriteLine("Invalid option");
        break;
}
