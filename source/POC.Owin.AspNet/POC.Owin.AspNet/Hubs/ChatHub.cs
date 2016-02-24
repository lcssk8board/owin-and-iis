using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using POC.Owin.AspNet.Models.Abstractions;

namespace POC.Owin.AspNet.Hubs
{
    public class ChatHub : Hub, IHub
    {
        private readonly ITest _Test;

        public ChatHub(ITest test)
        {
            _Test = test;
        }

        public void Send(string name, string message)
        {
            Clients.All.broadcastMessage(name, message);
        }
    }
}
