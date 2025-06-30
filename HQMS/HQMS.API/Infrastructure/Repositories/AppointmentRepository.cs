
using HQMS.API.Domain.Entities;
using HQMS.API.Domain.Interfaces;
using HQMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HQMS.API.Infrastructure.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly ApplicationDbContext _context;

        public AppointmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task AddAsync(Appointment entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Appointment entity)
        {
            throw new NotImplementedException();
        }

        public Task<int> DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Appointment>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<List<Appointment>> GetAppointmentsWithinNextMinutesAsync(int minutes)
        {
            var now = DateTime.Now;
            var threshold = now.AddMinutes(minutes);
            var today = now.Date;

            return await _context.Appointments
                .Where(a => a.AppointmentTime.Date == today) // Ensure it's today only
                .Where(a => a.AppointmentTime <= threshold && !a.QueueGenerated)
                .ToListAsync();
        }

        public async Task<Appointment?> GetByDoctorAndPatientAsync(Guid doctorId, Guid patientId)
        {
            var currentDateTime = DateTime.Now;

            return await _context.Appointments
                .FirstOrDefaultAsync(a =>
                    a.DoctorId == doctorId &&
                    a.PatientId == patientId &&
                    a.AppointmentTime.Date == currentDateTime.Date &&
                    a.AppointmentTime >= currentDateTime &&
                    a.QueueGenerated == false);
        }

        public async Task<List<Appointment>> GetAppointmentsForDoctorByDateAsync(Guid doctorId, DateTime date)
        {
            return await _context.Appointments
                .Where(a => a.DoctorId == doctorId && a.AppointmentTime.Date == date.Date)
                .OrderBy(a => a.AppointmentTime)
                .ToListAsync();
        }

        public Task<Appointment> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task RemoveRange(IEnumerable<Appointment> entities)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Appointment entity)
        {
            throw new NotImplementedException();
        }

        Task<int> IRepository<Appointment>.UpdateAsync(Appointment entity)
        {
            throw new NotImplementedException();
        }
    }
}
