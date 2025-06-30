
using HQMS.API.Application.CommandModel;
using HQMS.API.Domain.Entities;
using HQMS.Application.Common;
using MediatR;

namespace HQMS.API.Application.CommandHandlers
{
    public class GenerateDoctorQueueCommandHandler : IRequestHandler<GenerateDoctorQueueCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDomainEventPublisher _domainEventPublisher;

        public GenerateDoctorQueueCommandHandler(IUnitOfWork unitOfWork, IDomainEventPublisher domainEventPublisher)
        {
            _unitOfWork = unitOfWork;
            _domainEventPublisher = domainEventPublisher;
        }

        public async Task<bool> Handle(GenerateDoctorQueueCommand request, CancellationToken cancellationToken)
        {
            var today = DateTime.Now.Date;

            // Step 1: Check if queue already exists for this appointment
            var existingQueue = await _unitOfWork.QueueRepository
                .GetByAppointmentIdAsync(request.AppointmentId);

            if (existingQueue != null)
                return false;

            // Step 2: Get appointment
            var appointment = await _unitOfWork.AppointmentRepository
                .GetByDoctorAndPatientAsync(request.DoctorId, request.PatientId);

            if (appointment == null)
                throw new InvalidOperationException("Appointment not found for the given doctor and patient.");

            // Step 3: Get doctor with department and hospital
            var doctor = await _unitOfWork.DoctorRepository.GetByIdAsync(request.DoctorId);
            if (doctor == null || doctor.DepartmentId == Guid.Empty || doctor.HospitalId == Guid.Empty)
                throw new InvalidOperationException("Doctor, department, or hospital not found.");

            // Step 4: Time window check
            var now = DateTime.Now;
            var timeDifference = now - appointment.AppointmentTime;

            if (timeDifference.Duration() > TimeSpan.FromMinutes(15))
                return false;

            // Step 5: Get existing queues for today by Doctor + Department + Hospital
            var existingQueues = await _unitOfWork.QueueRepository.GetQueuesByDoctorDepartmentHospitalAsync(
            doctor.Id,
            doctor.DepartmentId.Value,
            doctor.HospitalId,
            today);

            int nextPosition = existingQueues.Count + 1;

            string hospitalPrefix = doctor.HospitalId.ToString().Substring(0, 3).ToUpper();
            string departmentPrefix = doctor.DepartmentId.ToString().Substring(0, 3).ToUpper();
            string doctorPrefix = doctor.Id.ToString().Substring(0, 3).ToUpper();
            string queueNumber = $"Q-{hospitalPrefix}-{departmentPrefix}-{doctorPrefix}-{nextPosition}";

            // Step 6: Create QueueItem
            var queueItem = new QueueItem(
                Guid.NewGuid(),
                request.DoctorId,
                request.PatientId,
                request.AppointmentId,
                doctor.DepartmentId.Value,
                nextPosition,
                TimeSpan.FromMinutes(15),
                queueNumber
            );

            await _unitOfWork.QueueRepository.AddAsync(queueItem);

            appointment.MarkQueueGenerated();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }

    }
}
