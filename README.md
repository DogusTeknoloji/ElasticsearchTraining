# Elasticsearch Training Materials & Log Simulator

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4.svg)](https://dotnet.microsoft.com/)
[![Elasticsearch](https://img.shields.io/badge/Elasticsearch-8.13.4-005571.svg)](https://www.elastic.co/)

This repository contains comprehensive Elasticsearch training materials designed for developers, including a practical .NET 9.0 log simulator application for hands-on learning.

**Türkçe:** Bu proje, şirket içi yazılım geliştiricilere yönelik kapsamlı bir Elasticsearch eğitimi için geliştirilmiş materyaller ve pratik bir .NET 9.0 log simülatörü uygulaması içerir. Daha fazla Türkçe kaynak için aşağıdaki belgelere göz atın.

## 📚 What's Included (İçerik)

- **Complete Training Curriculum (Kapsamlı Eğitim Müfredatı)**
  - English: [Lesson Plan](docs/Lesson-Plan.md) - Instructor guide with timing and detailed curriculum.
  - Türkçe: [Ders Planı](docs/Lesson-Plan.tr.md) - Eğitmen kılavuzu, zamanlama ve detaylı müfredat.
- **Student Textbook (Öğrenci Ders Kitabı)**
  - English: [Textbook](docs/Textbook/Textbook.md) - Complete learning materials in modular format.
  - Türkçe: [Ders Kitabı](docs/Textbook/Textbook.tr.md) - Modüler formatta eksiksiz öğrenim materyalleri.
- **Practical Exercises (Pratik Uygulamalar)** - HTTP request collections for hands-on practice. ([HTTP Examples](docs/HttpRequests/))
- **.NET Log Simulator (.NET Log Simülatörü)** - ASP.NET Core application that generates realistic log data for training scenarios.
- **Docker Environment (Docker Ortamı)** - Ready-to-use Elasticsearch and Kibana setup.

## 🎯 Features (Özellikler)

- **Index Template Management** - Automatic creation of `application_logs` template with proper mappings
- **Products Index Setup** - Pre-configured product data index for training exercises  
- **Sample Data Loading** - Automated loading of sample datasets for immediate practice
- **Real-time Log Generation** - Background service generating continuous log streams with realistic patterns
- **Historical Data Generation** - Creates 30 days of historical log data with business hour patterns
- **Connection Monitoring** - Real-time Elasticsearch cluster health and connection status
- **Training-focused UI** - Simple interface designed specifically for educational scenarios

## 🔧 Prerequisites

- .NET 9.0 SDK or later
- Docker and Docker Compose
- 4GB+ RAM available for Elasticsearch
- Web browser for Kibana access

## 🚀 Quick Start (Hızlı Başlangıç)

### 1. Clone the Repository

```bash
git clone https://github.com/DogusTeknoloji/ElasticsearchTraining.git
cd ElasticsearchTraining
```

### 2. Start Elasticsearch and Kibana

```bash
# Navigate to docker directory
cd docker

# Start the stack
docker-compose up -d

# Verify services are running
docker-compose ps
```

Wait for services to be ready (usually 2-3 minutes). You can check:

- Elasticsearch: <http://localhost:9200>
- Kibana: <http://localhost:5601>

### 3. Run the Training Application (Eğitim Uygulamasını Çalıştırın)

```bash
# Navigate to the application directory (Uygulama dizinine gidin)
cd src/ElasticTraining

# Restore dependencies (Bağımlılıkları yükleyin)
dotnet restore

# Run the application (Uygulamayı çalıştırın)
dotnet run
```

The application will be available at (Uygulama şu adreste mevcut olacaktır): <https://localhost:5001>

## 📖 Training Materials (Eğitim Materyalleri)

### Course Structure (Ders Yapısı)

The training is organized into 6 sections:

1. **Introduction to Elasticsearch** - Architecture, concepts, and basic operations
2. **Data Modeling and Mapping** - Document structure, field types, and index design
3. **Search and Query DSL** - Powerful search capabilities and query construction
4. **Aggregations and Analytics** - Data analysis and statistical operations
5. **Log Management** - Practical log analysis scenarios
6. **Production Considerations** - Performance, monitoring, and best practices

### Documentation (Dokümantasyon)

- **Lesson Plan (Ders Planı):**
  - English: [Lesson Plan](docs/Lesson-Plan.md)
  - Türkçe: [Ders Planı](docs/Lesson-Plan.tr.md)
- **Student Textbook (Öğrenci Ders Kitabı):**
  - English: [Textbook](docs/Textbook/Textbook.md) (links to `Section0X.md` files)
  - Türkçe: [Ders Kitabı](docs/Textbook/Textbook.tr.md) (links to `Section0X.tr.md` files)
- **HTTP Examples (HTTP Örnekleri):** [HTTP Requests](docs/HttpRequests/) (Contains `.http` files for practice)

## 💡 How to Use for Training (Eğitim İçin Nasıl Kullanılır)

### For Instructors (Eğitmenler İçin)

1. Review the lesson plans: [English Lesson Plan](docs/Lesson-Plan.md) or [Turkish Lesson Plan](docs/Lesson-Plan.tr.md).
2. Start Elasticsearch and Kibana using Docker Compose.
3. Run the log simulator application.
4. Use provided [HTTP request examples](docs/HttpRequests/) for demonstrations.
5. Guide students through hands-on exercises.

### For Self-Learning (Bireysel Öğrenim İçin)

1. Start with the textbook materials: [English Textbook](docs/Textbook/Textbook.md) or [Turkish Textbook](docs/Textbook/Textbook.tr.md).
2. Set up the environment using Docker Compose.
3. Work through practical exercises using the log simulator.
4. Practice with provided [HTTP request collections](docs/HttpRequests/).
5. Explore real-time data analysis with generated logs.

## 🔍 Training Scenarios (Eğitim Senaryoları)

### Real-time Log Analysis

The application generates realistic log data including:

- **Service Logs** - Application performance and error tracking
- **HTTP Request Logs** - API usage patterns and response times  
- **Exception Handling** - Error analysis with stack traces
- **Business Metrics** - Service-level indicators and trends

### Sample Queries

Test your skills with these scenarios in Kibana Dev Tools:

```bash
# Monitor error rates by service
GET /application_logs-*/_search
{
  "size": 0,
  "aggs": {
    "error_by_service": {
      "terms": {"field": "service_name"},
      "aggs": {
        "error_rate": {
          "filter": {"term": {"level": "ERROR"}}
        }
      }
    }
  }
}

# Find slow API responses
GET /application_logs-*/_search
{
  "query": {
    "range": {
      "response_time_ms": {"gte": 1000}
    }
  },
  "sort": [{"response_time_ms": {"order": "desc"}}]
}
```

## ⚙️ Configuration

### Application Settings

Configure Elasticsearch connection in `src/ElasticTraining/appsettings.json`:

```json
{
  "Elasticsearch": {
    "Uri": "http://localhost:9200",
    "Username": "",
    "Password": ""
  }
}
```

### Docker Configuration

The included Docker Compose setup provides:

- **Elasticsearch 8.13.4** - Single node cluster optimized for development
- **Kibana 8.13.4** - Data visualization and dev tools interface
- **Persistent Storage** - Data persists between container restarts
- **Memory Settings** - 1GB heap size (adjust based on your system)

## �️ Generated Data Structures

### Application Logs Schema

**Index Pattern:** `application_logs-YYYY-MM-DD`

```json
{
  "@timestamp": "2024-05-25T10:30:00.000Z",
  "correlation_id": "abc123-def456",
  "service_name": "user-service",
  "level": "INFO",
  "thread_name": "http-nio-8080-exec-1",
  "logger_name": "com.company.UserController",
  "host_ip": "192.168.1.10",
  "message": "User login successful",
  "http_status_code": 200,
  "response_time_ms": 145
}
```

### Products Index Schema

**Index Name:** `products`

```json
{
  "sku": "LAPTOP-DELL-001",
  "name": "Dell Latitude 7420",
  "description": "Business laptop with Intel i7",
  "price": 1299.99,
  "stock_quantity": 25,
  "category": "Electronics",
  "tags": ["laptop", "business", "intel"],
  "created_date": "2024-05-25",
  "is_active": true
}
```

## 🧪 Development and Testing

### Running Tests

```bash
# Run unit tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Development Mode

```bash
# Run in development mode with hot reload
dotnet watch run --project src/ElasticTraining
```

### Debugging

The application includes detailed logging and error handling for troubleshooting Elasticsearch connectivity and data generation issues.

## 🤝 Contributing

We welcome contributions to improve the training materials! Here's how:

1. **Fork** the repository
2. **Create** a feature branch (`git checkout -b feature/amazing-feature`)
3. **Commit** your changes (`git commit -m 'Add amazing feature'`)
4. **Push** to the branch (`git push origin feature/amazing-feature`)
5. **Open** a Pull Request

### Guidelines

- Follow existing code style and conventions
- Add tests for new functionality
- Update documentation as needed
- Ensure all training materials remain accurate

## 📋 Changelog

See [CHANGELOG.md](CHANGELOG.md) for detailed version history.

## 🐛 Troubleshooting

### Common Issues

**Elasticsearch Connection Failed**

- Verify Docker containers are running: `docker-compose ps`
- Check Elasticsearch logs: `docker-compose logs elasticsearch`
- Ensure port 9200 is not blocked by firewall

**Out of Memory Errors**

- Increase Docker memory allocation (4GB+ recommended)
- Adjust ES_JAVA_OPTS in docker-compose.yml

**Slow Performance**

- Monitor system resources during log generation
- Reduce log generation frequency in application settings
- Consider using SSD storage for Docker volumes

## 📞 Support

- 📖 **Documentation**: Check the [docs](docs/) directory
- 🐛 **Issues**: Report bugs via [GitHub Issues](https://github.com/YOUR_USERNAME/ElasticsearchTraining/issues)
- 💬 **Discussions**: Join discussions in [GitHub Discussions](https://github.com/YOUR_USERNAME/ElasticsearchTraining/discussions)

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- **Elastic Stack Team** - For creating powerful search and analytics tools
- **Microsoft .NET Team** - For the excellent development platform
- **Training Participants** - For valuable feedback and contributions

## 🔗 Related Resources

- [Elasticsearch Official Documentation](https://www.elastic.co/guide/en/elasticsearch/reference/current/)
- [Kibana User Guide](https://www.elastic.co/guide/en/kibana/current/)
- [.NET Documentation](https://docs.microsoft.com/en-us/dotnet/)
- [ASP.NET Core Guide](https://docs.microsoft.com/en-us/aspnet/core/)

---

**Made with ❤️ for the developer community**
