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
        private Matrix initialMat;
        private Point firstPoint = new Point();
        private Point rightClickPoint = new Point();
        List<Image> pins = new List<Image>();
        List<Point> rightClickPoints = new List<Point>();
        private double zoomPercentage = 1;
        private const int MAX_PIN_SIZE = 50;

        public MappingWindow()
        {
            InitializeComponent();

            initialMat = mapCanvas.RenderTransform.Value;
            BitmapImage img = DB.GetMap(1).LoadImage();
            imgSource.Source = img;
            init();
            Properties.Settings.Default.Campaign = "joe";
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
                else if (zoomPercentage > 0.5)
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

        private void Create_Pin_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Created a pin");

            Image pin = new Image();
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri("Assets/Pins/personpin.png", UriKind.Relative);
            bitmap.EndInit();
            pin.Source = bitmap;
            pin.Width = MAX_PIN_SIZE;
            pin.Height = MAX_PIN_SIZE;

            Canvas.SetTop(pin, rightClickPoint.Y - MAX_PIN_SIZE / 1.2);
            Canvas.SetLeft(pin, rightClickPoint.X - MAX_PIN_SIZE / 1.8);

            pins.Add(pin);
            rightClickPoints.Add(rightClickPoint);

            ResizePins();
            pinCanvas.Children.Add(pin);
        }

        private void Create_Map_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Created a map");
        }

        private void ResizePins() {
            if (zoomPercentage > 1)
            {
                foreach (Image pin in pins)
                {
                    pin.Width = MAX_PIN_SIZE / zoomPercentage;
                    pin.Height = MAX_PIN_SIZE / zoomPercentage;
                }
            }
            else
            {
                foreach (Image pin in pins)
                {
                    pin.Width = MAX_PIN_SIZE;
                    pin.Height = MAX_PIN_SIZE;
                }
            }
            for (int i = 0; i < pins.Count; i++)
            {
                Canvas.SetLeft(pins[i], rightClickPoints[i].X - pins[i].Width / 1.8);
                Canvas.SetTop(pins[i], rightClickPoints[i].Y - pins[i].Height / 1.2);
            }
        }
    }
}
