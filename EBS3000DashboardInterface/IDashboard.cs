using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EBS3000Dashboard.Interface
{
  public interface IDashboard : IDisposable
  {
    ObservableCollection<KeyValuePair<string, int>> GetRunTimeForOpMode(DateTime fromTime, DateTime toTime);

    ObservableCollection<KeyValuePair<string, int>> GetAmountOkNokParts(DateTime fromTime, DateTime toTime);

    ObservableCollection<KeyValuePair<DateTime, float>> GetTemperatureValues(DateTime fromTime, DateTime toTime);

    ObservableCollection<KeyValuePair<int, int>> GetOutputValuesFromLastHours(DateTime fromTime, int countHours);

    ObservableCollection<KeyValuePair<string, int>> GetCommonErrors(DateTime fromTime, DateTime toTime);

    ObservableCollection<IHmiMessage> GetHmiErrorMessages(DateTime fromTime, DateTime toTime, MessageTypeEnum messageType = MessageTypeEnum.All);
  }
}
