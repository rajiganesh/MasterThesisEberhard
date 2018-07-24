using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Autofac;
using Eberhard.Core.Utilities;
using EberhardFramework.Utilities.CompareObjects;
using EBS3000Dashboard.Vis.Services;
using EBS3000Dashboard.Vis.Views.Segments.DetailViews;

namespace EBS3000Dashboard.Vis.ViewModels.Segments
{
  public class CommonErrorsChartViewModel : BaseChartViewModel
  {
    #region Private Members
    private ObservableCollection<Tuple<string, int, string>> _errorsData;
    private ICollection<Tuple<string, int, string>> _oldErrorsData;
    #endregion

    #region Public Properties
    public ObservableCollection<Tuple<string, int, string>> ErrorsData
    {
      get { return _errorsData; }
      set
      {
        _errorsData = value;
        FirePropertyChanged();
      }
    }
    #endregion

    #region Constructors
    public CommonErrorsChartViewModel(IProductionServices production) : base(production)
    {
      UpdateInterval = Settings.Settings.Default.UpdateIntervalCommonErrorsChart;
      HoursToDisplay = Settings.Settings.Default.HoursToDisplayCommonErrors;
      UpdateData();
    }
    #endregion

    #region Protected Overwritten Methods
    protected override void OnChartDoubleClicked()
    {
      var detailView = App.Container.Resolve<CommonErrorsDetailView>();

      detailView.ShowDialog();
    }

    protected override void UpdateData()
    {
      var to = Settings.Settings.Default.TestModeActive ? new DateTime(2018, 05, 14, 23, 59, 00) : DateTime.Now;
      var from = to - new TimeSpan(HoursToDisplay, 0, 0);

      var newErrorsData = _productionServices.Context.GetCommonErrors(from, to);

      if(!CompareLogic.AreEqual(newErrorsData, _oldErrorsData))
        ErrorsData = new ObservableCollection<Tuple<string, int, string>>(newErrorsData);

      _oldErrorsData = newErrorsData;
    }

    protected override void SaveUpdateIntervalSettings()
    {
      Settings.Settings.Default.UpdateIntervalCommonErrorsChart = UpdateInterval;
      Settings.Settings.Default.Save();
    }

    protected override void SaveHoursToDisplaySettings()
    {
      Settings.Settings.Default.HoursToDisplayCommonErrors = HoursToDisplay;
      Settings.Settings.Default.Save();
    }

    protected override void OnUpdateIntervalToDefault()
    {
      Settings.Settings.Default.UpdateIntervalCommonErrorsChart = UpdateInterval = 10;
      base.OnUpdateIntervalToDefault();
    }
    #endregion
  }
}
