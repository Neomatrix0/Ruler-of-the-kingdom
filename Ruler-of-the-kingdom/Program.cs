using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

class Program
{

    // create folder data where the datas of kingdom will be inserted
    static string directoryPath = @"data/";
    static bool kingdomCreated = false;
    static void Main(string[] args)
    {

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        double budget = 1000000;
        int choice;

        do
        {
            Console.WriteLine("Welcome to the game Ruler of the Kingdom Main Menu!");
            //Console.WriteLine("Main menu");
            Console.WriteLine("Every path begins with a choice\n");
            Console.WriteLine("1. Create your own kingdom");
            Console.WriteLine("2. View all kingdoms");
            Console.WriteLine("3. Exit");
            Console.WriteLine("4. Fight with enemy\n");

            choice = Convert.ToInt32(Console.ReadLine());

            switch (choice)
            {

                case 1:
                if(!kingdomCreated){

                    CreateYourOwnKingdom(budget);

           
                }else{
                    Console.WriteLine("Your kingdom has been already created.You can't create another one until you win or lose the match");
                }


                    break;

                case 2:
                    Console.WriteLine("View kingdoms stats");
                    ViewAllKingdoms();

                    break;

                case 3:

                    Console.WriteLine("The game will be closed.Please wait...");

                    break;

                case 4:

                Console.WriteLine("The war begins");
                break;

                default:

                    Console.WriteLine("Make the right choice");
                    break;

            }

            if (choice != 3)
            {
                Console.WriteLine("\nPress a button to continue");
                Console.ReadKey();

            }

        } while (choice != 3) ;
        }


    // function to create a default enemy kingdom 
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

            string jsonString = JsonConvert.SerializeObject(kingdom,Formatting.Indented);
            string filePath = Path.Combine(directoryPath, $"{kingdom.Name}_{kingdom.TimeStamp}.json");
            File.WriteAllText(filePath,jsonString);
        }


    // method to view all kingdoms stats

        static void ViewAllKingdoms(){
             var files = Directory.GetFiles(directoryPath, "*.json"); 
              if (files.Length > 0)
        {
            Console.WriteLine("Complete list of all kingdoms:\n");
            //ReadJson();
             foreach (var file in files)
            {

                var kingdom = ReadJson(file);
                Console.WriteLine($"Kingdom name:{kingdom.Name}\nKingdom regions:{kingdom.Regions}\nKingdom budget:{kingdom.Budget}\nPopulation happiness:{kingdom.HappinessPopulation}\n");

        }

     //   createEnemyKingdom("Atlantis",new string[] { "Woodland region", "Star region", "Mars region" }, 800000, 80);

        }else{
                Console.WriteLine("Kingdom not found\n");
        }
        }

// method to read and deserialize json file
        static dynamic ReadJson(string filePath){

            string jsonRead = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<dynamic>(jsonRead);

        }

    // method to create your own kingdom
    // wrong correct the part to handle if reign already created

    static void CreateYourOwnKingdom(double budget){
                 Console.Write("Please insert here the name of your Kingdom: ");

                    string? inputName = Console.ReadLine();
                    //         Console.Write("Please insert the form of government:");
                    //        string inputGovernment = Console.ReadLine(); 

                 

                    Console.Write("Please insert here the name of the regions of your kingdom splitted by coma: ");

                    string? inputRegions = Console.ReadLine();
                    string[] region = inputRegions.Split(',');

                    if (region.Length != 3)
                    {
                        throw new FormatException("Input must include only 3 names,each one splitted by coma");
                    };

                    // creation of the object kingdom with properties

                    var kingdom = new
                    {
                        Name = inputName,
                        Regions = region,   // Convert.ToString(region[3]),
                        Budget = budget,
                        HappinessPopulation = 70,
                        TimeStamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")
                    };

                    // serialize and format json file 

                    createEnemyKingdom("Atlantis", new string[] { "Red", "Wald", "oceania" }, 800000, 80);

                    string jsonString = JsonConvert.SerializeObject(kingdom, Formatting.Indented);
                       string filePath = Path.Combine(directoryPath, $"{kingdom.Name}_{kingdom.TimeStamp}.json");
                    

                    File.WriteAllText(filePath, jsonString);

    }


    
}
