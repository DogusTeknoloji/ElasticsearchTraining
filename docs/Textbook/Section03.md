# Chapter 3: Mastering the Art of Search: Query DSL

We've successfully loaded our data into Elasticsearch and given it a nice identity card (mapping). Now it's time to navigate through that data without getting lost and reach the information we want at lightning speed! In this chapter, we'll meet Elasticsearch's heart, the **Query DSL (Domain Specific Language)**, and learn to write queries suitable for different search scenarios. We'll discover much more than just searching "like you type into Google." If you're ready, put on your search glasses and let's begin!

## 3.1 Anatomy of a Search Query: Meeting the `_search` API

To search in Elasticsearch, we primarily use the `_search` endpoint. We can send our queries to this endpoint via an HTTP GET or POST request.

* **URI Search (Simple Searches with GET):** For very simple searches, parameters can be sent via the URL (e.g., `q=searched_word`).

  ```http
  GET /products/_search?q=name:laptop
  GET /application_logs-2024-05-24/_search?q=level:ERROR
  ```

  This method is quick and easy but falls short for complex queries, and you might have to deal with URL encoding for some characters. It's okay for "quick peek" situations.

* **Request Body Search (Comprehensive Searches with POST):** This is where we'll show our real power! We send our queries in JSON format in the request body. This method is much more flexible and powerful.

  ```http
  POST /products/_search
  {
    "query": {
      // Query details will go here
    }
  }
  ```

  We will use this structure in all our subsequent examples.

**Query Context vs Filter Context: Score or Speed?**

When writing queries in Elasticsearch, we encounter two important "contexts": **Query Context** and **Filter Context**. Understanding the difference between these two is crucial for both getting accurate results and optimizing performance.

* **Query Context (under the `query` key):**
  * **Purpose:** Seeks to answer the question, "How **well** does this document match my search criteria?"
  * **Scoring (`_score`):** Calculates a **relevance score (`_score`)** for each matching document. The higher the score, the more relevant the document. Results are sorted by this score in descending order by default.
  * **Use Case:** Used in full-text searches when we want to find the results closest to what the user is looking for. For example, when searching based on words in a product name or description.
  * **Query Types:** Full-text queries like `match`, `multi_match`, `query_string` are typically used in this context.

* **Filter Context (under the `filter` key, usually within a `bool` query):**
  * **Purpose:** Seeks to answer the question, "Does this document match my search criteria? (Yes/No)".
  * **No Scoring:** Relevance score is not calculated. A document either matches the filter or it doesn't.
  * **Performance and Caching:** Since no scoring is done, it's generally faster than the query context. Also, filter results can often be **cached** by Elasticsearch, which significantly improves performance for repetitive queries. ES says, "I've seen this filter before, the result is ready!"
  * **Use Case:** Ideal for finding documents that meet exact matches, ranges, or a specific condition. For example, "products in the 'Electronics' category," "those with a price between 1000-2000 TL," "products in stock."
  * **Query Types:** Exact match or structural queries like `term`, `terms`, `range`, `exists` are typically used in this context.

**When to Use Which?**

* If you're doing a full-text search where finding the "most relevant" results and scoring is important, use **Query Context**.
* If you only want to filter documents that meet/do not meet a specific condition, get a yes/no answer, and prioritize performance, use **Filter Context**.
* Often, we use both together: within a `bool` query, we use both `must` (query context) and `filter` (filter context) clauses to get results that are both relevant and filtered.

This distinction will play a key role in optimizing both the accuracy and speed of your queries.

## 3.2 Basic Query Types: First Steps into Query DSL

Query DSL is a JSON-based language we use to tell Elasticsearch what we're looking for. It's quite rich and flexible. Let's now take a look at the most commonly used basic query types.

### 3.2.1 Full-Text Queries (Usually in Query Context)

These queries search on analyzed (tokenized) content in text fields.

* **`match` Query: Standard Full-Text Search**
  The most commonly used full-text query. It analyzes the given text (also analyzes the search term!) and finds matching documents.

  ```http
  POST /products/_search
  {
    "query": {
      "match": {
        "description": "powerful gaming laptop"
      }
    }
  }
  ```

  * `operator`: Defaults to `OR` (i.e., those containing "powerful" OR "gaming" OR "laptop"). If you set it to `AND`, all words must be present.

      ```http
      POST /products/_search
      {
        "query": {
          "match": {
            "description": {
              "query": "powerful gaming laptop",
              "operator": "AND"
            }
          }
        }
      }
      ```

  * `fuzziness`: Used to tolerate typos. Can take Levenshtein distance values like `AUTO` or `1`, `2`. So even if you type "Laptob," it finds "laptop."

  Can be used to search for a specific error code or word within log messages:

  ```http
  POST /application_logs-*/_search // Search across multiple log indices
  {
    "query": {
      "match": {
        "message": "timeout"
      }
    }
  }
  ```

* **`match_phrase` Query: Exact Phrase Matching**
  Expects the given words to appear in the same order and close to each other.

  ```http
  POST /products/_search
  {
    "query": {
      "match_phrase": {
        "name": "Awesome Laptop"
      }
    }
  }
  ```

  * `slop`: Specifies the maximum number of extra words allowed between the words in the phrase. With `slop: 1`, "Awesome Super Laptop" could also match.

* **`multi_match` Query: Searching in Multiple Fields**
  Used to search for the same term in multiple fields.

  ```http
  POST /products/_search
  {
    "query": {
      "multi_match": {
        "query": "silent keyboard",
        "fields": ["name", "description", "tags"]
      }
    }
  }
  ```

  * You can also use wildcards like `*` or `*_name` in the `fields` array. You can influence scoring by giving different weights to fields (e.g., `^3`).
  * `type`: Offers different matching strategies like `best_fields` (default, scores based on the best matching field), `most_fields` (matching in more fields gets a better score), `cross_fields` (treats fields as one large field, for structured data).

* **`query_string` / `simple_query_string` Query:**
  Allows you to use Lucene's powerful query syntax directly (AND, OR, NOT, wildcards, ranges, etc.).

  ```http
  POST /products/_search
  {
    "query": {
      "query_string": {
        "query": "(laptop OR gaming) AND category:Computers AND price:>1000",
        "default_field": "description"
      }
    }
  }
  ```

  `query_string` is very powerful but can lead to syntax errors or security vulnerabilities if used directly with user input. `simple_query_string` is a safer alternative; it ignores invalid syntax.

### 3.2.2 Term-Level Queries (Usually in Filter Context)

These queries search for **exact matches** on unanalyzed (stored as-is) values. They are typically used for `keyword`, numeric, date, and `boolean` fields. When used in the filter context, they are not included in scoring and can be cached.

* **`term` Query: Exact Match for a Single Value**
  Checks if the given value is found exactly in the field.

  ```http
  POST /products/_search
  {
    "query": {
      "term": {
        "category": "Accessories" // category field should be keyword
      }
    }
  }
  ```

  **Caution:** If you use the `term` query on a `text` field, the word you are searching for must exactly match the token produced after the analysis process (usually lowercase). You might need to search for "accessories" instead of "Accessories". That's why `match` is more suitable for `text` fields.

  To find records with a specific log level:

  ```http
  POST /application_logs-*/_search
  {
    "query": {
      "term": {
        "level": "ERROR" // level field should be keyword
      }
    }
  }
  ```

* **`terms` Query: Matching Multiple Values (like `IN`)**
  Checks if any of the specified values are found in the field.

  ```http
  POST /products/_search
  {
    "query": {
      "terms": {
        "tags": ["laptop", "gaming", "new-gen"] // tags field should be keyword
      }
    }
  }
  ```

* **`ids` Query: Fetching Documents with Specific IDs**

  ```http
  POST /products/_search
  {
    "query": {
      "ids": {
        "values": ["SKU001", "SKU003"]
      }
    }
  }
  ```

* **`range` Query: Values within a Specific Range**
  Finds values within a specific range in numeric, date, or string fields.

  ```http
  POST /products/_search
  {
    "query": {
      "range": {
        "price": {
          "gte": 70,     // greater than or equal to
          "lt": 500      // less than
        }
      }
    }
  }
  ```

  Other operators: `gt` (greater than), `lte` (less than or equal to). For dates, expressions like `now-1d/d` (from yesterday to today) can also be used.

  Searching logs within a time range is very common:

  ```http
  POST /application_logs-*/_search
  {
    "query": {
      "range": {
        "@timestamp": {
          "gte": "2024-05-24T10:00:00.000Z",
          "lte": "2024-05-24T10:05:00.000Z",
          "format": "strict_date_optional_time||epoch_millis"
        }
      }
    }
  }
  ```

* **`exists` Query: Whether a Field Exists**
  Checks if a specific field exists in the document (not null or an empty array).

  ```http
  POST /products/_search
  {
    "query": {
      "exists": {
        "field": "description"
      }
    }
  }
  ```

* **`prefix` Query: Starts with a Specific Prefix**
  Finds values in `keyword` fields that start with a specific prefix.

  ```http
  POST /products/_search
  {
    "query": {
      "prefix": {
        "sku": "SKU"
      }
    }
  }
  ```

* **`wildcard` Query: Pattern Matching with Wildcards**
  Performs pattern matching using `*` (multiple characters) and `?` (single character) wildcards.

  ```http
  POST /products/_search
  {
    "query": {
      "wildcard": {
        "name.keyword": { // Usually used on .keyword field
          "value": "Lap*Pro"
        }
      }
    }
  }
  ```

  **Performance Warning:** `wildcard` and `prefix` queries (especially those starting with `*` or `?`) can be slow. They should be used cautiously.

### 3.2.3 Other Useful Queries

* **`match_all` Query: Get All Documents**
  Retrieves all documents in the index without any filtering.

  ```http
  POST /products/_search
  {
    "query": {
      "match_all": {}
    }
  }
  ```

* **`match_none` Query: Get No Documents**
  Retrieves no documents. Rarely used.

## 3.3 The Art of Combining Queries: The `bool` Query

In real-world scenarios, we usually search by combining multiple conditions. This is where the `bool` (boolean) query comes into play. The `bool` query allows us to combine other query types with logical operators (`AND`, `OR`, `NOT`) and is one of the most fundamental building blocks of Query DSL.

The `bool` query has four main clauses:

* **`must`:** All queries in this clause must match (logical `AND`). Contributes to the score of matching documents.
* **`filter`:** All queries in this clause must match (logical `AND`). However, it operates in the filter context, meaning it's not included in scoring and can be cached. Ideal for performance.
* **`should`:** At least one of the queries in this clause should match (logical `OR`). Contributes to the score of matching documents. If there is no `must` or `filter`, at least one condition in `should` must be met.
  * `minimum_should_match`: Specifies the minimum number of `should` clauses that must match (e.g., `1`, `2`, `"75%"`).
* **`must_not`:** None of the queries in this clause should match (logical `NOT`). Operates in the filter context, not included in scoring.

**Example `bool` query (for Products):**
"Find products that are in the 'Accessories' category (filter), whose name or description contains 'gaming keyboard' (query) AND whose price is less than 100 (filter) BUT do not have 'refurbished' in their tags (must_not)."

```http
POST /products/_search
{
  "query": {
    "bool": {
      "must": [
        {
          "multi_match": {
            "query": "gaming keyboard",
            "fields": ["name", "description"]
          }
        }
      ],
      "filter": [
        { "term": { "category": "Accessories" } },
        { "range": { "price": { "lt": 100 } } }
      ],
      "must_not": [
        { "term": { "tags": "refurbished" } }
      ],
      "should": [
        { "term": { "tags": "new-arrival" } }
      ]
    }
  }
}
```

`bool` queries can also be nested, allowing you to create very complex search logic. It's fair to call it the "master of queries"!

**Example `bool` query (for Logs):**
"Find logs from the 'payment-service' that are at 'ERROR' level AND contain the word 'database' in the message."

```http
POST /application_logs-*/_search
{
  "query": {
    "bool": {
      "filter": [
        { "term": { "service_name": "payment-service" } },
        { "term": { "level": "ERROR" } }
      ],
      "must": [
        { "match": { "message": "database" } }
      ]
    }
  }
}
```

For more examples and details on all these query types and the `bool` query, be sure to check out the [Elasticsearch Query DSL Documentation](https://www.elastic.co/guide/en/elasticsearch/reference/current/query-dsl.html).

## 3.4 Practice Time: Let's Search the `products` Index!

Alright, let's open Kibana Dev Tools and try out the queries we've learned on our `products` index!

1. **Products with "Laptop" in the name and price greater than 1000:**

    ```http
    POST /products/_search
    {
      "query": {
        "bool": {
          "must": [
            { "match": { "name": "Laptop" } }
          ],
          "filter": [
            { "range": { "price": { "gt": 1000 } } }
          ]
        }
      }
    }
    ```

2. **Products in the "Accessories" OR "Monitors" category, that are in stock (stock_quantity > 0):**

    ```http
    POST /products/_search
    {
      "query": {
        "bool": {
          "filter": [
            {
              "terms": { "category": ["Accessories", "Monitors"] }
            },
            {
              "range": { "stock_quantity": { "gt": 0 } }
            }
          ]
        }
      }
    }
    ```

3. **Products with "latest features" in the description BUT without the "gaming" tag:**

    ```http
    POST /products/_search
    {
      "query": {
        "bool": {
          "must": [
            { "match_phrase": { "description": "latest features" } }
          ],
          "must_not": [
            { "term": { "tags": "gaming" } }
          ]
        }
      }
    }
    ```

It's possible to multiply these examples. Try writing different queries by thinking about your own scenarios. The best way to master Query DSL is to practice!

## 3.5 Practice Time: Let's Search the `application_logs` Index!

Now let's do some searches on our log data.

1. **Find all logs at "WARN" level:**

    ```http
    POST /application_logs-*/_search
    {
      "query": {
        "term": {
          "level": "WARN"
        }
      }
    }
    ```

2. **Find "ERROR" logs within a specific time range (e.g., last 1 hour):**

    ```http
    POST /application_logs-*/_search
    {
      "query": {
        "bool": {
          "filter": [
            { "term": { "level": "ERROR" } },
            {
              "range": {
                "@timestamp": {
                  "gte": "now-1h/h",
                  "lte": "now/h"
                }
              }
            }
          ]
        }
      },
      "sort": [ { "@timestamp": "desc" } ]
    }
    ```

3. **Logs containing the word "failed" in the message and generated by "auth-service" or "payment-service":**

    ```http
    POST /application_logs-*/_search
    {
      "query": {
        "bool": {
          "must": [
            { "match": { "message": "failed" } }
          ],
          "filter": [
            { "terms": { "service_name": ["auth-service", "payment-service"] } }
          ]
        }
      }
    }
    ```

When working with log data, filtering by time ranges, log levels, and specific keywords is very common. These examples will give you a starting point.

In the next chapter, we'll explore how to manage search results (paging, sorting) and Elasticsearch's powerful analysis capability: Aggregations.

---
[<- Previous Section: Section 02](Section02.md) | [Next Section: Section 04 ->](Section04.md)
