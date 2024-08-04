# RULER OF THE KINGDOM

Il gioco è basato sulla gestione del regno
L'obiettivo è quello di mantenere il proprio regno e allo stesso tempo di espandersi conquistando altri regni.
Bisogna sempre tenere a mente che bisogna trovare un equilibrio tra tasso di felicità della popolazione e conquiste per non perdere il regno e subire un colpo di stato.


# DEFINIZIONE DEI REQUISITI E ANALISI

L'applicazione dovrà consentire al giocatore di espandersi e acquistare i vari regni senza perdere il proprio.
Le battaglie si risolveranno tramite il lancio dei dadi e termineranno con la perdita o la conquista delle varie regioni.
Se il giocatore perderà tutte le regioni perderà la partita.
Il giocatore dovrà fare attenzione a tutti i parametri del regno dal budget,ai territori al tasso di felicità della popolazione.
L'applicazione gestirà tali parametri e li memorizzerà anche tramite la persistenza dei dati.
Se il giocatore dovesse conquistare tutti i regni vincerà la partita.




# PIANIFICAZIONE E DESIGN DELL'ARCHITETTURA

- [ ] Creazione di un menu generale con opzioni di scelta

- [ ] Creazione del proprio regno sarà un oggetto generico

- [ ] Le caratteristiche dell'oggetto saranno: nome regno,tipo di regno(Principato,repubblica),budget di default di 1000000 da destinare a guerre o al popolo,tasso di soddifazione del popolo da 0 a 100,tasse,esercito,nome delle 3 regioni del regno.

- [ ] Il tipo di regno scelto se è un principato aumenterà il budget a disposizione ma il valore di default del tasso di felicità sarà più basso viceversa se si sceglie una repubblica il tasso di felicità aument

- [ ] Ogni regno è diviso in tre regioni.Se tutte e tre le regioni vengono sconfitte hai acquisito il regno avversario

- [ ] Esistono in tutto 4 regni compreso quello creato dall'utente

- [ ] dinamica lancio dadi.Il primo giocatore tira 2 dadi  al turno successivo il secondo giocatore ovvero il pc lancerà 2 dadi.Dopo 2 giocate chi vince acquisterà la regione avversaria con un incremento del budget e del tasso di felicità.Se si conquisteranno tutte e 3 le regioni si otterrà il regno con un incremento ancora superiore del budget e del tasso di felicità della popolazione  

- [ ] Sistema di punteggio del regno per ogni regione conquistata se conquisti tutto il regno punti extra 


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

- [ ] se una regione viene conquistata si recuperano le spese e aumenta il budget del 10%

- [ ] implementare persistenza dei dati


# TERZA VERSIONE

- [ ]  Implementazione delle regioni

- [ ]  possibilità di scegliere quale regione attaccare con un sottomenu per scegliere quale regione sfidare

- [ ]  Implementazione della logica dei dadi applicata alle singole regioni invece che al regno

- [ ] Implementazione di una logica politica se il tasso di felicità da 0 a 100 scende sotto il 20% ci sarà una rivolta e verranno perse 2 regioni

- [ ] valutare se implementare un advisor  per ricevere dei suggerimenti sulla guerra o sulla gestione del regno

- [ ] Riducendo le tasse, oppure vincendo delle regioni aumenterà il tasso di felicità della popolazione

- [ ] se tutti i regni vengono conquistati la partita finisce con un determinato score .

- [ ] se si perde rimane lo score

- [ ] lo score verrà registrato in un file


# QUARTA VERSIONE IMPLEMENTAZIONE DI SPECTRE 

- [ ] Implementare spectre per il menu e sottomenu

- [ ] creazione di tabelle per descrivere vincite e dati 

# QUINTA VERSIONE 

- [ ] Implementare MapAscii

- [ ] installare telnet mapscii.me



```mermaid



```

