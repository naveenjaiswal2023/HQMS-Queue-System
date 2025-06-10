using System.Text.Json.Serialization;

namespace HospitalQueueSystem.Web.Models
{
    public class PatientModel
    {
        public Guid PatientId { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string Department { get; set; }
        public DateTime RegisteredAt { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string BloodGroup { get; set; }
        public Guid HospitalId { get; set; }
        public Guid DoctorId { get; set; }
    }

}
