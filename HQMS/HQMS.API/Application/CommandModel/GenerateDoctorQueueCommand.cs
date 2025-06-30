using MediatR;

namespace HQMS.API.Application.CommandModel
{
    public class GenerateDoctorQueueCommand : IRequest<bool>
    {
        public Guid DoctorId { get; }
        public Guid PatientId { get; }
        public Guid AppointmentId { get; }
        public Guid DepartmentId { get; }
        public DateTime AppointmentTime { get; }
        public DateTime JoinedAt { get; }
        public DateTime? CalledAt { get; }

        public GenerateDoctorQueueCommand(
            Guid doctorId,
            Guid patientId,
            Guid appointmentId,
            Guid departmentId,
            DateTime appointmentTime,
            DateTime joinedAt,
            DateTime? calledAt)
        {
            DoctorId = doctorId;
            PatientId = patientId;
            AppointmentId = appointmentId;
            DepartmentId = departmentId;
            AppointmentTime = appointmentTime;
            JoinedAt = joinedAt;
            CalledAt = calledAt;
        }
    }
}
