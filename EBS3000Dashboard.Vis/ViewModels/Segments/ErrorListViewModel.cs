using System;
using System.Collections.ObjectModel;
using Autofac;
using Eberhard.Core.Utilities;
using EBS3000Dashboard.Interface;
using EBS3000Dashboard.Vis.Services;

namespace EBS3000Dashboard.Vis.ViewModels.Segments
{
  public class ErrorListViewModel : NotifingObject
  {
    #region Private Members
    private bool _isVisible;
    private IProductionServices _productionServices;
    private AnalysisToolbarViewModel _analysisToolBarViewModel;
    private ObservableCollection<IHmiMessage> _messages;
    #endregion

    #region Public Properties
    public ObservableCollection<IHmiMessage> Messages
    {
      get { return _messages; }
      set
      {
        _messages = value;
        FirePropertyChanged();
      }
    }

    public bool IsVisible
    {
      get { return _isVisible; }
      set
      {
        _isVisible = value;
        FirePropertyChanged();
      }
    }
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorListViewModel"/> class.
    /// </summary>
    public ErrorListViewModel(IProductionServices production)
    {
      _productionServices = production;
      _analysisToolBarViewModel = App.Container.Resolve<AnalysisToolbarViewModel>();

      _analysisToolBarViewModel.PropertyChangedObserverBySource("IsAnySettingOpened")
        .Subscribe(vm => IsVisible = !vm.IsAnySettingOpened);

      IsVisible = true;
    }
    #endregion

    #region Public Methods
    public void UpdateView(DateTime fromTime, DateTime toTime, MessageTypeEnum messageType)
    {
      Messages = new ObservableCollection<IHmiMessage>(_productionServices.Context.GetHmiErrorMessages(fromTime, toTime, messageType));
    }
    #endregion
  }
}
