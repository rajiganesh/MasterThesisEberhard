using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Autofac;
using EberhardFramework.Utilities.CompareObjects;
using EBS3000Dashboard.Vis.Services;
using EBS3000Dashboard.Vis.Views.Segments.DetailViews;

namespace EBS3000Dashboard.Vis.ViewModels.Segments
{
  public class OutputChartViewModel : BaseChartViewModel
  {
    #region Private Members
    private ObservableCollection<KeyValuePair<int, int>> _outputData;
    private ICollection<KeyValuePair<int, int>> _oldOutputData;
    #endregion

    #region Public Properties
    public ObservableCollection<KeyValuePair<int, int>> OutputData
    {
      get { return _outputData; }
      set
      {
        _outputData = value;
        FirePropertyChanged();
      }
    }
    #endregion

    #region Constructors
    public OutputChartViewModel(IProductionServices production) : base(production)
    {
      UpdateInterval = Settings.Settings.Default.UpdateIntervalOutputChart;
      HoursToDisplay = Settings.Settings.Default.HoursToDisplayOutput;
      UpdateData();
    }
    #endregion

    #region Protected Overwritten Methods
    protected override void UpdateData()
    {
      var from = Settings.Settings.Default.TestModeActive ? new DateTime(2018, 05, 14, 23, 59, 00) : DateTime.Now;

      var newOutputData = _productionServices.Context.GetOutputValuesFromLastHours(from, HoursToDisplay);

      if(!CompareLogic.AreEqual(newOutputData, _oldOutputData))
        OutputData = new ObservableCollection<KeyValuePair<int, int>>(newOutputData);

      _oldOutputData = newOutputData;
    }

    protected override void SaveUpdateIntervalSettings()
    {
      Settings.Settings.Default.UpdateIntervalOutputChart = UpdateInterval;
      Settings.Settings.Default.Save();
    }

    protected override void SaveHoursToDisplaySettings()
    {
      Settings.Settings.Default.HoursToDisplayOutput = HoursToDisplay;
      Settings.Settings.Default.Save();
    }

    protected override void OnChartDoubleClicked()
    {
      var detailView = App.Container.Resolve<OutputDetailView>();

      detailView.ShowDialog();
    }

    protected override void OnUpdateIntervalToDefault()
    {
      Settings.Settings.Default.UpdateIntervalOutputChart = UpdateInterval = 10;
      base.OnUpdateIntervalToDefault();
    }
    #endregion
  }
}
