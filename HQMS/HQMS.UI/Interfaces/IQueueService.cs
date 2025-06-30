using HQMS.UI.Models;

namespace HQMS.UI.Interfaces
{
    public interface IQueueService
    {
        Task<List<QueueDashboardItemDto>> GetDashboardAsync(Guid? hospitalId, Guid? departmentId, Guid? doctorId);

    }

}
