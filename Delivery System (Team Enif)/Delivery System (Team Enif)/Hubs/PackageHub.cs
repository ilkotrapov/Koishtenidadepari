using Microsoft.AspNetCore.SignalR;

namespace Delivery_System__Team_Enif_.Hubs
{
    public class PackageHub : Hub 
    {
        public async Task JoinGroup(string trackingNumber)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, trackingNumber);
        }

        public async Task UpdateLocation(string trackingNumber, double lat, double lng)
        {
            await Clients.Group(trackingNumber).SendAsync("LocationUpdated", lat, lng);
        }
    }
}
