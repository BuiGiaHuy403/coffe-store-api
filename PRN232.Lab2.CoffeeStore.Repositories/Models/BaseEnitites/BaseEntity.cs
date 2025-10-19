namespace PRN232.Lab2.CoffeeStore.Repositories.Models.BaseEnitites
{
    public class BaseEntity<TId> where TId: notnull
    {
        public TId Id { get; set; } = default!;
    }
}