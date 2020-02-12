using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            int diceResult = rnd.Next(1, sides + 1);
            result.Text = diceResult.ToString();
        }

        private void FormulaCalculate(object sender, RoutedEventArgs e)
        {
            string s = formulaBox.Text.Trim().ToLower();
            result.Text = Calculate(s).ToString();
        }

        private int Calculate(string s)
        {
            int t = 0;
            Random r = new Random();
            var a = s.Split('+');

            if (a.Count() > 1)
                foreach (var b in a)
                    t += Calculate(b);
            else
            {
                var m = a[0].Split('*');

                if (m.Count() > 1)
                {
                    t = 1;

                    foreach (var n in m)
                        t *= Calculate(n);
                }
                else
                {
                    var d = m[0].Split('d');

                    if (!int.TryParse(d[0].Trim(), out t))
                        t = 0;

                    int f;

                    for (int i = 1; i < d.Count(); i++)
                    {
                        if (!int.TryParse(d[i].Trim(), out f))
                            f = 6;

                        int u = 0;

                        for (int j = 0; j < (t == 0 ? 1 : t); j++)
                            u += r.Next(0, f);

                        t += u;
                    }
                }
            }
            return t;
        }
    }
}
