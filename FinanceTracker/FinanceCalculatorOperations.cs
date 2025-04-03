using System;
using System.Data.SQLite;
using FinanceCalculatorProject;

namespace FinanceCalculator
{
    public class FinanceCalculatorOperations
    {
        // Method to calculate total income
        public decimal CalculateTotalIncome()
        {
            decimal totalIncome = 0;

            using (SQLiteConnection connection = Connection.GetConnection())
            {
                string query = "SELECT SUM(Amount) FROM Transactions WHERE Type = 'Income'";
                SQLiteCommand command = new SQLiteCommand(query, connection);
                try
                {
                    connection.Open();
                    var result = command.ExecuteScalar();
                    totalIncome = result != DBNull.Value ? Convert.ToDecimal(result) : 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred! " + ex.Message);
                }
                connection.Close();
            }

            return totalIncome;
        }

        // Method to calculate total expenses
        public decimal CalculateTotalExpenses()
        {
            decimal totalExpenses = 0;

            using (SQLiteConnection connection = Connection.GetConnection())
            {
                string query = "SELECT SUM(Amount) FROM Transactions WHERE Type = 'Expense'";
                SQLiteCommand command = new SQLiteCommand(query, connection);
                try
                {
                    connection.Open();
                    var result = command.ExecuteScalar();
                    totalExpenses = result != DBNull.Value ? Convert.ToDecimal(result) : 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred! " + ex.Message);
                }
                connection.Close();
            }

            return totalExpenses;
        }
    }
}
