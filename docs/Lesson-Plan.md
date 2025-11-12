# Elasticsearch Training: Comprehensive Course Content (6 Hours)

## Main Objective of the Training:

To enable participants to understand the basic architecture and key concepts of Elasticsearch, grasp data indexing, searching, and aggregation capabilities, and apply this knowledge in practical scenarios. Additionally, it aims for them to recognize the types of problems Elasticsearch solves and the fundamental points to consider in a production environment.

## Target Audience:

Mid to Senior Level Full Stack Software Developers. (Assumed to be familiar with REST APIs, JSON, basic database, and distributed system concepts.)

## Prerequisites and Required Environment:

* **Docker:** Docker Desktop must be installed and running on each participant's computer.

* **Elasticsearch & Kibana Setup:** Before the training, participants will be provided with a simple `docker-compose.yml` file to launch a recent and stable version of Elasticsearch and Kibana (e.g., 8.x series) on their machines with a single command. This prevents wasting time on setup during the training.

  * Example `docker-compose.yml` content:

    ```yaml
    version: '3.7'
    services:
      elasticsearch:
        image: docker.elastic.co/elasticsearch/elasticsearch:8.19.2 # Check for the current stable version
        container_name: es01
        environment:
          - discovery.type=single-node
          - xpack.security.enabled=false # Security disabled for training ease, should be enabled in production!
          - ES_JAVA_OPTS=-Xms1g -Xmx1g # Adjustable based on resources
        ports:
          - "9200:9200"
        volumes:
          - esdata01:/usr/share/elasticsearch/data
      kibana:
        image: docker.elastic.co/kibana/kibana:8.19.2 # Should be the same version as Elasticsearch
        container_name: kib01
        ports:
          - "5601:5601"
        depends_on:
          - elasticsearch
        environment:
          - ELASTICSEARCH_HOSTS=http://es01:9200
    volumes:
      esdata01:
        driver: local
    ```

* **Internet Access:** For documentation and resource research if needed.

* **Kibana Dev Tools:** All practical exercises will be performed via Kibana > Management > Dev Tools.

## Course Flow and Time Plan

**Total Training Duration:** ~360 Minutes (Net Lecture + Short Breaks)
*This plan covers 6 hours of *active* training and short breaks. If a long lunch break is given midday, the total duration will be extended accordingly.*

* **Module 1: Introduction to Elasticsearch and Basic Concepts** (80 Minutes)
* **Break** (15 Minutes)
* **Module 2: Data Management: Indexing and Mapping** (85 Minutes)
* **Break** (15 Minutes) *(If this falls midday, this break can also be planned as a longer lunch break.)*
* **Module 3: The Art of Searching: Querying with Query DSL** (80 Minutes)
* **Break** (15 Minutes)
* **Module 4: Advanced Search, Aggregations, and the Elastic Stack Ecosystem** (85 Minutes)

### Module Details

#### **Module 1: Introduction to Elasticsearch and Basic Concepts (80 Minutes)**

* **(10 min) Introductions, Expectations, and Training Roadmap**
  * Brief introduction of the instructor and participants.
  * Participants' current knowledge level and expectations regarding Elasticsearch.
  * Overview of the training content and objectives.
* **(20 min) "Why Elasticsearch?"**
  * Challenges of traditional relational databases (RDBMS) in full-text search and big data analysis.
  * Inadequacy of `LIKE '%...%'` queries: Performance, relevance, language support.
  * The growing importance of unstructured and semi-structured data (logs, texts, JSON documents).
  * Real-world scenarios: Product search in e-commerce, log analysis, anomaly detection.
* **(25 min) What is Elasticsearch?**
  * A distributed, RESTful search and analytics engine built on Apache Lucene.
  * Key Features: Speed, scalability, flexibility, near real-time results.
  * **What it is NOT?** Emphasize that it is not a primary ACID-compliant database. Position it as a "Derived Data Store" or "Search/Analytics Layer" instead of a "Source of Truth."
  * Common Use Cases:
    * Full-Text Search (Advanced site search, product catalog search)
    * Log and Metric Analysis (Observability: APM, logs, metrics)
    * Business Intelligence and Analytical Reporting
    * Security Analytics (SIEM - Security Information and Event Management)
    * Geospatial Search and Analysis
* **(25 min) Basic Architectural Concepts**
  * **Document:** The basic unit of data in JSON format.
  * **Index:** A collection of documents with similar characteristics. (Can be thought of as a "table" in RDBMS).
    * The role of `_type` in older versions and its removal in current versions (post-7.x).
  * **Node:** A single Elasticsearch server that is part of a cluster.
    * Node Types: Master-eligible, Data, Ingest, Coordinating, Machine Learning. (Briefly mention)
  * **Cluster:** A structure composed of one or more Nodes, sharing data and workload.
  * **Shard:** Horizontally divided parts of an index. Provides scalability and parallelization.
    * Primary Shard.
  * **Replica:** Copies of Primary Shards. Enhance high availability and read performance.
  * **Practical Application (Kibana Dev Tools):**
    * `GET /` : View cluster information.
    * `GET /_cluster/health` : Check cluster health status.
    * `GET /_cat/nodes?v` : List nodes.
    * `GET /_cat/indices?v` : List existing indices.

#### **Module 2: Data Management: Indexing and Mapping (85 Minutes)**

* **(20 min) Document Management (CRUD Operations)**
  * **Index API (Add/Update Document):**
    * `PUT /{index}/_doc/{id}` : Add/overwrite a document with a specific ID.
    * `POST /{index}/_doc` : Add a document with Elasticsearch automatically generating an ID.
    * `_create` endpoint: `POST /{index}/_create/{id}` (Creates only if ID does not exist).
  * **Get API (Read Document):** `GET /{index}/_doc/{id}`
  * **Update API (Update Document):** `POST /{index}/_update/{id}`
    * Partial update (with script or `doc`).
    * `upsert` concept.
  * **Delete API (Delete Document):** `DELETE /{index}/_doc/{id}`
  * **Meta Fields:** `_index`, `_id`, `_version`, `_source`, `_score` (in search results).
* **(15 min) Bulk API (`_bulk`)**
  * Why batch operations? Performance and efficiency.
  * Structure of the `_bulk` endpoint: action/metadata and optional source lines.
  * Supported actions: `index`, `create`, `update`, `delete`.
  * Error handling and interpretation of results.
* **(35 min) Mapping (Schema Management): The Constitution of Your Data**
  * **Dynamic Mapping:** Elasticsearch's automatic detection of data types.
    * Advantages: Quick start, flexibility.
    * Disadvantages: Incorrect type inferences, unnecessary fields, performance issues, "mapping explosion." Why it's not recommended for production.
  * **Explicit Mapping:** Manually defining field types and properties when creating an index.
    * Creating/updating mapping with `PUT /{index}` (with its limitations).
    * **Basic Data Types and Their Uses:**
      * `text`: Analyzed text for full-text search (e.g., product description, blog content). `analyzers`.
      * `keyword`: Text used for exact matches, sorting, aggregation (e.g., category ID, tags, status codes, email address).
      * Numeric Types: `long`, `integer`, `short`, `byte`, `double`, `float`, `half_float`, `scaled_float`.
      * `date` and `date_nanos`: Date and time formats. `format` parameter.
      * `boolean`: `true` / `false`.
      * `ip`: IPv4 and IPv6 addresses.
      * `geo_point`, `geo_shape`: Geospatial data.
      * Complex Types: `object` (nested JSON objects), `nested` (special type for arrays of objects).
  * **Mapping Parameters (Frequently Used):**
    * `index`: Whether the field should be indexed (`true`/`false`).
    * `analyzer`: Text analyzer for `text` fields (standard, simple, whitespace, custom, etc.).
    * `search_analyzer`: Analyzer to be used at search time.
    * `fields` (multi-fields): Indexing the same field in different ways (e.g., a string field as both `text` and `keyword`).
    * `dynamic`: `true`, `false`, `strict`. Controlling dynamic mapping behavior for object fields.
* **(15 min) Practical Application (Kibana Dev Tools):**
  * Create an explicit mapping for an index named `products` (e.g., `name: text`, `sku: keyword`, `price: float`, `tags: keyword[]`, `created_date: date`, `description: text` with multi-field for `.keyword`).
  * Add various product documents using the `_bulk` API.
  * Fetch a few documents with `GET`, update one with `_update`, and delete one with `DELETE`.
  * Examine the mapping with `GET /{index}/_mapping`.

#### **Module 3: The Art of Searching: Querying with Query DSL (80 Minutes)**

* **(15 min) Anatomy of a Search Query (`_search` API)**
  * URI Search vs Request Body Search. Why prefer Request Body?
  * Basic Query Structure: `query` object.
  * **Query Context vs Filter Context:**
    * **Query Context:** "How well does this document match the query?" Relevance score (`_score`) is calculated. Queries like `match`, `multi_match` run here.
    * **Filter Context:** "Does this document match the query (yes/no)?" Score is not calculated. Generally faster and cacheable. Queries like `term`, `range` are more effective here. The importance of using `filter` for performance.
* **(45 min) Basic Query Types (Query DSL)**
  * **Full-Text Queries (Generally used in Query Context):**
    * `match`: Standard full-text search. `operator` (`OR`/`AND`), `fuzziness`.
    * `match_phrase`: Exact phrase search. `slop` parameter.
    * `multi_match`: Run the same query on multiple fields. `fields`, `type` (best_fields, most_fields, cross_fields).
    * `query_string` / `simple_query_string`: Supports Lucene query syntax. Use with caution.
  * **Term-Level Queries (Generally used in Filter Context):**
    * `term`: Search for unanalyzed, exact matching values (usually for `keyword` fields).
    * `terms`: For multiple `term` values (like `IN` clause).
    * `ids`: Retrieve documents with specific IDs.
    * `range`: For numeric, date, or string ranges (`gt`, `gte`, `lt`, `lte`).
    * `exists`: Check if a specific field exists.
    * `prefix`: Search `keyword` fields starting with a specific prefix.
    * `wildcard`: Pattern matching with `*` and `?` characters (beware of performance!).
  * `match_all`: Retrieve all documents.
  * `match_none`: Retrieve no documents.
* **(20 min) Combining Queries: `bool` Query**
  * The most frequently used and powerful query combination tool.
  * Clause and Meaning:
    * `must`: All sub-queries must match (AND). Contributes to scoring in query context.
    * `filter`: All sub-queries must match (AND). Runs in filter context, no effect on scoring, cacheable.
    * `should`: At least one of the sub-queries must match (OR). `minimum_should_match` parameter. Contributes to scoring in query context.
    * `must_not`: None of the sub-queries should match (NOT). Runs in filter context.
  * Creating complex logic with nested `bool` queries.
* **Practical Application (Kibana Dev Tools):**
  * In the `products` index:
    * Search for products with "laptop" in their name using `match`.
    * Search for products with a price between 1000-2000 TL using `range` in filter context.
    * Search for products that have the "gaming" tag (`term`) AND a price higher than 5000 TL (`range`) using a `bool` query (using `must` or `filter`).
    * Search for products whose description contains "fast processor" OR whose name contains "new generation" using a `bool` query (using `should`).

#### **Module 4: Advanced Search, Aggregations, and the Elastic Stack Ecosystem (85 Minutes)**

* **(20 min) Managing and Improving Search Results**
  * Pagination: `from` and `size`. (Brief mention of deep pagination issues: `search_after`)
  * Sorting: `sort` (field name, `asc`/`desc`, by `_score`). Sorting by multiple fields.
  * Source Filtering: `_source` (retrieving/excluding specific fields).
  * Highlighting: Highlighting search terms in results.
  * Brief mention: Understanding how scoring is done with the `explain` API.
* **(30 min) Aggregations: Grouping and Analyzing Data**
  * The Elasticsearch equivalent of SQL's `GROUP BY` and aggregate functions (SUM, COUNT, etc.).
  * **Basic Aggregation Types:**
    * **Metric Aggregations:** Perform calculations on numeric values.
      * `min`, `max`, `avg`, `sum`
      * `stats` (provides all of them at once)
      * `cardinality` (estimates the number of unique values - HyperLogLog++)
      * `percentiles`, `percentile_ranks`
    * **Bucket Aggregations:** Group documents into buckets based on specific criteria.
      * `terms`: Group by unique values in a field (e.g., number of products per category). `size`, `order`.
      * `range`, `date_range`: Group by specified ranges.
      * `histogram`, `date_histogram`: Group by fixed intervals (e.g., price ranges, monthly sales). `interval`.
      * `filter` / `filters`: Put documents matching one or more filters into separate buckets.
  * **Nested Aggregations (Sub-aggregations):** Further detailing a bucket aggregation result with another aggregation (metric or bucket). (E.g., Average price of products in each category).
* **Practical Application (Kibana Dev Tools):**
  * In the `products` index:
    * Find how many products exist for each `tag` using `terms` aggregation.
    * Find the average, minimum, and maximum prices of all products using `avg`, `min`, `max` (or `stats`) aggregation.
    * Find the average product price for each `tag` using nested aggregation (`terms` bucket, with `avg` metric inside).
    * Find the monthly distribution of products based on their creation dates using `date_histogram`.
* **(20 min) Overview of the Elastic Stack (ELK/Elastic Stack) Ecosystem**
  * **Kibana:** Data visualization and exploration tool.
    * **Discover:** Interactively examine, filter, and search raw data.
    * **Visualize Library:** Create different types of charts (bar, line, pie, map, etc.).
    * **Dashboard:** Collect multiple visualizations on a single screen.
    * (A brief demo can show the steps to create a simple visualization and dashboard in Kibana.)
  * **Logstash:** Server-side data processing pipeline. Collect data from various sources, transform/enrich it, and send it to Elasticsearch (or other destinations).
  * **Beats:** Lightweight, single-purpose data shippers.
    * Filebeat (log files), Metricbeat (system/service metrics), Packetbeat (network data), Winlogbeat (Windows event logs), etc.
  * A simple diagram of the Stack's general architecture and how components work together.
* **(15 min) Closing: Summary, Production Tips, and Q&A**
  * A quick summary of the main topics learned in the training.
  * **Considerations for Moving to Production (Brief Headlines):**
    * Security (Enabling X-Pack Security, role-based access).
    * Monitoring (Monitoring cluster and node health).
    * Index Lifecycle Management (ILM) (Automatic index management for time-based data).
    * Shard and replica planning, capacity planning.
    * Backup and restore strategies.
  * Resources for continued learning: Official Elasticsearch documentation, Elastic Blog, Elastic Community.
  * Answering participants' questions.

This course content aims for participants to gain both theoretical knowledge and practical skills. I am confident that you will enrich this content with your experience and deliver an unforgettable training for the participants. Good luck!
