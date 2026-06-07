using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using VoltHome.Infrastructure.Configurations.Interfaces;

namespace VoltHome.Infrastructure.Repositories;

public class BaseRepository<TEntity> : IBaseRepository<TEntity>
    where TEntity : class
{
    protected readonly AppDbContext Context;
    protected readonly DbSet<TEntity> DbSet;

    public BaseRepository(AppDbContext context)
    {
        Context = context;
        DbSet = context.Set<TEntity>();
    }

    public async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await DbSet.FindAsync(new object[] { id }, ct);
    }

    public async Task<List<TEntity>> GetAllAsync(CancellationToken ct)
    {
        return await DbSet.ToListAsync(ct);
    }

    public async Task AddAsync(TEntity entity, CancellationToken ct)
    {
        await DbSet.AddAsync(entity, ct);
    }

    public void Update(TEntity entity)
    {
        DbSet.Update(entity);
    }

    public void Remove(TEntity entity)
    {
        DbSet.Remove(entity);
    }

    public IQueryable<TEntity> Query()
    {
        return DbSet.AsQueryable();
    }
}
