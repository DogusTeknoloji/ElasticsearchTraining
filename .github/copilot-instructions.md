# Proje Talimatları: Elasticsearch Eğitimi Materyalleri

## 1. Projeye Genel Bakış

Bu proje, şirket içi kıdemli full stack yazılım geliştiricilere yönelik kapsamlı bir Elasticsearch eğitimi için materyaller geliştirmeyi amaçlamaktadır. Temel hedef, katılımcıların Elasticsearch'ü etkin bir şekilde anlamalarını ve kullanmalarını sağlamaktır.

## 2. Temel Hedef

Katılımcıların Elasticsearch'ün temel mimarisini, anahtar kavramlarını anlamalarını, veri indeksleme (indexing), sorgulama (searching), analiz (aggregations) yeteneklerini kavramalarını ve bu bilgileri pratik senaryolarda (özellikle log yönetimi odaklı) uygulayabilir hale gelmelerini sağlamaktır.

## 3. Hedef Kitle

Şirket içinde çalışan, orta ve kıdemli seviye Full Stack Yazılım Geliştiriciler. Bu kitlenin REST API'ler, JSON, temel veritabanı ve dağıtık sistem kavramlarına aşina olduğu varsayılmaktadır.

## 4. Eğitim Formatı

* **Süre:** 1 günlük, toplam 6 saatlik aktif eğitim.
* **Tür:** Şirket içi iç eğitim.
* **Yaklaşım:** Teorik bilgilerin pratik uygulamalarla pekiştirildiği, interaktif bir eğitim.

## 5. Bugüne Kadar Oluşturulan Temel Çıktılar (Artifacts)

Bu projede şu ana kadar aşağıdaki iki ana doküman oluşturulmuştur:

1. **`elasticsearch_egitim_icerigi_v2_md_kaynak` (Elasticsearch Eğitimi: Kapsamlı Ders İçeriği - Markdown Kaynak):**
    * **Açıklama:** Eğitmenin kullanacağı, 6 saatlik ders akışını, modül detaylarını, konu başlıklarını, ayrılan süreleri (eğitim planlaması için) ve pratik uygulama adımlarını içeren detaylı bir ders planıdır.
    * **Dil:** Türkçe.
    * **Format:** Markdown.
    * **Amacı:** Eğitmenin dersi yapılandırması, slayt ve diğer materyalleri hazırlaması için bir yol haritası sunmak.

2. **`elasticsearch_ders_kitabi_v1` (Elasticsearch Macerası: Bir Geliştiricinin Kılavuzu - Markdown):**
    * **Açıklama:** Yukarıdaki ders içeriğini temel alan, öğrencilere yönelik hazırlanmış bir ders kitabıdır.
    * **Dil:** Anlatım dili Türkçe, teknik terimler, kod örnekleri, index/alan adları İngilizce'dir.
    * **Ton:** Öğrenci dostu, samimi, yer yer esprili ve ilgi çekici bir dil kullanılmıştır.
    * **Format:** Markdown.
    * **Amacı:** Öğrencilere ders öncesinde ön okuma materyali olarak sunmak, ders sırasında ve sonrasında başvuru kaynağı olarak kullanmalarını sağlamak. Log yönetimi senaryolarına özel vurgu yapılmıştır.

## 6. İçerik ve Stil Kuralları (Özellikle Ders Kitabı İçin)

* **Anlatım Dili:** Türkçe. Akıcı, anlaşılır ve samimi bir üslup tercih edilmelidir.
* **Teknik Terimler ve Kod Örnekleri:** Tüm Elasticsearch komutları, API endpoint'leri, index adları, alan adları (fields), JSON yapıları ve kod örnekleri **İngilizce** olmalıdır. Bu, uluslararası standartlara ve resmi dokümantasyona uyumu sağlar.
* **Ton:** Eğlenceli, esprili ve motive edici bir dil kullanılmalı, ancak teknik doğruluktan ödün verilmemelidir. "Cesur geliştirici", "veri okyanusu" gibi ifadeler bu tonu yansıtır.
* **Pratik Odaklılık:** Teorik bilgiler pratik örneklerle desteklenmelidir. Özellikle şirket içi yaygın kullanım alanı olan **log yönetimi ve analizi** senaryolarına özel örnekler ve vurgular içermelidir.
* **Resmi Dokümantasyona Atıflar:** Öğrencilerin daha fazla bilgi edinebilmeleri için ilgili ve önemli noktalarda Elasticsearch resmi dokümantasyonuna yönlendirmeler (linkler) eklenmelidir.
* **Öğrenci Dostu Yaklaşım:** Karmaşık konular basitleştirilerek anlatılmalı, "Neden?" sorularına cevap verilmelidir.
* **Markdown Formatı:** Tüm metin tabanlı çıktılar (ders kitabı, ders içeriği) Markdown formatında olmalıdır.

## 7. Projenin Mevcut Durumu

* Detaylı ders içeriği (`elasticsearch_egitim_icerigi_v2_md_kaynak`) oluşturulmuştur.
* Bu ders içeriğini temel alan, loglama örneklerini de içeren kapsamlı bir öğrenci ders kitabı (`elasticsearch_ders_kitabi_v1`) tamamlanmıştır.

## 8. LLM (Yapay Zeka Modeli) İçin Bağlam Notları

* Bu talimat dosyası, bu projeyle ilgili gelecekteki tüm etkileşimlerde LLM'e bağlam sağlamak için kullanılacaktır.
* LLM'den istenen güncellemeler veya yeni içerikler, yukarıda belirtilen stil, ton, dil ve içerik kurallarına uygun olmalıdır.
* Özellikle "log yönetimi" senaryolarının şirket içi eğitim için önemli olduğu unutulmamalıdır.
* Yeni içerik veya değişiklik taleplerinde, mevcut dokümanların (`elasticsearch_ders_kitabi_v1` ve `elasticsearch_egitim_icerigi_v2_md_kaynak`) en güncel versiyonları referans alınmalıdır.
* Projenin genel amacı, geliştiricilerin Elasticsearch'ü etkili bir şekilde öğrenmelerini ve kullanmalarını sağlamaktır.

## 9. Ders İçin Pratik Uygulama: ".NET Log Simülatörü"

Ders sırasında teorik bilgileri pekiştirmek ve canlı verilerle çalışabilmek amacıyla, log yazımını simüle eden bir .NET uygulaması geliştirilecektir. Bu uygulama, katılımcıların Elasticsearch'ü dinamik bir ortamda test etmelerini sağlayacaktır.

* **Teknoloji:** Uygulama, **.NET 9.0** ve **ASP.NET Core 9.0 Razor Pages** kullanılarak geliştirilecektir.
* **Amaç:** Uygulamanın temel amacı, `application_logs-*` desenine uyan index'lere sürekli olarak log verisi göndermektir. Bu sayede katılımcılar, akan veri üzerinde sorgulama, analiz ve görselleştirme pratikleri yapabilecektir.
* **Solution Dosyası:** `src/ElasticTraining.sln` 
* **Proje Adı:** `src/ElasticTraining/ElasticTraining.csproj`

### Uygulamanın Yetenekleri

Uygulama, eğitim sürecini kolaylaştıracak şu işlevlere sahip olacaktır:

1.  **Elasticsearch Yapılandırması:**
  * **Index Template Yönetimi:** Aşağıda belirtilen mapping'e sahip `application_logs` index template'ini tek bir tıklama ile Elasticsearch'e ekleyebilecektir. Bu template, `application_logs-*` ile başlayan tüm index'lere otomatik olarak uygulanacaktır.
  * **Index Oluşturma:** Eğitim kitabında (`Section02.md`) detaylandırılan `products` index'ini ve mapping'ini oluşturabilecektir.
2.  **Veri Yönetimi:**
  * **`products` Veri Seti:** `products` index'ine, eğitimde kullanılacak standart bir veri setini (`Section02.md` dosyasında belirtilen örnekler gibi) yükleyebilecek bir arayüz sunacaktır.
  * **Arka Plan Log Yazma Servisi:** Başlatılıp durdurulabilen bir arka plan hizmeti, `application_logs` index'ine sürekli olarak çeşitli seviyelerde (INFO, WARN, ERROR) loglar yazacaktır.

### `application_logs` Index Template Mapping'i

Uygulama tarafından oluşturulacak olan index template'i, log verilerinin doğru ve verimli bir şekilde indekslenmesi için aşağıdaki mapping'i kullanacaktır. Bu mapping, `dynamic` özelliği `false` olarak ayarlanarak, sadece tanımlanan alanların kabul edilmesini sağlar ve "mapping explosion" riskini ortadan kaldırır. `stack_trace` gibi uzun metinlerin aranmasına gerek olmadığı için `index: false` ayarı ile performanstan tasarruf edilir.

```json
{
  "mappings": {
    "dynamic": "false",
    "properties": {
      "@timestamp": { "type": "date" },
      "correlation_id": { "type": "keyword" },
      "service_name": { "type": "keyword" },
      "level": { "type": "keyword" },
      "thread_name": { "type": "keyword" },
      "logger_name": { "type": "keyword" },
      "host_ip": { "type": "ip" },
      "message": { "type": "text" },
      "exception": {
        "type": "object",
        "properties": {
          "type": { "type": "keyword" },
          "message": { "type": "text" },
          "stack_trace": { 
            "type": "text",
            "index": false 
          },
          "inner_exception": {
            "type": "object",
            "properties": {
              "type": { "type": "keyword" },
              "message": { "type": "text" },
              "stack_trace": { 
                "type": "text", 
                "index": false 
              }
            }
          }
        }
      },
      "http_status_code": { "type": "integer" },
      "response_time_ms": { "type": "long" }
    }
  }
}
```

## 10. Önemli Not

Bu talimatlar, projenin tutarlılığını ve kalitesini korumak için kritik öneme sahiptir. LLM'den gelen yanıtların bu talimatlara uygun olması beklenmektedir.
