using System;
using System.Reactive.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using EBS3000Dashboard.Vis.ViewModels.Segments;

namespace EBS3000Dashboard.Vis.Views.Segments
{
    /// <summary>
    /// Interaction logic for AnalysisToolbarView.xaml
    /// </summary>
    public partial class AnalysisToolbarView : UserControl
    {
        

        public AnalysisToolbarView()
        {
            InitializeComponent();

            Observable.FromEventPattern<KeyEventArgs>(this, "KeyDown")
                .Where(eve => eve.EventArgs.Key == Key.Escape &&
                     this.DataContext != null)
                .Subscribe(key => {
                    (DataContext as AnalysisToolbarViewModel).CloseAllTimeSpanSettingPopups();
                });
        }
    }
}