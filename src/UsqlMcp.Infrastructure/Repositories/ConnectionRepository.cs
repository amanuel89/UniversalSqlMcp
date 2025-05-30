namespace UsqlMcp.Infrastructure.Repositories;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using UsqlMcp.Domain.Interfaces;
using UsqlMcp.Domain.Models;

public class ConnectionRepository : IConnectionRepository
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<ConnectionRepository> _logger;
    private readonly string _connectionsFilePath;
    private Dictionary<string, DatabaseConnection> _connections;

    public ConnectionRepository(IConfiguration configuration, ILogger<ConnectionRepository> logger)
    {
        _configuration = configuration;
        _logger = logger;
        
        // Get connections file path from configuration or use default
        _connectionsFilePath = _configuration["ConnectionsFilePath"] ?? 
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "connections.json");
        
        LoadConnections();
    }

    public async Task<List<DatabaseConnection>> GetAllConnectionsAsync()
    {
        return await Task.FromResult(new List<DatabaseConnection>(_connections.Values));
    }

    public async Task<DatabaseConnection?> GetConnectionByIdAsync(string connectionId)
    {
        if (_connections.TryGetValue(connectionId, out var connection))
        {
            return await Task.FromResult(connection);
        }

        return await Task.FromResult<DatabaseConnection?>(null);
    }

    public async Task<bool> AddConnectionAsync(DatabaseConnection connection)
    {
        if (_connections.ContainsKey(connection.ConnectionId))
        {
            return await Task.FromResult(false);
        }

        _connections[connection.ConnectionId] = connection;
        await SaveConnectionsAsync();
        return true;
    }

    public async Task<bool> UpdateConnectionAsync(DatabaseConnection connection)
    {
        if (!_connections.ContainsKey(connection.ConnectionId))
        {
            return await Task.FromResult(false);
        }

        _connections[connection.ConnectionId] = connection;
        await SaveConnectionsAsync();
        return true;
    }

    public async Task<bool> DeleteConnectionAsync(string connectionId)
    {
        if (!_connections.ContainsKey(connectionId))
        {
            return await Task.FromResult(false);
        }

        _connections.Remove(connectionId);
        await SaveConnectionsAsync();
        return true;
    }

    private void LoadConnections()
    {
        _connections = new Dictionary<string, DatabaseConnection>();

        if (!File.Exists(_connectionsFilePath))
        {
            _logger.LogWarning("Connections file not found: {FilePath}", _connectionsFilePath);
            return;
        }

        try
        {
            _logger.LogInformation("Loading connections from {FilePath}", _connectionsFilePath);
            var json = File.ReadAllText(_connectionsFilePath);
            _logger.LogInformation("Connection file content: {Json}", json);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true,
                Converters = { new JsonStringEnumConverter(), new NullableDateTimeConverter() }
            };


            var connectionsFile = JsonSerializer.Deserialize<ConnectionsFile>(json, options);
            _logger.LogInformation("Deserialized connections file: {ConnectionsFile}", 
                JsonSerializer.Serialize(connectionsFile, options));

            if (connectionsFile?.Connections != null)
            {
                _logger.LogInformation("Found {Count} connections in file", connectionsFile.Connections.Count);
                foreach (var connection in connectionsFile.Connections)
                {
                    _logger.LogInformation("Processing connection: {ConnectionId}, {ConnectionName}", 
                        connection.ConnectionId, connection.ConnectionName);
                    
                    if (!string.IsNullOrEmpty(connection.ConnectionId))
                    {
                        _connections[connection.ConnectionId] = connection;
                        _logger.LogInformation("Added connection: {ConnectionId}", connection.ConnectionId);
                    }
                    else
                    {
                        _logger.LogWarning("Skipping connection with empty ConnectionId");
                    }
                }
            }

            _logger.LogInformation("Loaded {Count} connections from {FilePath}", _connections.Count, _connectionsFilePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading connections from {FilePath}", _connectionsFilePath);
        }
    }

    public class NullableDateTimeConverter : JsonConverter<DateTime?>
    {
        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var str = reader.GetString();
                if (string.IsNullOrWhiteSpace(str)) return null;

                if (DateTime.TryParse(str, out var date))
                    return date;
            }
            else if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            throw new JsonException("Invalid DateTime format.");
        }

        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
                writer.WriteStringValue(value.Value.ToString("o"));
            else
                writer.WriteNullValue();
        }
    }

    private async Task SaveConnectionsAsync()
    {
        try
        {
            var connectionsFile = new ConnectionsFile
            {
                Connections = new List<DatabaseConnection>(_connections.Values)
            };
            
            var json = JsonSerializer.Serialize(connectionsFile, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
            });

            await File.WriteAllTextAsync(_connectionsFilePath, json);
            _logger.LogInformation("Saved {Count} connections to {FilePath}", connectionsFile.Connections.Count, _connectionsFilePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving connections to {FilePath}", _connectionsFilePath);
        }
    }
}