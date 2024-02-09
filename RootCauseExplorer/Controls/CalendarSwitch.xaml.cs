using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace RootCauseExplorer
{
    public partial class CalendarSwitch : UserControl
    {
        public CalendarSwitch()
        {
            InitializeComponent();
            MonthlyText.MouseEnter += new MouseEventHandler(MonthlyText_MouseEnter);
            QuarterlyText.MouseEnter += new MouseEventHandler(QuarterlyText_MouseEnter);
            AllFYText.MouseEnter += new MouseEventHandler(AllFYText_MouseEnter);
            LayoutRoot.MouseLeave += new MouseEventHandler(LayoutRoot_MouseLeave);
            MonthlyText.MouseLeftButtonUp += new MouseButtonEventHandler(MonthlyText_MouseLeftButtonUp);
            QuarterlyText.MouseLeftButtonUp += new MouseButtonEventHandler(QuarterlyText_MouseLeftButtonUp);
            MonthlyText.MouseLeftButtonDown += new MouseButtonEventHandler(MonthlyText_MouseLeftButtonDown);
            QuarterlyText.MouseLeftButtonDown += new MouseButtonEventHandler(QuarterlyText_MouseLeftButtonDown);
            AllFYText.MouseLeftButtonUp += new MouseButtonEventHandler(AllFYText_MouseLeftButtonUp);
        }

        void QuarterlyText_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (OverViewCalendar.CalendarObject.CurrentView.Value != "Quarterly")
            {
                OverViewCalendar.CalendarObject.ViewChanger.DisplayText.Text = "Loading ...";
                OverViewCalendar.CalendarObject.EventsGrid.Children.Clear();
                OverViewCalendar.CalendarObject.DisableCalendarColumns();
            }
        }

        void MonthlyText_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (OverViewCalendar.CalendarObject.CurrentView.Value != "Monthly")
            {
                OverViewCalendar.CalendarObject.ViewChanger.DisplayText.Text = "Loading ...";
                OverViewCalendar.CalendarObject.EventsGrid.Children.Clear();
                OverViewCalendar.CalendarObject.DisableCalendarColumns();
            }
        }

        void AllFYText_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            string AllItemsCalendarView = "http://sharepoint/sites/compass/Lists/WBG%20Events%20Calendar/AllItems.aspx";
            System.Windows.Browser.HtmlPage.Window.Eval("window.location='" + AllItemsCalendarView + "';");
        }

        void QuarterlyText_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (OverViewCalendar.CalendarObject.CurrentView.Value != "Quarterly")
            {
                OverViewCalendar.CalendarObject.CurrentView.Value = "Quarterly";
                OverViewCalendar.CalendarObject.CalendarGrid.Children.Clear();
                OverViewCalendar.CalendarObject.populateCalender();
            }
        }

        void MonthlyText_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (OverViewCalendar.CalendarObject.CurrentView.Value != "Monthly")
            {
                OverViewCalendar.CalendarObject.CurrentView.Value = "Monthly";
                OverViewCalendar.CalendarObject.CalendarGrid.Children.Clear();
                OverViewCalendar.CalendarObject.populateCalender();
            }
        }

        void LayoutRoot_MouseLeave(object sender, MouseEventArgs e)
        {
            if (OverViewCalendar.CalendarObject.CurrentView.Value == "Monthly") VisualStateManager.GoToState(this, "Monthly", true);
            else if (OverViewCalendar.CalendarObject.CurrentView.Value == "Quarterly") VisualStateManager.GoToState(this, "Quarterly", true);
        }

        void AllFYText_MouseEnter(object sender, MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, "AllFY", true);
        }

        void QuarterlyText_MouseEnter(object sender, MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, "Quarterly", true);
        }

        void MonthlyText_MouseEnter(object sender, MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, "Monthly", true);
        }
    }
}
