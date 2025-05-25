# Elasticsearch Eğitimi - Log Simülatörü

Bu uygulama, Elasticsearch eğitimi için geliştirilmiş bir .NET 9.0 ASP.NET Core Razor Pages uygulamasıdır. Eğitim sırasında kullanılacak olan log verilerini otomatik olarak üretir ve eğitim için gerekli index'leri oluşturur.

## 🎯 Özellikler

- **Index Template Yönetimi:** `application_logs` template'ini otomatik oluşturma
- **Products Index:** Eğitimde kullanılacak ürün verilerini içeren index oluşturma
- **Örnek Veri Yükleme:** Products index'ine örnek ürün verilerini yükleme
- **Otomatik Log Üretimi:** Arka plan servisi ile sürekli log verisi üretme
- **Real-time Monitoring:** Elasticsearch bağlantı durumu ve cluster health izleme

## 🔧 Gereksinimler

- .NET 9.0 SDK
- Elasticsearch 7.17+ (varsayılan: `http://localhost:9200`)
- Kibana (opsiyonel, önerilir: `http://localhost:5601`)

## 🚀 Kurulum ve Çalıştırma

### 1. Elasticsearch ve Kibana Kurulumu

Docker kullanarak:

```bash
# Docker Compose ile Elasticsearch ve Kibana'yı başlatın
docker run -d --name elasticsearch \
  -p 9200:9200 -p 9300:9300 \
  -e "discovery.type=single-node" \
  -e "xpack.security.enabled=false" \
  docker.elastic.co/elasticsearch/elasticsearch:7.17.15

docker run -d --name kibana \
  -p 5601:5601 \
  -e "ELASTICSEARCH_HOSTS=http://host.docker.internal:9200" \
  docker.elastic.co/kibana/kibana:7.17.15
```

### 2. Uygulama Çalıştırma

```bash
# Proje klasörüne gidin
cd src/ElasticTraining

# Bağımlılıkları geri yükleyin
dotnet restore

# Uygulamayı çalıştırın
dotnet run
```

Uygulama `https://localhost:5001` adresinde çalışacaktır.

## 📋 Kullanım

### Ana Sayfa İşlevleri

1. **Elasticsearch Bağlantı Kontrolü**
   - Elasticsearch'ün durumunu kontrol eder
   - Cluster health bilgilerini gösterir

2. **Index Template Oluşturma**
   - "Application Logs Template Oluştur" butonuna tıklayın
   - `application_logs-*` desenindeki index'ler için mapping oluşturur

3. **Products Index Oluşturma**
   - "Products Index Oluştur" butonuna tıklayın
   - Eğitimde kullanılacak ürün verilerini için index oluşturur

4. **Örnek Veri Yükleme**
   - "Örnek Ürün Verilerini Yükle" butonuna tıklayın
   - 5 adet örnek ürün verisi yükler

5. **Log Üretimi**
   - "Log Üretimini Başlat" ile otomatik log üretimini başlatın
   - Loglar `application_logs-YYYY-MM-DD` formatında index'lere yazılır
   - INFO (%70), WARN (%20), ERROR (%10) seviyelerinde loglar üretir

## 📊 Üretilen Veri Yapıları

### Application Logs
- **Index Pattern:** `application_logs-*`
- **Alanlar:** timestamp, service_name, level, message, exception, http_status_code, vb.
- **Günlük Volume:** 2-10 saniye aralıklarla sürekli log üretimi

### Products Index
- **Index Name:** `products`
- **Alanlar:** sku, name, description, price, stock_quantity, category, tags, created_date, is_active
- **Örnek Veriler:** 5 adet farklı kategoriden ürün

## 🔍 Elasticsearch Sorguları

Kibana Dev Tools (`http://localhost:5601/app/dev_tools#/console`) ile test edebileceğiniz örnek sorgular:

```bash
# Index'leri listele
GET /_cat/indices?v

# Application loglarını görüntüle
GET /application_logs-*/_search

# Products verilerini görüntüle
GET /products/_search

# Template'leri listele
GET /_index_template

# Son 1 saatteki ERROR logları
GET /application_logs-*/_search
{
  "query": {
    "bool": {
      "must": [
        {"term": {"level": "ERROR"}},
        {"range": {"@timestamp": {"gte": "now-1h"}}}
      ]
    }
  }
}
```

## ⚙️ Konfigürasyon

`appsettings.json` dosyasında Elasticsearch ayarlarını yapılandırabilirsiniz:

```json
{
  "Elasticsearch": {
    "Uri": "http://localhost:9200",
    "Username": "",
    "Password": ""
  }
}
```

## 🎓 Eğitim İçin Notlar

Bu uygulama aşağıdaki Elasticsearch konularının pratik edilmesi için tasarlanmıştır:

- Index ve Mapping yönetimi
- Document CRUD operasyonları
- Query DSL ile sorgulama
- Aggregations ve analytics
- Log analizi ve monitoring
- Index templates ve lifecycle management

## 🤝 Katkıda Bulunma

Eğitim materyallerini geliştirmek için katkılarınızı bekliyoruz!

## 📄 Lisans

Bu proje eğitim amaçlı geliştirilmiştir.
