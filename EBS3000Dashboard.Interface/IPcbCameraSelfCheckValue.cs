using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBS3000Dashboard.Interface
{
  public interface IPcbCameraSelfCheckValue
  {
    float XValue { get; set; }
    float ZValue { get; set; }
    float XTolerancePos { get; set; }
    float ZTolerancePos { get; set; }
    float XToleranceNeg { get; set; }
    float ZToleranceNeg { get; set; }
  }
}
