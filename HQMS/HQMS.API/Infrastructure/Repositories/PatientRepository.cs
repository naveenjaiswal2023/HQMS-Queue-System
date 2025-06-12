using HospitalQueueSystem.Domain.Entities;
using HospitalQueueSystem.Infrastructure.Data;
using HQMS.API.Domain.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace HQMS.Infrastructure.Repositories
{
    public class PatientRepository : IRepository<Patient>
    {
        private readonly ApplicationDbContext _context;

        public PatientRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Patient patient)
        {
            await _context.Patients.AddAsync(patient);
        }

        public async Task<Patient?> GetByIdAsync(Guid id)
        {
            return await _context.Patients.FindAsync(id);
        }

        public async Task<IEnumerable<Patient>> GetAllAsync()
        {
            return await _context.Patients.ToListAsync();
        }

        public async Task<int> UpdateAsync(Patient model)
        {
            var parameters = new[]
            {
                new SqlParameter("@PatientId", model.PatientId),
                new SqlParameter("@Name", model.Name),
                new SqlParameter("@Age", model.Age),
                new SqlParameter("@Gender", model.Gender),
                new SqlParameter("@Department", model.Department),
                new SqlParameter("@PhoneNumber", model.PhoneNumber),
                new SqlParameter("@Email", model.Email),
                new SqlParameter("@Address", model.Address),
                new SqlParameter("@BloodGroup", model.BloodGroup),
                new SqlParameter("@HospitalId", model.HospitalId),
                new SqlParameter("@DoctorId", model.PrimaryDoctorId),
                new SqlParameter("@UpdatedAt", model.ModifiedAt)
            };

            return await _context.Database.ExecuteSqlRawAsync(
                "EXEC sp_UpdatePatient @PatientId, @Name, @Age, @Gender, @Department, @PhoneNumber, @Email, @Address, @BloodGroup, @HospitalId, @DoctorId, @UpdatedAt",
                parameters);

        }

        public async Task<int> DeleteAsync(Guid patientId)
        {
            var param = new SqlParameter("@PatientId", patientId);
            return await _context.Database.ExecuteSqlRawAsync(
                "EXEC sp_DeletePatient @PatientId", param);
        }

        public Task RemoveRange(IEnumerable<Patient> entities)
        {
            throw new NotImplementedException();
        }
    }
}