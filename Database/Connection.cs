using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database
{
    public class Connection
    {
        public static string LoadConnectionString(string id)
        { 
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }
        public static string LoadConnectionString()
        {
            //string id = Util.getCurrentCampaign();
            return ConfigurationManager.ConnectionStrings["jkdasd"].ConnectionString;
        }


        public static void CreateNewCampaign(string campaignName)
        {
            // Get the application configuration file.
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            // Get the conectionStrings section.
            ConnectionStringsSection csSection = config.ConnectionStrings;

            //Create your connection string into a connectionStringSettings object
            ConnectionStringSettings connection = new ConnectionStringSettings(campaignName, "Data Source=.\\DB" + campaignName + ".db;Version=3", "System.Data.SqlClient");

            //Add the object to the configuration
            csSection.ConnectionStrings.Add(connection);

            //Save the configuration
            config.Save(ConfigurationSaveMode.Modified);

            //Refresh the Section
            ConfigurationManager.RefreshSection("connectionStrings");

            using (SQLiteConnection source = new SQLiteConnection(LoadConnectionString("default")))
            using (SQLiteConnection destination = new SQLiteConnection(LoadConnectionString(campaignName)))
            {
                source.Open();
                destination.Open();
                source.BackupDatabase(destination, "main", "main", -1, null, -1);
            }

        }
    }
}
