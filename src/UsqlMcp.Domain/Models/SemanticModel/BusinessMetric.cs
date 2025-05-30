namespace UsqlMcp.Domain.Models.SemanticModel;

public class BusinessMetric
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? BusinessArea { get; set; }
    public string? Formula { get; set; }
    public string? SqlDefinition { get; set; }
}