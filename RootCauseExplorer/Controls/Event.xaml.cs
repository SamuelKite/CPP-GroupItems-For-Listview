using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;

namespace RootCauseExplorer
{
   
    public partial class Event : UserControl
    {
        public string AssociatedRecordID;
        public DateTime StartDate, EndDate;
        public bool Clicked = false;
        public List<XElement> EventsRecords = null;
        public XNamespace RecordNamespace = null;
        public Event(List<XElement> RecordSet, XNamespace z, DateTime EventDate, DateTime EventEndDate) 
        {
            InitializeComponent();
            EventTitleGrid.Visibility = Visibility.Collapsed;
            this.Height = 14;
            StartDate = EventDate;
            EndDate = EventEndDate;
            RecordNamespace = z;
            EventsRecords = RecordSet;

            DateBarText.Text = RecordSet.Count().ToString();
            DateBarText.HorizontalAlignment = HorizontalAlignment.Center;
            if (RecordSet.Count != 0)
            {
                MouseLeftButtonUp += new MouseButtonEventHandler(Event_MouseLeftButtonUp);
                MouseEnter += new MouseEventHandler(Event_MouseEnter);
                MouseLeave += new MouseEventHandler(Event_MouseLeave);
            }
        }

        public Event(string EventTitle, string EventDate, string EventEndDate, string ID, bool ExecComStatus, bool EventsStatus, bool DemoStatus)
        {
            InitializeComponent();
            AssociatedRecordID =  ID;
            EventTitleText.Text = EventTitle;

            //ows_EventDate='2010-07-12 00:00:00'
            StartDate = new DateTime(Convert.ToInt32(EventDate.Substring(0, 4)), 
                                    Convert.ToInt32(EventDate.Substring(5,2)), 
                                    Convert.ToInt32(EventDate.Substring(8,2)));
            
            

            EndDate = new DateTime(Convert.ToInt32(EventEndDate.Substring(0, 4)),
                                    Convert.ToInt32(EventEndDate.Substring(5, 2)),
                                    Convert.ToInt32(EventEndDate.Substring(8, 2)));
            

            if (StartDate == EndDate) 
            {
                DateBarText.Text = OverViewCalendar.CalendarObject.IntToMonthString(StartDate.Month).Substring(0, 3).ToUpper() + " " + StartDate.Day.ToString();
            }
            else if (StartDate.Month != EndDate.Month)
            {
                DateBarText.Text = OverViewCalendar.CalendarObject.IntToMonthString(StartDate.Month).Substring(0, 3).ToUpper() + " " + StartDate.Day.ToString() + " - " + OverViewCalendar.CalendarObject.IntToMonthString(EndDate.Month).Substring(0, 3).ToUpper() + " " + EndDate.Day.ToString();
            }
            else 
            {
                DateBarText.Text = OverViewCalendar.CalendarObject.IntToMonthString(StartDate.Month).Substring(0, 3).ToUpper() + " " + StartDate.Day.ToString() + " - " + EndDate.Day.ToString();
            }

            EventTitleStack.Width = 12 + EventTitleText.ActualWidth;
            if (ExecComStatus) { ExecComIcon.Visibility = Visibility.Visible; EventTitleStack.Width += ExecComIcon.Width; }
            if (EventsStatus) { EventsIcon.Visibility = Visibility.Visible; EventTitleStack.Width += EventsIcon.Width; }
            if (DemoStatus) { DemoIcon.Visibility = Visibility.Visible; EventTitleStack.Width += DemoIcon.Width; }

            EventTitleStack.LayoutUpdated += new EventHandler(EventTitleStack_LayoutUpdated);
           
            MouseLeftButtonUp += new MouseButtonEventHandler(Event_MouseLeftButtonUp);
            MouseEnter += new MouseEventHandler(Event_MouseEnter);
            MouseLeave += new MouseEventHandler(Event_MouseLeave);

        }

        void EventTitleStack_LayoutUpdated(object sender, EventArgs e)
        {
            double NewWidth = EventTitleText.ActualWidth +12;
            if (DemoIcon.Visibility == Visibility.Visible)
            {
                NewWidth += DemoIcon.Width;
            } if (EventsIcon.Visibility == Visibility.Visible)
            {
                NewWidth += EventsIcon.Width;
            } if (ExecComIcon.Visibility == Visibility.Visible)
            {
                NewWidth += ExecComIcon.Width;
            }
            EventTitleStack.Width = NewWidth;
            
           
        }

        void Event_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!Clicked)
            {
                BitmapImage BG = new BitmapImage();
                BG.UriSource = new Uri("DateBarBGInactive.png", UriKind.RelativeOrAbsolute);
                DateBarBG.SetValue(Image.SourceProperty, BG);
                VisualStateManager.GoToState(this, "Normal", true);
            }
        }

        void Event_MouseEnter(object sender, MouseEventArgs e)
        {
            BitmapImage BG = new BitmapImage();
            BG.UriSource = new Uri("DateBarBGActive.png", UriKind.RelativeOrAbsolute);
            DateBarBG.SetValue(Image.SourceProperty, BG);
            VisualStateManager.GoToState(this, "MouseOver", true);
        }
        

        void Event_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (OverViewCalendar.CalendarObject.CurrentView.Value == "Monthly")
            {
                OverViewCalendar.CalendarObject.ShowDetailsPane(AssociatedRecordID, e);
            }
            else OverViewCalendar.CalendarObject.ShowDetailsPane(this, e);
            Clicked = true;
        }
    }
}
