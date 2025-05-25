# Elasticsearch Macerası: Bir Maceracının Kılavuzu

## Hoş Geldin Cesur Arkadaşım!

Eğer bu satırları okuyorsan, muhtemelen sen de veri okyanusunda anlamlı bir şeyler arayan, `LIKE '%aramaKriterim%'` sorgularının yavaşlığından bıkmış, logların arasında kaybolmak yerine onlara hükmetmek isteyen bir kahramansın. Ya da sadece yöneticin "Şu Elasticsearch'e bir bakıver" dedi ve sen de kendini burada buldun. Her iki durumda da doğru yerdesin!

Bu kitapçık, seni Elasticsearch'ün büyülü dünyasında bir yolculuğa çıkarmak için tasarlandı. Amacımız, bu güçlü arama ve analiz motorunun temel taşlarını anlamanı, onu nasıl kullanacağını öğrenmeni ve projelerinde nasıl fark yaratabileceğini görmeni sağlamak. Kemerleri bağla, çünkü veriyle dolu bir maceraya atılıyoruz! Belki arada birkaç espriye de denk gelirsin, ne de olsa kod yazmak ciddi bir iş ama öğrenmek eğlenceli olabilir, değil mi?

**Bu Kitapçıkta Seni Neler Bekliyor?**

* **Neden Var Bu Elasticsearch?**: "İyi güzel de, benim zaten veritabanım var" diyenlere cevaplar. ([Bölüm 1](./Section01.tr.md))
* **Temel Kavramlar**: Elasticsearch jargonuna aşina olalım ki, kimse sana "Shard neydi ya?" diye sorduğunda boş bakma. ([Bölüm 1](./Section01.tr.md))
* **Arama Nasıl Çalışır?**: Tokenizer'lar, ters index'ler ve aramanın perde arkası. ([Bölüm 2](./Section02.tr.md))
* **Veri Yönetimi**: Verileri Elasticsearch'e nasıl atarız, nasıl düzenleriz? Ürün ve log verileriyle pratik örnekler. ([Bölüm 2](./Section02.tr.md))
* **Arama Sanatı**: Basit aramalardan, karmaşık sorgulara uzanan bir yolculuk. Ürün ve log verilerinde kayıp bilgiyi bulmanın keyfi! ([Bölüm 3](./Section03.tr.md))
* **Analizin Gücü**: Veriyi sadece bulmak yetmez, onu anlamlandırmak da lazım. Ürün ve log verileri üzerinden Aggregation'larla tanışma. ([Bölüm 4](./Section04.tr.md))
* **Elastic Stack Ailesi**: Elasticsearch'ün yalnız olmadığını, Kibana, Logstash ve Beats gibi kardeşleriyle nasıl bir takım oluşturduğunu göreceğiz. ([Bölüm 5](./Section05.tr.md))
* **Ve Daha Fazlası**: Production ipuçları, öğrenmeye devam etmek için kaynaklar ve belki birkaç sürpriz! ([Kapanış](./Section06.tr.md))

Hazırsan, başlayalım! Unutma, her büyük yolculuk tek bir adımla başlar. Bizim ilk adımımız ise Elasticsearch'ün neden bu kadar popüler olduğunu anlamak olacak. ([Bölüm 1](./Section01.tr.md))

* [Bölüm 1: Elasticsearch Dünyasına Merhaba De!](./Section01.tr.md)
* [Bölüm 2: Veri Krallığınızı Kurun: Indexing ve Mapping](./Section02.tr.md)
* [Bölüm 3: Arama Sanatında Ustalaşın: Query DSL](./Section03.tr.md)
* [Bölüm 4: Analizin Gücü: Aggregation'larla Veriye Hükmetmek](./Section04.tr.md)
* [Bölüm 5: Elastic Stack Ailesi ve Ötesi](./Section05.tr.md)
* [Kapanış: Maceranın Sonu mu, Başlangıcı mı?](./Section06.tr.md)

Bu kitapçık, umarım Elasticsearch'e olan merakını ateşlemiş ve sana sağlam bir temel vermiştir. Unutma, en iyi öğrenme yolu pratik yapmaktır. Kendi projelerinde Elasticsearch'ü kullanmaktan, farklı sorgular denemekten, yeni özelliklerini keşfetmekten çekinme.

Veriyle dolu maceralarında başarılar dilerim! Belki bir gün, bir Elastic Meetup'ında veya bir konferansta karşılaşırız, kim bilir? O zamana kadar, kodlamaya ve öğrenmeye devam!
