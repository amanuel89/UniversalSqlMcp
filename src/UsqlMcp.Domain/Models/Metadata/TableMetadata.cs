namespace UsqlMcp.Domain.Models.Metadata;

using System.Collections.Generic;

public class TableMetadata
{
    public string Name { get; set; } = string.Empty;
    public string? Schema { get; set; }
    public string? Description { get; set; }
    public long? RowCount { get; set; }
    public long? SizeInBytes { get; set; }
    public List<ColumnMetadata> Columns { get; set; } = new();
    public List<IndexMetadata> Indexes { get; set; } = new();
    public List<ForeignKeyMetadata> ForeignKeys { get; set; } = new();
    public List<string> PrimaryKeyColumns { get; set; } = new();
}