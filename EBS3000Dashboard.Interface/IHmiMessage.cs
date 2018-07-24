using System;

namespace EBS3000Dashboard.Interface
{
  public interface IHmiMessage
  {
    int Number { get; set; }
    DateTime Timestamp { get; set; }
    int Type { get; set; }
    string Text { get; set; }
    bool Appeared { get; set; }
  }
}
