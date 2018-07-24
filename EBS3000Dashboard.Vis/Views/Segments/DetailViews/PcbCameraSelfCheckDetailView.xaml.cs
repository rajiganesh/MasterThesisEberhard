using System.Windows;
using EBS3000Dashboard.Vis.ViewModels.Segments.DetailView;

namespace EBS3000Dashboard.Vis.Views.Segments.DetailViews
{
  /// <summary>
  /// Interaction logic for PcbCameraSelfCheckDetailView.xaml
  /// </summary>
  public partial class PcbCameraSelfCheckDetailView : Window
  {
    public PcbCameraSelfCheckDetailView(PcbCameraSelfCheckDetailViewModel model)
    {
      InitializeComponent();

      DataContext = model;
    }
  }
}
