using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rocket.Core.Plugins;
using Steamworks;
using UnityEngine;
using Rocket.Unturned.Player;
using Rocket.Unturned.Chat;
using Rocket.API.Collections;
using System.Timers;
using Rocket.Unturned.Events;
using Rocket.Unturned;

namespace Traffic_Logging
{
    public class Plugin : RocketPlugin<PluginConfiguration>
    {
        public static Plugin Instance;
        public static DatabaseController Database;
        /// <summary>
        /// ToDo:
        /// Add Shutdown Code
        /// log players in dictionary
        /// </summary>
        protected override void Load()
        {
            Instance = this;
            Database = new DatabaseController();
            U.Events.OnPlayerConnected += Events_OnPlayerConnected;
            U.Events.OnPlayerDisconnected += Events_OnPlayerDisconnected;
        }

        private void Events_OnPlayerDisconnected(UnturnedPlayer player)
        {
            Database.UpdateTimes(player);
        }

        private void Events_OnPlayerConnected(UnturnedPlayer player)
        {
            Database.AddPlayer(player);
        }

        protected override void Unload()
        {
            U.Events.OnPlayerConnected -= Events_OnPlayerConnected;
        }

        #region translations
        public override TranslationList DefaultTranslations
        {
            get { return new TranslationList() {}; }
        }
        #endregion
    }
}
