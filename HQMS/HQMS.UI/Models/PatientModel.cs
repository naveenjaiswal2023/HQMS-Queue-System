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
    }
}
