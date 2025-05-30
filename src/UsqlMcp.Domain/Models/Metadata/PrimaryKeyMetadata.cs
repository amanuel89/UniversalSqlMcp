
namespace UsqlMcp.Domain.Models.Metadata;

public class PrimaryKeyMetadata
{
    public string Name { get; set; } = string.Empty;
    public string ColumnName { get; set; } = string.Empty;
    public int OrdinalPosition { get; set; }
}