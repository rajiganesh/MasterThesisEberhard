using System.Windows;
using EBS3000Dashboard.Vis.ViewModels.Segments.DetailView;

namespace EBS3000Dashboard.Vis.Views.Segments.DetailViews
{
  /// <summary>
  /// Interaction logic for OkNokPartsDetailView.xaml
  /// </summary>
  public partial class OkNokPartsDetailView : Window
  {
        public OkNokPartsDetailView()
        {

        }
    public OkNokPartsDetailView(OkNokPartsDetailViewModel model)
    {
      InitializeComponent();

      DataContext = model;
    }
  }
}
