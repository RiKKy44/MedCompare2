using DrugCompare.Models;
using DrugCompare.Services.Contracts;

namespace DrugCompare.Services;

public sealed class DisabledDataManagementService : IDataManagementService
{
    public Task<DataManagementStatusResult> GetDataManagementStatusAsync()
    {
        return Task.FromResult(new DataManagementStatusResult());
    }
}