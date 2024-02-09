using RootCauseExplorer;
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace RootCauseExplorer
{
    public class NotifyingString
    {
        public delegate void StringChangedEventHandler(string NewInput);
        public event StringChangedEventHandler StringChanged;

        private string notifyingstringcontent;
        public string Value
        {
            get { return this.notifyingstringcontent; }
            set
            {
                this.notifyingstringcontent = value;
                if (this.StringChanged != null) this.StringChanged(value);
            }
        }
        public NotifyingString(string input)
        {
            notifyingstringcontent = input;
        }
    }

    public partial class OverViewCalendar : UserControl
    {
        private IDictionary<string, string> Parameters = new Dictionary<string, string>();
        private IDictionary<string, bool> Filters = new Dictionary<string, bool>();
        private XDocument XMLCache;
        public DateTime CurrentSetting = DateTime.Now;
        public int numEventsThisMonth = 0;
        public NotifyingString CurrentView = new NotifyingString("Quarterly");
        private XNamespace z = "#RowsetSchema";
        public DateTime SelectedDate1, SelectedDate2;
        public int numSelectedDates = 0;
        public static OverViewCalendar CalendarObject;
        private double ActualMargin = 22;
        public bool LeftMouseIsDown = false;


        public OverViewCalendar()
        {
            InitializeComponent();
                CalendarObject = this;

                FilterButtons.Children.Add(new FilterButton("EVENTS TEAM", "EventsIcon.png"));
                FilterButtons.Children.Add(new FilterButton("EXEC COMM. TEAM", "ExecComIcon.png"));
                FilterButtons.Children.Add(new FilterButton("DEMO TEAM", "DemoIcon.png"));
                Filters.Add("EVENTS TEAM", false);
                Filters.Add("EXEC COMM. TEAM", false);
                Filters.Add("DEMO TEAM", false);

                PointerPressed += OverViewCalendar_PointerPressed;
                PointerReleased += OverViewCalendar_PointerReleased;

                CurrentView.StringChanged += new NotifyingString.StringChangedEventHandler(CurrentView_StringChanged);

                ViewChanger.UpdateText();
                GetDummyData();
        }

        private void OverViewCalendar_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            LeftMouseIsDown = false;
        }

        private void OverViewCalendar_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            LeftMouseIsDown = true;
        }

        void CurrentView_StringChanged(string NewInput)
        {
            VisualStateManager.GoToState(CalendarSwitch, NewInput, true);
            numSelectedDates = 0;
            CalendarGrid.Children.Clear();
            ViewChanger.UpdateText();
            populateCalender();
        }

        public void ToggleFilter(string Filtername)
        {
            Filters[Filtername] = !Filters[Filtername];
            UpdateCalendar();
        }

        private void GetDummyData()
        {
            XMLCache = XDocument.Load("ms-appx:///Data/EventOverview.xml");
            CurrentView.Value = "Monthly";
        }
        

        public void populateCalender()
        {
            double offset = 0;
            CalendarGrid.Width = 0;

            if (CurrentView.Value == "Monthly")
            {
                numEventsThisMonth = 0;
                CalendarGrid.Width += 22;
                bool YesterdayWasWeekendOrHoliday = false;
                DateTime FirstOfMonth = new DateTime(CurrentSetting.Year, CurrentSetting.Month, 1);
                int DaysInMonth = DateTime.DaysInMonth(CurrentSetting.Year, CurrentSetting.Month);

                foreach (XElement record in XMLCache.Descendants(z + "row"))
                {
                    //setting a value that will return 0, when checked, so that if the event has no date 
                    //(should be impossible, but who knows), it will clear the checks below without breaking
                    string CheckDate = "0000000";
                    CheckDate = (string)record.Attribute("ows_EventDate");
                    if (Int32.Parse(CheckDate.Substring(5, 2)) == CurrentSetting.Month)
                    {
                        numEventsThisMonth++;
                    }
                    else
                    {
                        CheckDate = (string)record.Attribute("ows_EndDate");
                        if (Int32.Parse(CheckDate.Substring(5, 2)) == CurrentSetting.Month)
                        {
                            numEventsThisMonth++;
                        }
                    }
                }
                //38 is the Event.xaml usercontrol's height when displaying in month mode
                CalendarGrid.Height = numEventsThisMonth * 38 + 160;
                LayoutRoot.UpdateLayout();
                // System.Windows.Browser.HtmlPage.Window.Eval("document.getElementById('silverlightControlHost').style.height='" + LayoutRoot.ActualHeight.ToString() + "';");

                // start working through the days
                for (int count = 0; count < DaysInMonth; count++)
                {

                    DateColumn dc = new DateColumn(FirstOfMonth);

                    //Is a Weekend
                    if (FirstOfMonth.DayOfWeek == DayOfWeek.Sunday || FirstOfMonth.DayOfWeek == DayOfWeek.Saturday)
                    {
                        dc.MakeWeekend();
                        YesterdayWasWeekendOrHoliday = true;
                    }

                    if (FirstOfMonth.Day == DateTime.DaysInMonth(FirstOfMonth.Year, FirstOfMonth.Month))
                    {
                        dc.MakeLastDayOfMonth();
                    }

                    dc.Margin = new Thickness(offset, 0, 0, 0);
                    CalendarGrid.Width += dc.Width;
                    CalendarGrid.Children.Add(dc);

                    if (YesterdayWasWeekendOrHoliday) { offset += 22; YesterdayWasWeekendOrHoliday = false; } else offset += 43;

                    FirstOfMonth = FirstOfMonth.AddDays(1);



                }

                ActualMargin = (LayoutRoot.Width - CalendarGrid.Width) / 2;
            }
            else if (CurrentView.Value == "Quarterly")
            {
                CalendarGrid.Height = 196;
                LayoutRoot.UpdateLayout();
                //System.Windows.Browser.HtmlPage.Window.Eval("document.getElementById('silverlightControlHost').style.height='" + LayoutRoot.ActualHeight.ToString() + "';");
                DateTime FirstOfMonth = new DateTime();
                DateTime StartOfWeek = new DateTime();
                int DaysInMonth = 0;

                int FiscalQuarter = IntMonthToFiscalQuarterInt(CurrentSetting.Month);
                switch (FiscalQuarter)
                {
                    case (1): FirstOfMonth = new DateTime(CurrentSetting.Year, 7, 1); break;
                    case (2): FirstOfMonth = new DateTime(CurrentSetting.Year, 10, 1); break;
                    case (3): FirstOfMonth = new DateTime(CurrentSetting.Year, 1, 1); break;
                    case (4): FirstOfMonth = new DateTime(CurrentSetting.Year, 4, 1); break;
                }



                //First Month of the Quarter
                for (int QuarterMonth = 1; QuarterMonth <= 3; QuarterMonth++)
                {
                    DaysInMonth = DateTime.DaysInMonth(CurrentSetting.Year, FirstOfMonth.Month);
                    StartOfWeek = FirstOfMonth;

                    DateColumn header = new DateColumn(IntToMonthString(FirstOfMonth.Month));
                    header.Margin = new Thickness(offset, 0, 0, 0);
                    this.CalendarGrid.Children.Add(header);
                    header.Width = 0;

                    for (int count = 0; count < DaysInMonth; count++)
                    {


                        if (FirstOfMonth.DayOfWeek == DayOfWeek.Saturday)
                        {
                            DateColumn dc = new DateColumn(StartOfWeek, FirstOfMonth);
                            TimeSpan NumberOfDaysInColumn = FirstOfMonth - StartOfWeek;
                            if (dc.DayText.ActualWidth + 10 < NumberOfDaysInColumn.Days * 12)
                            {
                                dc.Width = NumberOfDaysInColumn.Days * 12;
                            }
                            else
                            {
                                dc.Width = dc.DayText.ActualWidth + 10;
                            }
                            dc.Margin = new Thickness(offset, 0, 0, 0);
                            StartOfWeek = FirstOfMonth.AddDays(1);
                            offset += dc.Width;
                            header.Width += dc.Width;
                            CalendarGrid.Width += dc.Width;
                            this.CalendarGrid.Children.Add(dc);

                        }
                        else if (FirstOfMonth.Day == DaysInMonth)
                        {
                            DateColumn dc = new DateColumn(StartOfWeek, FirstOfMonth);
                            TimeSpan NumberOfDaysInColumn = FirstOfMonth - StartOfWeek;
                            if (dc.DayText.ActualWidth + 10 < NumberOfDaysInColumn.Days * 12)
                            {
                                dc.Width = NumberOfDaysInColumn.Days * 12;
                            }
                            else
                            {
                                dc.Width = dc.DayText.ActualWidth + 10;
                            }
                            dc.Margin = new Thickness(offset, 0, 0, 0);
                            this.CalendarGrid.Children.Add(dc);
                            offset += dc.Width;
                            header.Width += dc.Width;
                            CalendarGrid.Width += dc.Width;
                            if (QuarterMonth == 3)
                            {
                                header.MakeLastDayOfMonth();
                                dc.MakeLastDayOfMonth();
                            }
                        }

                        FirstOfMonth = FirstOfMonth.AddDays(1);
                    }


                }
            }
            UpdateCalendar();
        }
        public void UpdateCalendar()
        {
            if (CurrentView.Value == "Monthly") UpdateMonthlyView();
            else if (CurrentView.Value == "Quarterly") UpdateQuarterlyView();
        }
        public void UpdateSelectedStates()
        {
            foreach (DateColumn date in OverViewCalendar.CalendarObject.CalendarGrid.Children)
            {
                if (((date.ThisStartDate > SelectedDate2 && date.ThisStartDate < SelectedDate1)
                     ||
                    (date.ThisStartDate < SelectedDate2 && date.ThisStartDate > SelectedDate1))
                     &&
                    numSelectedDates == 2)
                {
                    date.IsSelected = false;
                    VisualStateManager.GoToState(date, "Between", true);
                }
                else if ((date.ThisStartDate == SelectedDate1 && numSelectedDates >= 1) || (date.ThisStartDate == SelectedDate2 && numSelectedDates == 2))
                {
                    VisualStateManager.GoToState(date, "MouseOver", true);
                }
                else
                {
                    date.IsSelected = false;
                    VisualStateManager.GoToState(date, "Normal", true);
                }
            }
            UpdateCalendar();
        }
        public void SelectDate(DateColumn date)
        {
            LeftMouseIsDown = true;
            date.IsSelected = !date.IsSelected;
            if (date.IsSelected)
            {
                if (numSelectedDates == 0)
                {
                    SelectedDate1 = date.ThisStartDate;
                    numSelectedDates++;
                }
                else if (numSelectedDates == 1)
                {
                    if (date.ThisStartDate < SelectedDate1)
                    {
                        SelectedDate2 = SelectedDate1;
                        SelectedDate1 = date.ThisStartDate;
                    }
                    else
                    {
                        SelectedDate2 = date.ThisStartDate;
                    }
                    numSelectedDates = 2;

                }
                else if (numSelectedDates == 2)
                {
                    if (date.ThisStartDate < SelectedDate1)
                    {
                        SelectedDate1 = date.ThisStartDate;
                    }
                    else
                    {
                        SelectedDate2 = date.ThisStartDate;
                    }
                }

            }
            else if (!date.IsSelected)
            {
                if (numSelectedDates > 0)
                {
                    numSelectedDates--;
                    VisualStateManager.GoToState(this, "Normal", true);
                    if (date.ThisStartDate == SelectedDate1)
                    {
                        SelectedDate1 = SelectedDate2;
                    }
                }
            }
            UpdateSelectedStates();
        }
        private void UpdateMonthlyView()
        {
            int EventsAddedToCalendar = 0;
            int TopOffset = 40;


            //clear calendar
            EventsGrid.Children.Clear();
            EventsGrid.Width = 0;
            EventsGrid.HorizontalAlignment = HorizontalAlignment.Left;
            foreach (XElement record in XMLCache.Descendants(z + "row"))
            {
                Event e = new Event((string)record.Attribute("ows_LinkTitle"),
                    (string)record.Attribute("ows_EventDate"),
                    (string)record.Attribute("ows_EndDate"),
                    (string)record.Attribute("ows_ID"),
                    (bool)record.Attribute("ows_Exec_x0020_Comms_x0020_Support"),
                    (bool)record.Attribute("ows_Event_x0020_Support"),
                    (bool)record.Attribute("ows_Demo_x0020_Support"));
                //Check to make sure this event should be displayed this month.
                if (e.StartDate.Month == CurrentSetting.Month || e.EndDate.Month == CurrentSetting.Month)
                {   //the 3 teams who are involved with events each have a tag on the event. 
                    //If all the tags on this event are being filtered, then it should not be displayed.
                    //If even one tag is not being filtered, then it will be shown.
                    //If the event has no tags, it will be shown regardless of filtering.
                    if ((e.EventsIcon.Visibility == Visibility.Visible && !Filters["EVENTS TEAM"]) ||
                        (e.ExecComIcon.Visibility == Visibility.Visible && !Filters["EXEC COMM. TEAM"]) ||
                        (e.DemoIcon.Visibility == Visibility.Visible && !Filters["DEMO TEAM"]) ||
                        (e.DemoIcon.Visibility == Visibility.Collapsed && e.ExecComIcon.Visibility == Visibility.Collapsed && e.EventsIcon.Visibility == Visibility.Collapsed))
                    {
                        //If events are being filtered by date interval selected by clicking on date columns, 
                        //only display events which overlap the currently selected interval
                        if ((numSelectedDates == 0) ||
                            (((e.StartDate <= SelectedDate1 && e.EndDate >= SelectedDate1) && numSelectedDates >= 1) ||
                                ((e.StartDate <= SelectedDate2 && e.EndDate >= SelectedDate2) && numSelectedDates == 2) ||
                                ((e.StartDate <= SelectedDate2 && e.StartDate >= SelectedDate1) && numSelectedDates == 2) ||
                                ((e.EndDate <= SelectedDate2 && e.EndDate >= SelectedDate1) && numSelectedDates == 2))
                            )
                        {
                            double StartMargin = 0, EndMargin = 0;
                            //Find the correct position and dimensions of the event
                            foreach (DateColumn dc in CalendarGrid.Children)
                            {   //if this event started in a previous month, set the margin to the first of the month.
                                if (dc.ThisStartDate > e.StartDate && dc.ThisStartDate.Day == 1)
                                {
                                    StartMargin = dc.Margin.Left + ActualMargin;
                                }

                                //if we've found the start date's column, set the margin.
                                if (dc.ThisStartDate == e.StartDate)
                                {
                                    StartMargin = dc.Margin.Left + ActualMargin;

                                    //if the day after the end date of the event is not in the current month, extend the date bar to the end of the view
                                    //and add it to the eventsgrid...
                                    if (e.EndDate.AddDays(1).Month != CurrentSetting.Month)
                                    {
                                        EndMargin = LayoutRoot.Width - ActualMargin;
                                        e.Margin = new Thickness(StartMargin, TopOffset, 0, 0 - (EventsAddedToCalendar * e.Height * 2));
                                        e.SetValue(Canvas.TopProperty, (EventsAddedToCalendar * e.Height));
                                        e.DateBarBG.Width = EndMargin - StartMargin;

                                        if (StartMargin + e.EventTitleStack.Width > LayoutRoot.Width)
                                        {
                                            e.EventsTitleBorder.Margin = new Thickness(e.DateBarBG.Width - e.EventTitleStack.Width, 0, 0, 0);
                                        }
                                        EventsGrid.Children.Add(e);
                                        EventsAddedToCalendar++;
                                        break;
                                    }

                                }
                                //...otherwise if we can find the day after the end of the month, set the right margin
                                // setting the negative bottom margin to avoid expanding the Eventsgrid over the Datecolumns.
                                else if (dc.ThisStartDate == e.EndDate.AddDays(1))
                                {
                                    EndMargin = dc.Margin.Left + ActualMargin;
                                    e.Margin = new Thickness(StartMargin, TopOffset, 0, 0 - (EventsAddedToCalendar * e.Height * 2));
                                    e.DateBarBG.Width = EndMargin - StartMargin;
                                    e.SetValue(Canvas.TopProperty, (EventsAddedToCalendar * e.Height));
                                    if (StartMargin + e.EventTitleStack.Width > LayoutRoot.Width)
                                    {
                                        e.EventsTitleBorder.Margin = new Thickness(e.DateBarBG.Width - e.EventTitleStack.Width, 0, 0, 0);
                                    }
                                    EventsGrid.Children.Add(e);
                                    EventsAddedToCalendar++;
                                    break;
                                }
                            }

                        }
                    }

                }
            }





        }
        private void UpdateQuarterlyView()
        {

            int EventsAddedToCalendar = 0;
            int TopOffset = 40;
            XNamespace z = "#RowsetSchema";

            //clear calendar
            EventsGrid.Children.Clear();
            EventsGrid.Width = CalendarGrid.Width;
            EventsGrid.HorizontalAlignment = HorizontalAlignment.Center;
            foreach (DateColumn column in CalendarGrid.Children)
            {

                if (!column.IsHeaderColumn)
                {
                    List<XElement> RecordsForTheFiscalWeek = new List<XElement>();
                    foreach (XElement record in XMLCache.Descendants(z + "row"))
                    {
                        if (((bool)record.Attribute("ows_Event_x0020_Support") && !Filters["EVENTS TEAM"]) ||
                        ((bool)record.Attribute("ows_Exec_x0020_Comms_x0020_Support") && !Filters["EXEC COMM. TEAM"]) ||
                        ((bool)record.Attribute("ows_Demo_x0020_Support") && !Filters["DEMO TEAM"]) ||
                        (!(bool)record.Attribute("ows_Demo_x0020_Support") && !(bool)record.Attribute("ows_Exec_x0020_Comms_x0020_Support") && !(bool)record.Attribute("ows_Event_x0020_Support")))
                        {
                            string EventDate, EventEndDate;
                            EventDate = (string)record.Attribute("ows_EventDate");
                            EventEndDate = (string)record.Attribute("ows_EndDate");
                            DateTime EventStart = new DateTime(Convert.ToInt32(EventDate.Substring(0, 4)),
                                            Convert.ToInt32(EventDate.Substring(5, 2)),
                                            Convert.ToInt32(EventDate.Substring(8, 2)));
                            DateTime EventEnd = new DateTime(Convert.ToInt32(EventEndDate.Substring(0, 4)),
                                            Convert.ToInt32(EventEndDate.Substring(5, 2)),
                                            Convert.ToInt32(EventEndDate.Substring(8, 2)));

                            if ((column.ThisStartDate <= EventEnd && column.ThisEndDate >= EventEnd) || (column.ThisEndDate >= EventStart && column.ThisStartDate <= EventStart))
                            {
                                RecordsForTheFiscalWeek.Add(record);
                            }
                        }

                    }
                    if (RecordsForTheFiscalWeek.Count > 0)
                    {
                        Event e = new Event(RecordsForTheFiscalWeek, z, column.ThisStartDate, column.ThisEndDate);
                        double StartMargin = 0;

                        StartMargin = column.Margin.Left;
                        e.Margin = new Thickness(StartMargin, TopOffset, 0, 0);

                        e.DateBarBG.Width = column.Width;
                        e.SetValue(Canvas.TopProperty, ((EventsAddedToCalendar % 4) * e.Height));

                        EventsGrid.Children.Add(e);
                        EventsAddedToCalendar++;
                    }
                }
            }

        }
        public bool IsHoliday(DateTime Date)
        {

            if (Date.Month == 1)
            {
                if (Date.Day == 1) { return true; }
                if (Date.Day == 2 && Date.DayOfWeek == DayOfWeek.Monday) { return true; }
                if (Date.Day > 14 && Date.Day < 22 && Date.DayOfWeek == DayOfWeek.Monday) { return true; }
            }
            if (Date.Month == 2)
            {
                if (Date.Day > 14 && Date.Day < 22 && Date.DayOfWeek == DayOfWeek.Monday) { return true; }
            }
            if (Date.Month == 5)
            {
                if (Date.Day > 24 && Date.DayOfWeek == DayOfWeek.Monday) { return true; }
            }
            if (Date.Month == 7)
            {
                if (Date.Day == 4) { return true; }
                if (Date.Day == 3 && Date.DayOfWeek == DayOfWeek.Friday) { return true; }
                if (Date.Day == 5 && Date.DayOfWeek == DayOfWeek.Monday) { return true; }
            }
            if (Date.Month == 9)
            {
                if (Date.Day < 8 && Date.DayOfWeek == DayOfWeek.Monday) { return true; }
            }
            if (Date.Month == 10)
            {
                if (Date.Day > 7 && Date.Day < 15 && Date.DayOfWeek == DayOfWeek.Monday) { return true; }
            }
            if (Date.Month == 11)
            {
                if (Date.Day == 11) { return true; }
                if (Date.Day == 10 && Date.DayOfWeek == DayOfWeek.Friday) { return true; }
                if (Date.Day == 12 && Date.DayOfWeek == DayOfWeek.Monday) { return true; }
                if (Date.Day > 21 && Date.Day < 29 && Date.DayOfWeek == DayOfWeek.Thursday) { return true; }
                if (Date.Day > 22 && Date.Day < 30 && Date.DayOfWeek == DayOfWeek.Friday) { return true; }
            }
            if (Date.Month == 12)
            {
                if (Date.Day == 25) { return true; }
                if (Date.Day == 24 && Date.DayOfWeek == DayOfWeek.Friday) { return true; }
                if (Date.Day == 26 && Date.DayOfWeek == DayOfWeek.Monday) { return true; }
                if (Date.Day == 31 && Date.DayOfWeek == DayOfWeek.Friday) { return true; }
            }

            return false;


        }
        public string IntToMonthString(int month)
        {
            if (month % 12 == 1) return "January";
            else if (month % 12 == 2) return "February";
            else if (month % 12 == 3) return "March";
            else if (month % 12 == 4) return "April";
            else if (month % 12 == 5) return "May";
            else if (month % 12 == 6) return "June";
            else if (month % 12 == 7) return "July";
            else if (month % 12 == 8) return "August";
            else if (month % 12 == 9) return "September";
            else if (month % 12 == 10) return "October";
            else if (month % 12 == 11) return "November";
            else if (month % 12 == 0) return "December";
            else return "value out of range";
        }
        public int StringToMonthInt(string month)
        {
            if (month == "January") return 1;
            else if (month == "February") return 2;
            else if (month == "March") return 3;
            else if (month == "April") return 4;
            else if (month == "May") return 5;
            else if (month == "June") return 6;
            else if (month == "July") return 7;
            else if (month == "August") return 8;
            else if (month == "September") return 9;
            else if (month == "October") return 10;
            else if (month == "November") return 11;
            else if (month == "December") return 12;
            else return 1;
        }
        public int IntMonthToFiscalQuarterInt(int month)
        {
            if (7 <= month && month <= 9) return 1;
            if (10 <= month && month <= 12) return 2;
            if (1 <= month && month <= 3) return 3;
            if (4 <= month && month <= 6) return 4;
            else return 0;
        }

        public void DisableCalendarColumns()
        {
            foreach (DateColumn dc in this.CalendarGrid.Children)
            {
                dc.Disable();

            }
        }


        public void ShowDetailsPane(string ID, MouseButtonEventArgs e)
        {
            XNamespace z = "#RowsetSchema";
            Details.InitializeFields();

            foreach (XElement record in XMLCache.Descendants(z + "row"))
            {
                if ((string)record.Attribute("ows_ID") == ID)
                {
                    Details.Title.Text = (string)record.Attribute("ows_LinkTitle");

                    string StartDate = (string)record.Attribute("ows_EventDate");
                    string EndDate = (string)record.Attribute("ows_EndDate");

                    if ((StartDate != "" && StartDate != null) && (EndDate != "" && EndDate != null))
                    {
                        if (StartDate.Substring(0, 10) == EndDate.Substring(0, 10))
                        {
                            Details.DateText.Text = OverViewCalendar.CalendarObject.IntToMonthString(Int32.Parse(StartDate.Substring(5, 2))).Substring(0, 3).ToUpper() + " " + Int32.Parse(StartDate.Substring(8, 2)).ToString();
                        }
                        else
                        {
                            Details.DateText.Text = OverViewCalendar.CalendarObject.IntToMonthString(Int32.Parse(StartDate.Substring(5, 2))).Substring(0, 3).ToUpper() + " " + Int32.Parse(StartDate.Substring(8, 2)).ToString() + " - " + OverViewCalendar.CalendarObject.IntToMonthString(Int32.Parse(EndDate.Substring(5, 2))).Substring(0, 3).ToUpper() + " " + Int32.Parse(EndDate.Substring(8, 2)).ToString();
                        }
                    }

                    string DescriptionString = (string)record.Attribute("ows_Description0");
                    if (DescriptionString != null)
                    {
                        if (DescriptionString.Length > 180)
                        {
                            Details.Description.Text = DescriptionString.Substring(0, 180) + "...";
                        }
                        else
                        {
                            Details.Description.Text = DescriptionString;
                        }
                    }
                    else Details.Description.Visibility = Visibility.Collapsed;

                    if (!(bool)record.Attribute("ows_Event_x0020_Support")) { Details.EventsIcon.Visibility = Visibility.Collapsed; }
                    if (!(bool)record.Attribute("ows_Demo_x0020_Support")) { Details.DemoIcon.Visibility = Visibility.Collapsed; }
                    if (!(bool)record.Attribute("ows_Exec_x0020_Comms_x0020_Support")) { Details.ExecComIcon.Visibility = Visibility.Collapsed; }

                    Details.LocationText.Text = (string)record.Attribute("ows_Location");
                    if (Details.LocationText.Text == "" || Details.LocationText.Text == null) Details.LocationStack.Visibility = Visibility.Collapsed;

                    Details.ExecCommLeadText.Text = (string)record.Attribute("ows_Exec_x0020_Comms_x0020_Lead");
                    if (Details.ExecCommLeadText.Text == "" || Details.ExecCommLeadText.Text == null) Details.ExecCommLeadStack.Visibility = Visibility.Collapsed;
                    else Details.ExecCommLeadText.Text = Details.ExecCommLeadText.Text.Replace(";#", " ");
                    if (Details.ExecCommLeadText.Text.StartsWith(" ")) Details.ExecCommLeadText.Text = Details.ExecCommLeadText.Text.Substring(1);

                    Details.DemoLeadText.Text = (string)record.Attribute("ows_Demo_x0020_Team_x0020_Lead");
                    if (Details.DemoLeadText.Text == "" || Details.DemoLeadText.Text == null) Details.DemoLeadStack.Visibility = Visibility.Collapsed;
                    else Details.DemoLeadText.Text = Details.DemoLeadText.Text.Replace(";#", " ");
                    if (Details.DemoLeadText.Text.StartsWith(" ")) Details.DemoLeadText.Text = Details.DemoLeadText.Text.Substring(1);

                    Details.EventTeamLeadText.Text = (string)record.Attribute("ows_Event_x0020_Team_x0020_Lead");
                    if (Details.EventTeamLeadText.Text == "" || Details.EventTeamLeadText.Text == null) Details.EventTeamLeadStack.Visibility = Visibility.Collapsed;
                    else Details.EventTeamLeadText.Text = Details.EventTeamLeadText.Text.Replace(";#", " ");
                    if (Details.EventTeamLeadText.Text.StartsWith(" ")) Details.EventTeamLeadText.Text = Details.EventTeamLeadText.Text.Substring(1);

                    Details.CategoryText.Text = (string)record.Attribute("ows_Event_x0020_Category");
                    if (Details.CategoryText.Text == "" || Details.CategoryText.Text == null) Details.CategoryStack.Visibility = Visibility.Collapsed;
                    else Details.CategoryText.Text = Details.CategoryText.Text.Replace(";#", ", ");
                    if (Details.CategoryText.Text.StartsWith(", ")) Details.CategoryText.Text = Details.CategoryText.Text.Substring(2);
                    if (Details.CategoryText.Text.EndsWith(", ")) Details.CategoryText.Text = Details.CategoryText.Text.Substring(0, Details.CategoryText.Text.LastIndexOf(","));

                    string WorkSpaceLink = (string)record.Attribute("ows_Workspace_x0020_URL");
                    if (WorkSpaceLink != "" && WorkSpaceLink != null)
                    {
                        Details.EventsWorkspaceTitle.Tag = WorkSpaceLink.Substring(0, WorkSpaceLink.IndexOf(","));
                        Details.EventsWorkspaceTitle.MouseLeftButtonUp += new MouseButtonEventHandler(EventsWorkspaceTitle_MouseLeftButtonUp);
                        Details.EventsWorkspaceTitle.Visibility = Visibility.Visible;

                    }
                    break;

                }
            }
            Details.OuterStack.UpdateLayout();
            Details.Monthly.Visibility = Visibility.Visible;
            Details.Quarterly.Visibility = Visibility.Collapsed;
            Details.MonthlyShadow.Visibility = Visibility.Visible;
            Details.QuarterlyShadow.Visibility = Visibility.Collapsed;
            Details.Visibility = Visibility.Visible;
            LayoutRoot.UpdateLayout();
            Point p = e.GetPosition(OverViewCalendar.CalendarObject.LayoutRoot);
            double xValue = p.X, yValue = p.Y;
            if (xValue + Details.Monthly.Width > LayoutRoot.Width)
            {
                xValue = LayoutRoot.Width - Details.Monthly.Width;
            }
            if (xValue < 0) xValue = 0;
            if (yValue + Details.OuterStack.ActualHeight + 35 > LayoutRoot.ActualHeight)
            {
                yValue = LayoutRoot.ActualHeight - Details.OuterStack.ActualHeight - 35;
            }
            if (yValue < 0) yValue = 0;
            Details.Monthly.Margin = new Thickness(xValue, yValue, 0, 0);
            if (Details.OuterStack.ActualHeight + 35 > LayoutRoot.ActualHeight)
            {
                // System.Windows.Browser.HtmlPage.Window.Eval("document.getElementById('silverlightControlHost').style.height='" + (Details.OuterStack.ActualHeight + 35).ToString() + "';");
            }
            Details.LayoutRoot.MouseLeftButtonUp += new MouseButtonEventHandler(Details.DetailsPane_MouseLeftButtonUp);


        }

        void EventsWorkspaceTitle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Details.LayoutRoot.MouseLeftButtonUp -= Details.DetailsPane_MouseLeftButtonUp;
            TextBlock Workspacelink = sender as TextBlock;

            //System.Windows.Browser.HtmlPage.Window.Eval("window.location='" + Workspacelink.Tag.ToString() + "';");
        }
        public void ShowDetailsPane(Event SendingEvent, MouseButtonEventArgs e)
        {
            SolidColorBrush GreenLineBrush = new SolidColorBrush(Color.FromArgb(255, 157, 190, 130));
            SolidColorBrush BlueLinkBrush = new SolidColorBrush(Color.FromArgb(255, 68, 144, 160));
            FontFamily SegoeSemiBold = new FontFamily("SegoeSB.ttf#Segoe Semibold");
            FontFamily SegoeUI = new FontFamily("SEGOEUI.TTF#Segoe UI");
            FontFamily SegoeCondensedBold = new FontFamily("segoeb.ttf#Segoe Codensed");
            Details.QuarterlyOuterStack.Children.Clear();
            Details.QuarterlyOuterStack.UpdateLayout();

            foreach (XElement record in SendingEvent.EventsRecords.DescendantsAndSelf())
            {
                TextBlock EventTitleTruncated = new TextBlock();
                string TempStringToCatchRecordValuesWithoutTheirColumnTitleAttached = (string)record.Attribute("ows_LinkTitle");
                if (TempStringToCatchRecordValuesWithoutTheirColumnTitleAttached.Length > 20)
                {
                    EventTitleTruncated.Text = TempStringToCatchRecordValuesWithoutTheirColumnTitleAttached.Substring(0, 20) + "...";
                }
                else EventTitleTruncated.Text = TempStringToCatchRecordValuesWithoutTheirColumnTitleAttached;
                EventTitleTruncated.FontFamily = SegoeCondensedBold;
                EventTitleTruncated.FontWeight = FontWeights.Bold;
                EventTitleTruncated.FontSize = 12;

                this.Details.QuarterlyOuterStack.Children.Add(EventTitleTruncated);

                StackPanel DateAndLinkStack = new StackPanel();
                DateAndLinkStack.Orientation = Orientation.Horizontal;
                TextBlock EventDate = new TextBlock();
                EventDate.FontFamily = SegoeSemiBold;

                EventDate.FontSize = 11;
                TempStringToCatchRecordValuesWithoutTheirColumnTitleAttached = (string)record.Attribute("ows_EventDate");
                string SecondTempStringForComparison = (string)record.Attribute("ows_EndDate");
                if (TempStringToCatchRecordValuesWithoutTheirColumnTitleAttached.Substring(0, 10) == SecondTempStringForComparison.Substring(0, 10))
                {
                    EventDate.Text = TempStringToCatchRecordValuesWithoutTheirColumnTitleAttached.Substring(5, 2) + "/" + TempStringToCatchRecordValuesWithoutTheirColumnTitleAttached.Substring(8, 2);

                }
                else
                {

                    EventDate.Text = TempStringToCatchRecordValuesWithoutTheirColumnTitleAttached.Substring(5, 2) + "/" + TempStringToCatchRecordValuesWithoutTheirColumnTitleAttached.Substring(8, 2);
                    EventDate.Text += " - " + SecondTempStringForComparison.Substring(5, 2) + "/" + SecondTempStringForComparison.Substring(8, 2);

                }
                DateAndLinkStack.Children.Add(EventDate);

                TextBlock EventLink = new TextBlock();
                string WorkSpaceLink = (string)record.Attribute("ows_Workflow_x0020_Url");
                if (WorkSpaceLink != "" && WorkSpaceLink != null)
                {
                    EventLink.Tag = WorkSpaceLink.Substring(0, WorkSpaceLink.IndexOf(","));
                    EventLink.MouseLeftButtonUp += new MouseButtonEventHandler(EventsWorkspaceTitle_MouseLeftButtonUp);
                    EventLink.Name = "ParentRecordID_" + (string)record.Attribute("ows_ID");
                    EventLink.Text = "Go To Event >>";
                    EventLink.Foreground = BlueLinkBrush;
                    EventLink.FontFamily = SegoeUI;
                    EventLink.FontSize = 11;
                    EventLink.Margin = new Thickness(4, 0, 0, 0);
                    DateAndLinkStack.Children.Add(EventLink);
                }
                Details.QuarterlyOuterStack.Children.Add(DateAndLinkStack);

                Rectangle Line = new Rectangle();
                Line.Fill = GreenLineBrush;
                Line.Height = 1;
                Line.Width = Details.Width - 30;
                Line.Margin = new Thickness(0, 8, 0, 8);
                Details.QuarterlyOuterStack.Children.Add(Line);

            }
            Details.QuarterlyOuterStack.UpdateLayout();
            Details.QuarterlyShadow.Visibility = Visibility.Visible;
            Details.MonthlyShadow.Visibility = Visibility.Collapsed;
            Details.Visibility = Visibility.Visible;
            Details.Monthly.Visibility = Visibility.Collapsed;
            Details.Quarterly.Visibility = Visibility.Visible;
            LayoutRoot.UpdateLayout();
            Point p = e.GetPosition(OverViewCalendar.CalendarObject.LayoutRoot);
            double xValue = p.X, yValue = p.Y;
            if (xValue + Details.Quarterly.Width > LayoutRoot.Width)
            {
                xValue = LayoutRoot.Width - Details.Quarterly.Width;
            }
            if (xValue < 0) xValue = 0;

            if (yValue + Details.QuarterlyOuterStack.ActualHeight + 30 > LayoutRoot.ActualHeight)
            {
                yValue = LayoutRoot.ActualHeight - Details.QuarterlyOuterStack.ActualHeight - 30;
            }
            if (yValue < 0) yValue = 0;
            Details.Quarterly.Margin = new Thickness(xValue, yValue, 0, 0);

            if (Details.QuarterlyOuterStack.ActualHeight + 30 > LayoutRoot.ActualHeight)
            {
                System.Windows.Browser.HtmlPage.Window.Eval("document.getElementById('silverlightControlHost').style.height='" + (Details.QuarterlyOuterStack.ActualHeight + 30).ToString() + "';");
            }
            Details.LayoutRoot.MouseLeftButtonUp += new MouseButtonEventHandler(Details.DetailsPane_MouseLeftButtonUp);
        }
        // Defunct, used to navigate to the quarterly view and pop the details pane, but removed by request and replaced
        // with link to workspace, if available.
        void EventLink_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Details.LayoutRoot.MouseLeftButtonUp -= Details.DetailsPane_MouseLeftButtonUp;
            TextBlock Block = sender as TextBlock;

            foreach (XElement record in XMLCache.Descendants(z + "row"))
            {
                if (Block.Name.ToString().Substring(15) == (string)record.Attribute("ows_ID"))
                {
                    string DateString = (string)record.Attribute("ows_EventDate");
                    CurrentSetting = new DateTime(Convert.ToInt32(DateString.Substring(0, 4)),
                                    Convert.ToInt32(DateString.Substring(5, 2)),
                                    Convert.ToInt32(DateString.Substring(8, 2)));
                    CurrentView.Value = "Monthly";
                    ShowDetailsPane(Block.Name.ToString().Substring(15), e);
                    break;
                }
            }
        }

    }
}

