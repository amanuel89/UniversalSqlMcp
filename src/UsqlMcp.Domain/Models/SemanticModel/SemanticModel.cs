namespace UsqlMcp.Domain.Models.SemanticModel;

using System.Collections.Generic;

public class SemanticModel
{
    public string ConnectionId { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Dictionary<string, SemanticTable> Tables { get; set; } = new();
    public List<BusinessMetric> BusinessMetrics { get; set; } = new();
    public Dictionary<string, GlossaryTerm> BusinessGlossary { get; set; } = new();
}