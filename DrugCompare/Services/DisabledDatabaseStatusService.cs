using DrugCompare.Models;
using DrugCompare.Services.Contracts;

namespace DrugCompare.Services;

public sealed class DisabledDatabaseStatusService : IDatabaseStatusService
{
    public Task<DatabaseStatusResult> GetDatabaseStatusAsync()
    {
        return Task.FromResult(new DatabaseStatusResult());
    }
}