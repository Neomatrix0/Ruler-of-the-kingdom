using System;
using System.IO;
using Newtonsoft.Json;

class Program
{
    static Random random = new Random(); // Global random object
    static string directoryPath = @"data/";
    static bool kingdomCreated = false;
    static double happinessPopulation = 70; // Initial HappinessPopulation
    static string kingdomFilePath = "";

    static void Main(string[] args)
    {
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        double budget = 1000000;  // Initial budget
        Console.WriteLine("Initial Budget: " + budget);

        int choice;
        do
        {
            Console.WriteLine("\nWelcome to the game Ruler of the Kingdom Main Menu!");
            Console.WriteLine("Every path begins with a choice\n");
            Console.WriteLine("1. Create your own kingdom");
            Console.WriteLine("2. View all kingdoms");
            Console.WriteLine("3. Exit");
            Console.WriteLine("4. Fight with enemy");
            Console.WriteLine("5. Increase taxes");
            Console.WriteLine("6. Reduce taxes\n");

            choice = Convert.ToInt32(Console.ReadLine());

            switch (choice)
            {
                case 1:
                    if (!kingdomCreated && !DirectoryContainsJsonFiles(directoryPath))
                    {
                        CreateYourOwnKingdom(ref budget);
                        kingdomCreated = true;
                    }
                    else
                    {
                        Console.WriteLine("A kingdom has already been created. You can't create another one until you win or lose the match.");
                    }
                    break;

                case 2:
                    ViewAllKingdoms();
                    break;

                case 3:
                    Console.WriteLine("The game will be closed and the data deleted. Please wait...");
                    DeleteAllJsonFiles(directoryPath);
                    break;

                case 4:

                    if (kingdomCreated)
                    {
                        ChooseTheEnemy(ref budget, ref happinessPopulation);
                    }
                    else
                    {
                        Console.WriteLine("You must create a kingdom first.");
                    }
                    /*   if (ConfirmWar())
                       {
                           FightWar(ref budget, ref happinessPopulation);
                           Console.WriteLine($"Updated Budget After War: {budget}");
                           Console.WriteLine($"Updated Happiness Population After War: {happinessPopulation}");
                       }*/
                    break;

                case 5:

                    IncreaseTaxes(ref budget, ref happinessPopulation);
                    UpdateJsonValues(kingdomFilePath, budget, happinessPopulation);

                    break;

                case 6:

                    ReduceTaxes(ref budget, ref happinessPopulation);

                    break;

                default:
                    Console.WriteLine("Make the right choice.");
                    break;
            }

            if (budget == 0)
            {
                Console.WriteLine("Your budget has reached zero the kingdom declared default hence you will be beheaded. Game over.");
                break;
            }

             if (happinessPopulation < 20)
            {
                Coup(ref happinessPopulation, ref budget);
                UpdateJsonValues(kingdomFilePath, budget, happinessPopulation);
            }

            //   Console.WriteLine($"Current Budget: {budget}");
            //  Console.WriteLine($"Current Happiness Population: {happinessPopulation}");
            if (choice != 3)
            {
                Console.WriteLine("\nPress a button to continue.");
                Console.ReadKey();
            }
        } while (choice != 3);
    }

    static bool ConfirmWar()
    {
        Console.WriteLine("The war will cost you 15% of your budget. Do you want to proceed? y/n");
        string answer = Console.ReadLine().ToLower().Trim();
        return answer == "y";
    }

    static void CreateYourOwnKingdom(ref double budget)
    {
        Console.Write("Please insert here the name of your Kingdom: ");
        string? inputName = Console.ReadLine();
        kingdomFilePath = Path.Combine(directoryPath, $"{inputName}.json");

        if (File.Exists(kingdomFilePath))
        {
            Console.WriteLine($"Kingdom {inputName} has already been created.");
            return;
        }




        Console.Write("Please insert here the name of the regions of your kingdom split by comma: ");
        string? inputRegions = Console.ReadLine();
        string[] regions = inputRegions.Split(',');

        if (regions.Length != 3)
        {
            throw new FormatException("Input must include only 3 names, each one split by comma.");
        }

        var kingdom = new
        {
            Name = inputName,
            Regions = regions,
            Budget = budget,
            HappinessPopulation = happinessPopulation,
            TimeStamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")
        };

        WriteJson(kingdom, kingdomFilePath);
        Console.WriteLine($"Kingdom {kingdom.Name} data has been saved successfully!");
        createEnemyKingdom("Atlantis", new string[] { "Mirage region", "Wald region", "Oceania region" }, 700000, 80);
        createEnemyKingdom("Magonia", new string[] { "Star region", "Galaxy region", "Alternative dimension" }, 1000000, 80);
        createEnemyKingdom("Dark kingdom", new string[] { "Sorcerer region", "Necro region", "Hell dimension" }, 1400000, 70);
    }

    static void ViewAllKingdoms()
    {
        var files = Directory.GetFiles(directoryPath, "*.json");
        if (files.Length > 0)
        {
            Console.WriteLine("Complete list of all kingdoms:\n");
            foreach (var file in files)
            {
                var kingdom = ReadJson(file);
                Console.WriteLine($"Kingdom name: {kingdom.Name}, Regions: {string.Join(", ", kingdom.Regions)}, Budget: {kingdom.Budget}, HappinessPopulation: {kingdom.HappinessPopulation}\n");
            }
        }
        else
        {
            Console.WriteLine("No kingdoms found.\n");
        }
    }

    static void FightWar(ref double budget, ref double happinessPopulation, string enemyFilePath)
    {
        int playerDiceRoll1 = random.Next(1, 7);
        int playerDiceRoll2 = random.Next(1, 7);
        int sumPlayerRolls = playerDiceRoll1 + playerDiceRoll2;
        Console.WriteLine($"Player dice rolls: {playerDiceRoll1} and {playerDiceRoll2} (Total score: {sumPlayerRolls})");

        int pcDiceRoll1 = random.Next(1, 7);
        int pcDiceRoll2 = random.Next(1, 7);
        int sumPcRolls = pcDiceRoll1 + pcDiceRoll2;
        Console.WriteLine($"Enemy dice rolls: {pcDiceRoll1} and {pcDiceRoll2} (Total score: {sumPcRolls})");

        if (sumPlayerRolls > sumPcRolls)
        {
            budget *= 1.15;  // Increase budget by 15%
            happinessPopulation += 10; // Increase happiness population by 10
            Console.WriteLine("Congratulations, you won! War costs will be repaid, and you will earn 15% more budget and increase happiness population by 5.");

        }
        else
        {
            budget = 0;  // Loss all budget
            happinessPopulation -= 10; // Decrease happiness population by 10
            Console.WriteLine("The enemy has won. You lost your reign and 10 happiness population.");
        }
        UpdateJsonValues(kingdomFilePath, budget, happinessPopulation);
    }

    static void WriteJson(dynamic kingdom, string filePath)
    {
        string jsonString = JsonConvert.SerializeObject(kingdom, Formatting.Indented);
        //string filePath = Path.Combine(directoryPath, $"{kingdom.Name}_{kingdom.TimeStamp}.json");
        File.WriteAllText(filePath, jsonString);
    }

    static dynamic ReadJson(string filePath)
    {
        string jsonRead = File.ReadAllText(filePath);
        return JsonConvert.DeserializeObject<dynamic>(jsonRead);
    }


    static bool DirectoryContainsJsonFiles(string directoryPath)
    {
        return Directory.GetFiles(directoryPath, "*.json").Length > 0;

    }
    static void IncreaseTaxes(ref double budget, ref double happinessPopulation)
    {

        Console.WriteLine("Decide the % amount of taxes to increase but the rate of population happiness will decrease proportionally");

        int inputIncreaseTax = Convert.ToInt32(Console.ReadLine());

        budget *= 1 + inputIncreaseTax / 100.0;
        happinessPopulation *= 1 - inputIncreaseTax / 100.0;
        Console.WriteLine($"Now your budget is {budget} but the happiness of population is lower to {happinessPopulation}");
        UpdateJsonValues(kingdomFilePath, budget, happinessPopulation);

    }

    static void ReduceTaxes(ref double budget, ref double happinessPopulation)
    {

        Console.WriteLine("Decide the % amount of taxes to decrease  the rate of population happiness will increase but the budget will decrease proportionally");

        int inputIncreaseTax = Convert.ToInt32(Console.ReadLine());

        budget *= 1 - inputIncreaseTax / 100.0;
        happinessPopulation *= 1 + inputIncreaseTax / 100.0;
        Console.WriteLine($"Now your budget is {budget} and the happiness of population is higher {happinessPopulation}");
        UpdateJsonValues(kingdomFilePath, budget, happinessPopulation);

    }

    static void UpdateJsonValues(string filePath, double budget, double happinessPopulation)
    {
        var kingdom = ReadJson(filePath);
        kingdom.Budget = budget;
        kingdom.HappinessPopulation = happinessPopulation;
        WriteJson(kingdom, filePath);


    }

    static void DeleteAllJsonFiles(string directoryPath)
    {
        var files = Directory.GetFiles(directoryPath, "*.json");
        foreach (var file in files)
        {
            File.Delete(file);
        }

    }

    static void ChooseTheEnemy(ref double budget, ref double happinessPopulation)
    {
        Console.WriteLine("Choose the kingdom that you want to fight");
        Console.WriteLine("Digit 1 to choose Atlantis,2 to choose Magonia,3 to choose Dark Kingdom");
        int inputChoice = Convert.ToInt32(Console.ReadLine());
        string enemyKingdom = "";


        switch (inputChoice)
        {
            case 1:

                enemyKingdom = "Atlantis";



                break;

            case 2:

                enemyKingdom = "Magonia";

                break;

            case 3:

                enemyKingdom = "Dark Kingdom";

                break;

            default:

                Console.WriteLine("Invalid choice. Try again");

                return;
        }

        string enemyFilePath = SearchJson(enemyKingdom);

        if (enemyFilePath != null)
        {
            Console.WriteLine($"You have chosen to fight {enemyKingdom}");
            FightWar(ref budget, ref happinessPopulation, enemyFilePath);

        }
        else
        {
            Console.WriteLine("Enemy kingdom not found");
        }

    }

    static string SearchJson(string inputKingdom)
    {

        //string inputKingdom = Console.ReadLine();

        string searchPattern = $"{inputKingdom}_*.json";

        var matchingFiles = Directory.GetFiles(directoryPath, searchPattern);

        // Controlla corrispondenza pattern del file
        if (matchingFiles.Length == 0)
        {
            Console.WriteLine("Kingdom not found");
            return null;
        }
        // Return the first matching file (assuming there is only one)
        return matchingFiles[0];

    }

    static void Coup(ref double happinessPopulation,ref double budget){

        if(happinessPopulation < 20){
        Console.WriteLine("Attention!The kingdom population satisfaction is under 20%!");
        Console.WriteLine("They are staging a coup so you will lose the kingdom.You have to pay 200000 to stop this riot or you will lose the kingdom and the game(y/n)");
        string yourAnswer = Console.ReadLine().ToLower().Trim();

        if(yourAnswer == "y"){
        if( budget > 200000){

            budget-= 200000;
            happinessPopulation += 15;
            Console.WriteLine("You have successfully stopped the coup. Budget has been decreased by 200000 but happiness of population increased of 15 points.");


        }else{
             Console.WriteLine("You do not have enough budget to stop the coup so you will lose the kingdom. Game over.");
             budget =0;
        }
        }else if(yourAnswer == "n"){
            Console.WriteLine("Game over.You lost the kingdom and you will be beheaded");
             budget = 0;
        }else{
            Console.WriteLine("Invalid response.Give the proper answer y or n.");
        }

    }
    }
    static void createEnemyKingdom(dynamic Name, dynamic[] Regions, dynamic Budget, dynamic HappinessPopulation)
    {
        var kingdom = new
        {
            Name,
            Regions,
            Budget,
            HappinessPopulation,
            TimeStamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")
        };
        string filePath = Path.Combine(directoryPath, $"{kingdom.Name}_{kingdom.TimeStamp}.json");

        WriteJson(kingdom, filePath);
    }
}
