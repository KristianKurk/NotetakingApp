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
            //Connection.CreateNewCampaign("AwesomeLand");




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
            Button[] buttons = {
                (Button)grid.FindName("noteNavButton"),
                (Button)grid.FindName("rngNavButton"),
                (Button)grid.FindName("mapNavButton"),
                (Button)grid.FindName("mainNavButton"),
                (Button)grid.FindName("settingsNavButton")
                };
            foreach (Button b in buttons) {
                b.IsEnabled = true;
                b.Background = new SolidColorBrush(Color.FromRgb(255, 193, 140));
            }
        }

        //Top Nav Bar / Custom Window
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

       
        
    }
}
