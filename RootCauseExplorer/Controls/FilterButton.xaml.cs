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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RootCauseExplorer
{
    public partial class FilterButton : UserControl
    {
        BitmapImage NormalBackground = new BitmapImage(new Uri("FilterButtonBG.png", UriKind.RelativeOrAbsolute));
        BitmapImage DisabledBackground = new BitmapImage(new Uri("FilterButtonBGDisabled.png", UriKind.RelativeOrAbsolute));
        bool FilterIsDisabled = false;
        public FilterButton(string Label, string ImageSource)
        {
            InitializeComponent();
            this.Label.Text = Label;
            this.Icon.Source = new BitmapImage(new Uri(ImageSource, UriKind.RelativeOrAbsolute));
            LayoutRoot.Width = 30 + Icon.Width + this.Label.ActualWidth;
            MouseLeftButtonUp += new MouseButtonEventHandler(FilterButton_MouseLeftButtonUp);
            MouseEnter += new MouseEventHandler(FilterButton_MouseEnter);
            MouseLeave += new MouseEventHandler(FilterButton_MouseLeave);
        }

        void FilterButton_MouseLeave(object sender, MouseEventArgs e)
        {
            if (FilterIsDisabled)
            {
                VisualStateManager.GoToState(this, "Disabled", true);
                ImageBrush brush = new ImageBrush();
                brush.ImageSource = DisabledBackground;
                LayoutRoot.Background = brush;
            }
            else
            {
                VisualStateManager.GoToState(this, "Normal", true);
                ImageBrush brush = new ImageBrush();
                brush.ImageSource = NormalBackground;
                LayoutRoot.Background = brush;
            }
        }

        void FilterButton_MouseEnter(object sender, MouseEventArgs e)
        {
            if (FilterIsDisabled)
            {
                VisualStateManager.GoToState(this, "MouseOverDisabled", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "MouseOver", true);
            }
        }

        void FilterButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            FilterIsDisabled = !FilterIsDisabled;
            OverViewCalendar.CalendarObject.ToggleFilter(this.Label.Text);
            if (FilterIsDisabled) 
            {
                VisualStateManager.GoToState(this, "MouseOverDisabled", true);
                RedSlash.Visibility = Visibility.Visible;
                ImageBrush brush = new ImageBrush();
                brush.ImageSource = DisabledBackground;
                LayoutRoot.Background = brush;
            } else
            {
                VisualStateManager.GoToState(this, "MouseOver", true);
                RedSlash.Visibility = Visibility.Collapsed;
                ImageBrush brush = new ImageBrush();
                brush.ImageSource = NormalBackground;
                LayoutRoot.Background = brush;
            }
        }
        
    }
}
