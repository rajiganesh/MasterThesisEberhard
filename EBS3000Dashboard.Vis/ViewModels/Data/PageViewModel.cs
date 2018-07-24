using System;
using Eberhard.Core.Utilities;

namespace EBS3000Dashboard.Vis.ViewModels.Data
{
  public class PageViewModel : NotifingObject
  {
    public PageViewModel()
    {
    }

    private string _title;

    public string Title
    {
      get
      {
        return _title;
      }
      set
      {
        _title = value;
        FirePropertyChanged();
      }
    }

    public Type PageType
    {
      get
      {
        return this.GetType();
      }
    }

    private bool _isActive;

    public bool IsActive
    {
      get { return _isActive; }
      set
      {
        _isActive = value;
        FirePropertyChanged();
      }
    }

    private bool _isWaiting;

    public bool IsWaiting
    {
      get { return _isWaiting; }
      set
      {
        _isWaiting = value;
        FirePropertyChanged();
      }
    }


    private bool _isNavigatorOpened;

    /// <summary>
    /// Gets or sets a value indicating whether the navigator is opened.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the navigator is opened; otherwise, <c>false</c>.
    /// </value>
    public bool IsNavigatorOpened
    {
      get { return _isNavigatorOpened; }
      set
      {
        _isNavigatorOpened = value;
        FirePropertyChanged();
      }
    }



    protected virtual void OnActicated() { }
  }
}
