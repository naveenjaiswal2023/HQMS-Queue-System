namespace HQMS.API.Domain.Interfaces
{
    public interface INotificationService
    {
        Task SendNotificationAsync(string method, string eventName, object message);
        Task SendNotificationAsync(string eventName, object message);
    }

}
