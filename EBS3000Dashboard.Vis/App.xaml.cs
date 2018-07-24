using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using Autofac;
using EBS3000Dashboard.Interface;
using EBS3000Dashboard.Vis.Services;
using EBS3000Dashboard.Vis.ViewModels;
using EBS3000Dashboard.Vis.ViewModels.Pages;
using EBS3000Dashboard.Vis.ViewModels.Segments;
using EBS3000Dashboard.Vis.ViewModels.Segments.DetailView;
using EBS3000Dashboard.Vis.Views;
using EBS3000Dashboard.Vis.Views.Segments;
using EBS3000Dashboard.Vis.Views.Segments.DetailViews;

namespace EBS3000Dashboard.Vis
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    private static List<CultureInfo> m_Languages = new List<CultureInfo>();

    public static List<CultureInfo> Languages
    {
      get
      {
        return m_Languages;
      }
    }
    protected override void OnStartup(StartupEventArgs e)
    {

      //var _mutex = new Mutex(true, "ohiofhe-bfwoefweie-sqwpoiew-q892zwio9", out bool _isSingleInstance);
      //if (!_isSingleInstance)
      //{
      //    Environment.Exit(0);
      //}
      //if (Process.GetProcessesByName("Eberhard.Messe").Length > 1)
      //{
      //    //You could use a MessageBox but then the user could spam this MessageBox. So I just Use:
      //    MessageBox.Show("EIS3D App ist bereits geöffnet !!!");
      //    return;
      //}
      Process proc = Process.GetCurrentProcess();
      int count = Process.GetProcesses().Where(p =>
                       p.ProcessName == proc.ProcessName).Count();
      if(count > 1)
      {
        MessageBox.Show("EIS3D App ist bereits geöffnet !!!" + Environment.NewLine + "EIS3D App is already open !!!", "Warning !!", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
        App.Current.Shutdown();
      }


      App.LanguageChanged += App_LanguageChanged;
      Language = Settings.Settings.Default.DefaultLanguage;
      InitializeLanguages();

      base.OnStartup(e);

      // resolve dependecies
      BootStrap();
      // resolve the main window
      var mainWindow = Container.Resolve<MainWindow>();

      mainWindow.Show();
    }

    public static event EventHandler LanguageChanged;

    public static CultureInfo Language
    {
      get
      {
        return System.Threading.Thread.CurrentThread.CurrentUICulture;
      }
      set
      {
        if(value == null) throw new ArgumentNullException("value");
        if(value == System.Threading.Thread.CurrentThread.CurrentUICulture) return;

        System.Threading.Thread.CurrentThread.CurrentUICulture = value;

        ResourceDictionary dict = new ResourceDictionary();
        switch(value.Name)
        {
          case "en-US":
            dict.Source = new Uri(String.Format("Resources/lang.{0}.xaml", value.Name), UriKind.Relative);
            break;
          case "fr-FR":
            dict.Source = new Uri(String.Format("Resources/lang.{0}.xaml", value.Name), UriKind.Relative);
            break;
          case "es-ES":
            dict.Source = new Uri(String.Format("Resources/lang.{0}.xaml", value.Name), UriKind.Relative);
            break;
          case "de-DE":
            dict.Source = new Uri(String.Format("Resources/lang.{0}.xaml", value.Name), UriKind.Relative);
            break;
          default:
            dict.Source = new Uri(String.Format("Resources/lang.{0}.xaml", Settings.Settings.Default.DefaultLanguage), UriKind.Relative);
            break;
        }

        ResourceDictionary oldDict = (from d in Application.Current.Resources.MergedDictionaries
                                      where d.Source != null && d.Source.OriginalString.StartsWith("Resources/lang.")
                                      select d).First();
        if(oldDict != null)
        {
          int ind = Application.Current.Resources.MergedDictionaries.IndexOf(oldDict);
          Application.Current.Resources.MergedDictionaries.Remove(oldDict);
          Application.Current.Resources.MergedDictionaries.Insert(ind, dict);
        }
        else
        {
          Application.Current.Resources.MergedDictionaries.Add(dict);
        }

        LanguageChanged(Application.Current, new EventArgs());
      }
    }

    private void App_LanguageChanged(Object sender, EventArgs e)
    {
      Settings.Settings.Default.DefaultLanguage = Language;
      Settings.Settings.Default.Save();

    }

    private void InitializeLanguages()
    {
      m_Languages.Clear();
      if(Settings.Settings.Default.Activate_en)
        m_Languages.Add(new CultureInfo("en-US"));
      if(Settings.Settings.Default.Activate_de)
        m_Languages.Add(new CultureInfo("de-DE"));
      if(Settings.Settings.Default.Activate_es)
        m_Languages.Add(new CultureInfo("es-ES"));
      if(Settings.Settings.Default.Activate_fr)
        m_Languages.Add(new CultureInfo("fr-FR"));

    }

    public static IContainer Container
    {
      get;
      private set;
    }

    private void BootStrap()
    {
      var builder = new ContainerBuilder();

      // Services
      builder.RegisterType<DashboardServices>()
        .As<IDashboard>()
        .SingleInstance();

      builder.RegisterType<ProductionServices>()
          .As<IProductionServices>()
          .SingleInstance();

      // Model Classes
      builder.RegisterType<IHmiMessage>()
        .As<IHmiMessage>();

      // Windows
      builder.RegisterType<MainWindow>()
        .AsSelf()
        .SingleInstance();

      builder.RegisterType<SettingsWindowView>()
        .AsSelf()
        .SingleInstance();

      builder.RegisterType<OpModeDetailView>()
        .AsSelf()
        .SingleInstance();

      builder.RegisterType<CommonErrorsDetailView>()
        .AsSelf()
        .SingleInstance();

      builder.RegisterType<OkNokPartsDetailView>()
        .AsSelf()
        .SingleInstance();

      builder.RegisterType<OutputDetailView>()
        .AsSelf()
        .SingleInstance();

      builder.RegisterType<PcbCameraSelfCheckDetailView>()
        .AsSelf()
        .SingleInstance();

      builder.RegisterType<TemperatureDetailView>()
        .AsSelf()
        .SingleInstance();

      // Viewmodels
      builder.RegisterType<MainViewModel>()
          .AsSelf()
          .SingleInstance();
  
      builder.RegisterType<LiveStatusViewModel>()
          .AsSelf()
          .SingleInstance();

      builder.RegisterType<ErrorHistoryViewModel>()
        .AsSelf()
        .SingleInstance();

      builder.RegisterType<AnalysisToolbarViewModel>()
          .AsSelf()
          .SingleInstance();

      builder.RegisterType<ErrorListViewModel>()
          .AsSelf()
          .SingleInstance();

      builder.RegisterType<OpModeChartViewModel>()
        .AsSelf()
        .SingleInstance();

      builder.RegisterType<OpModeDetailViewModel>()
        .AsSelf()
        .SingleInstance();

      builder.RegisterType<OkNokPartsChartViewModel>()
        .AsSelf()
        .SingleInstance();

      builder.RegisterType<OkNokPartsDetailViewModel>()
        .AsSelf()
        .SingleInstance();

      builder.RegisterType<CommonErrorsChartViewModel>()
        .AsSelf()
        .SingleInstance();

      builder.RegisterType<CommonErrorsDetailViewModel>()
        .AsSelf()
        .SingleInstance();

      builder.RegisterType<TemperatureChartViewModel>()
        .AsSelf()
        .SingleInstance();

      builder.RegisterType<TemperatureDetailViewModel>()
        .AsSelf()
        .SingleInstance();

      builder.RegisterType<OutputChartViewModel>()
        .AsSelf()
        .SingleInstance();

      builder.RegisterType<OutputDetailViewModel>()
        .AsSelf()
        .SingleInstance();

      builder.RegisterType<PcbCameraSelfCheckChartViewModel>()
        .AsSelf()
        .SingleInstance();

      builder.RegisterType<PcbCameraSelfCheckDetailViewModel>()
        .AsSelf()
        .SingleInstance();

      builder.RegisterType<SettingsWindowViewModel>()
        .AsSelf()
        .SingleInstance();

      builder.RegisterType<GeneralMachineDataViewModel>()
        .AsSelf()
        .SingleInstance();

      Container = builder.Build();
    }

    private void Application_Exit(object sender, ExitEventArgs e)
    {
      App.Current.Shutdown();
      Environment.Exit(0);
    }
  }
}
