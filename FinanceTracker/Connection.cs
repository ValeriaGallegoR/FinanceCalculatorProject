using System;
using System.Data.SQLite;
using System.IO;

namespace FinanceCalculatorProject
{
    public class Connection
    {
        // Get full path to the SQLite database file
        private static string GetDatabasePath()
        {
            string databaseFileName = "FinanceDB.sqlite";
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string fullPath = Path.Combine(basePath, databaseFileName);
            return fullPath;
        }

        // Return a fresh SQLite connection (caller opens/closes it)
        public static SQLiteConnection GetConnection()
        {
            string dbPath = GetDatabasePath();

            // If database doesn't exist, create it and tables
            if (!File.Exists(dbPath))
            {
                SQLiteConnection.CreateFile(dbPath);
                CreateTables(dbPath);
            }

            return new SQLiteConnection($"Data Source={dbPath};Version=3;");
        }

        // Create table if not exists
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
