using MySql.Data.MySqlClient;
using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Unturned.Chat;
using System;
using System.Collections.Generic;
using Steamworks;
using Rocket.Unturned.Player;
using SDG.Unturned;

namespace Traffic_Logging
{
    public class DatabaseController
    {
        private MySqlConnection createConnection()
        {
            MySqlConnection connection = null;
            try
            {
                if (Plugin.Instance.Configuration.Instance.DatabasePort == 0) Plugin.Instance.Configuration.Instance.DatabasePort = 3306;
                connection = new MySqlConnection(String.Format("SERVER={0};DATABASE={1};UID={2};PASSWORD={3};PORT={4};", Plugin.Instance.Configuration.Instance.DatabaseAddress, Plugin.Instance.Configuration.Instance.DatabaseName, Plugin.Instance.Configuration.Instance.DatabaseUsername, Plugin.Instance.Configuration.Instance.DatabasePassword, Plugin.Instance.Configuration.Instance.DatabasePort));
            }
            catch (Exception ex)
            {

            }
            return connection;
        }

        public DatabaseController()
        {
            new I18N.West.CP1250();
            CheckSchema();
        }

        public void CheckSchema()
        {
            try
            {
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "show tables like '" + Plugin.Instance.Configuration.Instance.TableName + "'";
                connection.Open();
                object test = command.ExecuteScalar();

                if (test == null)
                {
                    command.CommandText = "CREATE TABLE `" + Plugin.Instance.Configuration.Instance.TableName + "` ( `id` INT NOT NULL AUTO_INCREMENT , `steamId` VARCHAR(255) NOT NULL , `playerName` TEXT NOT NULL , `joined_on` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP, `last_login` TIMESTAMP NULL, last_logout TIMESTAMP NULL, PRIMARY KEY (`id`), UNIQUE (`steamId`));";
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        public bool Exists(string steamId)
        {
            bool ret = false;
            try
            {
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select `joined_on` from `" + Plugin.Instance.Configuration.Instance.TableName + "` where `steamId` = '" + steamId + "';";
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null) ret = true;
                connection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return ret;
        }

        public DateTime LastPlayed(string steamId)
        {
            string mySqlTimestamp = "2003-12-31 00:00:00";
            
            try
            {
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select `last_login` from `" + Plugin.Instance.Configuration.Instance.TableName + "` where `steamId` = '" + steamId + "';";
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null) mySqlTimestamp = result.ToString();
                connection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return DateTime.Parse(mySqlTimestamp);
        }
        

        public void UpdateTimes(UnturnedPlayer player)
        {
            try
            {
                string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                command.Parameters.AddWithValue("@csteamid", player.CSteamID);
                command.Parameters.AddWithValue("@playerName", player.DisplayName);
                command.Parameters.AddWithValue("@lastlogout", date);

                string query = "insert into `" + Plugin.Instance.Configuration.Instance.TableName + "` (`steamId`,`playerName`,`last_logout`) values(@csteamid,@playerName,@lastlogout) on duplicate key update `playerName`=@playerName, `last_logout`=@lastlogout;";
                command.CommandText = query;
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                Rocket.Core.Logging.Logger.LogException(ex);
            }
        }

        public void AddPlayer(UnturnedPlayer player)
        {
            try
            {
                string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                command.Parameters.AddWithValue("@csteamid", player.CSteamID);
                command.Parameters.AddWithValue("@playerName", player.DisplayName);
                command.Parameters.AddWithValue("@joinedon", date);
                command.Parameters.AddWithValue("@lastlogin", date);
                string query = "insert into `" + Plugin.Instance.Configuration.Instance.TableName + "` (`steamId`,`playerName`,`last_login`) values(@csteamid,@playerName,@lastlogin) on duplicate key update `playerName`=@playerName, `last_login`=@lastlogin;";
                if (Exists(player.CSteamID.ToString()))
                {
                    query = "insert into `" + Plugin.Instance.Configuration.Instance.TableName + "` (`steamId`,`playerName`, `joined_on`,`last_login`) values(@csteamid,@playerName,@joinedon,@lastlogin) on duplicate key update `playerName`=@playerName, `last_login`=@lastlogin;";
                }
                command.CommandText = query;
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                Rocket.Core.Logging.Logger.LogException(ex);
            }
        }
    }
}