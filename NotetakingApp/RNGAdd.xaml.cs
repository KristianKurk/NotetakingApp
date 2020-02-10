using System;
using System.Collections.Generic;
using System.IO;
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
using BLL;
using Database;
using Microsoft.Win32;

namespace NotetakingApp
{
    /// <summary>
    /// Interaction logic for RNGAdd.xaml
    /// </summary>
    public partial class RNGAdd : Page
    {
        public RNGAdd()
        {
            InitializeComponent();
        }

        //Open file
        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Rich Text Format (*.rtf)|*.rtf|Text Format (*.txt)|*.txt|All files (*.*)|*.*";
            if (dlg.ShowDialog() == true)
            {
                //Crash if file being used by something
                //Can't open same file twice in a row, crashes

                //Same thing as notetaking
                try {
                FileStream fileStream = new FileStream(dlg.FileName, FileMode.Open);
                TextRange range = new TextRange(rngTB.Document.ContentStart, rngTB.Document.ContentEnd);
                range.Load(fileStream, DataFormats.Rtf);
                }
                catch (IOException ee)
                {
                    Console.WriteLine("Attempted to open same file twice.");
                }
            }
        }
        private void SaveText(object sender, RoutedEventArgs e)
        {

            TextRange textRange = new TextRange(
                 // TextPointer to the start of content in the RichTextBox.
                rngTB.Document.ContentStart,
                // TextPointer to the end of content in the RichTextBox.
                rngTB.Document.ContentEnd
            );       

            string text = textRange.Text;
            string title = rngTitle.Text;

            if (text != "" && title != "")
            {
                text = text.Replace("\r\n", "").Replace("\n", "").Replace("\r", "");
                RandomGenerator rng = new RandomGenerator();
                rng.rng_title = title;
                rng.rng_content = text;
                DB.Add(rng);
            }
        }
    }
}
