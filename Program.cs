using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

class Program
{
    private const string DEPOSIT = "Versement";
    private const string WITHDRAWAL = "Retrait";
    private const string INFORMATION = "Informations";
    private const string FILE_PATH = @"temp\fnuméro.txt";

    private static int depositCounter = 0;
    private static int withdrawalCounter = 0;
    private static int informationCounter = 0;

    private static List<Customer> customers = new List<Customer>();

    static void Main()
    {
        LoadTicketNumbers();
        bool continueRunning = true;

        while (continueRunning)
        {
            Console.Clear();
            Console.WriteLine("Veuillez choisir le type d'opération :");
            Console.WriteLine("1. Versement");
            Console.WriteLine("2. Retrait");
            Console.WriteLine("3. Informations");
            Console.WriteLine("4. Quitter");

            int choice;
            while (true)
            {
                string input = Console.ReadLine();
                if (input.Equals("q", StringComparison.OrdinalIgnoreCase))
                {
                    continueRunning = false;
                    DisplayAllTickets();
                    Console.WriteLine("Merci d'avoir utilisé le service. Au revoir !");
                    return;
                }

                if (int.TryParse(input, out choice))
                {
                    if (choice >= 1 && choice <= 4)
                        break;
                    else
                        Console.WriteLine("Veuillez entrer un nombre entre 1 et 4.");
                }
                else
                {
                    Console.WriteLine("Veuillez entrer un nombre valide (1, 2, 3 ou 4).");
                }
            }

            if (choice == 4)
            {
                continueRunning = false;
                break;
            }

            string accountNumber = GetValidatedAccountNumber();
            string name = GetValidatedString("nom");
            string surname = GetValidatedString("prénom");

            string ticketType = "";
            bool stayInCurrentOption = true;

            while (stayInCurrentOption)
            {
                switch (choice)
                {
                    case 1:
                        ticketType = DEPOSIT;
                        depositCounter++;
                        break;
                    case 2:
                        ticketType = WITHDRAWAL;
                        withdrawalCounter++;
                        break;
                    case 3:
                        ticketType = INFORMATION;
                        informationCounter++;
                        break;
                }

                string ticketNumber = $"{ticketType[0]}-{GetCurrentCounter(ticketType)}";
                string waitingMessage = GetWaitingCustomers(ticketType);
                Console.WriteLine($"Votre numéro est {ticketNumber}, {waitingMessage}");

                customers.Add(new Customer(accountNumber, name, surname, ticketNumber, ticketType));
                SaveTicketNumbers();

                Console.WriteLine("Souhaitez-vous prendre un autre numéro pour cette même opération ? (O/N)");
                string response;

                while (true)
                {
                    response = Console.ReadLine().ToUpper().Trim();
                    if (response == "O" || response == "OUI")
                    {
                        stayInCurrentOption = true;
                        break;
                    }
                    else if (response == "N" || response == "NON")
                    {
                        stayInCurrentOption = false;
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Veuillez entrer une réponse valide. (O/N)");
                    }
                }
            }
        }

        DisplayAllTickets();
        Console.WriteLine("Merci d'avoir utilisé le service. Au revoir !");
    }

    private static string GetValidatedAccountNumber()
    {
        while (true)
        {
            Console.WriteLine("Entrez le numéro de compte du client :");
            string accountNumber = Console.ReadLine().Trim();

            if (!string.IsNullOrEmpty(accountNumber))
            {
                return accountNumber;
            }
            else
            {
                Console.WriteLine("Le numéro de compte est obligatoire. Veuillez réessayer.");
            }
        }
    }

    private static string GetValidatedString(string fieldName)
    {
        while (true)
        {
            Console.WriteLine($"Entrez le {fieldName} du client (lettres uniquement) :");
            string input = Console.ReadLine().Trim();

            if (Regex.IsMatch(input, @"^[a-zA-Z\s]+$"))
            {
                return input;
            }
            else
            {
                Console.WriteLine($"Le {fieldName} doit contenir uniquement des lettres. Veuillez réessayer.");
            }
        }
    }

    private static void LoadTicketNumbers()
    {
        string directoryPath = Path.GetDirectoryName(FILE_PATH);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        if (File.Exists(FILE_PATH))
        {
            string[] lines = File.ReadAllLines(FILE_PATH);
            if (lines.Length == 4)
            {
                depositCounter = int.Parse(lines[1].Split(':')[1].Trim());
                withdrawalCounter = int.Parse(lines[2].Split(':')[1].Trim());
                informationCounter = int.Parse(lines[3].Split(':')[1].Trim());
            }
        }
        else
        {
            depositCounter = 0;
            withdrawalCounter = 0;
            informationCounter = 0;
            SaveTicketNumbers();
        }
    }

    private static void SaveTicketNumbers()
    {
        string[] lines = new string[]
        {
            "Derniers numéros attribués :",
            $"Versement : {depositCounter}",
            $"Retrait : {withdrawalCounter}",
            $"Informations : {informationCounter}"
        };
        File.WriteAllLines(FILE_PATH, lines);
    }

    private static int GetCurrentCounter(string ticketType)
    {
        return ticketType switch
        {
            DEPOSIT => depositCounter,
            WITHDRAWAL => withdrawalCounter,
            INFORMATION => informationCounter,
            _ => 0
        };
    }

    private static string GetWaitingCustomers(string ticketType)
    {
        int waitingCount = ticketType switch
        {
            DEPOSIT => depositCounter - 1,
            WITHDRAWAL => withdrawalCounter - 1,
            INFORMATION => informationCounter - 1,
            _ => 0
        };

        if (waitingCount > 0)
        {
            return $"et il y a {waitingCount} personne{(waitingCount > 1 ? "s" : "")} qui attend{(waitingCount > 1 ? "ent" : "")} avant vous.";
        }
        else
        {
            return "et il n'y a aucune personne qui attend avant vous.";
        }
    }

    private static void DisplayAllTickets()
    {
        if (customers.Count == 0)
        {
            Console.WriteLine("La liste des tickets est vide.");
        }
        else
        {
            Console.WriteLine("Liste de tous les tickets attribués :");
            foreach (var customer in customers)
            {
                Console.WriteLine($"Client: {customer.Name} {customer.Surname}, Compte: {customer.AccountNumber}, Opération: {customer.TicketType}, Numéro de ticket: {customer.TicketNumber}");
            }
        }
    }
}

class Customer
{
    public string AccountNumber { get; }
    public string Name { get; }
    public string Surname { get; }
    public string TicketNumber { get; }
    public string TicketType { get; }

    public Customer(string accountNumber, string name, string surname, string ticketNumber, string ticketType)
    {
        AccountNumber = accountNumber;
        Name = name;
        Surname = surname;
        TicketNumber = ticketNumber;
        TicketType = ticketType;
    }
}
