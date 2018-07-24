using EBS3000Dashboard.Interface;

namespace EBS3000Dashboard.Vis.Models
{
  public class PcbCameraSelfCheckValue : IPcbCameraSelfCheckValue
  {
    public float XValue { get; set; }
    public float ZValue { get; set; }
    public float XTolerancePos { get; set; }
    public float ZTolerancePos { get; set; }
    public float XToleranceNeg { get; set; }
    public float ZToleranceNeg { get; set; }
  }
}
