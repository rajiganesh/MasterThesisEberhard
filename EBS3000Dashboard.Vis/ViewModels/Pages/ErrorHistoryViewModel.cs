using System;
using Autofac;
using EBS3000Dashboard.Interface;
using EBS3000Dashboard.Vis.Services;
using EBS3000Dashboard.Vis.ViewModels.Data;
using EBS3000Dashboard.Vis.ViewModels.Segments;

namespace EBS3000Dashboard.Vis.ViewModels.Pages
{
  public class ErrorHistoryViewModel : PageViewModel
  {
    #region Private Members
    private IProductionServices _productionServices;
    private MainViewModel _parentViewModel;
    #endregion

    #region Public Properties
    public AnalysisToolbarViewModel ToolBarContext =>
      App.Container.Resolve<AnalysisToolbarViewModel>();

    public ErrorListViewModel ErrorListContext =>
      App.Container.Resolve<ErrorListViewModel>();
    #endregion

    #region Constructors
    public ErrorHistoryViewModel(IProductionServices production)
    {
      _productionServices = production;
      Title = "ErrorHistory";

      ToolBarContext.NewFilterSettingsApplied += ToolBarContext_NewFilterSettingsApplied;

      ToolBarContext.SelectedMessageType = MessageTypeEnum.ErrorMessage;

      UpdateView();
    }

    private void ToolBarContext_NewFilterSettingsApplied(object sender, EventArgs e)
    {
      UpdateView();
    }
    #endregion

    #region Private Methods
    private void UpdateView()
    {
      ErrorListContext.UpdateView(ToolBarContext.AnalysisStartDateTime, ToolBarContext.AnalysisEndDateTime, ToolBarContext.SelectedMessageType);
    }
    #endregion
  }
}
