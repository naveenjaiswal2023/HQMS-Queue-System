
using HQMS.API.Application.DTO;
using HQMS.API.Domain.Entities;
using HQMS.API.Domain.Enum;
using HQMS.API.Domain.Interfaces;
using HQMS.API.Shared.Helpers;
using HQMS.Domain.Entities;
using HQMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HQMS.Infrastructure.Repositories
{
    public class QueueRepository : IQueueRepository
    {
        private readonly ApplicationDbContext _context;

        public QueueRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(QueueItem item)
        {
            await _context.QueueItems.AddAsync(item);
        }

        public async Task DeleteAsync(QueueItem entity)
        {
            _context.QueueItems.Remove(entity);
            await Task.CompletedTask; // if your UoW handles SaveChangesAsync
        }

        public async Task<IEnumerable<QueueItem>> GetAllAsync()
        {
            return await _context.QueueItems.ToListAsync();
        }

        public async Task<QueueItem> GetByIdAsync(Guid id)
        {
            return await _context.QueueItems.FirstOrDefaultAsync(q => q.Id == id);
        }

        public async Task<int> GetNextPositionAsync(Guid doctorId)
        {
            var maxPosition = await _context.QueueItems
                .Where(q => q.DoctorId == doctorId && q.Status != QueueStatus.Completed)
                .MaxAsync(q => (int?)q.Position) ?? 0;

            return maxPosition + 1;
        }

        public async Task<List<QueueItem>> GetWaitingPatientsByDepartmentAsync(Guid departmentId)
        {
            return await _context.QueueItems
                .Where(q => q.DepartmentId == departmentId && q.Status == 0)
                .ToListAsync();
        }
        public async Task<List<QueueItem>> GetQueueItemsForDoctorByDateAsync(Guid doctorId, DateTime date)
        {
            var startOfDay = date.Date;
            var endOfDay = startOfDay.AddDays(1);

            return await _context.QueueItems
                .Where(q => q.DoctorId == doctorId && q.JoinedAt >= startOfDay && q.JoinedAt < endOfDay)
                .OrderBy(q => q.Position)
                .ToListAsync();
        }
        public async Task<QueueItem?> GetByAppointmentIdAsync(Guid appointmentId)
        {
            return await _context.QueueItems
                .FirstOrDefaultAsync(q => q.AppointmentId == appointmentId);
        }
        public async Task UpdateAsync(QueueItem entity)
        {
            _context.QueueItems.Update(entity);
            await Task.CompletedTask;
        }

        public async Task<List<QueueDashboardItemDto>> GetDashboardDataAsync(Guid? hospitalId, Guid? departmentId, IEnumerable<Guid> doctorIds)
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            var query = from queue in _context.QueueItems
                        join appointment in _context.Appointments on queue.AppointmentId equals appointment.Id
                        join patient in _context.Patients on appointment.PatientId equals patient.PatientId into patientJoin
                        from patient in patientJoin.DefaultIfEmpty()
                        join doctor in _context.Doctors on appointment.DoctorId equals doctor.Id into doctorJoin
                        from doctor in doctorJoin.DefaultIfEmpty()
                        join department in _context.Departments on queue.DepartmentId equals department.DepartmentId into deptJoin
                        from department in deptJoin.DefaultIfEmpty()
                        where !queue.IsDeleted &&
                              appointment.AppointmentTime >= today &&
                              appointment.AppointmentTime < tomorrow
                        select new
                        {
                            queue,
                            appointment,
                            patient,
                            doctor,
                            department
                        };

            if (hospitalId.HasValue)
            {
                query = query.Where(x => x.appointment.HospitalId == hospitalId.Value);
            }

            if (departmentId.HasValue)
            {
                query = query.Where(x => x.queue.DepartmentId == departmentId.Value);
            }

            if (doctorIds != null && doctorIds.Any())
            {
                query = query.Where(x => x.doctor != null && doctorIds.Contains(x.doctor.Id));
            }

            var result = await query
                .OrderBy(x => x.appointment.AppointmentTime)
                .Select(x => new QueueDashboardItemDto
                {
                    QueueNumber = x.queue.QueueNumber,
                    PatientName = x.patient != null ? x.patient.Name : "Unknown",
                    DoctorName = x.doctor != null ? x.doctor.FirstName + " " + x.doctor.LastName : "Unknown",
                    AppointmentTime = x.appointment.AppointmentTime,
                    Department = x.department != null ? x.department.DepartmentName : "Unknown",
                    Status = EnumHelper.GetName<QueueStatus>(x.queue.Status)
                })
                .ToListAsync();

            return result;
        }


        public Task<List<QueueEntry>> GetQueueByDoctorIdAsync(int doctorId)
        {
            throw new NotImplementedException();
        }

        Task<int> IRepository<QueueItem>.UpdateAsync(QueueItem entity)
        {
            throw new NotImplementedException();
        }

        public Task<int> DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task RemoveRange(IEnumerable<QueueItem> entities)
        {
            throw new NotImplementedException();
        }

        Task<List<QueueEntry>> IQueueRepository.GetQueueByDoctorIdAsync(int doctorId)
        {
            throw new NotImplementedException();
        }
    }
}
