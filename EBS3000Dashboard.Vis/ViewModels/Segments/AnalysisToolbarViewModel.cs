using System;
using System.Collections.Generic;
using System.Windows.Input;
using Eberhard.Core.BaseTypes;
using Eberhard.Core.Utilities;
using EBS3000Dashboard.Interface;
using EBS3000Dashboard.Vis.Services;

namespace EBS3000Dashboard.Vis.ViewModels.Segments
{
  /// <summary>
  /// The view model that contains the relevant settings for analysis
  /// view.
  /// </summary>
  /// <seealso cref="Eberhard.Core.Utilities.NotifingObject" />
  public class AnalysisToolbarViewModel : NotifingObject
  {
    #region Fields  

    private DateTime _analysisEndDateTime = DateTime.Now;
    private DateTime _analysisStartDateTime = DateTime.Now - new TimeSpan(30, 0, 0, 0);

    private string _fromPartId;
    private bool _isEndDatePopupOpen;
    private bool _isEndTimePopupOpen;
    private bool _isPinsSettingOpened;
    private bool _isSelectionManual;
    private bool _isFilteringBySelection = true;
    private bool _isStartDatePopupOpen;
    private bool _isStartTimePopupOpen;
    private bool _isTimeSpanSettingOpened;
    private MessageTypeEnum _selectedMessageType;
    //private SubPageViewModel _selectedSubpage;
    private IProductionServices _services;
    private string _toPartId;
    private int _numberOfMeasurements = 0;
    //private IEnumerable<SubPageViewModel> _subPages;

    #endregion Fields

    public event EventHandler NewFilterSettingsApplied;

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="AnalysisToolbarViewModel"/> class.
    /// </summary>
    /// <param name="services">The services.</param>
    public AnalysisToolbarViewModel(IProductionServices services)
    {
      _services = services;

      /*
        _services.AnalysisObservable.ObserveOnDispatcher()
            .Subscribe(measurements =>
            {
                _currentMeasurements = measurements;
            });
    */
      FirePropertyChanged("Products");

      // fill the result options here!
      FirePropertyChanged("ResultOptions");
      FirePropertyChanged("Coordinates");
    }

    #endregion Constructors

    #region Properties


    private Coordinate _selectedCoordinate;

    public Coordinate SelectedCoordinate
    {
      get { return _selectedCoordinate; }
      private set
      {
        _selectedCoordinate = value;
        FirePropertyChanged();
      }
    }

    /// <summary>
    /// Gets or sets the end date of the analysis data.
    /// </summary>
    /// <value>
    /// The analysis end date.
    /// </value>
    public DateTime AnalysisEndDate
    {
      get { return AnalysisEndDateTime.Date; }
      set
      {
        AnalysisEndDateTime = new DateTime(value.Year, value.Month, value.Day, AnalysisEndTime.Hour, AnalysisEndTime.Minute, AnalysisEndTime.Second, AnalysisEndTime.Millisecond);
        IsEndDatePopupOpen = false;
      }
    }

    /// <summary>
    /// Gets or sets the end timestamp (date and time) of the analysis data.
    /// </summary>
    /// <value>
    /// The analysis end date time.
    /// </value>
    public DateTime AnalysisEndDateTime
    {
      get
      {
        return _analysisEndDateTime;
      }
      set
      {
        _analysisEndDateTime = value;
        UpdateDateAndTime(false);

        //// Don't set ToPartId directly to avoid endless loop
        //_toPartId = GetLastPartIdBefore(value);//This creates problems in resetting identifiers/from time settings and overall filtered results are not shown as expected
        //FirePropertyChanged("ToPartId");
      }
    }

    /// <summary>
    /// Gets or sets the end time of the analysis data.
    /// </summary>
    /// <value>
    /// The analysis end time.
    /// </value>
    public DateTime AnalysisEndTime
    {
      get { return AnalysisEndDateTime; }
      set
      {
        AnalysisEndDateTime = new DateTime(AnalysisEndDate.Year, AnalysisEndDate.Month, AnalysisEndDate.Day, value.Hour, value.Minute, value.Second, value.Millisecond);
        IsEndTimePopupOpen = false;
      }
    }

    /// <summary>
    /// Gets or sets the start date of the analysis data.
    /// </summary>
    /// <value>
    /// The analysis start date.
    /// </value>
    public DateTime AnalysisStartDate
    {
      get { return AnalysisStartDateTime.Date; }
      set
      {
        AnalysisStartDateTime = new DateTime(value.Year, value.Month, value.Day, AnalysisStartTime.Hour, AnalysisStartTime.Minute, AnalysisStartTime.Second, AnalysisStartTime.Millisecond);
        IsStartDatePopupOpen = false;
      }
    }

    /// <summary>
    /// Gets or sets the start timestamp (date and time) of the analysis data.
    /// </summary>
    /// <value>
    /// The analysis start date time.
    /// </value>
    public DateTime AnalysisStartDateTime
    {
      get
      {
        return _analysisStartDateTime;
      }
      set
      {
        _analysisStartDateTime = value;
        UpdateDateAndTime(true);

        //// Don't set FromPartId directly to avoid endless loop
        //_fromPartId = GetFirstPartIdAfter(value);//This creates problems in resetting identifiers/from time settings and overall filtered results are not shown as expected
        //FirePropertyChanged("FromPartId");
      }
    }

    /// <summary>
    /// Gets or sets the start time of the analysis data.
    /// </summary>
    /// <value>
    /// The analysis start time.
    /// </value>
    public DateTime AnalysisStartTime
    {
      get { return AnalysisStartDateTime; }
      set
      {
        AnalysisStartDateTime = new DateTime(AnalysisStartDate.Year, AnalysisStartDate.Month, AnalysisStartDate.Day, value.Hour, value.Minute, value.Second, value.Millisecond);
        IsStartTimePopupOpen = false;
      }
    }

    /// <summary>
    /// Gets the time span of the analysis data.
    /// </summary>
    /// <value>
    /// The analysis time span.
    /// </value>
    public string AnalysisTimeSpan
    {
      get
      {
        return AnalysisStartDateTime.ToShortDateString() + " - " + AnalysisStartDateTime.ToLongTimeString() + "  >  "
            + AnalysisEndDateTime.ToShortDateString() + " - " + AnalysisEndDateTime.ToLongTimeString();
      }
    }

    public ICommand ConfirmPinsSettingsCommand =>
        new RelayCommand(ConfirmPinsSettings, (s) => CanFilter);

    /// <summary>
    /// Gets the command that confirms the pin settings.
    /// </summary>
    /// <value>
    /// The confirm pins settings command.
    /// </value>
    public ICommand SelectCoordinate =>
        new RelayCommand(OnCoordinateSelected);

    private void OnCoordinateSelected(object coordinate) =>
        SelectedCoordinate = (Coordinate)coordinate;

    /// <summary>
    /// Gets the command that confirms the time span settings.
    /// </summary>
    /// <value>
    /// The confirm time span settings command.
    /// </value>
    public ICommand ConfirmTimeSpanSettingsCommand => new RelayCommand(ConfirmTimeSpanSettings);

    /// <summary>
    /// Gets the command that resets the time span settings.
    /// </summary>
    /// <value>
    /// The reset time span settings command.
    /// </value>
    public ICommand ResetTimeSpanSettingsCommand => new RelayCommand(ResetTimeSpanSettings);

    /// <summary>
    /// Gets or sets the part id of the measurement according to the start timestamp.
    /// </summary>
    /// <value>
    /// The part id of the measurement according to the start timestamp.
    /// </value>
    public string FromPartId
    {
      get { return _fromPartId; }
      set
      {
        _fromPartId = value;
        // Don't set AnalysisStartDateTime directly to avoid endless loop
        //_analysisStartDateTime = GetDateTimeFromId(value, AnalysisStartDateTime);
        if(_fromPartId != null)
        {
          _analysisStartDateTime = DateTime.Now - new TimeSpan(24, 0, 0);//(_services.Context as PinScanServices).GetTimestampByDmc(_fromPartId);
          UpdateDateAndTime(true);
        }

        FirePropertyChanged();
      }
    }

    /// <summary>
    /// Gets a value indicating whether any product is selected.
    /// </summary>
    /// <value>
    ///   <c>true</c> if any product is selected; otherwise, <c>false</c>.
    /// </value>
    public bool IsAnyProductSelected => SelectedMessageType != null;

    /// <summary>
    /// Gets a value indicating whether any settings popup is opened.
    /// </summary>
    /// <value>
    ///   <c>true</c> if any settings popup is opened; otherwise, <c>false</c>.
    /// </value>
    public bool IsAnySettingOpened
    {
      get { return IsPinsSettingOpened || IsTimeSpanSettingOpened; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the popup for the end date setting is open.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the popup for the end date setting is open; otherwise, <c>false</c>.
    /// </value>
    public bool IsEndDatePopupOpen
    {
      get { return _isEndDatePopupOpen; }
      set
      {
        _isEndDatePopupOpen = value;
        FirePropertyChanged("IsEndDatePopupOpen");
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the popup for the end time setting is open.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the popup for the end time setting is open; otherwise, <c>false</c>.
    /// </value>
    public bool IsEndTimePopupOpen
    {
      get { return _isEndTimePopupOpen; }
      set
      {
        _isEndTimePopupOpen = value;
        FirePropertyChanged("IsEndTimePopupOpen");
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the popup for the pins settings is open.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the popup for the pins settings is open; otherwise, <c>false</c>.
    /// </value>
    public bool IsPinsSettingOpened
    {
      get { return _isPinsSettingOpened; }
      set
      {
        _isPinsSettingOpened = value;
        FirePropertyChanged();
        FirePropertyChanged("IsAnySettingOpened");
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the selection is manual.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the selection is manual; otherwise, <c>false</c>.
    /// </value>
    public bool IsSelectionManual
    {
      get { return _isSelectionManual; }
      set
      {
        _isSelectionManual = value;
        FirePropertyChanged();
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the selection is using filter.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the selection is using filter; otherwise, <c>false</c>.
    /// </value>
    public bool IsFilteringBySelection
    {
      get { return _isFilteringBySelection; }
      set
      {
        _isFilteringBySelection = value;
        FirePropertyChanged();
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the popup for the start date setting is open.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the popup for the start date setting is open; otherwise, <c>false</c>.
    /// </value>
    public bool IsStartDatePopupOpen
    {
      get { return _isStartDatePopupOpen; }
      set
      {
        _isStartDatePopupOpen = value;
        FirePropertyChanged();
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the popup for the start time setting is open.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the popup for the start time setting is open; otherwise, <c>false</c>.
    /// </value>
    public bool IsStartTimePopupOpen
    {
      get { return _isStartTimePopupOpen; }
      set
      {
        _isStartTimePopupOpen = value;
        FirePropertyChanged();
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the popup for the timespan settings is open.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the popup for the timespan settings is open; otherwise, <c>false</c>.
    /// </value>
    public bool IsTimeSpanSettingOpened
    {
      get { return _isTimeSpanSettingOpened; }
      set
      {
        _isTimeSpanSettingOpened = value;

        if(!value)
        {
          CloseAllTimeSpanSettingPopups();
        }
        FirePropertyChanged();
        FirePropertyChanged("IsAnySettingOpened");
        FirePropertyChanged("AnalysisTimeSpan");
      }
    }

    /// <summary>
    /// Gets the command to open the popup for end date setting.
    /// </summary>
    /// <value>
    /// The open end date setting popup command.
    /// </value>
    public ICommand OpenEndDateSettingPopupCommand => new RelayCommand(OpenEndDateCalendar);

    /// <summary>
    /// Gets the command to open the popup for end time setting.
    /// </summary>
    /// <value>
    /// The open end time setting popup command.
    /// </value>
    public ICommand OpenEndTimeSettingPopupCommand => new RelayCommand(OpenEndTimeTimePicker);

    /// <summary>
    /// Gets the command to open the popup for start date setting.
    /// </summary>
    /// <value>
    /// The open start date setting popup command.
    /// </value>
    public ICommand OpenStartDateSettingPopupCommand => new RelayCommand(OpenStartDateCalendar);

    /// <summary>
    /// Gets the command to open the popup for start time setting.
    /// </summary>
    /// <value>
    /// The open start time setting popup command.
    /// </value>
    public ICommand OpenStartTimeSettingPopupCommand => new RelayCommand(OpenStartTimeTimePicker);

    public IEnumerable<MessageTypeEnum> MessageTypes => new List<MessageTypeEnum> { MessageTypeEnum.All, MessageTypeEnum.ErrorMessage, MessageTypeEnum.OperatorMessage};
    /*
    /// <summary>
    /// Gets the command to select all filter options.
    /// </summary>
    /// <value>
    /// The select all filters command.
    /// </value>
    public ICommand SelectAllFilterOptionsCommand => new RelayCommand((args) =>
               {
                   foreach (var option in SelectedSubPage.PinMapFilterOptions)
                       option.IsSelected = true;
               });
     */

    /// <summary>
    /// Gets or sets the selected product.
    /// </summary>
    /// <value>
    /// The selected product.
    /// </value>
    public MessageTypeEnum SelectedMessageType
    {
      get
      {
        return _selectedMessageType;
      }
      set
      {
        _selectedMessageType = value;
        FirePropertyChanged();
        NewFilterSettingsApplied?.Invoke(this, new EventArgs());
      }
    }

    /*
            public SubPageViewModel SelectedSubPage
            {
                get
                {
                    if (_selectedSubpage == null)
                    {
                        _selectedSubpage = SubPages.FirstOrDefault();
                    }
                    return _selectedSubpage;
                }
                set
                {
                    _selectedSubpage = value;

                    foreach (var page in SubPages)
                    {
                        page.IsActive = _selectedSubpage.Equals(page);
                        if (page.IsActive)
                            SelectedCoordinate = page.SelectedCoordinate;
                    }

                    FirePropertyChanged();
                }
            }

            /// <summary>
            /// Gets the list of available diagrams.
            /// </summary>
            /// <value>
            /// The list of available diagrams.
            /// </value>
            public IEnumerable<SubPageViewModel> SubPages
            {
                get
                {
                    return _subPages;
                }
                set
                {
                    _subPages = value;
                    FirePropertyChanged();
                }
            }
    */


    /// <summary>
    /// Gets or sets the part id of the measurement according to the end timestamp.
    /// </summary>
    /// <value>
    /// The part id of the measurement according to the end timestamp.
    /// </value>
    public string ToPartId
    {
      get { return _toPartId; }
      set
      {
        _toPartId = value;
        if(_toPartId != null)
        {
          _analysisEndDateTime = DateTime.Now;//(_services.Context as PinScanServices).GetTimestampByDmc(_toPartId);
          UpdateDateAndTime(false);
          // Don't set AnalysisEndDateTime directly to avoid endless loop
          //_analysisEndDateTime = GetDateTimeFromId(value, AnalysisEndDateTime); //This function from UID does not work
        }

        FirePropertyChanged();
      }
    }

    /// <summary>
    /// Gets the last x number of measurements according to the user entry
    /// </summary>
    /// <value>
    /// The number of last x measurements the user wants to observe
    /// </value>
    public bool IsNumberOfMeasurementsGiven => NumberOfMeasurements != 0;

    public int NumberOfMeasurements
    {
      get { return _numberOfMeasurements; }
      set
      {
        _numberOfMeasurements = value;
        //Get the last measurement of the selected product before the actual time : Input Start TimeStamp or Start Identifier             
        //Get the last x measurements before the actual time for the selected product by either 
        //finding out the start time or get the last x measurements directly

        FirePropertyChanged();
        FirePropertyChanged("IsNumberOfMeasurementsGiven");
      }
    }

    #endregion Properties

    #region Methods


    public bool CanFilter { get; private set; }

    /// <summary>
    /// Closes all popups in time setting popup.
    /// </summary>
    public void CloseAllTimeSpanSettingPopups()
    {
      IsStartDatePopupOpen = false;
      IsStartTimePopupOpen = false;
      IsEndDatePopupOpen = false;
      IsEndTimePopupOpen = false;
    }

    /// <summary>
    /// Confirms the pins settings.
    /// </summary>
    /// <param name="obj">The object.</param>
    private void ConfirmPinsSettings(object obj)
    {
      IsPinsSettingOpened = false;
      //_selectedSubpage.IsFilterActive = true;
      this.FirePropertyChanged("SettingsChanged");
    }

    /// <summary>
    /// Resets the time span settings.
    /// </summary>
    /// <param name="obj">The object.</param>
    private void ResetTimeSpanSettings(object obj)
    {
      _fromPartId = null;
      _toPartId = null;
      _numberOfMeasurements = 0;
      _analysisStartDateTime = DateTime.Now;
      _analysisEndDateTime = DateTime.Now;
      FirePropertyChanged("ToPartId");
      FirePropertyChanged("FromPartId");
      FirePropertyChanged("NumberOfMeasurements");
      UpdateDateAndTime(false);
      UpdateDateAndTime(true);
    }


    /// <summary>
    /// Confirms the time span settings.
    /// </summary>
    /// <param name="obj">The object.</param>
    private void ConfirmTimeSpanSettings(object obj)
    {
      IsTimeSpanSettingOpened = false;
      CanFilter = true;
      FirePropertyChanged("CanFilter");
      /*
        if (_selectedProduct == "MOCKUP_PRODUCT")
        {
            //For mockup data, Identifiers does not make any sense, but number of measurements can make sense
            (_services.Context as PinScanServices).SimulateFilteringbyTime(_analysisStartDateTime, _analysisEndDateTime, _selectedProduct,_numberOfMeasurements);
        }
        else if (_selectedProduct != "MOCKUP_PRODUCT" && _fromPartId == null && _toPartId == null)
        {
            //When Start and End Datetime are same
            if(DateTime.Equals(_analysisStartDateTime,_analysisEndDateTime) == true)
            {
                (_services.Context as PinScanServices).SimulateFilteringbyTime(_analysisStartDateTime, _analysisEndDateTime, _selectedProduct, _numberOfMeasurements);
            }
            else
            {
                (_services.Context as PinScanServices).SimulateFilteringbyTime(_analysisStartDateTime, _analysisEndDateTime, _selectedProduct, 0);
            }
            //For real data with time interval input

        }
        else if (_selectedProduct != "MOCKUP_PRODUCT" && _fromPartId != null && _toPartId != null)
        {
            //For real data with "from to" identifiers input
            (_services.Context as PinScanServices).SimulateFilteringbyDmc(_fromPartId, _toPartId, _selectedProduct);


        }
        */
      FromPartId = null;
      ToPartId = null;

      NewFilterSettingsApplied?.Invoke(this, new EventArgs());
    }

    /// <summary>
    /// Opens the calendar to set the end date.
    /// </summary>
    /// <param name="obj">The object.</param>
    private void OpenEndDateCalendar(object obj)
    {
      CloseAllTimeSpanSettingPopups();
      IsEndDatePopupOpen = true;
    }

    /// <summary>
    /// Opens the time picker to set the end time.
    /// </summary>
    /// <param name="obj">The object.</param>
    private void OpenEndTimeTimePicker(object obj)
    {
      CloseAllTimeSpanSettingPopups();
      IsEndTimePopupOpen = true;
    }

    /// <summary>
    /// Opens the calendar to set the start date .
    /// </summary>
    /// <param name="obj">The object.</param>
    private void OpenStartDateCalendar(object obj)
    {
      CloseAllTimeSpanSettingPopups();
      IsStartDatePopupOpen = true;
    }

    /// <summary>
    /// Opens the time picker to set the start time.
    /// </summary>
    /// <param name="obj">The object.</param>
    private void OpenStartTimeTimePicker(object obj)
    {
      CloseAllTimeSpanSettingPopups();
      IsStartTimePopupOpen = true;
    }

    /// <summary>
    /// Updates the date and time displayed as button content.
    /// </summary>
    /// <param name="isStartDateTime">if set to <c>true</c> is for start date and time.</param>
    private void UpdateDateAndTime(bool isStartDateTime)
    {
      // the date and time of the start timestamp should be updated
      if(isStartDateTime)
      {
        FirePropertyChanged("AnalysisStartDate");
        FirePropertyChanged("AnalysisStartTime");
      }
      else // the date and time of the end timestamp should be updated
      {
        FirePropertyChanged("AnalysisEndDate");
        FirePropertyChanged("AnalysisEndTime");
      }
    }

    #endregion Methods
  }
}