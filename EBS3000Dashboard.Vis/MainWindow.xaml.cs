using System;
using System.Globalization;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using EBS3000Dashboard.Vis.ViewModels;

namespace EBS3000Dashboard.Vis
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    private IDisposable _windowMoveObserver;
    private IDisposable _toolbarDoubleClickObserver;

    public MainWindow(MainViewModel model)
    {
      InitializeComponent();

      App.LanguageChanged += LanguageChanged;
      CultureInfo currLang = App.Language;
      UpdateLanguage(currLang);

      this.DataContext = model;

      // move the window when the state is not full!
      _windowMoveObserver = Observable.FromEventPattern<MouseEventArgs>(PART_ToolBar, "MouseMove").
          Where(args => args.EventArgs.LeftButton == MouseButtonState.Pressed).
          Subscribe(eve => DragMove());

      // Maximize/ minimize the window by double clicking
      _toolbarDoubleClickObserver = Observable.FromEventPattern<MouseButtonEventArgs>(PART_ToolBar, "MouseLeftButtonDown").
        Where(args => args.EventArgs.ClickCount == 2).
        Subscribe(eve => WindowState = WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal);

      // start the production after the window has been activated!
      Observable.FromEventPattern<RoutedEventArgs>(Application.Current.MainWindow, "Loaded")
          .Subscribe(i => model.StartServices());


    }

    private void UpdateLanguage(CultureInfo currentCultureInfo)
    {
      GetMenuLanguage().Items.Clear();
      foreach(var lang in App.Languages)
      {
        MenuItem menuLang = new MenuItem();

        if(lang.IetfLanguageTag == "en-US")
        {
          menuLang.Header = lang.IetfLanguageTag + "," + lang.DisplayName;
        }
        else if(lang.IetfLanguageTag == "de-DE")
        {
          menuLang.Header = lang.IetfLanguageTag + "," + lang.DisplayName;
        }
        else if(lang.IetfLanguageTag == "fr-FR")
        {
          menuLang.Header = lang.IetfLanguageTag + "," + lang.DisplayName;
        }
        else if(lang.IetfLanguageTag == "es-ES")
        {
          menuLang.Header = lang.IetfLanguageTag + "," + lang.DisplayName;
        }
        menuLang.Tag = lang;
        menuLang.IsChecked = lang.Equals(currentCultureInfo);
        menuLang.Click += ChangeLanguageClick;
        GetMenuLanguage().Items.Add(menuLang);

      }
    }

    private MenuItem GetMenuLanguage()
    {
      return menuLanguage;
    }

    private void LanguageChanged(Object sender, EventArgs e)
    {
      CultureInfo currLang = App.Language;

      foreach(MenuItem i in menuLanguage.Items)
      {
        CultureInfo ci = i.Tag as CultureInfo;
        i.IsChecked = ci != null && ci.Equals(currLang);
      }

    }

    private void ChangeLanguageClick(Object sender, EventArgs e)
    {
      MenuItem mi = sender as MenuItem;
      if(mi != null)
      {
        CultureInfo lang = mi.Tag as CultureInfo;
        if(lang != null)
        {
          App.Language = lang;
        }
      }

    }
  }
}
