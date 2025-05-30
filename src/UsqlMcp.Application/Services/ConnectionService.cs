namespace UsqlMcp.Application.Services;

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UsqlMcp.Domain.Interfaces;
using UsqlMcp.Domain.Models;

public class ConnectionService
{
    private readonly IConnectionRepository _connectionRepository;
    private readonly ILogger<ConnectionService> _logger;

    public ConnectionService(IConnectionRepository connectionRepository, ILogger<ConnectionService> logger)
    {
        _connectionRepository = connectionRepository;
        _logger = logger;
    }

    public async Task<List<DatabaseConnection>> GetAllConnectionsAsync()
    {
        _logger.LogInformation("Getting all database connections");
        return await _connectionRepository.GetAllConnectionsAsync();
    }

    public async Task<DatabaseConnection?> GetConnectionByIdAsync(string connectionId)
    {
        _logger.LogInformation("Getting database connection with ID: {ConnectionId}", connectionId);
        return await _connectionRepository.GetConnectionByIdAsync(connectionId);
    }

    public async Task<bool> AddConnectionAsync(DatabaseConnection connection)
    {
        _logger.LogInformation("Adding new database connection with ID: {ConnectionId}", connection.ConnectionId);
        return await _connectionRepository.AddConnectionAsync(connection);
    }

    public async Task<bool> UpdateConnectionAsync(DatabaseConnection connection)
    {
        _logger.LogInformation("Updating database connection with ID: {ConnectionId}", connection.ConnectionId);
        return await _connectionRepository.UpdateConnectionAsync(connection);
    }

    public async Task<bool> DeleteConnectionAsync(string connectionId)
    {
        _logger.LogInformation("Deleting database connection with ID: {ConnectionId}", connectionId);
        return await _connectionRepository.DeleteConnectionAsync(connectionId);
    }
}