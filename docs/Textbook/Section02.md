# Bölüm 2: Veri Krallığınızı Kurun: Indexing ve Mapping

Elasticsearch'ün temel yapı taşlarını ve arama mekanizmasının temellerini öğrendik. Şimdi sıra geldi bu yapıların içini doldurmaya, yani veri eklemeye! Ama veriyi rastgele atmayacağız, onu bir düzen içinde, Elasticsearch'ün anlayacağı ve etkili bir şekilde arayabileceği şekilde nasıl yöneteceğimizi de göreceğiz. Bu bölümde hem ürünler hem de loglar gibi farklı veri türleri için örnekler göreceğiz. Kısacası, kendi veri krallığımızın temellerini atacağız.

## 2.1 Dokümanlarla Dans: Veri Ekleme, Okuma, Güncelleme ve Silme (CRUD)

Her veritabanı sisteminde olduğu gibi Elasticsearch'te de temel veri operasyonları var: Oluştur (Create), Oku (Read), Güncelle (Update), Sil (Delete). Yani meşhur **CRUD**!

### 2.1.1 Doküman Ekleme (Indexing / Create)

Bir dokümanı Elasticsearch'e eklemek için genellikle `PUT` veya `POST` HTTP metodlarını kullanırız.

#### Kendi ID'ni Belirleyerek Ekleme (`PUT`):

Eğer dokümanının benzersiz bir ID'si varsa (mesela veritabanındaki primary key) ve bunu Elasticsearch'te de kullanmak istiyorsan `PUT` kullanırsın.

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

Eğer `/products` index'i yoksa, Elasticsearch onu varsayılan ayarlarla otomatik olarak oluşturur (dynamic mapping sayesinde). Eğer `1` ID'li bir doküman zaten varsa, bu komut o dokümanı günceller (üzerine yazar). Sadece ID yoksa oluşturulsun istiyorsan `_create` endpoint'ini kullanabilirsin: `PUT /products/_create/1`.

#### Elasticsearch Otomatik ID Oluştursun (`POST`):

Eğer doküman ID'siyle uğraşmak istemiyorsan, bırak Elasticsearch senin için benzersiz bir ID oluştursun. Bu, özellikle loglar gibi sürekli akan veriler için yaygın bir yöntemdir.

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

Elasticsearch bu doküman için otomatik bir ID üretip sana cevapta dönecektir.

### 2.1.2 Doküman Okuma (Get)

Bir dokümanı ID'si ile çekmek için `GET` metodunu kullanırız.

```http
GET /products/_doc/1
```

Cevapta dokümanın kendisi (`_source` alanı içinde) ve `_index`, `_id`, `_version` gibi meta bilgiler gelir.

### 2.1.3 Doküman Güncelleme (Update)

Mevcut bir dokümanı güncellemek için birkaç yolun var.

* **Tamamen Üzerine Yazma (`PUT`):** Yukarıda bahsettiğimiz gibi, aynı ID ile `PUT` yaparsan doküman tamamen yeni içerikle değiştirilir.
* **Kısmi Güncelleme (`POST` ile `_update`):** Sadece belirli alanları güncellemek istiyorsan `_update` endpoint'i daha mantıklıdır.

  ```http
  POST /products/_update/1
  {
    "doc": {
      "price": 1249.99,
      "in_stock": true
    }
  }
  ```

  Bu komut sadece fiyat ve stok durumunu günceller, diğer alanlara dokunmaz. `doc_as_upsert` parametresi ile doküman yoksa oluşturulmasını da sağlayabilirsin ya da script kullanarak daha karmaşık güncellemeler yapabilirsin.

### 2.1.4 Doküman Silme (Delete)

Bir dokümanı silmek için `DELETE` metodunu kullanırız.

```http
DELETE /products/_doc/1
```

Ve puf! Dokümanımız artık yok (aslında hemen silinmez, silindi olarak işaretlenir ve daha sonra gerçekten temizlenir, ama o detaya şimdilik girmeyelim).

#### Meta Alanlar Ne İşe Yarar?

Her dokümanda Elasticsearch tarafından yönetilen bazı özel alanlar bulunur:

* `_index`: Dokümanın bulunduğu index'in adı.
* `_id`: Dokümanın benzersiz ID'si.
* `_version`: Dokümanın versiyon numarası. Her güncellemede artar. Optimistic locking için kullanılabilir.
* `_source`: Dokümanın asıl içeriği, yani senin eklediğin JSON verisi.
* `_score`: Bir arama sorgusu sonucunda dokümanın sorguyla ne kadar alakalı olduğunu gösteren skor. (Sadece arama sonuçlarında gelir).

Bu temel doküman operasyonları hakkında daha fazla bilgi ve örnek için [Elasticsearch Doküman API'leri Referansı'na](https://www.elastic.co/guide/en/elasticsearch/reference/current/docs.html) göz atabilirsiniz.

## 2.2 Toplu Taşıma: `_bulk` API'si ile Verimlilik Sanatı

Tek tek doküman eklemek, güncellemek veya silmek küçük işler için tamam ama yüzlerce, binlerce, hatta milyonlarca dokümanla uğraşırken (örneğin, saniyede yüzlerce log satırı!) her biri için ayrı HTTP isteği yapmak tam bir felaket olurdu. Hem ağ trafiği artar hem de Elasticsearch yorulur. "Bunun daha iyi bir yolu olmalı" dediğini duyar gibiyim. Evet, var: `_bulk` API'si!

`_bulk` API'si sayesinde birden fazla operasyonu (index, create, update, delete) tek bir HTTP isteği içinde gönderebilirsin. Bu, performansı ciddi şekilde artırır.

`_bulk` endpoint'ine gönderilen veri formatı biraz farklıdır. Her operasyon için iki satır gerekir:

1. **Action ve Metadata Satırı:** Hangi operasyonun yapılacağını (`index`, `create`, `update`, `delete`) ve hangi index/ID üzerinde çalışılacağını belirtir.
2. **Kaynak Satırı (Opsiyonel):** `index`, `create` ve `update` operasyonları için dokümanın içeriğini veya güncelleme detaylarını içerir. `delete` için bu satır gerekmez.

**Örnek bir `_bulk` isteği (hem ürün hem log):**

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

**Dikkat:** `_bulk` isteğinin gövdesi standart JSON değildir. Her satır bir JSON objesidir ama satırlar arasında virgül yoktur ve her JSON objesi yeni bir satırda (`\n`) olmalıdır. Bu yüzden bazı JSON formatlayıcılar bu yapıyı bozabilir. Kibana Dev Tools bu formatı doğru şekilde anlar.

Her operasyonun sonucu `_bulk` cevabında ayrı ayrı döner. Böylece hangi operasyon başarılı oldu, hangisi hata verdi görebilirsin. Production ortamında veri aktarımı yaparken `_bulk` API'si candır! Daha fazla detay ve en iyi pratikler için [Elasticsearch Bulk API Dokümantasyonu'nu](https://www.elastic.co/guide/en/elasticsearch/reference/current/docs-bulk.html) inceleyebilirsiniz.

## 2.3 Mapping: Verinizin Kimlik Kartı, Elasticsearch'ün Yol Haritası

Elasticsearch'e veri attığımızda, o verinin nasıl saklanacağını, nasıl analiz edileceğini ve nasıl aranacağını belirleyen kurallar bütününe **mapping** denir. Yani, her index'in bir mapping'i vardır ve bu mapping, index'teki dokümanların alanlarının (fields) veri tiplerini ve diğer özelliklerini tanımlar. Doğru bir mapping, arama kalitesi ve performansı için hayati önem taşır. Çünkü mapping, Bölüm 1.5'te bahsettiğimiz **analiz sürecini** doğrudan etkiler.

### 2.3.1 Dynamic Mapping: "Sen At, Ben Anlarım" Modu

Eğer bir index'e ilk dokümanı eklediğinde o index için özel bir mapping belirtmemişsen, Elasticsearch devreye girer ve dokümandaki alanlara bakarak veri tiplerini tahmin etmeye çalışır. Buna **dynamic mapping** denir.

* **Avantajları:**
  * Hızlı başlangıç: Hemen veri atmaya başlayabilirsin, mapping ile uğraşmana gerek kalmaz.
  * Esneklik: Yeni alanlar eklendiğinde Elasticsearch bunları otomatik olarak mapping'e dahil eder.
* **Dezavantajları (ve Neden Production'da Pek Sevilmez):**
  * **Yanlış Tip Çıkarımları:** Elasticsearch her zaman doğru tipi tahmin edemeyebilir. Örneğin, "123" gibi bir string değeri sayı (`long`) olarak algılayabilir veya tam tersi. Bu da ileride arama ve analizlerde sorun çıkarır. "Ben onu string sanmıştım, meğer numaraymış!" durumu. Bu durum, özellikle `text` ve `keyword` ayrımında kritik olabilir.
  * **Gereksiz Alanlar ve Analizler:** `text` tipindeki alanlar için varsayılan olarak hem full-text arama için analiz edilir hem de `keyword` olarak birebir eşleşme için saklanır (multi-fields). Bu her zaman gerekmeyebilir ve diskte yer kaplar, indexlemeyi yavaşlatır. Yanlış analizci seçimi de arama sonuçlarını olumsuz etkileyebilir.
  * **"Mapping Explosion":** Kontrolsüzce çok fazla sayıda yeni alan eklenmesi (özellikle dinamik alan adları kullanılıyorsa) cluster'ın performansını ciddi şekilde düşürebilir.

Kısacası, dynamic mapping geliştirme aşamasında işleri hızlandırsa da, production ortamı için genellikle **explicit mapping** (yani şemayı kendin tanımlaman) önerilir. Dynamic mapping'in davranışını kontrol etmek için [Dynamic Mapping Dokümantasyonu'na](https://www.elastic.co/guide/en/elasticsearch/reference/current/dynamic-mapping.html) bakabilirsiniz.

### 2.3.2 Explicit Mapping: "Kurallar Belli Olsun, Sonra Başım Ağrımasın" Modu

Explicit mapping ile bir index oluşturulurken (veya oluşturulduktan sonra, ama dikkat, mevcut alanların tipi genellikle değiştirilemez!) her alanın veri tipini ve diğer özelliklerini sen belirlersin.

#### Örnek: Müşteri Verisi İçin Mapping

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

#### Örnek: Uygulama Logları İçin Mapping

Log verileri genellikle zaman serisi verileridir ve belirli alanlar (log seviyesi, servis adı gibi) filtreleme ve aggregation için `keyword` olarak tanımlanırken, log mesajının kendisi `text` olarak tanımlanır. `@timestamp` alanı ise her zaman `date` tipinde olmalıdır.

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

Yukarıdaki örnek bir **index template**'tir. `application_logs-` ile başlayan yeni bir index (örneğin `application_logs-2024-05-25`) oluşturulduğunda bu mapping otomatik olarak uygulanır. Bu, loglar gibi günlük veya haftalık oluşturulan index'ler için çok kullanışlıdır. Index template'leri hakkında daha fazla bilgiyi [Index Templates Dokümantasyonu'nda](https://www.elastic.co/guide/en/elasticsearch/reference/current/index-templates.html) bulabilirsiniz.

#### En Sık Kullanılan Veri Tipleri ve Amaçları

* `text`: Full-text arama yapılacak metinler için (ürün açıklaması, blog yazısı, log mesajları). Bu tipteki alanlar bir **analyzer**'dan geçer.
* `keyword`: Birebir eşleşme, sıralama (sorting) ve aggregation (gruplama) yapılacak metinler için (kategori adı, etiketler, durum kodları, e-posta adresi, ID'ler, log seviyesi, servis adı).
* **Sayısal Tipler:** `long`, `integer`, `short`, `byte`, `double`, `float`, `half_float`, `scaled_float`.
* `date`: Tarih ve zaman bilgileri. Loglar için `@timestamp` alanı olmazsa olmazdır.
* `boolean`: `true` veya `false`.
* `ip`: IPv4 ve IPv6 adresleri için. Loglarda kaynak/hedef IP'ler için kullanılır.
* `geo_point`, `geo_shape`: Coğrafi veriler.
* `object`: İç içe JSON objeleri.
* `nested`: Obje dizileri için.

Bu bölümde mapping'in temellerine değindik. Veri tipleri, analizciler ve mapping parametreleri hakkında çok daha fazla detay ve ileri düzey seçenekler için [Elasticsearch Resmi Dokümantasyonu'ndaki Mapping bölümüne](https://www.elastic.co/guide/en/elasticsearch/reference/current/mapping.html) ve [Alan Veri Tipleri (Field datatypes) sayfasına](https://www.elastic.co/guide/en/elasticsearch/reference/current/mapping-types.html) göz atmanız, bu konudaki anlayışınızı derinleştirecektir.

#### Önemli Mapping Parametreleri

Mapping oluştururken `properties` içinde her alan için bir veri tipi (`type`) belirtmenin ötesinde, o alanın davranışını kontrol eden birçok güçlü parametre vardır. Bu parametreler, Elasticsearch'ün verinizi nasıl indeksleyeceğini, analiz edeceğini ve arayacağını özelleştirmenizi sağlar. Gelin en sık karşınıza çıkacak ve hayatınızı kurtaracak olanlara göz atalım.

* **`type`**: Bu en temel parametredir. Alanın hangi türde veri tutacağını belirtir (`text`, `keyword`, `date`, `long`, `boolean`, `ip` vb.). Doğru tipi seçmek, yapacağınız aramaların ve analizlerin temelini oluşturur. Örneğin, bir ürün adını hem serbest metin araması (full-text search) yapmak hem de tam eşleşme ile filtrelemek isteyebilirsiniz. İşte burada diğer parametreler devreye girer.

* **`analyzer`**: `text` tipindeki alanlar için kullanılır ve metnin nasıl parçalanacağını, filtreleneceğini ve indeksleneceğini belirleyen analiz zincirini tanımlar. Örneğin, `standard` analyzer metni kelimelere böler ve küçük harfe çevirirken, `english` analyzer İngilizce'ye özgü "stemming" (kelime köklerini bulma, "running" -> "run") gibi ek işlemler yapar. Log mesajlarını analiz ederken genellikle `standard` yeterli olur.

* **`search_analyzer`**: Normalde bir alan, `analyzer` ile belirtilen analizci kullanılarak hem indekslenir hem de aranır. Ancak bazen arama anında farklı bir analizci kullanmak isteyebilirsiniz. Örneğin, otomatik tamamlama (autocomplete) senaryolarında, veriyi indekslerken farklı, kullanıcı yazarken arama yaparken farklı bir analiz süreci (örn: `edge_ngram_analyzer`) kullanmak daha etkili sonuçlar verir.

* **`fields`**: İşte sihir burada başlar! Bu parametre, aynı kaynak veriyi farklı şekillerde indekslemenizi sağlar. En yaygın kullanımı, bir `text` alanının aynı zamanda `keyword` olarak da indekslenmesidir.

    ```json
    "full_name": {
      "type": "text",
      "analyzer": "standard",
      "fields": {
        "keyword": { "type": "keyword" }
      }
    }
    ```

    Bu sayede `full_name` alanında "Ahmet Yılmaz" araması yapabilirken, `full_name.keyword` alanını kullanarak sıralama yapabilir veya "Ahmet Yılmaz" ismine tam olarak uyan kayıtları filtreleyebilirsiniz.

* **`dynamic`**: Bu parametre, mapping'de tanımlanmamış yeni bir alan geldiğinde Elasticsearch'ün ne yapacağını belirler. Üç değeri olabilir:
  * `true`: (Varsayılan) Yeni alanı tahmin ettiği bir tiple mapping'e ekler.
  * `false`: Yeni alanı mapping'e eklemez, yani bu alan aranamaz olur. Ancak verisi `_source` içinde saklanmaya devam eder.
  * `strict`: "Bana sormadan asla!" modudur. Mapping'de olmayan bir alan gelirse dokümanın tamamını reddeder ve hata fırlatır. Production ortamında beklenmedik alanların mapping'i "kirletmesini" önlemek için `strict` veya `false` kullanmak iyi bir pratiktir.

* **`enabled`**: "Bu alanı indeksleme zahmetine hiç girme" demenin yoludur. `enabled: false` olarak ayarlanan bir alan, aranamaz ve hiçbir şekilde diskte indekslenmiş veri olarak yer kaplamaz. Sadece `_source` içinde depolanır. Belki de sadece bir API cevabında döndürmek istediğiniz ama asla arama kriteri olmayacak bir veri için kullanabilirsiniz.

* **`format`**: `date` tipindeki alanlar için kullanılır. Gelen tarih verisinin hangi formatta olduğunu belirtmenizi sağlar. Elasticsearch birçok standart formatı anlasa da, özel formatlarınız varsa burada belirtebilirsiniz: `"format": "yyyy-MM-dd HH:mm:ss||yyyy-MM-dd||epoch_millis"`. `||` ile birden çok format tanımlayabilirsiniz.

* **`ignore_above`**: Özellikle `keyword` alanları için kurtarıcı bir dosttur. Belirtilen karakter sayısından daha uzun olan string'lerin indekslenmesini engeller. Bu, kullanıcı tarafından girilen etiketler veya bazı log satırları gibi beklenmedik derecede uzun olabilecek verilerin, Elasticsearch'ün hata vermesini (ve indekslemeyi durdurmasını) engeller. Örneğin, `"ignore_above": 256` ayarı, 256 karakterden uzun etiketlerin indekslenmemesini sağlayarak gelecekteki baş ağrılarını önler.

Tüm bu mapping parametreleri ve daha fazlası için [Elasticsearch Mapping Parametreleri Dokümantasyonu'na](https://www.elastic.co/guide/en/elasticsearch/reference/current/mapping-params.html) başvurmak en doğru adres olacaktır.

Mapping, Elasticsearch'ün performansı ve doğruluğu için hayati öneme sahiptir. "Şimdi kim uğraşacak bununla" deme, gelecekteki sen sana teşekkür edecek!

## 2.4 Pratik Zamanı: `products` Index'imizi Kuralım!

Şimdi öğrendiklerimizi pekiştirmek için Kibana Dev Tools'da kendi `products` index'imizi oluşturalım, ona explicit bir mapping verelim ve birkaç ürün ekleyelim.

1. **`products` Index'ini Explicit Mapping ile Oluşturma:**

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

2. **`_bulk` API'si ile Birkaç Ürün Ekleme:**

    ```http
    POST /products/_bulk
    { "index": { "_id": "SKU001" } }
    { "sku": "SKU001", "name": "Awesome Laptop Model Z", "description": "A powerful laptop with the latest features.", "price": 1399.99, "stock_quantity": 15, "category": "Computers", "tags": ["laptop", "new-gen", "gaming"], "created_date": "2024-05-20T10:00:00Z", "is_active": true }
    { "index": { "_id": "SKU002" } }
    { "sku": "SKU002", "name": "Super Silent Mechanical Keyboard", "description": "RGB backlit, long-lasting mechanical keyboard.", "price": 75.50, "stock_quantity": 45, "category": "Accessories", "tags": ["keyboard", "mechanical", "rgb"], "created_date": "2024-05-21T14:30:00Z", "is_active": true }
    { "index": { "_id": "SKU003" } }
    { "sku": "SKU003", "name": "Wide Screen Monitor", "description": "Ideal for professional use, 32 inch 4K monitor.", "price": 450.00, "stock_quantity": 0, "category": "Monitors", "tags": ["monitor", "4k", "professional"], "created_date": "2024-05-15T09:15:00Z", "is_active": false }
    ```

## 2.5 Log Verileriyle Çalışmak: Pratik Bir Senaryo

Şimdi de tipik bir uygulama logu senaryosu için bir index oluşturalım ve birkaç örnek log kaydı ekleyelim. Önce yukarıda (Bölüm 2.3.2) tanımladığımız `application_logs_template`'i oluşturalım (eğer oluşturmadıysanız).

1. **`application_logs_template`'i Oluşturma (Eğer Daha Önce Yapılmadıysa):**

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

    *Not: Güncel Elasticsearch sürümlerinde `PUT /_template/application_logs_template` yerine `PUT /_index_template/application_logs_template` kullanılır ve `template` anahtarı altına alınır. Eski sürümler için sözdizimi farklı olabilir.*

2. **`_bulk` API'si ile Örnek Log Kayıtları Ekleme (Örneğin, `application_logs-2024-05-24` index'ine):**

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

    Bu loglar, `application_logs_template`'inde tanımladığımız mapping'e göre indekslenecektir.

Tebrikler! Artık Elasticsearch'e hem ürün hem de log verisi nasıl eklenir, güncellenir, silinir ve en önemlisi bu verilerin yapısı nasıl tanımlanır biliyorsun. Bu, arama ve analiz maceralarımız için çok sağlam bir temel oluşturdu. Bir sonraki bölümde bu verileri nasıl sorgulayacağımızı, yani arama sanatının inceliklerini öğreneceğiz.
