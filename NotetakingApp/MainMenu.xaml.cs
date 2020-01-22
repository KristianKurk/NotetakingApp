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

namespace NotetakingApp
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : Page
    {
       
        public MainMenu()
        {
            
            InitializeComponent();
          

            //Load map
            
                StartMap();
             
            //Load favorite notes
            //StartFavNotes();

            //Load 
            //StartMusic();
        }
        public void StartMap() {
            //Load map
            map.Content = new MappingWindow();
        }
        public void StartFavNotes()
        {

        }
        public void StartMusic()
        {

        }
        private void BtnClickMap(object sender, RoutedEventArgs e)
        {
            StartMap();
        }
    }
}
