using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using EberhardFramework.Utilities.CommonUtilities;
using EberhardFramework.Utilities.CommonUtilities.DatabaseEngine;
using EberhardFramework.Core.EberhardData;
using EBS3000Dashboard.Interface;

namespace EBS3000Dashboard.Core
{
  public class DatabaseOperations : IDashboard
  {
    #region Private Members
    private IDatabaseEngine m_sqlConnection;
    private readonly ProtocolManager m_protocolManager;
    #endregion

    #region Constructors
    public DatabaseOperations()
    {
      StaticData.SetDataRootFolder();
      m_protocolManager = ProtocolManager.NewInstance;
      m_protocolManager.ProtocolFileName = "EBS3000Dashboard.Core.DatabaseOperations.prot";

    }
    #endregion

    #region Destructor
    ~DatabaseOperations()
    {
      Dispose(false);
    }
    #endregion

    #region Private Methods
    private bool Connect(bool forceDisconnect = true)
    {
      if(forceDisconnect == false && IsConnected())
        return true;

      DisconnectFromDatabase();
      m_sqlConnection = OpenTraceDatabase(StaticData.TraceDataBase);

      m_protocolManager.WriteToProtocolAssemblyFile(m_sqlConnection != null
        ? $"Successfully connected to Database! Database-Provider: {StaticData.DatabaseProvider}, Connection-String: {StaticData.TraceDataBase}"
        : $"Failed to connect to Database! Database-Provider: {StaticData.DatabaseProvider}, Connection-String: {StaticData.TraceDataBase}");

      return m_sqlConnection != null;
    }

    private void DisconnectFromDatabase()
    {
      try
      {
        if(m_sqlConnection != null && m_sqlConnection.IsDatabaseOpen)
          m_sqlConnection.Close();
        m_sqlConnection = null;
      }
      catch(Exception e)
      {
        m_protocolManager.WriteToProtocolAssemblyFile($"Failed to disconnect from Database! {e.Message}");
        m_sqlConnection = null;
      }
    }

    private bool IsConnected()
    {
      return m_sqlConnection != null && m_sqlConnection.IsDatabaseOpen;
    }

    private IDatabaseEngine OpenTraceDatabase(string connStrOrFilename)
    {
      var databaseEngine = DatabaseEngineLoader.LoadEngine(StaticData.DatabaseProvider);
      if(databaseEngine == null)
      {
        m_protocolManager.WriteToProtocolFile($"failed to load databaseEngine! Method LoadEngine for Provider '{StaticData.DatabaseProvider}' returned null! connStrOrFilename = '{connStrOrFilename}'");
        return null;
      }

      if(databaseEngine.DoesDatabaseExists(connStrOrFilename))
      {
        if(databaseEngine.OpenDataBase(connStrOrFilename))
          return databaseEngine;

        m_protocolManager.WriteToProtocolFile($"failed to open database! Method OpenDataBase returned false! connStrOrFilename = '{connStrOrFilename}'");
        return null;
      }

      m_protocolManager.WriteToProtocolFile($"failed to open database! Method DoesDatabaseExists returned false! connStrOrFilename = '{connStrOrFilename}'");

      databaseEngine.Close();
      return null;

    }
    #endregion

    #region Protected Methods
    protected virtual void Dispose(bool disposing)
    {
      if(disposing)
      {
        DisconnectFromDatabase();
      }
    }
    #endregion

    #region Public Methods
    public void Dispose()
    {
      Dispose(true);
    }
    #endregion

    #region Public Interface Methods
    public ICollection<KeyValuePair<string, int>> GetRunTimeForOpMode(DateTime fromTime, DateTime toTime)
    {
      var sw = new Stopwatch();
      if(!Connect(false))
      {
        m_protocolManager.WriteToProtocolFile($"Method {StackTraceUtil.GetCurrentMethod()} failed! Failed to connect to database");
        return null;
      }

      sw.Start();

      var retval = new Collection<KeyValuePair<string, int>>();

      var from = m_sqlConnection.ColumnValueAsString(fromTime, DataTypes.DateTime);
      var to = m_sqlConnection.ColumnValueAsString(toTime, DataTypes.DateTime);

      var machineStates = m_sqlConnection.ReadDataFromDB("TraceMachineStates", "Time_Stamp between " + from + " And " + to, "Time_Stamp");

      var measuredTimepans = new Dictionary<string, TimeSpan>();

      for(var i = 0; i < machineStates.Count; i++)
      {
        var timeStamp1 = machineStates[i].GetValue<DateTime>("Time_Stamp");
        var timeStamp2 = i < machineStates.Count - 1 ? machineStates[i + 1].GetValue<DateTime>("Time_Stamp") : toTime;

        var opMode = machineStates[i].GetValue<string>("State");

        if(measuredTimepans.ContainsKey(opMode))
          measuredTimepans[opMode] += timeStamp2 - timeStamp1;
        else
          measuredTimepans.Add(opMode, timeStamp2 - timeStamp1);
      }

      sw.Stop();

      foreach(var timeSpan in measuredTimepans)
        retval.Add(new KeyValuePair<string, int>(timeSpan.Key, (int)timeSpan.Value.TotalMinutes));

      return retval;
    }

    public ICollection<KeyValuePair<OkNokEnum, int>> GetAmountOkNokParts(DateTime fromTime, DateTime toTime)
    {
      var sw = new Stopwatch();

      if(!Connect(false))
      {
        m_protocolManager.WriteToProtocolFile($"Method {StackTraceUtil.GetCurrentMethod()} failed! Failed to connect to database");
        return null;
      }

      sw.Start();
      var retval = new Collection<KeyValuePair<OkNokEnum, int>>();

      var from = m_sqlConnection.ColumnValueAsString(fromTime, DataTypes.DateTime);
      var to = m_sqlConnection.ColumnValueAsString(toTime, DataTypes.DateTime);
      var countWpcOkResults = m_sqlConnection.ReadDataFromDB("Select COUNT(*) From WPCResults Where ActDate between " + from + " And " + to + " And Totalresult = 2");
      retval.Add(new KeyValuePair<OkNokEnum, int>(OkNokEnum.OK, countWpcOkResults[0].GetValue<int>("COUNT(*)")));

      var countWpcNokResults = m_sqlConnection.ReadDataFromDB("Select COUNT(*) From WPCResults Where ActDate between " + from + " And " + to + " And Totalresult = 1");
      retval.Add(new KeyValuePair<OkNokEnum, int>(OkNokEnum.NOK, countWpcNokResults[0].GetValue<int>("COUNT(*)")));

      sw.Stop();

      return retval;
    }

    public ICollection<KeyValuePair<DateTime, float>> GetTemperatureValues(DateTime fromTime, DateTime toTime)
    {
      var sw = new Stopwatch();

      if(!Connect(false))
      {
        m_protocolManager.WriteToProtocolFile($"Method {StackTraceUtil.GetCurrentMethod()} failed! Failed to connect to database");
        return null;
      }

      sw.Start();
      var retval = new Collection<KeyValuePair<DateTime, float>>();

      var motorTemperatures = m_sqlConnection.ReadDataFromDB("TraceMotorTemperature", "Time_Stamp between " + m_sqlConnection.ColumnValueAsString(fromTime, DataTypes.DateTime) + " And " + m_sqlConnection.ColumnValueAsString(toTime, DataTypes.DateTime) + "And Axis is 'Y'", "Time_Stamp");

      foreach(var temperature in motorTemperatures)
        retval.Add(new KeyValuePair<DateTime, float>(temperature.GetValue<DateTime>("Time_Stamp"), temperature.GetValue<float>("Temperature")));

      sw.Stop();

      return retval;
    }

    public ICollection<KeyValuePair<int, int>> GetOutputValuesFromLastHours(DateTime fromTime, int countHours)
    {
      var sw = new Stopwatch();

      if(!Connect(false))
      {
        m_protocolManager.WriteToProtocolFile($"Method {StackTraceUtil.GetCurrentMethod()} failed! Failed to connect to database");
        return null;
      }

      sw.Start();
      var retval = new Collection<KeyValuePair<int, int>>();

      for(var i = countHours; i > 0; i--)
      {
        var from = m_sqlConnection.ColumnValueAsString(fromTime - new TimeSpan(i + 1, 0, 0), DataTypes.DateTime);
        var to = m_sqlConnection.ColumnValueAsString(fromTime - new TimeSpan(i, 0, 0), DataTypes.DateTime);
        var countWpcResults = m_sqlConnection.ReadDataFromDB("Select COUNT(*) From WPCResults Where ActDate between " + from + " And " + to);
        retval.Add(new KeyValuePair<int, int>((fromTime - new TimeSpan(i, 0, 0)).Hour, countWpcResults[0].GetValue<int>("COUNT(*)")));
      }
      sw.Stop();

      return retval;
    }

    public ICollection<Tuple<string, int, string>> GetCommonErrors(DateTime fromTime, DateTime toTime)
    {
      var sw = new Stopwatch();

      if(!Connect(false))
      {
        m_protocolManager.WriteToProtocolFile($"Method {StackTraceUtil.GetCurrentMethod()} failed! Failed to connect to database");
        return null;
      }

      sw.Start();
      var retval = new Collection<Tuple<string, int, string>>();
      var hmiErrorMessages = m_sqlConnection.ReadDataFromDB("TraceMessages", "Time_Stamp between " + m_sqlConnection.ColumnValueAsString(fromTime, DataTypes.DateTime) + " And " + m_sqlConnection.ColumnValueAsString(toTime, DataTypes.DateTime) + "And Appeared is 1 And Type is 1", "Time_Stamp");

      var orderdEnumerable = hmiErrorMessages.GroupBy(id => id.GetValue<int>("Id")).OrderByDescending(id => id.Count());

      for(var i = 0; i < 5; i++)
      {
        var hmiErrorMessage = orderdEnumerable.ElementAtOrDefault(i);

        if(hmiErrorMessage == null)
          break;

        var keyValueErrorNoCount = new Tuple<string, int, string>("No. " + hmiErrorMessage.First().GetValue<int>("Id"), hmiErrorMessage.Count(), hmiErrorMessages.First().GetValue<string>("Text"));
        retval.Add(keyValueErrorNoCount);
      }

      sw.Stop();
      return retval;
    }

    public ICollection<IHmiMessage> GetHmiErrorMessages(DateTime fromTime, DateTime toTime, MessageTypeEnum messageType)
    {
      if(!Connect(false))
      {
        m_protocolManager.WriteToProtocolFile($"Method {StackTraceUtil.GetCurrentMethod()} failed! Failed to connect to database");
        return null;
      }

      var retval = new Collection<IHmiMessage>();
      var messageTypeSelection = messageType == MessageTypeEnum.All ? string.Empty : " And Type Is " + (int)messageType;
      var hmiErrorMessages = m_sqlConnection.ReadDataFromDB("TraceMessages", "Time_Stamp between " + m_sqlConnection.ColumnValueAsString(fromTime, DataTypes.DateTime) + " And " + m_sqlConnection.ColumnValueAsString(toTime, DataTypes.DateTime) + messageTypeSelection, "Time_Stamp");

      foreach(var hmiErrorMessage in hmiErrorMessages)
      {
        retval.Add(new HmiMessage
        {
          Number = hmiErrorMessage.GetValue<int>("Id"),
          Text = hmiErrorMessage.GetValue<string>("Text"),
          Timestamp = hmiErrorMessage.GetValue<DateTime>("Time_Stamp"),
          Type = hmiErrorMessage.GetValue<int>("Type"),
          Appeared = hmiErrorMessage.GetValue<bool>("Appeared")
        });
      }
      return retval;
    }

    public ICollection<KeyValuePair<DateTime, IPcbCameraSelfCheckValue>> GetPcbCameraSelfCheckValues(DateTime fromTime, DateTime toTime)
    {
      var sw = new Stopwatch();

      if(!Connect(false))
      {
        m_protocolManager.WriteToProtocolFile($"Method {StackTraceUtil.GetCurrentMethod()} failed! Failed to connect to database");
        return null;
      }

      sw.Start();
      var retval = new Collection<KeyValuePair<DateTime, IPcbCameraSelfCheckValue>>();

      var pcbCameraSelfCheckResults = m_sqlConnection.ReadDataFromDB("PcbCameraSelfCheckResults", "Time_Stamp between " + m_sqlConnection.ColumnValueAsString(fromTime, DataTypes.DateTime) + " And " + m_sqlConnection.ColumnValueAsString(toTime, DataTypes.DateTime), "Time_Stamp");

      foreach(var pcbCameraSelfCheckResult in pcbCameraSelfCheckResults)
      {
        var val = new PcbCameraSelfCheckValue
        {
          XValue = pcbCameraSelfCheckResult.GetValue<float>("XVal"),
          ZValue = pcbCameraSelfCheckResult.GetValue<float>("ZVal"),
          XTolerancePos = pcbCameraSelfCheckResult.GetValue<float>("XTol"),
          XToleranceNeg = pcbCameraSelfCheckResult.GetValue<float>("XTol") * -1,
          ZTolerancePos = pcbCameraSelfCheckResult.GetValue<float>("ZTol"),
          ZToleranceNeg = pcbCameraSelfCheckResult.GetValue<float>("ZTol") * -1
        };

        retval.Add(new KeyValuePair<DateTime, IPcbCameraSelfCheckValue>(pcbCameraSelfCheckResult.GetValue<DateTime>("Time_Stamp"), val));
      }

      sw.Stop();

      return retval;
    }

    public TimeSpan GetCycleTime(uint fromLastXParts)
    {
      var sw = new Stopwatch();

      if(!Connect(false))
      {
        m_protocolManager.WriteToProtocolFile($"Method {StackTraceUtil.GetCurrentMethod()} failed! Failed to connect to database");
        return new TimeSpan();
      }

      sw.Start();
      var retval = new TimeSpan();

      var wpcResults = m_sqlConnection.ReadDataFromDB($"Select * From WPCResults order by ActDate DESC LIMIT {fromLastXParts + 1}");

      if(wpcResults.Count < 2)
        return retval;

      for(var i = 0; i < wpcResults.Count - 1; i++)
      {
        var actDate = wpcResults[i].GetValue<DateTime>("ActDate");
        var actDateLastPart = wpcResults[i + 1].GetValue<DateTime>("ActDate");

        retval += actDate - actDateLastPart;
      }

      retval = new TimeSpan(0, 0, 0, 0, (int) (retval.TotalMilliseconds / fromLastXParts));

      sw.Stop();

      return retval;
    }
    #endregion
  }
}
