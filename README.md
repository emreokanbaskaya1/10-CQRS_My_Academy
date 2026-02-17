# ?? Bakery Order App - CQRS Pattern

Modern bir pastane/fýrýn uygulamasý. **CQRS**, **Chain of Responsibility**, **Mediator** ve **Repository** pattern'leri kullanýlarak geliþtirilmiþtir.

## ?? Özellikler

### ?? Müþteri Özellikleri
- ? Ürün listeleme ve sepete ekleme
- ? Promosyon kodu uygulama
- ? Sipariþ oluþturma
- ? Ýletiþim formu
- ? Yorum/testimonial býrakma
- ? Galeri görüntüleme

### ????? Admin Paneli
- ? Dashboard (7 farklý grafik)
- ? Ürün yönetimi
- ? Kategori yönetimi
- ? Sipariþ yönetimi (pagination ile 1000'li)
- ? Sipariþ durumu deðiþtirme
- ? Slider yönetimi
- ? Galeri yönetimi
- ? Kampanya/promosyon yönetimi
- ? Yorum yönetimi (onaylama/reddetme)
- ? Mesaj yönetimi
- ? Hizmet yönetimi
- ? Feature Steps yönetimi
- ? Tarihçe yönetimi

## ??? Kullanýlan Teknolojiler

### Backend
- **.NET 9.0**
- **ASP.NET Core MVC**
- **Entity Framework Core**
- **SQL Server (LocalDB)**
- **Identity** (Authentication & Authorization)

### Design Patterns
- **CQRS Pattern** - Command Query Responsibility Segregation
- **Chain of Responsibility** - Sipariþ ve iletiþim validasyonu
- **Repository Pattern** - Veri eriþim katmaný
- **Dependency Injection**

### Frontend
- **Bootstrap 5**
- **Chart.js** - Dashboard grafikleri
- **SweetAlert2** - Bildirimler
- **Font Awesome** - Ýkonlar
- **Owl Carousel** - Slider

### Cloud Services
- **Google Cloud Storage** - Görsel yükleme

### Logging
- **Serilog** - Console ve SQL Server logging

## ?? Kurulum

### 1. Projeyi Klonlayýn
```bash
git clone https://github.com/YOUR_USERNAME/bakery-order-app.git
cd bakery-order-app
```

### 2. appsettings.json Oluþturun
```bash
cd MyAcademyCQRS
cp appsettings.example.json appsettings.json
```

### 3. Konfigürasyon Ayarlarý

#### Veritabaný Baðlantýsý
`appsettings.json` dosyasýnda connection string'i düzenleyin:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=BakeryOrderAppDb;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

#### Google Cloud Storage (Opsiyonel)
1. Google Cloud Console'dan Service Account oluþturun
2. JSON key dosyasýný indirin
3. `MyAcademyCQRS` klasörüne kopyalayýn
4. `appsettings.json`'da dosya adýný belirtin:
```json
"GoogleCloudStorage": {
  "BucketName": "your-bucket-name",
  "CredentialFile": "your-credentials.json"
}
```

### 4. Veritabaný Migrasyonu
```bash
dotnet ef database update
```

### 5. Projeyi Çalýþtýrýn
```bash
dotnet run
```

Tarayýcýda: `https://localhost:7260`

## ?? Admin Giriþi

Ýlk çalýþtýrmada seed data oluþturulur:

- **Kullanýcý:** `admin`
- **Þifre:** `Admin123*`

## ?? Proje Yapýsý

```
MyAcademyCQRS/
??? Areas/
?   ??? Admin/          # Admin paneli
?       ??? Controllers/
?       ??? Views/
??? Controllers/        # Ana controller'lar
??? Views/             # Ana view'lar
??? CQRSPattern/       # CQRS implementation
?   ??? Commands/
?   ??? Queries/
?   ??? Handlers/
?   ??? Results/
??? DesignPatterns/    # Design pattern'ler
?   ??? ChainOfResponsibility/
??? Entities/          # Entity modeller
??? Context/           # DbContext
??? Data/              # Seed data
??? Extensions/        # Extension metodlar
??? Services/          # Service layer
??? wwwroot/           # Static dosyalar
```

## ?? Ekran Görüntüleri

### Ana Sayfa
- Slider
- Öne çýkan ürünler
- Hizmetler
- Tarihçe
- Galeri
- Kampanyalar
- Müþteri yorumlarý

### Admin Dashboard
- 7 farklý grafik (Chart.js)
- Sipariþ durumu daðýlýmý
- Kategorilere göre ürünler
- 7 günlük trend
- Aylýk gelir
- Top 5 ürün
- Aktif/pasif ürün oraný
- Yorum durumu

## ?? Geliþtirme

### Yeni Migration Eklemek
```bash
dotnet ef migrations add MigrationName
dotnet ef database update
```

### Build
```bash
dotnet build
```

### Test
```bash
dotnet test
```

## ?? API Endpoints

### Public Endpoints
- `GET /` - Ana sayfa
- `GET /Home/Shop` - Ürünler
- `GET /Home/Gallery` - Galeri
- `GET /Home/Contact` - Ýletiþim
- `POST /Home/AddToCart` - Sepete ekle
- `POST /Home/Checkout` - Sipariþ ver

### Admin Endpoints (Authorized)
- `GET /Admin/Dashboard` - Dashboard
- `GET /Admin/Order` - Sipariþler
- `POST /Admin/Order/UpdateStatus` - Sipariþ durumu güncelle
- CRUD iþlemleri tüm entity'ler için

## ??? Güvenlik

- ? Identity ile authentication
- ? Role-based authorization (Admin)
- ? CSRF protection
- ? XSS korumasý
- ? SQL Injection korumasý (EF Core)
- ? Secure file upload validation

## ?? Database Schema

### Ana Tablolar
- `Users` - Kullanýcýlar
- `Products` - Ürünler
- `Categories` - Kategoriler
- `Orders` - Sipariþler
- `OrderItems` - Sipariþ detaylarý
- `Promotions` - Kampanyalar
- `Testimonials` - Yorumlar
- `Contacts` - Mesajlar
- `Sliders` - Slider görselleri
- `PhotoGallery` - Galeri
- `Services` - Hizmetler
- `ServiceSteps` - Feature steps
- `OurHistory` - Tarihçe
- `AppLogs` - Sistem loglarý

## ?? Deploy

### IIS Deploy
1. `dotnet publish -c Release -o ./publish`
2. IIS'de site oluþturun
3. Application Pool: No Managed Code
4. `publish` klasörünü kopyalayýn

### Azure Deploy
1. Azure App Service oluþturun
2. SQL Database oluþturun
3. Connection string'i güncelleyin
4. GitHub Actions veya Azure DevOps ile CI/CD

## ?? Katkýda Bulunma

1. Fork edin
2. Feature branch oluþturun (`git checkout -b feature/amazing-feature`)
3. Commit edin (`git commit -m 'Add some amazing feature'`)
4. Push edin (`git push origin feature/amazing-feature`)
5. Pull Request açýn

## ?? Lisans

Bu proje MIT lisansý altýnda lisanslanmýþtýr.

## ????? Geliþtirici

**Your Name**
- GitHub: [@yourusername](https://github.com/yourusername)
- Email: your.email@example.com

## ?? Teþekkürler

- [.NET Foundation](https://dotnetfoundation.org/)
- [Bootstrap](https://getbootstrap.com/)
- [Chart.js](https://www.chartjs.org/)
- [SweetAlert2](https://sweetalert2.github.io/)
- [Font Awesome](https://fontawesome.com/)

---

? Bu projeyi beðendiyseniz yýldýz vermeyi unutmayýn!
