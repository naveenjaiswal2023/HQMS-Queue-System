namespace HQMS.API.Domain.Enum
{
    public enum QueueStatus
    {
        Pending = 0,
        InProgress = 1,
        Completed = 2,
        Cancelled = 3,
        Called = 4,
        Snooze = 5,
    }
}
