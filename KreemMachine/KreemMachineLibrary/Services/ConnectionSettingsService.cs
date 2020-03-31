using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Configuration;
using System.Reflection;
using System.Data.SqlClient;

namespace KreemMachineLibrary.Services
{
    public class ConnectionSettingsService
    {
        public ConnectionSettingsService()
        {
        }

        internal string ReadJSON()
        {
            return File.ReadAllText(@"settings.json");
        }

        internal string WriteJSON(ConnectionSettingsDTO connectionSettingsDTO)
        {
            return JsonConvert.SerializeObject(connectionSettingsDTO);
        }

        public void SaveChanges(ConnectionSettingsDTO connectionSettingsDTO)
        {
            File.WriteAllText(@"settings.json", WriteJSON(connectionSettingsDTO));
        }

        public ConnectionSettingsDTO GetConnectionSettings()
        {
            var r = JsonConvert.DeserializeObject<ConnectionSettingsDTO>(ReadJSON());
            return r;
        }



        public void ChangeConnectionString(string newServer, string newUsername, string newPassword, string newDatabaseName)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(GetConnectionString());
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);


            builder.ConnectionString = "Server=studmysql01.fhict.local;Uid=dbi441044;Database=dbi441044;Pwd=qwerty;";

            builder["Server"] = newServer;
            builder["Uid"] = newUsername;
            builder["Database"] = newDatabaseName;
            builder["Pwd"] = newPassword;

            config.ConnectionStrings.ConnectionStrings["DataBaseContext"].ConnectionString = builder.ConnectionString;
            config.Save();
        }

        private static string GetConnectionString() => "Server=(studmysql01.fhict.local);Integrated Security=SSPI;" +
                                                        "Initial Catalog=dbi441044";
    }
}
