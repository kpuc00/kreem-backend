using KreemMachineLibrary.Models;
using KreemMachineLibrary.Models.Statics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace KreemMachine.ViewModels
{
    internal class RestockRequestViewModel
    {
        public RestockRequest Request { get; }

        public List<Button> Buttons { get; } = new List<Button>();

        public event EventHandler<RestockRequest> RequestApproved;
        public event EventHandler<RestockRequest> RequestDenied;
        public event EventHandler<RestockRequest> RequestRestocked;
        public event EventHandler<RestockRequest> RequestHidden;

        public RestockRequestViewModel(RestockRequest request)
        {
            Request = request;

            switch (request.LatestStage.Type)
            {
                case RestockStageType.Open:
                    AddButton("Approve", () => RequestApproved);
                    AddButton("Deny", () => RequestDenied);
                    break;
                case RestockStageType.Approve:
                    AddButton("Restock", () => RequestRestocked);
                    break;
                case RestockStageType.Deny:
                case RestockStageType.Restock:
                    AddButton("Hide", () => RequestHidden);
                    break;
            }
        }

        private void AddButton(string title, Func<EventHandler<RestockRequest>> getHandler)
        {
            var newButton = new Button();
            newButton.Content = title;
            newButton.Click += (sender, e) => getHandler()?.Invoke(sender, Request);
            Buttons.Add(newButton);
        }

    }
}