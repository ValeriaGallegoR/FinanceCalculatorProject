using System;
using System.Windows;
using System.Windows.Controls;
using FinanceManager;
using LiveCharts;
using LiveCharts.Wpf;
using System.Windows.Media;
using FinanceManagerProject;
using System.Data.SQLite;
using System.Data;
using System.Linq;
using System.Collections.Generic;

namespace FinanceManagerWPF
{
    public partial class MainWindow : Window
    {
        private FinanceManagerCalculator cm;
        private FinanceManagerOperations co;
        public Func<double, string> Formatter { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            var conn = Connection.GetConnection();
            conn.Open(); conn.Close();

            cm = new FinanceManagerCalculator();
            co = new FinanceManagerOperations();

            Formatter = value => value.ToString("C");
            DataContext = this;

            MessageBox.Show("✅ Database initialized successfully.");
            ShowAll_Click(null, null);
            UpdateGraph();
        }

        private void AddTransaction_Click(object sender, RoutedEventArgs e)
        {
            if (!decimal.TryParse(AmountTextBox.Text.Trim(), out decimal amount) || amount <= 0)
            {
                MessageBox.Show("❌ Please enter a valid positive amount.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!(TypeComboBox.SelectedItem is ComboBoxItem selectedType))
            {
                MessageBox.Show("❌ Please select a transaction type.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string typeText = selectedType.Content.ToString();
            string category = "";

            if (typeText == "Income")
            {
                if (string.IsNullOrWhiteSpace(IncomeCategoryComboBox.Text))
                {
                    MessageBox.Show("❌ Please select a source for the income.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                category = IncomeCategoryComboBox.Text;
                var income = new IncomeTransaction(DateTime.Now, amount, category);
                cm.AddTransactionIncome(income);
                MessageBox.Show("✅ Income added successfully.");
            }
            else if (typeText == "Expense")
            {
                if (string.IsNullOrWhiteSpace(ExpenseCategoryComboBox.Text))
                {
                    MessageBox.Show("❌ Please select a category for the expense.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                category = ExpenseCategoryComboBox.Text;
                var expense = new ExpenseTransaction(DateTime.Now, amount, category);
                cm.AddTransactionExpense(expense);
                MessageBox.Show("✅ Expense added successfully.");
            }

            ClearInputs();
            ShowAll_Click(null, null);
            UpdateGraph();
        }

        private void ShowAll_Click(object sender, RoutedEventArgs e)
        {
            var allTransactions = cm.DisplayAllTransactions();
            TransactionsDataGrid.ItemsSource = ParseTransactionList(allTransactions);
        }

        private void ShowByType_Click(object sender, RoutedEventArgs e)
        {
            if (TypeComboBox.SelectedItem is ComboBoxItem selectedType)
            {
                string typeText = selectedType.Content.ToString();
                TransactionType type = (typeText == "Income") ? TransactionType.Income : TransactionType.Expense;

                var filteredTransactions = cm.DisplayTransactionsByType(type);
                TransactionsDataGrid.ItemsSource = ParseTransactionList(filteredTransactions);

                UpdateGraph();
            }
            else
            {
                MessageBox.Show("Please select a transaction type to filter.");
            }
        }

        private void UpdateTransaction_Click(object sender, RoutedEventArgs e)
        {
            if (TransactionsDataGrid.SelectedItem is TransactionModel selectedTransaction)
            {
                if (!decimal.TryParse(AmountTextBox.Text.Trim(), out decimal amount) || amount <= 0)
                {
                    MessageBox.Show("❌ Please enter a valid positive amount.");
                    return;
                }

                if (!(TypeComboBox.SelectedItem is ComboBoxItem selectedType))
                {
                    MessageBox.Show("❌ Please select a transaction type.");
                    return;
                }

                string typeText = selectedType.Content.ToString();
                string category = (typeText == "Income") ? IncomeCategoryComboBox.Text : ExpenseCategoryComboBox.Text;

                if (string.IsNullOrWhiteSpace(category))
                {
                    MessageBox.Show("❌ Category or source is required.");
                    return;
                }

                using (var conn = Connection.GetConnection())
                {
                    conn.Open();
                    string query = "UPDATE Transactions SET Amount = @amount, Type = @type, Category = @category, Date = @date WHERE Id = @id";

                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@amount", amount);
                        cmd.Parameters.AddWithValue("@type", typeText);
                        cmd.Parameters.AddWithValue("@category", category);
                        cmd.Parameters.AddWithValue("@date", DateTime.Now.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@id", selectedTransaction.Id);
                        cmd.ExecuteNonQuery();
                    }
                    conn.Close();
                }

                MessageBox.Show("✅ Transaction updated successfully.");
                ClearInputs();
                ShowAll_Click(null, null);
                UpdateGraph();
            }
        }

        private void DeleteTransaction_Click(object sender, RoutedEventArgs e)
        {
            if (TransactionsDataGrid.SelectedItem is TransactionModel selectedTransaction)
            {
                var result = MessageBox.Show("Are you sure you want to delete this transaction?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    using (var conn = Connection.GetConnection())
                    {
                        conn.Open();
                        string query = "DELETE FROM Transactions WHERE Id = @id";

                        using (var cmd = new SQLiteCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", selectedTransaction.Id);
                            cmd.ExecuteNonQuery();
                        }
                        conn.Close();
                    }

                    MessageBox.Show("✅ Transaction deleted successfully.");
                    ClearInputs();
                    ShowAll_Click(null, null);
                    UpdateGraph();
                }
            }
        }

        private void TransactionsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TransactionsDataGrid.SelectedItem is TransactionModel row)
            {
                AmountTextBox.Text = row.Amount.ToString();

                TypeComboBox.SelectedItem = TypeComboBox.Items
                    .Cast<ComboBoxItem>()
                    .FirstOrDefault(i => i.Content.ToString() == row.Type);

                if (row.Type == "Income")
                {
                    IncomeCategoryComboBox.Visibility = Visibility.Visible;
                    ExpenseCategoryComboBox.Visibility = Visibility.Collapsed;
                    IncomeCategoryComboBox.SelectedItem = IncomeCategoryComboBox.Items
                        .Cast<ComboBoxItem>()
                        .FirstOrDefault(i => i.Content.ToString() == row.Category);
                }
                else if (row.Type == "Expense")
                {
                    IncomeCategoryComboBox.Visibility = Visibility.Collapsed;
                    ExpenseCategoryComboBox.Visibility = Visibility.Visible;
                    ExpenseCategoryComboBox.SelectedItem = ExpenseCategoryComboBox.Items
                        .Cast<ComboBoxItem>()
                        .FirstOrDefault(i => i.Content.ToString() == row.Category);
                }
            }
        }

        private void UpdateGraph()
        {
            decimal actualIncome = co.CalculateTotalIncome();
            decimal actualExpenses = co.CalculateTotalExpenses();

            if (!decimal.TryParse(PlannedIncomeTextBox.Text.Trim(), out decimal plannedIncome))
                plannedIncome = 0;

            if (!decimal.TryParse(ExpectedExpensesTextBox.Text.Trim(), out decimal expectedExpenses))
                expectedExpenses = 0;

            if (plannedIncome < 0 || expectedExpenses < 0)
            {
                MessageBox.Show("❌ Planned values must be positive.");
                return;
            }

            if (expectedExpenses > 0 && actualExpenses > expectedExpenses)
            {
                MessageBox.Show("⚠️ Warning: Actual expenses have exceeded expected expenses!", "Budget Alert", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            FinanceChart.Series = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Actual",
                    Values = new ChartValues<decimal> { actualIncome, actualExpenses },
                    Fill = new SolidColorBrush(Colors.SteelBlue)
                },
                new ColumnSeries
                {
                    Title = "Planned",
                    Values = new ChartValues<decimal> { plannedIncome, expectedExpenses },
                    Fill = new SolidColorBrush(Colors.Orange)
                }
            };

            FinanceChart.AxisX.Clear();
            FinanceChart.AxisX.Add(new Axis
            {
                Title = "Category",
                Labels = new[] { "Income", "Expenses" }
            });

            FinanceChart.AxisY.Clear();
            FinanceChart.AxisY.Add(new Axis
            {
                Title = "Amount",
                LabelFormatter = Formatter
            });
        }

        private void ClearInputs()
        {
            AmountTextBox.Clear();
            IncomeCategoryComboBox.SelectedIndex = -1;
            ExpenseCategoryComboBox.SelectedIndex = -1;
            TypeComboBox.SelectedIndex = -1;
        }

        private void TypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TypeComboBox.SelectedItem is ComboBoxItem selectedType)
            {
                string typeText = selectedType.Content.ToString();

                if (typeText == "Income")
                {
                    IncomeCategoryComboBox.Visibility = Visibility.Visible;
                    ExpenseCategoryComboBox.Visibility = Visibility.Collapsed;
                }
                else if (typeText == "Expense")
                {
                    IncomeCategoryComboBox.Visibility = Visibility.Collapsed;
                    ExpenseCategoryComboBox.Visibility = Visibility.Visible;
                }
            }
        }

        private void PlannedIncomeTextBox_TextChanged(object sender, TextChangedEventArgs e) => UpdateGraph();

        private void ExpectedExpensesTextBox_TextChanged(object sender, TextChangedEventArgs e) => UpdateGraph();

        // Helper to parse string list into model list
        private List<TransactionModel> ParseTransactionList(List<string> rawList)
        {
            var list = new List<TransactionModel>();
            foreach (var line in rawList)
            {
                var parts = line.Split('|');
                if (parts.Length == 2)
                {
                    var values = parts[1].Trim().Split('\t');
                    if (values.Length == 4 &&
                        int.TryParse(parts[0], out int id) &&
                        DateTime.TryParse(values[0], out DateTime date) &&
                        double.TryParse(values[1], out double amount))
                    {
                        list.Add(new TransactionModel
                        {
                            Id = id,
                            Date = date,
                            Amount = amount,
                            Type = values[2],
                            Category = values[3]
                        });
                    }
                }
            }
            return list;
        }
    }

    public class TransactionModel
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public double Amount { get; set; }
        public string Type { get; set; }
        public string Category { get; set; }
    }
}
