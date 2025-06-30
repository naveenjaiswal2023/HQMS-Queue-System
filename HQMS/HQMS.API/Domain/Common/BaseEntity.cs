using HQMS.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace HQMS.Domain.Entities.Common
{
    public abstract class BaseEntity
    {
        // -----------------------------
        // Domain Events
        // -----------------------------
        private readonly List<IDomainEvent> _domainEvents = new();
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        protected void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
        public void ClearDomainEvents() => _domainEvents.Clear();

        // -----------------------------
        // Auditing
        // -----------------------------
        public string? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }

        public void SetModified(string modifiedBy)
        {
            ModifiedBy = modifiedBy;
            ModifiedAt = DateTime.UtcNow;
        }

        // -----------------------------
        // Soft Delete
        // -----------------------------
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }

        public void MarkAsDeleted(string deletedBy)
        {
            IsDeleted = true;
            DeletedBy = deletedBy;
            DeletedAt = DateTime.UtcNow;
        }

        // -----------------------------
        // Concurrency Token
        // -----------------------------
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
