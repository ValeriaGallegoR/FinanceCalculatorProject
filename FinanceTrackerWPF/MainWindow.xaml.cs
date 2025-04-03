using System.Windows;
using System.Windows.Controls;
using FinanceCalculator;
using LiveCharts;
using LiveCharts.Wpf;
using System.Windows.Media;
using FinanceCalculatorProject;

namespace FinanceCalculatorWPF
{
    public partial class MainWindow : Window
    {
        private FinanceCalculatorManager cm;
        private FinanceCalculatorOperations co;
        public Func<double, string> Formatter { get; set; }

        public MainWindow()
  
        {
            InitializeComponent();

            // Force SQLite to create DB and table
            var conn = Connection.GetConnection();
            conn.Open(); // Ensures the file is physically created
            conn.Close();

            cm = new FinanceCalculatorManager();
            co = new FinanceCalculatorOperations();

            Formatter = value => value.ToString("C");
            DataContext = this;

            MessageBox.Show("DB connection initialized!");
        }

        private void AddTransaction_Click(object sender, RoutedEventArgs e)
        {
            if (decimal.TryParse(AmountTextBox.Text, out decimal amount) &&
                //!string.IsNullOrWhiteSpace(TypeComboBox_SelectionChanged.Text) &&
                TypeComboBox.SelectedItem is ComboBoxItem selectedType)
            {
                string typeText = selectedType.Content.ToString();

                if (typeText == "Income")
                {
                    var income = new IncomeTransaction(DateTime.Now, amount, IncomeCategoryComboBox.Text);
                    cm.AddTransactionIncome(income);
                    MessageBox.Show("Income added!");
                }
                else if (typeText == "Expense")
                {
                    var expense = new ExpenseTransaction(DateTime.Now, amount, ExpenseCategoryComboBox.Text);
                    cm.AddTransactionExpense(expense);
                    MessageBox.Show("Expense added!");
                }

                // Clear inputs
                AmountTextBox.Clear();
                //SourceCategoryComboBox.Clear();
                TypeComboBox.SelectedIndex = -1;

                UpdateGraph(); // Automatically refresh chart
            }
            else
            {
                MessageBox.Show("Please enter valid data.");
            }
        }

        private void ShowAll_Click(object sender, RoutedEventArgs e)
        {
            TransactionList.Items.Clear();
            var allTransactions = cm.DisplayAllTransactions();
            foreach (var transaction in allTransactions)
            {
                if (!string.IsNullOrWhiteSpace(transaction))
                {
                    TransactionList.Items.Add(transaction);
                }
            }
        }

        private void ShowByType_Click(object sender, RoutedEventArgs e)
        {
            TransactionList.Items.Clear();

            if (TypeComboBox.SelectedItem is ComboBoxItem selectedType)
            {
                string typeText = selectedType.Content.ToString();
                TransactionType type = (typeText == "Income") ? TransactionType.Income : TransactionType.Expense;

                var filteredTransactions = cm.DisplayTransactionsByType(type);
                foreach (var transaction in filteredTransactions)
                {
                    if (!string.IsNullOrWhiteSpace(transaction))
                    {
                        TransactionList.Items.Add(transaction);
                    }
                }

                UpdateGraph(); // Optional: update graph after filtering
            }
            else
            {
                MessageBox.Show("Please select a transaction type to filter.");
            }
        }

        private void UpdateGraph()
        {
            decimal actualIncome = co.CalculateTotalIncome();
            decimal actualExpenses = co.CalculateTotalExpenses();

            decimal plannedIncome = decimal.TryParse(PlannedIncomeTextBox.Text, out var pi) ? pi : 0;
            decimal expectedExpenses = decimal.TryParse(ExpectedExpensesTextBox.Text, out var ee) ? ee : 0;

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
    }
}
