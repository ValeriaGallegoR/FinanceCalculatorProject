namespace FinanceCalculator
{
    // Enum for transaction types
    public enum TransactionType
    {
        Income,
        Expense
    }

    // Abstract class for a financial transaction
    public abstract class Transaction
    {
        // Properties
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }

        // Constructor
        public Transaction(DateTime date, decimal amount, TransactionType type)
        {
            Date = date;
            Amount = amount;
            Type = type;
        }
        public abstract override string ToString();
    }

    // Derived class for income transactions
    public class IncomeTransaction : Transaction
    {
        public string Source { get; set; }

        // Constructor
        public IncomeTransaction(DateTime date, decimal amount, string source)
            : base(date, amount, TransactionType.Income)
        {
            Source = source;
        }

        // Override ToString for WPF display
        public override string ToString()
        {
            return $"Income: {Amount:C} from {Source} on {Date:d}";
        }
    }

    // Derived class for expense transactions
    public class ExpenseTransaction : Transaction
    {
        public string Category { get; set; }

        // Constructor
        public ExpenseTransaction(DateTime date, decimal amount, string category)
            : base(date, amount, TransactionType.Expense)
        {
            Category = category;
        }

        // Override ToString for WPF display
        public override string ToString()
        {
            return $"Expense: {Amount:C} for {Category} on {Date:d}";
        }
    }
}
