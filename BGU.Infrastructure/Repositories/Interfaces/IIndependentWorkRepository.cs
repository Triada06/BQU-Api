using System.Linq.Expressions;
using BGU.Core.Entities;
using Microsoft.EntityFrameworkCore.Query;

namespace BGU.Infrastructure.Repositories.Interfaces;

public interface IIndependentWorkRepository : IBaseRepository<IndependentWork>
{

    public Task<int> BulkUpdateAsync(
        Expression<Func<IndependentWork, bool>> predicate,
        Expression<Func<SetPropertyCalls<IndependentWork>, SetPropertyCalls<IndependentWork>>> setters);
}