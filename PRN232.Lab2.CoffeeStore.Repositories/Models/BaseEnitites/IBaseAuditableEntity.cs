namespace PRN232.Lab2.CoffeeStore.Repositories.Models.BaseEnitites
{
    public interface IBaseAuditableEntity
    {
        DateTime CreatedAt { get; set; }
        DateTime UpdatedAt { get; set; }
        DateTime? DeletedAt { get; set; }
        bool IsActive { get; set; }
    }
}