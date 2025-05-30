namespace UsqlMcp.Domain.Interfaces;

using System.Collections.Generic;
using System.Threading.Tasks;
using UsqlMcp.Domain.Models.SemanticModel;

public interface ISemanticModelRepository
{
    Task<SemanticModel?> GetSemanticModelAsync(string connectionId);
    Task<SemanticTable?> GetTableSemanticModelAsync(string connectionId, string tableName);
    Task<Dictionary<string, GlossaryTerm>> GetBusinessGlossaryAsync(string connectionId);
    Task<List<BusinessMetric>> GetBusinessMetricsAsync(string connectionId, string? businessArea = null);
}