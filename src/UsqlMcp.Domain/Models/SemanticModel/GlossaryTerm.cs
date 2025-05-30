namespace UsqlMcp.Domain.Models.SemanticModel;

public class GlossaryTerm
{
    public string Term { get; set; } = string.Empty;
    public string Definition { get; set; } = string.Empty;
    public string? BusinessArea { get; set; }
}