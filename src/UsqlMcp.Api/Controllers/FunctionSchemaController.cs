namespace UsqlMcp.Api.Controllers;

using System.IO;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Tags("Function Schema")]
public class FunctionSchemaController : ControllerBase
{
    private readonly IWebHostEnvironment _environment;

    public FunctionSchemaController(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    /// <summary>
    /// Gets the function schema for AI-assisted query generation
    /// </summary>
    /// <returns>The function schema definition</returns>
    /// <response code="200">Returns the function schema</response>
    /// <response code="404">If the function schema file is not found</response>
    /// <response code="500">If there's an error retrieving the schema</response>
    [HttpGet]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public ActionResult<object> GetFunctionSchema()
    {
        try
        {
            // Path to the sample function schema JSON file
            var schemaPath = Path.Combine(_environment.ContentRootPath, "..", "..", "sample_function_schema.json");
            
            if (!System.IO.File.Exists(schemaPath))
            {
                return NotFound("Function schema file not found");
            }

            var jsonString = System.IO.File.ReadAllText(schemaPath);
            var jsonDocument = JsonDocument.Parse(jsonString);
            
            return Ok(jsonDocument.RootElement);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error retrieving function schema: {ex.Message}");
        }
    }
}