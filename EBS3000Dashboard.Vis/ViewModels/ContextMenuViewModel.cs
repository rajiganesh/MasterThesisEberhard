using System.Windows.Input;
using Eberhard.Core.Utilities;

namespace EBS3000Dashboard.Vis.ViewModels
{
  public class ContextMenuViewModel : NotifingObject
  {
    #region Private Members
    private string _displayname;
    #endregion

    #region Public Properties
    public string Displayname
    {
      get { return _displayname;}
      set
      {
        _displayname = value;
        FirePropertyChanged();
      }
    }

    public ICommand MyContextMenuCommand { get; set; }
    #endregion
  }
}
