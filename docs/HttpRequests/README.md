# Elasticsearch Eğitimi - HTTP Request Örnekleri

Bu dizin, Elasticsearch eğitimi textbook'undaki tüm HTTP request örneklerini `.http` formatında içerir. Bu dosyalar, Kibana Dev Tools'a alternatif olarak çeşitli HTTP client araçlarıyla kullanılabilir.

## Dosya Listesi

| Dosya | İçerik | Örnek Sayısı |
|-------|--------|-------------|
| `Section01.http` | Cluster health, node bilgileri, index listesi | 3 request |
| `Section02.http` | Veri indexleme, mapping, CRUD işlemleri, bulk API | 12+ request grubu |
| `Section03.http` | Query DSL örnekleri, arama sorguları | 25+ request |
| `Section04.http` | Aggregation örnekleri, analiz sorguları | 17+ request |

## Kullanım Yöntemleri

### 1. VS Code REST Client Uzantısı (Önerilen)

**Kurulum:**

1. VS Code'da Extensions (Ctrl+Shift+X) menüsünü açın
2. "REST Client" araması yapın (yayıncı: Huachao Mao)
3. Uzantıyı yükleyin

**Kullanım:**

1. Herhangi bir `.http` dosyasını VS Code'da açın
2. İstediğiniz request'in üzerindeki `Send Request` linkine tıklayın
3. Sonuçlar yan panelde görüntülenecektir

### 2. IntelliJ IDEA HTTP Client

IntelliJ IDEA, WebStorm ve diğer JetBrains IDE'leri `.http` dosyalarını native olarak destekler.

**Kullanım:**

1. `.http` dosyasını IDE'da açın
2. Request'in yanındaki run (▶️) butonuna tıklayın

### 3. Curl ile Manuel Çevirme

HTTP request'leri curl komutlarına dönüştürmek için:

```bash
# Örnek: GET request
curl -X GET "http://localhost:9200/_cluster/health?pretty"

# Örnek: POST request with JSON body
curl -X POST "http://localhost:9200/products/_search" \
  -H "Content-Type: application/json" \
  -d '{
    "query": {
      "match_all": {}
    }
  }'
```

### 4. Postman

Postman'e import etmek için:

1. File > Import menüsünü açın
2. Raw text olarak `.http` dosyasının içeriğini yapıştırın
3. Postman otomatik olarak request'leri tanıyacaktır

## Ön Gereksinimler

Bu request'leri çalıştırmadan önce:

1. **Elasticsearch cluster'ının çalışıyor olması**

   ```bash
   # Docker Compose ile
   cd docker
   docker-compose up -d
   ```

2. **Sample data'nın yüklenmiş olması**
   - `.NET uygulamasını çalıştırarak sample data'yı yükleyebilirsiniz
   - Veya textbook'daki örnekleri takip ederek manuel olarak yükleyebilirsiniz

## Dosya Formatı Açıklaması

Her `.http` dosyası şu formatı kullanır:

```http
### Request Açıklaması
HTTP_METHOD http://localhost:9200/endpoint
Content-Type: application/json

{
  "json": "body"
}
```

- `###` ile başlayan satırlar yorum/açıklama
- `HTTP_METHOD` kısmı: GET, POST, PUT, DELETE
- Boş satır request'leri ayırır
- JSON body varsa Content-Type header'ı eklenir

## Elasticsearch Bağlantı Ayarları

Varsayılan olarak tüm request'ler `http://localhost:9200` adresini kullanır. Farklı bir adres kullanıyorsanız:

1. VS Code'da Find & Replace (Ctrl+H) kullanarak tüm dosyalarda `localhost:9200` kısmını değiştirin
2. Veya her request'i elle düzenleyin

## Güvenlik

Production ortamında:

- HTTPS kullanın (`https://` ile başlayan URL'ler)
- Authentication header'ları ekleyin:

  ```http
  Authorization: Basic dXNlcjpwYXNzd29yZA==
  # veya
  Authorization: ApiKey base64encodedkey
  ```

## Sorun Giderme

**Elasticsearch bağlantı hatası:**

- Elasticsearch'ün çalıştığını kontrol edin: `curl http://localhost:9200`
- Port numarasının doğru olduğunu kontrol edin
- Firewall ayarlarını kontrol edin

**CORS hatası:**

- Elasticsearch `elasticsearch.yml` dosyasına CORS ayarları ekleyin:

  ```yaml
  http.cors.enabled: true
  http.cors.allow-origin: "*"
  http.cors.allow-headers: "Content-Type, Authorization"
  ```

**Index bulunamadı hatası:**

- İlgili index'in oluşturulduğunu kontrol edin
- Sample data'nın yüklendiğini kontrol edin

## Daha Fazla Bilgi

- [Elasticsearch REST API Documentation](https://www.elastic.co/guide/en/elasticsearch/reference/current/rest-apis.html)
- [VS Code REST Client Documentation](https://marketplace.visualstudio.com/items?itemName=humao.rest-client)
- [HTTP Request Format Specification](https://tools.ietf.org/html/rfc7230)
