# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is an **Elasticsearch training materials repository** designed for corporate in-house developer training. It includes:
- Comprehensive training curriculum and textbooks (English and Turkish)
- ASP.NET Core 9.0 log simulator application for hands-on practice
- Docker environment with Elasticsearch 8.19.2 and Kibana
- Practical HTTP request examples for exercises

**Target Audience:** Mid to senior-level full-stack developers
**Training Duration:** 6-hour, 1-day workshop
**Focus:** Log management and analysis scenarios

## Development Commands

### Docker Environment
```bash
# Start Elasticsearch and Kibana
cd docker
docker-compose up -d

# Check service status
docker-compose ps

# View Elasticsearch logs
docker-compose logs elasticsearch

# Stop services
docker-compose down
```

Services will be available at:
- Elasticsearch: http://localhost:9200
- Kibana: http://localhost:5601
- Training App (when using Docker): http://localhost:8080

### .NET Application
```bash
# Navigate to application directory
cd src/ElasticTraining

# Restore dependencies
dotnet restore

# Run the application (local development)
dotnet run

# Run with hot reload
dotnet watch run

# Run tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"
```

The application will be available at: https://localhost:5001

## Architecture Overview

### Application Structure
- **ASP.NET Core 9.0 Razor Pages** - UI framework
- **NEST 7.17.5** - Elasticsearch .NET client library (⚠️ **End-of-life late 2025** - see migration note below)
- **Background Services** - Continuous log generation and historical data creation

### Key Components

#### Services Layer (`src/ElasticTraining/Services/`)
- **ElasticsearchService**: Core Elasticsearch operations
  - Index template creation (`application_logs`)
  - Products index setup with mappings
  - Cluster health monitoring
  - Sample data loading with bulk operations
- **LogGeneratorService**: Background service (implements `BackgroundService`)
  - Real-time log generation (200-1000ms intervals)
  - Historical data generation (30 days with business hour patterns)
  - Generates logs across 6 microservices: auth-service, user-service, product-catalog, order-service, payment-service, notification-service
  - Log level distribution: 70% INFO, 20% WARN, 10% ERROR

#### Models (`src/ElasticTraining/Models/`)
- **ApplicationLog**: Daily log indices (`application_logs-YYYY-MM-DD`)
  - Structured logging with correlation IDs, service names, log levels
  - HTTP metrics (status codes, response times)
  - Exception handling with stack traces and inner exceptions
- **Product**: Training dataset for search/aggregation exercises
  - SKU-based identification
  - Category, tags, pricing, and inventory fields

### Index Design

#### Application Logs Template
- **Pattern**: `application_logs-*`
- **Dynamic mapping**: `false` (prevents mapping explosion)
- **Key fields**:
  - `@timestamp` (date), `correlation_id` (keyword)
  - `service_name`, `level`, `logger_name` (keyword for aggregations)
  - `host_ip` (ip type), `message` (text for full-text search)
  - `exception.stack_trace` (text, `index: false` for performance)
  - `http_status_code` (integer), `response_time_ms` (long)

#### Products Index
- **Auto-mapped** from Product model using NEST attributes
- Sample data includes electronics, accessories, furniture categories
- Used for demonstrating search DSL and aggregations

### Configuration

**Elasticsearch Connection** (`appsettings.json`):
```json
{
  "Elasticsearch": {
    "Uri": "http://localhost:9200",
    "Username": "",
    "Password": ""
  }
}
```

**Docker Environment Variables**:
- `ASPNETCORE_ENVIRONMENT=Development`
- `Elasticsearch__Uri=http://es01:9200`

## Content Guidelines (from Copilot Instructions)

When working on training materials or documentation:

### Language and Style
- **Documentation language**: Turkish for narrative/explanations
- **Technical elements**: English for all code, API endpoints, index names, field names, JSON structures
- **Tone**: Friendly, occasionally humorous, motivating (e.g., "Cesur geliştirici", "veri okyanusu")
- **Format**: Markdown for all textual content
- **Practical focus**: Log management and analysis scenarios

### Key Training Materials
- **Lesson Plan**: `docs/Lesson-Plan.md` (English) / `docs/Lesson-Plan.tr.md` (Turkish)
  - Instructor guide with timing and detailed curriculum
  - 6 sections covering Elasticsearch fundamentals to production considerations
- **Student Textbook**: `docs/Textbook/Textbook.md` (English) / `docs/Textbook.tr.md` (Turkish)
  - Complete learning materials in modular format (Section01.md through Section06.md)
  - Turkish textbook uses Turkish narrative with English technical terms
- **HTTP Examples**: `docs/HttpRequests/` - Practical exercise collections

### Training Sections
1. Introduction to Elasticsearch - Architecture and basic operations
2. Data Modeling and Mapping - Document structure and index design
3. Search and Query DSL - Query construction and search capabilities
4. Aggregations and Analytics - Data analysis operations
5. Log Management - Practical log analysis scenarios
6. Production Considerations - Performance and best practices

## Important Notes

- **Security disabled**: The Docker setup has `xpack.security.enabled=false` for training simplicity. DO NOT use in production.
- **NEST bulk operations**: The codebase includes workarounds for NEST's bulk response parsing quirks (see ElasticsearchService.cs:159-206)
- **Historical data generation**: Business-aware patterns (10% less logs on weekends and outside 08:00-17:00 business hours)
- **Index lifecycle**: Daily indices allow for realistic time-series data management demonstrations

## Version Information and Migration Path

### Elasticsearch Versions
- **Current training version**: 8.19.2
- **Compatibility**: Training materials work with both Elasticsearch 8.x and 9.x
- **Production recommendation**: 8.18.2+ or 9.0+ depending on requirements
- For detailed version comparison and migration strategy, see `docs/Textbook/Section06.md`

### .NET Client Migration

**Current: NEST 7.17.5**
- Final version in the NEST 7.x series
- Compatible with .NET 9.0 for training purposes
- ⚠️ **Scheduled for end-of-life in late 2025**
- No further updates planned

**Future: Elastic.Clients.Elasticsearch**
- Official successor to NEST
- Latest version: 9.x
- Supports both Elasticsearch 8.x and 9.x
- Modern API with improved fluent syntax
- Active development and long-term support

**Migration considerations:**
- Training materials currently use NEST and will continue to work
- For new production projects, consider `Elastic.Clients.Elasticsearch`
- Migration guide: https://www.elastic.co/guide/en/elasticsearch/client/net-api/current/migration-guide.html
- Code changes required: API syntax differs between NEST and new client
