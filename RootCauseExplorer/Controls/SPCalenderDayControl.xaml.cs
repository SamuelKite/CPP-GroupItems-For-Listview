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
    public partial class SPCalenderDayControl : UserControl
    {
        public CalendarDisplay.CalendarPicker HostPage = null;
        public int Month = 0;
        public int Year = 0;
        public bool SelectedDate = false;
        public bool HighlightedDate = false;
        public string BookedRooms = "";
        public bool Disabled = false;
        
        public SPCalenderDayControl()
        {
            InitializeComponent();
            unGhost();
        }

        void Classroom_MouseEnter(object sender, MouseEventArgs e)
        {
            Grid sGrid = sender as Grid;
            Storyboard CorrectMouseOverAnim = this.FindName(sGrid.Name+"MouseOver") as Storyboard;
            if (CorrectMouseOverAnim != null) { 
            CorrectMouseOverAnim.Begin();
            }
            

        }
        void Classroom_MouseClick(object sender, MouseButtonEventArgs e) {
            Grid sGrid = sender as Grid;
            SelectClassroom(Convert.ToInt32(sGrid.Name.ToString().Substring(9,1)));

        }
        public void SelectClassroom(int CRNumber) {
            if (HostPage != null)
            {

                if (SelectedDate == false && HostPage.numSelectedDates < 2)
                {
                    SelectedDate = true;
                    HostPage.numSelectedDates++;
                    HostPage.setDate(Year, Month, Convert.ToInt32(this.Date.Text.ToString()), "Classroom"+CRNumber.ToString());
                    HostPage.UpdateCalendar();
                }
                else if (SelectedDate == true && HostPage.numSelectedDates > 0)
                {
                    SelectedDate = false;
                    HostPage.numSelectedDates--;
                    HostPage.unsetDate(Year, Month, Convert.ToInt32(this.Date.Text.ToString()), "Classroom" + CRNumber.ToString());
                    HostPage.UpdateCalendar();
                }
                else if(!HighlightedDate) {
                    SelectedDate = true;
                    HostPage.numSelectedDates++;
                }


            }


        }
        public void HighlightTile() {
            Highlight.Begin();
            if (HostPage.SelectedClassroom != null) { 
                Storyboard currentCR = this.FindName("HighlightCR" + HostPage.SelectedClassroom.Substring(9, 1).ToString()) as Storyboard;
                currentCR.Begin();
            }
            HighlightedDate = true;
           
        }
        public void unHighlightTile() {
            Reset.Begin();
            if (HostPage.SelectedClassroom != null && !BookedRooms.Contains(HostPage.SelectedClassroom.Substring(9,1)))
            {
                Storyboard currentCR = this.FindName("UnghostCR" + HostPage.SelectedClassroom.Substring(9, 1)) as Storyboard;
                currentCR.Begin();
            }
            HighlightedDate = false;
        }
        public void unGhost() {
            this.Classroom1.MouseLeftButtonUp -= Classroom_MouseClick;
            this.Classroom2.MouseLeftButtonUp -= Classroom_MouseClick;
            this.Classroom3.MouseLeftButtonUp -= Classroom_MouseClick;
            this.Classroom4.MouseLeftButtonUp -= Classroom_MouseClick;
            this.Classroom5.MouseLeftButtonUp -= Classroom_MouseClick;
            this.Classroom6.MouseLeftButtonUp -= Classroom_MouseClick;
            this.Classroom7.MouseLeftButtonUp -= Classroom_MouseClick;
            this.Classroom8.MouseLeftButtonUp -= Classroom_MouseClick;
            if(!BookedRooms.Contains("1")){
            this.Classroom1.MouseLeftButtonUp += new MouseButtonEventHandler(Classroom_MouseClick);
            UnghostCR1.Begin();
            } if (!BookedRooms.Contains("2"))
            {
                this.Classroom2.MouseLeftButtonUp += new MouseButtonEventHandler(Classroom_MouseClick);
                UnghostCR2.Begin();
            } if (!BookedRooms.Contains("3"))
            {
                this.Classroom3.MouseLeftButtonUp += new MouseButtonEventHandler(Classroom_MouseClick);
                UnghostCR3.Begin();
            } if (!BookedRooms.Contains("4"))
            {
                this.Classroom4.MouseLeftButtonUp += new MouseButtonEventHandler(Classroom_MouseClick);
                UnghostCR4.Begin();
            } if (!BookedRooms.Contains("5"))
            {
                this.Classroom5.MouseLeftButtonUp += new MouseButtonEventHandler(Classroom_MouseClick);
                UnghostCR5.Begin();
            } if (!BookedRooms.Contains("6"))
            {
                this.Classroom6.MouseLeftButtonUp += new MouseButtonEventHandler(Classroom_MouseClick);
                UnghostCR6.Begin();
            } if (!BookedRooms.Contains("7"))
            {
                this.Classroom7.MouseLeftButtonUp += new MouseButtonEventHandler(Classroom_MouseClick);
                UnghostCR7.Begin();
            } if (!BookedRooms.Contains("8"))
            {
                this.Classroom8.MouseLeftButtonUp += new MouseButtonEventHandler(Classroom_MouseClick);
                UnghostCR8.Begin();
            }
        }
        public void Ghost(){
            MakeGhosted.Begin();
        }
        public void Ghost(string allclassrooms)
        {
            for (int i = 1; i <= 8; i++)
            {
                Storyboard thisClassroom = (Storyboard)this.FindName("GhostCR" + i.ToString());
                Grid thisClassroomGrid = (Grid)this.FindName("Classroom" + i.ToString());
                thisClassroomGrid.MouseLeftButtonUp -= Classroom_MouseClick;
                thisClassroom.Begin();
            }
        }
        public void Ghost(int ClassNumber)
        {
            if (ClassNumber >= 1 && ClassNumber <= 8)
            {
                Storyboard thisClassroom = (Storyboard)this.FindName("GhostCR" + ClassNumber.ToString());
                Grid thisClassroomGrid = (Grid)this.FindName("Classroom" + ClassNumber.ToString());
                thisClassroomGrid.MouseLeftButtonUp -= Classroom_MouseClick;
                thisClassroom.Begin();

            }
        }
        public void makeToday()
        {
            MakeToday.Begin();

        }
        public void makeHoliday()
        {
            MakeHoliday.Begin();
        }
        public void bookRoom(int roomnumber) {
            BookedRooms += roomnumber.ToString();
        }
        
        
    }
}
