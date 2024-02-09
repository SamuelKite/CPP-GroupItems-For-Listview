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
    public partial class DateColumn : UserControl
    {
        public bool IsSelected = false;
        public bool IsHeaderColumn = false;
        public DateTime ThisStartDate, ThisEndDate;

        public DateColumn(DateTime Date)
        {
            InitializeComponent();
            ThisStartDate = Date;
            this.MouseEnter += new MouseEventHandler(DateColumn_MouseEnter);
            this.MouseLeave += new MouseEventHandler(DateColumn_MouseLeave);
            this.MouseLeftButtonDown += new MouseButtonEventHandler(DateColumn_MouseLeftButtonDown);
            this.MouseLeftButtonUp += new MouseButtonEventHandler(DateColumn_MouseLeftButtonUp);
            
            this.DateText.Text = ThisStartDate.Day.ToString();
            this.DayText.Text = ThisStartDate.DayOfWeek.ToString().Substring(0, 2);
        }

        public DateColumn(DateTime Date1, DateTime Date2) 
        {
            InitializeComponent();
            ThisStartDate = Date1;
            ThisEndDate = Date2;

            this.DateGrid.Visibility = Visibility.Collapsed;
            if (ThisEndDate != ThisStartDate)
            {
                this.DayText.Text = ThisStartDate.Day.ToString() + " - " + ThisEndDate.Day.ToString();
            }
            else 
            {
                this.DayText.Text = ThisStartDate.Day.ToString();
            }
        }
        public DateColumn(string Header) 
        {
            InitializeComponent();
            this.DayGrid.Visibility = Visibility.Collapsed;
            this.ColumnGrid.Visibility = Visibility.Collapsed;
            this.DateText.Text = Header;
            IsHeaderColumn = true;
        }



        void DateColumn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            OverViewCalendar.CalendarObject.LeftMouseIsDown = false;
        }

        void DateColumn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OverViewCalendar.CalendarObject.SelectDate(this);
        }

        void DateColumn_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!IsSelected)
            {
                if (((ThisStartDate > OverViewCalendar.CalendarObject.SelectedDate2 && ThisStartDate < OverViewCalendar.CalendarObject.SelectedDate1)
                     ||
                    (ThisStartDate < OverViewCalendar.CalendarObject.SelectedDate2 && ThisStartDate > OverViewCalendar.CalendarObject.SelectedDate1))
                     &&
                    OverViewCalendar.CalendarObject.numSelectedDates == 2)
                {
                    VisualStateManager.GoToState(this, "Between", true);
                }
                else
                {
                    VisualStateManager.GoToState(this, "Normal", true);
                }

            }
            
        }

        void DateColumn_MouseEnter(object sender, MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, "MouseOver", true);
            if (OverViewCalendar.CalendarObject.LeftMouseIsDown)
            {

                OverViewCalendar.CalendarObject.SelectDate(this);
            }
            
        }
        public void MakeLastDayOfMonth() 
        {
            RightBorder1.Visibility = Visibility.Visible;
            RightBorder2.Visibility = Visibility.Visible;
            RightBorder3.Visibility = Visibility.Visible;
        }
        public void MakeWeekend()
        {

            LayoutRoot.Background = new SolidColorBrush(Color.FromArgb((byte)255, (byte)228, (byte)241, (byte)228));
            this.Width = 22;
            DayGrid.Visibility = Visibility.Collapsed;
        }
        public void Disable() 
        {
            VisualStateManager.GoToState(this, "Disabled", true);
            this.MouseEnter -= DateColumn_MouseEnter;
            this.MouseLeave -= DateColumn_MouseLeave;
            this.MouseLeftButtonDown -= DateColumn_MouseLeftButtonDown;
            this.MouseLeftButtonUp -= DateColumn_MouseLeftButtonUp;
        }

    }
}
