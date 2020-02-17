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
        private int id;
        private string type;
        List<TextBox> hiddenNotes;
        private TextBox editedTitle; //Current Note or Category being edited
        private bool isClicked = false;
        private bool isDragged = false;
        private Point startPoint;
        string dragma = "";

        public Note_takingWindow()
        {
            InitializeComponent();
            cmbFontFamily.ItemsSource = Fonts.SystemFontFamilies.OrderBy(f => f.Source);
            cmbFontSize.ItemsSource = new List<double>() { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72 };


            hiddenNotes = new List<TextBox>();


            if (Properties.Settings.Default.currentNote != null)
            {
                openNote = DB.GetNote(Properties.Settings.Default.currentNote.note_id);
            }
            UpdateNotes();
            if (openNote != null)
                LoadNoteContent();
            else
                rtbEditor.Visibility = Visibility.Hidden;
        }
        private void BtnDeleteNote(object sender, RoutedEventArgs e)
        {
            if (openNote != null)
            {
                foreach (Pin p in DB.getPins())
                    if (p.attached_note_id == openNote.note_id)
                    {
                        p.attached_note_id = 0;
                        DB.Update(p);
                    }

                DB.DeleteNote(openNote.note_id);
                openNote = null;
                Properties.Settings.Default.currentNote = null;
                rtbEditor.Visibility = Visibility.Hidden;
                UpdateNotes();
            }
        }
        private void BtnAddNote(object sender, RoutedEventArgs e)
        {
            Note note = new Note();
            string rtfText = "{\\rtf1\\ansi\\ansicpg1252\\uc1\\htmautsp\\deff2{\\fonttbl{\\f0\\fcharset0 Times New Roman;}{\\f2\\fcharset0 Segoe UI;}}{\\colortbl\\red0\\green0\\blue0;\\red255\\green255\\blue255;}\\loch\\hich\\dbch\\pard\\plain\\ltrpar\\itap0{\\lang1033\\fs18\\f2\\cf0 \\cf0\\ql{\\f2 \\li0\\ri0\\sa0\\sb0\\fi0\\ql\\par}}}";
            note.note_title = "New Note";
            note.category_id = 1;
            note.note_content = rtfText;
            DB.Add(note);
            UpdateNotes();
        }
        private void BtnAddCategory(object sender, RoutedEventArgs e)
        {
            NoteCategory nc = new NoteCategory();
            nc.category_parent = 1;
            nc.category_title = "New Category";
            DB.Add(nc);
            UpdateNotes();
        }
        private void BtnFavorite(object sender, RoutedEventArgs e)
        {
            if (openNote != null)
            {
                if (openNote.is_favorite == 1)
                {
                    openNote.is_favorite = 0;
                    DB.Update(openNote);
                    Favorite.ToolTip = "Add to Favorites";
                    ((TextBlock)Favorite.Content).Text = "Add to Favorites";
                }
                else if (openNote.is_favorite == 0)
                {
                    openNote.is_favorite = 1;
                    DB.Update(openNote);
                    Favorite.ToolTip = "Remove from Favorites";
                    ((TextBlock)Favorite.Content).Text = "Remove from Favorites";
                }
            }
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

                //Should be restricted to RTF
                try
                {
                    FileStream fileStream = new FileStream(dlg.FileName, FileMode.Open);
                    TextRange range = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
                    range.Load(fileStream, DataFormats.Rtf);
                }
                catch (IOException ee) {
                    Console.WriteLine("Attempted to open same file twice.");
                }
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
            try
            {
                rtbEditor.Selection.ApplyPropertyValue(Inline.FontSizeProperty, cmbFontSize.Text);
            }
            catch (Exception ee)
            {
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
            if (openNote != null)
            {
                openNote.note_content = rtfText;
                DB.Update(openNote);
            }
        }

        private void OpenCM(object sender, MouseButtonEventArgs e)
        {
            ContextMenu cm = this.FindResource("cmButton") as ContextMenu;
            if (e.Source is TextBox)
            {
                MenuItem menuItem = cm.Items[2] as MenuItem;
                menuItem.IsEnabled = true;
                TextBox tb = e.Source as TextBox;
                id = int.Parse(tb.Name.Substring(4));
                type = tb.Name.Substring(0, 4);
            }
            else
            {
                MenuItem menuItem = cm.Items[2] as MenuItem;
                menuItem.IsEnabled = false;
                id = 1;
                type = "base";
            }

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
            note.is_favorite = 0;
            DB.Add(note);
            UpdateNotes();
        }

        private void DeleteClick(object sender, RoutedEventArgs e)
        {
            TextBox target = null;
            foreach (TextBox t in UnCategorised.Children)
                if (t.Name == type + id)
                    target = t;

            if (type == "cate")
            {
                List<TextBox> ToRemove = new List<TextBox>();
                GetVisibleChildren(target, ToRemove);
                GetHiddenChildren(target, ToRemove);
                ToRemove.Add(target);

                foreach (TextBox t in ToRemove)
                    if (t.Name.Substring(0, 4) == "cate")
                        DB.DeleteNoteCategory(int.Parse(t.Name.Substring(4)));
                    else if (t.Name.Substring(0, 4) == "note")
                    {
                        if (openNote != null)
                            if (openNote.note_id == int.Parse(t.Name.Substring(4)))
                            {
                                openNote = null;
                                Properties.Settings.Default.currentNote = null;
                                rtbEditor.Visibility = Visibility.Hidden;
                            }
                        foreach (Pin p in DB.getPins())
                            if (p.attached_note_id == int.Parse(t.Name.Substring(4)))
                            {
                                p.attached_note_id = 0;
                                DB.Update(p);
                            }

                        DB.DeleteNote(int.Parse(t.Name.Substring(4)));
                    }
            }
            else if (type == "note")
            {
                if (openNote.note_id == id)
                {
                    openNote = null;
                    Properties.Settings.Default.currentNote = null;
                    rtbEditor.Visibility = Visibility.Hidden;
                }
                foreach (Pin p in DB.getPins())
                    if (p.attached_note_id == id)
                    {
                        p.attached_note_id = 0;
                        DB.Update(p);
                    }
                DB.DeleteNote(id);
            }
            UpdateNotes();
        }

        private void CreateNewCategory(NoteCategory noteCategory)
        {
            TextBox tb = new TextBox();
            tb.Name = "cate" + noteCategory.category_id;
            tb.Text = noteCategory.category_title;
            tb.Focusable = true;
            tb.MaxLength = 20;
            tb.AllowDrop = true;
            tb.IsReadOnly = false;
            tb.Cursor = Cursors.Arrow;
            tb.PreviewMouseLeftButtonDown += OpenCloseCategory;
            tb.PreviewMouseDoubleClick += SetEditable;
            tb.LostFocus += SetUneditable;
            tb.TextChanged += Tb_TextChanged;
            tb.KeyDown += KeyDownEnter;
            tb.Style = FindResource("CategoryHover") as Style;

            tb.PreviewMouseMove += TB_Move;
            tb.PreviewDrop += TB_Drop;
            tb.PreviewDragEnter += TB_DragEnter;
            tb.PreviewDragOver += TB_DragEnter;
            tb.PreviewMouseLeftButtonUp += TB_MouseUp;

            TextBox parent = null;
            foreach (TextBox tb1 in UnCategorised.Children)
                if (tb1.Name == "cate" + noteCategory.category_parent)
                    parent = tb1;
            tb.Tag = parent;

            if (UnCategorised.Children.Count > 0)
            {
                int count = 0;
                TextBox loopTB = tb;
                while (loopTB.Tag != null)
                {
                    count++;
                    loopTB = loopTB.Tag as TextBox;
                }
                tb.Margin = new Thickness(count * 10, 3, 0, 0);
                UnCategorised.Children.Insert(UnCategorised.Children.IndexOf(parent) + 1, tb);
            }
            else
            {
                UnCategorised.Children.Add(tb);
            }
        }

        private void OpenCloseCategory(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBox)
            {
                TextBox tb = sender as TextBox;
                List<TextBox> visibleChildren = new List<TextBox>();
                List<TextBox> hiddenChildren = new List<TextBox>();
                GetVisibleChildren(tb, visibleChildren);
                GetHiddenChildren(tb, hiddenChildren);

                if (visibleChildren.Count > 0)
                    MakeHidden(visibleChildren);
                else if (hiddenChildren.Count > 0)
                {
                    hiddenChildren.Reverse();
                    MakeVisible(hiddenChildren);
                }

                isClicked = true;
                dragma = tb.Name;
                startPoint = e.GetPosition(UnCategorised);
            }
        }

        private void GetHiddenChildren(TextBox tb, List<TextBox> children)
        {
            foreach (TextBox potentialChild in hiddenNotes)
            {
                if (potentialChild.Tag == tb)
                {
                    children.Add(potentialChild);
                    GetVisibleChildren(potentialChild, children);
                }
            }
        }

        private void GetVisibleChildren(TextBox tb, List<TextBox> children)
        {
            foreach (TextBox potentialChild in UnCategorised.Children)
            {
                if (potentialChild.Tag == tb)
                {
                    children.Add(potentialChild);
                    GetVisibleChildren(potentialChild, children);
                }
            }
        }

        private void MakeVisible(List<TextBox> children)
        {
            foreach (TextBox t in children)
            {
                hiddenNotes.Remove(t);
                UnCategorised.Children.Insert(UnCategorised.Children.IndexOf((TextBox)t.Tag) + 1, t);
            }
        }
        private void MakeHidden(List<TextBox> children)
        {
            foreach (TextBox t in children)
            {
                UnCategorised.Children.Remove(t);
                hiddenNotes.Add(t);
            }
        }

        private void CreateNewNote(Note note)
        {
            TextBox tb = new TextBox();
            tb.Name = "note" + note.note_id;
            tb.Text = note.note_title;
            tb.Focusable = true;
            tb.MaxLength = 20;
            tb.IsReadOnly = false;
            tb.Cursor = Cursors.Arrow;
            tb.AllowDrop = true;
            tb.PreviewMouseLeftButtonDown += ClickNote;
            tb.PreviewMouseDoubleClick += SetEditable;
            tb.KeyDown += KeyDownEnter;
            tb.LostFocus += SetUneditable;
            tb.TextChanged += Tb_TextChanged;
            tb.Style = FindResource("BorderHover") as Style;

            tb.PreviewMouseMove += TB_Move;
            tb.PreviewDrop += TB_Drop;
            tb.PreviewDragEnter += TB_DragEnter;
            tb.PreviewDragOver += TB_DragEnter;
            tb.PreviewMouseLeftButtonUp += TB_MouseUp;

            TextBox parent = null;
            foreach (TextBox tb1 in UnCategorised.Children)
                if (tb1.Name == "cate" + note.category_id)
                    parent = tb1;
            tb.Tag = parent;

            if (UnCategorised.Children.Count > 0)
            {
                int count = 0;
                TextBox loopTB = tb;
                while (loopTB.Tag != null)
                {
                    count++;
                    loopTB = loopTB.Tag as TextBox;
                }
                tb.Margin = new Thickness(count * 10, 2, 0, 0);
                UnCategorised.Children.Insert(UnCategorised.Children.IndexOf(parent) + 1, tb);
            }
            else
            {
                UnCategorised.Children.Insert(0,tb);
            }
        }

        private void Tb_TextChanged(object sender, TextChangedEventArgs e)
        {
            editedTitle = sender as TextBox;
            if (editedTitle.Name.Substring(0, 4) == "cate")
            {
                NoteCategory nc = DB.GetNoteCategory(int.Parse(editedTitle.Name.Substring(4)));
                nc.category_title = editedTitle.Text;
                DB.Update(nc);
            }
            else if (editedTitle.Name.Substring(0, 4) == "note")
            {
                Note n = DB.GetNote(int.Parse(editedTitle.Name.Substring(4)));
                if (n != null)
                {
                    n.note_title = editedTitle.Text;
                    DB.Update(n);
                }
            }
        }

        private void KeyDownEnter(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                editedTitle = sender as TextBox;
                editedTitle.IsReadOnly = true;
                editedTitle.Cursor = Cursors.Arrow;
                if (editedTitle.Name.Substring(0, 4) == "cate")
                {
                    NoteCategory nc = DB.GetNoteCategory(int.Parse(editedTitle.Name.Substring(4)));
                    nc.category_title = editedTitle.Text;
                    DB.Update(nc);
                }
                else if (editedTitle.Name.Substring(0, 4) == "note")
                {
                    Note n = DB.GetNote(int.Parse(editedTitle.Name.Substring(4)));
                    if (n != null)
                    {
                        n.note_title = editedTitle.Text;
                        DB.Update(n);
                    }
                }
                rtbEditor.Focus();
            }
        }

        private void SetUneditable(object sender, RoutedEventArgs e)
        {
            editedTitle = sender as TextBox;
            editedTitle.IsReadOnly = true;
            editedTitle.Cursor = Cursors.Arrow;
            if (editedTitle.Name.Substring(0, 4) == "cate")
            {
                NoteCategory nc = DB.GetNoteCategory(int.Parse(editedTitle.Name.Substring(4)));
                nc.category_title = editedTitle.Text;
                DB.Update(nc);
            }
            else if (editedTitle.Name.Substring(0, 4) == "note")
            {
                Note n = DB.GetNote(int.Parse(editedTitle.Name.Substring(4)));
                if (n != null)
                {
                    n.note_title = editedTitle.Text;
                    DB.Update(n);
                }
            }
        }

        private void SetEditable(object sender, MouseButtonEventArgs e)
        {
            editedTitle = sender as TextBox;
            editedTitle.Cursor = Cursors.IBeam;
            editedTitle.IsReadOnly = false;
        }

        private void ClickNote(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBox)
            {
                int noteId = int.Parse(((TextBox)sender).Name.Substring(4));
                openNote = DB.GetNote(noteId);
                Properties.Settings.Default.currentNote = openNote;
                LoadNoteContent();

                isClicked = true;
                dragma = ((TextBox)(sender)).Name;
                startPoint = e.GetPosition(UnCategorised);
            }
        }

        private void LoadNoteContent()
        {
            rtbEditor.Visibility = Visibility.Visible;
            if (openNote.is_favorite == 1)
            {
                Favorite.ToolTip = "Remove from Favorites";
                ((TextBlock)Favorite.Content).Text = "Remove from Favorites";
            }
            else if (openNote.is_favorite == 0)
            {
                Favorite.ToolTip = "Add to Favorites";
                ((TextBlock)Favorite.Content).Text = "Add to Favorites";
            }

            foreach (TextBox tb in UnCategorised.Children)
                if (tb.Name == "note" + openNote.note_id)
                    tb.Style = FindResource("SelectedNoteStyle") as Style;
                else
                {
                    if (tb.Name.Contains("note"))
                        tb.Style = FindResource("BorderHover") as Style;
                    else
                        tb.Style = FindResource("CategoryHover") as Style;
                }
            string rtfText = openNote.note_content;
            byte[] byteArray = Encoding.ASCII.GetBytes(rtfText);
            using (MemoryStream ms = new MemoryStream(byteArray))
            {
                TextRange tr = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
                tr.Load(ms, DataFormats.Rtf);
            }
        }

        private void AddCategories(NoteCategory noteCategory)
        {
            foreach (NoteCategory nc in DB.GetNoteCategories())
            {
                if (nc.category_parent == noteCategory.category_id)
                {
                    CreateNewCategory(nc);
                    AddCategories(nc);
                }
            }
        }

        private void AddNotes()
        {
            List<Note> nList = DB.GetNotes();
            foreach (Note n in nList)
            {
                CreateNewNote(n);
            }
        }

        private void UpdateNotes()
        {
            UnCategorised.Children.Clear();
            AddCategories(DB.GetNoteCategories()[0]);
            AddNotes();
        }



        private void TB_Move(object sender, MouseEventArgs e)
        {
            if (isClicked && !isDragged)
            {
                Point current = e.GetPosition(UnCategorised);
                if (Math.Abs(current.Y - startPoint.Y) >= SystemParameters.MinimumVerticalDragDistance)
                {
                    DataObject dataObject = new DataObject((TextBox)sender);
                    DragDrop.DoDragDrop((TextBox)sender, dataObject, DragDropEffects.Move);
                    isDragged = true;
                }
            }
        }

        private void TB_Drop(object sender, DragEventArgs e)
        {
            isClicked = false;
            isDragged = false;
            TextBox draggedTB = (TextBox)e.Data.GetData(typeof(TextBox));
            if (draggedTB.Name == dragma)
            {
                Console.WriteLine(draggedTB.Name + " ");
                DragTB(draggedTB, sender as TextBox);
            }
        }

        private void TB_DragEnter(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }

        private void TB_MouseUp(object sender, MouseButtonEventArgs e)
        {
            dragma = "";
            isDragged = false;
            isClicked = false;
        }

        private void DropUnCategorised(object sender, DragEventArgs e)
        {
            if (e.Source is StackPanel)
            {
                isClicked = false;
                isDragged = false;
                TextBox draggedTB = (TextBox)e.Data.GetData(typeof(TextBox));
                DragTB(draggedTB, UnCategorised);
            }
        }

        private void DragTB(TextBox dragElement, TextBox destination)
        {
            if (dragElement.Name.Substring(0, 4) == "note")
            {
                Note dragNote = DB.GetNote(int.Parse(dragElement.Name.Substring(4)));
                if (destination.Name.Substring(0, 4) == "note")
                {
                    Note destNote = DB.GetNote(int.Parse(destination.Name.Substring(4)));
                    dragNote.category_id = destNote.category_id;
                    DB.Update(dragNote);
                }
                else if (destination.Name.Substring(0, 4) == "cate")
                {
                    NoteCategory destCategory = DB.GetNoteCategory(int.Parse(destination.Name.Substring(4)));
                    dragNote.category_id = destCategory.category_id;
                    DB.Update(dragNote);
                }
                else
                {
                    dragNote.category_id = 1;
                    DB.Update(dragNote);
                }
            }
            else if (dragElement.Name.Substring(0, 4) == "cate")
            {
                NoteCategory dragCategory = DB.GetNoteCategory(int.Parse(dragElement.Name.Substring(4)));
                if (destination.Name.Substring(0, 4) == "note")
                {
                    Note destNote = DB.GetNote(int.Parse(destination.Name.Substring(4)));
                    if (dragCategory.category_id != destNote.category_id)
                        dragCategory.category_parent = destNote.category_id;
                    DB.Update(dragCategory);
                }
                else if (destination.Name.Substring(0, 4) == "cate")
                {
                    NoteCategory destCategory = DB.GetNoteCategory(int.Parse(destination.Name.Substring(4)));
                    if (dragCategory.category_id != destCategory.category_id && dragCategory.category_parent != destCategory.category_id)
                    {
                        dragCategory.category_parent = destCategory.category_id;
                        DB.Update(dragCategory);
                    }
                }
            }
            UpdateNotes();
        }

        private void DragTB(TextBox dragElement, StackPanel destination)
        {
            if (dragElement.Name.Substring(0, 4) == "note")
            {
                Note dragNote = DB.GetNote(int.Parse(dragElement.Name.Substring(4)));
                dragNote.category_id = 1;
                if (dragNote.note_id == openNote.note_id)
                    openNote = dragNote;
                DB.Update(dragNote);
            }
            else if (dragElement.Name.Substring(0, 4) == "cate")
            {
                NoteCategory dragCat = DB.GetNoteCategory(int.Parse(dragElement.Name.Substring(4)));
                dragCat.category_parent = 1;
                DB.Update(dragCat);
            }
            UpdateNotes();
        }
    }
}

