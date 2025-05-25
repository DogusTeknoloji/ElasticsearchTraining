# Chapter 4: The Power of Analysis: Mastering Data with Aggregations

Elasticsearch not only finds data but also has incredibly powerful tools to make sense of this data, summarize it, and extract insights. The most important of these tools is the **Aggregation API**. You can think of it as a much more flexible and powerful version of SQL's `GROUP BY` and aggregate functions (SUM, COUNT, AVG, etc.). With the philosophy "Let the data speak, we listen," we will uncover the stories hidden in your data using aggregations.

## 4.1 Introduction to Aggregations: What Are They, What Do They Do?

Aggregations allow you to divide documents in your search results into groups (bucketing) and perform various metrics (calculations) on these groups. For example:

* How many products are in each category?
* Which are the most popular tags?
* What is the average stock quantity of products in a specific price range?
* What are the monthly sales trends?
* Which service generates the most error logs?
* At what times of the day does error density increase?

We can easily find the answers to such questions with aggregations. Aggregations are defined under the `aggs` (or `aggregations`) key of the query sent to the `_search` endpoint.

```http
POST /products/_search
{
  "query": {
    "match_all": {}
  },
  "aggs": {
    "my_first_aggregation": {
      // Aggregation definition will go here
    }
  },
  "size": 0
}
```

The `"my_first_aggregation"` part determines the name under which the aggregation result will be returned in the response. You can give it any name you want.

Aggregations are basically divided into two main categories:

1.  **Bucket Aggregations:** Divides documents into groups (buckets) based on specific criteria. Each bucket contains documents that match that criterion.
2.  **Metric Aggregations:** Performs numerical calculations (sum, average, min, max, etc.) on documents in buckets (or in the entire result set).

These two types are often used nested: first, we divide into buckets, then we calculate metrics for each bucket.

## 4.2 Bucket Aggregations: Dividing Data into Meaningful Groups

Let's take a look at the most commonly used bucket aggregations:

*   **`terms` Aggregation: Grouping by Unique Values**
    Groups documents based on unique values in a specific field. Like `GROUP BY some_field` in SQL.
    Example: Let's find out how many products are in each category.

    ```http
    POST /products/_search
    {
      "aggs": {
        "products_by_category": {
          "terms": { "field": "category" }
        }
      },
      "size": 0
    }
    ```

    In the response, a bucket for each category and the number of documents in that bucket (`doc_count`) will be returned.
    *   `size`: Determines how many buckets will be returned (default is 10).
    *   `order`: Specifies how the buckets will be sorted (e.g., `{"_count": "desc"}` by document count descending).

*   **`range` / `date_range` Aggregation: Grouping by Specific Ranges**
    Groups by ranges you define in numeric or date fields.
    Example: Product counts by price ranges.

    ```http
    POST /products/_search
    {
      "aggs": {
        "products_by_price_range": {
          "range": {
            "field": "price",
            "ranges": [
              { "to": 100.0 },
              { "from": 100.0, "to": 500.0 },
              { "from": 500.0 }
            ]
          }
        }
      },
      "size": 0
    }
    ```

    `date_range` is used similarly for date ranges (`format` and `time_zone` parameters are important).

*   **`histogram` / `date_histogram` Aggregation: Grouping by Fixed Intervals**
    Creates buckets based on a fixed interval (`interval`) in numeric or date fields.
    Example: Distribution of product prices in intervals of 500.

    ```http
    POST /products/_search
    {
      "aggs": {
        "products_by_price_histogram": {
          "histogram": {
            "field": "price",
            "interval": 500,
            "min_doc_count": 1
          }
        }
      },
      "size": 0
    }
    ```

    For `date_histogram`, the `interval` value can be time units like `day`, `week`, `month`, `1d`, `7d`, `1M`. Very useful for log analysis and time series data.
    Example: Hourly log counts.

    ```http
    POST /application_logs-*/_search
    {
      "aggs": {
        "logs_over_time": {
          "date_histogram": {
            "field": "@timestamp",
            "calendar_interval": "1h",
            "time_zone": "Europe/Istanbul"
          }
        }
      },
      "size": 0
    }
    ```

*   **`filter` / `filters` Aggregation: Grouping Those That Match a Filter**
    `filter` puts documents matching a single filter into a single bucket. `filters` allows you to define multiple named filters and put those matching each into separate buckets.
    Example: Number of in-stock and out-of-stock products.

    ```http
    POST /products/_search
    {
      "aggs": {
        "stock_status_split": {
          "filters": {
            "filters": {
              "in_stock_products": { "term": { "is_active": true } },
              "out_of_stock_products": { "term": { "is_active": false } }
            }
          }
        }
      },
      "size": 0
    }
    ```

## 4.3 Metric Aggregations: Performing Calculations on Groups

After creating our buckets, we can perform various calculations on the data in these buckets (or in the entire result set).

*   **`min`, `max`, `avg`, `sum` Aggregations: Basic Statistics**
    Calculates the minimum, maximum, average, or total value of a specific numeric field.
    Example: Average price of all products.

    ```http
    POST /products/_search
    {
      "aggs": {
        "average_price": {
          "avg": { "field": "price" }
        },
        "total_stock": {
            "sum": { "field": "stock_quantity"}
        }
      },
      "size": 0
    }
    ```

*   **`stats` / `extended_stats` Aggregations: Comprehensive Statistics**
    `stats` provides `count`, `min`, `max`, `avg`, `sum` values in one go. `extended_stats` additionally offers more advanced statistics like `sum_of_squares`, `variance`, `std_deviation` (standard deviation).

    ```http
    POST /products/_search
    {
      "aggs": {
        "price_stats": {
          "stats": { "field": "price" }
        }
      },
      "size": 0
    }
    ```

*   **`cardinality` Aggregation: Count of Unique Values (Approximate)**
    Estimates the number of unique values in a specific field. Like `COUNT(DISTINCT field)` in SQL. Since exact counting can be expensive in large datasets, it uses the HyperLogLog++ algorithm to provide an approximate but fast result.
    Example: How many different categories are there?

    ```http
    POST /products/_search
    {
      "aggs": {
        "distinct_categories": {
          "cardinality": { "field": "category" }
        }
      },
      "size": 0
    }
    ```

    The `precision_threshold` parameter can be used to balance accuracy and performance.

*   **`percentiles` / `percentile_ranks` Aggregations: Percentiles**
    `percentiles` shows specific percentiles of the data (e.g., 50th (median), 95th, 99th). `percentile_ranks` shows which percentile specific values correspond to. Very useful for performance monitoring (e.g., what percentage of requests complete under X ms?).

## 4.4 Nested Aggregations: Delving into the Details of Details

The real power of aggregations comes from their ability to be nested. We can perform much more detailed analyses by adding another bucket or metric aggregation under a bucket aggregation.

Example (for Products): Let's find the average price (`avg` metric) of products in each category (`terms` bucket).

```http
POST /products/_search
{
  "aggs": {
    "categories": {
      "terms": { "field": "category" },
      "aggs": {
        "average_price_per_category": {
          "avg": { "field": "price" }
        }
      }
    }
  },
  "size": 0
}
```

In the response, the average price of products belonging to that category will also be included within each category bucket.

Example (for Logs): Let's find the number of logs at different log levels (`level`) for each service (`service_name`).

```http
POST /application_logs-*/_search
{
  "aggs": {
    "logs_by_service": {
      "terms": { "field": "service_name" },
      "aggs": {
        "logs_by_level": {
          "terms": { "field": "level" }
        }
      }
    }
  },
  "size": 0
}
```

You can deepen this nested structure as much as you want (of course, keeping performance implications in mind).

Aggregations are one of the most important features that show Elasticsearch is not just a search engine, but also a powerful analytical platform. Be sure to take a detailed look at the [Elasticsearch Aggregations Documentation](https://www.elastic.co/guide/en/elasticsearch/reference/current/search-aggregations.html) for all aggregation types and options.

## 4.5 Practice Time: Let's Analyze Our `products` Data!

1.  **Find the top 5 categories with the most products and the number of products in each category:**

    ```http
    POST /products/_search
    {
      "aggs": {
        "top_categories": {
          "terms": {
            "field": "category",
            "size": 5,
            "order": { "_count": "desc" }
          }
        }
      },
      "size": 0
    }
    ```

2.  **Find the minimum, maximum, and average prices of all products:**

    ```http
    POST /products/_search
    {
      "aggs": {
        "price_overview": {
          "stats": { "field": "price" }
        }
      },
      "size": 0
    }
    ```

3.  **Find the number of products with each tag (`tags`) and the average stock quantity of products with these tags:**

    ```http
    POST /products/_search
    {
      "aggs": {
        "tags_analysis": {
          "terms": { "field": "tags" },
          "aggs": {
            "average_stock_per_tag": {
              "avg": { "field": "stock_quantity" }
            }
          }
        }
      },
      "size": 0
    }
    ```

These examples are just a starting point. You can create much more complex and interesting aggregations based on your own data and the questions you are curious about.

## 4.6 Practice Time: Let's Analyze Our `application_logs` Data!

Now let's do some meaningful analysis on our log data.

1.  **Find the total number of logs for each service (`service_name`):**

    ```http
    POST /application_logs-*/_search
    {
      "aggs": {
        "logs_per_service": {
          "terms": { "field": "service_name", "size": 10 }
        }
      },
      "size": 0
    }
    ```

2.  **Find the hourly counts of "ERROR" level logs within the last 24 hours:**

    ```http
    POST /application_logs-*/_search
    {
      "query": {
        "bool": {
          "filter": [
            { "term": { "level": "ERROR" } },
            { "range": { "@timestamp": { "gte": "now-24h/h" } } }
          ]
        }
      },
      "aggs": {
        "hourly_errors": {
          "date_histogram": {
            "field": "@timestamp",
            "calendar_interval": "1h",
            "min_doc_count": 1
          }
        }
      },
      "size": 0
    }
    ```

3.  **Find the top 5 host IPs (`host_ip`) generating the most errors (`level: ERROR`):**

    ```http
    POST /application_logs-*/_search
    {
      "query": {
        "term": { "level": "ERROR" }
      },
      "aggs": {
        "top_error_hosts": {
          "terms": {
            "field": "host_ip",
            "size": 5
          }
        }
      },
      "size": 0
    }
    ```

Aggregations performed on log data are invaluable for monitoring system health, detecting errors, and understanding performance bottlenecks.

---
[<- Previous Section: Section 03](Section03.md) | [Next Section: Section 05 ->](Section05.md)
