namespace UsqlMcp.Domain.Models.Metadata;

using System;
using System.Collections.Generic;

public class DatabaseMetadata
{
    public string DatabaseName { get; set; } = string.Empty;
    public string? DatabaseVersion { get; set; }
    public List<TableMetadata> Tables { get; set; } = new();
    public List<ViewMetadata> Views { get; set; } = new();
    public List<StoredProcedureMetadata> StoredProcedures { get; set; } = new();
    public List<FunctionMetadata> Functions { get; set; } = new();
    public DateTime ExtractedAt { get; set; } = DateTime.UtcNow;
}