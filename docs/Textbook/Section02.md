# Chapter 2: Build Your Data Kingdom: Indexing and Mapping

We've learned the basic building blocks of Elasticsearch and the fundamentals of its search mechanism. Now it's time to fill these structures, meaning, to add data! But we won't just throw data in randomly; we'll also see how to manage it in an organized way, in a manner that Elasticsearch can understand and search effectively. In this chapter, we'll see examples for different data types like products and logs. In short, we'll lay the foundations of our own data kingdom.

## 2.1 Dancing with Documents: Adding, Reading, Updating, and Deleting Data (CRUD)

Like any database system, Elasticsearch has basic data operations: Create, Read, Update, Delete. The famous **CRUD**!

### 2.1.1 Adding Documents (Indexing / Create)

We generally use `PUT` or `POST` HTTP methods to add a document to Elasticsearch.

#### Adding with Your Own ID (`PUT`):

If your document has a unique ID (like a primary key from a database) and you want to use it in Elasticsearch, you use `PUT`.

```http
PUT /products/_doc/1
{
  "product_name": "Super Fast Laptop Pro",
  "category": "Computers",
  "price": 1299.99,
  "in_stock": true,
  "features": ["16GB RAM", "512GB SSD", "Next-Gen CPU"]
}
```

If the `/products` index doesn't exist, Elasticsearch will automatically create it with default settings (thanks to dynamic mapping). If a document with ID `1` already exists, this command will update that document (overwrite it). If you want to create it only if the ID doesn't exist, you can use the `_create` endpoint: `PUT /products/_create/1`.

#### Let Elasticsearch Auto-Generate ID (`POST`):

If you don't want to deal with document IDs, let Elasticsearch generate a unique ID for you. This is a common method, especially for continuously flowing data like logs.

```http
POST /application_logs/_doc
{
  "@timestamp": "2024-05-24T14:33:15.234Z",
  "level": "ERROR",
  "service_name": "payment-service",
  "host_ip": "10.0.1.25",
  "message": "Failed to process payment for order 12345: Connection timed out"
}
```

Elasticsearch will generate an automatic ID for this document and return it in the response.

### 2.1.2 Reading Documents (Get)

We use the `GET` method to retrieve a document by its ID.

```http
GET /products/_doc/1
```

The response will include the document itself (in the `_source` field) and metadata like `_index`, `_id`, `_version`.

### 2.1.3 Updating Documents (Update)

You have a few ways to update an existing document.

* **Complete Overwrite (`PUT`):** As mentioned above, if you `PUT` with the same ID, the document will be completely replaced with the new content.
* **Partial Update (`POST` with `_update`):** If you only want to update specific fields, the `_update` endpoint is more sensible.

  ```http
  POST /products/_update/1
  {
    "doc": {
      "price": 1249.99,
      "in_stock": true
    }
  }
  ```

  This command only updates the price and stock status, leaving other fields untouched. You can also ensure the document is created if it doesn't exist using the `doc_as_upsert` parameter, or perform more complex updates using scripts.

### 2.1.4 Deleting Documents (Delete)

We use the `DELETE` method to delete a document.

```http
DELETE /products/_doc/1
```

And poof! Our document is gone (actually, it's not deleted immediately; it's marked as deleted and then truly cleaned up later, but let's not get into that detail for now).

#### What Are Meta-Fields For?

Every document has some special fields managed by Elasticsearch:

* `_index`: The name of the index where the document resides.
* `_id`: The unique ID of the document.
* `_version`: The version number of the document. It increments with each update. Can be used for optimistic locking.
* `_source`: The original content of the document, i.e., the JSON data you added.
* `_score`: A score indicating how relevant the document is to a search query. (Only comes in search results).

For more information and examples on these basic document operations, you can refer to the [Elasticsearch Document APIs Reference](https://www.elastic.co/guide/en/elasticsearch/reference/current/docs.html).

## 2.2 Bulk Transport: The Art of Efficiency with the `_bulk` API

Adding, updating, or deleting documents one by one is fine for small tasks, but when dealing with hundreds, thousands, or even millions of documents (e.g., hundreds of log lines per second!), making a separate HTTP request for each would be a disaster. Both network traffic would increase, and Elasticsearch would get tired. I can hear you saying, "There must be a better way." Yes, there is: the `_bulk` API!

The `_bulk` API allows you to send multiple operations (index, create, update, delete) in a single HTTP request. This significantly improves performance.

The data format sent to the `_bulk` endpoint is slightly different. Each operation requires two lines:

1. **Action and Metadata Line:** Specifies which operation to perform (`index`, `create`, `update`, `delete`) and on which index/ID to operate.
2. **Source Line (Optional):** For `index`, `create`, and `update` operations, it contains the document's content or update details. This line is not needed for `delete`.

**Example `_bulk` request (for both products and logs):**

```http
POST /_bulk
{ "index" : { "_index" : "products", "_id" : "SKU004" } }
{ "product_name" : "Ergonomic Office Chair", "category" : "Furniture", "price": 299.00, "in_stock": true }
{ "index" : { "_index" : "application_logs-2024-05-24" } }
{ "@timestamp": "2024-05-24T15:01:00.000Z", "level": "INFO", "service_name": "user-service", "message": "User logged in: user@example.com" }
{ "index" : { "_index" : "application_logs-2024-05-24" } }
{ "@timestamp": "2024-05-24T15:01:05.123Z", "level": "WARN", "service_name": "user-service", "message": "Login attempt failed for unknown_user" }
{ "delete" : { "_index" : "products", "_id" : "old_product_id" } }
```

**Caution:** The body of a `_bulk` request is not standard JSON. Each line is a JSON object, but there are no commas between lines, and each JSON object must be on a new line (`\n`). Therefore, some JSON formatters might break this structure. Kibana Dev Tools understands this format correctly.

The result of each operation is returned separately in the `_bulk` response. This way, you can see which operation succeeded and which failed. The `_bulk` API is a lifesaver when transferring data in a production environment! For more details and best practices, you can review the [Elasticsearch Bulk API Documentation](https://www.elastic.co/guide/en/elasticsearch/reference/current/docs-bulk.html).

## 2.3 Mapping: Your Data's ID Card, Elasticsearch's Roadmap

When we send data to Elasticsearch, the set of rules that determines how that data will be stored, analyzed, and searched is called **mapping**. In other words, every index has a mapping, and this mapping defines the data types and other properties of the fields in the documents within that index. Correct mapping is crucial for search quality and performance because mapping directly affects the **analysis process** we talked about in [Section 1.5: Analyzers, Tokenizers, and Filters: The Secret Sauce of Search](../Textbook/Section01.md#15-analyzers-tokenizers-and-filters-the-secret-sauce-of-search).

### 2.3.1 Dynamic Mapping: "You Send It, I'll Figure It Out" Mode

If you haven't specified a custom mapping for an index when you add the first document to it, Elasticsearch steps in and tries to guess the data types by looking at the fields in the document. This is called **dynamic mapping**.

* **Advantages:**
  * Quick start: You can start sending data immediately without worrying about mapping.
  * Flexibility: When new fields are added, Elasticsearch automatically includes them in the mapping.
* **Disadvantages (and Why It's Not Loved in Production):**
  * **Incorrect Type Inferences:** Elasticsearch may not always guess the correct type. For example, it might interpret a string value like "123" as a number (`long`) or vice versa. This can cause problems in searches and analyses later on. The "I thought it was a string, turns out it was a number!" situation. This is particularly critical for the `text` vs. `keyword` distinction.
  * **Unnecessary Fields and Analyses:** By default, `text` type fields are both analyzed for full-text search and stored as `keyword` for exact matching (multi-fields). This may not always be necessary and can take up disk space and slow down indexing. Choosing the wrong analyzer can also negatively impact search results.
  * **"Mapping Explosion":** Uncontrolled addition of too many new fields (especially if dynamic field names are used) can severely degrade cluster performance.

In short, while dynamic mapping can speed things up during development, **explicit mapping** (i.e., defining the schema yourself) is generally recommended for production environments. To control the behavior of dynamic mapping, you can refer to the [Dynamic Mapping Documentation](https://www.elastic.co/guide/en/elasticsearch/reference/current/dynamic-mapping.html).

### 2.3.2 Explicit Mapping: "Let's Define the Rules, So I Don't Get a Headache Later" Mode

With explicit mapping, you define the data type and other properties of each field when an index is created (or after it's created, but be careful, the type of existing fields usually cannot be changed!).

#### Example: Mapping for Customer Data

```http
PUT /customers
{
  "mappings": {
    "properties": {
      "customer_id": { "type": "keyword" },
      "full_name": {
        "type": "text",
        "analyzer": "standard",
        "fields": {
          "keyword": { "type": "keyword", "ignore_above": 256 }
        }
      },
      "email": { "type": "keyword" },
      "birth_date": { "type": "date", "format": "yyyy-MM-dd" },
      "order_count": { "type": "integer" },
      "last_login_date": { "type": "date" },
      "is_active": { "type": "boolean" },
      "address": {
        "type": "object",
        "properties": {
          "city": { "type": "keyword" },
          "postal_code": { "type": "keyword" }
        }
      }
    }
  }
}
```

#### Example: Mapping for Application Logs

Log data is often time-series data, and specific fields (like log level, service name) are defined as `keyword` for filtering and aggregation, while the log message itself is defined as `text`. The `@timestamp` field should always be of type `date`.

```http
PUT /_index_template/application_logs_template
{
  "index_patterns": ["application_logs-*"],
  "template": {
    "mappings": {
      "properties": {
        "@timestamp": { "type": "date" },
        "level": { "type": "keyword" },
        "service_name": { "type": "keyword" },
        "host_ip": { "type": "ip" },
        "thread_name": { "type": "keyword" },
        "logger_name": { "type": "keyword" },
        "message": {
          "type": "text",
          "analyzer": "standard"
        },
        "stack_trace": { "type": "text" },
        "http_status_code": { "type": "integer" },
        "response_time_ms": { "type": "long" }
      }
    }
  }
}
```

The example above is an **index template**. When a new index starting with `application_logs-` (e.g., `application_logs-2024-05-25`) is created, this mapping is automatically applied. This is very useful for logs that are created daily or weekly. You can find more information about index templates in the [Index Templates Documentation](https://www.elastic.co/guide/en/elasticsearch/reference/current/index-templates.html).

#### Most Commonly Used Data Types and Their Purposes

* `text`: For texts that will be full-text searched (product description, blog post, log messages). Fields of this type go through an **analyzer**.
* `keyword`: For texts that will be used for exact matching, sorting, and aggregation (category name, tags, status codes, email address, IDs, log level, service name).
* **Numeric Types:** `long`, `integer`, `short`, `byte`, `double`, `float`, `half_float`, `scaled_float`.
* `date`: Date and time information. The `@timestamp` field is essential for logs.
* `boolean`: `true` or `false`.
* `ip`: For IPv4 and IPv6 addresses. Used for source/destination IPs in logs.
* `geo_point`, `geo_shape`: Geographic data.
* `object`: Nested JSON objects.
* `nested`: For arrays of objects.

In this section, we've covered the basics of mapping. For much more detail and advanced options on data types, analyzers, and mapping parameters, you should consult the [Mapping section in the Elasticsearch Official Documentation](https://www.elastic.co/guide/en/elasticsearch/reference/current/mapping.html) and the [Field datatypes page](https://www.elastic.co/guide/en/elasticsearch/reference/current/mapping-types.html) to deepen your understanding.

#### Important Mapping Parameters

When creating a mapping, beyond specifying a data type (`type`) for each field within `properties`, there are many powerful parameters that control the behavior of that field. These parameters allow you to customize how Elasticsearch will index, analyze, and search your data. Let's look at the ones you'll encounter most often and that will save your life.

* **`type`**: This is the most fundamental parameter. It specifies what type of data the field will hold (`text`, `keyword`, `date`, `long`, `boolean`, `ip`, etc.). Choosing the correct type is the foundation for the searches and analyses you will perform. For example, you might want to perform a free-text search on a product name and also filter by exact match. This is where other parameters come into play.

* **`analyzer`**: Used for `text` type fields, it defines the analysis chain that determines how the text will be tokenized, filtered, and indexed. For example, the `standard` analyzer splits text into words and converts them to lowercase, while the `english` analyzer performs additional operations specific to English, like "stemming" (finding word roots, "running" -> "run"). The `standard` analyzer is usually sufficient for analyzing log messages.

* **`search_analyzer`**: Normally, a field is both indexed and searched using the analyzer specified by `analyzer`. However, sometimes you might want to use a different analyzer at search time. For example, in autocomplete scenarios, using a different analysis process (e.g., `edge_ngram_analyzer`) when indexing data versus when the user is typing and searching can yield more effective results.

* **`fields`**: This is where the magic begins! This parameter allows you to index the same source data in different ways. The most common use is to index a `text` field as a `keyword` as well.

    ```json
    "full_name": {
      "type": "text",
      "analyzer": "standard",
      "fields": {
        "keyword": { "type": "keyword" }
      }
    }
    ```

    This way, you can search for "John Doe" in the `full_name` field, and also use the `full_name.keyword` field for sorting or filtering records that exactly match the name "John Doe".

* **`dynamic`**: This parameter determines what Elasticsearch does when a new field, not defined in the mapping, arrives. It can have three values:
  * `true`: (Default) Adds the new field to the mapping with a guessed type.
  * `false`: Does not add the new field to the mapping, meaning this field becomes unsearchable. However, its data continues to be stored in `_source`.
  * `strict`: The "Don't you dare without asking me!" mode. If a field not in the mapping arrives, it rejects the entire document and throws an error. In a production environment, using `strict` or `false` is good practice to prevent unexpected fields from "polluting" the mapping.

* **`enabled`**: This is the way to say, "Don't even bother indexing this field." A field set to `enabled: false` is not searchable and does not take up any space as indexed data on disk. It is only stored in `_source`. You might use this for data that you only want to return in an API response but will never be a search criterion.

* **`format`**: Used for `date` type fields. It allows you to specify the format of the incoming date data. Although Elasticsearch understands many standard formats, you can specify your custom formats here: `"format": "yyyy-MM-dd HH:mm:ss||yyyy-MM-dd||epoch_millis"`. You can define multiple formats with `||`.

* **`ignore_above`**: A lifesaver, especially for `keyword` fields. It prevents the indexing of strings longer than the specified number of characters. This prevents unexpectedly long data, such as user-entered tags or some log lines, from causing Elasticsearch to error out (and stop indexing). For example, an `"ignore_above": 256` setting ensures that tags longer than 256 characters are not indexed, preventing future headaches.

For all these mapping parameters and more, the [Elasticsearch Mapping Parameters Documentation](https://www.elastic.co/guide/en/elasticsearch/reference/current/mapping-params.html) is the best place to refer to.

Mapping is vitally important for Elasticsearch's performance and accuracy. Don't say, "Who has time for this now"; your future self will thank you!

## 2.4 Practice Time: Let's Set Up Our `products` Index!

Now, to reinforce what we've learned, let's create our own `products` index in Kibana Dev Tools, give it an explicit mapping, and add a few products.

1. **Creating the `products` Index with Explicit Mapping:**

    ```http
    PUT /products
    {
      "mappings": {
        "properties": {
          "sku": { "type": "keyword" },
          "name": {
            "type": "text",
            "analyzer": "english",
            "fields": {
              "keyword": { "type": "keyword" }
            }
          },
          "description": { "type": "text", "analyzer": "english" },
          "price": { "type": "scaled_float", "scaling_factor": 100 },
          "stock_quantity": { "type": "integer" },
          "category": { "type": "keyword" },
          "tags": { "type": "keyword" },
          "created_date": { "type": "date" },
          "is_active": { "type": "boolean" }
        }
      }
    }
    ```

2. **Adding a Few Products with the `_bulk` API:**

    ```http
    POST /products/_bulk
    { "index": { "_id": "SKU001" } }
    { "sku": "SKU001", "name": "Awesome Laptop Model Z", "description": "A powerful laptop with the latest features.", "price": 1399.99, "stock_quantity": 15, "category": "Computers", "tags": ["laptop", "new-gen", "gaming"], "created_date": "2024-05-20T10:00:00Z", "is_active": true }
    { "index": { "_id": "SKU002" } }
    { "sku": "SKU002", "name": "Super Silent Mechanical Keyboard", "description": "RGB backlit, long-lasting mechanical keyboard.", "price": 75.50, "stock_quantity": 45, "category": "Accessories", "tags": ["keyboard", "mechanical", "rgb"], "created_date": "2024-05-21T14:30:00Z", "is_active": true }
    { "index": { "_id": "SKU003" } }
    { "sku": "SKU003", "name": "Wide Screen Monitor", "description": "Ideal for professional use, 32 inch 4K monitor.", "price": 450.00, "stock_quantity": 0, "category": "Monitors", "tags": ["monitor", "4k", "professional"], "created_date": "2024-05-15T09:15:00Z", "is_active": false }
    ```

## 2.5 Working with Log Data: A Practical Scenario

Now let's create an index for a typical application log scenario and add a few sample log records. First, let's create the `application_logs_template` we defined above (in Section 2.3.2), if you haven't already.

1. **Creating the `application_logs_template` (If Not Done Before):**

    ```http
    PUT /_index_template/application_logs_template
    {
      "index_patterns": ["application_logs-*"],
      "template": {
        "mappings": {
          "properties": {
            "@timestamp": { "type": "date" },
            "level": { "type": "keyword" },
            "service_name": { "type": "keyword" },
            "host_ip": { "type": "ip" },
            "thread_name": { "type": "keyword" },
            "logger_name": { "type": "keyword" },
            "message": {
              "type": "text",
              "analyzer": "standard"
            },
            "stack_trace": { "type": "text" },
            "http_status_code": { "type": "integer" },
            "response_time_ms": { "type": "long" }
          }
        }
      }
    }
    ```

    *Note: In current Elasticsearch versions, `PUT /_index_template/application_logs_template` is used instead of `PUT /_template/application_logs_template`, and it's placed under the `template` key. The syntax might be different for older versions.*

2. **Adding Sample Log Records with the `_bulk` API (e.g., to the `application_logs-2024-05-24` index):**

    ```http
    POST /_bulk
    { "index" : { "_index" : "application_logs-2024-05-24" } }
    { "@timestamp": "2024-05-24T10:00:00.123Z", "level": "INFO", "service_name": "auth-service", "host_ip": "192.168.1.10", "message": "User 'alice' logged in successfully." }
    { "index" : { "_index" : "application_logs-2024-05-24" } }
    { "@timestamp": "2024-05-24T10:01:30.456Z", "level": "WARN", "service_name": "product-catalog", "host_ip": "192.168.1.11", "message": "Product SKU00X not found, returning 404." }
    { "index" : { "_index" : "application_logs-2024-05-24" } }
    { "@timestamp": "2024-05-24T10:02:15.789Z", "level": "ERROR", "service_name": "payment-service", "host_ip": "192.168.1.12", "message": "Database connection failed: timeout", "stack_trace": "java.net.SocketTimeoutException: connect timed out\n  at java.net.PlainSocketImpl.socketConnect(Native Method)\n  ..." }
    { "index" : { "_index" : "application_logs-2024-05-24" } }
    { "@timestamp": "2024-05-24T10:03:00.000Z", "level": "INFO", "service_name": "order-service", "host_ip": "192.168.1.10", "message": "Order ORD001 processed successfully.", "response_time_ms": 150 }
    ```

    These logs will be indexed according to the mapping we defined in `application_logs_template`.

Congratulations! You now know how to add, update, delete both product and log data in Elasticsearch, and most importantly, how to define the structure of this data. This has built a very solid foundation for our search and analysis adventures. In the next chapter, we'll learn how to query this data, i.e., the intricacies of the art of searching.

---

[<- Previous Section: Section 01](Section01.md) | [Next Section: Section 03 ->](Section03.md)
