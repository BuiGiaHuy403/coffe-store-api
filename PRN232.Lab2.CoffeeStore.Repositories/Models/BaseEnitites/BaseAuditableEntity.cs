namespace PRN232.Lab2.CoffeeStore.Repositories.Models.BaseEnitites
{
    public class BaseAuditableEntity<TId> : BaseEntity<TId>, IBaseAuditableEntity
        where TId : notnull
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
