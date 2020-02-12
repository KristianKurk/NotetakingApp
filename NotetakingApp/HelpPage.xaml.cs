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
    /// Interaction logic for HelpPage.xaml
    /// </summary>
    public partial class HelpPage : Page
    {
        String disabledButton;
        public HelpPage()
        {
            InitializeComponent();
        }
        private void BtnHelpDash(object sender, RoutedEventArgs e)
        {
            disabledButton = "helpDash";
            DisableButton("helpDash");
            help.Content = new HelpDash();
        }
        private void BtnHelpMap(object sender, RoutedEventArgs e)
        {
            disabledButton = "helpMap";
            DisableButton("helpMap");
            help.Content = new HelpMap();
        }
        private void BtnHelpNote(object sender, RoutedEventArgs e)
        {
            disabledButton = "helpNote";
            DisableButton("helpNote");
            help.Content = new HelpNote();
        }
        private void BtnHelpRNG(object sender, RoutedEventArgs e)
        {
            disabledButton = "helpRNG";
            DisableButton("helpRNG");
            help.Content = new HelpRNG();
        }
        private void BtnHelpSettings(object sender, RoutedEventArgs e)
        {
            disabledButton = "helpSettings";
            DisableButton("helpSettings");
            help.Content = new HelpSettings();
        }
        private void DisableButton(string btn)
        {

            EnableAll();

            //Disable Selected button

            object button = helpGrid.FindName(btn);
            Button button1 = (Button)button;
            button1.Background = new SolidColorBrush(Color.FromArgb(255, 86, 50, 50)) { Opacity = 0 };

            //  button1.IsEnabled = false;
            button1.Focusable = false;
            button1.IsEnabled = false;

        }
        private void EnableAll()
        {

            //Enable all buttons

            object button1 = helpGrid.FindName("helpDash");
            object button2 = helpGrid.FindName("helpMap");
            object button3 = helpGrid.FindName("helpNote");
            object button4 = helpGrid.FindName("helpRNG");
            object button5 = helpGrid.FindName("helpSettings");

            Button navb1 = (Button)button1;
            Button navb2 = (Button)button2;
            Button navb3 = (Button)button3;
            Button navb4 = (Button)button4;
            Button navb5 = (Button)button5;

            navb1.IsEnabled = true;
            navb2.IsEnabled = true;
            navb3.IsEnabled = true;
            navb4.IsEnabled = true;
            navb5.IsEnabled = true;

            navb1.Background = new SolidColorBrush(Color.FromRgb(255, 229, 207)) { Opacity = 1 };
            navb2.Background = new SolidColorBrush(Color.FromRgb(255, 229, 207)) { Opacity = 1 };
            navb3.Background = new SolidColorBrush(Color.FromRgb(255, 229, 207)) { Opacity = 1 };
            navb4.Background = new SolidColorBrush(Color.FromRgb(255, 229, 207)) { Opacity = 1 };
            navb5.Background = new SolidColorBrush(Color.FromRgb(255, 229, 207)) { Opacity = 1 };

        }
        //Set button hover color
        private void rngNavButton_MouseEnter(object sender, MouseEventArgs e)
        {

            Button button1 = (Button)sender;
            if (disabledButton != button1.Name)
            {
                button1.Background = new SolidColorBrush(Color.FromRgb(250, 238, 227));
            }
        }

        //Reset button hover color on mouse leave
        private void rngNavButton_MouseLeave(object sender, MouseEventArgs e)
        {
            Button button1 = (Button)sender;

            if (disabledButton != button1.Name)
            {
                button1.Background = new SolidColorBrush(Color.FromRgb(255, 229, 207));
            }
        }
    }
}
