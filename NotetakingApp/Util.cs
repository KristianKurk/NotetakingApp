using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotetakingApp
{
    public static class Util
    {
        public static string getCurrentCampaign() {
            return Properties.Settings.Default.Campaign;
        }
    }
}
