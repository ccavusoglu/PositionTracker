using System;
using PositionTracker.Utility;
using PositionTracker.WebUI.Controllers;
using PositionTracker.WebUI.Models;

namespace PositionTracker.WebUI.Hub
{
    public class MainHub : Microsoft.AspNetCore.SignalR.Hub
    {
        private readonly Guid id = Guid.NewGuid();
        private readonly PositionController positionController;

        public MainHub(PositionController positionController)
        {
            this.positionController = positionController;
        }

        public void ButtonPressed(string button)
        {
            switch (button)
            {
                case "FetchPositions":
                    positionController.FetchPositions();

                    break;
                case "Save":
                    positionController.Save();

                    break;
            }
        }

        public string CellEdit(CellEditMessage message)
        {
            var retVal = "";

            if (message.Field == "Notes")
            {
                positionController.EditNotes(message.Coin, message.Exchange, message.Value);
            }
            else
            {
                Logger.LogDebug($"Invalid Edit: {message.MessageType} {message.Coin} " +
                                $"{message.Exchange} {message.Field} {message.Value}");
            }

            return retVal;
        }
    }
}