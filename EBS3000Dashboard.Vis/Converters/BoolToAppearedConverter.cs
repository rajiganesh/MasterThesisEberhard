using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace EBS3000Dashboard.Vis.Converters
{
  public class BooleanToAppearedConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var rd = System.Windows.Application.Current.Resources.MergedDictionaries.Single(x => x.Source.OriginalString.StartsWith("Resources/"));
      var appearedString = (string)rd["Appeared"];
      var disappearedString = (string)rd["Disappeared"];
      return value != null && (bool)value ? appearedString : disappearedString;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
