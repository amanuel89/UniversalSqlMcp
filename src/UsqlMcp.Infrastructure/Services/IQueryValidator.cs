namespace UsqlMcp.Infrastructure.Services;

using System.Threading.Tasks;

public interface IQueryValidator
{
    Task<bool> ValidateQueryAsync(string connectionId, string query);
}