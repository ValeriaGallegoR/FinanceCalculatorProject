using Microsoft.Data.SqlClient;

namespace FinanceCalculator
{
    public class FinanceCalculatorOperations
    {
        // Method to calculate total income
        public decimal CalculateTotalIncome()
        {
            decimal totalIncome = 0;

            using (SqlConnection connection = Connection.createInstance().createConnection())
            {
                string query = "SELECT SUM(amount) FROM tb_transactions WHERE type = 'Income'";
                SqlCommand command = new SqlCommand(query, connection);
                try
                {
                    connection.Open();
                    totalIncome = Convert.ToDecimal(command.ExecuteScalar());
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred! " + ex.Message);
                }
            }
            return totalIncome;
        }


        // Method to calculate total expenses
        public decimal CalculateTotalExpenses()
        {
            decimal totalExpenses = 0;

            using (SqlConnection connection = Connection.createInstance().createConnection())
            {
                string query = "SELECT SUM(amount) FROM tb_transactions WHERE type = 'Expense'";
                SqlCommand command = new SqlCommand(query, connection);
                try
                {
                    connection.Open();
                    totalExpenses = Convert.ToDecimal(command.ExecuteScalar());
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred! " + ex.Message);
                }
            }
            return totalExpenses;
        }
    }
}