using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using EBS3000Dashboard.Interface;

namespace EBS3000Dashboard.Vis.Services
{
  public class DashboardServices : IDashboard
  {
    #region Private Members
    private IDashboard _backend;
    #endregion

    #region Constructors
    public DashboardServices()
    {
      _backend = GetBackend();
    }
    #endregion Constructors

    #region Destructor
    ~DashboardServices()
    {
      Dispose(false);
    }
    #endregion

    #region Private Methods
    private IDashboard GetBackend()
    {
      var entryAssembly = Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly();
      try
      {
        var dllName = "EBS3000Dashboard.Core.dll";
        var fileName = Path.Combine(Path.GetDirectoryName(entryAssembly.Location), dllName);

        var oh = Activator.CreateComInstanceFrom(fileName, "EBS3000Dashboard.Core.DatabaseOperations");
        var classInstance = oh.Unwrap() as IDashboard;
        return classInstance;

      }
      catch(Exception e)
      {
        return null;
      }
    }

    protected virtual void Dispose(bool disposing)
    {
      if(disposing)
      {
        _backend?.Dispose();
        _backend = null;
      }
    }
    #endregion

    #region Public Methods
    public void Dispose()
    {
      Dispose(true);
    }

    public ICollection<KeyValuePair<string, int>> GetRunTimeForOpMode(DateTime fromTime, DateTime toTime)
    {
      return _backend?.GetRunTimeForOpMode(fromTime, toTime) ?? new Collection<KeyValuePair<string, int>>();
    }

    public ICollection<KeyValuePair<OkNokEnum, int>> GetAmountOkNokParts(DateTime fromTime, DateTime toTime)
    {
      return _backend?.GetAmountOkNokParts(fromTime, toTime) ?? new Collection<KeyValuePair<OkNokEnum, int>>();
    }

    public ICollection<KeyValuePair<DateTime, float>> GetTemperatureValues(DateTime fromTime, DateTime toTime)
    {
      return _backend?.GetTemperatureValues(fromTime, toTime) ?? new Collection<KeyValuePair<DateTime, float>>();
    }

    public ICollection<KeyValuePair<int, int>> GetOutputValuesFromLastHours(DateTime fromTime, int countHours)
    {
      return _backend?.GetOutputValuesFromLastHours(fromTime, countHours) ?? new Collection<KeyValuePair<int, int>>();
    }

    public ICollection<Tuple<string, int, string>> GetCommonErrors(DateTime fromTime, DateTime toTime)
    {
      return _backend?.GetCommonErrors(fromTime, toTime) ?? new Collection<Tuple<string, int, string>>();
    }

    public ICollection<IHmiMessage> GetHmiErrorMessages(DateTime fromTime, DateTime toTime, MessageTypeEnum messageType)
    {
      return _backend?.GetHmiErrorMessages(fromTime, toTime, messageType) ?? new Collection<IHmiMessage>();
    }

    public ICollection<KeyValuePair<DateTime, IPcbCameraSelfCheckValue>> GetPcbCameraSelfCheckValues(DateTime fromTime, DateTime toTime)
    {
      return _backend?.GetPcbCameraSelfCheckValues(fromTime, toTime) ?? new Collection<KeyValuePair<DateTime, IPcbCameraSelfCheckValue>>();
    }

    public TimeSpan GetCycleTime(uint fromLastXParts)
    {
      return _backend?.GetCycleTime(fromLastXParts) ?? new TimeSpan();
    }
    #endregion
  }
}
