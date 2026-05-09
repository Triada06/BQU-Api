using System.Linq.Expressions;
using BGU.Core;
using BGU.Infrastructure.Constants;
using BGU.Infrastructure.Data;
using BGU.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace BGU.Infrastructure.Repositories;

public class BaseRepository<T> : IBaseRepository<T> where T : class, IBaseEntity
{
    private readonly AppDbContext _context;
    private readonly DbSet<T> _entities;

    public BaseRepository(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _entities = _context.Set<T>();
    }

    public async Task<bool> CreateAsync(T entity)
    {
        _entities.Add(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(T entity)
    {
        _entities.Update(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(T entity)
    {
        _entities.Remove(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> BulkCreateAsync(List<T> entities)
    {
        _entities.AddRange(entities);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<T?> GetByIdAsync(string id, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        bool tracking = true)
    {
        IQueryable<T> query = _context.Set<T>();
        query = tracking ? query : query.AsNoTracking();
        if (include != null)
            query = include(query);
        var data = await query.FirstOrDefaultAsync(m => m.Id == id);
        return data;
    }

    public virtual async Task<PagedResponse<T>> GetAllPaginatedAsync(
        Expression<Func<T, bool>>? predicate,
        int page = 1,
        int pageSize = 5,
        bool tracking = true,
        Func<IQueryable<T>, IQueryable<T>>? include = null)
    {
        IQueryable<T> query = _context.Set<T>();

        query = tracking ? query : query.AsNoTracking();

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        if (include != null)
            query = include(query);

        var totalCount = await query.CountAsync(); // IMPORTANT: before paging

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResponse<T>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? predicate, bool tracking = true,
        Func<IQueryable<T>, IQueryable<T>>? include = null)
    {
        IQueryable<T> query = _context.Set<T>();

        query = tracking ? query : query.AsNoTracking();

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        if (include != null)
            query = include(query);

        var items = await query.ToListAsync();

        return items;
    }

    public async Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null, bool tracking = false)
    {
        IQueryable<T> query = _context.Set<T>();
        query = tracking ? query.AsTracking() : query;
        if (include != null)
            query = include(query);
        query = query.Where(predicate);
        var data = await query.ToListAsync();
        return data;
    }

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        => await _context.Set<T>().AnyAsync(predicate);

    public IQueryable<T> Table => _entities.AsNoTracking();
}