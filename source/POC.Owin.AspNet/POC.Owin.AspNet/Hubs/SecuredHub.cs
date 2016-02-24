using Microsoft.AspNet.SignalR;

namespace POC.Owin.AspNet.Hubs
{
    [Authorize(Roles = "admin")]
    public class SecuredHub : Hub
    {
        public void Secret(string data)
        {
            Clients.Caller.replySecret($"this is a secret: {data}");
        }
    }
}
