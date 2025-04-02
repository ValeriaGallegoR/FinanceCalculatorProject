using Microsoft.Data.SqlClient;

namespace FinanceCalculator
{
    public class FinanceCalculatorManager
    {

        // List to store transactions
        private List<Transaction> transactions;

        // Constructor
        public FinanceCalculatorManager()
        {
            transactions = new List<Transaction>();
        }


        // Method to add a transaction Income
        public void AddTransactionIncome(IncomeTransaction transaction)
        {
            using (SqlConnection connection = Connection.createInstance().createConnection())
            {
                string query = "INSERT INTO tb_transactions (date,amount,type,detail) values('" + transaction.Date + "'," + transaction.Amount + ",'" + transaction.Type + "','" + transaction.Source + "')";
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }

            transactions.Add(transaction);
            Console.WriteLine("Transaction added successfully.");
        }

        // Method to add a transaction Expense
        public void AddTransactionExpense(ExpenseTransaction transaction)
        {
            using (SqlConnection connection = Connection.createInstance().createConnection())
            {
                string query = "INSERT INTO tb_transactions (date,amount,type,detail) values('" + transaction.Date + "'," + transaction.Amount + ",'" + transaction.Type + "','" + transaction.Category + "')";
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }

            transactions.Add(transaction);
            Console.WriteLine("Transaction added successfully.");
        }
        


        // Method to display all transactions
        public List<string> DisplayAllTransactions()
        {
            List<string> transactionsList = new List<string>();

            using (SqlConnection connection = Connection.createInstance().createConnection())
            {
                string query = "SELECT CONVERT(VARCHAR(10), date, 23), amount, type, detail FROM tb_transactions";
                SqlCommand command = new SqlCommand(query, connection);
                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        string transaction = $"{reader[0]}\t{reader[1]}\t{reader[2]}\t{reader[3]}";
                        transactionsList.Add(transaction);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred! " + ex.Message);
                }
            }

            return transactionsList;
        }


        // Method to display transactions by type
        public List<string> DisplayTransactionsByType(TransactionType type)
        {
            List<string> transactionsList = new List<string>();

            using (SqlConnection connection = Connection.createInstance().createConnection())
            {
                string query = "SELECT CONVERT(VARCHAR(10), date, 23), amount, type, detail FROM tb_transactions WHERE type = @type";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@type", type.ToString());
                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        string transaction = $"{reader[0]}\t{reader[1]}\t{reader[2]}\t{reader[3]}";
                        transactionsList.Add(transaction);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred! " + ex.Message);
                }
            }

            return transactionsList;
        }
    }
}