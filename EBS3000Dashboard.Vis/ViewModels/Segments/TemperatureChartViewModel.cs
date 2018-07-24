using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Autofac;
using EberhardFramework.Utilities.CompareObjects;
using EBS3000Dashboard.Vis.Services;
using EBS3000Dashboard.Vis.Views.Segments.DetailViews;

namespace EBS3000Dashboard.Vis.ViewModels.Segments
{
  public class TemperatureChartViewModel : BaseChartViewModel
  {
    #region Private Members
    private ObservableCollection<KeyValuePair<DateTime, float>> _temperatureData;
    private ICollection<KeyValuePair<DateTime, float>> _oldTemperatureData;
    #endregion

    #region Public Properties
    public ObservableCollection<KeyValuePair<DateTime, float>> TemperatureData
    {
      get { return _temperatureData; }
      set
      {
        _temperatureData = value;
        FirePropertyChanged();
      }
    }
    #endregion

    #region Constructors
    public TemperatureChartViewModel(IProductionServices production) : base(production)
    {
      UpdateInterval = Settings.Settings.Default.UpdateIntervalTemperatureChart;
      HoursToDisplay = Settings.Settings.Default.HoursToDisplayTemperature;
      UpdateData();
    }
    #endregion

    #region Protected Overwritten Methods
    protected override void UpdateData()
    {
      var to = Settings.Settings.Default.TestModeActive ? new DateTime(2018, 05, 14, 23, 59, 00) : DateTime.Now;
      var from = to - new TimeSpan(HoursToDisplay, 0, 0);

      var newTemperatureData = _productionServices.Context.GetTemperatureValues(from, to);

      if(!CompareLogic.AreEqual(newTemperatureData, _oldTemperatureData))
        TemperatureData = new ObservableCollection<KeyValuePair<DateTime, float>>(newTemperatureData);

      _oldTemperatureData = newTemperatureData;
    }

    protected override void SaveUpdateIntervalSettings()
    {
      Settings.Settings.Default.UpdateIntervalTemperatureChart = UpdateInterval;
      Settings.Settings.Default.Save();
    }

    protected override void SaveHoursToDisplaySettings()
    {
      Settings.Settings.Default.HoursToDisplayTemperature = HoursToDisplay;
      Settings.Settings.Default.Save();
    }

    protected override void OnChartDoubleClicked()
    {
      var detailView = App.Container.Resolve<TemperatureDetailView>();

      detailView.ShowDialog();
    }

    protected override void OnUpdateIntervalToDefault()
    {
      Settings.Settings.Default.UpdateIntervalTemperatureChart = UpdateInterval = 60;
      base.OnUpdateIntervalToDefault();
    }
    #endregion
  }
}
