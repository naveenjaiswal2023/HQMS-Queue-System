
using HQMS.API.Domain.Entities;
using HQMS.API.Domain.Interfaces;
using HQMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;

namespace HQMS.API.Application.Services
{
    public class HospitalDataSyncService : BackgroundService
    {
        private readonly ILogger<HospitalDataSyncService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public HospitalDataSyncService(IServiceProvider serviceProvider, ILogger<HospitalDataSyncService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    await SyncDepartmentsAsync(scope.ServiceProvider, stoppingToken);
                    await SyncDoctorSlotsAsync(scope.ServiceProvider, stoppingToken);
                    await SyncAppointmentsAsync(scope.ServiceProvider, stoppingToken);

                    _logger.LogInformation("Data sync completed at {time}", DateTimeOffset.Now);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Data sync failed");
                }

                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }

        private async Task SyncDepartmentsAsync(IServiceProvider scopedProvider, CancellationToken cancellationToken)
        {
            var apiService = scopedProvider.GetRequiredService<IExternalHospitalService>();
            var dbContext = scopedProvider.GetRequiredService<ApplicationDbContext>();

            var departments = await apiService.GetDepartmentsAsync();

            foreach (var dept in departments)
            {
                var entity = await dbContext.Departments
                    .FirstOrDefaultAsync(d => d.DepartmentId == dept.Id, cancellationToken);

                //if (entity == null)
                //{
                //    var newDepartment = new Department
                //    {
                //        DepartmentId = dept.Id,
                //        DepartmentName = dept.Name
                //    };

                //    dbContext.Departments.Add(newDepartment);
                //}
                //else
                //{
                //    // Update only if value has changed
                //    if (entity.DepartmentName != dept.Name)
                //    {
                //        entity.DepartmentName = dept.Name;
                //        dbContext.Departments.Update(entity); // Optional: EF tracks automatically
                //    }
                //}
            }

            await dbContext.SaveChangesAsync(cancellationToken);
        }


        private async Task SyncDoctorSlotsAsync(IServiceProvider scopedProvider, CancellationToken cancellationToken)
        {
            var apiService = scopedProvider.GetRequiredService<IExternalHospitalService>();
            var dbContext = scopedProvider.GetRequiredService<ApplicationDbContext>();

            var doctorIds = await dbContext.DoctorSlots.Select(d => d.Id).ToListAsync(cancellationToken);

            foreach (var doctorId in doctorIds)
            {
                var slots = await apiService.GetDoctorSlotsAsync(doctorId);

                foreach (var slot in slots)
                {
                    var entity = await dbContext.DoctorSlots.FindAsync(new object[] { slot.DoctorId, slot.StartTime }, cancellationToken);
                    if (entity == null)
                    {
                        dbContext.DoctorSlots.Add(new DoctorSlot
                        {
                            DoctorId = slot.DoctorId,
                            StartTime = slot.StartTime,
                            EndTime = slot.EndTime
                        });
                    }
                    else
                    {
                        entity.EndTime = slot.EndTime;
                    }
                }
            }

            await dbContext.SaveChangesAsync(cancellationToken);
        }

        private async Task SyncAppointmentsAsync(IServiceProvider scopedProvider, CancellationToken cancellationToken)
        {
            var apiService = scopedProvider.GetRequiredService<IExternalHospitalService>();
            var dbContext = scopedProvider.GetRequiredService<ApplicationDbContext>();
            var logger = scopedProvider.GetRequiredService<ILogger<HospitalDataSyncService>>();

            var patientIds = await dbContext.Patients.Select(p => p.PatientId).ToListAsync(cancellationToken);

            foreach (var pId in patientIds)
            {
                try
                {
                    var appointments = await apiService.GetAppointmentsByPatientIdAsync(pId);

                    foreach (var appt in appointments)
                    {
                        var existing = await dbContext.Appointments.FindAsync(new object[] { appt.Id }, cancellationToken);

                        if (existing == null)
                        {
                            //dbContext.Appointments.Add(new Appointment(
                            //    appt.Id, // Pass the required 'id' parameter
                            //    appt.PatientId,
                            //    appt.DoctorId,
                            //    appt.AppointmentTime
                            //));
                        }
                        else
                        {
                            // Use a method to update the properties since the setters are private
                            existing.SetModified("System");
                            existing.UpdateAppointmentDetails(appt.DoctorId, appt.HospitalId, appt.AppointmentTime);
                        }
                    }

                    await dbContext.SaveChangesAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to sync appointments for patient {PatientId}", pId);
                }
            }
        }
    }

}
