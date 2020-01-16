using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class Note
    {
        public int note_id { get; set; }
        public string note_title { get; set; }
        public string note_content { get; set; }
        public int category_id { get; set; }

        public override string ToString()
        {
            return note_title;
        }
    }
}
