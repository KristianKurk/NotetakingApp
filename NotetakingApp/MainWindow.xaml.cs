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
using System.Windows.Controls.Primitives;

namespace NotetakingApp
{
    /// <summary>
    /// This is a test
    /// </summary>
    public partial class MainWindow : Window
    {
        bool normalSize = true;
        public string disabledButton;

        public MainWindow()
        {
            InitializeComponent();
            // WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;

            //Set window size to previous window size
            this.Top = Properties.Settings.Default.Top;
            this.Left = Properties.Settings.Default.Left;
            this.Height = Properties.Settings.Default.Height;
            this.Width = Properties.Settings.Default.Width;

            Console.WriteLine("Main");
            Console.WriteLine("Width:" + this.Width);
            Console.WriteLine("Height:" + this.Height);
            Console.WriteLine("Maximized?:" + Properties.Settings.Default.Maximized);

            if (Properties.Settings.Default.Maximized==true)
            {
                this.WindowState = WindowState.Maximized;
            }
            if (this.WindowState == System.Windows.WindowState.Maximized)
            {
                SetRestoreDownIcon();
            }

            Properties.Settings.Default.currentNote = null;
            mainNavButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                // Use the RestoreBounds as the current values will be 0, 0 and the size of the screen
                Properties.Settings.Default.Top = RestoreBounds.Top;
                Properties.Settings.Default.Left = RestoreBounds.Left;
                Properties.Settings.Default.Height = RestoreBounds.Height;
                Properties.Settings.Default.Width = RestoreBounds.Width;
                Properties.Settings.Default.Maximized = true;
                Properties.Settings.Default.MaxMin = System.Windows.WindowState.Maximized;
            }
            else
            {
                Properties.Settings.Default.Top = this.Top;
                Properties.Settings.Default.Left = this.Left;
                Properties.Settings.Default.Height = this.Height;
                Properties.Settings.Default.Width = this.Width;
                Properties.Settings.Default.Maximized = false;
                Properties.Settings.Default.MaxMin = System.Windows.WindowState.Normal;
            }

            Properties.Settings.Default.Save();
        }

        private void BtnClickMain(object sender, RoutedEventArgs e)
        {

            disabledButton ="mainNavButton";
            DisableButton("mainNavButton");
            main.Content = new MainMenu();
        }

        private void BtnClickMap(object sender, RoutedEventArgs e)
        {
            disabledButton = "mapNavButton";
            DisableButton("mapNavButton");
            main.Content = new MappingWindow();
        }
        private void BtnClickNote(object sender, RoutedEventArgs e)
        {
            disabledButton = "noteNavButton";
            DisableButton("noteNavButton");
            main.Content = new Note_takingWindow();
        }
        private void BtnClickRNG(object sender, RoutedEventArgs e)
        {
            disabledButton = "rngNavButton";
            DisableButton("rngNavButton");
            main.Content = new RNGWindow();
        }

        public void DisableButton(string btn) {

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
                (Button)grid.FindName("mainNavButton")
                
                };
            foreach (Button b in buttons) {
                b.IsEnabled = true;
                b.Background = new SolidColorBrush(Color.FromRgb(255, 229, 207));
            }
        }

        //Top Nav Bar / Custom Window
       
            private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {

          

            if (e.ChangedButton == MouseButton.Left && this.WindowState == System.Windows.WindowState.Normal)
            {
                this.DragMove();
            }
            /*
            else if (System.Windows.Input.Mouse.LeftButton == MouseButtonState.Pressed && this.WindowState == WindowState.Maximized)
            {
                this.WindowState = System.Windows.WindowState.Normal;
                this.DragMove();

            }
            */
            if (e.ClickCount == 2 && this.WindowState != WindowState.Maximized && normalSize == true)
            {
                this.WindowState = WindowState.Maximized;
                
                SetRestoreDownIcon();

            }
            else if (e.ClickCount == 2 && this.WindowState == WindowState.Maximized)
            {
                this.WindowState = System.Windows.WindowState.Normal;
                SetMaximizeIcon();
                this.DragMove();
               
            }


        }
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.WindowState=System.Windows.WindowState.Normal;
                SetMaximizeIcon();
                this.DragMove();

            }
        }
            private void BtnSettings(object sender, RoutedEventArgs e)
        {

            EnableAll();
            main.Content = new SettingsWindow();
           
        }
        private void BtnHelp(object sender, RoutedEventArgs e)
        {

            EnableAll();
            main.Content = new HelpPage();

        }
        private void BtnMin(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void BtnMax(object sender, RoutedEventArgs e)
        {
            //Icon should change depending on window state

            Button button = sender as Button;

            //Maximize window or else turn back to normal

            if (this.WindowState != WindowState.Maximized)
            {
                button.ToolTip = "Restore Down";
                this.WindowState = WindowState.Maximized;
                SetRestoreDownIcon();
            }
            else
            {
                button.ToolTip = "Maximize";
                this.WindowState = System.Windows.WindowState.Normal;
                SetMaximizeIcon();
            }

        }
        private void BtnClose(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void BtnCampaign(object sender, RoutedEventArgs e)
        {
            EnableAll();
            //main.Content = new CampaignSelector();

            if (WindowState == WindowState.Maximized)
            {
                // Use the RestoreBounds as the current values will be 0, 0 and the size of the screen
                Properties.Settings.Default.Top = RestoreBounds.Top;
                Properties.Settings.Default.Left = RestoreBounds.Left;
                Properties.Settings.Default.Height = RestoreBounds.Height;
                Properties.Settings.Default.Width = RestoreBounds.Width;
                Properties.Settings.Default.Maximized = true;
                Properties.Settings.Default.MaxMin = System.Windows.WindowState.Maximized;
            }
            else
            {
                Properties.Settings.Default.Top = this.Top;
                Properties.Settings.Default.Left = this.Left;
                Properties.Settings.Default.Height = this.Height;
                Properties.Settings.Default.Width = this.Width;
                Properties.Settings.Default.Maximized = false;
                Properties.Settings.Default.MaxMin = System.Windows.WindowState.Normal;
            }

            Properties.Settings.Default.Save();
            
            var newForm = new CampaignSelectWindow(); //create your new window.
            newForm.Show(); //show the new window.
            this.Close(); //close the current window.
            
        }

        private void SetMaximizeIcon() {
            Image icon = switchIcon;
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri("Assets/Navigation/maximize.png", UriKind.Relative);
            bitmap.EndInit();
            icon.Source = bitmap;
        }

        private void SetRestoreDownIcon() {
            Image icon = switchIcon;
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri("Assets/Navigation/restoredown.png", UriKind.Relative);
            bitmap.EndInit();
            icon.Source = bitmap;
        }

        //Change icon when window is dragged to the top to maximize
        private void topNav_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.WindowState == System.Windows.WindowState.Maximized)
            {
                SetRestoreDownIcon();
            }
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
