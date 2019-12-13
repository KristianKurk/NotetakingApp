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
        public RNGWindow()
        {
            InitializeComponent();
        }
        
        private void BtnAddRNG(object sender, RoutedEventArgs e)
        {
            DisableButton("settingsNavButton");
            rng.Content = new RNGAdd();
        }
        private void BtnGenerate(object sender, RoutedEventArgs e)
        {
            DisableButton("mainNavButton");
            rng.Content = new RNGGenerate();
        }
        private void DisableButton(string btn)
        {

            EnableAll();

            //Disable Selected button

            object button = rngGrid.FindName(btn);
            Button button1 = (Button)button;
            button1.Background = new SolidColorBrush(Color.FromArgb(255, 86, 50, 50));

            //  button1.IsEnabled = false;
            button1.Focusable = false;
            button1.IsEnabled = false;



        }
        private void EnableAll()
        {

            //Enable all buttons

            object button1 = rngGrid.FindName("noteNavButton");
            object button2 = rngGrid.FindName("rngNavButton");
            object button3 = rngGrid.FindName("mapNavButton");
            object button4 = rngGrid.FindName("mainNavButton");
            object button5 = rngGrid.FindName("settingsNavButton");

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

            navb1.Background = new SolidColorBrush(Color.FromRgb(255, 193, 140));
            navb2.Background = new SolidColorBrush(Color.FromRgb(255, 193, 140));
            navb3.Background = new SolidColorBrush(Color.FromRgb(255, 193, 140));
            navb4.Background = new SolidColorBrush(Color.FromRgb(255, 193, 140));
            navb5.Background = new SolidColorBrush(Color.FromRgb(255, 193, 140));
        }
    }
}
