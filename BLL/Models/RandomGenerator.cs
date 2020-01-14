using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class RandomGenerator
    {
        public int rng_id { get; set; }
        public string rng_title { get; set; }
        public string rng_content { get; set; }

        public override string ToString() {
            return rng_title;
        }
    }
}
