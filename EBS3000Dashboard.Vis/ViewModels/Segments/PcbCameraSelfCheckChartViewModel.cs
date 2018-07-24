using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Autofac;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using EberhardFramework.Utilities.CompareObjects;
using EBS3000Dashboard.Core;
using EBS3000Dashboard.Interface;
using EBS3000Dashboard.Vis.Services;
using EBS3000Dashboard.Vis.Views.Segments.DetailViews;

namespace EBS3000Dashboard.Vis.ViewModels.Segments
{
  public class PcbCameraSelfCheckChartViewModel : BaseChartViewModel
  {
    #region Private Members
    private ObservableCollection<KeyValuePair<DateTime, IPcbCameraSelfCheckValue>> _pcbCameraSelfCheckData;
    private ICollection<KeyValuePair<DateTime, IPcbCameraSelfCheckValue>> _oldPcbCameraSelfCheckData;
    private double _averageX;
    private double _stdDeviationX;
    private double _cmX;
    private double _cmkX;
    private double _averageZ;
    private double _stdDeviationZ;
    private double _cmZ;
    private double _cmkZ;
    #endregion

    #region Public Properties
    public ObservableCollection<KeyValuePair<DateTime, IPcbCameraSelfCheckValue>> PcbCameraSelfCheckData
    {
      get { return _pcbCameraSelfCheckData; }
      set
      {
        _pcbCameraSelfCheckData = value;
        FirePropertyChanged();
      }
    }

    public double AverageX
    {
      get
      {
        return _averageX;
      }
      set
      {
        _averageX = value;
        FirePropertyChanged();
      }
    }

    public double StdDeviationX
    {
      get
      {
        return _stdDeviationX;
      }
      set
      {
        _stdDeviationX = value;
        FirePropertyChanged();
      }
    }

    public double CmX
    {
      get
      {
        return _cmX;
      }
      set
      {
        _cmX = value;
        FirePropertyChanged();
      }
    }

    public double CmkX
    {
      get
      {
        return _cmkX;
      }
      set
      {
        _cmkX = value;
        FirePropertyChanged();
      }
    }

    public double AverageZ
    {
      get
      {
        return _averageZ;
      }
      set
      {
        _averageZ = value;
        FirePropertyChanged();
      }
    }

    public double StdDeviationZ
    {
      get
      {
        return _stdDeviationZ;
      }
      set
      {
        _stdDeviationZ = value;
        FirePropertyChanged();
      }
    }

    public double CmZ
    {
      get
      {
        return _cmZ;
      }
      set
      {
        _cmZ = value;
        FirePropertyChanged();
      }
    }

    public double CmkZ
    {
      get
      {
        return _cmkZ;
      }
      set
      {
        _cmkZ = value;
        FirePropertyChanged();
      }
    }
    #endregion

    #region Constructors
    public PcbCameraSelfCheckChartViewModel(IProductionServices production) : base(production)
    {
      UpdateInterval = Settings.Settings.Default.UpdateIntervalPcbCameraSelfCheckChart;
      HoursToDisplay = Settings.Settings.Default.HoursToDisplayPcbCameraSelfCheckChart;
      UpdateData();
    }
    #endregion

    #region Protected Overwritten Methods
    protected override void UpdateData()
    {
      var to = Settings.Settings.Default.TestModeActive ? new DateTime(2018, 07, 02, 13, 37, 00) : DateTime.Now;
      var from = to - new TimeSpan(HoursToDisplay, 0, 0);

      var newPcbCameraSelfCheckData = _productionServices.Context.GetPcbCameraSelfCheckValues(from, to);

      if(!CompareLogic.AreEqual(newPcbCameraSelfCheckData, _oldPcbCameraSelfCheckData))
      {
        PcbCameraSelfCheckData = new ObservableCollection<KeyValuePair<DateTime, IPcbCameraSelfCheckValue>>(newPcbCameraSelfCheckData);

        if(PcbCameraSelfCheckData.Count > 1)
        {
          var valuesX = new List<double>();
          var valuesZ = new List<double>();

          foreach(var value in PcbCameraSelfCheckData)
          {
            valuesX.Add(value.Value.XValue);
            valuesZ.Add(value.Value.ZValue);
          }

          AverageX = Math.Round(Spc.GetAverage(valuesX), 2);
          StdDeviationX = Math.Round(Spc.GetStandardDeviation(valuesX), 2);
          CmX = Math.Round(Spc.GetCm(valuesX, PcbCameraSelfCheckData[0].Value.XTolerancePos, PcbCameraSelfCheckData[0].Value.XToleranceNeg), 2);
          CmkX = Math.Round(Spc.GetCmk(valuesX, 0.0, PcbCameraSelfCheckData[0].Value.XTolerancePos, PcbCameraSelfCheckData[0].Value.XToleranceNeg), 2);

          AverageZ = Math.Round(Spc.GetAverage(valuesZ), 2);
          StdDeviationZ = Math.Round(Spc.GetStandardDeviation(valuesZ), 2);
          CmZ = Math.Round(Spc.GetCm(valuesZ, PcbCameraSelfCheckData[0].Value.ZTolerancePos, PcbCameraSelfCheckData[0].Value.ZToleranceNeg), 2);
          CmkZ = Math.Round(Spc.GetCmk(valuesZ, 0.0, PcbCameraSelfCheckData[0].Value.ZTolerancePos, PcbCameraSelfCheckData[0].Value.ZToleranceNeg), 2);
        }
      }

      _oldPcbCameraSelfCheckData = newPcbCameraSelfCheckData;
    }

    protected override void SaveUpdateIntervalSettings()
    {
      Settings.Settings.Default.UpdateIntervalPcbCameraSelfCheckChart = UpdateInterval;
      Settings.Settings.Default.Save();
    }

    protected override void SaveHoursToDisplaySettings()
    {
      Settings.Settings.Default.HoursToDisplayPcbCameraSelfCheckChart = HoursToDisplay;
      Settings.Settings.Default.Save();
    }

    protected override void OnChartDoubleClicked()
    {
      var detailView = App.Container.Resolve<PcbCameraSelfCheckDetailView>();

      detailView.ShowDialog();
    }

    protected override void OnUpdateIntervalToDefault()
    {
      Settings.Settings.Default.UpdateIntervalPcbCameraSelfCheckChart = UpdateInterval = 60;
      base.OnUpdateIntervalToDefault();
    }
    #endregion
  }
}
