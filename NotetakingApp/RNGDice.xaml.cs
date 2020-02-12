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
    /// Interaction logic for RNGDice.xaml
    /// </summary>
    public partial class RNGDice : Page
    {
        Random rnd;

        public RNGDice()
        {
            InitializeComponent();
            rnd = new Random();
        }

        private void diceRoll(object sender, RoutedEventArgs e)
        {
            Button dicebtn = sender as Button;
            int sides = int.Parse(dicebtn.Name.Substring(1));
            int diceResult = rnd.Next(1, sides+1);
            result.Text = diceResult.ToString();
        }

        private void FormulaCalculate(object sender, RoutedEventArgs e)
        {
            string text = formulaBox.Text.Trim().ToUpper();

            string[] parts = text.Split('D');
        }
    }
}
