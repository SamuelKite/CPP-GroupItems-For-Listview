using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace RootCauseExplorer
{
    public sealed partial class MainPage : Page
    {
        string Selected = "none";
        public MainPage()
        {
            InitializeComponent();
                IDictionary<string, string> Initparms = new Dictionary<string, string>();
                Initparms.Add("userNumber", "1");
                CalendarPicker ThePicker = new CalendarPicker(Initparms);
                ThePicker.Name = "PickerCalendar";
                ThePicker.SetValue(Grid.ColumnProperty, 2);
                ThePicker.MouseEnter += new MouseEventHandler(ThePicker_MouseEnter);
                ThePicker.MouseLeave += new MouseEventHandler(ThePicker_MouseLeave);
                TransformGroup tg = new TransformGroup();
                ScaleTransform st = new ScaleTransform();
                tg.Children.Add(st);
                st.SetValue(ScaleTransform.ScaleXProperty, (double).1);
                st.SetValue(ScaleTransform.ScaleYProperty, (double).1);
                ThePicker.RenderTransform = tg;

                rightSV.Content = ThePicker;

                rightclickintercept.MouseEnter += new MouseEventHandler(RightOuterBorder_MouseEnter);
                rightclickintercept.MouseLeave += new MouseEventHandler(RightOuterBorder_MouseLeave);

                leftclickintercept.MouseEnter += new MouseEventHandler(ThePicker_MouseEnter);
                leftclickintercept.MouseLeave += new MouseEventHandler(RightOuterBorder_MouseLeave);

                rightclickintercept.MouseLeftButtonDown += new MouseButtonEventHandler(rightclickintercept_MouseLeftButtonDown);

                leftclickintercept.MouseLeftButtonDown += new MouseButtonEventHandler(leftclickintercept_MouseLeftButtonDown);
           
        }

        void leftclickintercept_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Selected == "none")
            {
                VisualStateManager.GoToState(this, "LeftSelected", true); Selected = "left";
            }
            else { VisualStateManager.GoToState(this, "NoneSelected", true); Selected = "none"; }
        }

        void rightclickintercept_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Selected == "none") { VisualStateManager.GoToState(this, "RightSelected", true); Selected = "right"; }
            else { VisualStateManager.GoToState(this, "NoneSelected", true); Selected = "none"; }
        }

        void ThePicker_MouseLeave(object sender, MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, "MouseOff", true);
        }

        void RightOuterBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, "MouseOff", true);
        }

        void RightOuterBorder_MouseEnter(object sender, MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, "MouseOverRight", true);

        }
        void ThePicker_MouseEnter(object sender, MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, "MouseOverLeft", true);
        }

    }
}
