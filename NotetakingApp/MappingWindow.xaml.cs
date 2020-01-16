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
using Microsoft.Win32;
using System.IO;

namespace NotetakingApp
{
    public partial class MappingWindow : Page
    {
        private const int MAX_PIN_SIZE = 50;
        private const int MAX_PIN_ZOOM_IN = 3;
        private const float MAX_MAP_ZOOM_OUT = 0.5f;

        private Map currentMap = DB.GetMap(1);

        private Matrix initialMat;
        private Point firstPoint = new Point();
        private Point rightClickPoint = new Point();
        List<Button> pins = new List<Button>();
        List<Button> maps = new List<Button>();
        private double zoomPercentage = 1;
        private Canvas displayCanvas;
        private bool isHover = false;
        List<Map> tbd = new List<Map>();

        public MappingWindow()
        {
            InitializeComponent();

            initialMat = mapCanvas.RenderTransform.Value;
            BitmapImage img = DB.GetMap(1).LoadImage();
            imgSource.Source = img;
            dbInit();
            init();
        }

        public void init()
        {
            mapCanvas.MouseLeftButtonDown += (ss, ee) =>
            {
                firstPoint = ee.GetPosition(this);
                mapCanvas.CaptureMouse();
                if (displayCanvas != null)
                    SaveAndClosePin();

                AreYouSure.Visibility = Visibility.Hidden;
            };

            mapCanvas.PreviewMouseRightButtonDown += (ss, ee) =>
            {
                if (isHover == false) {
                    ContextMenu cm = this.FindResource("cmButton") as ContextMenu;
                    cm.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
                    cm.IsOpen = true;
                    rightClickPoint = ee.GetPosition(mapCanvas);
                    Console.WriteLine("the button was clicked 1");
                }
            };

            mapCanvas.MouseWheel += (ss, ee) =>
            {
                Matrix mat = mapCanvas.RenderTransform.Value;
                Point mouse = ee.GetPosition(mapCanvas);
                zoomPercentage = mat.M11 / initialMat.M11;

                if (ee.Delta > 0)
                {
                    mat.ScaleAtPrepend(1.15, 1.15, mouse.X, mouse.Y);
                    ResizePins();
                }
                else if (zoomPercentage > MAX_MAP_ZOOM_OUT)
                {
                    mat.ScaleAtPrepend(1 / 1.15, 1 / 1.15, mouse.X, mouse.Y);
                    ResizePins();

                }
                else
                {
                    if (currentMap.parent_map_id != 0)
                    {
                        Map attachedMap = DB.GetMap(currentMap.parent_map_id);
                        BitmapImage img = attachedMap.LoadImage();
                        imgSource.Source = img;
                        currentMap = attachedMap;
                        foreach (Button map in maps)
                            pinCanvas.Children.Remove(map);
                        foreach (Button pin in pins)
                            pinCanvas.Children.Remove(pin);
                        maps.Clear();
                        pins.Clear();
                        dbInit();
                    }
                }

                MatrixTransform mtf = new MatrixTransform(mat);
                mapCanvas.RenderTransform = mtf;
            };


            mapCanvas.MouseMove += (ss, ee) =>
            {
                if (ee.LeftButton == MouseButtonState.Pressed &&
                    !firstPoint.Equals(new Point(-9999, -9999)))
                {
                    Point temp = ee.GetPosition(this);
                    Point res = new Point(firstPoint.X - temp.X, firstPoint.Y - temp.Y);

                    double tentativeLeft = Canvas.GetLeft(mapCanvas) - res.X;
                    double tentativeTop = Canvas.GetTop(mapCanvas) - res.Y;

                    Canvas.SetLeft(mapCanvas, tentativeLeft);
                    Canvas.SetTop(mapCanvas, tentativeTop);
                    firstPoint = temp;
                }
            };

            mapCanvas.MouseUp += (ss, ee) =>
            {
                firstPoint = new Point(-9999, -9999);
                mapCanvas.ReleaseMouseCapture();
            };
        }

        private void dbInit()
        {
            List<Pin> dbPins = DB.getPins();
            foreach (Pin dbPin in dbPins)
            {
                if (dbPin.parent_map_id == currentMap.map_id)
                {
                    Image pin = new Image();
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri("Assets/Pins/personpin.png", UriKind.Relative);
                    bitmap.EndInit();
                    pin.Source = bitmap;
                    pin.Stretch = Stretch.UniformToFill;

                    Button button = new Button();
                    button.Click += new RoutedEventHandler(Click_Pin);
                    button.MouseEnter += new MouseEventHandler(Mouse_Enter);
                    button.MouseLeave += new MouseEventHandler(Mouse_Leave);
                    button.PreviewMouseRightButtonDown += new MouseButtonEventHandler(Right_Click_Pin);
                    button.Background = Brushes.Transparent;
                    button.BorderThickness = new Thickness(0);

                    button.Name = "id" + dbPin.pin_id;
                    button.Content = pin;
                    Canvas.SetLeft(button, dbPin.pin_x);
                    Canvas.SetTop(button, dbPin.pin_y);

                    pins.Add(button);

                    pinCanvas.Children.Add(button);
                }
            }

            List<Map> dbMaps = DB.GetMaps();
            foreach (Map dbMap in dbMaps)
            {
                if (currentMap.map_id == dbMap.parent_map_id)
                {
                    Image map = new Image();
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri("Assets/Pins/map.png", UriKind.Relative);
                    bitmap.EndInit();
                    map.Source = bitmap;
                    map.Stretch = Stretch.UniformToFill;

                    Button button = new Button();
                    //button.Click += new RoutedEventHandler(Click_Map);
                    button.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(Click_Map);
                    button.PreviewMouseRightButtonDown += new MouseButtonEventHandler(Right_Click_Map);
                    button.MouseEnter += new MouseEventHandler(Mouse_Enter);
                    button.MouseLeave += new MouseEventHandler(Mouse_Leave);
                    button.Background = Brushes.Transparent;
                    button.BorderThickness = new Thickness(0);

                    button.Name = "mid" + dbMap.map_id;
                    button.Content = map;
                    Canvas.SetLeft(button, dbMap.map_x);
                    Canvas.SetTop(button, dbMap.map_y);

                    maps.Add(button);

                    pinCanvas.Children.Add(button);
                }
            }
            ResizePins();
        }

        private void Mouse_Leave(object sender, MouseEventArgs e)
        {
            isHover = false;
        }

        private void Mouse_Enter(object sender, MouseEventArgs e)
        {
            isHover = true;
        }

        private void Create_Pin_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Created a pin");

            Image pin = new Image();
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri("Assets/Pins/personpin.png", UriKind.Relative);
            bitmap.EndInit();
            pin.Source = bitmap;
            pin.Stretch = Stretch.UniformToFill;

            Button button = new Button();
            button.Click += new RoutedEventHandler(Click_Pin);
            button.MouseEnter += new MouseEventHandler(Mouse_Enter);
            button.MouseLeave += new MouseEventHandler(Mouse_Leave);
            button.PreviewMouseRightButtonDown += new MouseButtonEventHandler(Right_Click_Pin);
            button.Background = Brushes.Transparent;
            button.BorderThickness = new Thickness(0);

            Pin dbPin = new Pin();
            dbPin.pin_title = "Untitled";
            dbPin.pin_content = "";
            dbPin.pin_x = rightClickPoint.X;
            dbPin.pin_y = rightClickPoint.Y;
            dbPin.parent_map_id = currentMap.map_id;
            DB.Add(dbPin);

            button.Name = "id" + DB.getPins().Last().pin_id;
            button.Content = pin;

            pins.Add(button);
            ResizePins();

            pinCanvas.Children.Add(button);
        }

        private void Create_Map_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Created a map");

            Image map = new Image();
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri("Assets/Pins/map.png", UriKind.Relative);
            bitmap.EndInit();
            map.Source = bitmap;
            map.Stretch = Stretch.UniformToFill;

            Button button = new Button();
            //button.Click += new RoutedEventHandler(Click_Map);
            button.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(Click_Map);
            button.PreviewMouseRightButtonDown += new MouseButtonEventHandler(Right_Click_Map);
            button.MouseEnter += new MouseEventHandler(Mouse_Enter);
            button.MouseLeave += new MouseEventHandler(Mouse_Leave);
            button.Background = Brushes.Transparent;
            button.BorderThickness = new Thickness(0);

            Map dbMap = new Map();
            dbMap.map_name = "Untitled";
            dbMap.map_x = rightClickPoint.X;
            dbMap.map_y = rightClickPoint.Y;
            dbMap.parent_map_id = currentMap.map_id;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select a Map Image File";
            openFileDialog.CheckFileExists = true;
            openFileDialog.CheckPathExists = true;
            openFileDialog.ReadOnlyChecked = true;
            openFileDialog.ShowReadOnly = true;
            openFileDialog.Filter = "Image files (*.png;*.jpeg)|*.png;*.jpeg|All files (*.*)|*.*";
            var result = openFileDialog.ShowDialog();
            if (openFileDialog.FileName != "")
            {
                byte[] buffer = File.ReadAllBytes(openFileDialog.FileName);
                dbMap.map_file = buffer;
                dbMap.map_name = System.IO.Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                DB.Add(dbMap);
                button.Name = "mid" + DB.GetMaps().Last().map_id;
                button.Content = map;

                maps.Add(button);

                pinCanvas.Children.Add(button);
                ResizePins();
            }
        }

        private void ResizePins()
        {
            if (zoomPercentage > 1)
            {
                if (zoomPercentage < MAX_PIN_ZOOM_IN)
                {

                    foreach (Button pin in pins)
                    {
                        pin.Width = MAX_PIN_SIZE / zoomPercentage;
                        pin.Height = MAX_PIN_SIZE / zoomPercentage;
                    }

                    foreach (Button map in maps)
                    {
                        map.Width = MAX_PIN_SIZE / zoomPercentage;
                        map.Height = MAX_PIN_SIZE / zoomPercentage;
                    }
                }
            }
            else
            {
                foreach (Button pin in pins)
                {
                    pin.Width = MAX_PIN_SIZE;
                    pin.Height = MAX_PIN_SIZE;
                }

                foreach (Button map in maps)
                {
                    map.Width = MAX_PIN_SIZE;
                    map.Height = MAX_PIN_SIZE;
                }
            }
            List<Pin> dbPins = DB.getPins();
            for (int i = 0; i < dbPins.Count; i++)
            {
                if (dbPins[i].parent_map_id == currentMap.map_id)
                {
                    Button pin = null;

                    foreach (Button mypin in pins)
                        if (int.Parse(mypin.Name.Substring(2)) == dbPins[i].pin_id)
                            pin = mypin;

                    Canvas.SetLeft(pin, dbPins[i].pin_x - pin.Width / 1.8);
                    Canvas.SetTop(pin, dbPins[i].pin_y - pin.Height / 1.2);
                }
            }
            List<Map> dbMaps = DB.GetMaps();

            for (int i = 0; i < dbMaps.Count; i++)
            {
                Console.WriteLine("count: " + maps.Count);
                Console.WriteLine(dbMaps[i].parent_map_id + " " + currentMap.map_id);

                if (dbMaps[i].parent_map_id == currentMap.map_id)
                {
                    Button map = null;

                    foreach (Button mymap in maps)
                        if (int.Parse(mymap.Name.Substring(3)) == dbMaps[i].map_id)
                            map = mymap;

                    Canvas.SetLeft(map, dbMaps[i].map_x - map.Width / 1.8);
                    Canvas.SetTop(map, dbMaps[i].map_y - map.Height / 1.2);
                }
            }
        }

        private void Click_Map(object sender, MouseButtonEventArgs e)
        {
                Button button = sender as Button;
                Map attachedMap = DB.GetMap(int.Parse(button.Name.Substring(3)));
                BitmapImage img = attachedMap.LoadImage();
                imgSource.Source = img;
                currentMap = attachedMap;
                foreach (Button map in maps)
                    pinCanvas.Children.Remove(map);
                foreach (Button pin in pins)
                    pinCanvas.Children.Remove(pin);
                maps.Clear();
                pins.Clear();
                dbInit();
                mapCanvas.RenderTransform.Value.Scale(1, 1);
        }

        private void Right_Click_Map(object sender, MouseButtonEventArgs e)
        {
            Button button = sender as Button;
            Button child = AreYouSure.Children[1] as Button;
            child.Name = button.Name;
            TextBlock text = AreYouSure.Children[0] as TextBlock;
            text.Text = "This action is irreversible. This will delete all inner maps and pins. Are you sure?";
            child.Click -= DeletePin2;
            child.Click += DeleteMap;
            AreYouSure.Visibility = Visibility.Visible;
        }

        private void Right_Click_Pin(object sender, MouseButtonEventArgs e)
        {
            Button button = sender as Button;
            Button child = AreYouSure.Children[1] as Button;
            child.Name = button.Name;
            TextBlock text = AreYouSure.Children[0] as TextBlock;
            text.Text = "You are about to delete a pin. Are you sure?";
            child.Click -= DeleteMap;
            child.Click += DeletePin2;
            AreYouSure.Visibility = Visibility.Visible;


        }

        private void Click_Pin(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("pin clicked");

            Button button = sender as Button;
            Pin attachedPin = DB.GetPin(int.Parse(button.Name.Substring(2)));

            TextBox pinText = new TextBox();
            TextBox pinTitle = new TextBox();
            if (attachedPin.pin_content != null)
                pinText.Text = attachedPin.pin_content;
            else
                pinText.Text = "";
            pinText.AcceptsReturn = true;
            pinText.TextWrapping = TextWrapping.Wrap;
            pinText.MaxWidth = 200;
            pinText.Width = 200;
            pinText.MinLines = 3;
            pinText.MaxLines = 10;
            pinText.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;

            pinTitle.Width = 150;
            if (attachedPin.pin_title != null)
                pinTitle.Text = attachedPin.pin_title;
            else
                pinTitle.Text = "Enter a title";
            pinTitle.MinLines = 1;
            pinTitle.MaxLines = 1;
            pinTitle.Height = 23;

            Button closeButton = new Button();
            closeButton.Click += new RoutedEventHandler(SaveAndCloseButton);
            closeButton.ClickMode = ClickMode.Press;
            closeButton.ToolTip = "Close Pin";

            Image closeImage = new Image();
            BitmapImage bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.UriSource = new Uri("Assets/Navigation/close.png", UriKind.Relative);
            bmp.EndInit();
            closeImage.Source = bmp;
            closeButton.Content = closeImage;

            Button deleteButton = new Button();
            deleteButton.Click += new RoutedEventHandler(DeletePin);
            deleteButton.ClickMode = ClickMode.Press;
            deleteButton.ToolTip = "Delete Pin";

            Image deleteImage = new Image();
            BitmapImage bmp2 = new BitmapImage();
            bmp2.BeginInit();
            bmp2.UriSource = new Uri("Assets/Navigation/delete.png", UriKind.Relative);
            bmp2.EndInit();
            deleteImage.Source = bmp2;
            deleteButton.Content = deleteImage;

            Canvas textCanvas = new Canvas();
            textCanvas.Children.Add(pinTitle);
            textCanvas.Children.Add(pinText);
            textCanvas.Children.Add(deleteButton);
            textCanvas.Children.Add(closeButton);
            textCanvas.Height = 75;
            textCanvas.Width = 200;
            Canvas.SetTop(pinText, 25);
            Canvas.SetTop(pinTitle, 0);
            Canvas.SetLeft(closeButton, 178);
            Canvas.SetLeft(deleteButton, 153);
            textCanvas.Name = button.Name;

            Canvas.SetLeft(textCanvas, attachedPin.pin_x);
            Canvas.SetTop(textCanvas, attachedPin.pin_y);
            pinCanvas.Children.Add(textCanvas);

            if (displayCanvas == null)
                displayCanvas = textCanvas;
            else
            {
                SaveAndClosePin();
                displayCanvas = textCanvas;
            }
        }

        private void SaveAndClosePin()
        {
            Pin attachedPin = DB.GetPin(Int32.Parse(displayCanvas.Name.Substring(2)));

            TextBox titlebox = displayCanvas.Children[0] as TextBox;
            string title = titlebox.Text;
            attachedPin.pin_title = title;

            TextBox textbox = displayCanvas.Children[1] as TextBox;
            string text = textbox.Text;
            attachedPin.pin_content = text;

            DB.Update(attachedPin);
            pinCanvas.Children.Remove(displayCanvas);
        }

        private void SaveAndCloseButton(object sender, RoutedEventArgs e)
        {
            SaveAndClosePin();
        }

        private void DeletePin(object sender, RoutedEventArgs e)
        {
            Pin attachedPin = DB.GetPin(Int32.Parse(displayCanvas.Name.Substring(2)));
            DB.DeletePin(attachedPin.pin_id);
            pinCanvas.Children.Remove(displayCanvas);
            Button pin = null;

            foreach (Button mypin in pins)
                if (int.Parse(mypin.Name.Substring(2)) == attachedPin.pin_id)
                    pin = mypin;

            pinCanvas.Children.Remove(pin);
        }

        private void DeletePin2(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Pin attachedPin = DB.GetPin(Int32.Parse(button.Name.Substring(2)));

            Button pin = null;

            foreach (Button mypin in pins)
                if (int.Parse(mypin.Name.Substring(2)) == attachedPin.pin_id)
                    pin = mypin;

            pinCanvas.Children.Remove(pin);
            AreYouSure.Visibility = Visibility.Hidden;
        }

        private void DeleteMap(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Map mapToBeDeleted = DB.GetMap(int.Parse(button.Name.Substring(3)));

            tbd.Clear();
            RecursiveGetMapChildren(mapToBeDeleted);
            tbd.Add(mapToBeDeleted);

            foreach (Pin pin in DB.getPins()) {
                foreach (Map mymap in tbd) {
                    if (pin.parent_map_id == mymap.map_id)
                        DB.DeletePin(pin.pin_id);
                }
            }

            foreach (Map mymap in tbd) {
                DB.DeleteMap(mymap.map_id);
            }

            Button map = null;

            foreach (Button mymap in maps)
                if (int.Parse(mymap.Name.Substring(3)) == mapToBeDeleted.map_id)
                    map = mymap;
            pinCanvas.Children.Remove(map);
            AreYouSure.Visibility = Visibility.Hidden;
        }

        private void RecursiveGetMapChildren(Map mapToBeDeleted)
        {
            foreach (Map map in DB.GetMaps()) {
                if (map.parent_map_id == mapToBeDeleted.map_id) {
                    tbd.Add(map);
                    RecursiveGetMapChildren(map);
                }
            }
        }

        private void HidePanel(object sender, RoutedEventArgs e)
        {
            AreYouSure.Visibility = Visibility.Hidden;
        }
    }
}
