using System;
using System.Data.SQLite;
using System.IO;

namespace FinanceCalculatorProject
{
    public class Connection
    {
        // Hardcoded full path to the SQLite database file
        private static string GetDatabasePath()
        {
            return @"C:\Users\arun7\source\repos\FinanceCalculatorProject\tree\SQLite_Version\bin\Debug\net8.0\FinanceDB.sqlite";
        }

        // Return a fresh SQLite connection (caller opens/closes it)
        public static SQLiteConnection GetConnection()
        {
            string dbPath = GetDatabasePath();
            string directoryPath = Path.GetDirectoryName(dbPath);

            // Create directory if it doesn't exist
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // Create database file if it doesn't exist
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
