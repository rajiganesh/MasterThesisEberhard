using Eberhard.Core.Utilities;

namespace EBS3000Dashboard.Vis.ViewModels
{
  public class SettingsWindowViewModel : NotifingObject
  {
    #region Private Members
    private string _machineName;
    private bool _testModeActive;
    private bool _opModeChartVisible;
    private bool _okNokChartVisible;
    private bool _temperatureChartVisible;
    private bool _outputChartVisible;
    private bool _commonErrorsChartVisible;
    private bool _pcbCameraSelfCheckChartVisible;
    #endregion

    #region Public Properties
    public string MachineName
    {
      get
      {
        return _machineName;
      }
      set
      {
        _machineName = value;
        FirePropertyChanged();
      }
    }

    public bool TestModeActive
    {
      get
      {
        return _testModeActive;
      }
      set
      {
        _testModeActive = value;
        FirePropertyChanged();
      }
    }

    public bool OpModeChartVisible
    {
      get
      {
        return _opModeChartVisible;
      }
      set
      {
        _opModeChartVisible = value;
        FirePropertyChanged();
      }
    }

    public bool OkNokChartVisible
    {
      get
      {
        return _okNokChartVisible;
      }
      set
      {
        _okNokChartVisible = value;
        FirePropertyChanged();
      }
    }

    public bool TemperatureChartVisible
    {
      get
      {
        return _temperatureChartVisible;
      }
      set
      {
        _temperatureChartVisible = value;
        FirePropertyChanged();
      }
    }

    public bool OutputChartVisible
    {
      get
      {
        return _outputChartVisible;
      }
      set
      {
        _outputChartVisible = value;
        FirePropertyChanged();
      }
    }

    public bool CommonErrorsChartVisible
    {
      get
      {
        return _commonErrorsChartVisible;
      }
      set
      {
        _commonErrorsChartVisible = value;
        FirePropertyChanged();
      }
    }

    public bool PcbCameraSelfCheckChartVisible
    {
      get
      {
        return _pcbCameraSelfCheckChartVisible;
      }
      set
      {
        _pcbCameraSelfCheckChartVisible = value;
        FirePropertyChanged();
      }
    }

    public RelayCommand CancelClicked => new RelayCommand(OnCancelClicked);

    public RelayCommand OkClicked => new RelayCommand(OnOkClicked);
    #endregion

    #region Constructors
    public SettingsWindowViewModel()
    {
      InitializeProperties();
    }
    #endregion

    #region Private Methods
    private void InitializeProperties()
    {
      MachineName = Settings.Settings.Default.MachineName;
      TestModeActive = Settings.Settings.Default.TestModeActive;
      OpModeChartVisible = Settings.Settings.Default.OpModeChartVisible;
      OkNokChartVisible = Settings.Settings.Default.OkNokChartVisible;
      TemperatureChartVisible = Settings.Settings.Default.TemperatureChartVisible;
      OutputChartVisible = Settings.Settings.Default.OutputChartVisible;
      CommonErrorsChartVisible = Settings.Settings.Default.CommonErrorsChartVisible;
      PcbCameraSelfCheckChartVisible = Settings.Settings.Default.PcbCameraSelfCheckChartVisible;
    }

    private void OnCancelClicked(object obj)
    {
      InitializeProperties();
    }

    private void OnOkClicked(object obj)
    {
      Settings.Settings.Default.MachineName = MachineName;
      Settings.Settings.Default.TestModeActive = TestModeActive;
      Settings.Settings.Default.OpModeChartVisible = OpModeChartVisible;
      Settings.Settings.Default.OkNokChartVisible = OkNokChartVisible;
      Settings.Settings.Default.TemperatureChartVisible = TemperatureChartVisible;
      Settings.Settings.Default.OutputChartVisible = OutputChartVisible;
      Settings.Settings.Default.CommonErrorsChartVisible = CommonErrorsChartVisible;
      Settings.Settings.Default.PcbCameraSelfCheckChartVisible = PcbCameraSelfCheckChartVisible;
      
      Settings.Settings.Default.Save();
    }
    #endregion
  }
}
