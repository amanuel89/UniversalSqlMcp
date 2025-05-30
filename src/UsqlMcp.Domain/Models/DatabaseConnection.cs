namespace UsqlMcp.Domain.Models;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class DatabaseConnection
{
    [Required]
    [JsonPropertyName("connection_id")]
    public string ConnectionId { get; set; } = string.Empty;
    
    [JsonPropertyName("connection_name")]
    public string? ConnectionName { get; set; }
    
    [Required]
    [JsonPropertyName("database_type")]
    public DatabaseType DatabaseType { get; set; }
    
    [Required]
    [JsonPropertyName("connection_string")]
    public string ConnectionString { get; set; } = string.Empty;
    
    [JsonPropertyName("schema_name")]
    public string? SchemaName { get; set; }
    
    [JsonPropertyName("max_connections")]
    public int MaxConnections { get; set; } = 10;
    
    [JsonPropertyName("connection_timeout")]
    public int ConnectionTimeout { get; set; } = 30;
    
    [JsonPropertyName("command_timeout")]
    public int CommandTimeout { get; set; } = 60;
    
    [JsonPropertyName("read_only")]
    public bool ReadOnly { get; set; } = true;
    
    [JsonPropertyName("semantic_model_path")]
    public string? SemanticModelPath { get; set; }
    
    [JsonPropertyName("metadata_cache_duration")]
    public int MetadataCacheDuration { get; set; } = 3600;
    
    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; } = new();
    
    [JsonPropertyName("description")]
    public string? Description { get; set; }
    
    [JsonPropertyName("created_by")]
    public string? CreatedBy { get; set; }
    
    [JsonPropertyName("created_at")]
    public DateTime? CreatedAt { get; set; }
    
    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}