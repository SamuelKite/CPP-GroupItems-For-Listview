using System;
using Windows.UI.Xaml.Controls;

namespace RootCauseExplorer
{
    public partial class DetailsPane : UserControl
    {
        public DetailsPane()
        {
            InitializeComponent();
            Monthly.Width = Quarterly.Width = MonthlyShadow.Width = QuarterlyShadow.Width = 242;
            
            Monthly.LayoutUpdated += new EventHandler(Monthly_LayoutUpdated);
            Quarterly.LayoutUpdated += new EventHandler(Quarterly_LayoutUpdated);
        }
        public void InitializeFields() 
        {
            ExecComIcon.Visibility = DemoIcon.Visibility = EventsIcon.Visibility = 
               ExecCommLeadStack.Visibility = EventTeamLeadStack.Visibility = DemoLeadStack.Visibility = 
               LocationStack.Visibility = CategoryStack.Visibility = Description.Visibility = Visibility.Visible;
                        
            Title.Text = Description.Text = LocationText.Text = CategoryText.Text = EventTeamLeadText.Text = "";
            EventsWorkspaceTitle.Visibility = Visibility.Collapsed;
        }

        void Quarterly_LayoutUpdated(object sender, EventArgs e)
        {
            QuarterlyShadow.SetValue(Canvas.HeightProperty, Quarterly.ActualHeight);
            QuarterlyShadow.Margin = new Thickness(Quarterly.Margin.Left + 5, Quarterly.Margin.Top + 5, 0, 0);
        }

        void Monthly_LayoutUpdated(object sender, EventArgs e)
        {
            MonthlyShadow.SetValue(Canvas.HeightProperty, Monthly.ActualHeight);
            MonthlyShadow.Margin = new Thickness(Monthly.Margin.Left + 5, Monthly.Margin.Top + 5, 0, 0);
                
        }

    }
}
