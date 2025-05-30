 
# 🧠 Universal SQL MCP (Metadata Collection Platform)

**Universal SQL MCP** (aka **UsqlMcp**) is extensible platform for extracting, managing, and semantically enriching database metadata across multiple SQL engines. Built with clean architecture and a layered .NET 9 foundation, this tool gives you visibility and control over your databases like never before.

---

## 🚀 Features

- ✅ **Multi-Database Support**: PostgreSQL, SQL Server, MySQL, Oracle, SQLite  
- 🔍 **Deep Metadata Extraction**: Tables, columns, views, functions, procedures, indexes, triggers, and more  
- 🧩 **Semantic Models**: Add business context to raw metadata for smarter querying  
- 🧠 **Function-Call API Schema Generation**  
- ⚙️ **Layered Architecture**: API, Application, Domain, and Infrastructure layers  
- 📡 **RESTful API**: Interact with metadata and run queries programmatically  
- 🔒 **Read-Only by Design**: Safe metadata access without data tampering

---

## 🧱 Project Structure

```bash
src/
├── UsqlMcp.Api           # API endpoints (REST)
├── UsqlMcp.Application   # Application services & business logic
├── UsqlMcp.Domain        # Core entities and interfaces
└── UsqlMcp.Infrastructure # Data access, DB connectors, SQL scripts
```

---

## 🛠️ Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- Access to any supported SQL database engine

---

## ⚙️ Setup

1. **Clone the repo**
   ```bash
   git clone https://github.com/your-org/usqlmcp.git
   cd usqlmcp
   ```

2. **Configure Database Connections**
   - Edit `connections.json` in `UsqlMcp.Api`
   - Follow the schema in `connection_config_schema.json`
   - Use `sample_connections.json` as a reference

3. **Build the solution**
   ```bash
   dotnet build UsqlMcp.sln
   ```

4. **Run the API**
   ```bash
   cd src/UsqlMcp.Api
   dotnet run
   ```

---

## 🌐 API Endpoints

### 🔗 Connection Management
| Method | Endpoint                        | Description                         |
|--------|----------------------------------|-------------------------------------|
| GET    | `/api/connections`              | List all configured connections     |
| GET    | `/api/connections/{id}`         | Get specific connection details     |
| POST   | `/api/connections`              | Add new DB connection               |
| DELETE | `/api/connections/{id}`         | Remove a DB connection              |

### 📊 Metadata Access
| Method | Endpoint                                           | Description                        |
|--------|----------------------------------------------------|------------------------------------|
| GET    | `/api/metadata/{connectionId}/database`            | Full metadata dump                 |
| GET    | `/api/metadata/{connectionId}/tables`              | List all tables                    |
| GET    | `/api/metadata/{connectionId}/views`               | List all views                     |
| GET    | `/api/metadata/{connectionId}/procedures`          | List stored procedures             |
| GET    | `/api/metadata/{connectionId}/functions`           | List database functions            |

### 🧪 Querying
| Method | Endpoint                                                | Description                        |
|--------|----------------------------------------------------------|------------------------------------|
| POST   | `/api/query/{connectionId}`                             | Execute a SQL query                |
| GET    | `/api/query/{connectionId}/table/{tableName}/sample`    | Get table sample data              |

### 🧠 Semantic Models
| Method | Endpoint                                  | Description                        |
|--------|--------------------------------------------|------------------------------------|
| GET    | `/api/semantic/{connectionId}`             | Get semantic model                 |
| POST   | `/api/semantic/{connectionId}`             | Create or update semantic model    |

---

## 🧠 Semantic Models

Semantic models allow you to attach **business context** to your database structures (think: "CustomerId" → "Unique identifier for customer").  
See `sample_semantic_model.yaml` for how to define and upload semantic metadata.

---

## 🧠 Metadata Collected

- Tables, Columns, Views
- Indexes, Triggers
- Primary & Foreign Keys
- Stored Procedures, Functions
- Table Size & Row Stats

---

## 💡 Usage Flow

```plaintext
API Layer → Application Layer → Infrastructure Layer → DB Scripts
            ↓                             ↑
     Domain Models ← Metadata Mapping ← Raw SQL Output
```

---

## 🧩 Supported Databases

| Database     | Notes                                                                 |
|--------------|-----------------------------------------------------------------------|
| PostgreSQL   | Uses `information_schema` + `pg_catalog` views                        |
| SQL Server   | Uses `sys.*` catalog views (2005+)                                    |
| MySQL        | Uses `information_schema` and custom queries                          |
| Oracle       | Uses `ALL_*` views (DBA_* with higher privileges)                     |
| SQLite       | Uses PRAGMA statements per table (app must loop over tables manually) |

---

## 🔐 Security Considerations

This system is **read-only**. It doesn't touch or modify data. All metadata comes from system views and catalogs.

---

## 🔧 Extending for New Databases

1. Create `{newdb}_metadata.sql` with required queries  
2. Add a new extractor in `UsqlMcp.Infrastructure`  
3. Register the new extractor in DI container  
4. Done. 🎯

---

## 🗺️ Roadmap

### ✅ Near-Term Goals
- Semantic models auto-generation using LLMs
- Function-call API schema generation
- Webhook integration for schema changes
- Query validation and linting
- Parametrized queries + history

### 🔮 Mid-Term Vision
- Schema change notifications via Webhook
- Metadata export (JSON, XML, CSV)
- Business glossary + lineage

### ⚡ Performance Plans
- Connection pooling
- Metadata caching
- Parallel metadata queries

---

 
