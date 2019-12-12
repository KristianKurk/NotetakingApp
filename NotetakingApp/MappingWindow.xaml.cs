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
        public MappingWindow()
        {
            InitializeComponent();
            BitmapImage img = DB.GetMap(1).LoadImage();
            imgSource.Source = img;
            imgSource2.Source = img;
            init();
        }

        private Point firstPoint = new Point();

        public void init()
        {
            cavRoot.MouseLeftButtonDown += (ss, ee) => {
                firstPoint = ee.GetPosition(this);
                cavRoot.CaptureMouse();
            };

            cavRoot.MouseWheel += (ss, ee) => {
                Matrix mat = cavRoot.RenderTransform.Value;
                Point mouse = ee.GetPosition(cavRoot);

                if (ee.Delta > 0)
                    mat.ScaleAtPrepend(1.15, 1.15, mouse.X, mouse.Y);
                else
                    mat.ScaleAtPrepend(1 / 1.15, 1 / 1.15, mouse.X, mouse.Y);
                MatrixTransform mtf = new MatrixTransform(mat);

                cavRoot.RenderTransform = mtf;
            };


            cavRoot.MouseMove += (ss, ee) =>
            {
                if (ee.LeftButton == MouseButtonState.Pressed)
                {
                    Point temp = ee.GetPosition(this);
                    Point res = new Point(firstPoint.X - temp.X, firstPoint.Y - temp.Y);

                    double tentativeLeft = Canvas.GetLeft(cavRoot) - res.X;
                    double tentativeTop = Canvas.GetTop(cavRoot) - res.Y;

                    /*
                    if (tentativeLeft > 0 && tentativeTop < imgSource.ActualWidth)
                        

                    if (tentativeTop > 0 && tentativeTop < imgSource.ActualHeight)
                        */
                    Canvas.SetLeft(cavRoot, tentativeLeft);
                    Canvas.SetTop(cavRoot, tentativeTop);
                    //Console.WriteLine("Mouse movement is being registered. Left: "+ relativePoint.X + " Top: "+ relativePoint.Y);
                    firstPoint = temp;
                }
            };

            cavRoot.MouseUp += (ss, ee) => { cavRoot.ReleaseMouseCapture(); };
        }
    }
}
