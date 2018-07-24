using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Autofac;
using EberhardFramework.Utilities.CompareObjects;
using EBS3000Dashboard.Interface;
using EBS3000Dashboard.Vis.Services;
using EBS3000Dashboard.Vis.Views.Segments.DetailViews;

namespace EBS3000Dashboard.Vis.ViewModels.Segments
{
  public class OkNokPartsChartViewModel : BaseChartViewModel
  {
    #region Private Members
    private ObservableCollection<KeyValuePair<OkNokEnum, int>> _okNokData;
    private ICollection<KeyValuePair<OkNokEnum, int>> _oldOkNokData;
    #endregion

    #region Public Properties
    public ObservableCollection<KeyValuePair<OkNokEnum, int>> OkNokData
    {
      get { return _okNokData; }
      set
      {
        _okNokData = value;
        FirePropertyChanged();
      }
    }
    #endregion

    #region Constructors
    public OkNokPartsChartViewModel(IProductionServices production) :base(production)
    {
      UpdateInterval = Settings.Settings.Default.UpdateIntervalIoNioPartsChart;
      HoursToDisplay = Settings.Settings.Default.HoursToDisplayIoNioParts;
      UpdateData();
    }
    #endregion

    #region Protected Overwritten Methods
    protected override void UpdateData()
    {
      var to = Settings.Settings.Default.TestModeActive ? new DateTime(2018, 05, 14, 23, 59, 00) : DateTime.Now;
      var from = to - new TimeSpan(HoursToDisplay, 0, 0);

      var newOkNokData = _productionServices.Context.GetAmountOkNokParts(from, to);

      if(!CompareLogic.AreEqual(newOkNokData, _oldOkNokData))
        OkNokData = new ObservableCollection<KeyValuePair<OkNokEnum, int>>(newOkNokData);

      _oldOkNokData = newOkNokData;
    }

    protected override void SaveUpdateIntervalSettings()
    {
      Settings.Settings.Default.UpdateIntervalIoNioPartsChart = UpdateInterval;
      Settings.Settings.Default.Save();
    }

    protected override void SaveHoursToDisplaySettings()
    {
      Settings.Settings.Default.HoursToDisplayIoNioParts = HoursToDisplay;
      Settings.Settings.Default.Save();
    }

    protected override void OnChartDoubleClicked()
    {
      var detailView = App.Container.Resolve<OkNokPartsDetailView>();

      detailView.ShowDialog();
    }

    protected override void OnUpdateIntervalToDefault()
    {
      Settings.Settings.Default.UpdateIntervalIoNioPartsChart = UpdateInterval = 5;
      base.OnUpdateIntervalToDefault();
    }
    #endregion
  }
}
