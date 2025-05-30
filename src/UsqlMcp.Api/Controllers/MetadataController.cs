namespace UsqlMcp.Api.Controllers;

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UsqlMcp.Application.Services;
using UsqlMcp.Domain.Models.Metadata;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Tags("Metadata")]
public class MetadataController : ControllerBase
{
    private readonly MetadataService _metadataService;

    public MetadataController(MetadataService metadataService)
    {
        _metadataService = metadataService;
    }

    /// <summary>
    /// Gets database metadata for a specific connection
    /// </summary>
    /// <param name="connectionId">The unique identifier of the database connection</param>
    /// <returns>The database metadata</returns>
    /// <response code="200">Returns the database metadata</response>
    /// <response code="404">If the connection is not found</response>
    [HttpGet("database/{connectionId}")]
    [ProducesResponseType(typeof(DatabaseMetadata), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DatabaseMetadata>> GetDatabaseMetadata(string connectionId)
    {
        var metadata = await _metadataService.GetDatabaseMetadataAsync(connectionId);
        if (metadata == null)
        {
            return NotFound();
        }

        return Ok(metadata);
    }

    /// <summary>
    /// Gets all tables for a specific database connection
    /// </summary>
    /// <param name="connectionId">The unique identifier of the database connection</param>
    /// <returns>A list of table metadata</returns>
    /// <response code="200">Returns the list of tables</response>
    [HttpGet("tables/{connectionId}")]
    [ProducesResponseType(typeof(List<TableMetadata>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<TableMetadata>>> GetTables(string connectionId)
    {
        var tables = await _metadataService.GetTablesAsync(connectionId);
        return Ok(tables);
    }

    /// <summary>
    /// Gets metadata for a specific table
    /// </summary>
    /// <param name="connectionId">The unique identifier of the database connection</param>
    /// <param name="tableName">The name of the table</param>
    /// <param name="schema">Optional schema name</param>
    /// <returns>The table metadata</returns>
    /// <response code="200">Returns the table metadata</response>
    /// <response code="404">If the connection or table is not found</response>
    [HttpGet("tables/{connectionId}/{tableName}")]
    [ProducesResponseType(typeof(TableMetadata), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TableMetadata>> GetTableMetadata(string connectionId, string tableName, [FromQuery] string? schema = null)
    {
        var tableMetadata = await _metadataService.GetTableMetadataAsync(connectionId, tableName, schema);
        if (tableMetadata == null)
        {
            return NotFound();
        }

        return Ok(tableMetadata);
    }

    /// <summary>
    /// Gets all views for a specific database connection
    /// </summary>
    /// <param name="connectionId">The unique identifier of the database connection</param>
    /// <returns>A list of view metadata</returns>
    /// <response code="200">Returns the list of views</response>
    [HttpGet("views/{connectionId}")]
    [ProducesResponseType(typeof(List<ViewMetadata>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ViewMetadata>>> GetViews(string connectionId)
    {
        var views = await _metadataService.GetViewsAsync(connectionId);
        return Ok(views);
    }

    /// <summary>
    /// Gets metadata for a specific view
    /// </summary>
    /// <param name="connectionId">The unique identifier of the database connection</param>
    /// <param name="viewName">The name of the view</param>
    /// <param name="schema">Optional schema name</param>
    /// <returns>The view metadata</returns>
    /// <response code="200">Returns the view metadata</response>
    /// <response code="404">If the connection or view is not found</response>
    [HttpGet("views/{connectionId}/{viewName}")]
    [ProducesResponseType(typeof(ViewMetadata), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ViewMetadata>> GetViewMetadata(string connectionId, string viewName, [FromQuery] string? schema = null)
    {
        var viewMetadata = await _metadataService.GetViewMetadataAsync(connectionId, viewName, schema);
        if (viewMetadata == null)
        {
            return NotFound();
        }

        return Ok(viewMetadata);
    }

    /// <summary>
    /// Gets all stored procedures for a specific database connection
    /// </summary>
    /// <param name="connectionId">The unique identifier of the database connection</param>
    /// <returns>A list of stored procedure metadata</returns>
    /// <response code="200">Returns the list of stored procedures</response>
    [HttpGet("procedures/{connectionId}")]
    [ProducesResponseType(typeof(List<StoredProcedureMetadata>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<StoredProcedureMetadata>>> GetStoredProcedures(string connectionId)
    {
        var procedures = await _metadataService.GetStoredProceduresAsync(connectionId);
        return Ok(procedures);
    }

    /// <summary>
    /// Gets all functions for a specific database connection
    /// </summary>
    /// <param name="connectionId">The unique identifier of the database connection</param>
    /// <returns>A list of function metadata</returns>
    /// <response code="200">Returns the list of functions</response>
    [HttpGet("functions/{connectionId}")]
    [ProducesResponseType(typeof(List<FunctionMetadata>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<FunctionMetadata>>> GetFunctions(string connectionId)
    {
        var functions = await _metadataService.GetFunctionsAsync(connectionId);
        return Ok(functions);
    }
}