# Chapter 1: Hello Elasticsearch World!

In this chapter, we'll learn what Elasticsearch is, what problems it solves, its basic architecture, and most importantly, how it performs those famous fast searches. In other words, we'll seek answers to the questions, "What is this tool and how does it search so well?"

## 1.1 Introduction to the Adventure: You, Me, and Elasticsearch

Hello, adventurous developer! Since you've reached these lines, it means you're ready to uncover the secrets of this popular and powerful tool called Elasticsearch. Maybe you've heard its name often, maybe you encountered it in a project and wondered, "What is this?" Or maybe your enthusiasm for learning something new brought you here. Whatever your reason, you're not alone in this adventure!

This booklet was written to help you break the ice with Elasticsearch, get to know it better, and find answers to the question, "Where and how can I use this?" Our goal is to provide a learning experience filled with "Aha! So that's why!" moments, in an understandable language, without getting bogged down in technical details.

So, who are you? Have you had any previous acquaintance with Elasticsearch? What are your expectations from it? Although the answers to these questions are hidden within you, we've tried to design this booklet so that curious developers of all levels can benefit from it. Whether you're taking your first step into Elasticsearch or want to refresh your existing knowledge, we're sure you'll find something for yourself here.

Now, prepare your coffee (or tea, your choice!), settle into a comfortable chair, and get ready for an enjoyable journey into the data-filled world of Elasticsearch. Our first stop: the crucial answers to the question, "Why do we even need something like this?"

## 1.2 "Why Elasticsearch?" Are You Still Using `LIKE`?

Ah, those famous `LIKE '%search_term%'` queries... We've all been there at some point. Searching for a word among millions of rows in a database was no different from looking for a needle in a haystack, right? Especially when the user came up with requests like "it should contain this and not that, but start with this..." This is precisely where traditional relational databases (RDBMS) start to falter a bit.

* **Performance Issues:** As data grows, `LIKE` queries slow down, users get impatient, and systems sound alarms.
* **Lack of Relevance:** `LIKE` only says "exists" or "doesn't exist." It doesn't care much about which is more relevant or more important.
* **Language Support and Flexibility:** Word stems in different languages, synonyms, typos... These are nightmares for `LIKE`.
* **Unstructured Data:** Today, we deal with text-based, unstructured, or semi-structured data like logs, tweets, and product reviews. Analyzing these in an RDBMS isn't very pleasant. Elasticsearch is a perfect fit, especially for data like application logs and server logs, which can reach enormous sizes and need to be searched and analyzed quickly.

Elasticsearch comes to the rescue for these kinds of problems. Think of smart product searches on e-commerce sites, instant queries on log analysis platforms, and anomaly detection systems. Elasticsearch or similar technology lies behind many of them.

## 1.3 Elasticsearch: The Swiss Army Knife of the Data World (and a Bit of NoSQL Talk)

So, what is this Elasticsearch? It's also briefly known as **ES** (for lazy developers).

* **Built on Apache Lucene:** At its heart is Apache Lucene, a powerful search library with years of experience. So, its foundations are solid.
* **Distributed:** It distributes data across multiple servers, increasing performance and making the system harder to crash. The principle of "Don't put all your eggs in one basket" for a single server.
* **RESTful API:** It communicates via HTTP in JSON format. This means you can easily interact with it using your favorite programming language. You can even send queries with `curl`, it's that accessible!
* **Search and Analytics Engine:** It not only searches but is also quite capable of analyzing data, grouping, and extracting statistics.
* **Near Real-Time (NRT):** Data becomes visible in searches very shortly after being added (usually within 1 second). Ideal for those who want it "now!"
* **Schema-Flexible / Not Schemaless!:** You don't have to specify a schema when you add data; ES can infer a schema for you (dynamic mapping). However, in a production environment, creating your own schema (explicit mapping) is strongly recommended. Saying "Let it be messy" can cause headaches later.

**So, is Elasticsearch a NoSQL Database?**

This question comes up frequently. The answer is a bit of a yes and no. I can hear you saying, "What do you mean?" Let's explain:

**What is NoSQL Briefly?** NoSQL, meaning "Not Only SQL," is a broad category of databases that emerged as an alternative to some limitations of traditional relational databases (SQL-based ones). Issues like the rigid schema structure of RDBMSs, difficulties in horizontal scaling, and dependency on specific data models (tables, rows, columns) have popularized NoSQL solutions, especially in scenarios requiring Big Data, high-traffic applications, and flexible data models.

NoSQL databases typically offer one or more of the following features:

* Flexible schemas (or schemalessness)
* Horizontal scalability (increasing capacity by adding more servers)
* High performance and availability
* Different data models (document, key-value, column-family, graph)

**Elasticsearch and its NoSQL Relationship:**

Elasticsearch is fundamentally a **search and analytics engine**. However, because it stores data in a **document-oriented** way (JSON documents), it is very similar to **document databases (document stores)**, which are a type of NoSQL database.

* **Document-Oriented:** It stores data as JSON documents and operates on these documents. This is a similar approach to other document databases like MongoDB.
* **Flexible Schema:** Its dynamic mapping feature offers flexibility in terms of schema, but as we said, explicit mapping is healthier.
* **Horizontal Scalability:** It can scale horizontally thanks to shards.
* **REST API:** It provides an HTTP-based API for data access.

With these features, Elasticsearch can be considered part of the NoSQL ecosystem. In many cases, it can even be used as a primary NoSQL database (especially if search and analytics capabilities are a priority). However, it's important to remember that Elasticsearch's main strength and the area it's optimized for is **search and analysis**. Some transactional guarantees or general-purpose database features offered by a traditional NoSQL document database may not be at the same level in Elasticsearch.

So, yes, Elasticsearch carries many features of a NoSQL database and can be included in that category, but it's something like a "NoSQL document store with search engine superpowers." It's best to position it according to its purpose.

**What Elasticsearch Is Not?**

This is also an important point. Elasticsearch is not a magic wand for every problem. It's not designed to be a primary data source (source of truth), meaning it's not meant to replace a traditional RDBMS. Its ACID guarantees are not as strong as RDBMSs. It's generally used as a layer that takes a copy of the data and optimizes it for search and analysis.

**Where Is This Thing Used?**

* **Full-Text Search:** Smart and fast search on websites, applications, and product catalogs.
* **Log and Metric Analysis:** Server logs, application logs, performance metrics... It's perfect for collecting and analyzing them to identify problems (Observability). This is one of the most common use cases you'll encounter internally!
* **Business Intelligence:** Real-time analysis and reporting on large datasets.
* **Security Analytics (SIEM):** Collecting security events and detecting suspicious activities.
* **Geospatial Data Search:** Location-based searches and analysis on maps.

## **1.4 Basic Architectural Concepts: The Elasticsearch Alphabet**

Before we start talking to Elasticsearch, we need to learn some basic words in its language. Don't worry, it's not rocket science!

* **Document:** The smallest unit of data in Elasticsearch. It's in JSON format. Think of it like a row in an RDBMS.
  
  ```json
  {
      "id": 1,
      "product_name": "Smartphone X1000",
      "price": 799.99,
      "category": "Electronics"
  }
  ```

* **Index:** A collection of documents with similar characteristics. You can think of it like a table in an RDBMS. For example, you might have an index named `products` where all your product documents are stored. Similarly, you can create date-based indices for your application logs, like `application_logs-2024-05-24`.
  * *Note:* There used to be a concept called `_type` within an index, but it's now obsolete (in versions 7.x and later, a single index has a single type, usually referred to as `_doc`). So, don't get confused; it's simpler now.
* **Node:** A single running Elasticsearch server. An ES program running on a computer is a node.
  * *Node Types:* Each node can have different roles within the cluster: `master-eligible` (manages the cluster), `data` (holds data), `ingest` (transforms data before indexing), `coordinating` (receives queries, distributes them, collects results), `machine learning` (for special ML jobs). Initially, they can all be combined, but in large systems, these roles are separated.
* **Cluster:** A structure formed by one or more nodes coming together. These nodes work together, sharing data and workload. A standalone node is also considered a cluster (single-node cluster).
* **Shard:** When an index becomes too large, it needs to be divided into smaller, manageable pieces. These pieces are called shards. Shards allow Elasticsearch to scale data horizontally and parallelize operations.
  * **Primary Shard:** Every document belongs to a primary shard. The number of primary shards is determined when an index is created (and cannot be changed later, be careful!).
* **Replica:** Copies of primary shards. They have two main purposes:
    1. **High Availability:** If a node crashes and the primary shards on that node become inaccessible, replica shards take over, preventing data loss.
    2. **Increase Read Performance:** Search queries can be performed on both primary and replica shards, which increases read capacity.
  * A replica shard is never located on the same node as its primary shard. Clever, isn't it?

**Time to Get Your Hands Dirty! (First Encounter with Kibana Dev Tools)**

Theory is good, but it's nothing without practice. We assume you've installed Elasticsearch and Kibana (with Docker). In the Kibana interface, you'll see a section called "Management" > "Dev Tools." This is our playground where we'll talk to Elasticsearch and send commands.

You can paste the following commands into Dev Tools and run them by clicking the green "play" button:

1. **Check Cluster Health:**

    ```http
    GET /_cluster/health
    ```

    This command gives you information about the general status of your cluster (`status`: green, yellow, red). Green means everything is fine. Yellow means primary shards are okay, but some replicas may not have been assigned yet (normal for a single-node cluster). Red means danger bells are ringing; some primary shards are unreachable!

2. **List Nodes:**

    ```http
    GET /_cat/nodes?v&h=ip,name,heap.percent,load_1m
    ```

    `?v` provides more detailed (verbose) output. With `&h=`, you can select which columns you want to see.

3. **List Existing Indices:**

    ```http
    GET /_cat/indices?v
    ```

    Initially, you might see system indices used by Kibana itself, like `.kibana`.

That's it! You've made your first contact with Elasticsearch. Now, if someone asks you, "What was a node, what does a shard do?" you have the answer ready.

## 1.5 How Does Search Work? The Magic Behind the Curtain

What's the secret to Elasticsearch's famous speed and relevance? Why does it provide results that are orders of magnitude more performant and intelligent than `LIKE` queries? The answer lies in an ingenious data structure called an **inverted index** and the **analysis** process that creates this structure.

### 1.5.1 The Analysis Process: Making Sense of Text

When a document is added to Elasticsearch (indexing), fields of type `text` go through an analysis process. This process breaks the text into small pieces (tokens) and processes these tokens to make the text searchable. The analysis process usually consists of three steps:

1. **Character Filters:** They clean up the raw text before it's tokenized. For example, they can remove HTML tags (like `<b>`, `<i>`) or convert the "&" character to the word "and".
2. **Tokenizer:** It breaks the cleaned text into individual words (tokens). Different tokenizers work according to different rules.
    * **Standard Tokenizer:** A general-purpose tokenizer that works well for most languages, splitting by spaces and punctuation.
    * **Whitespace Tokenizer:** Splits only by whitespace.
    * **Pattern Tokenizer:** Splits according to a specific regex pattern. Can be used to extract specific patterns from log lines.
    * **Language-Specific Tokenizers:** (e.g., `turkish` tokenizer) Produce better tokens by considering language-specific rules (e.g., suffixes in Turkish).
3. **Token Filters:** They perform additional operations on the tokens output by the tokenizer.
    * **Lowercase Token Filter:** Converts all tokens to lowercase (so "Apple" and "apple" can be searched in the same way).
    * **Stop Token Filter:** Removes "stop words" like "the", "a", "is", "bir", "ve", which are common but don't add much meaning to a search.
    * **Stemmer Token Filter:** Reduces words to their root form (like "running", "ran", "runs" to the root "run"). This way, when you search for "koşu" (run/running), results containing "koşuyor" (is running) can also appear. There are special stemmers for Turkish, like `turkish_stemmer`.
    * **Synonym Token Filter:** Adds synonymous words (e.g., so that results containing "notebook" also appear when searching for "laptop").

As a result of this analysis process, a sentence like "The quick brown fox jumps over the lazy dog" might be converted into a token list like `["quick", "brown", "fox", "jump", "over", "lazy", "dog"]`.
For more information on this process and its components, you can refer to the [Elasticsearch Analysis Documentation](https://www.elastic.co/guide/en/elasticsearch/reference/current/analysis.html).

### 1.5.2 Inverted Index: The Key to Fast Searching

After the analysis process is complete, Elasticsearch uses these tokens to create an **inverted index**. An inverted index is like the index in a book: it lists words and indicates in which documents each word appears and at what positions within those documents.

A sample inverted index (highly simplified):

| Token   | Document IDs (and Positions)      |
| :------ | :-------------------------------- |
| quick   | Doc1 (position 2)                 |
| brown   | Doc1 (position 3), Doc2 (position 1) |
| error   | LogDoc5 (position 7), LogDoc12 (position 3) |
| fox     | Doc1 (position 4)                 |
| ...     | ...                               |

When a user searches for "brown fox", Elasticsearch:

1. Looks at the list of documents containing the token "brown" (Doc1, Doc2).
2. Looks at the list of documents containing the token "fox" (Doc1).
3. Compares these two lists and finds the documents that contain both tokens (Doc1 in this example).
4. If a positional search like `match_phrase` is performed, it also checks the positions of the tokens within the document to see if the word "fox" immediately follows the word "brown".

Thanks to this structure, searching among millions of documents is as fast as looking directly at the lists of relevant tokens, instead of scanning all documents one by one. This is the secret to Elasticsearch's speed, unlike the slowness of `LIKE '%...%'` queries!

### 1.5.3 The Role of Shards and Replicas in Search

So, how do the shards and replicas we mentioned earlier affect this search process?

* **Shards and Parallel Search:** When an index is divided into multiple shards, and a search query arrives, Elasticsearch sends this query in parallel to all primary and replica shards of the index. Each shard performs the search on its own data and sends its results back to the coordinating node. The coordinating node merges these results, sorts them, and presents them to the user. This parallelization greatly increases search speed, especially for large indices. The more shards (up to a certain limit), the more parallel processing power!
* **Replicas and Search Capacity/Availability:** Replica shards not only provide data redundancy (high availability) but can also serve search queries. This means when a query arrives, it can be sent to both the primary shard and its replicas. This distributes the read (search) load, allowing the system to respond to more concurrent search requests (increasing search capacity). Also, if a node crashes and its primary shards become unavailable, searches can continue uninterrupted thanks to the replicas.

In short, the analysis process breaks text into meaningful tokens, the inverted index enables lightning-fast searches on these tokens, shards parallelize the search, and replicas both increase search capacity and ensure system resilience. This perfect harmony is what makes Elasticsearch such a powerful search engine!

Now that we understand these basic search mechanisms, in Chapter 2, we can examine in more detail how to add our data to Elasticsearch and how to manage the structure (mapping) of this data. Because correct mapping forms the foundation of the analysis process and, therefore, the search quality!

---
[Next Section: Section 02 ->](Section02.md)
