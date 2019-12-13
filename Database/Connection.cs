using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Database
{
    public class Connection
    {
        private static string xmlPath = "./campaigns.xml";
        XmlDocument xmlDoc;

        public static string LoadConnectionString(string id)
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }
        public static string LoadConnectionString()
        {
            //string id = Util.getCurrentCampaign();
            return ConfigurationManager.ConnectionStrings[GetActiveCampaignDirectory()].ConnectionString;
        }

        public static void CreateNewCampaign(string campaignName)
        {
            CreateXmlCampaign(campaignName);
            //setCampaignAsActive(GetActiveCampaignDirectory());

            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            ConnectionStringsSection csSection = config.ConnectionStrings;
            ConnectionStringSettings connection = new ConnectionStringSettings(GetActiveCampaignDirectory(), "Data Source=.\\DB" + GetActiveCampaignDirectory() + ".db;Version=3", "System.Data.SqlClient");
            csSection.ConnectionStrings.Add(connection);
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("connectionStrings");

            using (SQLiteConnection source = new SQLiteConnection(LoadConnectionString("default")))
            using (SQLiteConnection destination = new SQLiteConnection(LoadConnectionString(GetActiveCampaignDirectory())))
            {
                source.Open();
                destination.Open();
                source.BackupDatabase(destination, "main", "main", -1, null, -1);
            }

        }

        private static void CreateXmlCampaign(string campaignName)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlPath);
            XmlNode newCampaign = xmlDoc.CreateNode(XmlNodeType.Element, "campaign", null);


            XmlAttribute nameAttr = xmlDoc.CreateAttribute("name");
            nameAttr.Value = campaignName;
            XmlAttribute directoryAttr = xmlDoc.CreateAttribute("directory");
            directoryAttr.Value = "dir" + getDirectoryNum();
            XmlAttribute statusAttr = xmlDoc.CreateAttribute("status");
            statusAttr.Value = "active";

            newCampaign.Attributes.Append(nameAttr);
            newCampaign.Attributes.Append(directoryAttr);
            newCampaign.Attributes.Append(statusAttr);

            xmlDoc.GetElementsByTagName("campaigns").Item(0).AppendChild(newCampaign);
            xmlDoc.Save(xmlPath);
            xmlDoc = null;
        }

        public static string GetActiveCampaignDirectory()
        {
            XmlReader xmlReader = XmlReader.Create(xmlPath);
            while (xmlReader.Read())
            {
                if ((xmlReader.NodeType == XmlNodeType.Element) && (xmlReader.Name == "campaign"))
                {
                    if (xmlReader.HasAttributes)
                        if (xmlReader.GetAttribute("status") == "active")
                            return xmlReader.GetAttribute("directory");
                }
            }
            return "";
        }

        public static string GetActiveCampaignName()
        {
            XmlReader xmlReader = XmlReader.Create(xmlPath);
            while (xmlReader.Read())
            {
                if ((xmlReader.NodeType == XmlNodeType.Element) && (xmlReader.Name == "campaign"))
                {
                    if (xmlReader.HasAttributes)
                        if (xmlReader.GetAttribute("status") == "active")
                            return xmlReader.GetAttribute("name");
                }
            }
            return "";
        }

        public static List<string> GetCampaignNames()
        {
            List<string> campaigns = new List<string>();
            XmlReader xmlReader = XmlReader.Create(xmlPath);
            while (xmlReader.Read())
            {
                if ((xmlReader.NodeType == XmlNodeType.Element) && (xmlReader.Name == "campaign"))
                {
                    if (xmlReader.HasAttributes)
                        if (xmlReader.GetAttribute("status") == "active")
                            campaigns.Add(xmlReader.GetAttribute("name"));
                }
            }
            return campaigns;
        }

        public static int getDirectoryNum()
        {
            int nodeCount = 0;
            using (var reader = XmlReader.Create(xmlPath))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element &&
                        reader.Name == "campaign")
                    {
                        nodeCount++;
                    }
                }
            }
            return nodeCount;
        }

        public static void setCampaignAsActive(string campaignDirectory)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlPath);

            XmlNodeList campaigns = xmlDoc.GetElementsByTagName("campaign");

            for (int i = 0; i < campaigns.Count; i++)
                if (campaigns.Item(i).Attributes["directory"].Value == campaignDirectory)
                {
                    campaigns.Item(i).Attributes["status"].Value = "active";
                }
                else
                {
                    campaigns.Item(i).Attributes["status"].Value = "inactive";
                }
            {
                xmlDoc.Save(xmlPath);

            }
        }
    }
}
