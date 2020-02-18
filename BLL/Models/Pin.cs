using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class Pin
    {
        public int pin_id { get; set; }
        public string pin_title { get; set; }
        public string pin_content { get; set; }
        public double pin_x { get; set; }
        public double pin_y { get; set; }
        public int parent_map_id { get; set; }
        public int attached_note_id { get; set; }
        public string pin_type { get; set; }
    }
}
