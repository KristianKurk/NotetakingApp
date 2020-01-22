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
using System.Windows.Shapes;

namespace NotetakingApp
{
    /// <summary>
    /// Interaction logic for CampaignSelectWindow.xaml
    /// </summary>
    public partial class CampaignSelectWindow : Window
    {
        bool normalSize = true;
        
        public CampaignSelectWindow()
        {
          
            InitializeComponent();
           

            this.Top = Properties.Settings.Default.Top;
            this.Left = Properties.Settings.Default.Left;
            this.Height = Properties.Settings.Default.Height;
            this.Width = Properties.Settings.Default.Width;

            this.WindowState = Properties.Settings.Default.MaxMin;

            Console.WriteLine("CampaignSelect");
            Console.WriteLine("Width:"+ this.Width);
            Console.WriteLine("Height:" + this.Height);
            Console.WriteLine("Maximized?:" + Properties.Settings.Default.Maximized);
            

            if (Properties.Settings.Default.Maximized==true)
            {            
                this.WindowState = System.Windows.WindowState.Maximized;   
            }



            if (this.WindowState == System.Windows.WindowState.Maximized)
            {
                SetRestoreDownIcon();
            }

            Console.WriteLine("Maximized?:" + Properties.Settings.Default.Maximized);

            //WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;

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
                Properties.Settings.Default.Maximized = true;
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
                this.WindowState = System.Windows.WindowState.Normal;
                SetMaximizeIcon();
                this.DragMove();

            }
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

            //main.Content = new CampaignSelector();

            var newForm = new MainWindow(); //create your new window.
            newForm.Show(); //show the new window.
            this.Close(); //close the current window.
        }

        private void SetMaximizeIcon()
        {
            Properties.Settings.Default.Maximized = false;
            Image icon = switchIcon;
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri("Assets/Navigation/maximize.png", UriKind.Relative);
            bitmap.EndInit();
            icon.Source = bitmap;
        }

        private void SetRestoreDownIcon()
        {
            Properties.Settings.Default.Maximized = true;
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


    }
}
