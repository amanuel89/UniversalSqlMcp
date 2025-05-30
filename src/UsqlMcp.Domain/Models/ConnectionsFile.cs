namespace UsqlMcp.Domain.Models;

using System.Collections.Generic;

public class ConnectionsFile
{
    public List<DatabaseConnection> Connections { get; set; } = new List<DatabaseConnection>();
}
