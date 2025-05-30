namespace UsqlMcp.Infrastructure.Services;

using System.Data;
using UsqlMcp.Domain.Models;

public interface IDatabaseConnectionFactory
{
    IDbConnection CreateConnection(DatabaseConnection connectionConfig);
    IDbConnection CreateConnection(string connectionId);
}