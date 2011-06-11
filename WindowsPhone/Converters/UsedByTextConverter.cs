namespace Opuno.Brenn.WindowsPhone.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Windows.Data;

    using Opuno.Brenn.Models;
    using System.Linq;

    using Opuno.Brenn.ViewModels;

    public class UsedByTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var obj = (System.Collections.ObjectModel.ObservableCollection<PersonViewModel>)value;

            return string.Join(", ", obj.Select(p => p.DisplayName).ToArray());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
