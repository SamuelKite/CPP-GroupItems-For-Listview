using Windows.UI.Xaml.Controls;

namespace RootCauseExplorer
{
    public partial class ViewChanger : UserControl
    {
        public ViewChanger()
        {
            InitializeComponent();
            LeftArrow.MouseEnter += new MouseEventHandler(LeftArrow_MouseEnter);
            RightArrow.MouseEnter += new MouseEventHandler(RightArrow_MouseEnter);
            LeftArrow.MouseLeave += new MouseEventHandler(Arrow_MouseLeave);
            RightArrow.MouseLeave += new MouseEventHandler(Arrow_MouseLeave);
            LeftArrow.MouseLeftButtonUp += new MouseButtonEventHandler(LeftArrow_MouseLeftButtonUp);
            RightArrow.MouseLeftButtonUp += new MouseButtonEventHandler(RightArrow_MouseLeftButtonUp);
            LeftArrow.MouseLeftButtonDown += new MouseButtonEventHandler(LeftArrow_MouseLeftButtonDown);
            RightArrow.MouseLeftButtonDown += new MouseButtonEventHandler(RightArrow_MouseLeftButtonDown);
        }

        void RightArrow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DisplayText.Text = "Loading ...";
            OverViewCalendar.CalendarObject.numSelectedDates = 0;
            OverViewCalendar.CalendarObject.EventsGrid.Children.Clear();
            OverViewCalendar.CalendarObject.DisableCalendarColumns();

        }

        void LeftArrow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DisplayText.Text = "Loading ...";
            OverViewCalendar.CalendarObject.numSelectedDates = 0;
            OverViewCalendar.CalendarObject.EventsGrid.Children.Clear();
            OverViewCalendar.CalendarObject.DisableCalendarColumns();
        }

        public void UpdateText()
        {
            if (OverViewCalendar.CalendarObject.CurrentView.Value == "Monthly")
            {
                this.DisplayText.Text = OverViewCalendar.CalendarObject.IntToMonthString(OverViewCalendar.CalendarObject.CurrentSetting.Month).ToUpper() + " " + OverViewCalendar.CalendarObject.CurrentSetting.Year.ToString();
            }
            if (OverViewCalendar.CalendarObject.CurrentView.Value == "Quarterly")
            {
                if (OverViewCalendar.CalendarObject.IntMonthToFiscalQuarterInt(OverViewCalendar.CalendarObject.CurrentSetting.Month) < 3)
                {
                    this.DisplayText.Text = "Q" + OverViewCalendar.CalendarObject.IntMonthToFiscalQuarterInt(OverViewCalendar.CalendarObject.CurrentSetting.Month).ToString() + " FY" + OverViewCalendar.CalendarObject.CurrentSetting.AddYears(1).Year.ToString().Substring(2);
                }
                else this.DisplayText.Text = "Q" + OverViewCalendar.CalendarObject.IntMonthToFiscalQuarterInt(OverViewCalendar.CalendarObject.CurrentSetting.Month).ToString() + " FY" + OverViewCalendar.CalendarObject.CurrentSetting.Year.ToString().Substring(2);

            }
        }

        void RightArrow_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (OverViewCalendar.CalendarObject.CurrentView.Value == "Monthly")
            {
                OverViewCalendar.CalendarObject.CurrentSetting = OverViewCalendar.CalendarObject.CurrentSetting.AddMonths(1);
            }

            if (OverViewCalendar.CalendarObject.CurrentView.Value == "Quarterly")
            {
                OverViewCalendar.CalendarObject.CurrentSetting = OverViewCalendar.CalendarObject.CurrentSetting.AddMonths(3);
            }
            UpdateText();
            OverViewCalendar.CalendarObject.CalendarGrid.Children.Clear();
            OverViewCalendar.CalendarObject.populateCalender();



        }

        void LeftArrow_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (OverViewCalendar.CalendarObject.CurrentView.Value == "Monthly")
            {
                OverViewCalendar.CalendarObject.CurrentSetting = OverViewCalendar.CalendarObject.CurrentSetting.AddMonths(-1);
            }
            if (OverViewCalendar.CalendarObject.CurrentView.Value == "Quarterly")
            {
                OverViewCalendar.CalendarObject.CurrentSetting = OverViewCalendar.CalendarObject.CurrentSetting.AddMonths(-3);
            }
            UpdateText();
            OverViewCalendar.CalendarObject.CalendarGrid.Children.Clear();
            OverViewCalendar.CalendarObject.populateCalender();

        }

        void Arrow_MouseLeave(object sender, MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, "Normal", true);
        }

        void RightArrow_MouseEnter(object sender, MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, "MouseOverRightArrow", true);
        }

        void LeftArrow_MouseEnter(object sender, MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, "MouseOverLeftArrow", true);
        }

    }
}
