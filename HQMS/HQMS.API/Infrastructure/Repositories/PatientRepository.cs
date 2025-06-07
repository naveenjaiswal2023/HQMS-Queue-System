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

        public async Task<Patient?> GetByIdAsync(string id)
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
            };

            return await _context.Database.ExecuteSqlRawAsync(
                "EXEC UpdatePatient @PatientId, @Name, @Age, @Gender, @Department", parameters);
        }

        public async Task<int> DeleteAsync(string patientId)
        {
            var param = new SqlParameter("@PatientId", patientId);
            return await _context.Database.ExecuteSqlRawAsync(
                "EXEC DeletePatient @PatientId", param);
        }
    }
}