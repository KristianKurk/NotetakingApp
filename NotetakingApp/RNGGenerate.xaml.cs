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
using System.Text.RegularExpressions;

namespace NotetakingApp
{
    /// <summary>
    /// Interaction logic for RNGGenerate.xaml
    /// </summary>
    public partial class RNGGenerate : Page
    {
        public RNGGenerate()
        {
            InitializeComponent();
            rngCombo.SelectedIndex = 0;
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        private void GenerateData(object sender, RoutedEventArgs e)
        {


        }
    }
}
