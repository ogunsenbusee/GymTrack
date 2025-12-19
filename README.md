# GymTrack (ASP.NET Core MVC)

GymTrack, Entity Framework Core + Identity ile geliştirilmiş bir spor salonu yönetim uygulamasıdır.
Ek olarak OpenAI API entegrasyonu ile AI Coach modülü kullanıcı bilgilerine göre 4 haftalık antrenman planı üretir.

## Özellikler
- Identity (Login/Logout, Rol bazlı yetki)
- EF Core ile MSSQL LocalDB
- CRUD: Trainers, FitnessCenters (ve projedeki diğer varlıklar)
- AI Coach: OpenAI API ile JSON plan üretimi + JSON indirme
- Panel: /Dashboard üzerinden tüm modüllere tek ekrandan erişim

## Admin Hesabı
- Email: admin@gymtrack.com
- Şifre: Admin123!

## Kurulum
```bash
dotnet restore
dotnet ef database update
dotnet run
