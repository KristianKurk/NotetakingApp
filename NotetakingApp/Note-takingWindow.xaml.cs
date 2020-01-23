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
        private NoteCategory openNC;
        int id;
        List<TextBlock> hiddenNotes;

        public Note_takingWindow()
        {
            InitializeComponent();
            cmbFontFamily.ItemsSource = Fonts.SystemFontFamilies.OrderBy(f => f.Source);
            cmbFontSize.ItemsSource = new List<double>() { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72 };

            UpdateNotes();
            hiddenNotes = new List<TextBlock>();

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



        /*private void OpenCMUncategorised(object sender, MouseButtonEventArgs e)
        {
            MenuItem newCat = new MenuItem { Header = "New Category" };
            newCat.Click += CreateNewCategoryClick;
            MenuItem newNote = new MenuItem { Header = "New Note" };
            newNote.Click += CreateNewNoteClick;
            MenuItem[] uncategorisedCM = { newCat, newNote };
            ContextMenu cm = this.FindResource("cmButton") as ContextMenu;
            cm.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
            cm.ItemsSource = null;
            cm.ItemsSource = uncategorisedCM;

            cm.IsOpen = true;
        }*/

        private void OpenCM(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is TextBlock)
            {
                TextBlock tb = e.OriginalSource as TextBlock;
                id = int.Parse(tb.Name.Substring(4));
                Console.WriteLine("monka"+id);
            }
            else {
                id = 1;
            }

            ContextMenu cm = this.FindResource("cmButton") as ContextMenu;
            cm.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
            cm.IsOpen = true;

        }


        private void CreateNewCategoryClick(object sender, RoutedEventArgs e)
        {
            NoteCategory nc = new NoteCategory();
            nc.category_title = "New Category";
            nc.category_parent = id;
            DB.Add(nc);
            UpdateNotes();
        }

        private void CreateNewNoteClick(object sender, RoutedEventArgs e)
        {
            Note note = new Note();
            string rtfText = "{\\rtf1\\ansi\\ansicpg1252\\uc1\\htmautsp\\deff2{\\fonttbl{\\f0\\fcharset0 Times New Roman;}{\\f2\\fcharset0 Segoe UI;}}{\\colortbl\\red0\\green0\\blue0;\\red255\\green255\\blue255;}\\loch\\hich\\dbch\\pard\\plain\\ltrpar\\itap0{\\lang1033\\fs18\\f2\\cf0 \\cf0\\ql{\\f2 \\li0\\ri0\\sa0\\sb0\\fi0\\ql\\par}}}";
            note.note_title = "New Note";
            note.category_id = id;
            note.note_content = rtfText;
            DB.Add(note);
            UpdateNotes();
        }

        private void DeleteClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void CreateNewCategory(NoteCategory noteCategory) {
            TextBlock tb = new TextBlock();
            tb.Name = "cate"+noteCategory.category_id;
            tb.Text = noteCategory.category_id+" meme";
            tb.Focusable = true;
            tb.Background = Brushes.Red;
            tb.MouseLeftButtonDown += OpenCloseCategory;

            TextBlock parent = null;
            foreach (TextBlock tb1 in UnCategorised.Children)
                if (tb1.Name == "cate" + noteCategory.category_parent)
                    parent = tb1;
            tb.Tag = parent;

            if (UnCategorised.Children.Count > 0)
            {
                int count = 0;
                TextBlock loopTB = tb;
                while (loopTB.Tag != null) {
                    count++;
                    loopTB = loopTB.Tag as TextBlock;
                }
                tb.Margin = new Thickness(count * 10, 0, 0, 0);
                UnCategorised.Children.Insert(UnCategorised.Children.IndexOf(parent) + 1, tb);
            }
            else
            {
                UnCategorised.Children.Add(tb);
            }
        }

        private void OpenCloseCategory(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock)
            {
                TextBlock tb = sender as TextBlock;
                List<TextBlock> visibleChildren = new List<TextBlock>();
                List<TextBlock> hiddenChildren = new List<TextBlock>();
                GetVisibleChildren(tb, visibleChildren);
                GetHiddenChildren(tb, hiddenChildren);

                if (visibleChildren.Count > 0)
                    MakeHidden(visibleChildren);
                else if (hiddenChildren.Count > 0)
                {
                    hiddenChildren.Reverse();
                    MakeVisible(hiddenChildren);
                }
            }
        }

        private void GetHiddenChildren(TextBlock tb, List<TextBlock> children)
        {
            foreach (TextBlock potentialChild in hiddenNotes)
            {
                if (potentialChild.Tag == tb)
                {
                    children.Add(potentialChild);
                    GetVisibleChildren(potentialChild, children);
                }
            }
        }

        private void GetVisibleChildren(TextBlock tb, List<TextBlock> children)
        {
            foreach (TextBlock potentialChild in UnCategorised.Children)
            {
                if (potentialChild.Tag == tb) {
                    children.Add(potentialChild);
                    GetVisibleChildren(potentialChild, children);
                }
            }
        }

        private void MakeVisible(List<TextBlock> children)
        {
            foreach (TextBlock t in children) {
                hiddenNotes.Remove(t);
                UnCategorised.Children.Insert(UnCategorised.Children.IndexOf((TextBlock)t.Tag) + 1, t);
            }
        }
        private void MakeHidden(List<TextBlock> children) {
            foreach (TextBlock t in children) {
                UnCategorised.Children.Remove(t);
                hiddenNotes.Add(t);
            }
        }

        private void CreateNewNote(Note note)
        {
            TextBlock tb = new TextBlock();
            tb.Name = "note" + note.note_id;
            tb.Text = note.note_id + " note";
            tb.Focusable = true;
            tb.Background = Brushes.Yellow;
            tb.MouseLeftButtonDown += ClickNote;

            TextBlock parent = null;
            foreach (TextBlock tb1 in UnCategorised.Children)
                if (tb1.Name == "cate" + note.category_id)
                    parent = tb1;
            tb.Tag = parent;

            if (UnCategorised.Children.Count > 0)
            {
                int count = 0;
                TextBlock loopTB = tb;
                while (loopTB.Tag != null)
                {
                    count++;
                    loopTB = loopTB.Tag as TextBlock;
                }
                tb.Margin = new Thickness(count * 10, 0, 0, 0);
                UnCategorised.Children.Insert(UnCategorised.Children.IndexOf(parent) + 1, tb);
            }
            else
            {
                UnCategorised.Children.Add(tb);
            }
        }

        private void ClickNote(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock)
            {
                int noteId = int.Parse(((TextBlock)sender).Name.Substring(4));
                openNote = DB.GetNote(noteId);
                string rtfText = openNote.note_content;
                byte[] byteArray = Encoding.ASCII.GetBytes(rtfText);
                using (MemoryStream ms = new MemoryStream(byteArray))
                {
                    TextRange tr = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
                    tr.Load(ms, DataFormats.Rtf);
                }
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

        private void UpdateNotes() {
            UnCategorised.Children.Clear();
            AddCategories(DB.GetNoteCategories()[0]);
            AddNotes();
        }
    }
}