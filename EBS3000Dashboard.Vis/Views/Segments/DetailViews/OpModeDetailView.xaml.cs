using System.Windows;
using EBS3000Dashboard.Vis.ViewModels.Segments.DetailView;

namespace EBS3000Dashboard.Vis.Views.Segments.DetailViews
{
  /// <summary>
  /// Interaction logic for OpModeDetailView.xaml
  /// </summary>
  public partial class OpModeDetailView : Window
  {
        public OpModeDetailView()
        {

        }
    public OpModeDetailView(OpModeDetailViewModel model)
    {
      InitializeComponent();

      DataContext = model;
    }
  }
}
