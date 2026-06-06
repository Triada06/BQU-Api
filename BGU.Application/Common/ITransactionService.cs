namespace BGU.Application.Common;

public interface ITransactionService
{
    Task<T> ExecuteAsync<T>(Func<Task<T>> operation, Func<T, bool> shouldCommit);
}
