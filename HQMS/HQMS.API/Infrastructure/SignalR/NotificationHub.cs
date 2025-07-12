using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace HQMS.Infrastructure.SignalR
{
    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var user = Context.User;

            if (user?.Identity?.IsAuthenticated == true)
            {
                // Role-based group assignment
                if (user.IsInRole("Admin"))
                    await Groups.AddToGroupAsync(Context.ConnectionId, "ADMIN");

                if (user.IsInRole("Pod"))
                    await Groups.AddToGroupAsync(Context.ConnectionId, "POD");

                if (user.IsInRole("Nursing"))
                    await Groups.AddToGroupAsync(Context.ConnectionId, "NURSING");

                if (user.IsInRole("Doctor"))
                {
                    var doctorId = user.FindFirst("DoctorId")?.Value;
                    if (!string.IsNullOrEmpty(doctorId))
                        await Groups.AddToGroupAsync(Context.ConnectionId, $"DOCTOR_{doctorId}");
                }
            }

            await base.OnConnectedAsync();
        }

        // Optional: Allow manual joining if required
        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        // Optional: Allow leaving a group
        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }
    }
}
