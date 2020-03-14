using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
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
    }
}
