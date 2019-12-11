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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        

        private void BtnClickMap(object sender, RoutedEventArgs e)
        {
            main.Content = new MappingWindow();
        }
        private void BtnClickNote(object sender, RoutedEventArgs e)
        {
            main.Content = new Note_takingWindow();
        }
        private void BtnClickRNG(object sender, RoutedEventArgs e)
        {
            DisableButton("");
            main.Content = new RNGWindow();
        }

        private void DisableButton(string btn) {

        }

    }
}
