using Microsoft.AspNetCore.SignalR;
using PositionTracker.WebUI.Models;

namespace PositionTracker.WebUI.Hub
{
    public static class HubExtensions
    {
        public static void SendMessage(this IHubContext<MainHub> hub, string type, object obj)
        {
            var data = new LiveDataModel(type, obj);

            hub.Clients.All.SendAsync("update", data);
        }
    }
}