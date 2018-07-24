using System.Windows;
using EBS3000Dashboard.Vis.ViewModels.Segments.DetailView;

namespace EBS3000Dashboard.Vis.Views.Segments.DetailViews
{
  /// <summary>
  /// Interaction logic for OutputDetailView.xaml
  /// </summary>
  public partial class OutputDetailView : Window
  {
        public OutputDetailView()
        {


        }
    public OutputDetailView(OutputDetailViewModel model)
    {
      InitializeComponent();

      DataContext = model;
    }
  }
}
