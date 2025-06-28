
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

            // Step 1: Check if queue already exists
            var existingQueue = await _unitOfWork.QueueRepository
                .GetByAppointmentIdAsync(request.AppointmentId);

            if (existingQueue != null)
                return false;

            // Step 2: Get appointment
            var appointment = await _unitOfWork.AppointmentRepository
                .GetByDoctorAndPatientAsync(request.DoctorId, request.PatientId);

            if (appointment == null)
                throw new InvalidOperationException("Appointment not found for the given doctor and patient.");

            // Step 3: Get doctor with department
            var doctor = await _unitOfWork.DoctorRepository.GetByIdAsync(request.DoctorId);
            if (doctor == null || doctor.DepartmentId == Guid.Empty)
                throw new InvalidOperationException("Doctor or Department not found.");

            // Step 4: Time window check
            var now = DateTime.Now; // ✅ use UTC
            var timeDifference = now - appointment.AppointmentTime;

            if (timeDifference.Duration() > TimeSpan.FromMinutes(15))
                return false;

            // Step 5: Get today's appointments
            var allAppointmentsToday = await _unitOfWork.AppointmentRepository
                .GetAppointmentsForDoctorByDateAsync(request.DoctorId, today);

            var orderedAppointments = allAppointmentsToday
                .OrderBy(a => a.AppointmentTime)
                .ToList();

            var index = orderedAppointments.FindIndex(a => a.Id == appointment.Id);
            if (index == -1)
                throw new InvalidOperationException("Appointment not found in today's schedule.");

            int nextPosition = index + 1;

            string doctorPrefix = request.DoctorId.ToString().Substring(0, 4).ToUpper();
            string queueNumber = $"DR{doctorPrefix}-{nextPosition}";

            // Step 6: Create QueueItem — this raises PatientQueuedEvent automatically
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

            // 🔥 This will save + trigger PatientQueuedEvent
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }

    }
}
