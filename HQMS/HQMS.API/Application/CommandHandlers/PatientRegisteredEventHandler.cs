using HQMS.Domain.Events;

namespace HQMS.Application.Handlers
{
    public class PatientRegisteredEventHandler
    {
        public Task HandleAsync(PatientRegisteredEvent @event)
        {
            // Handle the patient registered event
            return Task.CompletedTask;
        }
    }
}
