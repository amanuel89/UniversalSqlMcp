namespace UsqlMcp.Api.Controllers;

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UsqlMcp.Application.Services;
using UsqlMcp.Domain.Models.SemanticModel;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Tags("Semantic Model")]
public class SemanticModelController : ControllerBase
{
    private readonly SemanticModelService _semanticModelService;

    public SemanticModelController(SemanticModelService semanticModelService)
    {
        _semanticModelService = semanticModelService;
    }

    /// <summary>
    /// Gets the semantic model for a specific connection
    /// </summary>
    /// <param name="connectionId">The connection identifier</param>
    /// <returns>The semantic model for the connection</returns>
    /// <response code="200">Returns the semantic model</response>
    /// <response code="404">If the connection is not found</response>
    [HttpGet("{connectionId}")]
    [ProducesResponseType(typeof(SemanticModel), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<SemanticModel>> GetSemanticModel(string connectionId)
    {
        var model = await _semanticModelService.GetSemanticModelAsync(connectionId);
        if (model == null)
        {
            return NotFound();
        }

        return Ok(model);
    }

    /// <summary>
    /// Gets the semantic model for a specific table
    /// </summary>
    /// <param name="connectionId">The connection identifier</param>
    /// <param name="tableName">The table name</param>
    /// <returns>The semantic model for the table</returns>
    /// <response code="200">Returns the table semantic model</response>
    /// <response code="404">If the connection or table is not found</response>
    [HttpGet("{connectionId}/tables/{tableName}")]
    [ProducesResponseType(typeof(SemanticTable), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<SemanticTable>> GetTableSemanticModel(string connectionId, string tableName)
    {
        var tableModel = await _semanticModelService.GetTableSemanticModelAsync(connectionId, tableName);
        if (tableModel == null)
        {
            return NotFound();
        }

        return Ok(tableModel);
    }

    /// <summary>
    /// Gets the business glossary for a connection
    /// </summary>
    /// <param name="connectionId">The connection identifier</param>
    /// <returns>The business glossary</returns>
    /// <response code="200">Returns the business glossary</response>
    [HttpGet("{connectionId}/glossary")]
    [ProducesResponseType(typeof(Dictionary<string, GlossaryTerm>), 200)]
    public async Task<ActionResult<Dictionary<string, GlossaryTerm>>> GetBusinessGlossary(string connectionId)
    {
        var glossary = await _semanticModelService.GetBusinessGlossaryAsync(connectionId);
        return Ok(glossary);
    }

    /// <summary>
    /// Gets the business metrics for a connection
    /// </summary>
    /// <param name="connectionId">The connection identifier</param>
    /// <param name="businessArea">Optional business area filter</param>
    /// <returns>The business metrics</returns>
    /// <response code="200">Returns the business metrics</response>
    [HttpGet("{connectionId}/metrics")]
    [ProducesResponseType(typeof(List<BusinessMetric>), 200)]
    public async Task<ActionResult<List<BusinessMetric>>> GetBusinessMetrics(string connectionId, [FromQuery] string? businessArea = null)
    {
        var metrics = await _semanticModelService.GetBusinessMetricsAsync(connectionId, businessArea);
        return Ok(metrics);
    }
}