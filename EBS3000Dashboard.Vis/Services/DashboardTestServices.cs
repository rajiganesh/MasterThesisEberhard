using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Subjects;
using EBS3000Dashboard.Interface;
using EBS3000Dashboard.Vis.Models;

namespace EBS3000Dashboard.Vis.Services
{
  public class DashboardTestServices : IDashboard
  {
    public void Dispose()
    { }

    public ICollection<KeyValuePair<string, int>> GetRunTimeForOpMode(DateTime fromTime, DateTime toTime)
    {
      var retval = new Collection<KeyValuePair<string, int>>();
      var ran = new Random();

      retval.Add(new KeyValuePair<string, int>($"Auto", ran.Next(100, 200)));
      retval.Add(new KeyValuePair<string, int>($"Manual", ran.Next(10, 30)));
      retval.Add(new KeyValuePair<string, int>($"Step", ran.Next(5, 20)));
      retval.Add(new KeyValuePair<string, int>($"Special", ran.Next(0, 10)));

      return retval;
    }

    public ICollection<KeyValuePair<OkNokEnum, int>> GetAmountOkNokParts(DateTime fromTime, DateTime toTime)
    {
      var retval = new Collection<KeyValuePair<OkNokEnum, int>>();
      var ran = new Random();

      retval.Add(new KeyValuePair<OkNokEnum, int>(OkNokEnum.OK, ran.Next(100, 1000)));
      retval.Add(new KeyValuePair<OkNokEnum, int>(OkNokEnum.NOK, ran.Next(0, 10)));

      return retval;
    }

    public ICollection<KeyValuePair<DateTime, float>> GetTemperatureValues(DateTime fromTime, DateTime toTime)
    {
      var retval = new Collection<KeyValuePair<DateTime, float>>();

      var dtNow = DateTime.Now;

      for(var i = 0; i < 100; i++)
      {
        retval.Add(new KeyValuePair<DateTime, float>(dtNow + new TimeSpan(0, i, 0) - new TimeSpan(0, 100, 0), 33.6f + i * 0.1f));
      }

      return retval;
    }

    public ICollection<KeyValuePair<int, int>> GetOutputValuesFromLastHours(DateTime fromTime, int countHours)
    {
      var retval = new Collection<KeyValuePair<int, int>>();

      var dtNow = DateTime.Now;
      var ran = new Random();

      for(var i = 0; i < countHours; i++)
      {
        retval.Add(new KeyValuePair<int, int>((dtNow + new TimeSpan(i, 0, 0) - new TimeSpan(12, 0, 0)).Hour, 200 + ran.Next(-20, 20)));
      }

      return retval;
    }

    public ICollection<Tuple<string, int, string>> GetCommonErrors(DateTime fromTime, DateTime toTime)
    {
      var retval = new Collection<Tuple<string, int, string>>();
      var ran = new Random();
      var errorNo = new string[5];
      var errorTexts = new string[5];

      errorNo[0] = "001";
      errorNo[1] = "002";
      errorNo[2] = "003";
      errorNo[3] = "004";
      errorNo[4] = "005";

      errorTexts[0] = "Errormessage 001";
      errorTexts[1] = "Errormessage 002";
      errorTexts[2] = "Errormessage 003";
      errorTexts[3] = "Errormessage 004";
      errorTexts[4] = "Errormessage 005";

      for(var i = 0; i < 5; i++)
      {
        retval.Add(new Tuple<string, int, string>(errorNo[i], ran.Next(0, 10), errorTexts[i]));
      }

      return retval;
    }

    public ICollection<IHmiMessage> GetHmiErrorMessages(DateTime fromTime, DateTime toTime, MessageTypeEnum messageType)
    {
      var retval = new Collection<IHmiMessage>();
      var timestamp = DateTime.Now - new TimeSpan(26, 0, 0);
      var ran = new Random();
      var message = new string[2];
      message[0] = "Fehlermeldung";
      message[1] = "Bedienermeldung";

      for(var i = 0; i < 25; i++)
      {
        var msgType = ran.Next(0, 2);
        retval.Add(new HmiMessage { Number = 1, Text = $"{message[msgType]} {ran.Next(1, 254)}", Timestamp = timestamp + new TimeSpan(i, 0, 0), Type = (short)msgType });
      }

      return retval;
    }

    public ICollection<KeyValuePair<DateTime, IPcbCameraSelfCheckValue>> GetPcbCameraSelfCheckValues(DateTime fromTime, DateTime toTime)
    {
      var retval = new Collection<KeyValuePair<DateTime, IPcbCameraSelfCheckValue>>();
      var ran = new Random();

      var dtNow = DateTime.Now;

      for(var i = 0; i < 30; i++)
      {
        var val = new PcbCameraSelfCheckValue
        {
          XValue = (float)ran.Next(-250, 250) / 1000,
          ZValue = (float)ran.Next(-150, 150) / 1000,
          XTolerancePos = 0.25f,
          ZTolerancePos = 0.25f,
          XToleranceNeg = -0.25f,
          ZToleranceNeg = -0.25f
        };

        retval.Add(new KeyValuePair<DateTime, IPcbCameraSelfCheckValue>(dtNow + new TimeSpan(0, i, 0) - new TimeSpan(0, 100, 0), val));
      }

      return retval;
    }

    public TimeSpan GetCycleTime(uint fromLastXParts)
    {
      var ran = new Random();
      return fromLastXParts==1 ? new TimeSpan(0, 0, 0, 20, ran.Next(400, 600)) : new TimeSpan(0, 0, 0, 20, ran.Next(450, 550));
    }
  }
}
