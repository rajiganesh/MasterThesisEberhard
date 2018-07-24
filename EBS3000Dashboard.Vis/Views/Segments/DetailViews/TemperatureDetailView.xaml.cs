using System.Windows;
using EBS3000Dashboard.Vis.ViewModels.Segments.DetailView;

namespace EBS3000Dashboard.Vis.Views.Segments.DetailViews
{
  /// <summary>
  /// Interaction logic for TemperatureDetailView.xaml
  /// </summary>
  public partial class TemperatureDetailView : Window
  {
    public TemperatureDetailView()
    {

    }
    public TemperatureDetailView(TemperatureDetailViewModel model)
    {
      InitializeComponent();

      DataContext = model;
    }
  }
}
