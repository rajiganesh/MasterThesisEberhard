using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Eberhard.Core.Utilities;
using EBS3000Dashboard.Vis.Services;

namespace EBS3000Dashboard.Vis.ViewModels.Segments.DetailView
{
  public class OpModeDetailViewModel : NotifingObject
  {
    private IProductionServices _productionServices;

    private ObservableCollection<KeyValuePair<DateTime, int>> _opMode1Data;

    public ObservableCollection<KeyValuePair<DateTime, int>> OpMode1Data
    {
      get
      {
        return _opMode1Data;
      }
      set
      {
        _opMode1Data = value;
        FirePropertyChanged();
      }
    }

    private ObservableCollection<KeyValuePair<DateTime, int>> _opMode2Data;

    public ObservableCollection<KeyValuePair<DateTime, int>> OpMode2Data
    {
      get
      {
        return _opMode2Data;
      }
      set
      {
        _opMode2Data = value;
        FirePropertyChanged();
      }
    }

    public OpModeDetailViewModel(IProductionServices production)
    {
      _productionServices = production;

      UpdateView();
    }

    public RelayCommand CloseClicked => new RelayCommand(OnCloseClicked);

    public void UpdateView()
    {
      var opMode1 = new Collection<KeyValuePair<DateTime, bool>>
              {
                new KeyValuePair<DateTime, bool>(DateTime.Today + new TimeSpan(0, 0, 0), false),
                new KeyValuePair<DateTime, bool>(DateTime.Today + new TimeSpan(1, 0, 0), true),
                new KeyValuePair<DateTime, bool>(DateTime.Today + new TimeSpan(2, 0, 0), false),
                new KeyValuePair<DateTime, bool>(DateTime.Today + new TimeSpan(5, 0, 0), true),
                new KeyValuePair<DateTime, bool>(DateTime.Today + new TimeSpan(7, 0, 0), false),
                new KeyValuePair<DateTime, bool>(DateTime.Today + new TimeSpan(8, 0, 0), true)
              };

      var opMode2 = new Collection<KeyValuePair<DateTime, bool>>
                    {
                      new KeyValuePair<DateTime, bool>(DateTime.Today + new TimeSpan(0, 0, 0), true),
                      new KeyValuePair<DateTime, bool>(DateTime.Today + new TimeSpan(1, 0, 0), false),
                      new KeyValuePair<DateTime, bool>(DateTime.Today + new TimeSpan(2, 0, 0), true),
                      new KeyValuePair<DateTime, bool>(DateTime.Today + new TimeSpan(5, 0, 0), false),
                      new KeyValuePair<DateTime, bool>(DateTime.Today + new TimeSpan(7, 0, 0), true),
                      new KeyValuePair<DateTime, bool>(DateTime.Today + new TimeSpan(8, 0, 0), false)
                    };

      OpMode1Data = GetDiagrammData(opMode1, -1, 0);
      OpMode2Data = GetDiagrammData(opMode2, 1, 2);
    }

    private void OnCloseClicked(object obj)
    {
      ;
    }

    private static ObservableCollection<KeyValuePair<DateTime, int>> GetDiagrammData(Collection<KeyValuePair<DateTime, bool>> values, int offPos, int onPos)
    {
      var retval = new ObservableCollection<KeyValuePair<DateTime, int>>();

      for(var index = 0; index < values.Count; index++)
      {
        if(index > 0)
          retval.Add(new KeyValuePair<DateTime, int>(values[index].Key - new TimeSpan(1), values[index - 1].Value ? onPos : offPos));

        retval.Add(values[index].Value ? new KeyValuePair<DateTime, int>(values[index].Key, onPos) : new KeyValuePair<DateTime, int>(values[index].Key, offPos));
      }

      return retval;
    }
  }
}
