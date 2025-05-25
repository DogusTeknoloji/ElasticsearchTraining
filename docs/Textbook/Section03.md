# Bölüm 3: Arama Sanatında Ustalaşın: Query DSL

Verilerimizi Elasticsearch'e başarıyla yükledik ve onlara güzel bir kimlik kartı (mapping) verdik. Şimdi sıra geldi o verilerin içinde kaybolmadan, istediğimiz bilgiye ışık hızıyla ulaşmaya! İşte bu bölümde Elasticsearch'ün kalbi olan **Query DSL (Domain Specific Language)** ile tanışacak, farklı arama senaryolarına uygun sorgular yazmayı öğreneceğiz. "Google'a yazar gibi" arama yapmaktan çok daha fazlasını keşfedeceğiz. Hazırsan, arama gözlüklerini tak ve başlayalım!

## 3.1 Arama Sorgusunun Anatomisi: `_search` API'si ile Tanışma

Elasticsearch'te arama yapmak için temel olarak `_search` endpoint'ini kullanırız. Bu endpoint'e bir HTTP GET veya POST isteği göndererek sorgularımızı iletebiliriz.

* **URI Search (GET ile Basit Aramalar):** Çok basit aramalar için URL üzerinden parametreler (`q=aranan_kelime` gibi) gönderilebilir.

  ```http
  GET /products/_search?q=name:laptop
  GET /application_logs-2024-05-24/_search?q=level:ERROR
  ```

  Bu yöntem hızlı ve kolaydır ama karmaşık sorgular için yetersiz kalır ve bazı karakterlerin URL encoding'i ile uğraşmak gerekebilir. "Hızlıca bir bakıp çıkacağım" durumları için idare eder.

* **Request Body Search (POST ile Kapsamlı Aramalar):** Asıl gücümüzü göstereceğimiz yer burası! Sorgularımızı JSON formatında, isteğin gövdesinde (`request body`) göndeririz. Bu yöntem çok daha esnek ve güçlüdür.

  ```http
  POST /products/_search
  {
    "query": {
      // Sorgu detayları buraya gelecek
    }
  }
  ```

  Bundan sonraki tüm örneklerimizde bu yapıyı kullanacağız.

**Query Context vs Filter Context: Skor mu, Hız mı?**

Elasticsearch'te sorgu yazarken karşımıza iki önemli "context" (bağlam) çıkar: **Query Context** ve **Filter Context**. Bu ikisinin farkını anlamak, hem doğru sonuçlar almak hem de performansı optimize etmek için çok önemlidir.

* **Query Context (`query` anahtarı altında):**
  * **Amacı:** "Bu doküman, arama kriterlerime **ne kadar iyi** eşleşiyor?" sorusuna cevap arar.
  * **Skorlama (`_score`):** Eşleşen her doküman için bir **alaka skoru (`_score`)** hesaplar. Skor ne kadar yüksekse, doküman o kadar alakalı demektir. Sonuçlar varsayılan olarak bu skora göre büyükten küçüğe sıralanır.
  * **Kullanım Alanı:** Full-text aramalarda, kullanıcının aradığı şeye en yakın sonuçları bulmak istediğimizde kullanılır. Örneğin, bir ürün adında veya açıklamasında geçen kelimelere göre arama yaparken.
  * **Sorgu Tipleri:** Genellikle `match`, `multi_match`, `query_string` gibi full-text sorguları bu context'te kullanılır.

* **Filter Context (`filter` anahtarı altında, genellikle `bool` sorgusu içinde):**
  * **Amacı:** "Bu doküman, arama kriterlerime eşleşiyor mu? (Evet/Hayır)" sorusuna cevap arar.
  * **Skorlama Yok:** Alaka skoru hesaplanmaz. Bir doküman ya filtreye uyar ya da uymaz.
  * **Performans ve Cache'leme:** Skorlama yapılmadığı için genellikle query context'ten daha hızlıdır. Ayrıca, filtre sonuçları Elasticsearch tarafından sıkça **cache'lenebilir**, bu da tekrarlayan sorgularda performansı ciddi şekilde artırır. "Bu filtreyi daha önce görmüştüm, sonucu hazır!" der ES.
  * **Kullanım Alanı:** Kesin eşleşmeler, aralıklar veya belirli bir koşulu sağlayan dokümanları bulmak için idealdir. Örneğin, "kategorisi 'Elektronik' olan ürünler", "fiyatı 1000-2000 TL arasında olanlar", "stokta olan ürünler" gibi.
  * **Sorgu Tipleri:** Genellikle `term`, `terms`, `range`, `exists` gibi birebir eşleşme veya yapısal sorgular bu context'te kullanılır.

**Ne Zaman Hangisini Kullanmalı?**

* Eğer "en alakalı" sonuçları bulmak ve skorlamanın önemli olduğu bir full-text arama yapıyorsan **Query Context**.
* Eğer sadece belirli bir koşula uyan/uymayan dokümanları filtrelemek, evet/hayır cevabı almak ve performansı önceliklendirmek istiyorsan **Filter Context**.
* Çoğu zaman bu ikisini birlikte kullanırız: `bool` sorgusu içinde hem `must` (query context) hem de `filter` (filter context) kısımlarını kullanarak hem alakalı hem de filtrelenmiş sonuçlar elde ederiz.

Bu ayrım, sorgularınızın hem doğruluğunu hem de hızını optimize etmenizde kilit rol oynayacaktır.

## 3.2 Temel Sorgu Tipleri: Query DSL'e İlk Adımlar

Query DSL, Elasticsearch'e ne aradığımızı anlatmak için kullandığımız JSON tabanlı bir dildir. Oldukça zengin ve esnektir. Şimdi en sık kullanılan temel sorgu tiplerine bir göz atalım.

### 3.2.1 Full-Text Sorguları (Genellikle Query Context'te)

Bu sorgular, metin alanlarında analiz edilmiş (token'lara ayrılmış) içerik üzerinde arama yapar.

* **`match` Sorgusu: Standart Full-Text Arama**
  En yaygın kullanılan full-text sorgusudur. Verilen metni analiz eder (arama terimini de analiz eder!) ve eşleşen dokümanları bulur.

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

  * `operator`: Varsayılan olarak `OR`'dur (yani "powerful" VEYA "gaming" VEYA "laptop" içerenler). `AND` yaparsanız tüm kelimelerin geçmesi gerekir.

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

  * `fuzziness`: Yazım hatalarını tolere etmek için kullanılır. `AUTO` veya `1`, `2` gibi Levenshtein mesafesi değerleri alabilir. "Laptob" yazsan da "laptop" bulsun diye.

  Log mesajları içinde belirli bir hata kodunu veya kelimeyi aramak için kullanılabilir:

  ```http
  POST /application_logs-*/_search // Birden fazla log index'inde arama
  {
    "query": {
      "match": {
        "message": "timeout"
      }
    }
  }
  ```

* **`match_phrase` Sorgusu: Kelime Grubunu Tam Olarak Arama**
  Verilen kelimelerin aynı sırada ve birbirine yakın geçmesini bekler.

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

  * `slop`: Kelimeler arasında izin verilen maksimum ekstra kelime sayısını belirtir. `slop: 1` ile "Awesome Super Laptop" da eşleşebilir.

* **`multi_match` Sorgusu: Birden Fazla Alanda Arama**
  Aynı arama terimini birden fazla alanda aramak için kullanılır.

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

  * `fields` alanına `*` veya `*_name` gibi wildcard'lar da verebilirsiniz. Alanlara farklı ağırlıklar (`^3` gibi) vererek skorlamayı etkileyebilirsiniz.
  * `type`: `best_fields` (varsayılan, en iyi eşleşen alana göre skorlar), `most_fields` (daha fazla alanda eşleşen daha iyi skor alır), `cross_fields` (alanları tek bir büyük alan gibi düşünür, yapısal veriler için) gibi farklı eşleşme stratejileri sunar.

* **`query_string` / `simple_query_string` Sorgusu:**
  Lucene'in güçlü sorgu sentaksını (AND, OR, NOT, wildcard'lar, aralıklar vb.) doğrudan kullanmanızı sağlar.

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

  `query_string` çok güçlüdür ama kullanıcı girdisiyle doğrudan kullanılırsa sentaks hatalarına veya güvenlik açıklarına yol açabilir. `simple_query_string` daha güvenli bir alternatifidir, hatalı sentaksı görmezden gelir.

### 3.2.2 Term-Level Sorguları (Genellikle Filter Context'te)

Bu sorgular, analiz edilmemiş (olduğu gibi saklanmış) değerler üzerinde **birebir eşleşme** arar. Genellikle `keyword`, sayısal, tarih ve `boolean` alanları için kullanılırlar. Filter context'te kullanıldıklarında skorlamaya dahil olmazlar ve cache'lenebilirler.

* **`term` Sorgusu: Tek Bir Değerle Birebir Eşleşme**
  Verilen değerin alanda tam olarak bulunup bulunmadığını kontrol eder.

  ```http
  POST /products/_search
  {
    "query": {
      "term": {
        "category": "Accessories" // category alanı keyword olmalı
      }
    }
  }
  ```

  **Dikkat:** `term` sorgusunu `text` alanında kullanırsanız, aradığınız kelimenin analiz sürecinden sonra oluşan token ile tam olarak eşleşmesi gerekir (genellikle küçük harf). "Accessories" yerine "accessories" aramanız gerekebilir. Bu yüzden `text` alanları için `match` daha uygundur.

  Belirli bir log seviyesindeki kayıtları bulmak için:

  ```http
  POST /application_logs-*/_search
  {
    "query": {
      "term": {
        "level": "ERROR" // level alanı keyword olmalı
      }
    }
  }
  ```

* **`terms` Sorgusu: Birden Fazla Değerle Eşleşme (`IN` gibi)**
  Alanda belirtilen değerlerden herhangi birinin bulunup bulunmadığını kontrol eder.

  ```http
  POST /products/_search
  {
    "query": {
      "terms": {
        "tags": ["laptop", "gaming", "new-gen"] // tags alanı keyword olmalı
      }
    }
  }
  ```

* **`ids` Sorgusu: Belirli ID'lere Sahip Dokümanları Getirme**

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

* **`range` Sorgusu: Belirli Bir Aralıktaki Değerler**
  Sayısal, tarih veya string alanlarında belirli bir aralıktaki değerleri bulur.

  ```http
  POST /products/_search
  {
    "query": {
      "range": {
        "price": {
          "gte": 70,     // greater than or equal to (büyük veya eşit)
          "lt": 500      // less than (küçük)
        }
      }
    }
  }
  ```

  Diğer operatörler: `gt` (büyük), `lte` (küçük veya eşit). Tarihler için `now-1d/d` (dünden bugüne) gibi ifadeler de kullanılabilir.

  Loglar için zaman aralığında arama yapmak çok yaygındır:

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

* **`exists` Sorgusu: Alanın Var Olup Olmadığı**
  Belirli bir alanın dokümanda var olup olmadığını (null veya boş dizi olmaması) kontrol eder.

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

* **`prefix` Sorgusu: Belirli Bir Önekle Başlayanlar**
  `keyword` alanlarında belirli bir önekle başlayan değerleri bulur.

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

* **`wildcard` Sorgusu: Joker Karakterlerle Desen Eşleştirme**
  `*` (birden fazla karakter) ve `?` (tek karakter) jokerlerini kullanarak desen eşleştirmesi yapar.

  ```http
  POST /products/_search
  {
    "query": {
      "wildcard": {
        "name.keyword": { // Genellikle .keyword alanında kullanılır
          "value": "Lap*Pro"
        }
      }
    }
  }
  ```

  **Performans Uyarısı:** `wildcard` ve `prefix` sorguları (özellikle başta `*` veya `?` ile başlayanlar) yavaş olabilir. Dikkatli kullanılmalıdır.

### 3.2.3 Diğer Kullanışlı Sorgular

* **`match_all` Sorgusu: Tüm Dokümanları Getir**
  Herhangi bir filtreleme yapmadan index'teki tüm dokümanları getirir.

  ```http
  POST /products/_search
  {
    "query": {
      "match_all": {}
    }
  }
  ```

* **`match_none` Sorgusu: Hiçbir Dokümanı Getirme**
  Hiçbir dokümanı getirmez. Nadiren kullanılır.

## 3.3 Sorguları Birleştirme Sanatı: `bool` Sorgusu

Gerçek dünya senaryolarında genellikle birden fazla koşulu birleştirerek arama yaparız. İşte bu noktada `bool` (boolean) sorgusu devreye girer. `bool` sorgusu, diğer sorgu tiplerini mantıksal operatörlerle (`AND`, `OR`, `NOT`) birleştirmemizi sağlar ve Query DSL'in en temel yapı taşlarından biridir.

`bool` sorgusunun dört ana bölümü vardır:

* **`must`:** Bu bölümdeki tüm sorgular eşleşmelidir (lojik `AND`). Eşleşen dokümanların skoruna katkıda bulunur.
* **`filter`:** Bu bölümdeki tüm sorgular eşleşmelidir (lojik `AND`). Ancak, filter context'te çalışır, yani skorlamaya dahil olmaz ve cache'lenebilir. Performans için idealdir.
* **`should`:** Bu bölümdeki sorgulardan en az biri eşleşmelidir (lojik `OR`). Eşleşen dokümanların skoruna katkıda bulunur. Eğer `must` veya `filter` yoksa, `should` içindeki en az bir koşulun sağlanması gerekir.
  * `minimum_should_match`: `should` bölümünde en az kaç koşulun sağlanması gerektiğini belirtir (örneğin, `1`, `2`, `"75%"`).
* **`must_not`:** Bu bölümdeki hiçbir sorgu eşleşmemelidir (lojik `NOT`). Filter context'te çalışır, skorlamaya dahil olmaz.

**Örnek bir `bool` sorgusu (Ürünler için):**
"Kategorisi 'Accessories' olan (filter), adında veya açıklamasında 'gaming keyboard' geçen (query) VE fiyatı 100'den az olan (filter) AMA etiketlerinde 'refurbished' geçmeyen (must_not) ürünleri bul."

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

`bool` sorguları iç içe de kullanılabilir, bu da çok karmaşık arama mantıkları oluşturmanıza olanak tanır. "Sorguların efendisi" desek yeridir!

**Örnek bir `bool` sorgusu (Loglar için):**
"'payment-service' servisinden gelen, 'ERROR' seviyesindeki VE mesajında 'database' kelimesi geçen logları bul."

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

Tüm bu sorgu tipleri ve `bool` sorgusu hakkında daha fazla örnek ve detay için [Elasticsearch Query DSL Dokümantasyonu'na](https://www.elastic.co/guide/en/elasticsearch/reference/current/query-dsl.html) mutlaka göz atın.

## 3.4 Pratik Zamanı: `products` Index'inde Arama Yapalım!

Haydi, Kibana Dev Tools'u açıp `products` index'imizde öğrendiğimiz sorguları deneyelim!

1. **Adında "Laptop" geçen ve fiyatı 1000'den büyük olan ürünler:**

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

2. **Kategorisi "Accessories" VEYA "Monitors" olan, stokta bulunan (stock_quantity > 0) ürünler:**

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

3. **Açıklamasında "latest features" geçen AMA "gaming" etiketi olmayan ürünler:**

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

Bu örnekleri çoğaltmak mümkün. Kendi senaryolarınızı düşünerek farklı sorgular yazmayı deneyin. Query DSL'de ustalaşmanın en iyi yolu pratik yapmaktır!

## 3.5 Pratik Zamanı: `application_logs` Index'inde Arama Yapalım!

Şimdi de log verilerimiz üzerinde bazı aramalar yapalım.

1. **"WARN" seviyesindeki tüm logları bulun:**

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

2. **Belirli bir zaman aralığındaki (örneğin, son 1 saat) "ERROR" loglarını bulun:**

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

3. **Mesajında "failed" kelimesi geçen ve "auth-service" veya "payment-service" tarafından üretilen loglar:**

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

Log verileriyle çalışırken zaman aralıkları, log seviyeleri ve belirli anahtar kelimeler üzerinden filtreleme yapmak çok yaygındır. Bu örnekler size bir başlangıç noktası sunacaktır.

Bir sonraki bölümde, arama sonuçlarını nasıl yöneteceğimizi (sayfalama, sıralama) ve Elasticsearch'ün güçlü analiz yeteneği olan Aggregation'ları keşfedeceğiz.
