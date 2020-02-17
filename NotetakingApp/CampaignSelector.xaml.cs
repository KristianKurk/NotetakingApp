using Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NotetakingApp
{
    /// <summary>
    /// Interaction logic for CampaignSelector.xaml
    /// </summary>
    public partial class CampaignSelector : Page
    {
        public CampaignSelector()
        {
            InitializeComponent();
        }

        private void SetTitle_Click(object sender, RoutedEventArgs e)
        {
            Go();
        }

        private void Go() {
            string newName = title.Text;

            if (newName.Length > 0)
            {
                foreach (Window window in Application.Current.Windows.OfType<CampaignSelectWindow>())
                    ((CampaignSelectWindow)window).NewCampaign(newName);
            }
            else
            {
                ErrorMsg.Text = "Please input a value for the name.";
            }
        }

        private void Title_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return) {
                Go();
            }
        }
    }
}
