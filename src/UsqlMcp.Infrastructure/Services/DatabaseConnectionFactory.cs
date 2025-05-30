namespace UsqlMcp.Infrastructure.Services;

using System;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using MySql.Data.MySqlClient;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using UsqlMcp.Domain.Interfaces;
using UsqlMcp.Domain.Models;

public class DatabaseConnectionFactory : IDatabaseConnectionFactory
{
    private readonly IConnectionRepository _connectionRepository;

    public DatabaseConnectionFactory(IConnectionRepository connectionRepository)
    {
        _connectionRepository = connectionRepository;
    }

    public IDbConnection CreateConnection(string connectionId)
    {
        var connectionConfig = _connectionRepository.GetConnectionByIdAsync(connectionId).Result;
        if (connectionConfig == null)
        {
            throw new ArgumentException($"Connection with ID {connectionId} not found");
        }

        return CreateConnection(connectionConfig);
    }

    // Set connection timeout if specified - REMOVE THIS BLOCK
    // if (connectionConfig.ConnectionTimeout > 0)
    // {
    //     connection.ConnectionTimeout = connectionConfig.ConnectionTimeout;
    // }
    
    // Instead, set the timeout when creating the connection for each database type
    public IDbConnection CreateConnection(DatabaseConnection connectionConfig)
    {
        IDbConnection connection;
        
        switch (connectionConfig.DatabaseType)
        {
            case DatabaseType.PostgreSQL:
                // NpgsqlConnection doesn't allow setting ConnectionTimeout directly
                var npgsqlConnectionStringBuilder = new NpgsqlConnectionStringBuilder(connectionConfig.ConnectionString);
                if (connectionConfig.ConnectionTimeout > 0)
                    npgsqlConnectionStringBuilder.Timeout = connectionConfig.ConnectionTimeout;
                connection = new NpgsqlConnection(npgsqlConnectionStringBuilder.ConnectionString);
                break;
                
            case DatabaseType.SQLServer:
                // SqlConnection doesn't allow setting ConnectionTimeout directly
                var sqlConnectionStringBuilder = new SqlConnectionStringBuilder(connectionConfig.ConnectionString);
                if (connectionConfig.ConnectionTimeout > 0)
                    sqlConnectionStringBuilder.ConnectTimeout = connectionConfig.ConnectionTimeout;
                connection = new SqlConnection(sqlConnectionStringBuilder.ConnectionString);
                break;
                
            case DatabaseType.MySQL:
                // MySqlConnection doesn't allow setting ConnectionTimeout directly
                var mySqlConnectionStringBuilder = new MySqlConnectionStringBuilder(connectionConfig.ConnectionString);
                if (connectionConfig.ConnectionTimeout > 0)
                    mySqlConnectionStringBuilder.ConnectionTimeout = (uint)connectionConfig.ConnectionTimeout;
                connection = new MySqlConnection(mySqlConnectionStringBuilder.ConnectionString);
                break;
                
            case DatabaseType.Oracle:
                // OracleConnection doesn't allow setting ConnectionTimeout directly
                var oracleConnectionStringBuilder = new OracleConnectionStringBuilder(connectionConfig.ConnectionString);
                if (connectionConfig.ConnectionTimeout > 0)
                    oracleConnectionStringBuilder.ConnectionTimeout = connectionConfig.ConnectionTimeout;
                connection = new OracleConnection(oracleConnectionStringBuilder.ConnectionString);
                break;
                
            case DatabaseType.SQLite:
                var sqliteConnection = new SqliteConnection(connectionConfig.ConnectionString);
                // SQLite doesn't have a ConnectionTimeout property
                connection = sqliteConnection;
                break;
                
            default:
                throw new NotSupportedException($"Database type {connectionConfig.DatabaseType} is not supported");
        }
    
        return connection;
    }
}