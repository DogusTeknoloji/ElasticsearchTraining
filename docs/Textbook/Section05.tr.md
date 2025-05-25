# Bölüm 5: Elastic Stack Ailesi ve Ötesi

Elasticsearch genellikle tek başına kullanılmaz. Çoğu zaman, **Elastic Stack** (eskiden ELK Stack olarak bilinirdi) adı verilen bir ailenin parçasıdır. Bu aile, veri toplama, işleme, depolama, arama, analiz ve görselleştirme gibi uçtan uca çözümler sunar. Özellikle log yönetimi ve analizi söz konusu olduğunda, bu bileşenlerin uyumu hayati önem taşır. "Bir elin nesi var, iki elin sesi var" misali, bu araçlar birlikte çalıştığında çok daha güçlü olurlar.

## 5.1 Kibana: Veriyle Dans Etmenin Görsel Yolu

Eğer Elasticsearch veritabanı ve arama motoruysa, **Kibana** da onun kullanıcı arayüzü, gösterge paneli ve veri kaşifidir. Kibana sayesinde:

* **Dev Tools:** Zaten bolca kullandığımız, Elasticsearch'e doğrudan sorgu göndermemizi sağlayan arayüz.
* **Discover:** Index'lerinizdeki ham veriyi (örneğin, `application_logs-*` index'lerindeki logları) interaktif bir şekilde keşfedebilir, filtreleyebilir ve arayabilirsiniz. "Acaba şu saatteki hata logunda ne yazıyordu?" sorusunun cevabı burada.
* **Visualize Library:** Elasticsearch aggregation'larından (örneğin, [Bölüm 4.6: Pratik Zamanı: `application_logs` Verimizi Analiz Edelim!](../Textbook/Section04.tr.md#46-pratik-zamanı-application_logs-verimizi-analiz-edelim)'daki log analizlerinden) elde ettiğiniz verileri kullanarak çeşitli grafikler (saatlik hata sayıları için çizgi grafik, servis bazlı log seviyeleri için pasta grafik vb.) oluşturabilirsiniz.
* **Dashboard:** Oluşturduğunuz farklı görselleştirmeleri tek bir ekranda toplayarak interaktif gösterge panelleri (dashboard'lar) tasarlayabilirsiniz. Sistem sağlığı, hata oranları, en çok log üreten servisler gibi metrikleri canlı olarak izleyebilirsiniz.
* **Maps:** Coğrafi verilerinizi harita üzerinde görselleştirebilir ve analiz edebilirsiniz.
* **Machine Learning:** Anomali tespiti (örneğin, log sayılarındaki ani artışlar), tahminleme gibi makine öğrenmesi özelliklerini kullanabilirsiniz (X-Pack lisansı gerektirebilir).
* **Stack Management:** Index yönetimi (ILM ile log index'lerinin yaşam döngüsünü yönetme), kullanıcı rolleri, güvenlik ayarları gibi cluster yönetim işlemlerini yapabilirsiniz.

Kibana, Elasticsearch'teki verilerinize hayat verir ve onları herkesin anlayabileceği bir şekilde sunmanızı sağlar. Kısacası, Elasticsearch'ün "görünen yüzü"dür.

## 5.2 Logstash: Veri Hattınızın Çalışkan İşçisi

**Logstash**, sunucu taraflı bir veri işleme hattıdır (data processing pipeline). Farklı kaynaklardan (log dosyaları, veritabanları, mesaj kuyrukları vb.) veri toplar, bu veriyi dönüştürür (transform/enrich - örneğin, IP adresinden coğrafi konum bilgisi ekleme, yapısal olmayan log satırlarını `grok` filtresi ile parse ederek `service_name`, `level`, `message` gibi alanlara ayırma) ve ardından Elasticsearch'e (veya başka hedeflere) gönderir.

Logstash'in temel yapısı şöyledir:

* **Input Plugin'leri:** Verinin nereden alınacağını tanımlar (ör: `file` ile sunucudaki log dosyalarını okuma, `beats` ile Filebeat'ten veri alma, `syslog` ile ağ üzerinden log toplama).
* **Filter Plugin'leri:** Veri üzerinde çeşitli dönüşümler, zenginleştirmeler veya ayrıştırmalar yapar (ör: `grok` ile yapısal olmayan logları parse etme, `mutate` ile alanları değiştirme/ekleme/silme, `date` ile timestamp formatını düzeltme, `geoip` ile IP'den konum bulma). "Veriyi biraz yontalım, şekle sokalım, Elasticsearch'e hazır hale getirelim" dediğimiz yer.
* **Output Plugin'leri:** İşlenmiş verinin nereye gönderileceğini tanımlar (ör: `elasticsearch`'e yazma, `file`'a kaydetme, `s3`'e arşivleme).

Logstash, özellikle farklı formatlardaki logları standartlaştırarak Elasticsearch'e göndermeden önce işlemek için çok güçlüdür.

## **5.3 Beats: Hafif Siklet Veri Taşıyıcıları**

**Beats**, tek bir amaca hizmet eden, hafif (lightweight) veri göndericileridir (data shippers). Genellikle sunucularınıza veya uç noktalara kurularak belirli türdeki verileri toplar ve doğrudan Elasticsearch'e veya işlenmek üzere Logstash'e gönderirler. Loglama senaryolarında en sık kullanılan Beat, **Filebeat**'tir.

* **Filebeat:** Sunucularınızdaki log dosyalarını (`/var/log/app.log`, `nginx_access.log` vb.) anlık olarak takip eder ve yeni eklenen satırları toplar. "Log'ları bana bırakın, ben taşırım!" der.
* **Metricbeat:** Sistem (CPU, RAM, disk, ağ) ve servis (Apache, Nginx, MySQL, Docker vb.) metriklerini toplar.
* **Packetbeat:** Ağ trafiğini dinleyerek uygulama protokolleri (HTTP, DNS, MySQL vb.) hakkında bilgi toplar.
* **Winlogbeat:** Windows event log'larını toplar.
* **Auditbeat:** Linux audit framework verilerini ve dosya bütünlüğü olaylarını toplar.
* **Heartbeat:** Servislerin "ayakta" olup olmadığını düzenli aralıklarla kontrol eder.

Beats, kaynak tüketimi düşük olduğu için geniş çaplı dağıtımlarda (yüzlerce, binlerce sunucuda) log ve metrik toplamak için idealdir.

## **5.4 Elastic Stack Mimarisi: Takım Oyunu**

Bu üç ana bileşen (Elasticsearch, Logstash, Kibana) ve Beats ailesi, genellikle aşağıdaki gibi bir mimaride birlikte çalışır:

`Beats (Örn: Filebeat logları toplar) -> Logstash (Opsiyonel: Logları parse eder, zenginleştirir) -> Elasticsearch (Logları depolar, indeksler, aranabilir hale getirir) -> Kibana (Logları görselleştirir, analiz eder, dashboard'lar sunar)`

Bazen Beats veriyi doğrudan Elasticsearch'e de gönderebilir (özellikle loglar zaten yapısal ise veya Elasticsearch Ingest Node'ları ile basit dönüşümler yapılıyorsa). Bu esnek yapı, farklı ihtiyaçlara göre uyarlanabilir.

Elastic Stack, sadece log analizi (meşhur ELK Stack) için değil, aynı zamanda güvenlik analitiği (SIEM), uygulama performans izleme (APM), iş zekası ve daha birçok alanda güçlü çözümler sunar. Tüm bu bileşenler hakkında daha fazla bilgi için [Elastic Stack Resmi Sayfası'na](https://www.elastic.co/elastic-stack/) bakabilirsiniz.

---
[<- Önceki Bölüm: Bölüm 04](Section04.tr.md) | [Sonraki Bölüm: Bölüm 06 ->](Section06.tr.md)
