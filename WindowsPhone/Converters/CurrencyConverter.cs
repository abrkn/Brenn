namespace Opuno.Brenn.WindowsPhone.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class CurrencyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
			if (parameter.Equals("Input"))
			{
				return string.Format(culture, "{0:######.##}", (decimal)value);
			}
			else
			{
				return parameter == null ? "Null" : (parameter.ToString() + "-" + parameter.GetType().ToString());
				return string.Format(culture, "{0:$###,###.##}", (decimal)value);
			}
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            decimal result;
            decimal.TryParse(value.ToString(), NumberStyles.Currency, culture, out result);
            return result;
        }
    }
}
