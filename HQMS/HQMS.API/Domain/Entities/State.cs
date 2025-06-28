using HQMS.Domain.Entities.Common;

namespace HQMS.API.Domain.Entities
{
    public class State : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<City> Cities { get; set; }
    }
}
