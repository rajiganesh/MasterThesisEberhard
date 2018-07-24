using System;
using Autofac;
using Eberhard.Core.Utilities;
using EBS3000Dashboard.Vis.Services;
using EBS3000Dashboard.Vis.ViewModels.Data;
using EBS3000Dashboard.Vis.ViewModels.Segments;
using EBS3000Dashboard.Vis.Views.Pages;

namespace EBS3000Dashboard.Vis.ViewModels.Pages
{
  /// <summary>
  /// ViewModel for the <see cref="LiveStatusView" /></summary>
  /// <seealso cref="PageViewModel" />
  public class LiveStatusViewModel : PageViewModel
  {
    #region Private Members
    private IProductionServices _productionServices;
    private MainViewModel _parentViewModel;
    private SettingsWindowViewModel _settingsWindowViewModel;
    /// <summary>
    /// The view model that contains the relevant settings for analysis view.
    /// </summary>
    private AnalysisToolbarViewModel _toolBarViewModel;
    #endregion

    #region Public Properties
    public OpModeChartViewModel OpModeChartContext =>
      App.Container.Resolve<OpModeChartViewModel>();

    public OkNokPartsChartViewModel OkNokPartsChartContext =>
      App.Container.Resolve<OkNokPartsChartViewModel>();

    public OutputChartViewModel OutputChartContext =>
      App.Container.Resolve<OutputChartViewModel>();

    public TemperatureChartViewModel TemperatureChartContext =>
      App.Container.Resolve<TemperatureChartViewModel>();

    public CommonErrorsChartViewModel CommonErrorsChartContext =>
      App.Container.Resolve<CommonErrorsChartViewModel>();

    public PcbCameraSelfCheckChartViewModel PcbCameraSelfCheckChartContext =>
      App.Container.Resolve<PcbCameraSelfCheckChartViewModel>();

    public GeneralMachineDataViewModel GeneralMachineDataContext =>
      App.Container.Resolve<GeneralMachineDataViewModel>();
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="LiveStatusViewModel"/> class.
    /// </summary>
    /// <param name="prouction">The prouction.</param>
    public LiveStatusViewModel(IProductionServices prouction)
    {
      _productionServices = prouction;
      Title = "LiveStatus";

      _toolBarViewModel = App.Container.Resolve<AnalysisToolbarViewModel>();

      _settingsWindowViewModel = App.Container.Resolve<SettingsWindowViewModel>();

      OpModeChartContext.ChartVisible = Settings.Settings.Default.OpModeChartVisible;
      OkNokPartsChartContext.ChartVisible = Settings.Settings.Default.OkNokChartVisible;
      TemperatureChartContext.ChartVisible = Settings.Settings.Default.TemperatureChartVisible;
      OutputChartContext.ChartVisible = Settings.Settings.Default.OutputChartVisible;
      CommonErrorsChartContext.ChartVisible = Settings.Settings.Default.CommonErrorsChartVisible;
      PcbCameraSelfCheckChartContext.ChartVisible = Settings.Settings.Default.PcbCameraSelfCheckChartVisible;

      _settingsWindowViewModel.PropertyChangedObserverBySource("OpModeChartVisible")
        .Subscribe(vm => OpModeChartContext.ChartVisible = vm.OpModeChartVisible);

      _settingsWindowViewModel.PropertyChangedObserverBySource("OkNokChartVisible")
        .Subscribe(vm => OkNokPartsChartContext.ChartVisible = vm.OkNokChartVisible);

      _settingsWindowViewModel.PropertyChangedObserverBySource("TemperatureChartVisible")
        .Subscribe(vm => TemperatureChartContext.ChartVisible = vm.TemperatureChartVisible);

      _settingsWindowViewModel.PropertyChangedObserverBySource("OutputChartVisible")
        .Subscribe(vm => OutputChartContext.ChartVisible = vm.OutputChartVisible);

      _settingsWindowViewModel.PropertyChangedObserverBySource("CommonErrorsChartVisible")
        .Subscribe(vm => CommonErrorsChartContext.ChartVisible = vm.CommonErrorsChartVisible);

      _settingsWindowViewModel.PropertyChangedObserverBySource("PcbCameraSelfCheckChartVisible")
        .Subscribe(vm => PcbCameraSelfCheckChartContext.ChartVisible = vm.PcbCameraSelfCheckChartVisible);
    }
    #endregion
  }
}
