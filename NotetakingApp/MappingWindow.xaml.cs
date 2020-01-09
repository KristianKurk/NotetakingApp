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
        List<Point> rightClickPoints = new List<Point>();
        private double zoomPercentage = 1;
        

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
            };

            mapCanvas.MouseRightButtonDown += (ss, ee) =>
            {
                ContextMenu cm = this.FindResource("cmButton") as ContextMenu;
                cm.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
                cm.IsOpen = true;
                rightClickPoint = ee.GetPosition(mapCanvas);
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

        private void dbInit() {
            List<Pin> dbPins = DB.getPins();
            foreach (Pin dbPin in dbPins) {

                Image pin = new Image();
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri("Assets/Pins/personpin.png", UriKind.Relative);
                bitmap.EndInit();
                pin.Source = bitmap;
                pin.Stretch = Stretch.UniformToFill;

                Button button = new Button();
                button.Click += new RoutedEventHandler(Click_Pin);
                button.Background = Brushes.Transparent;
                button.BorderThickness = new Thickness(0);

                button.Name = "id" + dbPin.pin_id;
                button.Content = pin;
                Canvas.SetLeft(button, dbPin.pin_x);
                Canvas.SetTop(button, dbPin.pin_y);

                pins.Add(button); 

                pinCanvas.Children.Add(button);
            }
            ResizePins();
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
            button.Background = Brushes.Transparent;
            button.BorderThickness = new Thickness(0);

            Pin dbPin = new Pin();
            dbPin.pin_title = "Untitled";
            dbPin.pin_content = "";
            dbPin.pin_x = rightClickPoint.X;
            dbPin.pin_y = rightClickPoint.Y;
            dbPin.parent_map_id = currentMap.map_id;
            DB.Add(dbPin);

            button.Name = "id"+DB.getPins().Last().pin_id;
            button.Content = pin;

            pins.Add(button);
            rightClickPoints.Add(rightClickPoint);
            ResizePins();

            pinCanvas.Children.Add(button);
        }

        private void Create_Map_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Created a map");
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
                }
            }
            else
            {
                foreach (Button pin in pins)
                {
                    pin.Width = MAX_PIN_SIZE;
                    pin.Height = MAX_PIN_SIZE;
                }
            }
            List<Pin> dbPins = DB.getPins();
            for (int i = 0; i < pins.Count; i++)
            {
                Canvas.SetLeft(pins[i], dbPins[i].pin_x - pins[i].Width / 1.8);
                Canvas.SetTop(pins[i], dbPins[i].pin_y - pins[i].Height / 1.2);
                Console.WriteLine(dbPins.Count + " " + pins.Count);
            }
        }

        private void Click_Pin(object sender, RoutedEventArgs e) {
            Console.WriteLine("pin clicked");
            Button button = sender as Button;

            TextBox myText = new TextBox();
            myText.Name = "pinText";
            myText.Text = "Hello World!";
            myText.AcceptsReturn = true;
            myText.TextWrapping = TextWrapping.Wrap;
            myText.MaxWidth = 200;
            myText.Width = 200;
            myText.MinLines = 3;
            myText.MaxLines = 10;
            myText.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            Pin attachedPin = DB.GetPin(Int32.Parse(button.Name.Substring(2)));
            Canvas.SetLeft(myText, attachedPin.pin_x);
            Canvas.SetTop(myText, attachedPin.pin_y);
            pinCanvas.Children.Add(myText);
        }
    }
}
