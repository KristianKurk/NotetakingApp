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
using Database;
using BLL;

namespace NotetakingApp
{
    /// <summary>
    /// This is a test
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            /*
            DB.Add(new Note() {note_title="New Note",note_content="New Content"});
            DB.DeleteNote(6);
            DB.Add(new Note() { note_title = "New Note 2", note_content = "New Content 2" });
            */

        }
        private void BtnClickSettings(object sender, RoutedEventArgs e)
        {
            DisableButton("settingsNavButton");
            main.Content = new SettingsWindow();
        }
        private void BtnClickMain(object sender, RoutedEventArgs e)
        {
            DisableButton("mainNavButton");
            main.Content = new MainMenu();
        }

        private void BtnClickMap(object sender, RoutedEventArgs e)
        {
            DisableButton("mapNavButton");
            main.Content = new MappingWindow();
        }
        private void BtnClickNote(object sender, RoutedEventArgs e)
        {
            DisableButton("noteNavButton");
            main.Content = new Note_takingWindow();
        }
        private void BtnClickRNG(object sender, RoutedEventArgs e)
        {
            DisableButton("rngNavButton");
            main.Content = new RNGWindow();
        }

        private void DisableButton(string btn) {

            EnableAll();

            //Disable Selected button

            object button = grid.FindName(btn);
            Button button1 = (Button)button;
             button1.Background = new SolidColorBrush(Color.FromArgb(255,86, 50, 50));

            //  button1.IsEnabled = false;
            button1.Focusable = false;
            button1.IsEnabled = false;
           
            

        }
        private void EnableAll() {

            //Enable all buttons

            object button1 = grid.FindName("noteNavButton");
            object button2 = grid.FindName("rngNavButton");
            object button3 = grid.FindName("mapNavButton");
            object button4 = grid.FindName("mainNavButton");
            object button5 = grid.FindName("settingsNavButton");

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
