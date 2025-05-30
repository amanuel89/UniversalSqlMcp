namespace UsqlMcp.Api.Controllers;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UsqlMcp.Application.Services;
using UsqlMcp.Domain.Models.Query;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Tags("Query")]
public class QueryController : ControllerBase
{
    private readonly QueryService _queryService;

    public QueryController(QueryService queryService)
    {
        _queryService = queryService;
    }

    /// <summary>
    /// Executes a read-only SQL query against a database connection
    /// </summary>
    /// <param name="connectionId">The unique identifier of the database connection</param>
    /// <param name="request">The query request containing the SQL query and max rows</param>
    /// <returns>The query result with columns and data</returns>
    /// <response code="200">Returns the query result</response>
    /// <response code="400">If the query is not read-only or contains syntax errors</response>
    /// <response code="404">If the connection is not found</response>
    [HttpPost("execute/{connectionId}")]
    [ProducesResponseType(typeof(QueryResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<QueryResult>> ExecuteQuery(string connectionId, [FromBody] QueryRequest request)
    {
        try
        {
            var result = await _queryService.ExecuteQueryAsync(connectionId, request.Query, request.MaxRows);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
        catch (System.InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Gets a sample of data from a specific table
    /// </summary>
    /// <param name="connectionId">The unique identifier of the database connection</param>
    /// <param name="tableName">The name of the table</param>
    /// <param name="schema">Optional schema name</param>
    /// <param name="maxRows">Maximum number of rows to return (default: 10)</param>
    /// <returns>The query result with sample data</returns>
    /// <response code="200">Returns the sample data</response>
    /// <response code="404">If the connection or table is not found</response>
    [HttpGet("sample/{connectionId}/{tableName}")]
    [ProducesResponseType(typeof(QueryResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<QueryResult>> GetTableSample(string connectionId, string tableName, [FromQuery] string? schema = null, [FromQuery] int maxRows = 10)
    {
        var result = await _queryService.GetTableSampleAsync(connectionId, tableName, schema, maxRows);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }
}

/// <summary>
/// Represents a query request with SQL and row limit
/// </summary>
public class QueryRequest
{
    /// <summary>
    /// The SQL query to execute (must be read-only)
    /// </summary>
    public string Query { get; set; } = string.Empty;
    
    /// <summary>
    /// Maximum number of rows to return
    /// </summary>
    public int MaxRows { get; set; } = 100;
}