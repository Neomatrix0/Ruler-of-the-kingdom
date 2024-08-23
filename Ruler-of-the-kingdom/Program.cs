using System;
using System.IO;
using Newtonsoft.Json;

class Program
{
    static Random random = new Random(); // Global random object
    static string directoryPath = @"data/";  //directory for json files
    static bool kingdomCreated = false; //condition to evaluate if player has created its own kingdom to play
    static double happinessPopulation = 70; // Initial HappinessPopulation
    static string kingdomFilePath = "";  // variable used to store the file path where the json data of the player's kingdom is saved

    static string playerKingdomName = ""; // store the name of the player kingdom

    static List<Dictionary<string,dynamic>> playerArmy = new List<Dictionary<string,dynamic>>();
    static Dictionary<string,dynamic>[] availableUnits = {

        new Dictionary<string,dynamic> {{ "name","Infantry"},{"cost",100},{"strength",10},{"strongAgainst","Archers"}},
        new Dictionary<string,dynamic> {{"name","Cavalry"},{"cost",200},{"strength",15},{"strongAgainst","Infantry"}},
        new Dictionary<string,dynamic> {{"name","Archers"},{"cost",150},{"strength",12},{"strongAgainst","Cavalry"}}


    };

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
            Console.WriteLine("3. Buy units army");
            Console.WriteLine("4. Fight with enemy");
            Console.WriteLine("5. Increase taxes");
            Console.WriteLine("6. Reduce taxes\n");
            Console.WriteLine("7. Exit");

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
                BuyUnits(ref budget,playerArmy,availableUnits);
                  
                    break;

                case 4:

                    if (kingdomCreated)
                    {
                        ChooseTheEnemy(ref budget, ref happinessPopulation);


                        if(AllEnemiesDefeated())
                        {
                            Console.WriteLine("\nCongratulations you conquered the entire world.You won the game!");
                            choice = 3; // to exit the game
                            break;
                         }
                    }

                    else
                    {
                        Console.WriteLine("You must create a kingdom first.");
                    }

                    break;

                case 5:

                    IncreaseTaxes(ref budget, ref happinessPopulation);
                    UpdateJsonValues(kingdomFilePath, budget, happinessPopulation);

                    break;

                case 6:

                    ReduceTaxes(ref budget, ref happinessPopulation);

                    break;

                case 7:
                  Console.WriteLine("The game will be closed and the data deleted. Please wait...");
                    DeleteAllJsonFiles(directoryPath);
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
            if (choice != 7)
            {
                Console.WriteLine("\nPress a button to continue.");
                Console.ReadKey();
            }
        } while (choice != 7);
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
         playerKingdomName = inputName;
        kingdomFilePath = Path.Combine(directoryPath, $"{inputName}.json");

        if (File.Exists(kingdomFilePath))
        {
            Console.WriteLine($"Kingdom {inputName} has already been created.");
            return;
        }




      /*  Console.Write("Please insert here the name of the regions of your kingdom split by comma: ");
        string? inputRegions = Console.ReadLine();
        string[] regions = inputRegions.Split(',');

        if (regions.Length != 3)
        {
            throw new FormatException("Input must include only 3 names, each one split by comma.");
        } */

        var kingdom = new
        {
            Name = inputName,
           // Regions = regions,
            Budget = budget,
            HappinessPopulation = happinessPopulation,
            TimeStamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"),
            Defeated = false
        };

        WriteJson(kingdom, kingdomFilePath);
        Console.WriteLine($"Kingdom {kingdom.Name} data has been saved successfully!");
        createEnemyKingdom("Atlantis", 700000, 80);
        createEnemyKingdom("Magonia",  1000000, 80);
        createEnemyKingdom("Star Empire",1200000,90);
        createEnemyKingdom("Dark Kingdom", 1800000, 90);
        createEnemyKingdom("Dream Realm",2000000,110);


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
                Console.WriteLine($"Kingdom name: {kingdom.Name}, Budget: {kingdom.Budget}, HappinessPopulation: {kingdom.HappinessPopulation}\n");
            }
        }
        else
        {
            Console.WriteLine("No kingdoms found.\n");
        }
    }

    static void FightWar(ref double budget, ref double happinessPopulation, string enemyFilePath)
    {
        Thread.Sleep(1000);

        var enemyArmy = CreateEnemyArmy(500000,availableUnits); // create enemy army

        int playerStrength = CalculateArmyStrength(playerArmy,enemyArmy);

        int enemyStrength = CalculateArmyStrength(enemyArmy,playerArmy);

        int playerDiceRoll1 = random.Next(1, 7);
        int playerDiceRoll2 = random.Next(1, 7);
        int sumPlayerRolls = playerDiceRoll1 + playerDiceRoll2;
        Console.WriteLine($"Player dice rolls: {playerDiceRoll1} and {playerDiceRoll2} (Total score: {sumPlayerRolls})");

        int pcDiceRoll1 = random.Next(1, 7);
        int pcDiceRoll2 = random.Next(1, 7);
        int sumPcRolls = pcDiceRoll1 + pcDiceRoll2;
        Console.WriteLine($"Enemy dice rolls: {pcDiceRoll1} and {pcDiceRoll2} (Total score: {sumPcRolls})");

        // add to the dices results the army strength

        playerStrength += sumPlayerRolls;
        enemyStrength += sumPcRolls;

        Console.WriteLine($"Your total strength (army + dice): {playerStrength}");
        Console.WriteLine($"Enemy total strength (army + dice): {enemyStrength}");

        // set the winner

        var enemyKingdom = ReadJson(enemyFilePath);
        if (playerStrength > enemyStrength)
        {
            budget *= 1.20;  // Increase budget by 20%
            happinessPopulation += 10; // Increase happiness population by 10
            Thread.Sleep(1000);
            Console.WriteLine("\nCongratulations, you won! War costs will be repaid, and you will earn 15% more budget and increase happiness population by 10.");
            enemyKingdom.Defeated = true;

        }
        else
        {
            budget -= 300000;  // Loss  budget
            happinessPopulation -= 15; // Decrease happiness population by 15
            Thread.Sleep(1000);
            Console.WriteLine("\nThe enemy has won. You lost 300000 points  and 15 points of happiness population.");
        }
        WriteJson(enemyKingdom, enemyFilePath);
        UpdateJsonValues(kingdomFilePath, budget, happinessPopulation);
    }

    static int CalculateArmyStrength(List<Dictionary<string,dynamic>>army,List<Dictionary<string,dynamic>>opposingArmy){

        int totalStrength =0;
        foreach(var unit in army){
            int strength = unit["strength"];
            //verify if this army unit is stronger than the enemy's unit
            foreach(var opposingUnit in opposingArmy){
                if(unit["strongAgainst"] == opposingUnit["name"]){
                    strength +=5;  // strength bonus if unit is stronger against the enemy's unit


                }
            }

            totalStrength += strength;

        }

        return totalStrength;

    }

// function to buy units
    static void BuyUnits(ref double budget, List<Dictionary<string,dynamic>> playerArmy, Dictionary<string,dynamic>[]availableUnits){
        

        Console.WriteLine("Choose a unit to buy:");

        for(int i =0;i < availableUnits.Length;i++){
            Console.WriteLine($"{i+1}. {availableUnits[i]["name"]} (Cost: {availableUnits[i]["cost"]}, Strength: {availableUnits[i]["strength"]})");
        }
         int choice = Convert.ToInt32(Console.ReadLine());

         if(choice < 1 || choice > availableUnits.Length){
            Console.WriteLine("Invalid choice");
            return;
         }

         var chosenUnit = availableUnits[choice-1];
         if(budget >= chosenUnit["cost"]){
            budget -= chosenUnit["cost"];
            playerArmy.Add(chosenUnit);
            Console.WriteLine($"You have bought {chosenUnit["name"]}. Remaining budget: {budget}");
         }else{
            Console.WriteLine("Not enough budget to buy this unit.");
         }
    }

    static List<Dictionary<string,dynamic>> CreateEnemyArmy(double enemyBudget,Dictionary<string,dynamic>[]availableUnits ){
        List<Dictionary<string,dynamic>> enemyArmy = new List<Dictionary<string,dynamic>>();
        while(enemyBudget >0){
            int index = random.Next(availableUnits.Length);
            var unitToBuy = availableUnits[index];

            if(enemyBudget >= unitToBuy["cost"]){
                enemyArmy.Add(unitToBuy);
                enemyBudget -= unitToBuy["cost"];

            }else{
                break;  // exit from loop if the budget is not enough
            }

        }
        return enemyArmy;
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
        Console.WriteLine("Digit 1 to choose Atlantis,2 to choose Magonia,3 Star Empire,4 to choose Dark Kingdom,5 Dream Realm");
        int inputChoice = Convert.ToInt32(Console.ReadLine());

        string[] kingdoms = { "Atlantis", "Magonia", "Star Empire", "Dark Kingdom", "Dream Realm" };

        if (inputChoice < 1 || inputChoice > kingdoms.Length)
        {
            Console.WriteLine("Invalid choice. Try again.");
            return;
        }
        
        string enemyKingdom = kingdoms[inputChoice - 1];
        string enemyFilePath = SearchJson(enemyKingdom);

        if (enemyFilePath != null)
        {
            var enemyData = ReadJson(enemyFilePath);
            if (enemyData.Defeated == true)
            {
                Console.WriteLine($"You have already defeated {enemyKingdom}. Choose another enemy.");
                return;
            }


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

    static void Coup(ref double happinessPopulation, ref double budget)
    {

        if (happinessPopulation < 20)
        {
            Console.WriteLine("Attention!The kingdom population satisfaction is under 20%!");
            Console.WriteLine("They are staging a coup so you will lose the kingdom.You have to pay 200000 to stop this riot or you will lose the kingdom and the game(y/n)");
            string yourAnswer = Console.ReadLine().ToLower().Trim();

            if (yourAnswer == "y")
            {
                if (budget > 200000)
                {

                    budget -= 200000;
                    happinessPopulation += 15;
                    Console.WriteLine("You have successfully stopped the coup. Budget has been decreased by 200000 but happiness of population increased of 15 points.");


                }
                else
                {
                    Console.WriteLine("You do not have enough budget to stop the coup so you will lose the kingdom. Game over.");
                    budget = 0;
                }
            }
            else if (yourAnswer == "n")
            {
                Console.WriteLine("Game over.You lost the kingdom and you will be beheaded");
                budget = 0;
            }
            else
            {
                Console.WriteLine("Invalid response.Give the proper answer y or n.");
            }

        }
    }
    static void createEnemyKingdom(dynamic Name, dynamic Budget, dynamic HappinessPopulation)
    {
        var kingdom = new
        {
            Name,
            Budget,
            HappinessPopulation,
            TimeStamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"),
            Defeated = false
        };
        string filePath = Path.Combine(directoryPath, $"{kingdom.Name}_{kingdom.TimeStamp}.json");

        WriteJson(kingdom, filePath);
    }

    static bool AllEnemiesDefeated(){
        var files = Directory.GetFiles(directoryPath,"*.json");
        foreach(var file in files){
            var kingdom = ReadJson(file);
            if(kingdom.Defeated == false && kingdom.Name != playerKingdomName ){
                return false;

            }
        }
        return true; // all enemies are defeated
    } 
}
