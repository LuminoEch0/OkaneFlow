using Service.Models;
using System.Globalization;

namespace OkaneFlow.Helpers
{
    public static class FormatHelper
    {
        private static readonly Dictionary<string, CultureInfo> CurrencyCultures = new()
        {
            { "EUR", new CultureInfo("de-DE") },  // Euro format
            { "USD", new CultureInfo("en-US") },  // US Dollar format
            { "GBP", new CultureInfo("en-GB") },  // British Pound format
            { "JPY", new CultureInfo("ja-JP") },  // Japanese Yen format
            { "CHF", new CultureInfo("de-CH") },  // Swiss Franc format
            { "CAD", new CultureInfo("en-CA") },  // Canadian Dollar format
            { "AUD", new CultureInfo("en-AU") },  // Australian Dollar format
        };

        private static readonly Dictionary<string, string> CurrencySymbols = new()
        {
            { "EUR", "€" },
            { "USD", "$" },
            { "GBP", "£" },
            { "JPY", "¥" },
            { "CHF", "Fr" },
            { "CAD", "$" },
            { "AUD", "$" },
        };

        public static string FormatCurrency(decimal amount, string currencyCode)
        {
            var symbol = CurrencySymbols.GetValueOrDefault(currencyCode, "€");
            
            // Format with 2 decimal places and thousands separator
            return $"{symbol}{amount:N2}";
        }

        public static string FormatDate(DateTime date, string dateFormat)
        {
            try
            {
                return date.ToString(dateFormat);
            }
            catch
            {
                return date.ToString("dd/MM/yyyy");
            }
        }

        public static string FormatDate(DateTime? date, string dateFormat, string defaultValue = "N/A")
        {
            if (!date.HasValue)
                return defaultValue;
            
            return FormatDate(date.Value, dateFormat);
        }

        public static string FormatDateTime(DateTime date, string dateFormat)
        {
            try
            {
                return date.ToString($"{dateFormat} HH:mm");
            }
            catch
            {
                return date.ToString("dd/MM/yyyy HH:mm");
            }
        }
    }
}
