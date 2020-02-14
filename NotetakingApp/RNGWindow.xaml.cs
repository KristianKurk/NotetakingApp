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
    /// Interaction logic for RNGWindow.xaml
    /// </summary>
    public partial class RNGWindow : Page
    {
        String disabledButton;
        public RNGWindow()
        {
            InitializeComponent();
        }
        
        private void BtnAddRNG(object sender, RoutedEventArgs e)
        {
            disabledButton = "addData";
            DisableButton("addData");
            rng.Content = new RNGAdd();
        }
        private void BtnGenerate(object sender, RoutedEventArgs e)
        {
            disabledButton = "generateRNG";
            DisableButton("generateRNG");
            rng.Content = new RNGGenerate();
        }
        private void BtnDice(object sender, RoutedEventArgs e)
        {
            disabledButton = "rngDice";
            DisableButton("rngDice");
            rng.Content = new RNGDice();
        }
        private void DisableButton(string btn)
        {

            EnableAll();

            //Disable Selected button

            object button = rngGrid.FindName(btn);
            Button button1 = (Button)button;
            button1.Background = new SolidColorBrush(Color.FromArgb(Properties.Settings.Default.Color16a, Properties.Settings.Default.Color16b, Properties.Settings.Default.Color16c, Properties.Settings.Default.Color16d)) { Opacity = 0 };

            //  button1.IsEnabled = false;
            button1.Focusable = false;
            button1.IsEnabled = false;

        }
        private void EnableAll()
        {

            //Enable all buttons

            object button1 = rngGrid.FindName("addData");
            object button2 = rngGrid.FindName("generateRNG");
            object button3 = rngGrid.FindName("rngDice");

            Button navb1 = (Button)button1;
            Button navb2 = (Button)button2;
            Button navb3 = (Button)button3;


            navb1.IsEnabled = true;
            navb2.IsEnabled = true;
            navb3.IsEnabled = true;

            navb1.Background = new SolidColorBrush(Color.FromRgb(Properties.Settings.Default.Color17a, Properties.Settings.Default.Color17b, Properties.Settings.Default.Color17c)){ Opacity = 1 };
            navb2.Background = new SolidColorBrush(Color.FromRgb(Properties.Settings.Default.Color17a, Properties.Settings.Default.Color17b, Properties.Settings.Default.Color17c)){ Opacity = 1 };
            navb3.Background = new SolidColorBrush(Color.FromRgb(Properties.Settings.Default.Color17a, Properties.Settings.Default.Color17b, Properties.Settings.Default.Color17c)) { Opacity = 1 };

        }
        //Set button hover color
        private void rngNavButton_MouseEnter(object sender, MouseEventArgs e)
        {

            Button button1 = (Button)sender;
            if (disabledButton != button1.Name)
            {
                button1.Background = new SolidColorBrush(Color.FromRgb(Properties.Settings.Default.Color18a, Properties.Settings.Default.Color18b, Properties.Settings.Default.Color18c));
            }
        }

        //Reset button hover color on mouse leave
        private void rngNavButton_MouseLeave(object sender, MouseEventArgs e)
        {
            Button button1 = (Button)sender;

            if (disabledButton != button1.Name)
            {
                button1.Background = new SolidColorBrush(Color.FromRgb(Properties.Settings.Default.Color17a, Properties.Settings.Default.Color17b, Properties.Settings.Default.Color17c));
            }
        }
    }
}
