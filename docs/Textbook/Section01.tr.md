# Bölüm 1: Elasticsearch Dünyasına Merhaba De!

Bu bölümde, Elasticsearch'ün ne olduğunu, hangi sorunlara çözüm getirdiğini, temel mimarisini ve en önemlisi o meşhur hızlı aramalarını nasıl yaptığını öğreneceğiz. Yani, "Bu alet de neyin nesi ve nasıl bu kadar iyi arama yapıyor?" sorularının cevabını arayacağız.

## 1.1 Maceraya Giriş: Sen, Ben ve Elasticsearch

Merhaba kaşif ruhlu geliştirici! Bu satırlara ulaştığına göre, Elasticsearch denen bu popüler ve güçlü aracın sırlarını çözmeye hazırsın demektir. Belki de adını sıkça duyuyorsun, belki de bir projede karşına çıktı ve "Bu da neymiş?" diye merak ettin. Belki de sadece yeni bir şeyler öğrenme hevesin seni buraya getirdi. Sebebin ne olursa olsun, bu macerada yalnız değilsin!

Bu kitapçık, senin Elasticsearch ile arandaki buzları kırmak, onu daha yakından tanımanı sağlamak ve "Acaba ben bunu nerede, nasıl kullanırım?" sorularına cevap bulmana yardımcı olmak için kaleme alındı. Amacımız, teknik detaylarda boğulmadan, anlaşılır bir dille ve bolca "Aha! Demek bu yüzdenmüş!" anıyla dolu bir öğrenme deneyimi sunmak.

Peki, sen kimsin? Elasticsearch ile daha önce bir tanışıklığın oldu mu? Ondan beklentilerin neler? Bu soruların cevapları sende saklı olsa da, biz bu kitapçığı her seviyeden meraklı geliştiricinin faydalanabileceği şekilde tasarlamaya çalıştık. İster Elasticsearch'e ilk adımını atıyor ol, ister mevcut bilgilerini tazelemek iste, burada kendine göre bir şeyler bulacağına eminiz.

Şimdi, kahveni (veya çayını, tercih senin!) hazırla, rahat bir koltuğa kurul ve Elasticsearch'ün veriyle dolu dünyasına doğru keyifli bir yolculuğa çıkmaya hazırlan. İlk durağımız: "Neden böyle bir şeye ihtiyacımız var ki?" sorusunun can alıcı cevapları!

**Bu Eğitim Hakkında:**

Bu eğitim materyali Elasticsearch 8.19.2 ve NEST 7.17.5 (.NET istemcisi) kullanılarak geliştirilmiştir. Burada ele alınan temel kavramlar, mimari ve arama prensipleri hem Elasticsearch 8.x hem de 9.x sürümleri için geçerlidir. Production ortamı değerlendirmeleri ve versiyon karşılaştırması için Kapanış bölümüne (Bölüm 6) bakınız.

## 1.2 "Neden Elasticsearch?" Yoksa Sen Hala `LIKE` mı Kullanıyorsun?

Ah, o meşhur `LIKE '%search_term%'` sorguları... Bir zamanlar hepimiz oradaydık. Veritabanında milyonlarca satır arasında bir kelime aramak, samanlıkta iğne aramaktan farksızdı, değil mi? Hele bir de kullanıcı "hem onu içersin hem de bunu içermesin ama şununla başlasın" gibi isteklerle gelince... İşte tam da bu noktada geleneksel ilişkisel veritabanları (RDBMS) biraz teklemeye başlıyor.

* **Performans Sorunları:** Veri büyüdükçe `LIKE` sorguları yavaşlar, kullanıcılar sabırsızlanır, sistemler alarm verir.
* **Alaka (Relevance) Eksikliği:** `LIKE` sadece "var" ya da "yok" der. Hangisi daha alakalı, hangisi daha önemli, pek umursamaz.
* **Dil Desteği ve Esneklik:** Farklı dillerdeki kelime kökleri, eş anlamlılar, yazım hataları... Bunlar `LIKE` için kabus gibidir.
* **Yapısal Olmayan Veri:** Günümüzde loglar, tweetler, ürün yorumları gibi metin tabanlı, yapısal olmayan (unstructured) veya yarı yapısal (semi-structured) verilerle uğraşıyoruz. Bunları RDBMS'te analiz etmek pek de keyifli değil. Özellikle uygulama logları, sunucu logları gibi devasa boyutlara ulaşabilen ve hızlıca aranması, analiz edilmesi gereken veriler için Elasticsearch biçilmiş kaftandır.

İşte Elasticsearch tam da bu tür dertlere derman olmak için geliyor. E-ticaret sitelerindeki akıllı ürün aramalarını, log analizi platformlarındaki anlık sorguları, anomali tespit sistemlerini düşün. Birçoğunun arkasında Elasticsearch veya benzeri bir teknoloji yatıyor.

## 1.3 Elasticsearch: Veri Dünyasının İsviçre Çakısı (ve Biraz da NoSQL Muhabbeti)

Peki, nedir bu Elasticsearch? Kısaca **ES** olarak da anılır (tembel geliştiriciler için).

* **Apache Lucene Üzerine Kurulu:** Kalbinde, yılların deneyimine sahip, güçlü bir arama kütüphanesi olan Apache Lucene var. Yani, temelleri sağlam.
* **Dağıtık (Distributed):** Veriyi birden fazla sunucuya dağıtarak hem performansı artırır hem de sistemin çökmesini zorlaştırır. Tek bir sunucuya "Tüm yumurtaları aynı sepete koyma" prensibi.
* **RESTful API:** HTTP üzerinden JSON formatında konuşur. Yani, en sevdiğin programlama diliyle kolayca iletişim kurabilirsin. `curl` ile bile sorgu atabilirsin, o derece!
* **Arama ve Analitik Motoru:** Sadece arama yapmakla kalmaz, aynı zamanda veriyi analiz etme, gruplama, istatistik çıkarma gibi konularda da oldukça yeteneklidir.
* **Gerçek Zamanlıya Yakın (Near Real-Time - NRT):** Veriyi ekledikten çok kısa bir süre sonra (genellikle 1 saniye içinde) aramalarda görünür hale gelir. "Hemen olsun şimdi olsun" diyenler için ideal.
* **Şema Esnek (Schema-Flexible) / Şemasız (Schemaless) Değil!:** Veriyi atarken bir şema belirtmek zorunda değilsin, ES senin için bir şema çıkarabilir (dynamic mapping). Ama production ortamında kendi şemanı (explicit mapping) oluşturman şiddetle tavsiye edilir. "Bırak dağınık kalsın" demek, ileride başını ağrıtabilir.

**Peki, Elasticsearch bir NoSQL Veritabanı mı?**

Bu soru sıkça karşımıza çıkar. Cevap hem evet hem de hayır gibi biraz. "Nasıl yani?" dediğini duyar gibiyim. Açıklayalım:

**NoSQL Nedir Kısaca?** "Not Only SQL" (Sadece SQL Değil) anlamına gelen NoSQL, geleneksel ilişkisel veritabanlarının (SQL tabanlı olanlar) bazı kısıtlamalarına alternatif olarak ortaya çıkmış geniş bir veritabanı kategorisidir. RDBMS'lerin katı şema yapısı, yatay ölçeklenme zorlukları ve belirli veri modellerine (tablolar, satırlar, sütunlar) bağımlılığı gibi durumlar, özellikle büyük veri (Big Data), yüksek trafikli uygulamalar ve esnek veri modelleri gerektiren senaryolarda NoSQL çözümlerini popüler hale getirmiştir.

NoSQL veritabanları genellikle şu özelliklerden bir veya birkaçını sunar:

* Esnek şemalar (veya şemasızlık)
* Yatay ölçeklenebilirlik (daha fazla sunucu ekleyerek kapasite artırma)
* Yüksek performans ve erişilebilirlik
* Farklı veri modelleri (doküman, anahtar-değer, kolon ailesi, graf)

**Elasticsearch ve NoSQL İlişkisi:**

Elasticsearch, temel olarak bir **arama ve analitik motorudur**. Ancak, veriyi **doküman (document)** odaklı bir şekilde sakladığı için (JSON dokümanları), NoSQL veritabanlarından biri olan **doküman veritabanlarına (document store)** çok benzer.

* **Doküman Odaklı:** Veriyi JSON dokümanları olarak saklar ve bu dokümanlar üzerinden işlem yapar. Bu, MongoDB gibi diğer doküman veritabanlarıyla benzer bir yaklaşımdır.
* **Esnek Şema:** Dynamic mapping özelliği sayesinde şema konusunda esneklik sunar, ancak dediğimiz gibi explicit mapping daha sağlıklıdır.
* **Yatay Ölçeklenebilirlik:** Shard'lar sayesinde yatay olarak ölçeklenebilir.
* **REST API:** Veriye erişim için HTTP tabanlı bir API sunar.

Bu özellikleriyle Elasticsearch, NoSQL ekosisteminin bir parçası olarak kabul edilebilir. Hatta birçok durumda, birincil NoSQL veritabanı olarak da kullanılabilir (özellikle arama ve analitik yetenekleri ön plandaysa). Ancak unutmamak gerekir ki, Elasticsearch'ün asıl gücü ve optimize edildiği alan **arama ve analizdir**. Geleneksel bir NoSQL doküman veritabanının sunduğu bazı transactional garantiler veya genel amaçlı veritabanı özellikleri Elasticsearch'te aynı seviyede olmayabilir.

Yani, evet, Elasticsearch bir NoSQL veritabanının birçok özelliğini taşır ve o kategoriye dahil edilebilir, ama o bir "arama motoru süper güçlerine sahip NoSQL doküman deposu" gibi bir şeydir. Amacına göre konumlandırmak en doğrusu.

**Elasticsearch Ne Değildir?**

Bu da önemli bir nokta. Elasticsearch her derde deva bir sihirli değnek değil. Özellikle birincil veri kaynağı (source of truth) olarak, yani geleneksel bir RDBMS'nin yerine geçecek şekilde tasarlanmamıştır. ACID garantileri RDBMS'ler kadar güçlü değildir. Genellikle verinin bir kopyasını alıp arama ve analiz için optimize eden bir katman olarak kullanılır.

**Nerelerde Kullanılır Bu Meret?**

* **Full-Text Search:** Web sitelerinde, uygulamalarda, ürün kataloglarında akıllı ve hızlı arama.
* **Log ve Metrik Analizi:** Sunucu logları, uygulama logları, performans metrikleri... Bunları toplayıp analiz etmek, sorunları tespit etmek için birebir (Observability). Şirket içinde en sık karşılaşacağınız kullanım alanlarından biri budur!
* **İş Zekası (Business Intelligence):** Büyük veri setleri üzerinde anlık analizler ve raporlamalar.
* **Güvenlik Analitiği (SIEM):** Güvenlik olaylarını toplayıp, şüpheli aktiviteleri tespit etme.
* **Coğrafi Veri Arama:** Harita üzerinde konum tabanlı aramalar ve analizler.

## **1.4 Temel Mimarî Kavramlar: Elasticsearch Alfabesi**

Elasticsearch ile konuşmaya başlamadan önce onun dilindeki bazı temel kelimeleri öğrenmemiz gerekiyor. Korkma, roket bilimi değil!

* **Document (Doküman):** Verinin Elasticsearch'teki en küçük birimi. JSON formatındadır. Şöyle düşün, RDBMS'teki bir satır (row) gibi.
  
  ```json
  {
      "id": 1,
      "product_name": "Smartphone X1000",
      "price": 799.99,
      "category": "Electronics"
  }
  ```

* **Index (İndeks):** Benzer özelliklere sahip dokümanların toplandığı yer. RDBMS'teki bir tablo (table) gibi düşünebilirsin. Örneğin, `products` adında bir index'in olabilir ve tüm ürün dokümanların burada saklanır. Benzer şekilde, uygulama loglarınız için `application_logs-2024-05-24` gibi tarih bazlı index'ler oluşturabilirsiniz.
  * *Not:* Eskiden index içinde `_type` diye bir kavram daha vardı ama artık tarih oldu (7.x ve sonrası sürümlerde tek bir index'te tek bir type oluyor, o da genellikle `_doc` olarak geçiyor). Yani, kafan karışmasın, artık daha basit.
* **Node (Düğüm):** Çalışan tek bir Elasticsearch sunucusu. Bir bilgisayarda çalışan ES programı bir node'dur.
  * *Node Tipleri:* Her node'un cluster içinde farklı görevleri olabilir: `master-eligible` (cluster'ı yönetir), `data` (veriyi tutar), `ingest` (veriyi işlemeden önce dönüştürür), `coordinating` (sorguları alır, dağıtır, sonuçları toplar), `machine learning` (özel ML işleri için). Başlangıçta hepsi bir arada olabilir ama büyük sistemlerde bu roller ayrılır.
* **Cluster (Küme):** Bir veya daha fazla node'un bir araya gelerek oluşturduğu yapı. Bu node'lar birlikte çalışır, veriyi ve iş yükünü paylaşır. Tek başına takılan bir node da bir cluster sayılır (single-node cluster).
* **Shard (Parça):** Bir index çok büyüdüğünde, onu daha küçük ve yönetilebilir parçalara bölmek gerekir. İşte bu parçalara shard denir. Shard'lar sayesinde Elasticsearch veriyi yatay olarak ölçekleyebilir (horizontal scaling) ve işlemleri paralelleştirebilir.
  * **Primary Shard:** Her doküman bir primary shard'a aittir. Bir index oluşturulurken kaç primary shard olacağı belirlenir (ve sonradan değiştirilemez, dikkat!).
* **Replica (Kopya):** Primary shard'ların kopyalarıdır. İki temel amacı vardır:
    1. **Yüksek Erişilebilirlik (High Availability):** Bir node çökerse ve o node'daki primary shard'lara erişilemezse, replica shard'lar devreye girer ve veri kaybı önlenir.
    2. **Okuma Performansını Artırma:** Arama sorguları hem primary hem de replica shard'lar üzerinden yapılabilir, bu da okuma kapasitesini artırır.
  * Bir replica shard asla kendi primary shard'ı ile aynı node üzerinde bulunmaz. Akıllıca, değil mi?

**Şimdi Biraz El Kirletme Zamanı! (Kibana Dev Tools ile İlk Tanışma)**

Teori güzel ama pratik olmadan olmaz. Elasticsearch ve Kibana'yı (Docker ile) kurduğunu varsayıyoruz. Kibana arayüzünde "Management" > "Dev Tools" diye bir bölüm göreceksin. Burası bizim Elasticsearch ile konuşacağımız, komutlar göndereceğimiz oyun alanımız.

Aşağıdaki komutları Dev Tools'a yapıştırıp yeşil "play" butonuna basarak çalıştırabilirsin:

1. **Cluster Sağlığını Kontrol Et:**

    ```http
    GET /_cluster/health
    ```

    Bu komut sana cluster'ının genel durumu hakkında bilgi verir (`status`: green, yellow, red). Green ise her şey yolunda demektir. Yellow ise primary shard'lar tamam ama bazı replica'lar henüz atanmamış olabilir (tek node'lu cluster'da normaldir). Red ise tehlike çanları çalıyor, bazı primary shard'lara ulaşılamıyor demektir!

2. **Node'ları Listele:**

    ```http
    GET /_cat/nodes?v&h=ip,name,heap.percent,load_1m
    ```

    `?v` daha detaylı (verbose) çıktı verir. `&h=` ile hangi sütunları görmek istediğini seçebilirsin.

3. **Mevcut Index'leri Listele:**

    ```http
    GET /_cat/indices?v
    ```

    Başlangıçta Kibana'nın kendi kullandığı `.kibana` gibi sistem index'lerini görebilirsin.

İşte bu kadar! Elasticsearch ile ilk temasını kurdun. Artık "Node neydi, shard ne işe yarar?" diye sorsalar, cevabın hazır.

## 1.5 Arama Nasıl Çalışır? Perde Arkasındaki Sihir

Elasticsearch'ün o meşhur hızının ve alaka düzeyinin (relevance) sırrı ne? Neden `LIKE` sorgularından kat kat daha performanslı ve akıllı sonuçlar veriyor? Cevap, **ters index (inverted index)** adı verilen dahiyane bir veri yapısında ve bu yapıyı oluşturan **analiz (analysis)** sürecinde gizli.

### 1.5.1 Analiz Süreci: Metni Anlamlandırmak

Bir doküman Elasticsearch'e eklendiğinde (indexing), `text` tipindeki alanlar bir analiz sürecinden geçer. Bu süreç, metni aranabilir hale getirmek için onu küçük parçalara (token'lara) ayırır ve bu token'ları işler. Analiz süreci genellikle üç adımdan oluşur:

1. **Character Filters (Karakter Filtreleri):** Metni token'lara ayırmadan önce ham metin üzerinde temizlik yaparlar. Örneğin, HTML etiketlerini (`<b>`, `<i>` gibi) kaldırabilir veya "&" karakterini "and" kelimesine dönüştürebilirler.
2. **Tokenizer (Kelime Ayırıcı):** Temizlenmiş metni bireysel kelimelere (token'lara) böler. Farklı tokenizer'lar farklı kurallara göre çalışır.
    * **Standard Tokenizer:** Çoğu dil için iyi çalışan, boşluklara ve noktalama işaretlerine göre ayıran genel amaçlı bir tokenizer.
    * **Whitespace Tokenizer:** Sadece boşluklara göre ayırır.
    * **Pattern Tokenizer:** Belirli bir regex desenine göre ayırır. Log satırlarındaki belirli kalıpları ayıklamak için kullanılabilir.
    * **Language-Specific Tokenizers:** (Örn: `turkish` tokenizer) Dile özgü kuralları (örneğin Türkçe'deki ekler) dikkate alarak daha iyi token'lar üretir.
3. **Token Filters (Token Filtreleri):** Tokenizer'dan çıkan token'lar üzerinde ek işlemler yaparlar.
    * **Lowercase Token Filter:** Tüm token'ları küçük harfe çevirir ("Apple" ve "apple" aynı şekilde aranabilsin diye).
    * **Stop Token Filter:** "the", "a", "is", "bir", "ve" gibi sık kullanılan ama arama için pek anlam ifade etmeyen "stop word"leri kaldırır.
    * **Stemmer Token Filter:** Kelimeleri köklerine indirir ("running", "ran", "runs" kelimelerini "run" köküne gibi). Bu sayede "koşu" aradığınızda "koşuyor" içeren sonuçlar da gelebilir. Türkçe için `turkish_stemmer` gibi özel stemmer'lar vardır.
    * **Synonym Token Filter:** Eş anlamlı kelimeleri ekler (örneğin "laptop" arandığında "notebook" içeren sonuçların da gelmesi için).

Bu analiz süreci sonucunda, "The quick brown fox jumps over the lazy dog" gibi bir cümle, örneğin `["quick", "brown", "fox", "jump", "over", "lazy", "dog"]` gibi bir token listesine dönüşebilir.
Bu süreç ve bileşenleri hakkında daha fazla bilgi için [Elasticsearch Analiz Dokümantasyonu'na](https://www.elastic.co/guide/en/elasticsearch/reference/current/analysis.html) başvurabilirsiniz.

### 1.5.2 Ters Index (Inverted Index): Hızlı Aramanın Anahtarı

Analiz süreci tamamlandıktan sonra Elasticsearch, bu token'ları kullanarak bir **ters index** oluşturur. Ters index, bir kitaptaki dizin (index) gibidir: Kelimeleri listeler ve her kelimenin hangi dokümanlarda ve o dokümanların içinde hangi pozisyonlarda geçtiğini işaret eder.

Örnek bir ters index (çok basitleştirilmiş):

| Token   | Doküman ID'leri (ve Pozisyonlar) |
| :------ | :-------------------------------- |
| quick   | Doc1 (pozisyon 2)                 |
| brown   | Doc1 (pozisyon 3), Doc2 (pozisyon 1) |
| error   | LogDoc5 (pozisyon 7), LogDoc12 (pozisyon 3) |
| fox     | Doc1 (pozisyon 4)                 |
| ...     | ...                               |

Bir kullanıcı "brown fox" diye bir arama yaptığında, Elasticsearch:

1. "brown" token'ının geçtiği doküman listesine bakar (Doc1, Doc2).
2. "fox" token'ının geçtiği doküman listesine bakar (Doc1).
3. Bu iki listeyi karşılaştırır ve her iki token'ı da içeren dokümanları (bu örnekte Doc1) bulur.
4. Eğer `match_phrase` gibi pozisyonel bir arama yapılıyorsa, token'ların doküman içindeki pozisyonlarını da kontrol ederek "brown" kelimesinden hemen sonra "fox" kelimesinin gelip gelmediğine bakar.

Bu yapı sayesinde, milyonlarca doküman arasında bile arama yapmak, tüm dokümanları tek tek taramak yerine doğrudan ilgili token'ların listelerine bakmak kadar hızlı olur. İşte `LIKE '%...%'` sorgusunun yavaşlığının aksine Elasticsearch'ün hızının sırrı budur!

### 1.5.3 Shard ve Replica'ların Aramadaki Rolü

Peki, daha önce bahsettiğimiz shard'lar ve replica'lar bu arama sürecini nasıl etkiliyor?

* **Shard'lar ve Paralel Arama:** Bir index birden fazla shard'a bölündüğünde, bir arama sorgusu geldiğinde Elasticsearch bu sorguyu index'in tüm primary ve replica shard'larına paralel olarak gönderir. Her shard kendi içindeki verilerde aramayı yapar ve sonuçlarını coordinating node'a geri gönderir. Coordinating node bu sonuçları birleştirir, sıralar ve kullanıcıya sunar. Bu paralelleştirme, arama hızını büyük ölçüde artırır, özellikle büyük index'lerde. Ne kadar çok shard (belli bir sınıra kadar), o kadar çok paralel işlem gücü!
* **Replica'lar ve Arama Kapasitesi/Erişilebilirlik:** Replica shard'lar sadece veri yedekliliği (high availability) sağlamakla kalmaz, aynı zamanda arama sorgularını da karşılayabilirler. Yani, bir sorgu geldiğinde hem primary shard'a hem de onun replica'larına gönderilebilir. Bu, okuma (arama) yükünü dağıtarak sistemin daha fazla eş zamanlı arama isteğine cevap vermesini sağlar (arama kapasitesini artırır). Ayrıca, bir node çökerse ve üzerindeki primary shard'lara ulaşılamazsa, replica'lar sayesinde aramalar kesintisiz devam edebilir.

Kısacası, analiz süreci metni anlamlı token'lara böler, ters index bu token'lar üzerinden ışık hızında arama yapılmasını sağlar, shard'lar aramayı paralelleştirir ve replica'lar da hem arama kapasitesini artırır hem de sistemin dayanıklılığını sağlar. İşte bu mükemmel uyum, Elasticsearch'ü bu kadar güçlü bir arama motoru yapıyor!

Şimdi, bu temel arama mekanizmalarını anladığımıza göre, Bölüm 2'de verilerimizi Elasticsearch'e nasıl ekleyeceğimizi ve bu verilerin yapısını (mapping) nasıl yöneteceğimizi daha detaylı inceleyebiliriz. Çünkü doğru mapping, analiz sürecinin ve dolayısıyla arama kalitesinin temelini oluşturur!

---
[Sonraki Bölüm: Bölüm 02 ->](Section02.tr.md)
