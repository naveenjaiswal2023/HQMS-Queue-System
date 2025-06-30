
using Dapper;
using HQMS.API.Application.DTO;
using HQMS.API.Domain.Entities;
using HQMS.API.Domain.Enum;
using HQMS.API.Domain.Interfaces;
using HQMS.API.Shared.Helpers;
using HQMS.Domain.Entities;
using HQMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Data;

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

        //public async Task<List<QueueDashboardItemDto>> GetDashboardDataAsync(Guid? hospitalId, Guid? departmentId, IEnumerable<Guid> doctorIds)
        //{
        //    var today = DateTime.Today;
        //    var tomorrow = today.AddDays(1);

        //    try
        //    {
        //        var baseQuery = from queue in _context.QueueItems
        //                        join appointment in _context.Appointments on queue.AppointmentId equals appointment.Id
        //                        join patient in _context.Patients on appointment.PatientId equals patient.PatientId into patientJoin
        //                        from patient in patientJoin.DefaultIfEmpty()
        //                        join doctor in _context.Doctors on queue.DoctorId equals doctor.Id into doctorJoin
        //                        from doctor in doctorJoin.DefaultIfEmpty()
        //                        join department in _context.Departments on queue.DepartmentId equals department.DepartmentId into deptJoin
        //                        from department in deptJoin.DefaultIfEmpty()
        //                        join hospital in _context.Hospitals on appointment.HospitalId equals hospital.HospitalId
        //                        where !queue.IsDeleted &&
        //                              queue.JoinedAt >= today &&
        //                              queue.JoinedAt < tomorrow
        //                        select new
        //                        {
        //                            queue,
        //                            appointment,
        //                            patient,
        //                            doctor,
        //                            department,
        //                            hospital
        //                        };

        //        // ✅ Apply filters BEFORE switching to AsEnumerable()
        //        if (hospitalId.HasValue)
        //        {
        //            baseQuery = baseQuery.Where(x => x.appointment.HospitalId == hospitalId.Value);
        //        }

        //        if (departmentId.HasValue)
        //        {
        //            baseQuery = baseQuery.Where(x => x.queue.DepartmentId == departmentId.Value);
        //        }

        //        if (doctorIds != null && doctorIds.Any())
        //        {
        //            baseQuery = baseQuery.Where(x => doctorIds.Contains(x.queue.DoctorId));
        //        }

        //        // ✅ Now switch to in-memory for row-number like logic
        //        var groupedResult = baseQuery
        //            .AsEnumerable()
        //            .GroupBy(x => new { x.queue.DoctorId, x.appointment.HospitalId, x.queue.DepartmentId })
        //            .SelectMany(group => group
        //                .OrderBy(x => x.queue.JoinedAt)
        //                .Select((x, index) => new QueueDashboardItemDto
        //                {
        //                    QueueId = x.queue.Id, // original QueueItem.Id
        //                    QueueNumber = x.queue.QueueNumber,
        //                    PatientName = x.patient?.Name ?? "Unknown",
        //                    DoctorName = $"{x.doctor?.FirstName} {x.doctor?.LastName}".Trim(),
        //                    AppointmentTime = x.appointment.AppointmentTime,
        //                    Department = x.department?.DepartmentName ?? "Unknown",
        //                    HospitalName = x.hospital?.Name ?? "Unknown",
        //                    Status = EnumHelper.GetName<QueueStatus>(x.queue.Status)
        //                }))
        //            .ToList(); // ✅ No await here — purely in-memory now

        //        return groupedResult;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"[QueueRepository] GetDashboardDataAsync failed: {ex.Message}");
        //        return new List<QueueDashboardItemDto>(); // ✅ safe fallback
        //    }
        //}

        public async Task<List<QueueItem>> GetQueuesByDoctorDepartmentHospitalAsync(Guid doctorId,  Guid departmentId, Guid hospitalId, DateTime date)
        {
            return await (from q in _context.QueueItems
                          join a in _context.Appointments
                              on q.AppointmentId equals a.Id
                          where q.DoctorId == doctorId
                                && q.DepartmentId == departmentId
                                && a.HospitalId == hospitalId
                                && q.JoinedAt.Date == date
                          orderby q.JoinedAt
                          select q)
                .ToListAsync();
        }


        public async Task<List<QueueDashboardItemDto>> GetDashboardDataAsync(Guid? hospitalId, Guid? departmentId, IEnumerable<Guid> doctorIds)
        {
            using var connection = _context.GetConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@HospitalId", hospitalId, DbType.Guid);
            parameters.Add("@DepartmentId", departmentId, DbType.Guid);
            parameters.Add("@DoctorId", doctorIds?.FirstOrDefault(), DbType.Guid); // ✅ Explicit type

            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();

            var result = await connection.QueryAsync<QueueDashboardItemDto>(
                "GetQueueDashboardData",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result.ToList();
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
