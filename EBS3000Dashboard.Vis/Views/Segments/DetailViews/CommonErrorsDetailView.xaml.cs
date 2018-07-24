using System.Windows;
using EBS3000Dashboard.Vis.ViewModels.Segments.DetailView;

namespace EBS3000Dashboard.Vis.Views.Segments.DetailViews
{
  /// <summary>
  /// Interaction logic for CommonErrorsDetailView.xaml
  /// </summary>
  public partial class CommonErrorsDetailView : Window
  {
        public CommonErrorsDetailView()
        {

        }
    public CommonErrorsDetailView(CommonErrorsDetailViewModel model)
    {
      InitializeComponent();

      DataContext = model;
    }
  }
}
