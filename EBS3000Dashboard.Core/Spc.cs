using System;
using System.Collections.Generic;
using System.Linq;

namespace EBS3000Dashboard.Core
{
  /// <summary>
  /// Static Helper-Class to calculate SPC (statistical process control) values
  /// </summary>
  public static class Spc
  {
    public static double GetAverage(List<double> values)
    {
      return values.Sum() / values.Count;
    }

    public static double GetStandardDeviation(List<double> values)
    {
      if(values.Count < 2)
        return 0.0;

      var sumQuadrations = 0.0;
      var average = values.Average();

      foreach(var value in values)
        sumQuadrations += Math.Pow((value - average), 2);

      return Math.Sqrt(sumQuadrations / (values.Count - 1));
    }

    public static double GetCm(List<double> values, double upperLimit, double lowerLimit)
    {
      return ( upperLimit - lowerLimit ) / (6 * GetStandardDeviation(values));
    }

    public static double GetCmk(List<double> values, double nominalValue, double upperLimit, double lowerLimit)
    {
      double zCritical;

      var average = GetAverage(values);
      var x = nominalValue - average;

      if(x > 0)
        zCritical = average - lowerLimit;
      else
        zCritical = upperLimit  - average;

      return zCritical / (3 * GetStandardDeviation(values));
    }
  }
}
