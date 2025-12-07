using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.Sqlite;
using System.Linq;

namespace GrandStyleCityWhole
{
    public static class DatabaseHelper
    {
        // Using the latest file name from the user's snippet
        public static string DbFile = "GrandStlyeCitySecondEditionDatabase.DB";

        public static SqliteConnection GetConnection()
        {
            return new SqliteConnection($"Data Source={DbFile}");
        }

        public static void InitializeDatabase()
        {
            if (!File.Exists(DbFile))
            {
                // Create the file if it doesn't exist.
                using (File.Create(DbFile)) { }
            }

            using var conn = GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                PRAGMA foreign_keys = ON;
                
                CREATE TABLE IF NOT EXISTS Options (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Type TEXT NOT NULL,
                    Name TEXT NOT NULL,
                    UNIQUE(Type, Name)
                );

                CREATE TABLE IF NOT EXISTS Players (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    PlayerName TEXT NOT NULL,
                    GenderId INTEGER,
                    HairId INTEGER,
                    HairCustomizationId INTEGER,
                    HairColorId INTEGER,
                    FaceShapeId INTEGER,
                    NoseShapeId INTEGER,
                    EyeColorId INTEGER,
                    SkinToneId INTEGER,
                    BodyTypeId INTEGER,
                    TopAttireId INTEGER,
                    ShoesId INTEGER,
                    ShoeColorId INTEGER,
                    PoseId INTEGER,
                    VideoModeId INTEGER,
                    BackgroundId INTEGER,
                    PetId INTEGER,
                    WalkAnimationId INTEGER,
                    SaveDate TEXT,
                    
                    FOREIGN KEY(GenderId) REFERENCES Options(Id),
                    FOREIGN KEY(HairId) REFERENCES Options(Id),
                    FOREIGN KEY(HairCustomizationId) REFERENCES Options(Id),
                    FOREIGN KEY(HairColorId) REFERENCES Options(Id),
                    FOREIGN KEY(FaceShapeId) REFERENCES Options(Id),
                    FOREIGN KEY(NoseShapeId) REFERENCES Options(Id),
                    FOREIGN KEY(EyeColorId) REFERENCES Options(Id),
                    FOREIGN KEY(SkinToneId) REFERENCES Options(Id),
                    FOREIGN KEY(BodyTypeId) REFERENCES Options(Id),
                    FOREIGN KEY(TopAttireId) REFERENCES Options(Id),
                    FOREIGN KEY(ShoesId) REFERENCES Options(Id),
                    FOREIGN KEY(ShoeColorId) REFERENCES Options(Id),
                    FOREIGN KEY(PoseId) REFERENCES Options(Id),
                    FOREIGN KEY(VideoModeId) REFERENCES Options(Id),
                    FOREIGN KEY(BackgroundId) REFERENCES Options(Id),
                    FOREIGN KEY(PetId) REFERENCES Options(Id),
                    FOREIGN KEY(WalkAnimationId) REFERENCES Options(Id)
                );
                
                -- This table links players to multiple accessories options
                CREATE TABLE IF NOT EXISTS PlayerAccessories (
                    PlayerId INTEGER,
                    AccessoryType TEXT,
                    OptionId INTEGER,   -- Reference to the Options table Id
                    PRIMARY KEY(PlayerId, AccessoryType, OptionId),
                    FOREIGN KEY(PlayerId) REFERENCES Players(Id) ON DELETE CASCADE,
                    FOREIGN KEY(OptionId) REFERENCES Options(Id)
                );
            ";
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        public static void InitializeArrays()
        {
            using var conn = GetConnection();
            conn.Open();

            void InsertArray(string typeName, string[] array)
            {
                using var cmd = conn.CreateCommand();
                // Check if any items of this Type already exist
                cmd.CommandText = $"SELECT COUNT(*) FROM Options WHERE Type = @typeName";
                cmd.Parameters.AddWithValue("@typeName", typeName);
                long count = (long)cmd.ExecuteScalar()!;
                if (count > 0) return;

                foreach (var item in array)
                {
                    cmd.CommandText = "INSERT INTO Options (Type, Name) VALUES (@type, @name)";
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@type", typeName);
                    cmd.Parameters.AddWithValue("@name", item);
                    cmd.ExecuteNonQuery();
                }
            }

            // All original arrays are now inserted into the single 'Options' table with a 'Type' column
            InsertArray("GameModes", new string[] { "New Game", "Load Game", "Campaign Mode", "Credits", "Exit Game", "Show database Tables" });
            InsertArray("GenderOptions", new string[] { "Male", "Female" });
            InsertArray("HairOptions", new string[] { "Curly", "Straight", "Braided", "Wavy" });
            InsertArray("HairColors", new string[] { "Blonde", "Black", "Red", "Orange", "Ash Gray" });
            InsertArray("HairCustomizationBraided", new string[] { "Cornrows", "Twist", "Dreadlocks" });
            InsertArray("HairCustomizationFemale", new string[] { "Ponytail", "Regular" });
            InsertArray("FaceShapes", new string[] { "Oval", "Rectangular", "Heart", "Diamond" });
            InsertArray("NoseShapes", new string[] { "Rounded", "Pointed", "Upturned", "Downturned" });
            InsertArray("EyeColors", new string[] { "Black", "Brown", "Blue" });
            InsertArray("SkinTones", new string[] { "Dark", "Pale", "Fair" });
            InsertArray("BodyTypes", new string[] { "Slim", "Muscular", "Chubby" });
            InsertArray("TopAttireOptions", new string[] { "School uniform", "Gown", "Street wear", "Formal wear", "Swim suit" });
            InsertArray("AccessoryEarrings", new string[] { "Stud", "Hoop", "Dangle" });
            InsertArray("AccessoryNecklaces", new string[] { "Bead", "Tennis", "Pearl" });
            InsertArray("AccessoryBracelets", new string[] { "Chain", "Tennis", "Pearl" });
            InsertArray("AccessoryRings", new string[] { "Band", "Stackable", "Solitaire" });
            InsertArray("Shoes", new string[] { "Sneakers", "Boots", "Sandals" });
            InsertArray("ShoeColors", new string[] { "Red", "Black", "White" });
            InsertArray("Poses", new string[] { "Leaning", "Hand-on-waist", "Cross arm" });
            InsertArray("VideoModes", new string[] { "Slow motion", "Close up", "Hybrid" });
            InsertArray("Backgrounds", new string[] { "Garden", "Beach", "Runway", "City" });
            InsertArray("Pets", new string[] { "Dogs", "Cats", "Hamster", "Bird" });
            InsertArray("WalkAnimations", new string[] { "Classic runway walk", "Pose-and-walk" });

            conn.Close();
        }

        // New method to fetch all options for a given Type. Returns list of (Id, Name)
        public static List<(int Id, string Name)> GetOptionsByType(string typeName)
        {
            var options = new List<(int Id, string Name)>();
            using var conn = GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, Name FROM Options WHERE Type = @type ORDER BY Id";
            cmd.Parameters.AddWithValue("@type", typeName);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                options.Add((reader.GetInt32(0), reader.GetString(1)));
            }

            return options;
        }

        public static List<(string TableName, List<string> Rows)> GetAllTablesAndRows()
        {
            List<(string TableName, List<string> Rows)> result = new();

            using var conn = GetConnection();
            conn.Open();

            // tatawagin mga tables (only show the two main tables for the normalized view)
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name IN ('Options', 'Players', 'PlayerAccessories') ORDER BY name;";
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string tableName = reader.GetString(0);
                    result.Add((tableName, new List<string>()));
                }
            }

            // tatawagin mga rows
            for (int i = 0; i < result.Count; i++)
            {
                var (tableName, rows) = result[i];

                using var cmd = conn.CreateCommand();
                cmd.CommandText = $"SELECT * FROM {tableName};";

                using var reader = cmd.ExecuteReader();
                int fieldCount = reader.FieldCount;

                while (reader.Read())
                {
                    List<string> row = new();
                    for (int f = 0; f < fieldCount; f++)
                        row.Add($"{reader.GetName(f)}={reader.GetValue(f)}");

                    rows.Add(string.Join(", ", row));
                }

                // re-assign modified tuple back into list
                result[i] = (tableName, rows);
            }

            return result;
        }
    }
}