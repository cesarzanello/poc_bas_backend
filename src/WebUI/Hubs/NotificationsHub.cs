using Microsoft.AspNetCore.SignalR;

namespace WebUI.Hubs
{
    public class NotificationsHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var tenantIdRaw = Context.GetHttpContext()?.Request.Query["tenantId"].ToString();
            if (Guid.TryParse(tenantIdRaw, out var tenantId) && tenantId != Guid.Empty)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, GetTenantGroupName(tenantId));
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var tenantIdRaw = Context.GetHttpContext()?.Request.Query["tenantId"].ToString();
            if (Guid.TryParse(tenantIdRaw, out var tenantId) && tenantId != Guid.Empty)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, GetTenantGroupName(tenantId));
            }

            await base.OnDisconnectedAsync(exception);
        }

        public static string GetTenantGroupName(Guid tenantId) => $"tenant:{tenantId}";
    }
}
