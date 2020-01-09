using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace BLL
{
    public class Map
    {
        public int map_id { get; set; }
        public string map_name { get; set; }
        public byte[] map_file { get; set; }
        public double map_x { get; set; }
        public double map_y { get; set; }
        public int parent_map_id { get; set; }

        public BitmapImage LoadImage()
        {
            if (map_file == null || map_file.Length == 0) return null;
            var image = new BitmapImage();
            using (var mem = new System.IO.MemoryStream(map_file))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }
    }
}
