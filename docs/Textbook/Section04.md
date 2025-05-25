# Bölüm 4: Analizin Gücü: Aggregation'larla Veriye Hükmetmek

Elasticsearch sadece veri bulmakla kalmaz, aynı zamanda bu veriyi anlamlandırmak, özetlemek ve içgörüler çıkarmak için de inanılmaz güçlü araçlara sahiptir. İşte bu araçların en önemlisi **Aggregation API**'sidir. SQL'deki `GROUP BY` ve aggregate fonksiyonlarının (SUM, COUNT, AVG vb.) çok daha esnek ve güçlü bir versiyonu gibi düşünebilirsiniz. "Veri konuşsun, biz dinleyelim" felsefesiyle, aggregation'lar sayesinde verilerinizin sakladığı hikayeleri ortaya çıkaracağız.

## 4.1 Aggregation'lara Giriş: Nedir, Ne İşe Yarar?

Aggregation'lar, arama sonuçlarınızdaki dokümanları gruplara ayırmanıza (bucket'lama) ve bu gruplar üzerinde çeşitli metrikler (hesaplamalar) yapmanıza olanak tanır. Örneğin:

* Her kategoride kaç ürün var?
* En popüler etiketler hangileri?
* Belirli bir fiyat aralığındaki ürünlerin ortalama stok miktarı nedir?
* Aylık satış trendleri nasıl?
* Hangi servis en çok hata logu üretiyor?
* Günün hangi saatlerinde hata yoğunluğu artıyor?

Bu tür soruların cevaplarını aggregation'lar ile kolayca bulabiliriz. Aggregation'lar, `_search` endpoint'ine gönderilen sorgunun `aggs` (veya `aggregations`) anahtarı altında tanımlanır.

```http
POST /products/_search
{
  "query": {
    "match_all": {}
  },
  "aggs": {
    "my_first_aggregation": {
      // Aggregation tanımı buraya gelecek
    }
  },
  "size": 0
}
```

`"my_first_aggregation"` kısmı, aggregation sonucunun cevapta hangi isimle döneceğini belirler. İstediğiniz bir ismi verebilirsiniz.

Aggregation'lar temel olarak iki ana kategoriye ayrılır:

1. **Bucket Aggregations:** Dokümanları belirli kriterlere göre gruplara (bucket'lara) ayırır. Her bucket, o kritere uyan dokümanları içerir.
2. **Metric Aggregations:** Bucket'lardaki (veya tüm sonuç setindeki) dokümanlar üzerinde sayısal hesaplamalar yapar (toplam, ortalama, min, max vb.).

Bu iki tür genellikle iç içe kullanılır: Önce bucket'lara ayırır, sonra her bucket için metrik hesaplarız.

## 4.2 Bucket Aggregations: Veriyi Anlamlı Gruplara Ayırmak

En sık kullanılan bucket aggregation'larına bir göz atalım:

* **`terms` Aggregation: Benzersiz Değerlere Göre Gruplama**
  Belirli bir alandaki benzersiz değerlere göre dokümanları gruplar. SQL'deki `GROUP BY some_field` gibi.
  Örnek: Her kategoride kaç ürün olduğunu bulalım.

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

  Cevapta, her bir kategori için bir bucket ve o bucket'taki doküman sayısı (`doc_count`) gelir.
  * `size`: Kaç tane bucket döneceğini belirler (varsayılan 10).
  * `order`: Bucket'ların neye göre sıralanacağını belirtir (ör: `{"_count": "desc"}` doküman sayısına göre çoktan aza).

* **`range` / `date_range` Aggregation: Belirli Aralıklara Göre Gruplama**
  Sayısal veya tarih alanlarında sizin tanımladığınız aralıklara göre gruplama yapar.
  Örnek: Fiyat aralıklarına göre ürün sayıları.

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

  `date_range` da benzer şekilde tarih aralıkları için kullanılır (`format` ve `time_zone` parametreleri önemlidir).

* **`histogram` / `date_histogram` Aggregation: Sabit Aralıklara Göre Gruplama**
  Sayısal veya tarih alanlarında sabit bir aralığa (`interval`) göre bucket'lar oluşturur.
  Örnek: Ürün fiyatlarının 500'lük aralıklarla dağılımı.

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

  `date_histogram` için `interval` değeri `day`, `week`, `month`, `1d`, `7d`, `1M` gibi zaman birimleri olabilir. Log analizi ve zaman serisi verileri için çok kullanışlıdır.
  Örnek: Saatlik log sayıları.

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

* **`filter` / `filters` Aggregation: Filtreye Uyanları Gruplama**
  `filter` tek bir filtreye uyan dokümanları tek bir bucket'a koyar. `filters` ise birden fazla isimlendirilmiş filtre tanımlamanızı ve her birine uyanları ayrı bucket'lara koymanızı sağlar.
  Örnek: Stokta olan ve olmayan ürün sayıları.

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

## 4.3 Metric Aggregations: Gruplar Üzerinde Hesaplamalar Yapmak

Bucket'larımızı oluşturduktan sonra, bu bucket'lardaki (veya tüm sonuç setindeki) veriler üzerinde çeşitli hesaplamalar yapabiliriz.

* **`min`, `max`, `avg`, `sum` Aggregations: Temel İstatistikler**
  Belirli bir sayısal alanın minimum, maksimum, ortalama veya toplam değerini hesaplar.
  Örnek: Tüm ürünlerin ortalama fiyatı.

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

* **`stats` / `extended_stats` Aggregations: Kapsamlı İstatistikler**
  `stats`, tek seferde `count`, `min`, `max`, `avg`, `sum` değerlerini verir. `extended_stats` ise bunlara ek olarak `sum_of_squares`, `variance`, `std_deviation` (standart sapma) gibi daha ileri istatistikleri de sunar.

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

* **`cardinality` Aggregation: Benzersiz Değer Sayısı (Yaklaşık)**
  Belirli bir alandaki benzersiz değer sayısını tahmin eder. SQL'deki `COUNT(DISTINCT field)` gibi. Büyük veri setlerinde tam sayım pahalı olabileceği için HyperLogLog++ algoritmasını kullanarak yaklaşık ama hızlı bir sonuç verir.
  Örnek: Kaç farklı kategori var?

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

  `precision_threshold` parametresi ile doğruluk ve performans arasında denge kurulabilir.

* **`percentiles` / `percentile_ranks` Aggregations: Yüzdelik Dilimler**
    `percentiles`, verinin belirli yüzdelik dilimlerini (örneğin, %50 (medyan), %95, %99) gösterir. `percentile_ranks` ise belirli değerlerin hangi yüzdelik dilime denk geldiğini gösterir. Performans izleme (örneğin, isteklerin %99'u kaç ms altında tamamlanıyor?) için çok kullanışlıdır.

## 4.4 İç İçe Aggregation'lar: Detayın Detayına İnmek

Aggregation'ların asıl gücü, iç içe kullanılabilmelerinden gelir. Bir bucket aggregation'ın altına başka bir bucket veya metric aggregation ekleyerek çok daha detaylı analizler yapabiliriz.

Örnek (Ürünler için): Her bir kategorideki (`terms` bucket) ürünlerin ortalama fiyatını (`avg` metric) bulalım.

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

Cevapta, her kategori bucket'ının içinde o kategoriye ait ürünlerin ortalama fiyatı da gelecektir.

Örnek (Loglar için): Her bir servis (`service_name`) için farklı log seviyelerindeki (`level`) log sayılarını bulalım.

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

Bu iç içe yapıyı istediğiniz kadar derinleştirebilirsiniz (tabii performans implications'ı göz önünde bulundurarak).

Aggregation'lar, Elasticsearch'ün sadece bir arama motoru olmadığını, aynı zamanda güçlü bir analitik platformu olduğunu gösteren en önemli özelliklerdendir. Tüm aggregation türleri ve seçenekleri için [Elasticsearch Aggregations Dokümantasyonu'na](https://www.elastic.co/guide/en/elasticsearch/reference/current/search-aggregations.html) mutlaka detaylıca bakın.

## 4.5 Pratik Zamanı: `products` Verimizi Analiz Edelim!

1. **En çok ürüne sahip ilk 5 kategoriyi ve her kategorideki ürün sayısını bulun:**

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

2. **Tüm ürünlerin minimum, maksimum ve ortalama fiyatlarını bulun:**

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

3. **Her bir etikete (`tags`) sahip ürün sayısını ve bu etiketlere sahip ürünlerin ortalama stok miktarını bulun:**

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

Bu örnekler sadece bir başlangıç. Kendi verilerinize ve merak ettiğiniz sorulara göre çok daha karmaşık ve ilginç aggregation'lar oluşturabilirsiniz.

## 4.6 Pratik Zamanı: `application_logs` Verimizi Analiz Edelim!

Şimdi de log verilerimiz üzerinde bazı anlamlı analizler yapalım.

1. **Her bir servis (`service_name`) için toplam log sayısını bulun:**

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

2. **Son 24 saat içindeki saatlik "ERROR" seviyesindeki log sayılarını bulun:**

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

3. **En çok hata (`level: ERROR`) üreten ilk 5 host IP'sini (`host_ip`) bulun:**

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

Log verileri üzerinde yapılan aggregation'lar, sistem sağlığını izlemek, hataları tespit etmek ve performans darboğazlarını anlamak için paha biçilmezdir.
