using KreemMachineLibrary.Models;
using KreemMachineLibrary.Models.Statics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KreemMachineLibrary.Services
{
    public class StockService
    {
        public void CreateRequestFromOpenStage(RestockStage stage)
        {
            stage.Status = RestockStageType.Open;
            using (var db = new DataBaseContext())
            {
                db.Users.Attach(stage.User);
                db.RestockStages.Add(stage);
                db.SaveChanges();
            }
        }
    }
}
