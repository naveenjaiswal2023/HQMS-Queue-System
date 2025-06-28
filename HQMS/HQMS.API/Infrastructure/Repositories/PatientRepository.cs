using HQMS.API.Domain.Entities;
using HQMS.API.Domain.Interfaces;
using HQMS.Domain.Entities;
using HQMS.Domain.Events;
using HQMS.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace HQMS.Infrastructure.Repositories
{
    public class PatientRepository : IPatientRepository
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

        public async Task<List<PatientRegisteredEvent>> GetAllPatientsAsync(CancellationToken cancellationToken = default)
        {
            return await (
                from patient in _context.Patients
                join hospital in _context.Hospitals on patient.HospitalId equals hospital.HospitalId
                join department in _context.Departments on patient.DepartmentId equals department.DepartmentId
                join doctor in _context.Doctors on patient.PrimaryDoctorId equals doctor.Id into doctorJoin
                from doctor in doctorJoin.DefaultIfEmpty() // 👈 handle null doctor
                orderby patient.CreatedAt descending
                select new PatientRegisteredEvent(
                    patient.PatientId,
                    patient.Name,
                    patient.Age,
                    patient.Gender,
                    department.DepartmentName,
                    patient.PhoneNumber,
                    patient.Email,
                    patient.Address,
                    patient.BloodGroup,
                    hospital.Name,
                    doctor != null ? doctor.FullName : "N/A"
                )
            ).ToListAsync(cancellationToken);
        }

    }
}