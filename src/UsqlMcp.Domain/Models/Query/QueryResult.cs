namespace UsqlMcp.Domain.Models.Query;

using System.Collections.Generic;

public class QueryResult
{
    public List<string> Columns { get; set; } = new();
    public List<List<object?>> Rows { get; set; } = new();
    public int TotalRows { get; set; }
    public bool Truncated { get; set; }
    public long ExecutionTimeMs { get; set; }
}