using Eberhard.Core.Utilities;

namespace EBS3000Dashboard.Vis.ViewModels.Segments.DetailView
{
  public class CommonErrorsDetailViewModel : NotifingObject
  {
    public RelayCommand CloseClicked => new RelayCommand(OnCloseClicked);

    private void OnCloseClicked(object obj)
    {
      ;
    }
  }
}
