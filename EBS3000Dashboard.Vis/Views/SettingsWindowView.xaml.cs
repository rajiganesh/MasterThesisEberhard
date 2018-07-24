using System.Windows;
using EBS3000Dashboard.Vis.ViewModels;

namespace EBS3000Dashboard.Vis.Views
{
  /// <summary>
  /// Interaction logic for SettingsWindow.xaml
  /// </summary>
  public partial class SettingsWindowView : Window
  {
    public SettingsWindowView(SettingsWindowViewModel model)
    {
      InitializeComponent();

      DataContext = model;
    }
  }
}
