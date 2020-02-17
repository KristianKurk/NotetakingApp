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
using System.Windows.Controls.Primitives;

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

            MaxZoomInput.Text = Properties.Settings.Default.MaxZoom.ToString();
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
            bool? isCool = saveFileDialog.ShowDialog();

            string path = saveFileDialog.FileName;
            if (path != null && path != "" && isCool == true)
            {
                string myFile = Directory.GetCurrentDirectory() + "/DB" + Connection.GetActiveCampaignDirectory() + ".db";
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
                List<Note> notes = DB.GetNotes().Where(x => x.category_id == noteCategory.category_id).ToList();

                foreach (Note n in notes) {
                    string rtfText = n.note_content;
                    byte[] byteArray = Encoding.ASCII.GetBytes(rtfText);
                    nc1.AppendText(n.note_title+": ");
                    nc1.AppendText(Environment.NewLine);
                    using (MemoryStream ms = new MemoryStream(byteArray))
                    {
                        TextRange tr = new TextRange(nc1.Document.ContentEnd, nc1.Document.ContentEnd);
                        tr.Load(ms, DataFormats.Rtf);
                    }
                    nc1.AppendText(Environment.NewLine);
                    nc1.AppendText(Environment.NewLine);
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Rich Text Format (*.rtf)|*.rtf";
                saveFileDialog.Title = "Save Note Category as:";
                saveFileDialog.FileName = noteCategory.category_title;
                if (saveFileDialog.ShowDialog() == true)
                {
                    FileStream fileStream = new FileStream(saveFileDialog.FileName, FileMode.Create);
                    TextRange range = new TextRange(nc1.Document.ContentStart, nc1.Document.ContentEnd);
                    range.Save(fileStream, DataFormats.Rtf);
                }
            }
        }

        private void NoteSaveFile(object sender, RoutedEventArgs e)
        {
            if (NoteFilteredListBox.SelectedItem != null)
            {
                Note note = NoteFilteredListBox.SelectedItem as Note;

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Rich Text File (*.rtf)|*.rtf";
                saveFileDialog.Title = "Save Note as:";
                saveFileDialog.FileName = note.note_title;
                saveFileDialog.ShowDialog();
                File.WriteAllText(saveFileDialog.FileName, note.note_content);
            }
        }

        private void MapSaveFile(object sender, RoutedEventArgs e)
        {
            if (MapFilteredListBox.SelectedItem != null)
            {
                Map map = MapFilteredListBox.SelectedItem as Map;
                BitmapImage img = map.LoadImage();
                UIElement elt = ((MappingWindow)attempt.Content).mapCanvas;

                PresentationSource source = PresentationSource.FromVisual(elt);
                RenderTargetBitmap rtb = new RenderTargetBitmap((int)map.LoadImage().PixelWidth,
                      (int)map.LoadImage().PixelHeight, 96, 96, PixelFormats.Default);

                VisualBrush sourceBrush = new VisualBrush(elt);
                DrawingVisual drawingVisual = new DrawingVisual();
                DrawingContext drawingContext = drawingVisual.RenderOpen();
                using (drawingContext)
                {
                    drawingContext.DrawRectangle(sourceBrush, null, new Rect(new Point(0, 0),
                          new Point(elt.RenderSize.Width, elt.RenderSize.Height)));
                }
                rtb.Render(drawingVisual);

                PngBitmapEncoder pngImage = new PngBitmapEncoder();
                pngImage.Frames.Add(BitmapFrame.Create(rtb));

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Image file (*.png)|*.png";
                saveFileDialog.Title = "Save Map as:";
                saveFileDialog.FileName = map.map_name;
                if (saveFileDialog.ShowDialog() == true)
                {
                    using (Stream fileStream = File.Create(saveFileDialog.FileName))
                    {
                        pngImage.Save(fileStream);
                    }
                }
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
            else
            {
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

        private void Change_Map(object sender, RoutedEventArgs e)
        {
            MapChangePopup.IsOpen = false;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select a Map Image File";
            openFileDialog.CheckFileExists = true;
            openFileDialog.CheckPathExists = true;
            openFileDialog.ReadOnlyChecked = true;
            openFileDialog.ShowReadOnly = true;
            openFileDialog.Filter = "Image files (*.png;*.jpeg)|*.png;*.jpeg|All files (*.*)|*.*";
            var result = openFileDialog.ShowDialog();
            Map dbMap = DB.GetMap(1);
            if (openFileDialog.FileName != "")
            {
                byte[] buffer = File.ReadAllBytes(openFileDialog.FileName);
                dbMap.map_file = buffer;
                dbMap.map_name = System.IO.Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                DB.Update(dbMap);
                List<Map> maps = DB.GetMaps();
                List<Pin> pins = DB.getPins();
                foreach (Pin pin in pins)
                    DB.DeletePin(pin.pin_id);

                for (int i = 1; i < maps.Count; i++)
                    DB.DeleteMap(maps[i].map_id);
            }
        }

        private void ShowMapChangePopup(object sender, RoutedEventArgs e)
        {
            MapChangePopup.IsOpen = true;
        }

        private void HideMapChangePopup(object sender, RoutedEventArgs e)
        {
            MapChangePopup.IsOpen = false;
        }

        private void HideNamePopup(object sender, RoutedEventArgs e)
        {
            NamePopup.IsOpen = false;
        }

        private void ShowNamePopup(object sender, RoutedEventArgs e)
        {
            NamePopup.IsOpen = true;
        }

        private void ChangeMapName(object sender, RoutedEventArgs e)
        {
            string newName = NewName.Text;

            if (newName.Length > 0)
            {
                Connection.SetActiveCampaignName(newName);
                NamePopup.IsOpen = false;
            }
        }

        private void MapFilteredListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MapFilteredListBox.SelectedItem != null)
            {
                MappingWindow window = ((MappingWindow)(attempt.Content));
                Map map = MapFilteredListBox.SelectedItem as Map;
                window.LoadMap(map);
                
                window.mapCanvas.Height = map.LoadImage().Height;
                window.mapCanvas.Width = map.LoadImage().Width;
                Canvas.SetLeft(window.imgSource,0);
                Canvas.SetTop(window.imgSource, 0);
                window.imgSource.Width = window.mapCanvas.Width;
                window.imgSource.Height = window.mapCanvas.Height;
            }
        }
        private void ColorScribe()
        {
            //XAML Colors

            Properties.Settings.Default.Color1 = "#563232";
            Properties.Settings.Default.Color2 = "#ffe5cf";
            Properties.Settings.Default.Color3 = "#332323";
            Properties.Settings.Default.Color4 = "#755454";
            Properties.Settings.Default.Color5 = "#f5d2b5";
            Properties.Settings.Default.Color6 = "#b38d6d";
            Properties.Settings.Default.Color7 = "#ffecdb";
            Properties.Settings.Default.Color8 = "#d6c0ae";
            Properties.Settings.Default.Color9 = "#806650";
            Properties.Settings.Default.Color10 = "#876b54";
            Properties.Settings.Default.Color11 = "#cca17c";
            Properties.Settings.Default.Color12 = "#debb9e";
            Properties.Settings.Default.Color13 = "#fccba2";
            Properties.Settings.Default.Color14 = "#fcf4ed";
            Properties.Settings.Default.Color15 = "#ffdf5c";

            //C# Colors

            Properties.Settings.Default.Color16a = 255;
            Properties.Settings.Default.Color16b = 86;
            Properties.Settings.Default.Color16c = 50;
            Properties.Settings.Default.Color16d = 50;
            Properties.Settings.Default.Color17a = 255;
            Properties.Settings.Default.Color17b = 229;
            Properties.Settings.Default.Color17c = 207;
            Properties.Settings.Default.Color18a = 250;
            Properties.Settings.Default.Color18b = 238;
            Properties.Settings.Default.Color18c = 227;


            foreach (Window window in Application.Current.Windows.OfType<MainWindow>())
                ((MainWindow)window).OpenSettings();
        }

        private void ColorVampire() 
        {
            //XAML Colors

            Properties.Settings.Default.Color1 = "#474747";
            Properties.Settings.Default.Color2 = "#d1d1d1";
            Properties.Settings.Default.Color3 = "#2e2e2e";
            Properties.Settings.Default.Color4 = "#757575";
            Properties.Settings.Default.Color5 = "#ebebeb";
            Properties.Settings.Default.Color6 = "#a6a6a6";
            Properties.Settings.Default.Color7 = "#f7f7f7";
            Properties.Settings.Default.Color8 = "#d4d4d4";
            Properties.Settings.Default.Color9 = "#d1d1d1";
            Properties.Settings.Default.Color10 = "#858585";
            Properties.Settings.Default.Color11 = "#bfbfbf";
            Properties.Settings.Default.Color12 = "#cfcfcf";
            Properties.Settings.Default.Color13 = "#b8b7b6";
            Properties.Settings.Default.Color14 = "#f2f2f2";
            Properties.Settings.Default.Color15 = "#c9c9c9";

            //C# Colors

            Properties.Settings.Default.Color16a = 255;
            Properties.Settings.Default.Color16b = 71;
            Properties.Settings.Default.Color16c = 71;
            Properties.Settings.Default.Color16d = 71;
            Properties.Settings.Default.Color17a = 209;
            Properties.Settings.Default.Color17b = 209;
            Properties.Settings.Default.Color17c = 209;
            Properties.Settings.Default.Color18a = 242;
            Properties.Settings.Default.Color18b = 242;
            Properties.Settings.Default.Color18c = 242;

            foreach (Window window in Application.Current.Windows.OfType<MainWindow>())
                ((MainWindow)window).OpenSettings();
        }

        private void ChangeColor(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem item = colorCombo.SelectedItem as ComboBoxItem;
            string name = item.Name;

            switch (name) {
                case "Scribe":
                    ColorScribe();
                    break;
                case "Vampire":
                    ColorVampire();
                    break;
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return) {
                SetMaxZoom();
            }
        }

        private void SetMaxZoom() {
            float newZoom = float.Parse(MaxZoomInput.Text);
            if (newZoom < 1 && newZoom > 0)
                Properties.Settings.Default.MaxZoom = float.Parse(MaxZoomInput.Text);
        }

        private void MaxZoomInput_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SetMaxZoom();
        }
    }
}

