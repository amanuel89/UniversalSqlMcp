namespace UsqlMcp.Domain.Models.Metadata;

public class ParameterMetadata
{
    public string Name { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public bool IsOutput { get; set; }
    public int OrdinalPosition { get; set; }
    public string? DefaultValue { get; set; }
}