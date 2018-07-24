using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eberhard.Core.Utilities;

namespace EBS3000Dashboard.Vis.ViewModels.Segments.DetailView
{
  public class PcbCameraSelfCheckDetailViewModel : NotifingObject
  {
    public RelayCommand CloseClicked => new RelayCommand(OnCloseClicked);

    private void OnCloseClicked(object obj)
    {
      ;
    }
  }
}
