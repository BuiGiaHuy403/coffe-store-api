using Microsoft.EntityFrameworkCore.Storage;

namespace PRN232.Lab2.CoffeeStore.Repositories.Interfaces;

public interface IUnitOfWork
{
    IGenericRepository<T, TId> GetRepository<T, TId>()
        where T : class
        where TId : notnull;
    
    Task<int> SaveChangesAsync();

    Task<IDbContextTransaction> BeginTransactionAsync();

}