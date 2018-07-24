using System;
using System.Linq;
using System.Windows;
using EBS3000Dashboard.Interface;

namespace EBS3000Dashboard.Core
{
  /// <summary>
  /// Represents a HMI-Message
  /// </summary>
  public class HmiMessage : IHmiMessage
  {
    #region Public Properties
    public int Number { get; set; }
    public DateTime Timestamp { get; set; }
    public int Type { get; set; }
    public string Text { get; set; }
    public bool Appeared { get; set; }   // appeared = true, disappeared = false
    #endregion
  }
}
