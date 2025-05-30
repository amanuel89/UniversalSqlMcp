namespace UsqlMcp.Domain.Models;

using System.Runtime.Serialization;
using System.Text.Json.Serialization;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DatabaseType
{
    [EnumMember(Value = "postgresql")]
    PostgreSQL = 0,
    
    [EnumMember(Value = "sqlserver")]
    SQLServer = 1,
    
    [EnumMember(Value = "mysql")]
    MySQL = 2,
    
    [EnumMember(Value = "oracle")]
    Oracle = 3,
    
    [EnumMember(Value = "sqlite")]
    SQLite = 4
}