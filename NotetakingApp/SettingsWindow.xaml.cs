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
using System.ComponentModel;
using Database;
using BLL;
using System.IO;
using Microsoft.Win32;

namespace NotetakingApp
{
    public partial class SettingsWindow : Page
    {
        private List<RandomGenerator> RNGfilteredList;
        private List<NoteCategory> noteCategoryfilteredList;
        private List<Map> mapfilteredList;
        private List<Note> notefilteredList;

        public SettingsWindow()
        {
            InitializeComponent();

            RNGfilteredList = DB.getRandomGenerators();
            RNGfilteredListBox.ItemsSource = RNGfilteredList;
            notefilteredList = DB.GetNotes();
            NoteFilteredListBox.ItemsSource = notefilteredList;
            noteCategoryfilteredList = DB.GetNoteCategories();
            NoteCategoryFilteredListBox.ItemsSource = noteCategoryfilteredList;
            mapfilteredList = DB.GetMaps();
            MapFilteredListBox.ItemsSource = mapfilteredList;
        }

        private void Note_Report_Click(object sender, RoutedEventArgs e)
        {
            NoteSearch.Visibility = Visibility.Visible;
        }

        private void RNG_Report_Click(object sender, RoutedEventArgs e)
        {
            RNGSearch.Visibility = Visibility.Visible;
        }

        private void Note_Category_Report_Click(object sender, RoutedEventArgs e)
        {
            NoteCategorySearch.Visibility = Visibility.Visible;
        }

        private void Map_Report_Click(object sender, RoutedEventArgs e)
        {
            MapSearch.Visibility = Visibility.Visible;
        }
        
        //Crashs when you close the file explorer without seleccting anything
        private void Export_Database(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Campaign File (*.*)|*.*";
            saveFileDialog.Title = "Save campaign as:";
            saveFileDialog.FileName = Connection.GetActiveCampaignName();
            saveFileDialog.ShowDialog();

            string path = saveFileDialog.FileName;
            if (path != null && path != "") {
                string myFile = Directory.GetCurrentDirectory() +"/DB" + Connection.GetActiveCampaignDirectory() + ".db";
                string dest = System.IO.Path.Combine(path, "DB" + Connection.GetActiveCampaignDirectory() + ".db");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                File.Copy(myFile, dest);
            }
        }

        private void RNGSaveFile(object sender, RoutedEventArgs e)
        {
            if (RNGfilteredListBox.SelectedItem != null)
            {
                RandomGenerator rng = RNGfilteredListBox.SelectedItem as RandomGenerator;

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Text file (*.txt)|*.txt";
                saveFileDialog.Title = "Save RNG as:";
                saveFileDialog.FileName = rng.rng_title;
                saveFileDialog.ShowDialog();
                File.WriteAllText(saveFileDialog.FileName, rng.rng_content);
            }
        }

        private void NoteCategorySaveFile(object sender, RoutedEventArgs e)
        {
            if (NoteCategoryFilteredListBox.SelectedItem != null)
            {
                NoteCategory noteCategory = NoteCategoryFilteredListBox.SelectedItem as NoteCategory;

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Text file (*.txt)|*.txt";
                saveFileDialog.Title = "Save Note Category as:";
                saveFileDialog.FileName = noteCategory.category_title;
                saveFileDialog.ShowDialog();
                File.WriteAllText(saveFileDialog.FileName, noteCategory.category_title);
                //Logic goes here for note category.
            }
        }

        private void NoteSaveFile(object sender, RoutedEventArgs e)
        {
            if (NoteFilteredListBox.SelectedItem != null)
            {
                Note note = NoteFilteredListBox.SelectedItem as Note;

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Text file (*.txt)|*.txt";
                saveFileDialog.Title = "Save Note as:";
                saveFileDialog.FileName = note.note_title;
                saveFileDialog.ShowDialog();
                File.WriteAllText(saveFileDialog.FileName, note.note_content);
                //Logic goes here for note.
            }
        }

        private void MapSaveFile(object sender, RoutedEventArgs e)
        {
            if (MapFilteredListBox.SelectedItem != null)
            {
                Map map = MapFilteredListBox.SelectedItem as Map;

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Text file (*.txt)|*.txt";
                saveFileDialog.Title = "Save Map as:";
                saveFileDialog.FileName = map.map_name;
                saveFileDialog.ShowDialog();
                File.WriteAllText(saveFileDialog.FileName, map.map_file.ToString());
                //Logic goes here for Map.
            }
        }

        private void RNG_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (RNGSearchBar.Text != null && RNGSearchBar.Text != "" && DB.getRandomGenerators() != null)
            {
                RNGfilteredList.Clear();
                foreach (RandomGenerator rng in DB.getRandomGenerators())
                {
                    if (rng.rng_title.ToUpper().StartsWith(RNGSearchBar.Text.ToUpper()))
                    {
                        RNGfilteredList.Add(rng);
                    }
                }
                Console.WriteLine(RNGfilteredList.Count);
                RNGfilteredListBox.ItemsSource = null;
                RNGfilteredListBox.ItemsSource = RNGfilteredList;
            }
            else {
                RNGfilteredList = DB.getRandomGenerators();
                RNGfilteredListBox.ItemsSource = null;
                RNGfilteredListBox.ItemsSource = RNGfilteredList;
            }
        }

        private void NoteCategory_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (NoteCategorySearchBar.Text != null && NoteCategorySearchBar.Text != "" && DB.GetNoteCategories() != null)
            {
                noteCategoryfilteredList.Clear();
                foreach (NoteCategory NoteCategory in DB.GetNoteCategories())
                {
                    if (NoteCategory.category_title.ToUpper().StartsWith(NoteCategorySearchBar.Text.ToUpper()))
                    {
                        noteCategoryfilteredList.Add(NoteCategory);
                    }
                }
                NoteCategoryFilteredListBox.ItemsSource = null;
                NoteCategoryFilteredListBox.ItemsSource = noteCategoryfilteredList;
            }
            else
            {
                noteCategoryfilteredList = DB.GetNoteCategories();
                NoteCategoryFilteredListBox.ItemsSource = null;
                NoteCategoryFilteredListBox.ItemsSource = noteCategoryfilteredList;
            }
        }

        private void Note_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (NoteSearchBar.Text != null && NoteSearchBar.Text != "" && DB.GetNotes() != null)
            {
                notefilteredList.Clear();
                foreach (Note note in DB.GetNotes())
                {
                    if (note.note_title.ToUpper().StartsWith(NoteSearchBar.Text.ToUpper()))
                    {
                        notefilteredList.Add(note);
                    }
                }
                NoteFilteredListBox.ItemsSource = null;
                NoteFilteredListBox.ItemsSource = notefilteredList;
            }
            else
            {
                notefilteredList = DB.GetNotes();
                NoteFilteredListBox.ItemsSource = null;
                NoteFilteredListBox.ItemsSource = notefilteredList;
            }
        }

        private void Map_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (MapSearchBar.Text != null && MapSearchBar.Text != "" && DB.GetMaps() != null)
            {
                mapfilteredList.Clear();
                foreach (Map map in DB.GetMaps())
                {
                    if (map.map_name.ToUpper().StartsWith(MapSearchBar.Text.ToUpper()))
                    {
                        mapfilteredList.Add(map);
                    }
                }
                MapFilteredListBox.ItemsSource = null;
                MapFilteredListBox.ItemsSource = mapfilteredList;
            }
            else
            {
                mapfilteredList = DB.GetMaps();
                MapFilteredListBox.ItemsSource = null;
                MapFilteredListBox.ItemsSource = mapfilteredList;
            }
        }

        private void RNGCloseMenu(object sender, RoutedEventArgs e)
        {
            RNGSearch.Visibility = Visibility.Hidden; 
        }

        private void NoteCategoryCloseMenu(object sender, RoutedEventArgs e)
        {
            NoteCategorySearch.Visibility = Visibility.Hidden;
        }

        private void NoteCloseMenu(object sender, RoutedEventArgs e)
        {
            NoteSearch.Visibility = Visibility.Hidden;
        }

        private void MapCloseMenu(object sender, RoutedEventArgs e)
        {
            MapSearch.Visibility = Visibility.Hidden;
        }
    }
}

