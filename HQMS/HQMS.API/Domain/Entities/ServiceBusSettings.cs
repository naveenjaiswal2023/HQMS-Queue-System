using HQMS.Domain.Entities;

namespace HQMS.API.Domain.Entities
{
    public class ServiceBusSettings
    {
        //public string TopicName { get; set; }
        public List<TopicSubscriptionPair> TopicSubscriptions { get; set; } = new();
    }
}
