
using HQMS.API.Application.CommandModel;
using HQMS.API.Domain.Entities;
using HQMS.API.Domain.Events;
using MediatR;

namespace HQMS.API.Infrastructure
{
    public class QueueScheduler : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly int _intervalMinutes;

        private readonly ILogger<QueueScheduler> _logger;

        public QueueScheduler(IServiceProvider serviceProvider, IConfiguration config, ILogger<QueueScheduler> logger)
        {
            _serviceProvider = serviceProvider;
            _intervalMinutes = config.GetValue<int>("QueueScheduler:IntervalMinutes", 1);
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceProvider.CreateScope();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                var upcomingAppointments = await unitOfWork.AppointmentRepository
                    .GetAppointmentsWithinNextMinutesAsync(15);

                foreach (var appt in upcomingAppointments)
                {
                    try
                    {
                        // ✅ Just send command to handler — let it do everything else
                        var command = new GenerateDoctorQueueCommand(
                            appt.DoctorId,
                            appt.PatientId,
                            appt.Id,
                            appt.Patient?.DepartmentId ?? Guid.Empty,           // Get from navigation or flat property
                            appt.AppointmentTime,
                            DateTime.Now,            // JoinedAt
                            null                        // CalledAt
                        );

                        await mediator.Send(command, stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Error scheduling queue for appointment {appt.Id}: {ex.Message}");
                        // optionally log
                    }
                }

                await Task.Delay(TimeSpan.FromMinutes(_intervalMinutes), stoppingToken);
            }
        }

    }
}
