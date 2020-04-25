using KreemMachineLibrary.Models;
using KreemMachineLibrary.Models.Statics;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KreemMachineLibrary.Services
{
    public class StockService
    {
        public void CreateRequestFromOpenStage(RestockStage stage)
        {
            stage.Type = RestockStageType.Open;
            using (var db = new DataBaseContext())
            {
                db.Users.Attach(stage.User);
                db.RestockStages.Add(stage);
                db.SaveChanges();
            }
        }

        public async Task<List<RestockRequest>> GetActiveRequestsAsync()
        {
            string RESTOCK_STAGE_HIDE = RestockStageType.Hide.ToString();
            using (var db = new DataBaseContext())
            {
                return await db.RestockRequests
                    .Where(r => !r.Stages.Any(s => s.TypeStr == RESTOCK_STAGE_HIDE))
                    .Include(r => r.Product.Department)
                    .Include(r => r.Stages.Select(s => s.User))
                    .ToListAsync();
            }
        }
    }
}
