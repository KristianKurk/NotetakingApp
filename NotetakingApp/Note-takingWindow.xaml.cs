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
using System.IO;
using Microsoft.Win32;
using ColorPicker;
using System.Runtime.InteropServices;
using Database;
using BLL;

namespace NotetakingApp
{

    public partial class Note_takingWindow : Page
    {

        //private members being used
        private bool dataChanged = false;
        private string privateText = null;

        private Note openNote;

        public string text {
            get {
                TextRange range = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
                return range.Text;
            }
            set {
                privateText = value;
            }
        }

        public Note_takingWindow()
        {
            InitializeComponent();
            cmbFontFamily.ItemsSource = Fonts.SystemFontFamilies.OrderBy(f => f.Source);
            cmbFontSize.ItemsSource = new List<double>() { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72 };

            AddCategories(DB.GetNoteCategories()[0]);
            AddNotes();

            if (Properties.Settings.Default.currentNote != null)
                openNote = Properties.Settings.Default.currentNote;
        }

        private void rtbEditor_SelectionChanged(object sender, RoutedEventArgs e)
        {
            object temp = rtbEditor.Selection.GetPropertyValue(Inline.FontWeightProperty);
            btnBold.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(FontWeights.Bold));
            temp = rtbEditor.Selection.GetPropertyValue(Inline.FontStyleProperty);
            btnItalic.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(FontStyles.Italic));
            temp = rtbEditor.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            btnUnderline.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(TextDecorations.Underline));

            temp = rtbEditor.Selection.GetPropertyValue(Inline.FontFamilyProperty);
            cmbFontFamily.SelectedItem = temp;
            temp = rtbEditor.Selection.GetPropertyValue(Inline.FontSizeProperty);
            cmbFontSize.Text = temp.ToString();
        }
        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Rich Text Format (*.rtf)|*.rtf|All files (*.*)|*.*";
            if (dlg.ShowDialog() == true)
            {
               //Crash if file being used by something
               //Can't open same file twice in a row, crashes

                FileStream fileStream = new FileStream(dlg.FileName, FileMode.Open);
                TextRange range = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
                range.Load(fileStream, DataFormats.Rtf);
            }
        }
        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Rich Text Format (*.rtf)|*.rtf|All files (*.*)|*.*";
            if (dlg.ShowDialog() == true)
            {
                FileStream fileStream = new FileStream(dlg.FileName, FileMode.Create);
                TextRange range = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
                range.Save(fileStream, DataFormats.Rtf);
            }
        }
        private void cmbFontFamily_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbFontFamily.SelectedItem != null)
                rtbEditor.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, cmbFontFamily.SelectedItem);
        }
        private void cmbFontSize_TextChanged(object sender, TextChangedEventArgs e)
        {
            try { 
            rtbEditor.Selection.ApplyPropertyValue(Inline.FontSizeProperty, cmbFontSize.Text);
        }
            catch (Exception ee) {
                rtbEditor.Selection.ApplyPropertyValue(Inline.FontSizeProperty, "12");
            }
        }
        private void ToolStripButtonTextcolor_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            //colorDialog.Owner = this;
            if ((bool)colorDialog.ShowDialog())
            {
                TextRange range = new TextRange(rtbEditor.Selection.Start,
                    rtbEditor.Selection.End);

                range.ApplyPropertyValue(FlowDocument.ForegroundProperty,
                    new SolidColorBrush(colorDialog.SelectedColor));
            }
        }
        private void ToolStripButtonBackcolor_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            //colorDialog.Owner = this;
            if ((bool)colorDialog.ShowDialog())
            {
                TextRange range = new TextRange(rtbEditor.Selection.Start, rtbEditor.Selection.End);

                range.ApplyPropertyValue(FlowDocument.BackgroundProperty, new SolidColorBrush(colorDialog.SelectedColor));
            }
        }
        private void ToolStripButtonAlignLeft_Click(object sender, RoutedEventArgs e)
        {
            if (ToolStripButtonAlignLeft.IsChecked == true)
            {
                ToolStripButtonAlignCenter.IsChecked = false;
                ToolStripButtonAlignRight.IsChecked = false;
            }
        }
        private void ToolStripButtonAlignCenter_Click(object sender, RoutedEventArgs e)
        {
            if (ToolStripButtonAlignCenter.IsChecked == true)
            {
                ToolStripButtonAlignLeft.IsChecked = false;
                ToolStripButtonAlignRight.IsChecked = false;
            }

        }
        private void ToolStripButtonAlignRight_Click(object sender, RoutedEventArgs e)
        {
            if (ToolStripButtonAlignRight.IsChecked == true)
            {
                ToolStripButtonAlignCenter.IsChecked = false;
                ToolStripButtonAlignLeft.IsChecked = false;
            }

        }
        private void RichTextControl_SelectionChanged(object sender, RoutedEventArgs e)
        {
            // markierten Text holen
            TextRange selectionRange = new TextRange(rtbEditor.Selection.Start, rtbEditor.Selection.End);


            

            

            if (selectionRange.GetPropertyValue(FlowDocument.TextAlignmentProperty).ToString() == "Left")
            {
                ToolStripButtonAlignLeft.IsChecked = true;
            }

            if (selectionRange.GetPropertyValue(FlowDocument.TextAlignmentProperty).ToString() == "Center")
            {
                ToolStripButtonAlignCenter.IsChecked = true;
            }

            if (selectionRange.GetPropertyValue(FlowDocument.TextAlignmentProperty).ToString() == "Right")
            {
                ToolStripButtonAlignRight.IsChecked = true;
            }

          

          
        }
        private void RichTextControl_TextChanged(object sender, TextChangedEventArgs e)
        {
            dataChanged = true;
        }
        public void Clear()
        {
            dataChanged = false;
            rtbEditor.Document.Blocks.Clear();
        }

        private void RtbEditor_TextChanged(object sender, TextChangedEventArgs e)
        {
            string rtfText;
            TextRange tr = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
            using (MemoryStream ms = new MemoryStream())
            {
                tr.Save(ms, DataFormats.Rtf);
                rtfText = Encoding.ASCII.GetString(ms.ToArray());
            }
        
            openNote.note_content = rtfText;
            DB.Update(openNote);
        }

        MenuItem[] regularCM = { new MenuItem { Header = "New Category" }, new MenuItem { Header = "New Note" }, new MenuItem { Header="Delete" } };
        MenuItem[] uncategorisedCM = { new MenuItem { Header = "New Category" }, new MenuItem { Header = "New Note" } };

        private void OpenCMUncategorised(object sender, MouseButtonEventArgs e)
        {
            ContextMenu cm = this.FindResource("cmButton") as ContextMenu;
            cm.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
            SetUncategorisedCM();

            cm.IsOpen = true;
        }

        private void OpenCMRegular(object sender, MouseButtonEventArgs e)
        {
            ContextMenu cm = this.FindResource("cmButton") as ContextMenu;
            cm.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
            SetRegularCM();

            cm.IsOpen = true;
        }

        private void SetRegularCM()
        {
            ContextMenu cm = this.FindResource("cmButton") as ContextMenu;
            cm.ItemsSource = null;
            cm.ItemsSource = regularCM;
        }

        private void SetUncategorisedCM()
        {
            ContextMenu cm = this.FindResource("cmButton") as ContextMenu;
            cm.ItemsSource = null;
            cm.ItemsSource = uncategorisedCM;
        }

        private void CreateNewCategory(NoteCategory noteCategory) {
            TreeViewItem cateTV = new TreeViewItem();
            cateTV.Name = "cate"+noteCategory.category_id;
            Border border = new Border() {Style= FindResource("CategoryHover") as Style };
            border.Child = new TextBlock() { Text = noteCategory.category_title };
            cateTV.Header = border;

            TreeViewItem parent = null;
            foreach (TreeViewItem tvi in treeView.Items)
                if (noteCategory.category_parent == int.Parse(tvi.Name.Substring(4)))
                    parent = tvi;

            parent.Items.Add(cateTV);
        }

        private void CreateNewNote(Note note)
        {
            TreeViewItem noteTV = new TreeViewItem();
            Border border = new Border() { Style = FindResource("BorderHover") as Style };
            border.Child = new TextBlock() { Text = note.note_title };
            noteTV.Header = border;
            noteTV.MouseLeftButtonDown += ClickNote;
            noteTV.MouseRightButtonDown += OpenCMRegular;

            TreeViewItem parent = null;
            foreach (TreeViewItem tvi in treeView.Items)
                if (note.category_id == int.Parse(tvi.Name.Substring(4)))
                    parent = tvi;
            parent.Items.Add(noteTV);
        }

        private void ClickNote(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem tv = sender as TreeViewItem;
            string rtfText = DB.GetNote(int.Parse(tv.Name.Substring(4))).note_content;
            byte[] byteArray = Encoding.ASCII.GetBytes(rtfText);
            using (MemoryStream ms = new MemoryStream(byteArray))
            {
                TextRange tr = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
                tr.Load(ms, DataFormats.Rtf);
            }
        }

        private void AddCategories(NoteCategory noteCategory)
        {
            foreach (NoteCategory nc in DB.GetNoteCategories()) {
                if (nc.category_parent == noteCategory.category_id) {
                    CreateNewCategory(nc);
                    AddCategories(nc);
                }
            } 
        }

        private void AddNotes() {
            List<Note> nList = DB.GetNotes();
            foreach (Note n in nList) {
                CreateNewNote(n);
            }
        }
    }
}