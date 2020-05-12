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
        public void CreateRequestFromOpenStage(RestockRequest request, int quantity)
        {

            RestockStage openStage = new RestockStage()
            {
                Quantity = quantity,
                Request = request,
                Type = RestockStageType.Open,
                UserId = SecurityContext.CurrentUser.Id,
            };

            AddStageToRequest(openStage);
        }

        private void AddStageToRequest(RestockStage stage)
        {
            using (var db = new DataBaseContext())
            {
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
                    .Where(r => r.Product.Deleted != 1)
                    .Include(r => r.Product.Department)
                    .Include(r => r.Stages.Select(s => s.User))
                    .ToListAsync();
            }
        }

        public void ApproveRequest(RestockRequest request, int quantity)
        {
            RestockStage acceptStage = new RestockStage()
            {
                Type = RestockStageType.Approve,
                Quantity = quantity,
                RequestId = request.Id,
                UserId = SecurityContext.CurrentUser.Id,
            };

            AddStageToRequest(acceptStage);
        }
        public void DenyRequest(RestockRequest request)
        {
            RestockStage acceptStage = new RestockStage()
            {
                Type = RestockStageType.Deny,
                RequestId = request.Id,
                UserId = SecurityContext.CurrentUser.Id,
            };
            AddStageToRequest(acceptStage);
        }
        public void RestockRequest(RestockRequest request, int quantity)
        {
            RestockStage restockStage = new RestockStage()
            {
                Type = RestockStageType.Restock,
                Quantity = quantity,
                RequestId = request.Id,
                UserId = SecurityContext.CurrentUser.Id,
            };

            using (var db = new DataBaseContext())
            {
                db.RestockStages.Add(restockStage);

                request.Product.Quantity += restockStage.Quantity ?? 0;
                db.Entry(request.Product).State = EntityState.Modified;

                db.SaveChanges();
            }
        }

        public void HideRequest(RestockRequest request)
        {
            RestockStage hideStage = new RestockStage()
            {
                Type = RestockStageType.Hide,
                RequestId = request.Id,
                UserId = SecurityContext.CurrentUser.Id,
            };
            AddStageToRequest(hideStage);
        }
    }
}
