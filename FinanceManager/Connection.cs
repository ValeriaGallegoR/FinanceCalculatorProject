using System;
using System.Data.SQLite;
using System.IO;

namespace FinanceManagerProject
{
    public class Connection
    {
        
        private static string GetDatabasePath()
        {
            string folderPath = @"C:\Users\arun7\source\repos\FinanceCalculatorProject\tree\SQLite_Version\bin\Debug\net8.0";
            string fileName = "FinanceDB.sqlite";

            // Combine folder and filename safely
            string fullPath = Path.Combine(folderPath, fileName);

            // Ensure the folder exists
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            return fullPath;
        }

        // Return a fresh SQLite connection (caller opens/closes it)
        public static SQLiteConnection GetConnection()
        {
            string dbPath = GetDatabasePath();

            // If database file doesn't exist, create it and the tables
            if (!File.Exists(dbPath))
            {
                SQLiteConnection.CreateFile(dbPath);
                CreateTables(dbPath);
            }

            return new SQLiteConnection($"Data Source={dbPath};Version=3;");
        }

        // Create Transactions table if not exists
        private static void CreateTables(string dbPath)
        {
            using (var conn = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
            {
                conn.Open();

                string createTableQuery = @"
                    CREATE TABLE IF NOT EXISTS Transactions (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Type TEXT NOT NULL,
                        Category TEXT NOT NULL,
                        Amount REAL NOT NULL,
                        Description TEXT,
                        Date TEXT NOT NULL
                    );";

                using (var cmd = new SQLiteCommand(createTableQuery, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                conn.Close();
            }
        }
    }
}
