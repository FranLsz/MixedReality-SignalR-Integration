using Microsoft.AspNet.SignalR;
using SignalRService.Models;

namespace SignalRService.Hubs
{
    public class HoloHub : Hub
    {
        public void SendTransform(TransformModel transformModel)
        {
            Clients.Others.SendTransform(transformModel);
        }
    }
}