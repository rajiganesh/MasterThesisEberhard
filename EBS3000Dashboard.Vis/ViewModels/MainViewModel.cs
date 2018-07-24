using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Autofac;
using Eberhard.Core.Utilities;
using EBS3000Dashboard.Vis.Services;
using EBS3000Dashboard.Vis.ViewModels.Data;
using EBS3000Dashboard.Vis.ViewModels.Pages;
using EBS3000Dashboard.Vis.Views;

namespace EBS3000Dashboard.Vis.ViewModels
{
  public class MainViewModel : NotifingObject
  {
    private PageViewModel _selectedPage;
    private RelayCommand _closeWindow;
    private RelayCommand _minimizeWindow;

    private IProductionServices _productionServices;
    private bool _isLiveStatus = true;
    private bool _isErrorHistory;

    public ICommand SettingsClicked => new RelayCommand(OnSettingsClicked);

    /// <summary>
    /// Initializes a new instance of the <see cref="MainViewModel"/> class.
    /// </summary>
    /// <param name="productionServices">The production services.</param>
    public MainViewModel(IProductionServices productionServices)
    {
      _productionServices = productionServices;

      // Initalize the pages here!
      this.Pages = new PageViewModel[]{
                App.Container.Resolve<LiveStatusViewModel>(),
                App.Container.Resolve<ErrorHistoryViewModel>()
            };


      this.SelectedPage = Pages.First();

      FirePropertyChanged("ToolBarContext");

    }

    public void StartServices()
    {
      /*
      foreach(var page in Pages)
      {
        (page as LiveStatusViewModel)?.UpdateViewIntervall(10, 5, 10, 5, 60);
      }
      */
    }

    /// <summary>
    /// Gets the pages.
    /// </summary>
    /// <value>
    /// The pages.
    /// </value>
    public IEnumerable<PageViewModel> Pages
    {
      get;
      private set;
    }

    public SettingsWindowViewModel SettingsWindowViewModel => App.Container.Resolve<SettingsWindowViewModel>();

    //public ToolbarViewModel ToolBarContext =>
    //    App.Container.Resolve<ToolbarViewModel>();

    /// <summary>
    /// Gets or sets the selected page.
    /// </summary>
    /// <value>
    /// The selected page.
    /// </value>
    public PageViewModel SelectedPage
    {
      get
      {
        return _selectedPage;
      }
      set
      {
        _selectedPage = value;
        foreach(var page in Pages)
          page.IsActive = _selectedPage.Equals(page);
        FirePropertyChanged();
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the live status view is currently selected.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the live status view is currently selected; otherwise, <c>false</c>.
    /// </value>
    public bool IsLiveStatus
    {
      get { return _isLiveStatus; }
      set
      {
        _isLiveStatus = value;
        if(value)
        {
          SelectedPage = Pages.FirstOrDefault(p => p.PageType == typeof(LiveStatusViewModel));
        }
        FirePropertyChanged();
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the error history view is currently selected.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the analysis view is currently selected; otherwise, <c>false</c>.
    /// </value>
    public bool IsErrorHistory
    {
      get { return _isErrorHistory; }
      set
      {
        _isErrorHistory = value;
        if(value)
        {
          SelectedPage = Pages.FirstOrDefault(p => p.PageType == typeof(ErrorHistoryViewModel));
        }
        FirePropertyChanged();
      }
    }

    /// <summary>
    /// Gets the command to close window.
    /// </summary>
    /// <value>
    /// The close window command.
    /// </value>
    public ICommand CloseWindow
    {
      get
      {
        return _closeWindow ??
               (_closeWindow =
                 new RelayCommand(new Action<object>((args) =>
                                                       ExitApplication()),
                   new Predicate<object>(delegate
                   {
                     return true;
                   })));
      }
    }

    private void ExitApplication()
    {
      _productionServices?.Context?.Dispose();

      foreach(Window window in App.Current.Windows)
        window.Close();
      
      //App.Current.MainWindow.Close();
    }

    /// <summary>
    /// Gets the command to minimize window.
    /// </summary>
    /// <value>
    /// The minimize window command.
    /// </value>
    public ICommand MinimizeWindow => _minimizeWindow ??
                       (_minimizeWindow =
                           new RelayCommand(new Action<object>((args) =>
                           App.Current.MainWindow.WindowState = WindowState.Minimized),
                               new Predicate<object>(delegate
                               {
                                 return true;
                               })));

    /// <summary>
    /// Gets the command to switch window between full screen and normal screen.
    /// </summary>
    /// <value>
    /// The switch full screen command.
    /// </value>
    public ICommand SwitchFullScreenCommand => new RelayCommand(SwitchFullScreen);

    /// <summary>
    /// Gets a value indicating whether the window is currently full screen.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the window is currently full screen; otherwise, <c>false</c>.
    /// </value>
    public bool IsFullScreen
    {
      get { return App.Current.MainWindow.WindowState == WindowState.Maximized; }
    }

    /// <summary>
    /// Switches the window between full screen and normal screen.
    /// </summary>
    /// <param name="obj">The object.</param>
    private void SwitchFullScreen(object obj)
    {
      App.Current.MainWindow.WindowState = IsFullScreen ? WindowState.Normal : WindowState.Maximized;

      FirePropertyChanged("IsFullScreen");
    }

    private void OnSettingsClicked(object obj)
    {
      var settingsWindow = App.Container.Resolve<SettingsWindowView>();

      settingsWindow.ShowDialog();
    }
  }
}
