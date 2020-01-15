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
using Database;
using BLL;

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
            rngCombo.ItemsSource = DB.getRandomGenerators();
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        private void GenerateData(object sender, RoutedEventArgs e)
        {
            if (DB.getRandomGenerators().Count() > 0) {
                RandomGenerator rng = rngCombo.SelectedItem as RandomGenerator;
                List<String> options = rng.rng_content.Split(',').ToList();
                foreach (string s in options)
                    s.Trim();

                int number = 0;
                try
                {
                    if (int.Parse(NumberTextBox.Text.Trim()) < options.Count)
                        number = int.Parse(NumberTextBox.Text.Trim());
                    else
                        number = options.Count;
                }
                catch (FormatException exc) {
                    Console.WriteLine(exc.Message);
                }
                Random random = new Random();

                string answer = "";
                for (int i = 0; i < number; i++) {
                    int index = random.Next(options.Count);
                    answer = answer + " " + options[index];
                    options.RemoveAt(index);
                }

                displayText.Text = answer;
                Console.WriteLine(answer);
            }
        }
    }
}
