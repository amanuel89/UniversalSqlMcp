namespace UsqlMcp.Domain.Interfaces;

using System.Collections.Generic;
using System.Threading.Tasks;
using UsqlMcp.Domain.Models;

public interface IConnectionRepository
{
    Task<List<DatabaseConnection>> GetAllConnectionsAsync();
    Task<DatabaseConnection?> GetConnectionByIdAsync(string connectionId);
    Task<bool> AddConnectionAsync(DatabaseConnection connection);
    Task<bool> UpdateConnectionAsync(DatabaseConnection connection);
    Task<bool> DeleteConnectionAsync(string connectionId);
}