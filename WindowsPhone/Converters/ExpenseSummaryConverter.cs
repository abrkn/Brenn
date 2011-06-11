namespace Opuno.Brenn.WindowsPhone.Converters
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Data;

    using Opuno.Brenn.ViewModels;

    public class ExpenseSummaryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var expense = (ExpenseViewModel)value;

            Debug.Assert(expense.Sender != null, "expense.sender is null.");
            Debug.Assert(expense.Receivers != null, "expense.receivers is null.");

            return string.Format(
                "{0} paid {1:C0} for {2}",
                expense.Sender.DisplayName,
                expense.Amount,
                string.Join(", ", expense.Receivers.Select(u => u.DisplayName).ToArray()));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
