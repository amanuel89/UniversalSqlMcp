namespace UsqlMcp.Domain.Models.SemanticModel;

using System.Collections.Generic;

public class SemanticTable
{
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? BusinessArea { get; set; }
    public string? UpdateFrequency { get; set; }
    public Dictionary<string, SemanticColumn> Columns { get; set; } = new();
}