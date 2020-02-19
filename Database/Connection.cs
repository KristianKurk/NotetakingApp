using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Database
{
    public class Connection
    {
        private static string xmlPath = "./campaigns.xml";

        public static string LoadConnectionString(string id)
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }
        public static string LoadConnectionString()
        {
            return ConfigurationManager.ConnectionStrings[GetActiveCampaignDirectory()].ConnectionString;
        }

        public static void CreateNewCampaign(string campaignName)
        {
            CreateXmlCampaign(campaignName);
            CreateConnectionString();


            using (SQLiteConnection source = new SQLiteConnection(LoadConnectionString("default")))
            using (SQLiteConnection destination = new SQLiteConnection(LoadConnectionString(GetActiveCampaignDirectory())))
            {
                source.Open();
                destination.Open();
                source.BackupDatabase(destination, "main", "main", -1, null, -1);
            }
        }

        public static void ImportExistingCampaign(string path) {
            CreateXmlCampaign("Imported Campaign");
            CreateConnectionString();

            if (path != null && path != "")
            {
                string dest = System.IO.Directory.GetCurrentDirectory() + "\\DB" + GetActiveCampaignDirectory() + ".db";
                System.IO.File.Copy(path, dest);
            }
        }

        private static void CreateConnectionString() {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            ConnectionStringsSection csSection = config.ConnectionStrings;
            ConnectionStringSettings connection = new ConnectionStringSettings(GetActiveCampaignDirectory(), "Data Source=.\\DB" + GetActiveCampaignDirectory() + ".db;Version=3", "System.Data.SqlClient");
            csSection.ConnectionStrings.Add(connection);
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("connectionStrings");
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

            XmlNodeList campaigns = xmlDoc.GetElementsByTagName("campaign");
            foreach (XmlNode campaign in campaigns)
                campaign.Attributes["status"].Value = "inactive";

            campaigns.Item(campaigns.Count - 1).Attributes["status"].Value = "active";
            xmlDoc.Save(xmlPath);
        }

        public static void SetActiveCampaign(string directory) {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlPath);
            XmlNodeList campaigns = xmlDoc.GetElementsByTagName("campaign");
            foreach (XmlNode campaign in campaigns)
            {
                campaign.Attributes["status"].Value = "inactive";

                if (campaign.Attributes["directory"].Value == directory)
                    campaign.Attributes["status"].Value = "active";
            }
            xmlDoc.Save(xmlPath);
        }

        public static void DeleteCampaign()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlPath);
           XmlNodeList campaigns = xmlDoc.GetElementsByTagName("campaign");
            XmlNode joe = null;
            foreach (XmlNode campaign in campaigns)
            {
                if (campaign.Attributes["status"].Value == "active")
                    joe = campaign;
            }
            string dest = System.IO.Directory.GetCurrentDirectory() + "\\DB" + GetActiveCampaignDirectory() + ".db";
            System.IO.File.Delete(dest);
            joe.ParentNode.RemoveChild(joe);
            xmlDoc.Save(xmlPath);
        }

        public static string GetActiveCampaignDirectory()
        {
            using (XmlReader xmlReader = XmlReader.Create(xmlPath))
            {
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
        }

        public static string GetActiveCampaignName()
        {
            using (XmlReader xmlReader = XmlReader.Create(xmlPath)) {
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
        }

        public static void SetActiveCampaignName(string newName)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlPath);
            XmlNodeList campaigns = xmlDoc.GetElementsByTagName("campaign");
            foreach (XmlNode campaign in campaigns)
            {
                if (campaign.Attributes["status"].Value == "active")
                    campaign.Attributes["name"].Value = newName;
            }
            xmlDoc.Save(xmlPath);
        }

        public static List<string> GetCampaignNames()
        {
            List<string> campaigns = new List<string>();
            using (XmlReader xmlReader = XmlReader.Create(xmlPath))
            {
                while (xmlReader.Read())
                {
                    if ((xmlReader.NodeType == XmlNodeType.Element) && (xmlReader.Name == "campaign"))
                    {
                        if (xmlReader.HasAttributes)
                            campaigns.Add(xmlReader.GetAttribute("name"));
                    }
                }
                return campaigns;
            }
        }

        public static List<string> GetCampaignDirectories() {
            List<string> campaigns = new List<string>();
            using (XmlReader xmlReader = XmlReader.Create(xmlPath))
            {
                while (xmlReader.Read())
                {
                    if ((xmlReader.NodeType == XmlNodeType.Element) && (xmlReader.Name == "campaign"))
                    {
                        if (xmlReader.HasAttributes)
                            campaigns.Add(xmlReader.GetAttribute("directory"));
                    }
                }
                return campaigns;
            }
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
    }
}
