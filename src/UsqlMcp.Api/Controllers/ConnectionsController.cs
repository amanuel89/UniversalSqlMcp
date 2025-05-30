namespace UsqlMcp.Api.Controllers;

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UsqlMcp.Application.Services;
using UsqlMcp.Domain.Models;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Tags("Connections")]
public class ConnectionsController : ControllerBase
{
    private readonly ConnectionService _connectionService;

    public ConnectionsController(ConnectionService connectionService)
    {
        _connectionService = connectionService;
    }

    /// <summary>
    /// Gets all database connections
    /// </summary>
    /// <returns>A list of all database connections</returns>
    /// <response code="200">Returns the list of connections</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<DatabaseConnection>), 200)]
    public async Task<ActionResult<List<DatabaseConnection>>> GetAllConnections()
    {
        var connections = await _connectionService.GetAllConnectionsAsync();

       if(connections !=null  || connections.Count > 0)
        {
            connections.ForEach(c =>
            {
                c.ConnectionString = "sensitive information";
                c.SemanticModelPath = "sensitive information";
            });
        }
        return Ok(connections);
    }

    /// <summary>
    /// Gets a specific database connection by ID
    /// </summary>
    /// <param name="id">The connection identifier</param>
    /// <returns>The database connection</returns>
    /// <response code="200">Returns the connection</response>
    /// <response code="404">If the connection is not found</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(DatabaseConnection), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<DatabaseConnection>> GetConnection(string id)
    {
        var connection = await _connectionService.GetConnectionByIdAsync(id);
        if (connection == null)
        {
            return NotFound();
        }
         
            connection.ConnectionString = "sensitive information";
            connection.SemanticModelPath = "sensitive information";
       
        return Ok(connection);
    }

    /// <summary>
    /// Adds a new database connection
    /// </summary>
    /// <param name="connection">The connection to add</param>
    /// <returns>The result of the operation</returns>
    /// <response code="201">Returns the newly created connection</response>
    /// <response code="400">If the connection could not be added</response>
    [HttpPost]
    [ProducesResponseType(typeof(DatabaseConnection), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<bool>> AddConnection(DatabaseConnection connection)
    {
        var result = await _connectionService.AddConnectionAsync(connection);
        if (result)
        {
            return CreatedAtAction(nameof(GetConnection), new { id = connection.ConnectionId }, connection);
        }

        return BadRequest("Failed to add connection");
    }

    /// <summary>
    /// Updates an existing database connection
    /// </summary>
    /// <param name="id">The connection identifier</param>
    /// <param name="connection">The updated connection details</param>
    /// <returns>The result of the operation</returns>
    /// <response code="200">If the connection was updated successfully</response>
    /// <response code="400">If the connection ID doesn't match the provided ID</response>
    /// <response code="404">If the connection is not found</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(bool), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<bool>> UpdateConnection(string id, DatabaseConnection connection)
    {
        if (id != connection.ConnectionId)
        {
            return BadRequest("Connection ID mismatch");
        }

        var result = await _connectionService.UpdateConnectionAsync(connection);
        if (result)
        {
            return Ok(true);
        }

        return NotFound();
    }

    /// <summary>
    /// Deletes a database connection
    /// </summary>
    /// <param name="id">The connection identifier</param>
    /// <returns>The result of the operation</returns>
    /// <response code="200">If the connection was deleted successfully</response>
    /// <response code="404">If the connection is not found</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(bool), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<bool>> DeleteConnection(string id)
    {
        var result = await _connectionService.DeleteConnectionAsync(id);
        if (result)
        {
            return Ok(true);
        }

        return NotFound();
    }
}