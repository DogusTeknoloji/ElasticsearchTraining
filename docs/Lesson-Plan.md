# Elasticsearch Eğitimi: Kapsamlı Ders İçeriği (6 Saat)

## Eğitimin Temel Hedefi:

Katılımcıların Elasticsearch'ün temel mimarisini, anahtar kavramlarını anlamalarını, veri indeksleme (indexing), sorgulama (searching), analiz (aggregations) yeteneklerini kavramalarını ve bu bilgileri pratik senaryolarda uygulayabilir hale gelmelerini sağlamaktır. Ayrıca, Elasticsearch'ün hangi tür problemlere çözüm sunduğunu ve production ortamında dikkat edilmesi gereken temel noktaları fark etmeleri hedeflenir.

## Hedef Kitle:

Orta ve Kıdemli Seviye Full Stack Yazılım Geliştiriciler. (REST API'ler, JSON, temel veritabanı ve dağıtık sistem kavramlarına aşina oldukları varsayılır.)

## Ön Hazırlıklar ve Gerekli Ortam:

* **Docker:** Her katılımcının bilgisayarında Docker Desktop'ın kurulu ve çalışır durumda olması.

* **Elasticsearch & Kibana Kurulumu:** Eğitim öncesinde katılımcılara sağlanacak basit bir `docker-compose.yml` dosyası ile tek komutla Elasticsearch ve Kibana'nın en güncel ve stabil versiyonlarından birini (örneğin 8.x serisi) kendi makinelerinde ayağa kaldırmaları istenir. Bu, eğitim sırasında kurulumla vakit kaybedilmesini önler.

  * Örnek `docker-compose.yml` içeriği:

    ```yaml
    version: '3.7'
    services:
      elasticsearch:
        image: docker.elastic.co/elasticsearch/elasticsearch:8.13.4 # Güncel stabil sürümü kontrol edin
        container_name: es01
        environment:
          - discovery.type=single-node
          - xpack.security.enabled=false # Eğitim kolaylığı için security kapalı, production'da açık olmalı!
          - ES_JAVA_OPTS=-Xms1g -Xmx1g # Kaynaklara göre ayarlanabilir
        ports:
          - "9200:9200"
        volumes:
          - esdata01:/usr/share/elasticsearch/data
      kibana:
        image: docker.elastic.co/kibana/kibana:8.13.4 # Elasticsearch ile aynı sürüm olmalı
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

* **İnternet Erişimi:** Dokümantasyon ve gerektiğinde kaynak araştırması için.

* **Kibana Dev Tools:** Tüm pratik uygulamalar Kibana > Management > Dev Tools üzerinden gerçekleştirilecektir.

## Ders Akışı ve Zaman Planı

**Toplam Eğitim Süresi:** ~360 Dakika (Net Ders + Kısa Aralar)
*Bu plan, 6 saatlik *aktif* eğitim ve kısa araları kapsamaktadır. Gün ortasında uzun bir öğle yemeği molası verilirse, toplam süre buna göre uzayacaktır.*

* **Modül 1: Elasticsearch'e Giriş ve Temel Kavramlar** (80 Dakika)
* **Ara** (15 Dakika)
* **Modül 2: Veri Yönetimi: Indexing ve Mapping** (85 Dakika)
* **Ara** (15 Dakika) *(Eğer gün ortasına denk geliyorsa, bu ara daha uzun bir öğle yemeği molası olarak da planlanabilir.)*
* **Modül 3: Arama Sanatı: Query DSL ile Sorgulama** (80 Dakika)
* **Ara** (15 Dakika)
* **Modül 4: İleri Arama, Aggregations ve Elastic Stack Ekosistemi** (85 Dakika)

### Modül Detayları

#### **Modül 1: Elasticsearch'e Giriş ve Temel Kavramlar (80 Dakika)**

* **(10 dk) Tanışma, Beklentiler ve Eğitimin Yol Haritası**
  * Eğitmenin ve katılımcıların kısa tanıtımı.
  * Katılımcıların Elasticsearch ile ilgili mevcut bilgi düzeyleri ve beklentileri.
  * Eğitim içeriği ve hedeflerinin üzerinden geçilmesi.
* **(20 dk) "Neden Elasticsearch?"**
  * Geleneksel ilişkisel veritabanlarının (RDBMS) full-text search ve büyük veri analizindeki zorlukları.
  * `LIKE '%...%'` sorgularının yetersizliği: Performans, alaka (relevance), dil desteği.
  * Yapısal olmayan (unstructured) ve yarı yapısal (semi-structured) verilerin (loglar, metinler, JSON dokümanları) artan önemi.
  * Gerçek dünya senaryoları: E-ticarette ürün arama, log analizi, anomali tespiti.
* **(25 dk) Elasticsearch Nedir?**
  * Apache Lucene üzerine kurulu, dağıtık, RESTful arama ve analitik motoru.
  * Temel Özellikleri: Hız, ölçeklenebilirlik, esneklik, gerçek zamanlıya yakın (near real-time) sonuçlar.
  * **Ne Değildir?** Birincil ACID uyumlu veritabanı olmadığını vurgulamak. "Source of Truth" yerine "Derived Data Store" veya "Search/Analytics Layer" olarak konumlandırmak.
  * Yaygın Kullanım Alanları:
    * Full-Text Search (Gelişmiş site içi arama, ürün kataloğu arama)
    * Log ve Metrik Analizi (Gözlemlenebilirlik - Observability: APM, logs, metrics)
    * İş Zekası (Business Intelligence) ve Analitik Raporlama
    * Güvenlik Analitiği (SIEM - Security Information and Event Management)
    * Coğrafi Veri Arama ve Analizi (Geospatial Search)
* **(25 dk) Temel Mimarî Kavramlar**
  * **Document (Doküman):** Verinin JSON formatındaki temel birimi.
  * **Index (İndeks):** Benzer özelliklere sahip dokümanların koleksiyonu. (RDBMS'teki "tablo" gibi düşünülebilir).
    * `_type` kavramının eski sürümlerdeki yeri ve güncel sürümlerde (7.x sonrası) kaldırılması.
  * **Node (Düğüm):** Cluster'ın bir parçası olan tek bir Elasticsearch sunucusu.
    * Node Tipleri: Master-eligible, Data, Ingest, Coordinating, Machine Learning. (Kısaca değinmek)
  * **Cluster (Küme):** Bir veya daha fazla Node'dan oluşan, veriyi ve iş yükünü paylaşan yapı.
  * **Shard (Parça):** Bir index'in yatay olarak bölündüğü parçalar. Ölçeklenebilirlik ve paralelleştirme sağlar.
    * Primary Shard.
  * **Replica (Kopya):** Primary Shard'ların kopyaları. Yüksek erişilebilirlik (high availability) ve okuma performansını artırma.
  * **Pratik Uygulama (Kibana Dev Tools):**
    * `GET /` : Cluster bilgilerini görme.
    * `GET /_cluster/health` : Cluster sağlık durumunu kontrol etme.
    * `GET /_cat/nodes?v` : Node'ları listeleme.
    * `GET /_cat/indices?v` : Mevcut index'leri listeleme.

#### **Modül 2: Veri Yönetimi: Indexing ve Mapping (85 Dakika)**

* **(20 dk) Doküman Yönetimi (CRUD Operasyonları)**
  * **Index API (Doküman Ekleme/Güncelleme):**
    * `PUT /{index}/_doc/{id}` : Belirli bir ID ile doküman ekleme/üzerine yazma.
    * `POST /{index}/_doc` : Elasticsearch'ün otomatik ID oluşturmasıyla doküman ekleme.
    * `_create` endpoint'i: `POST /{index}/_create/{id}` (Sadece ID yoksa oluşturur).
  * **Get API (Doküman Okuma):** `GET /{index}/_doc/{id}`
  * **Update API (Doküman Güncelleme):** `POST /{index}/_update/{id}`
    * Partial update (script ile veya `doc` ile).
    * `upsert` kavramı.
  * **Delete API (Doküman Silme):** `DELETE /{index}/_doc/{id}`
  * **Meta Alanlar:** `_index`, `_id`, `_version`, `_source`, `_score` (arama sonuçlarında).
* **(15 dk) Bulk API (`_bulk`)**
  * Neden toplu işlemler? Performans ve verimlilik.
  * `_bulk` endpoint'inin yapısı: action/metadata ve optional source satırları.
  * Desteklenen action'lar: `index`, `create`, `update`, `delete`.
  * Hata yönetimi ve sonuçların yorumlanması.
* **(35 dk) Mapping (Şema Yönetimi): Verinizin Anayasası**
  * **Dynamic Mapping:** Elasticsearch'ün veri tiplerini otomatik algılaması.
    * Avantajları: Hızlı başlangıç, esneklik.
    * Dezavantajları: Yanlış tip çıkarımları, gereksiz alanlar, performans sorunları, "mapping explosion". Production için neden önerilmediği.
  * **Explicit Mapping:** Index oluşturulurken alanların tiplerini ve özelliklerini manuel olarak tanımlama.
    * `PUT /{index}` ile mapping oluşturma/güncelleme (kısıtlamalarıyla).
    * **Temel Veri Tipleri ve Kullanım Amaçları:**
      * `text`: Full-text arama için analiz edilen metin (ör: ürün açıklaması, blog içeriği). `analyzers`.
      * `keyword`: Birebir eşleşme, sıralama, aggregation için kullanılan metin (ör: kategori ID, etiketler, durum kodları, e-posta adresi).
      * Sayısal Tipler: `long`, `integer`, `short`, `byte`, `double`, `float`, `half_float`, `scaled_float`.
      * `date` ve `date_nanos`: Tarih ve zaman formatları. `format` parametresi.
      * `boolean`: `true` / `false`.
      * `ip`: IPv4 ve IPv6 adresleri.
      * `geo_point`, `geo_shape`: Coğrafi veriler.
      * Karmaşık Tipler: `object` (iç içe JSON objeleri), `nested` (obje dizileri için özel tip).
  * **Mapping Parametreleri (Sık Kullanılanlar):**
    * `index`: Alanın indekslenip indekslenmeyeceği (`true`/`false`).
    * `analyzer`: `text` alanları için metin analizcisi (standard, simple, whitespace, custom vb.).
    * `search_analyzer`: Arama sırasında kullanılacak analizci.
    * `fields` (multi-fields): Aynı alanı farklı şekillerde indekslemek (ör: bir string alanı hem `text` hem `keyword` olarak).
    * `dynamic`: `true`, `false`, `strict`. Object alanları için dinamik mapping davranışını kontrol etme.
* **(15 dk) Pratik Uygulama (Kibana Dev Tools):**
  * `products` adında bir index için explicit mapping oluşturma (ör: `name: text`, `sku: keyword`, `price: float`, `tags: keyword[]`, `created_date: date`, `description: text` with multi-field for `.keyword`).
  * `_bulk` API'si ile çeşitli ürün dokümanları ekleme.
  * Birkaç dokümanı `GET` ile çekme, birini `_update` ile güncelleme, birini `DELETE` ile silme.
  * `GET /{index}/_mapping` ile mapping'i inceleme.

#### **Modül 3: Arama Sanatı: Query DSL ile Sorgulama (80 Dakika)**

* **(15 dk) Arama Sorgusunun Anatomisi (`_search` API)**
  * URI Search vs Request Body Search. Neden Request Body tercih edilmeli?
  * Temel Sorgu Yapısı: `query` objesi.
  * **Query Context vs Filter Context:**
    * **Query Context:** "Bu doküman sorguya ne kadar iyi eşleşiyor?" Alaka skoru (`_score`) hesaplanır. `match`, `multi_match` gibi sorgular burada çalışır.
    * **Filter Context:** "Bu doküman sorguyla eşleşiyor mu (evet/hayır)?" Skor hesaplanmaz. Genellikle daha hızlıdır ve cache'lenebilir. `term`, `range` gibi sorgular burada daha etkilidir. Performans için `filter` kullanmanın önemi.
* **(45 dk) Temel Sorgu Tipleri (Query DSL)**
  * **Full-Text Queries (Genellikle Query Context'te kullanılır):**
    * `match`: Standart full-text arama. `operator` (`OR`/`AND`), `fuzziness`.
    * `match_phrase`: Kelime grubunu tam olarak arama. `slop` parametresi.
    * `multi_match`: Birden fazla alanda aynı sorguyu çalıştırma. `fields`, `type` (best_fields, most_fields, cross_fields).
    * `query_string` / `simple_query_string`: Lucene sorgu sentaksını destekler. Dikkatli kullanılmalı.
  * **Term-Level Queries (Genellikle Filter Context'te kullanılır):**
    * `term`: Analiz edilmemiş, birebir eşleşen değerleri arama (genellikle `keyword` alanları için).
    * `terms`: Birden fazla `term` değeri için (`IN` clause gibi).
    * `ids`: Belirli ID'lere sahip dokümanları getirme.
    * `range`: Sayısal, tarih veya string aralıkları için (`gt`, `gte`, `lt`, `lte`).
    * `exists`: Belirli bir alanın var olup olmadığını kontrol etme.
    * `prefix`: Belirli bir önekle başlayan `keyword` alanlarını arama.
    * `wildcard`: `*` ve `?` karakterleriyle desen eşleştirme (performansa dikkat!).
  * `match_all`: Tüm dokümanları getirme.
  * `match_none`: Hiçbir dokümanı getirmeme.
* **(20 dk) Sorguları Birleştirme: `bool` Sorgusu**
  * En sık kullanılan ve en güçlü sorgu birleştirme aracı.
  * Kullanıldığı Kısım ve Anlamı:
    * `must`: Tüm alt sorgular eşleşmeli (AND). Query context'te skorlamaya katkıda bulunur.
    * `filter`: Tüm alt sorgular eşleşmeli (AND). Filter context'te çalışır, skorlamaya etkisi olmaz, cache'lenebilir.
    * `should`: Alt sorgulardan en az biri eşleşmeli (OR). `minimum_should_match` parametresi. Query context'te skorlamaya katkıda bulunur.
    * `must_not`: Hiçbir alt sorgu eşleşmemeli (NOT). Filter context'te çalışır.
  * İç içe `bool` sorguları ile karmaşık mantıklar oluşturma.
* **Pratik Uygulama (Kibana Dev Tools):**
  * `products` index'inde:
    * Adında "laptop" geçen ürünleri `match` ile arama.
    * Fiyatı 1000-2000 TL arasında olan ürünleri `range` ile `filter` context'inde arama.
    * Hem "gaming" etiketine sahip (`term`) hem de fiyatı 5000 TL'den yüksek (`range`) olan ürünleri `bool` sorgusu ( `must` veya `filter` kullanarak) ile arama.
    * Açıklamasında "hızlı işlemci" geçen VEYA adında "yeni nesil" geçen ürünleri `bool` sorgusu (`should` kullanarak) ile arama.

#### **Modül 4: İleri Arama, Aggregations ve Elastic Stack Ekosistemi (85 Dakika)**

* **(20 dk) Arama Sonuçlarını Yönetme ve İyileştirme**
  * Sayfalama: `from` ve `size`. (Deep pagination sorunlarına kısa bir değinme: `search_after`)
  * Sıralama: `sort` (alan adı, `asc`/`desc`, `_score`'a göre). Birden fazla alana göre sıralama.
  * Kaynak Filtreleme: `_source` (belirli alanları getirme/getirmeme).
  * Highlighting: Arama terimlerinin sonuçlarda vurgulanması.
  * Kısa bir değinme: `explain` API'si ile skorlamanın nasıl yapıldığını anlama.
* **(30 dk) Aggregations: Veriyi Gruplama ve Analiz Etme**
  * SQL'deki `GROUP BY` ve aggregate fonksiyonlarının (SUM, COUNT vb.) Elasticsearch'teki karşılığı.
  * **Temel Aggregation Türleri:**
    * **Metric Aggregations:** Sayısal değerler üzerinde hesaplama yapar.
      * `min`, `max`, `avg`, `sum`
      * `stats` (hepsini birden verir)
      * `cardinality` (benzersiz değer sayısını tahmin eder - HyperLogLog++)
      * `percentiles`, `percentile_ranks`
    * **Bucket Aggregations:** Dokümanları belirli kriterlere göre gruplara (bucket'lara) ayırır.
      * `terms`: Bir alandaki benzersiz değerlere göre gruplama (ör: kategorilere göre ürün sayısı). `size`, `order`.
      * `range`, `date_range`: Belirtilen aralıklara göre gruplama.
      * `histogram`, `date_histogram`: Sabit aralıklara göre gruplama (ör: fiyat aralıkları, aylık satışlar). `interval`.
      * `filter` / `filters`: Bir veya daha fazla filtreye uyan dokümanları ayrı bucket'lara koyma.
  * **İç İçe Aggregation'lar (Sub-aggregations):** Bir bucket aggregation sonucunu başka bir aggregation (metric veya bucket) ile daha detaylandırma. (Ör: Her kategorideki ürünlerin ortalama fiyatı).
* **Pratik Uygulama (Kibana Dev Tools):**
  * `products` index'inde:
    * Her bir `tag` için kaç ürün olduğunu `terms` aggregation ile bulma.
    * Tüm ürünlerin ortalama, minimum ve maksimum fiyatlarını `avg`, `min`, `max` (veya `stats`) aggregation ile bulma.
    * Her bir `tag` için ortalama ürün fiyatını iç içe aggregation ( `terms` bucket, içinde `avg` metric) ile bulma.
    * Ürünlerin oluşturulma tarihlerine göre aylık dağılımını `date_histogram` ile bulma.
* **(20 dk) Elastic Stack (ELK/Elastic Stack) Ekosistemine Genel Bakış**
  * **Kibana:** Veri görselleştirme ve keşif aracı.
    * **Discover:** Ham veriyi interaktif olarak inceleme, filtreleme, arama.
    * **Visualize Library:** Farklı grafik türleri (bar, line, pie, map vb.) oluşturma.
    * **Dashboard:** Birden fazla görselleştirmeyi tek bir ekranda toplama.
    * (Kısa bir demo ile Kibana'da basit bir görselleştirme ve dashboard oluşturma adımları gösterilebilir.)
  * **Logstash:** Sunucu taraflı veri işleme hattı. Farklı kaynaklardan veri toplama, dönüştürme (transform/enrich) ve Elasticsearch'e (veya başka hedeflere) gönderme.
  * **Beats:** Hafif, tek amaçlı veri göndericileri (data shippers).
    * Filebeat (log dosyaları), Metricbeat (sistem/servis metrikleri), Packetbeat (ağ verileri), Winlogbeat (Windows event logları) vb.
  * Stack'in genel mimarisi ve bileşenlerin birbiriyle nasıl çalıştığına dair basit bir şema.
* **(15 dk) Kapanış: Özet, Production İpuçları ve Soru-Cevap**
  * Eğitimde öğrenilen ana konuların hızlı bir özeti.
  * **Production'a Geçişte Dikkat Edilmesi Gerekenler (Kısa Başlıklar):**
    * Güvenlik (X-Pack Security'nin etkinleştirilmesi, rol tabanlı erişim).
    * Monitoring (Cluster ve node sağlığının izlenmesi).
    * Index Lifecycle Management (ILM) (Zamana bağlı veriler için otomatik index yönetimi).
    * Shard ve replica planlaması, kapasite planlaması.
    * Backup ve restore stratejileri.
  * Öğrenmeye devam etmek için kaynaklar: Resmi Elasticsearch dokümantasyonu, Elastic Blog, Elastic Community.
  * Katılımcıların sorularının yanıtlanması.

Bu ders içeriği, katılımcıların hem teorik bilgi edinmelerini hem de pratik beceriler kazanmalarını hedeflemektedir. Sizin deneyiminizle bu içeriği zenginleştireceğinizden ve katılımcılar için unutulmaz bir eğitim sunacağınızdan eminim. Başarılar dilerim!
