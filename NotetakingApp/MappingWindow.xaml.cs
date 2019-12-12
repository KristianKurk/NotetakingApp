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
    /// Interaction logic for MappingWindow.xaml
    /// </summary>
    public partial class MappingWindow : Page
    {
        private Matrix initialMat;
        private Point firstPoint = new Point();
        private Point rightClickPoint = new Point();

        public MappingWindow()
        {
            InitializeComponent();

            initialMat = cavRoot.RenderTransform.Value;
            BitmapImage img = DB.GetMap(1).LoadImage();
            imgSource.Source = img;
            init();
        }

        public void init()
        {
            cavRoot.MouseLeftButtonDown += (ss, ee) => {
                firstPoint = ee.GetPosition(this);
                cavRoot.CaptureMouse();
            };

            imgSource.MouseRightButtonDown += (ss, ee) =>
            {
                //Console.WriteLine("Right Click X: " + ee.GetPosition(imgSource).X +" Right Click Y: "+ ee.GetPosition(imgSource).Y);
                ContextMenu cm = this.FindResource("cmButton") as ContextMenu;
                cm.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
                cm.IsOpen = true;
                rightClickPoint = ee.GetPosition(imgSource);
            };

            cavRoot.MouseWheel += (ss, ee) => {
                Matrix mat = cavRoot.RenderTransform.Value;
                Point mouse = ee.GetPosition(cavRoot);


                    if (ee.Delta > 0)
                        mat.ScaleAtPrepend(1.15, 1.15, mouse.X, mouse.Y);
                    else if (mat.M11 > initialMat.M11 * 0.5)
                        mat.ScaleAtPrepend(1 / 1.15, 1 / 1.15, mouse.X, mouse.Y);
                    MatrixTransform mtf = new MatrixTransform(mat);
                    cavRoot.RenderTransform = mtf;
                     
            };


            cavRoot.MouseMove += (ss, ee) =>
            {
                if (ee.LeftButton == MouseButtonState.Pressed &&
                    !firstPoint.Equals(new Point(-9999,-9999)))
                {
                    Point temp = ee.GetPosition(this);
                    Point res = new Point(firstPoint.X - temp.X, firstPoint.Y - temp.Y);

                    double tentativeLeft = Canvas.GetLeft(cavRoot) - res.X;
                    double tentativeTop = Canvas.GetTop(cavRoot) - res.Y;

                    Canvas.SetLeft(cavRoot, tentativeLeft);
                    Canvas.SetTop(cavRoot, tentativeTop);
                    firstPoint = temp;
                }
            };

            cavRoot.MouseUp += (ss, ee) => {
                firstPoint = new Point(-9999,-9999);
                cavRoot.ReleaseMouseCapture(); };
        }

        private void Create_Pin_Click(object sender, RoutedEventArgs e)
        {
            int pin_size = 50;

            Console.WriteLine("Created a pin");
            Image pin = new Image();
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri("Assets/Pins/pin.png", UriKind.Relative);
            bitmap.EndInit();
            pin.Source = bitmap;
            pin.Width = pin_size;
            pin.Height = pin_size;

            Canvas.SetTop(pin,rightClickPoint.Y);
            Canvas.SetLeft(pin, rightClickPoint.X);

            cavRoot.Children.Add(pin);
        }

        private void Create_Map_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Created a map");
        }
    }
}
