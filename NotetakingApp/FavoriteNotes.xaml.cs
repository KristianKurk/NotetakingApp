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
using BLL;
using Database;

namespace NotetakingApp
{
    /// <summary>
    /// Interaction logic for FavoriteNotes.xaml
    /// <Button Style="{StaticResource InformButton}" Name="favoritenote3"  Content="Favorite Note 3" ClickMode="Press" />
    /// </summary>
    public partial class FavoriteNotes : Page
    {
        public FavoriteNotes()
        {
            InitializeComponent();
            List<Note> favNotes = DB.GetNotes().Where(x => x.is_favorite == 1).ToList();
            NotesStackPanel.Children.Clear();
            if (favNotes != null) {
                foreach (Note n in favNotes) {
                    AddNoteButton(n);
                }
            }
        }

        private void AddNoteButton(Note n)
        {
            Button btn = new Button();
            btn.Style = FindResource("InformButton") as Style;
            TextBlock tb = new TextBlock { Text = n.note_title };
            btn.ClickMode = ClickMode.Press;
            btn.Name = "btn"+n.note_id;
            btn.Content = tb;
            btn.Click += GoToNote;
            NotesStackPanel.Children.Add(btn);
        }

        private void GoToNote(object sender, RoutedEventArgs e)
        {
            Note n = DB.GetNote(int.Parse(((Button)(sender)).Name.Substring(3)));
            Properties.Settings.Default.currentNote = n;
            foreach (Window window in Application.Current.Windows.OfType<MainWindow>())
            {
                ((MainWindow)window).main.Content = new Note_takingWindow();
                ((MainWindow)window).disabledButton = "noteNavButton";
                ((MainWindow)window).DisableButton("noteNavButton");
            }
        }
    }
}
