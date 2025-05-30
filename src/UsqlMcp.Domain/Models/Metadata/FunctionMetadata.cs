namespace UsqlMcp.Domain.Models.Metadata;

using System.Collections.Generic;

public class FunctionMetadata
{
    public string Name { get; set; } = string.Empty;
    public string? Schema { get; set; }
    public string? Description { get; set; }
    public string? Definition { get; set; }
    public string? ReturnType { get; set; }
    public List<ParameterMetadata> Parameters { get; set; } = new();
}