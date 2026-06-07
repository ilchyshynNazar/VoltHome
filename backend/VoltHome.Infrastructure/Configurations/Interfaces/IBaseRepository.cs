using System;
using System.Collections.Generic;
using System.Text;

namespace VoltHome.Infrastructure.Configurations.Interfaces;

public interface IBaseRepository<TEntity>
    where TEntity : class
{
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken ct);

    Task<List<TEntity>> GetAllAsync(CancellationToken ct);

    Task AddAsync(TEntity entity, CancellationToken ct);

    void Update(TEntity entity);

    void Remove(TEntity entity);

    IQueryable<TEntity> Query();
}