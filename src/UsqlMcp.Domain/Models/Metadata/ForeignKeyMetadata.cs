namespace UsqlMcp.Domain.Models.Metadata;

using System.Collections.Generic;

public class ForeignKeyMetadata
{
    public string Name { get; set; } = string.Empty;
    public string ReferencedTableName { get; set; } = string.Empty;
    public string? ReferencedTableSchema { get; set; }
    public List<ForeignKeyColumnPair> ColumnPairs { get; set; } = new();
}

public class ForeignKeyColumnPair
{
    public string ColumnName { get; set; } = string.Empty;
    public string ReferencedColumnName { get; set; } = string.Empty;
}