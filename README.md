# RULER OF THE KINGDOM

Il gioco è basato sulla gestione del regno
L'obiettivo è quello di mantenere il proprio regno e allo stesso tempo di espandersi conquistando altri regni.
Bisogna sempre tenere a mente che bisogna trovare un equilibrio tra tasso di felicità della popolazione e conquiste per non perdere il regno e subire un colpo di stato.


# DEFINIZIONE DEI REQUISITI E ANALISI

L'applicazione dovrà consentire al giocatore di espandersi e acquistare i vari regni senza perdere il proprio.
Le battaglie si risolveranno tramite il lancio dei dadi e il calcolo della potenza dell'esercito, termineranno con  la conquista dei regni avversari o con la perdita del proprio regno.

Il giocatore dovrà fare attenzione a tutti i parametri del regno ovvero budget, tasso di felicità della popolazione ed esercito.
L'applicazione gestirà tali parametri e li memorizzerà anche tramite la persistenza dei dati.
Se il giocatore dovesse conquistare tutti i regni vincerà la partita.
Se il proprio budget dovesse scendere a 0 perderà la partita.




# PIANIFICAZIONE E DESIGN DELL'ARCHITETTURA

- [ ] Creazione di un menu generale con opzioni di scelta

- [ ] Creazione del proprio regno sarà un oggetto generico

- [ ] Le caratteristiche dell'oggetto saranno: nome regno,tipo di regno(Principato,repubblica),budget di default di 1000000 da destinare a guerre o al popolo,tasso di soddifazione del popolo da 0 a 100,tasse,esercito

- [ ] Il tipo di regno scelto se è un principato aumenterà il budget a disposizione ma il valore di default del tasso di felicità sarà più basso viceversa se si sceglie una repubblica il tasso di felicità aumenterà

- [ ] Esistono in tutto 6 regni compreso quello creato dall'utente

- [ ] Possibilità di creare un esercito acquistando singole unità.La potenza dell'esercito influisce sull'esito dei conflitti tramite punti bonus

- [ ] dinamica lancio dadi.Il primo giocatore tira 2 dadi  al turno successivo il secondo giocatore ovvero il pc lancerà 2 dadi.Dopo 2 giocate chi vince acquisterà la regione avversaria con un incremento del budget e del tasso di felicità.

- [ ]  Possibilità d'influire su budget e tasso di felicità aumentando o riducendo le tasse

- [ ]  Possibilità di subire un colpo di stato


# PRIMA VERSIONE SEMPLIFICATA

- [ ] Creazione di un menu con le opzioni di scelta
    - [ ] inizio Creazione regno ->
    - [ ] fine

- [ ] Opzione gioca: chiede di creare il proprio regno con le caratteristiche previste.Verrà creato un file json con le caratteristiche del regno messe dall' utente

- [ ] Le caratteristiche dell'oggetto saranno: nome regno,tipo di regno(Principato,repubblica),budget da destinare a guerre o al popolo,tasso di soddifazione del popolo da 0 a 100,tasse,esercito,nome delle 3 regioni del regno.

- [ ] Opzione esci dal gioco

- [ ] Creazione di un regno di default da sfidare implementare subito una funzione per creare anche i regni futuri usando i parametri

- [ ] Lancio di 2 dadi ciascuno.Ogni 2 vittorie si acquista un regno

<details>
<summary>Visualizza codice</summary>

```csharp

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


```
</details>

# SECONDA VERSIONE 

- [ ] Creazione di altri 2 regni di default da sfidare 

- [ ] OpzioneMenu di scelta del regno da sfidare 

- [ ] Funzione di visualizzazione delle caratteristiche del proprio regno

- [ ] Funzioni per aumentare e diminuire le tasse in percentuale con impatto sul tasso di felicità


- [ ] vengono sottratti fondi pubblici e aumentate le tasse tutte le volte che si fa guerra riducendo il tasso di felicità della popolazione

- [ ] se un regno viene conquistata si recuperano le spese e aumenta il budget del 10%

- [ ] implementare persistenza dei dati

<details>
<summary>Visualizza codice</summary>

```csharp

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



```

</details>

<summary>Versione modificata</summary>

<details>

```csharp

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
            budget *= 1.20;  // Increase budget by 20%
            happinessPopulation += 10; // Increase happiness population by 10
            Console.WriteLine("Congratulations, you won! War costs will be repaid, and you will earn 15% more budget and increase happiness population by 10.");

        }
        else
        {
            budget -= 300000;  // Loss  budget
            happinessPopulation -= 15; // Decrease happiness population by 15
            Console.WriteLine("The enemy has won. You lost 300000 points  and 15 points of happiness population.");
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

```
</details>


# TERZA VERSIONE

- [ ] Miglioramento persistenza dei dati

- [ ] Implementazione logica di vittoria se tutti i regni sono stati battuti e di sconfitta del giocatore per concludere la partita

- [ ] Aggiunta di funzionalità come il colpo di stato se il tasso di felicità scende sotto ad un certo livello


<summary>Visualizza codice</summary>

<details>

```csharp

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
         playerKingdomName = inputName;
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
            TimeStamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"),
            Defeated = false
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
        var enemyKingdom = ReadJson(enemyFilePath);
        if (sumPlayerRolls > sumPcRolls)
        {
            budget *= 1.20;  // Increase budget by 20%
            happinessPopulation += 10; // Increase happiness population by 10
            Console.WriteLine("Congratulations, you won! War costs will be repaid, and you will earn 15% more budget and increase happiness population by 10.");
            enemyKingdom.Defeated = true;

        }
        else
        {
            budget -= 300000;  // Loss  budget
            happinessPopulation -= 15; // Decrease happiness population by 15
            Console.WriteLine("The enemy has won. You lost 300000 points  and 15 points of happiness population.");
        }
        WriteJson(enemyKingdom, enemyFilePath);
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

        string[] kingdoms = { "Atlantis", "Magonia", "Dark Kingdom" };

        if (inputChoice < 1 || inputChoice > kingdoms.Length)
        {
            Console.WriteLine("Invalid choice. Try again.");
            return;
        }
        string enemyKingdom = "";
        string chosenKingdom = kingdoms[inputChoice - 1];
        string enemyFilePath = SearchJson(chosenKingdom);

        if (enemyFilePath != null)
        {
            var enemyData = ReadJson(enemyFilePath);
            if (enemyData.Defeated == true)
            {
                Console.WriteLine($"You have already defeated {chosenKingdom}. Choose another enemy.");
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
    static void createEnemyKingdom(dynamic Name, dynamic[] Regions, dynamic Budget, dynamic HappinessPopulation)
    {
        var kingdom = new
        {
            Name,
            Regions,
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




```



</details>



# QUARTA VERSIONE


- [ ]  Creazione dell'esercito e possibilità di acquistare unità (fanteria,cavalleria,arcieri,maghi) con relativi punti di forza

- [ ]  Calcolo della  potenza dell' esercito e Implementazione della logica  dei dadi nelle guerre

- [ ] Implementazione di una logica politica se il tasso di felicità da 0 a 100 scende sotto il 20% ci sarà una rivolta 


- [ ] se tutti i regni vengono conquistati la partita finisce

<summary>Visualizza codice</summary>

<details>

```csharp

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

    // List to hold the player's army

    static List<Dictionary<string, dynamic>> playerArmy = new List<Dictionary<string, dynamic>>();

    // Available army units for purchase with their respective properties
    static Dictionary<string, dynamic>[] availableUnits = {

        new Dictionary<string,dynamic> {{ "name","Infantry"},{"cost",1000},{"strength",10},{"strongAgainst","Archers"}},
        new Dictionary<string,dynamic> {{"name","Cavalry"},{"cost",2000},{"strength",15},{"strongAgainst","Infantry"}},
        new Dictionary<string,dynamic> {{"name","Archers"},{"cost",1500},{"strength",12},{"strongAgainst","Cavalry"}},
        new Dictionary<string,dynamic> {{"name","Wizards"},{"cost",3000},{"strength",20},{"strongAgainst","Archers"}}

    };

    static void Main(string[] args)
    {
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }


        // Check if a kingdom JSON file already exists
        // Load an existing kingdom if it exists, otherwise start from scratch
        var existingKingdomFiles = Directory.GetFiles(directoryPath, "*.json");
        if (existingKingdomFiles.Length > 0)
        {
            foreach (var file in existingKingdomFiles)
            {
                var kingdom = ReadJson(file);

                // Skip enemy kingdoms and check if the player's kingdom is already created

                if (kingdom.Defeated == false && kingdom.Name != "Atlantis" && kingdom.Name != "Magonia" &&
                    kingdom.Name != "Star Empire" && kingdom.Name != "Dark Kingdom" && kingdom.Name != "Dream Realm")
                {
                    // Load the first non-defeated player's kingdom found
                    kingdomFilePath = file;
                    playerKingdomName = kingdom.Name;
                    happinessPopulation = kingdom.HappinessPopulation;
                    kingdomCreated = true;
                    Console.WriteLine($"Loaded existing kingdom: {playerKingdomName} with a budget of {kingdom.Budget} and happiness of {happinessPopulation}.");
                    break;
                }
            }
        }

        double budget = 1000000;  // Initial budget
        Console.WriteLine("Initial Budget: " + budget);


        if (!kingdomCreated)
        {
            // Loop until the kingdom is created
            while (!kingdomCreated)
            {
                Console.WriteLine("\nWelcome to the game Ruler of the Kingdom!");
                Console.WriteLine("You must create a kingdom before performing any other actions.");
                Console.WriteLine("1. Give a name to your kingdom");
                Console.WriteLine("2. Exit");

                int initialChoice = Convert.ToInt32(Console.ReadLine());

                switch (initialChoice)
                {
                    case 1:
                        CreateYourOwnKingdom(ref budget);   // Create a new kingdom
                        kingdomCreated = true;
                        break;
                    case 2:
                        Console.WriteLine("Exiting game. Goodbye!");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please select 1 to create a kingdom or 2 to exit.");
                        break;
                }
            }
        }


        // kingdom is created now appear the menu

        int choice;
        do
        {
            Console.WriteLine("\nWelcome to the game Ruler of the Kingdom Main Menu!");
            Console.WriteLine("Every path begins with a choice\n");
            // Console.WriteLine("1. Welcome");
            Console.WriteLine("2. View all kingdoms");
            Console.WriteLine("3. Buy units army");
            Console.WriteLine("4. Fight with enemy");
            Console.WriteLine("5. Increase taxes");
            Console.WriteLine("6. Reduce taxes");
            Console.WriteLine("7. Exit\n");

            choice = Convert.ToInt32(Console.ReadLine());

            switch (choice)
            {
                case 1:
                    /*  if (!kingdomCreated && !DirectoryContainsJsonFiles(directoryPath))
                      {
                          CreateYourOwnKingdom(ref budget);
                          kingdomCreated = true;
                      }
                      else
                      {
                          Console.WriteLine("A kingdom has already been created. You can't create another one until you win or lose the match.");
                      }*/
                    Console.WriteLine("Welcome to the game!");
                    break;

                case 2:
                    ViewAllKingdoms();
                    break;

                case 3:
                    BuyUnits(ref budget, playerArmy, availableUnits);

                    break;

                case 4:

                    if (kingdomCreated)
                    {
                        ChooseTheEnemy(ref budget, ref happinessPopulation);


                        if (AllEnemiesDefeated())
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

            // Check for game over conditions

            if (budget == 0)
            {
                Console.WriteLine("Your budget has reached zero the kingdom declared default hence you will be beheaded. Game over.");
                break;
            }

            if (happinessPopulation < 20)
            {
                Coup(ref happinessPopulation, ref budget);   // Handle coup if happiness is too low
                UpdateJsonValues(kingdomFilePath, budget, happinessPopulation);
            }


            if (choice != 7)
            {
                Console.WriteLine("\nPress a button to continue.");
                Console.ReadKey();
            }
        } while (choice != 7);      // Loop until the player chooses to exit
    }
    // Function to confirm war
    static bool ConfirmWar()
    {
        Console.WriteLine("The war will cost you 15% of your budget. Do you want to proceed? y/n");
        string answer = Console.ReadLine().ToLower().Trim();
        return answer == "y";
    }

    // Function to create a new kingdom
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

        // Create a new kingdom object and save it to a JSON file
        var kingdom = new
        {
            Name = inputName,
            Budget = budget,
            HappinessPopulation = happinessPopulation,
            TimeStamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"),
            Defeated = false
        };

        WriteJson(kingdom, kingdomFilePath);
        Console.WriteLine($"Kingdom {kingdom.Name} data has been saved successfully!");

        // Create predefined enemy kingdoms
        createEnemyKingdom("Atlantis", 700000, 80);
        createEnemyKingdom("Magonia", 1000000, 80);
        createEnemyKingdom("Star Empire", 1200000, 90);
        createEnemyKingdom("Dark Kingdom", 1800000, 90);
        createEnemyKingdom("Dream Realm", 2000000, 110);

    }
    // Function to view all kingdoms and their details
    static void ViewAllKingdoms()
    {
        var files = Directory.GetFiles(directoryPath, "*.json");
        if (files.Length > 0)
        {
            Console.WriteLine("Complete list of all kingdoms:\n");
            foreach (var file in files)
            {
                var kingdom = ReadJson(file);
                bool isDefeated = kingdom.Defeated;
                Console.WriteLine($"Kingdom name: {kingdom.Name}, Budget: {kingdom.Budget}, HappinessPopulation: {kingdom.HappinessPopulation}, Defeated: {isDefeated}\n");

                // If it's the player's kingdom, show additional details

                if (kingdom.Name == playerKingdomName)
                {

                    // Display the player's army details
                    Console.WriteLine("--- Your Army Details ---");
                    DisplayArmyDetails(playerArmy);
                }
            }
        }
        else
        {
            Console.WriteLine("No kingdoms found.\n");
        }
    }
    // Function to handle a war between the player's kingdom and an enemy kingdom
    static void FightWar(ref double budget, ref double happinessPopulation, string enemyFilePath)
    {

        Thread.Sleep(1000);

        var enemyArmy = CreateEnemyArmy(500000, availableUnits); // create enemy army

        int playerStrength = CalculateArmyStrength(playerArmy, enemyArmy);

        int enemyStrength = CalculateArmyStrength(enemyArmy, playerArmy);
        // Roll the dice for both player and enemy
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

    // Function to display the player's army details

    static void DisplayArmyDetails(List<Dictionary<string, dynamic>> army)
    {
        if (army.Count > 0)
        {
            int totalStrength = 0;
            Dictionary<string, int> unitCount = new Dictionary<string, int>();

            // Calculate total strength and unit counts
            foreach (var unit in army)
            {
                totalStrength += unit["strength"];
                if (unitCount.ContainsKey(unit["name"]))
                {
                    unitCount[unit["name"]]++;
                }
                else
                {
                    unitCount[unit["name"]] = 1;
                }
            }

            // Display each unit type and their count
            foreach (var unitType in unitCount)
            {
                Console.WriteLine($"{unitType.Value} x {unitType.Key} (Strength: {availableUnits.First(u => u["name"] == unitType.Key)["strength"]})");
            }
            Console.WriteLine($"Total Army Strength: {totalStrength}\n");
        }
        else
        {
            Console.WriteLine("You have no units in your army.\n");
        }
    }
    // Function to calculate the strength of an army
    static int CalculateArmyStrength(List<Dictionary<string, dynamic>> army, List<Dictionary<string, dynamic>> opposingArmy)
    {

        int totalStrength = 0;
        foreach (var unit in army)
        {
            int strength = unit["strength"];
            //verify if this army unit is stronger than the enemy's unit
            foreach (var opposingUnit in opposingArmy)
            {
                if (unit["strongAgainst"] == opposingUnit["name"])
                {
                    strength += 5;  // strength bonus if unit is stronger against the enemy's unit


                }
            }

            totalStrength += strength;

        }

        return totalStrength;

    }

    // function to buy units
    static void BuyUnits(ref double budget, List<Dictionary<string, dynamic>> playerArmy, Dictionary<string, dynamic>[] availableUnits)
    {
        //  if (!EnsureKingdomCreated()) return;
        bool continueBuying = true;

        while (continueBuying)
        {
            // Display the current budget
            Console.WriteLine($"\nYour current budget: {budget}\n");

            Console.WriteLine("Choose a unit to buy:");

            for (int i = 0; i < availableUnits.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {availableUnits[i]["name"]} (Cost: {availableUnits[i]["cost"]}, Strength: {availableUnits[i]["strength"]})");
            }
            int choice = Convert.ToInt32(Console.ReadLine());

            if (choice < 1 || choice > availableUnits.Length)
            {
                Console.WriteLine("Invalid choice");
                return;
            }

            var chosenUnit = availableUnits[choice - 1];

            Console.Write($"How many {chosenUnit["name"]} units would you like to buy? ");

            int quantity = Convert.ToInt32(Console.ReadLine());

            double totalCost = chosenUnit["cost"] * quantity;

            // Check if the player has enough budget to buy the units
            if (budget >= totalCost)
            {
                budget -= totalCost;

                for (int i = 0; i < quantity; i++)
                {
                    //add unity to the army for each quantity purchased
                    playerArmy.Add(new Dictionary<string, dynamic>(chosenUnit));
                }
                Console.WriteLine($"You have bought {quantity} {chosenUnit["name"]} units. Remaining budget: {budget}");
                // Display the updated army details after purchasing
                Console.WriteLine("\n--- Your Updated Army Details ---");
                DisplayArmyDetails(playerArmy);
            }
            else
            {
                Console.WriteLine("Not enough budget to buy these units.");
            }
            // Ask if the player wants to buy more units
            Console.WriteLine("Do you want to buy more units? (y/n)");
            string response = Console.ReadLine().Trim().ToLower();
            if (response != "y")
            {
                continueBuying = false;
            }
        }
    }
    // Function to create an enemy army
    static List<Dictionary<string, dynamic>> CreateEnemyArmy(double enemyBudget, Dictionary<string, dynamic>[] availableUnits)
    {
        List<Dictionary<string, dynamic>> enemyArmy = new List<Dictionary<string, dynamic>>();
        while (enemyBudget > 0)
        {
            int index = random.Next(availableUnits.Length);
            var unitToBuy = availableUnits[index];

            if (enemyBudget >= unitToBuy["cost"])
            {
                enemyArmy.Add(unitToBuy);
                enemyBudget -= unitToBuy["cost"];

            }
            else
            {
                break;  // exit from loop if the budget is not enough
            }

        }
        return enemyArmy;
    }

    // Function to write kingdom data to a JSON file
    static void WriteJson(dynamic kingdom, string filePath)
    {
        string jsonString = JsonConvert.SerializeObject(kingdom, Formatting.Indented);
        //string filePath = Path.Combine(directoryPath, $"{kingdom.Name}_{kingdom.TimeStamp}.json");
        File.WriteAllText(filePath, jsonString);
    }
    // Function to read kingdom data from a JSON file
    static dynamic ReadJson(string filePath)
    {
        string jsonRead = File.ReadAllText(filePath);
        return JsonConvert.DeserializeObject<dynamic>(jsonRead);
    }
    // Function to ensure that a kingdom has been created before performing any action
    static bool EnsureKingdomCreated()
    {
        if (!kingdomCreated || string.IsNullOrEmpty(kingdomFilePath))
        {
            Console.WriteLine("You must create a kingdom before to perform an action\n");
            return false;
        }
        //if kingdom exists proceed
        return true;
    }

    // Function to check if the data directory contains any JSON files
    static bool DirectoryContainsJsonFiles(string directoryPath)
    {
        return Directory.GetFiles(directoryPath, "*.json").Length > 0;

    }
    // Function to increase taxes and adjust the budget and happiness accordingly
    static void IncreaseTaxes(ref double budget, ref double happinessPopulation)
    {

        if (!EnsureKingdomCreated()) return;
        Console.WriteLine("Decide the % amount of taxes to increase but the rate of population happiness will decrease proportionally");

        int inputIncreaseTax = Convert.ToInt32(Console.ReadLine());

        budget *= 1 + inputIncreaseTax / 100.0;
        happinessPopulation *= 1 - inputIncreaseTax / 100.0;
        Console.WriteLine($"Now your budget is {budget} but the happiness of population is lower to {happinessPopulation}");
        UpdateJsonValues(kingdomFilePath, budget, happinessPopulation);

    }

    // Function to reduce taxes and adjust the budget and happiness accordingly
    static void ReduceTaxes(ref double budget, ref double happinessPopulation)
    {
        if (!EnsureKingdomCreated()) return;

        Console.WriteLine("Decide the % amount of taxes to decrease  the rate of population happiness will increase but the budget will decrease proportionally");

        int inputIncreaseTax = Convert.ToInt32(Console.ReadLine());

        budget *= 1 - inputIncreaseTax / 100.0;
        happinessPopulation *= 1 + inputIncreaseTax / 100.0;
        Console.WriteLine($"Now your budget is {budget} and the happiness of population is higher {happinessPopulation}");
        UpdateJsonValues(kingdomFilePath, budget, happinessPopulation);

    }
    // Function to update the kingdom data in the JSON file
    static void UpdateJsonValues(string filePath, double budget, double happinessPopulation)
    {
        var kingdom = ReadJson(filePath);
        kingdom.Budget = budget;
        kingdom.HappinessPopulation = happinessPopulation;
        WriteJson(kingdom, filePath);


    }

    // Function to delete all JSON files in the data directory
    static void DeleteAllJsonFiles(string directoryPath)
    {
        var files = Directory.GetFiles(directoryPath, "*.json");
        foreach (var file in files)
        {
            File.Delete(file);
        }

    }
    // Function to choose an enemy kingdom to fight
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
    // Function to search for a kingdom's JSON file by name
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
    // Function to handle a coup if the population's happiness is too low
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
    // Function to create predefined enemy kingdoms
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
    // Function to check if all enemy kingdoms have been defeated
    static bool AllEnemiesDefeated()
    {
        var files = Directory.GetFiles(directoryPath, "*.json");
        foreach (var file in files)
        {
            var kingdom = ReadJson(file);
            if (kingdom.Defeated == false && kingdom.Name != playerKingdomName)
            {
                return false;

            }
        }
        return true; // all enemies are defeated
    }
}




```

</details>


# QUINTA VERSIONE IMPLEMENTAZIONE DI SPECTRE 

- [ ] Implementare spectre per il menu e sottomenu

- [ ] creazione di tabelle per descrivere vincite e dati 



# SESTA VERSIONE 

- [ ] Implementare MapAscii

- [ ] installare telnet mapscii.me

- [ ] valutare se implementare un advisor  per ricevere dei suggerimenti sulla guerra o sulla gestione del regno



```mermaid



```

