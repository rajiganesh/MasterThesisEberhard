using System;
using System.Collections.Generic;

namespace EBS3000Dashboard.Interface
{
  public interface IDashboard : IDisposable
  {
    /// <summary>
    /// Gets a collection of Key-Value-Pairs with Operationmodes as Key and total-time of the specified
    /// Operationmode in minutes
    /// </summary>
    /// <param name="fromTime">start time</param>
    /// <param name="toTime">end time</param>
    /// <returns>collection of Key-Value-Pairs -> Key = Operationmode, Value = time in minutes</returns>
    ICollection<KeyValuePair<string, int>> GetRunTimeForOpMode(DateTime fromTime, DateTime toTime);

    /// <summary>
    /// Gets a collection of OK/NOK parts (amount of OK/NOK Parts)
    /// </summary>
    /// <param name="fromTime">start time</param>
    /// <param name="toTime">end time</param>
    /// <returns>collection of Key-Value-Pairs -> Key = OK or NOK, Value = amount of parts</returns>
    ICollection<KeyValuePair<OkNokEnum, int>> GetAmountOkNokParts(DateTime fromTime, DateTime toTime);

    /// <summary>
    /// Gets a collection of Key-Value-Pairs with Timestamp as Key and temperature-value in degree celsius
    /// </summary>
    /// <param name="fromTime">start time</param>
    /// <param name="toTime">end time</param>
    /// <returns>collection of Key-Value-Pairs -> Key = Timestamp, Value = temperature in degree celcius</returns>
    ICollection<KeyValuePair<DateTime, float>> GetTemperatureValues(DateTime fromTime, DateTime toTime);

    /// <summary>
    /// Gets a collection of Key-Value-Pairs with Hour as Key and amount of Parts as Value
    /// </summary>
    /// <param name="fromTime">start time</param>
    /// <param name="countHours">count of hours backward</param>
    /// <returns>collection of Key-Value-Pairs -> Key = hour, Value = amount of parts</returns>
    ICollection<KeyValuePair<int, int>> GetOutputValuesFromLastHours(DateTime fromTime, int countHours);

    /// <summary>
    /// Gets a collection of Key-Value-Pairs with Error-Number as Key and amount of errors as Value
    /// </summary>
    /// <param name="fromTime">start time</param>
    /// <param name="toTime">end time</param>
    /// <returns>collection of Tuples -> Item1 = Errornumber, Item2 = Amount Error appeared, Item3 = Errortext </returns>
    ICollection<Tuple<string, int, string>> GetCommonErrors(DateTime fromTime, DateTime toTime);

    /// <summary>
    /// Gets a collection of HMI-Messages
    /// </summary>
    /// <param name="fromTime">start time</param>
    /// <param name="toTime">end time</param>
    /// <param name="messageType">type of requestet messages. E.q. operatormessage, errormessage or all</param>
    /// <returns></returns>
    ICollection<IHmiMessage> GetHmiErrorMessages(DateTime fromTime, DateTime toTime, MessageTypeEnum messageType = MessageTypeEnum.All);

    /// <summary>
    /// Gets a collection of Key-Value-Pairs with Timestamp as Key and Pcb-Camera-Self-Check-Result as value
    /// </summary>
    /// <param name="fromTime">start time</param>
    /// <param name="toTime">end time</param>
    /// <returns>collection of Key-Value-Pairs -> Key = Timestamp, Value = Pcb-Camera-Self-Check-Result</returns>
    ICollection<KeyValuePair<DateTime, IPcbCameraSelfCheckValue>> GetPcbCameraSelfCheckValues(DateTime fromTime, DateTime toTime);

    /// <summary>
    /// Gets the cycletime of last x parts
    /// </summary>
    /// <param name="fromLastXParts">amount of parts</param>
    /// <returns>average of cycletime</returns>
    TimeSpan GetCycleTime(uint fromLastXParts);
  }
}
