using BGU.Infrastructure.Data;

namespace BGU.Application.Common;

public class TransactionService(AppDbContext context) : ITransactionService
{
    public async Task<T> ExecuteAsync<T>(Func<Task<T>> operation, Func<T, bool> shouldCommit)
    {
        if (context.Database.CurrentTransaction is not null)
        {
            return await operation();
        }

        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            var result = await operation();

            if (shouldCommit(result))
            {
                await transaction.CommitAsync();
            }
            else
            {
                await transaction.RollbackAsync();
            }

            return result;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
