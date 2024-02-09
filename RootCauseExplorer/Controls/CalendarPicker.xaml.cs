using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace RootCauseExplorer
{
    public partial class CalendarPicker : UserControl
    {
        private IDictionary<string, string> Parameters = new Dictionary<string, string>();
        private XDocument XMLCacheCR1, XMLCacheCR2, XMLCacheCR3, XMLCacheCR4, XMLCacheCR5, XMLCacheCR6, XMLCacheCR7, XMLCacheCR8;
        private int Month = DateTime.Now.Month;
        private int Year = DateTime.Now.Year;
        private bool warnedUserOfConflict = false;
        public string SelectedClassroom = null;
        public int numSelectedDates = 0;
        private bool userIsPrivileged = true;
        public DateTime FirstDate = DateTime.MinValue;
        public DateTime SecondDate = DateTime.MinValue;
        private int LoadedSchedules = 0;

        public CalendarPicker(IDictionary<string, string> InitParams)
        {
            InitializeComponent();
            if (!DesignerProperties.IsInDesignTool)
            {
                Parameters.Add("ListID", "%7B635EE722%2DA081%2D4D56%2DACCE%2D8501696D1C4D%7D");
                Parameters.Add("ViewID", "%7BA16BABAE%2D9916%2D4AC3%2D8BA4%2D6371F4C82F4C%7D");
                Parameters.Add("ListLocation", "http://sharepoint/sites/classroom");
                Parameters.Add("UserListLocation", "http://sharepoint/sites/classroom");
                Parameters.Add("UserIDsListID", "%7BE153F8B4%2D70F3%2D4E44-8703%2D3CD4C248A0DC%7D");
                Parameters.Add("MembershipGroupId", "533");
                Parameters.Add("UserIDFilter", "&MembershipGroupId=711");
                Parameters.Add("CR1Filter", "&FilterField1=Location&FilterValue1=Classroom1");
                Parameters.Add("CR2Filter", "&FilterField1=Location&FilterValue1=Classroom2");
                Parameters.Add("CR3Filter", "&FilterField1=Location&FilterValue1=Classroom3");
                Parameters.Add("CR4Filter", "&FilterField1=Location&FilterValue1=Classroom4");
                Parameters.Add("CR5Filter", "&FilterField1=Location&FilterValue1=Classroom5");
                Parameters.Add("CR6Filter", "&FilterField1=Location&FilterValue1=Classroom6");
                Parameters.Add("CR7Filter", "&FilterField1=Location&FilterValue1=Classroom7");
                Parameters.Add("CR8Filter", "&FilterField1=Location&FilterValue1=Classroom8");


                if (InitParams.ContainsKey("userNumber"))
                {
                    //System.Windows.Browser.HtmlPage.Window.Alert(InitParams["userNumber"]);
                    Parameters["MembershipGroupId"] = InitParams["userNumber"];

                }
                else
                {
                    //System.Windows.Browser.HtmlPage.Window.Alert("no");

                }
                string NoViewNeeded = null;
                CallOWSSVRToReturnGalleryXML(Parameters["UserIDsListID"], Parameters["UserListLocation"], Parameters["UserIDFilter"], NoViewNeeded, 0);

                Today.Text = DateTime.Today.ToString().Substring(0, DateTime.Today.ToString().IndexOf(" "));

                CallOWSSVRToReturnGalleryXML(Parameters["ListID"], Parameters["ListLocation"], Parameters["CR1Filter"], Parameters["ViewID"], 1);
                CallOWSSVRToReturnGalleryXML(Parameters["ListID"], Parameters["ListLocation"], Parameters["CR2Filter"], Parameters["ViewID"], 2);
                CallOWSSVRToReturnGalleryXML(Parameters["ListID"], Parameters["ListLocation"], Parameters["CR3Filter"], Parameters["ViewID"], 3);
                CallOWSSVRToReturnGalleryXML(Parameters["ListID"], Parameters["ListLocation"], Parameters["CR4Filter"], Parameters["ViewID"], 4);
                CallOWSSVRToReturnGalleryXML(Parameters["ListID"], Parameters["ListLocation"], Parameters["CR5Filter"], Parameters["ViewID"], 5);
                CallOWSSVRToReturnGalleryXML(Parameters["ListID"], Parameters["ListLocation"], Parameters["CR6Filter"], Parameters["ViewID"], 6);

                for (int i = 1; i < 8; i++)
                {
                    SPCalendarWeekControl newWeekColumn = new SPCalendarWeekControl();
                    switch (i)
                    {
                        case (1): newWeekColumn.DayColumnName.Text = "SUNDAY"; break;
                        case (2): newWeekColumn.DayColumnName.Text = "MONDAY"; break;
                        case (3): newWeekColumn.DayColumnName.Text = "TUESDAY"; break;
                        case (4): newWeekColumn.DayColumnName.Text = "WEDNESDAY"; break;
                        case (5): newWeekColumn.DayColumnName.Text = "THURSDAY"; break;
                        case (6): newWeekColumn.DayColumnName.Text = "FRIDAY"; break;
                        case (7): newWeekColumn.DayColumnName.Text = "SATURDAY"; break;

                    }
                    newWeekColumn.HorizontalAlignment = HorizontalAlignment.Left;
                    newWeekColumn.VerticalAlignment = VerticalAlignment.Top;
                    newWeekColumn.Margin = new Thickness((i * 103) + 76, 35, 0, 0);
                    newWeekColumn.SetValue(Canvas.ZIndexProperty, -10);
                    this.LayoutRoot.Children.Add(newWeekColumn);
                }
                for (int i = 0; i < 12; i++)
                {
                    if (i == 0) { DatePicker.Items.Clear(); }
                    if (i + Month <= 12)
                    {
                        ComboBoxItem newItem = new ComboBoxItem();
                        newItem.Content = IntToMonthString(i + Month) + " " + Year.ToString();
                        DatePicker.Items.Add(newItem);
                    }
                    else if (i + Month >= 12)
                    {
                        ComboBoxItem newItem = new ComboBoxItem();
                        newItem.Content = IntToMonthString((i + Month) - 12) + " " + (Year + 1).ToString();
                        DatePicker.Items.Add(newItem);
                    }
                    if (i == 0) { DatePicker.SelectedIndex = 0; }

                }
                DatePicker.SelectionChanged += DatePicker_Select;
                Reset_button.MouseLeftButtonUp += new MouseButtonEventHandler(Reset_button_MouseLeftButtonUp);
                Confirm_button.MouseLeftButtonUp += new MouseButtonEventHandler(Confirm_button_MouseLeftButtonUp);
            }
        }

        void Confirm_button_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (warnedUserOfConflict) { System.Windows.Browser.HtmlPage.Window.Alert("There is a scheduling conflict between the dates you've selected for Classroom "+SelectedClassroom.Substring(9,1)+". Please select another classroom or date range."); }
            else if (FirstDate != DateTime.MinValue)
            {
                string ReservationWithQuerystring = "http://sharepoint/sites/classroom/Submit-Reservation.aspx?" +
                                                            "Start Time=" + StartDateTextblock.Text.ToString() +
                                                            "&End Time=" + EndDateTextblock.Text.ToString() +
                                                            "&Location=" + SelectedClassroom +
                                                            "&All Day Event=1"+
                                                            "&Source=http://sharepoint/sites/classroom/View-Reservation.aspx";
                System.Windows.Browser.HtmlPage.Window.Eval("window.location='" + ReservationWithQuerystring + "';");
            }
            else System.Windows.Browser.HtmlPage.Window.Alert("Please Select a date and classroom");
        }

        void Reset_button_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            numSelectedDates = 1;
            unsetDate(FirstDate.Year, FirstDate.Month, FirstDate.Day, "");
            numSelectedDates = 0;
            unsetDate(SecondDate.Year, SecondDate.Month, SecondDate.Day, "");
            MonthDisplay.Children.Clear();
            populateCalender();
            UpdateCalendar();
        }

        private void CallOWSSVRToReturnGalleryXML(string ListID, string ListLocation, string FilterValues, string ViewID, int CRNumber)
        {
            string url = null;
            if (ViewID != null) {
                url = ListLocation + "/_vti_bin/owssvr.dll?Cmd=Display&XMLDATA=TRUE&List=" + ListID + "&View=" + ViewID + FilterValues;
            }
            else
            {
                url = ListLocation + "/_vti_bin/owssvr.dll?Cmd=Display&XMLDATA=TRUE&List=" + ListID + FilterValues;
            }


            WebClient sp = new WebClient();
            switch (CRNumber) {
                case (1): sp.OpenReadCompleted += new OpenReadCompletedEventHandler(sp_OpenReadCompleted1); break;
                case (2): sp.OpenReadCompleted += new OpenReadCompletedEventHandler(sp_OpenReadCompleted2); break;
                case (3): sp.OpenReadCompleted += new OpenReadCompletedEventHandler(sp_OpenReadCompleted3); break;
                case (4): sp.OpenReadCompleted += new OpenReadCompletedEventHandler(sp_OpenReadCompleted4); break;
                case (5): sp.OpenReadCompleted += new OpenReadCompletedEventHandler(sp_OpenReadCompleted5); break;
                case (6): sp.OpenReadCompleted += new OpenReadCompletedEventHandler(sp_OpenReadCompleted6); break;
                case (7): sp.OpenReadCompleted += new OpenReadCompletedEventHandler(sp_OpenReadCompleted7); break;
                case (8): sp.OpenReadCompleted += new OpenReadCompletedEventHandler(sp_OpenReadCompleted8); break;
                case (0): sp.OpenReadCompleted += new OpenReadCompletedEventHandler(sp_OpenReadCompletedUserID); break;
            }

            //sp.OpenReadAsync(new Uri(url));
            //if (CRNumber == 0) { sp.OpenReadAsync(new Uri("tempFile3.xml", UriKind.RelativeOrAbsolute)); }
            //else 
            sp.OpenReadAsync(new Uri("PickerCalendar.xml", UriKind.RelativeOrAbsolute));


        }
        private void sp_OpenReadCompletedUserID(object sender, OpenReadCompletedEventArgs e) {
            XNamespace z = "#RowsetSchema";
            bool foundId = false;
            if (e.Error == null) {
                XDocument PrivilegedUserList = XDocument.Load(e.Result);
                foreach(XElement User in PrivilegedUserList.Descendants(z+"row")){
                    if (Parameters["MembershipGroupId"] == (string)User.Attribute("ows_ID")) {
                        foundId = true;
                        CallOWSSVRToReturnGalleryXML(Parameters["ListID"], Parameters["ListLocation"], Parameters["CR7Filter"], Parameters["ViewID"], 7);
                        CallOWSSVRToReturnGalleryXML(Parameters["ListID"], Parameters["ListLocation"], Parameters["CR8Filter"], Parameters["ViewID"], 8);

                    }
                }
                if (!foundId) userIsPrivileged = false;
                if (!userIsPrivileged && LoadedSchedules >= 6) populateCalender();
                
            } else { System.Windows.Browser.HtmlPage.Window.Alert(e.Error.ToString()); }
        }
        private void sp_OpenReadCompleted8(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                XMLCacheCR8 = XDocument.Load(e.Result);
                LoadedSchedules++;
                if (userIsPrivileged && LoadedSchedules >= 8) populateCalender();
                else if (!userIsPrivileged && LoadedSchedules >= 6) populateCalender();
            }
            else
            {
                System.Windows.Browser.HtmlPage.Window.Alert(e.Error.ToString());
            }
        }
        private void sp_OpenReadCompleted7(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                XMLCacheCR7 = XDocument.Load(e.Result);
                LoadedSchedules++;
                if (userIsPrivileged && LoadedSchedules >= 8) populateCalender();
                else if (!userIsPrivileged && LoadedSchedules >= 6) populateCalender();
            }
            else
            {
                System.Windows.Browser.HtmlPage.Window.Alert(e.Error.ToString());
            }
        }
        private void sp_OpenReadCompleted6(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                XMLCacheCR6 = XDocument.Load(e.Result);
                LoadedSchedules++;
                if (userIsPrivileged && LoadedSchedules >= 8) populateCalender();
                else if (!userIsPrivileged && LoadedSchedules >= 6) populateCalender();
            }
            else
            {
                System.Windows.Browser.HtmlPage.Window.Alert(e.Error.ToString());
            }
        }
        private void sp_OpenReadCompleted5(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                XMLCacheCR5 = XDocument.Load(e.Result);
                LoadedSchedules++;
                if (userIsPrivileged && LoadedSchedules >= 8) populateCalender();
                else if (!userIsPrivileged && LoadedSchedules >= 6) populateCalender();
            }
            else
            {
                System.Windows.Browser.HtmlPage.Window.Alert(e.Error.ToString());
            }
        }
        private void sp_OpenReadCompleted4(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                XMLCacheCR4 = XDocument.Load(e.Result);
                LoadedSchedules++;
                if (userIsPrivileged && LoadedSchedules >= 8) populateCalender();
                else if (!userIsPrivileged && LoadedSchedules >= 6) populateCalender();
            }
            else
            {
                System.Windows.Browser.HtmlPage.Window.Alert(e.Error.ToString());
            }
        }
        private void sp_OpenReadCompleted3(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                XMLCacheCR3 = XDocument.Load(e.Result);
                LoadedSchedules++;
                if (userIsPrivileged && LoadedSchedules >= 8) populateCalender();
                else if (!userIsPrivileged && LoadedSchedules >= 6) populateCalender();
            }
            else
            {
                System.Windows.Browser.HtmlPage.Window.Alert(e.Error.ToString());
            }
        }
        private void sp_OpenReadCompleted2(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                XMLCacheCR2 = XDocument.Load(e.Result);
                LoadedSchedules++;
                if (userIsPrivileged && LoadedSchedules >= 8) populateCalender();
                else if (!userIsPrivileged && LoadedSchedules >= 6) populateCalender();
            }
            else
            {
                System.Windows.Browser.HtmlPage.Window.Alert(e.Error.ToString());
            }
        }       
        private void sp_OpenReadCompleted1(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error == null)
            {   
                XMLCacheCR1 = XDocument.Load(e.Result);
                LoadedSchedules++;
                if (userIsPrivileged && LoadedSchedules >= 8) populateCalender();
                else if (!userIsPrivileged && LoadedSchedules >= 6) populateCalender();
            }
            else
            {
                System.Windows.Browser.HtmlPage.Window.Alert(e.Error.ToString());
            }
        }
        private void populateCalender() 
        {
            XNamespace z = "#RowsetSchema";
            int count = 0;
            DateTime FirstOfMonth = new DateTime(Year, Month, 1);
            
            // roll back to the beginning of the week
            while (FirstOfMonth.DayOfWeek != DayOfWeek.Sunday) { FirstOfMonth = FirstOfMonth.AddDays(-1); count--; }
            
            // start working through the tiles
            while (count < DateTime.DaysInMonth(Year, Month) || FirstOfMonth.DayOfWeek != DayOfWeek.Sunday) {

                SPCalenderDayControl cNewDay = new SPCalenderDayControl();

                cNewDay.HostPage = this;
                cNewDay.Month = FirstOfMonth.Month;
                cNewDay.Year = FirstOfMonth.Year;
                cNewDay.Date.Text = FirstOfMonth.Day.ToString();

                if (FirstOfMonth < DateTime.Today) {
                    cNewDay.Ghost();
                    cNewDay.Disabled = true;
                }
                if (IsHoliday(FirstOfMonth)){
                    cNewDay.makeHoliday();
                }
                if (FirstOfMonth == DateTime.Today) {

                    cNewDay.makeToday();
                }
                if (!userIsPrivileged) {
                    cNewDay.Classroom7.Visibility = Visibility.Collapsed;
                    cNewDay.Classroom8.Visibility = Visibility.Collapsed;
                }
                    //ghost rooms which are already booked
                if (FirstOfMonth >= DateTime.Today)
                {
                    CheckAgainstExistingReservations(XMLCacheCR1, cNewDay, FirstOfMonth, z);
                    CheckAgainstExistingReservations(XMLCacheCR2, cNewDay, FirstOfMonth, z);
                    CheckAgainstExistingReservations(XMLCacheCR3, cNewDay, FirstOfMonth, z);
                    CheckAgainstExistingReservations(XMLCacheCR4, cNewDay, FirstOfMonth, z);
                    CheckAgainstExistingReservations(XMLCacheCR5, cNewDay, FirstOfMonth, z);
                    CheckAgainstExistingReservations(XMLCacheCR6, cNewDay, FirstOfMonth, z);
                    if (userIsPrivileged)
                    {
                        CheckAgainstExistingReservations(XMLCacheCR7, cNewDay, FirstOfMonth, z);
                        CheckAgainstExistingReservations(XMLCacheCR8, cNewDay, FirstOfMonth, z);
                    }
                }
                    // if there is a logged first date or second date, select the tile.
                 
                
                if (FirstDate != DateTime.MinValue && FirstOfMonth == FirstDate){
                    numSelectedDates--;
                    cNewDay.SelectClassroom(Convert.ToInt32(SelectedClassroom.Substring(9, 1)));
                } if (SecondDate != DateTime.MinValue && FirstOfMonth == SecondDate){
                    numSelectedDates--;
                    cNewDay.SelectClassroom(Convert.ToInt32(SelectedClassroom.Substring(9, 1)));
                }

                MonthDisplay.Children.Add(cNewDay);
                count++;
                FirstOfMonth = FirstOfMonth.AddDays(1);
                
            }
                   
        }
        private void CheckAgainstExistingReservations(XDocument ClassroomReservations, SPCalenderDayControl Day, DateTime DateToBeChecked, XNamespace z) {
            
            foreach (XElement child in ClassroomReservations.Descendants(z + "row"))
            {

                if ((string)child.Attribute("ows_Status") != "Cancelled")
                {
                    string StartDate = (string)child.Attribute("ows_EventDate");
                    string EndDate = (string)child.Attribute("ows_EndDate");
                    string Classroom = (string)child.Attribute("ows_Location");
                    /*
                    string StartDate = (string)child.Attribute("ows_Full_x0020_Day_x0020_Start_x0020_Date");
                    string EndDate = (string)child.Attribute("ows_Full_x0020_Day_x0020_End_x0020_Date0");
                    string Classroom = (string)child.Attribute("ows_Title0");*/
                    StartDate = StartDate.Substring(0, StartDate.IndexOf(" "));
                    EndDate = EndDate.Substring(0, EndDate.IndexOf(" "));
                    //int ClassNumber = Convert.ToInt32(Classroom.Substring(Classroom.IndexOf("_") - 1, 1));
                    int ClassNumber = Convert.ToInt32(Classroom.Substring(9, 1));
                    DateTime Date1 = new DateTime(Convert.ToInt32(StartDate.Substring(0, 4)), Convert.ToInt32(StartDate.Substring(5, 2)), Convert.ToInt32(StartDate.Substring(8, 2)));
                    DateTime Date2 = new DateTime(Convert.ToInt32(EndDate.Substring(0, 4)), Convert.ToInt32(EndDate.Substring(5, 2)), Convert.ToInt32(EndDate.Substring(8, 2)));
                    if (DateToBeChecked.CompareTo(Date1) > 0 && DateToBeChecked.CompareTo(Date2) <= 0)
                    {
                        Day.bookRoom(ClassNumber);
                        Day.Ghost(ClassNumber);
                    }
                }
            }
            
        }
        private bool CheckAgainstExistingReservations(int ClassroomNumber, XNamespace z) {
            XDocument ThisClassroom = null;

            switch (ClassroomNumber) {
                case (1): ThisClassroom = XMLCacheCR1; break;
                case (2): ThisClassroom = XMLCacheCR2; break;
                case (3): ThisClassroom = XMLCacheCR3; break;
                case (4): ThisClassroom = XMLCacheCR4; break;
                case (5): ThisClassroom = XMLCacheCR5; break;
                case (6): ThisClassroom = XMLCacheCR6; break;
                case (7): ThisClassroom = XMLCacheCR7; break;
                case (8): ThisClassroom = XMLCacheCR7; break;
            }
            if (ThisClassroom != null)
            {
                foreach (XElement child in ThisClassroom.Descendants(z + "row"))
                {
                    if ((string)child.Attribute("ows_Status") != "Cancelled")
                    {
                        string Classroom = (string)child.Attribute("ows_Location");
                        int ClassNumber = Convert.ToInt32(Classroom.Substring(9, 1));

                        if (Convert.ToInt32(SelectedClassroom.Substring(9, 1)) == ClassNumber)
                        {
                            string StartDate = (string)child.Attribute("ows_EventDate");
                            string EndDate = (string)child.Attribute("ows_EndDate");
                            StartDate = StartDate.Substring(0, StartDate.IndexOf(" "));
                            EndDate = EndDate.Substring(0, EndDate.IndexOf(" "));
                            DateTime Date1 = new DateTime(Convert.ToInt32(StartDate.Substring(0, 4)), Convert.ToInt32(StartDate.Substring(5, 2)), Convert.ToInt32(StartDate.Substring(8, 2)));
                            DateTime Date2 = new DateTime(Convert.ToInt32(EndDate.Substring(0, 4)), Convert.ToInt32(EndDate.Substring(5, 2)), Convert.ToInt32(EndDate.Substring(8, 2)));

                            if ((Date1.CompareTo(FirstDate) >= 0 && Date1.CompareTo(SecondDate) <= 0) || (Date2.CompareTo(FirstDate) >= 0 && Date2.CompareTo(SecondDate) <= 0))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
        public void UpdateCalendar(){
            bool selecting = false;
            XNamespace z = "#RowsetSchema";

            if (numSelectedDates == 0) { DirectionsTextblock.Text = "Please select an available classroom to reserve it."; }
            else if (numSelectedDates == 1) { DirectionsTextblock.Text = "If you wish to reserve multiple days, please select another date. Otherwise, to reserve a classroom for a single day, click Confirm Request to continue the reservation process."; }
            else if (numSelectedDates == 2) { DirectionsTextblock.Text = "To change dates, click Reset, or click on the selected date and classroom on the calendar to deselect that day only. Otherwise, click Confirm Request to continue the reservation process."; }

            foreach (SPCalenderDayControl Day in MonthDisplay.Children)
            {
                if (!Day.Disabled)
                {
                    int CRNumber = 0;
                    if (SelectedClassroom != null) CRNumber = Convert.ToInt32(SelectedClassroom.Substring(9, 1));
                    // ghost room selection buttons for other classrooms if a classroom is selected
                    for (int i = 1; i <= 8; i++)
                    {
                        if (i == 1) Day.unGhost();
                        if (SelectedClassroom == null)
                        {
                            break;
                        }
                   
                        if (i != CRNumber || Day.BookedRooms.Contains(i.ToString()))
                        {
                            Day.Ghost(i);
                        }
                    }

                    // highlight the dates and classroom
                    if (!Day.SelectedDate)
                    {
                        Day.unHighlightTile();
                    }
                    else if (Day.SelectedDate)
                    {
                        Day.HighlightTile();
                    }

                    //if two dates are selected, highlight dates in between the two, to indicate they are part of the reservation.
                    //also, if a day is outside 2 selected dates, disable it.
                    if (numSelectedDates == 2)
                    {
                        DateTime thisDay = new DateTime(Day.Year, Day.Month, Convert.ToInt32(Day.Date.Text));
                        if (FirstDate < thisDay) selecting = true;
                        if (SecondDate < thisDay) selecting = false;

                        if (!Day.SelectedDate && selecting == true)
                        {
                            if (Day.BookedRooms.Contains(SelectedClassroom.Substring(9, 1)) && !warnedUserOfConflict)
                            {
                                System.Windows.Browser.HtmlPage.Window.Alert("There is a scheduling Conflict. Classroom " + SelectedClassroom.Substring(9, 1) + " is unavailable between the dates selected. Please book a different classroom or a different timeframe.");
                                Day.Tiletray.Background = new SolidColorBrush(Color.FromArgb(255, 255, 200, 0));
                                warnedUserOfConflict = true;
                            }
                            else if (Day.BookedRooms.Contains(SelectedClassroom.Substring(9, 1)) && warnedUserOfConflict)
                            {
                                Day.Tiletray.Background = new SolidColorBrush(Color.FromArgb(255, 255, 200, 0));

                            }
                            else
                            {
                                Day.HighlightTile(); // highlighting behavior goes here
                            }
                        }
                        if (!(thisDay <= SecondDate && thisDay >= FirstDate)) Day.Ghost("all classrooms");
                    }
                }
            }
            if (!warnedUserOfConflict && numSelectedDates == 2) {
                if (CheckAgainstExistingReservations(Convert.ToInt32(SelectedClassroom.Substring(9, 1)), z))
                {
                    warnedUserOfConflict = true;
                    System.Windows.Browser.HtmlPage.Window.Alert("There is a scheduling Conflict. Classroom " + SelectedClassroom.Substring(9, 1) + " is unavailable between the dates selected. Please book a different classroom or a different timeframe.");

                }
            }
        
        }
        public void setDate(int year, int month, int day, string classroom) {
            DateTime workingDate = new DateTime(year, month, day);
            if (workingDate != FirstDate && workingDate != SecondDate)
            {
                if (numSelectedDates == 1)
                {
                    FirstDate = workingDate;
                    SelectedClassroom = classroom;
                }
                else if (numSelectedDates == 2)
                {
                    SecondDate = workingDate;
                }
            }
            if (SecondDate < FirstDate && SecondDate != DateTime.MinValue) {
                workingDate = FirstDate;
                FirstDate = SecondDate;
                SecondDate = workingDate;
            }
            StartDateTextblock.Text = FirstDate.ToShortDateString();
            if (numSelectedDates == 2) EndDateTextblock.Text = SecondDate.ToShortDateString();
            else if (numSelectedDates == 1) EndDateTextblock.Text = FirstDate.ToShortDateString();

            if (SelectedClassroom != null) ClassroomTextblock.Text = SelectedClassroom.Substring(9, 1);

        }
        public void unsetDate(int year, int month, int day, string classroom) {
            warnedUserOfConflict = false;
            DateTime workingDate = new DateTime(year, month, day);
            if (numSelectedDates == 0) {
                FirstDate = DateTime.MinValue;
                SelectedClassroom = null;
            }
            else if (numSelectedDates == 1) {
                if (workingDate == FirstDate) {
                    FirstDate = SecondDate;
                    SecondDate = DateTime.MinValue;
                }
                else if (workingDate == SecondDate) {
                    SecondDate = DateTime.MinValue;
                }
            }
            if (numSelectedDates == 1) { EndDateTextblock.Text = FirstDate.ToShortDateString(); StartDateTextblock.Text = FirstDate.ToShortDateString(); }
            else if (numSelectedDates == 0) { StartDateTextblock.Text = " "; EndDateTextblock.Text = " "; }

            if (SelectedClassroom == null) ClassroomTextblock.Text = " ";
        }
        public bool IsHoliday(DateTime Date) {

            if (Date.Month == 1) {
                if (Date.Day == 1) { return true; }
                if (Date.Day == 2 && Date.DayOfWeek == DayOfWeek.Monday) { return true; }
                if (Date.Day > 14 && Date.Day < 22 && Date.DayOfWeek == DayOfWeek.Monday) { return true; }
            }
            if (Date.Month == 2) {
                if (Date.Day > 14 && Date.Day < 22 && Date.DayOfWeek == DayOfWeek.Monday) { return true; }
            }
            if (Date.Month == 5) {
                if (Date.Day > 24 && Date.DayOfWeek == DayOfWeek.Monday) { return true; }
            }
            if (Date.Month == 7) {
                if (Date.Day == 4) { return true; }
                if (Date.Day == 3 && Date.DayOfWeek == DayOfWeek.Friday) { return true; }
                if (Date.Day == 5 && Date.DayOfWeek == DayOfWeek.Monday) { return true; }
            }
            if (Date.Month == 9) {
                if (Date.Day < 8 && Date.DayOfWeek == DayOfWeek.Monday) { return true; }
            }
            if (Date.Month == 10) {
                if (Date.Day > 7 && Date.Day < 15 && Date.DayOfWeek == DayOfWeek.Monday) { return true; }
            }
            if (Date.Month == 11) {
                if (Date.Day == 11) { return true; }
                if (Date.Day == 10 && Date.DayOfWeek == DayOfWeek.Friday) { return true; }
                if (Date.Day == 12 && Date.DayOfWeek == DayOfWeek.Monday) { return true; }
                if (Date.Day > 21 && Date.Day < 29 && Date.DayOfWeek == DayOfWeek.Thursday){ return true;}
                if (Date.Day > 22 && Date.Day < 30 && Date.DayOfWeek == DayOfWeek.Friday) { return true; }
            }
            if (Date.Month == 12) {
                if (Date.Day == 25) { return true; }
                if (Date.Day == 24 && Date.DayOfWeek == DayOfWeek.Friday) { return true; }
                if (Date.Day == 26 && Date.DayOfWeek == DayOfWeek.Monday) { return true; }
                if (Date.Day == 31 && Date.DayOfWeek == DayOfWeek.Friday) { return true; }
            }
            
            return false;
        
        
        }
        public string IntToMonthString(int month) {
            if (month%12 == 1) return "January";
            else if (month%12 == 2) return "February";
            else if (month%12 == 3) return "March";
            else if (month%12 == 4) return "April";
            else if (month%12 == 5) return "May";
            else if (month%12 == 6) return "June";
            else if (month%12 == 7) return "July";
            else if (month%12 == 8) return "August";
            else if (month%12 == 9) return "September";
            else if (month%12 == 10) return "October";
            else if (month%12 == 11) return "November";
            else if (month%12 == 0) return "December";
            else  return "value out of range"; 
        }
        public int StringToMonthInt(string month) {
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
        private void DatePicker_Select(object sender, EventArgs e){
                ComboBox DatePickerCombo = sender as ComboBox;
                ComboBoxItem SelectedItem = DatePickerCombo.SelectedItem as ComboBoxItem;
                string SelectedMonth = SelectedItem.Content.ToString().Substring(0, SelectedItem.Content.ToString().IndexOf(" "));
                if (Month != StringToMonthInt(SelectedMonth) || Year != Convert.ToInt32(SelectedItem.Content.ToString().Substring(SelectedItem.Content.ToString().IndexOf(" ")+1, 4))) {
                    Month = StringToMonthInt(SelectedMonth);
                    Year = Convert.ToInt32(SelectedItem.Content.ToString().Substring(SelectedItem.Content.ToString().IndexOf(" ")+1, 4));
                    MonthDisplay.Children.Clear();
                    populateCalender();
                    UpdateCalendar();
                }
        }
        
    }
}
