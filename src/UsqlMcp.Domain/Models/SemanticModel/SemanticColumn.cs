namespace UsqlMcp.Domain.Models.SemanticModel;

using System.Collections.Generic;

public class SemanticColumn
{
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? BusinessDefinition { get; set; }
    public string? DataType { get; set; }
    public string? Format { get; set; }
    public string? Example { get; set; }
    public bool IsPrimaryKey { get; set; }
    public ForeignKeyReference? ForeignKey { get; set; }
    public List<string>? AllowedValues { get; set; }
    public bool ContainsPII { get; set; }
}

public class ForeignKeyReference
{
    public string Table { get; set; } = string.Empty;
    public string Column { get; set; } = string.Empty;
}