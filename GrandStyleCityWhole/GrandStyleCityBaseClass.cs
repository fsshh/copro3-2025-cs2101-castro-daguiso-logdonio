using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using System.Threading;

namespace GrandStyleCityWhole
{
    public class GrandStyleCityBaseClass : GameBaseAbstract
    {
        public override void MainMenu()
        {
            LoadingScreen();

            while (true)
            {
                // Main menu options are still array-based for simplicity
                string[] gameModes = { "New Game", "Load Game", "Campaign Mode", "Credits", "Exit Game", "Show database Tables" };
                PrintGameName();
                byte mode = PickMainMenuOption("MAIN MENU", gameModes);

                try
                {
                    switch (mode)
                    {
                        case 0: NewGame(); break;
                        case 1: LoadGame(); break;
                        case 2: CampaignMode(); break;
                        case 3: Credits(); break;
                        case 4: ExitGame(); break;
                        case 5: ShowAllDatabaseTables(); break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                    Console.WriteLine("Returning to Main Menu.\nPress any key to continue");
                    Console.ReadKey();
                }
            }
        }


        public override void NewGame()
        {
            PlayerStruct player = new PlayerStruct();
            player.InitializeLists();
            player.SaveDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            Console.Clear();
            PrintGameName();
            Console.Write("Enter your name (or press Enter to set as 'Player'): ");
            string? name = Console.ReadLine();
            player.PlayerName = string.IsNullOrWhiteSpace(name) ? "Player" : name;

            // Helper to handle the special return values from PickOption and jump
            void HandleSpecialReturn(int? result)
            {
                if (result == -2) MainMenu(); // Go to Main Menu
                if (result == -1) ExitGame(); // Exit Game
            }

            int? selectedId;

            // 1. Gender
            selectedId = PickOption("Select Gender", "GenderOptions");
            HandleSpecialReturn(selectedId);
            if (selectedId == null) return;
            player.GenderId = selectedId.Value;

            // 2. Hair Style
            selectedId = PickOption("Select Hair Style", "HairOptions");
            HandleSpecialReturn(selectedId);
            if (selectedId == null) return;
            player.HairId = selectedId.Value;

            // 3. Hair Customization (dependent on Hair Style and Gender)
            string hairName = GetOptionNameById("HairOptions", player.HairId);
            string genderName = GetOptionNameById("GenderOptions", player.GenderId);

            if (hairName == "Braided")
            {
                selectedId = PickOption("Select Braided Hair", "HairCustomizationBraided");
            }
            else if (genderName == "Female")
            {
                selectedId = PickOption("Select Female Hair", "HairCustomizationFemale");
            }
            else
            {
                // Non-Braided Male, auto-set to 'Regular' ID
                var defaultOption = OptionsCache["HairCustomizationFemale"].FirstOrDefault(o => o.Name == "Regular");
                selectedId = defaultOption.Id;
            }

            HandleSpecialReturn(selectedId);
            if (selectedId == null) return;
            player.HairCustomizationId = selectedId.Value;


            // 4. Hair Color
            selectedId = PickOption("Select Hair Color", "HairColors");
            HandleSpecialReturn(selectedId);
            if (selectedId == null) return;
            player.HairColorId = selectedId.Value;

            // 5. Face Shape
            selectedId = PickOption("Select Face Shape", "FaceShapes");
            HandleSpecialReturn(selectedId);
            if (selectedId == null) return;
            player.FaceShapeId = selectedId.Value;

            // 6. Nose Shape
            selectedId = PickOption("Select Nose Shape", "NoseShapes");
            HandleSpecialReturn(selectedId);
            if (selectedId == null) return;
            player.NoseShapeId = selectedId.Value;

            // 7. Eye Color
            selectedId = PickOption("Select Eye Color", "EyeColors");
            HandleSpecialReturn(selectedId);
            if (selectedId == null) return;
            player.EyeColorId = selectedId.Value;

            // 8. Skin Tone
            selectedId = PickOption("Select Skin Tone", "SkinTones");
            HandleSpecialReturn(selectedId);
            if (selectedId == null) return;
            player.SkinToneId = selectedId.Value;

            // 9. Body Type
            selectedId = PickOption("Select Body Type", "BodyTypes");
            HandleSpecialReturn(selectedId);
            if (selectedId == null) return;
            player.BodyTypeId = selectedId.Value;

            // 10. Top Attire
            selectedId = PickOption("Select Top Attire", "TopAttireOptions");
            HandleSpecialReturn(selectedId);
            if (selectedId == null) return;
            player.TopAttireId = selectedId.Value;


            // --- Accessory Picking (Handles null return from PickAccessoryMultiple) ---
            List<int>? accList;

            accList = PickAccessoryMultiple("Select Earrings", "AccessoryEarrings", "Earrings");
            if (accList == null) return;
            player.EarringsList = accList;

            accList = PickAccessoryMultiple("Select Necklaces", "AccessoryNecklaces", "Necklaces");
            if (accList == null) return;
            player.NecklacesList = accList;

            accList = PickAccessoryMultiple("Select Bracelets", "AccessoryBracelets", "Bracelets");
            if (accList == null) return;
            player.BraceletsList = accList;

            accList = PickAccessoryMultiple("Select Rings", "AccessoryRings", "Rings");
            if (accList == null) return;
            player.RingsList = accList;

            // --- Remaining Single-Choice Options ---

            // 11. Shoes
            selectedId = PickOption("Select Shoes", "Shoes");
            HandleSpecialReturn(selectedId);
            if (selectedId == null) return;
            player.ShoesId = selectedId.Value;

            // 12. Shoe Color
            selectedId = PickOption("Select Shoe Color", "ShoeColors");
            HandleSpecialReturn(selectedId);
            if (selectedId == null) return;
            player.ShoeColorId = selectedId.Value;

            // 13. Pose
            selectedId = PickOption("Select Pose", "Poses");
            HandleSpecialReturn(selectedId);
            if (selectedId == null) return;
            player.PoseId = selectedId.Value;

            // 14. Video Mode
            selectedId = PickOption("Select Video Mode", "VideoModes");
            HandleSpecialReturn(selectedId);
            if (selectedId == null) return;
            player.VideoModeId = selectedId.Value;

            // 15. Background
            selectedId = PickOption("Select Background", "Backgrounds");
            HandleSpecialReturn(selectedId);
            if (selectedId == null) return;
            player.BackgroundId = selectedId.Value;

            // 16. Pet
            selectedId = PickOption("Select Pet", "Pets");
            HandleSpecialReturn(selectedId);
            if (selectedId == null) return;
            player.PetId = selectedId.Value;

            // 17. Walk Animation
            selectedId = PickOption("Select Walk Animation", "WalkAnimations");
            HandleSpecialReturn(selectedId);
            if (selectedId == null) return;
            player.WalkAnimationId = selectedId.Value;


            ShowPlayerSummary(player);

            // --- Save Logic (Only runs if all options were successfully picked) ---
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO Players (
                    PlayerName, GenderId, HairId, HairCustomizationId, HairColorId, FaceShapeId, NoseShapeId,
                    EyeColorId, SkinToneId, BodyTypeId, TopAttireId, ShoesId, ShoeColorId, PoseId,
                    VideoModeId, BackgroundId, PetId, WalkAnimationId, SaveDate
                ) VALUES (
                    @PlayerName, @GenderId, @HairId, @HairCustomizationId, @HairColorId, @FaceShapeId, @NoseShapeId,
                    @EyeColorId, @SkinToneId, @BodyTypeId, @TopAttireId, @ShoesId, @ShoeColorId, @PoseId,
                    @VideoModeId, @BackgroundId, @PetId, @WalkAnimationId, @SaveDate
                );
                SELECT last_insert_rowid();";

            cmd.Parameters.AddWithValue("@PlayerName", player.PlayerName);
            cmd.Parameters.AddWithValue("@GenderId", player.GenderId);
            cmd.Parameters.AddWithValue("@HairId", player.HairId);
            cmd.Parameters.AddWithValue("@HairCustomizationId", player.HairCustomizationId);
            cmd.Parameters.AddWithValue("@HairColorId", player.HairColorId);
            cmd.Parameters.AddWithValue("@FaceShapeId", player.FaceShapeId);
            cmd.Parameters.AddWithValue("@NoseShapeId", player.NoseShapeId);
            cmd.Parameters.AddWithValue("@EyeColorId", player.EyeColorId);
            cmd.Parameters.AddWithValue("@SkinToneId", player.SkinToneId);
            cmd.Parameters.AddWithValue("@BodyTypeId", player.BodyTypeId);
            cmd.Parameters.AddWithValue("@TopAttireId", player.TopAttireId);
            cmd.Parameters.AddWithValue("@ShoesId", player.ShoesId);
            cmd.Parameters.AddWithValue("@ShoeColorId", player.ShoeColorId);
            cmd.Parameters.AddWithValue("@PoseId", player.PoseId);
            cmd.Parameters.AddWithValue("@VideoModeId", player.VideoModeId);
            cmd.Parameters.AddWithValue("@BackgroundId", player.BackgroundId);
            cmd.Parameters.AddWithValue("@PetId", player.PetId);
            cmd.Parameters.AddWithValue("@WalkAnimationId", player.WalkAnimationId);
            cmd.Parameters.AddWithValue("@SaveDate", player.SaveDate);

            long playerId = (long)cmd.ExecuteScalar();

            void InsertAccessory(string type, List<int> list)
            {
                foreach (var optionId in list)
                {
                    using var accCmd = conn.CreateCommand();
                    accCmd.CommandText = "INSERT INTO PlayerAccessories (PlayerId, AccessoryType, OptionId) VALUES (@PlayerId, @Type, @OptionId)";
                    accCmd.Parameters.AddWithValue("@PlayerId", playerId);
                    accCmd.Parameters.AddWithValue("@Type", type);
                    accCmd.Parameters.AddWithValue("@OptionId", optionId);
                    accCmd.ExecuteNonQuery();
                }
            }

            InsertAccessory("AccessoryEarrings", player.EarringsList);
            InsertAccessory("AccessoryNecklaces", player.NecklacesList);
            InsertAccessory("AccessoryBracelets", player.BraceletsList);
            InsertAccessory("AccessoryRings", player.RingsList);

            conn.Close();
            Console.WriteLine("\nGame saved successfully!");
            Thread.Sleep(1000);
            AskReturnToMenu();
        }

        public override void LoadGame()
        {
            Console.Clear();
            PrintGameName();

            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, PlayerName, SaveDate FROM Players ORDER BY Id";
            using var reader = cmd.ExecuteReader();

            List<(int Id, string Name, string SaveDate)> saves = new List<(int, string, string)>();
            while (reader.Read())
            {
                saves.Add((reader.GetInt32(0), reader.GetString(1), reader.GetString(2)));
            }
            conn.Close();

            // Prepare the menu options
            List<string> menuOptions = saves.Select(s => s.Name + $" (Saved at: {s.SaveDate})").ToList();
            menuOptions.Add("Return to Main Menu");
            menuOptions.Add("Exit Game");


            if (saves.Count == 0)
            {
                Console.WriteLine("No saved games found. Returning to Main Menu.");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                MainMenu();
                return;
            }

            // Display saves and the new menu options
            Console.WriteLine("=== Saved Games ===");
            for (int i = 0; i < saves.Count; i++)
            {
                Console.WriteLine($"[{i + 1}] {menuOptions[i]}");
            }
            Console.WriteLine($"[{saves.Count + 1}] {menuOptions[saves.Count]}");
            Console.WriteLine($"[{saves.Count + 2}] {menuOptions[saves.Count + 1]}");

            // Get the choice index
            byte choiceIndex = PickMainMenuOption("Select an option", menuOptions.ToArray());

            if (choiceIndex == saves.Count) // Main Menu (last option index is saves.Count)
            {
                MainMenu();
                return;
            }
            else if (choiceIndex == saves.Count + 1) // Exit Game (last+1 option index is saves.Count + 1)
            {
                ExitGame();
                return;
            }

            // A save was selected
            var selectedSave = saves[choiceIndex];

            // Re-open connection for load/delete action
            conn.Open();

            byte action = PickMainMenuOption($"What do you want to do with '{selectedSave.Name}'?", new[] { "Load", "Delete" });

            if (action == 0) // Load
            {
                PlayerStruct player = new PlayerStruct();
                player.InitializeLists();

                using var playerCmd = conn.CreateCommand();
                playerCmd.CommandText = "SELECT * FROM Players WHERE Id = @id";
                playerCmd.Parameters.AddWithValue("@id", selectedSave.Id);

                using var playerReader = playerCmd.ExecuteReader();
                if (playerReader.Read())
                {
                    player.PlayerName = playerReader.GetString(playerReader.GetOrdinal("PlayerName"));
                    player.GenderId = playerReader.GetInt32(playerReader.GetOrdinal("GenderId"));
                    player.HairId = playerReader.GetInt32(playerReader.GetOrdinal("HairId"));
                    player.HairCustomizationId = playerReader.GetInt32(playerReader.GetOrdinal("HairCustomizationId"));
                    player.HairColorId = playerReader.GetInt32(playerReader.GetOrdinal("HairColorId"));
                    player.FaceShapeId = playerReader.GetInt32(playerReader.GetOrdinal("FaceShapeId"));
                    player.NoseShapeId = playerReader.GetInt32(playerReader.GetOrdinal("NoseShapeId"));
                    player.EyeColorId = playerReader.GetInt32(playerReader.GetOrdinal("EyeColorId"));
                    player.SkinToneId = playerReader.GetInt32(playerReader.GetOrdinal("SkinToneId"));
                    player.BodyTypeId = playerReader.GetInt32(playerReader.GetOrdinal("BodyTypeId"));
                    player.TopAttireId = playerReader.GetInt32(playerReader.GetOrdinal("TopAttireId"));
                    player.ShoesId = playerReader.GetInt32(playerReader.GetOrdinal("ShoesId"));
                    player.ShoeColorId = playerReader.GetInt32(playerReader.GetOrdinal("ShoeColorId"));
                    player.PoseId = playerReader.GetInt32(playerReader.GetOrdinal("PoseId"));
                    player.VideoModeId = playerReader.GetInt32(playerReader.GetOrdinal("VideoModeId"));
                    player.BackgroundId = playerReader.GetInt32(playerReader.GetOrdinal("BackgroundId"));
                    player.PetId = playerReader.GetInt32(playerReader.GetOrdinal("PetId"));
                    player.WalkAnimationId = playerReader.GetInt32(playerReader.GetOrdinal("WalkAnimationId"));
                    player.SaveDate = playerReader.GetString(playerReader.GetOrdinal("SaveDate"));
                }

                using var accCmd = conn.CreateCommand();
                accCmd.CommandText = "SELECT AccessoryType, OptionId FROM PlayerAccessories WHERE PlayerId = @id";
                accCmd.Parameters.AddWithValue("@id", selectedSave.Id);
                using var accReader = accCmd.ExecuteReader();
                while (accReader.Read())
                {
                    string type = accReader.GetString(0);
                    int optionId = accReader.GetInt32(1);

                    switch (type)
                    {
                        case "AccessoryEarrings": player.EarringsList.Add(optionId); break;
                        case "AccessoryNecklaces": player.NecklacesList.Add(optionId); break;
                        case "AccessoryBracelets": player.BraceletsList.Add(optionId); break;
                        case "AccessoryRings": player.RingsList.Add(optionId); break;
                    }
                }

                ShowPlayerSummary(player);
                AskReturnToMenu();
            }
            else // Delete
            {
                // Delete accessories first due to foreign key constraint
                using (var deleteAccCmd = conn.CreateCommand())
                {
                    deleteAccCmd.CommandText = "DELETE FROM PlayerAccessories WHERE PlayerId = @id";
                    deleteAccCmd.Parameters.AddWithValue("@id", selectedSave.Id);
                    deleteAccCmd.ExecuteNonQuery();
                }

                // Delete player record
                using (var deletePlayerCmd = conn.CreateCommand())
                {
                    deletePlayerCmd.CommandText = "DELETE FROM Players WHERE Id = @id";
                    deletePlayerCmd.Parameters.AddWithValue("@id", selectedSave.Id);
                    deletePlayerCmd.ExecuteNonQuery();
                }

                Console.WriteLine($"Save '{selectedSave.Name}' has been deleted.");
                Thread.Sleep(1000);

                LoadGame(); // Reload LoadGame to refresh the list after deletion
            }
            conn.Close();
        }

    }
}