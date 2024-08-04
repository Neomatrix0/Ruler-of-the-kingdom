using System;
using System.IO;
using Newtonsoft.Json;

class Program
{
    static Random random = new Random(); // Global random object
    static string directoryPath = @"data/";
    static bool kingdomCreated = false;
    static double happinessPopulation = 70; // Initial HappinessPopulation

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
            Console.WriteLine("4. Fight with enemy\n");

            choice = Convert.ToInt32(Console.ReadLine());

            switch (choice)
            {
                case 1:
                    if (!kingdomCreated)
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
                    Console.WriteLine("The game will be closed. Please wait...");
                    break;

                case 4:
                    if (ConfirmWar())
                    {
                        FightWar(ref budget, ref happinessPopulation);
                        Console.WriteLine($"Updated Budget After War: {budget}");
                        Console.WriteLine($"Updated Happiness Population After War: {happinessPopulation}");
                    }
                    break;

                default:
                    Console.WriteLine("Make the right choice.");
                    break;
            }

            Console.WriteLine($"Current Budget: {budget}");
            Console.WriteLine($"Current Happiness Population: {happinessPopulation}");
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
        string filePath = Path.Combine(directoryPath, $"{inputName}.json");

        if (File.Exists(filePath))
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

        WriteJson(kingdom);
        Console.WriteLine($"Kingdom {kingdom.Name} data has been saved successfully!");
        createEnemyKingdom("Atlantis", new string[] { "Red", "Wald", "Oceania" }, 800000, 80);
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

    static void FightWar(ref double budget, ref double happinessPopulation)
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
            Console.WriteLine("The enemy has won. You lost your reign and 10 happiness population. Game over.");
        }
    }

    static void WriteJson(dynamic kingdom)
    {
        string jsonString = JsonConvert.SerializeObject(kingdom, Formatting.Indented);
        string filePath = Path.Combine(directoryPath, $"{kingdom.Name}_{kingdom.TimeStamp}.json");
        File.WriteAllText(filePath, jsonString);
    }

    static dynamic ReadJson(string filePath)
    {
        string jsonRead = File.ReadAllText(filePath);
        return JsonConvert.DeserializeObject<dynamic>(jsonRead);
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

        WriteJson(kingdom);
    }
}
