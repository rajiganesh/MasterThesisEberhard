using System;
using System.Reactive.Linq;
using Autofac;
using Eberhard.Core.Utilities;
using EBS3000Dashboard.Interface;
using EBS3000Dashboard.Vis.Services;

namespace EBS3000Dashboard.Vis.ViewModels.Segments
{
  public class GeneralMachineDataViewModel : NotifingObject
  {
    #region Private Members
    private IProductionServices _productionServices;
    private SettingsWindowViewModel _settingsWindowViewModel;
    private string _machineName;
    private TimeSpan _cycleTimeLastPart;
    private TimeSpan _cycleTimeLastXParts;
    private int _totalCountParts;
    private int _countOkParts;
    private int _countNokParts;
    private IDisposable _updateObserver;
    #endregion

    #region Public Properties
    public string MachineName
    {
      get { return _machineName; }
      set
      {
        _machineName = value;
        FirePropertyChanged();
      }
    }

    public TimeSpan CycleTimeLastPart
    {
      get
      {
        return _cycleTimeLastPart;
      }
      set
      {
        _cycleTimeLastPart = value;
        FirePropertyChanged();
      }
    }

    public TimeSpan CycleTimeLastXParts
    {
      get
      {
        return _cycleTimeLastXParts;
      }
      set
      {
        _cycleTimeLastXParts = value;
        FirePropertyChanged();
      }
    }

    public int TotalCountParts
    {
      get
      {
        return _totalCountParts;
      }
      set
      {
        _totalCountParts = value;
        FirePropertyChanged();
      }
    }

    public int CountOkParts
    {
      get
      {
        return _countOkParts;
      }
      set
      {
        _countOkParts = value;
        FirePropertyChanged();
      }
    }

    public int CountNokParts
    {
      get
      {
        return _countNokParts;
      }
      set
      {
        _countNokParts = value;
        FirePropertyChanged();
      }
    }
    #endregion

    #region Constructors
    public GeneralMachineDataViewModel(IProductionServices production)
    {
      _productionServices = production;
      _settingsWindowViewModel = App.Container.Resolve<SettingsWindowViewModel>();

      MachineName = Settings.Settings.Default.MachineName;

      _settingsWindowViewModel.PropertyChangedObserverBySource("MachineName")
        .Subscribe(vm => MachineName = vm.MachineName);

      _updateObserver = Observable.Interval(TimeSpan.FromSeconds(5))
        .ObserveOnDispatcher()
        .Subscribe(args => UpdateData());

      UpdateData();
    }
    #endregion

    #region Private Methods
    private void UpdateData()
    {
      CycleTimeLastPart = _productionServices.Context.GetCycleTime(1);
      CycleTimeLastXParts = _productionServices.Context.GetCycleTime(10);

      var to = Settings.Settings.Default.TestModeActive ? new DateTime(2018, 05, 14, 23, 59, 59) : DateTime.Now; // oder Today + 23:59:59.999;
      var from = Settings.Settings.Default.TestModeActive ? new DateTime(2018, 05, 14) : DateTime.Today;

      var x = _productionServices.Context.GetAmountOkNokParts(from, to);

      var countUndefined = 0;

      foreach(var y in x)
      {
        switch(y.Key)
        {
          case OkNokEnum.OK:
            CountOkParts = y.Value;
            break;
          case OkNokEnum.NOK:
            CountNokParts = y.Value;
            break;
          case OkNokEnum.UNDEFINED:
            countUndefined = y.Value;
            break;
          default:
            throw new ArgumentOutOfRangeException();
        }
      }

      TotalCountParts = CountOkParts + CountNokParts + countUndefined;
    }
    #endregion
  }
}
