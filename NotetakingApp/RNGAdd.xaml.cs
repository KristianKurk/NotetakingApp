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
        private bool isEditing;
        private RandomGenerator currentRNG;

        public RNGAdd()
        {
            InitializeComponent();
            LoadRNGLists();
        }

        private void LoadRNGLists()
        {
            RNGLists.Children.Clear();
            List<RandomGenerator> rngList = DB.getRandomGenerators();
            foreach (RandomGenerator rng in rngList) {
                Border border = new Border();
                border.Style = FindResource("BorderHover") as Style;
                border.Name = "rng"+rng.rng_id;
                border.MouseLeftButtonDown += ClickRNG;
                border.MouseRightButtonDown += OpenCM;
                border.Child = new TextBlock() { HorizontalAlignment = HorizontalAlignment.Center, Text = rng.rng_title };
                RNGLists.Children.Add(border);
            }
        }

        private void OpenCM(object sender, MouseButtonEventArgs e)
        {
            ContextMenu cm = this.FindResource("deleteCM") as ContextMenu;
            cm.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
            ((MenuItem)cm.Items[0]).Name = ((Border)(sender)).Name;
            cm.IsOpen = true;
        }

        private void ClickRNG(object sender, MouseButtonEventArgs e)
        {
            if (!isEditing)
            {
                isEditing = true;
                RandomGenerator rng = DB.GetRandomGenerator(int.Parse(((Border)(sender)).Name.Substring(3)));
                currentRNG = rng;
                rngTitle.Text = rng.rng_title;

                string rtfText = rng.rng_content;
                byte[] byteArray = Encoding.ASCII.GetBytes(rtfText);
                using (MemoryStream ms = new MemoryStream(byteArray))
                {
                    TextRange tr = new TextRange(rngTB.Document.ContentStart, rngTB.Document.ContentEnd);
                    tr.Load(ms, DataFormats.Rtf);
                }
            }
            else {
                isEditing = false;
                currentRNG = null;
                rngTitle.Text = "";
                rngTB.Document.Blocks.Clear();
            }
        }

        //Open file
        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Text Format (*.txt)|*.txt|All files (*.*)|*.*";
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

                if (!isEditing)
                {
                    RandomGenerator rng = new RandomGenerator();
                    rng.rng_title = title;
                    rng.rng_content = text;
                    DB.Add(rng);
                    LoadRNGLists();
                    rngTB.Document.Blocks.Clear();
                    rngTitle.Text = "";
                }
                else {
                    currentRNG.rng_title = rngTitle.Text;
                    currentRNG.rng_content = text;
                    DB.Update(currentRNG);
                }
            }
        }

        private void DeleteRNG(object sender, RoutedEventArgs e)
        {
            if (int.Parse(((MenuItem)(sender)).Name.Substring(3)) == currentRNG.rng_id) {
                isEditing = false;
                currentRNG = null;
                rngTitle.Text = "";
                rngTB.Document.Blocks.Clear();
            }

            DB.DeleteRandomGenerator(int.Parse(((MenuItem)(sender)).Name.Substring(3)));
            LoadRNGLists();
        }
    }
}
