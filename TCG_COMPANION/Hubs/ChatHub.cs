using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace TCG_COMPANION.Hubs
{
    public class ChatHub : Hub
    {
        public async Task Message(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}