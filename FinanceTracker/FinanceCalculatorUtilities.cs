using System;
using System.Collections.Generic;
using System.Globalization;

namespace FinanceCalculator
{
    public static class FinanceCalculatorUtilities
    {
        // Format a decimal as currency (used in ToString overrides or summaries)
        public static string FormatCurrency(decimal amount)
        {
            return string.Format(CultureInfo.CurrentCulture, "{0:C}", amount);
        }

        // Try parsing a date safely from string input
        public static bool TryParseDate(string input, out DateTime date)
        {
            return DateTime.TryParseExact(input, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
        }

        // Try parsing a decimal safely from string input
        public static bool TryParseAmount(string input, out decimal amount)
        {
            return decimal.TryParse(input, out amount);
        }
    }
}
