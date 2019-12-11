using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class Map
    {
        public int map_id { get; set; }
        public string map_name { get; set; }
        public byte[] map_file { get; set; }
        public int map_x { get; set; }
        public int map_y { get; set; }
        public int parent_map_id { get; set; }
    }
}
