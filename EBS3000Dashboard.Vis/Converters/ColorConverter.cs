using System;
using System.Globalization;
using System.Windows.Controls.DataVisualization;
using System.Windows.Data;

namespace EBS3000Dashboard.Vis.Converters
{
  public class ColorConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var colorList = parameter as ResourceDictionaryCollection;
      var mode = (string)value;
      int idx;

      switch(mode)
      {
        case "Manual":
          idx = 0;
          break;

        case "Step":
          idx = 1;
          break;

        case "Auto":
          idx = 2;
          break;

        default:
          idx = 3;
          break;
      }

      return colorList[idx]["Background"];
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
