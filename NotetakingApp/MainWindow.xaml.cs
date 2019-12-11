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

namespace NotetakingApp
{
    /// <summary>
    /// This is a test
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DB.Add(new Note() {note_title="New Note",note_content="New Content"});
            DB.DeleteNote(6);
            DB.Add(new Note() { note_title = "New Note 2", note_content = "New Content 2" });


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
