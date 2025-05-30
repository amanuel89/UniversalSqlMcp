namespace UsqlMcp.Domain.Models.Metadata;

using System.Collections.Generic;

public class ViewMetadata
{
    public string Name { get; set; } = string.Empty;
    public string? Schema { get; set; }
    public string? Description { get; set; }
    public string? Definition { get; set; }
    public List<ColumnMetadata> Columns { get; set; } = new();
}