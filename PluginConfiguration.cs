using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rocket.API;
using System.Xml.Serialization;
using UnityEngine;
using Steamworks;

namespace Traffic_Logging
{
    public class PluginConfiguration : IRocketPluginConfiguration
    {
        public string DatabaseAddress;
        public string DatabaseUsername;
        public string DatabasePassword;
        public string DatabaseName;
        public int DatabasePort;
        public string TableName;

        public void LoadDefaults()
        {
            DatabaseAddress = "localhost";
            DatabaseUsername = "Unturned";
            DatabasePassword = "password";
            DatabaseName = "Unturned";
            DatabasePort = 3306;
            TableName = "traffic_logging";
        }
    }
}
