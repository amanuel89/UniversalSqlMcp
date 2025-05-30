namespace UsqlMcp.Domain.Models.Metadata;

using System.Collections.Generic;

public class IndexMetadata
{
    public string Name { get; set; } = string.Empty;
    public bool IsUnique { get; set; }
    public List<string> ColumnNames { get; set; } = new();
}