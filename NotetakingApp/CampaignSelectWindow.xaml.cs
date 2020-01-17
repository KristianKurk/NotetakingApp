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
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
          //  var h = ((MainWindow)Application.Current.MainWindow).ActualHeight;
           // var w = ((MainWindow)Application.Current.MainWindow).ActualWidth;

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
            
            //main.Content = new CampaignSelector();

            var newForm = new MainWindow(); //create your new window.
            newForm.Show(); //show the new window.
            this.Close(); //close the current window.
        }

        private void SetMaximizeIcon()
        {
            Image icon = switchIcon;
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri("Assets/Navigation/maximize.png", UriKind.Relative);
            bitmap.EndInit();
            icon.Source = bitmap;
        }

        private void SetRestoreDownIcon()
        {
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
