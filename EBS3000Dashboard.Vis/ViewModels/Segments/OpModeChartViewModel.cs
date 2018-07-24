using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Autofac;
using EberhardFramework.Utilities.CompareObjects;
using EBS3000Dashboard.Vis.Services;
using EBS3000Dashboard.Vis.ViewModels.Segments.DetailView;
using EBS3000Dashboard.Vis.Views.Segments.DetailViews;

namespace EBS3000Dashboard.Vis.ViewModels.Segments
{
  public class OpModeChartViewModel : BaseChartViewModel
  {
    #region Private Members
    private ObservableCollection<KeyValuePair<string, int>> _opModeData;
    private ICollection<KeyValuePair<string, int>> _oldOpModeData;
    #endregion

    #region Public Properties
    public ObservableCollection<KeyValuePair<string, int>> OpModeData
    {
      get { return _opModeData; }
      set
      {
        _opModeData = value;
        FirePropertyChanged();
      }
    }
    public OpModeDetailViewModel OpModeDetailViewContext =>
      App.Container.Resolve<OpModeDetailViewModel>();
    #endregion

    #region Constructors
    public OpModeChartViewModel(IProductionServices production) : base(production)
    {
      UpdateInterval = Settings.Settings.Default.UpdateIntervalOpModeChart;
      HoursToDisplay = Settings.Settings.Default.HoursToDisplayOpMode;
      UpdateData();
    }
    #endregion

    #region Protected Overwritten Methods
    protected override void UpdateData()
    {
      var to = Settings.Settings.Default.TestModeActive ? new DateTime(2018, 05, 14, 23, 59, 00) : DateTime.Now;
      var from = to - new TimeSpan(HoursToDisplay, 0, 0);

      var newOpModeData = _productionServices.Context.GetRunTimeForOpMode(from, to);

      if(!CompareLogic.AreEqual(newOpModeData, _oldOpModeData))
        OpModeData = new ObservableCollection<KeyValuePair<string, int>>(newOpModeData);

      _oldOpModeData = newOpModeData;
    }

    protected override void SaveUpdateIntervalSettings()
    {
      Settings.Settings.Default.UpdateIntervalOpModeChart = UpdateInterval;
      Settings.Settings.Default.Save();
    }

    protected override void SaveHoursToDisplaySettings()
    {
      Settings.Settings.Default.HoursToDisplayOpMode = HoursToDisplay;
      Settings.Settings.Default.Save();
    }

    protected override void OnChartDoubleClicked()
    {
      var detailView = App.Container.Resolve<OpModeDetailView>();

      detailView.ShowDialog();
    }

    protected override void OnUpdateIntervalToDefault()
    {
      Settings.Settings.Default.UpdateIntervalOpModeChart = UpdateInterval = 10;
      base.OnUpdateIntervalToDefault();
    }
    #endregion
  }
}
