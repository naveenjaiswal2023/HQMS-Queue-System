using HQMS.Domain.Entities.Common;

namespace HQMS.API.Domain.Entities
{
    public class Department : BaseEntity
    {
        public Guid DepartmentId { get;  set; }
        public string DepartmentName { get;  set; }

        public Guid? HospitalId { get; set; }             // Foreign Key
        public Hospital Hospital { get; set; }           // Navigation Property
    }
}
