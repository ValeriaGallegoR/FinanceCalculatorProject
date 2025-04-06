using System;
using System.Collections.Generic;
using System.Data.SQLite;
using FinanceManagerProject;

namespace FinanceManager
{
    public class FinanceManagerCalculator
    {
        private List<Transaction> transactions;

        public FinanceManagerCalculator()
        {
            transactions = new List<Transaction>();
        }

        public void AddTransactionIncome(IncomeTransaction transaction)
        {
            using (SQLiteConnection connection = Connection.GetConnection())
            {
                string query = "INSERT INTO Transactions (Date, Amount, Type, Category) VALUES (@date, @amount, @type, @category)";
                SQLiteCommand command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@date", transaction.Date.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("@amount", transaction.Amount);
                command.Parameters.AddWithValue("@type", transaction.Type.ToString());
                command.Parameters.AddWithValue("@category", transaction.Source);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }

            transactions.Add(transaction);
            Console.WriteLine("Income transaction added successfully.");
        }

        public void AddTransactionExpense(ExpenseTransaction transaction)
        {
            using (SQLiteConnection connection = Connection.GetConnection())
            {
                string query = "INSERT INTO Transactions (Date, Amount, Type, Category) VALUES (@date, @amount, @type, @category)";
                SQLiteCommand command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@date", transaction.Date.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("@amount", transaction.Amount);
                command.Parameters.AddWithValue("@type", transaction.Type.ToString());
                command.Parameters.AddWithValue("@category", transaction.Category);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }

            transactions.Add(transaction);
            Console.WriteLine("Expense transaction added successfully.");
        }

        public List<string> DisplayAllTransactions()
        {
            List<string> transactionsList = new List<string>();

            using (SQLiteConnection connection = Connection.GetConnection())
            {
                string query = "SELECT Id, Date, Amount, Type, Category FROM Transactions ORDER BY Date DESC";
                SQLiteCommand command = new SQLiteCommand(query, connection);
                try
                {
                    connection.Open();
                    SQLiteDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        string transaction = $"{reader["Id"]}|{reader["Date"]}\t{reader["Amount"]}\t{reader["Type"]}\t{reader["Category"]}";
                        transactionsList.Add(transaction);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred! " + ex.Message);
                }
                connection.Close();
            }

            return transactionsList;
        }

        public List<string> DisplayTransactionsByType(TransactionType type)
        {
            List<string> transactionsList = new List<string>();

            using (SQLiteConnection connection = Connection.GetConnection())
            {
                string query = "SELECT Id, Date, Amount, Type, Category FROM Transactions WHERE Type = @type ORDER BY Date DESC";
                SQLiteCommand command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@type", type.ToString());

                try
                {
                    connection.Open();
                    SQLiteDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        string transaction = $"{reader["Id"]}|{reader["Date"]}\t{reader["Amount"]}\t{reader["Type"]}\t{reader["Category"]}";
                        transactionsList.Add(transaction);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred! " + ex.Message);
                }
                connection.Close();
            }

            return transactionsList;
        }
    }
}
