using System;
using System.Reactive.Linq;
using System.Windows.Input;
using Autofac;
using Eberhard.Core.Utilities;
using EBS3000Dashboard.Vis.Services;
using EBS3000Dashboard.Vis.Views;
using EBS3000Dashboard.Vis.Views.Segments;
using EBS3000Dashboard.Vis.Views.Segments.DetailViews;

namespace EBS3000Dashboard.Vis.ViewModels.Segments
{
  public abstract class BaseChartViewModel : NotifingObject
  {
    #region Private members
    private int _updateInterval;
    private int _hoursToDisplay;
    private IDisposable _updateObserver;
    private bool _chartVisible;
    #endregion

    #region Protected Members
    protected readonly IProductionServices _productionServices;
    #endregion

    #region Public Properties
    public ICommand UpdateIntervalPlus1 => new RelayCommand(args => OnUpdateInterval(1));

    public ICommand UpdateIntervalMinus1 => new RelayCommand(args => OnUpdateInterval(-1));

    public ICommand UpdateIntervalPlus5 => new RelayCommand(args => OnUpdateInterval(5));

    public ICommand UpdateIntervalMinus5 => new RelayCommand(args => OnUpdateInterval(-5));

    public ICommand UpdateIntervalToDefault => new RelayCommand(args => OnUpdateIntervalToDefault());

    public ICommand ChartDoubleClicked => new RelayCommand(args => OnChartDoubleClicked());

    public int UpdateInterval
    {
      get { return _updateInterval; }
      set
      {
        _updateInterval = value > 0 ? value : 1;
        FirePropertyChanged();

        _updateObserver?.Dispose();

        _updateObserver = Observable.Interval(TimeSpan.FromSeconds(UpdateInterval))
          .ObserveOnDispatcher()
          .Subscribe(args => UpdateData());

        SaveUpdateIntervalSettings();
      }
    }

    public int HoursToDisplay
    {
      get { return _hoursToDisplay; }
      set
      {
        _hoursToDisplay = value;
        FirePropertyChanged();

        SaveHoursToDisplaySettings();
      }
    }

    public bool ChartVisible
    {
      get { return _chartVisible; }
      set
      {
        _chartVisible = value;

        _updateObserver?.Dispose();
        _updateObserver = null;

        if(_chartVisible)
        {
          _updateObserver = Observable.Interval(TimeSpan.FromSeconds(UpdateInterval))
            .ObserveOnDispatcher()
            .Subscribe(args => UpdateData());
        }

        FirePropertyChanged();
      }
    }
    #endregion

    #region Constructors
    protected BaseChartViewModel(IProductionServices production)
    {
      _productionServices = production;
    }
    #endregion

    #region Private Methods
    private void OnUpdateInterval(object seconds)
    {
      if(UpdateInterval + (int)seconds > 0)
        UpdateInterval += (int)seconds;
    }
    #endregion

    #region Protected Abstract Methods
    protected abstract void UpdateData();
    protected abstract void SaveUpdateIntervalSettings();
    protected abstract void SaveHoursToDisplaySettings();
    protected abstract void OnChartDoubleClicked();
    #endregion

    #region Protected Virtual Methods
    protected virtual void OnUpdateIntervalToDefault()
    {
      Settings.Settings.Default.Save();
    }
    #endregion
  }
}
