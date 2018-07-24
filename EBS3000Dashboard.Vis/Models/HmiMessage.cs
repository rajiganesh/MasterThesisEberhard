using System;
using EBS3000Dashboard.Interface;

namespace EBS3000Dashboard.Vis.Models
{
  public class HmiMessage : IHmiMessage
  {
    public int Number { get; set; }
    public DateTime Timestamp { get; set; }
    public int Type { get; set; }
    public string Text { get; set; }
    public bool Appeared { get; set; }
  }
}
