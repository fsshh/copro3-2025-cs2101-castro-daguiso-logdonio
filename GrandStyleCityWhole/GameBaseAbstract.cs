using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using System.Threading;

namespace GrandStyleCityWhole
{
    // Abstract class to tas tinatawag nya yung interface
    public abstract class GameBaseAbstract : GameInterface
    {
        // New structure to hold the options dynamically loaded from the database: (Id, Name)
        protected Dictionary<string, List<(int Id, string Name)>> OptionsCache = new();

        // Helper method to load options from DB into cache
        protected void LoadOptions(string typeName)
        {
            if (!OptionsCache.ContainsKey(typeName))
            {
                OptionsCache[typeName] = DatabaseHelper.GetOptionsByType(typeName);
            }
        }

        // Helper method to get the name of an option by its ID
        protected string GetOptionNameById(string typeName, int id)
        {
            // Ensure options for the type are loaded
            LoadOptions(typeName);

            if (OptionsCache.ContainsKey(typeName))
            {
                return OptionsCache[typeName].FirstOrDefault(o => o.Id == id).Name ?? "Unknown Option";
            }
            return "Option Type Not Loaded";
        }

        // Setting colors na naka base sa index  
        protected ConsoleColor[] questionColors = { ConsoleColor.Cyan, ConsoleColor.Magenta, ConsoleColor.DarkGreen };
        protected ConsoleColor[] optionColors = { ConsoleColor.Yellow, ConsoleColor.DarkMagenta, ConsoleColor.DarkCyan, ConsoleColor.Red };
        protected ConsoleColor errorColor = ConsoleColor.Red;
        protected ConsoleColor inputColor = ConsoleColor.White;

        // Constructor nato man - now loads all necessary options
        public GameBaseAbstract()
        {
            DatabaseHelper.InitializeDatabase();
            DatabaseHelper.InitializeArrays();
        }

        // --- Core Abstract Methods ---
        public abstract void MainMenu();
        public abstract void NewGame();
        public abstract void LoadGame();

        // --- Game Flow Methods (omitted bodies for brevity, assuming standard implementation) ---
        public void LoadingScreen()
        {
            Console.Clear();
            PrintGameName();
            Console.ForegroundColor = ConsoleColor.Green;

            int totalBlocks = 45;
            char block = '█';
            char empty = ' ';

            for (int i = 0; i <= totalBlocks; i++)
            {
                Console.Write("\r");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("[");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(new string(block, i));
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(new string(empty, totalBlocks - i));
                Console.Write($"] {i * 100 / totalBlocks}%");
                Thread.Sleep(20);
            }

            Console.ResetColor();
            Console.WriteLine("\n\nPress any key to continue...");
            Console.ReadKey();
        }

        public void PrintGameName()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("=========================================");
            Console.ForegroundColor= ConsoleColor.Magenta;
            Console.WriteLine("========== Grand Style City! ============");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("=========================================");
            Console.WriteLine();Console.ResetColor();
        }

        public void CampaignMode()
        {
            LoadingScreen();
            Console.Clear();
            PrintGameName();
            Console.WriteLine("Campaign Mode is not yet implemented.");
            Thread.Sleep(1500);
            AskReturnToMenu();
        }

        public void Credits()
        {
            LoadingScreen();
            Console.Clear();
            PrintGameName();
            Console.WriteLine("Game designed and developed by: Dagz (2024)");
            Console.WriteLine("Special thanks to Professor Recluta for the guidance.");
            Thread.Sleep(1500);
            AskReturnToMenu();
        }

        public void ExitGame()
        {
            Console.Clear();
            PrintGameName();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Thank you for playing Grand Style City! Goodbye.");
            Console.ResetColor();
            Environment.Exit(0);
        }

        // UPDATED PickOption: Now returns Option Id (int) or special codes.
        // Returns: [> 0] = Option ID, [-2] = Main Menu, [-1] = Exit Game
        protected int? PickOption(string title, string optionType)
        {
            LoadOptions(optionType);
            var options = OptionsCache[optionType];

            if (options == null || options.Count == 0)
                throw new ArgumentException($"No options available for selection for type: {optionType}", nameof(optionType));

            int lastOptionIndex = options.Count;

            while (true)
            {
                Console.Clear();
                PrintGameName();

                Console.ForegroundColor = questionColors[Math.Abs(title.GetHashCode()) % questionColors.Length];
                Console.WriteLine(title);
                Console.ResetColor();

                for (int i = 0; i < options.Count; i++)
                {
                    Console.ForegroundColor = optionColors[i % optionColors.Length];
                    Console.WriteLine($"[{i + 1}] {options[i].Name}");
                }

                // Add the two new fixed options at the end
                Console.WriteLine();
                Console.WriteLine($"[{lastOptionIndex + 1}] Return to Main Menu");
                Console.WriteLine($"[{lastOptionIndex + 2}] Exit Game");
                Console.ResetColor();

                Console.ForegroundColor = inputColor;
                Console.Write("Enter choice: ");
                try
                {
                    string? input = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(input))
                    {
                        Console.ForegroundColor = errorColor;
                        Console.WriteLine("Please enter a number.");
                        Console.ReadKey();
                        continue;
                    }

                    int choice = int.Parse(input);

                    if (choice >= 1 && choice <= options.Count)
                    {
                        return options[choice - 1].Id; // Return the database ID
                    }
                    else if (choice == lastOptionIndex + 1)
                    {
                        return -2; // Main Menu
                    }
                    else if (choice == lastOptionIndex + 2)
                    {
                        return -1; // Exit Game
                    }

                    Console.ForegroundColor = errorColor;
                    Console.WriteLine("Pick only the options above.");
                }
                catch (FormatException)
                {
                    Console.ForegroundColor = errorColor;
                    Console.WriteLine("Invalid input. Please enter a number.");
                }
                catch (OverflowException)
                {
                    Console.ForegroundColor = errorColor;
                    Console.WriteLine("Number is too large. Pick only the options above.");
                }

                Console.ResetColor();
                Console.WriteLine("Press any key to retry.");
                Console.ReadKey();
            }
        }

        protected byte PickMainMenuOption(string title, string[] options)
        {
            if (options == null || options.Length == 0)
                throw new ArgumentException("No options available for selection.", nameof(options));

            while (true)
            {
                Console.Clear();
                PrintGameName();

                Console.ForegroundColor = questionColors[Math.Abs(title.GetHashCode()) % questionColors.Length];
                Console.WriteLine(title);
                Console.ResetColor();

                for (int i = 0; i < options.Length; i++)
                {
                    Console.ForegroundColor = optionColors[i % optionColors.Length];
                    Console.WriteLine($"[{i + 1}] {options[i]}");
                }
                Console.ResetColor();

                Console.ForegroundColor = inputColor;
                Console.Write("Enter choice: ");
                try
                {
                    string? input = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(input))
                    {
                        Console.ForegroundColor = errorColor;
                        Console.WriteLine("Please enter a number.");
                        Console.ReadKey();
                        continue;
                    }

                    byte choice = byte.Parse(input);

                    if (choice >= 1 && choice <= options.Length)
                        return (byte)(choice - 1);

                    Console.ForegroundColor = errorColor;
                    Console.WriteLine("Pick only the options above.");
                }
                catch (FormatException)
                {
                    Console.ForegroundColor = errorColor;
                    Console.WriteLine("Invalid input. Please enter a number.");
                }
                catch (OverflowException)
                {
                    Console.ForegroundColor = errorColor;
                    Console.WriteLine($"Number is too large. Pick only the options above.");
                }

                Console.ResetColor();
                Console.WriteLine("Press any key to retry.");
                Console.ReadKey();
            }
        }

        // PickAccessoryMultiple now returns List<int>? and handles the special return values
        protected List<int>? PickAccessoryMultiple(string title, string optionType, string itemName)
        {
            byte count = PickCount(itemName, 10);
            List<int> selections = new();

            for (int i = 0; i < count; i++)
            {
                int? pickResult = PickOption($"{title} ({i + 1} of {count})", optionType);

                // Handle the special return values from PickOption
                if (pickResult == -2) return null; // Signal Main Menu jump
                if (pickResult == -1)
                {
                    ExitGame(); // Exit Game immediately
                    return null; // Should not be reached
                }

                selections.Add(pickResult.Value);
            }

            return selections;
        }

        protected byte PickCount(string itemName, byte maxCount)
        {
            while (true)
            {
                Console.Clear();
                PrintGameName();


                Console.ForegroundColor = questionColors[Math.Abs(itemName.GetHashCode()) % questionColors.Length];
                Console.Write($"How many {itemName}? (0-{maxCount}): ");
                Console.ForegroundColor = inputColor;

                try
                {
                    string? input = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(input))
                    {
                        Console.ForegroundColor = errorColor;
                        Console.WriteLine("Please enter a number.");
                        Console.ReadKey();
                        continue;
                    }

                    byte count = byte.Parse(input);
                    if (count <= maxCount)
                        return count;

                    Console.ForegroundColor = errorColor;
                    Console.WriteLine($"Pick only within 0 to {maxCount}.");
                }
                catch (FormatException)
                {

                    Console.ForegroundColor = errorColor;
                    Console.WriteLine("Invalid input. Please enter a number.");
                }
                catch (OverflowException)
                {

                    Console.ForegroundColor = errorColor;
                    Console.WriteLine($"Number is too large. Pick only within 0 to {maxCount}.");
                }

                Console.ResetColor();
                Console.WriteLine("Press any key to retry.");
                Console.ReadKey();
            }
        }

        protected void AskReturnToMenu()
        {
            byte choice = PickMainMenuOption("What do you want to do next?", new[] { "Main Menu", "Exit Game" });
            if (choice == 0) MainMenu();
            else ExitGame();
        }

        protected void ShowPlayerSummary(PlayerStruct player)
        {
            LoadingScreen();
            Console.Clear();
            PrintGameName();

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("=== CHARACTER SUMMARY ===");

            // --- Single-choice fields display ---
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Name: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(player.PlayerName);

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Gender: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(GetOptionNameById("GenderOptions", player.GenderId));

            string hairName = GetOptionNameById("HairOptions", player.HairId);
            string customizationType = (hairName == "Braided") ? "HairCustomizationBraided" : "HairCustomizationFemale";
            string hairCustomizationName = GetOptionNameById(customizationType, player.HairCustomizationId);

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Hair: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"{hairCustomizationName} - {GetOptionNameById("HairColors", player.HairColorId)}");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Face Shape: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(GetOptionNameById("FaceShapes", player.FaceShapeId));

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Nose Shape: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(GetOptionNameById("NoseShapes", player.NoseShapeId));

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Eye Color: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(GetOptionNameById("EyeColors", player.EyeColorId));

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Skin Tone: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(GetOptionNameById("SkinTones", player.SkinToneId));

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Body Type: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(GetOptionNameById("BodyTypes", player.BodyTypeId));

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Top Attire: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(GetOptionNameById("TopAttireOptions", player.TopAttireId));

            // --- Multi-choice accessory fields ---
            void PrintAccessories(string title, string optionType, List<int> ids)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write($"{title} ({ids.Count}): ");
                Console.ForegroundColor = ConsoleColor.White;

                if (ids.Count == 0)
                {
                    Console.WriteLine("None");
                    return;
                }

                List<string> accessoryNames = new();
                foreach (int id in ids)
                {
                    accessoryNames.Add(GetOptionNameById(optionType, id));
                }

                Console.WriteLine(string.Join(", ", accessoryNames));
            }

            PrintAccessories("Earrings", "AccessoryEarrings", player.EarringsList);
            PrintAccessories("Necklaces", "AccessoryNecklaces", player.NecklacesList);
            PrintAccessories("Bracelets", "AccessoryBracelets", player.BraceletsList);
            PrintAccessories("Rings", "AccessoryRings", player.RingsList);

            // --- Remaining single-choice fields display ---
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Shoes: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"{GetOptionNameById("Shoes", player.ShoesId)} - {GetOptionNameById("ShoeColors", player.ShoeColorId)}");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Pose: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(GetOptionNameById("Poses", player.PoseId));

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Video Mode: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(GetOptionNameById("VideoModes", player.VideoModeId));

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Background: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(GetOptionNameById("Backgrounds", player.BackgroundId));

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Pet: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(GetOptionNameById("Pets", player.PetId));

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Walk Animation: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(GetOptionNameById("WalkAnimations", player.WalkAnimationId));

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Saved At: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(player.SaveDate);

            Console.ResetColor();
            Thread.Sleep(1500);
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        public void ShowAllDatabaseTables()
        {
            LoadingScreen();
            Console.Clear();
            PrintGameName();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("=== ALL DATABASE TABLES (Normalized View) ===\n");
            Console.ResetColor();

            var tables = DatabaseHelper.GetAllTablesAndRows();

            foreach (var (TableName, Rows) in tables)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"--- {TableName} ---");
                Console.ResetColor();

                if (Rows.Count == 0)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("(EMPTY)");
                    Console.ResetColor();
                }
                else
                {
                    foreach (var row in Rows)
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine(row);
                    }
                }

                Console.WriteLine();
            }
            Thread.Sleep(5000);

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
            AskReturnToMenu();
        }
    }
}