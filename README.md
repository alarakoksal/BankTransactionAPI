# Bank Transaction Tracker API

Hesap yönetimi ve para transferi işlemleri için geliştirilmiş bir bankacılık REST API'si.

## Teknolojiler

- .NET 8 Web API
- Entity Framework Core
- SQLite
- Swagger / OpenAPI

## Özellikler

- Hesap oluşturma, listeleme, güncelleme ve silme
- Hesaplar arası para transferi (atomik — DB transaction ile)
- İşlem geçmişi listeleme ve filtreleme (tutar, tarih aralığı)
- Hesaba göre işlem geçmişi
- Duplicate hesap numarası engeli (DB unique constraint)
- Negatif bakiye ve yetersiz bakiye koruması

## Kurulum

```bash
git clone https://github.com/alarakoksal/BankTransactionAPI.git
cd BankTransactionAPI
dotnet ef database update
dotnet run
```

Swagger UI: `http://localhost:5299/swagger`

## Endpoint'ler

### Hesaplar

| Method | URL | Açıklama |
|--------|-----|----------|
| GET | `/api/accounts` | Tüm hesapları listele |
| GET | `/api/accounts/{id}` | Tek hesap getir |
| GET | `/api/accounts/{id}/transactions` | Hesabın işlem geçmişi |
| POST | `/api/accounts` | Yeni hesap oluştur |
| PUT | `/api/accounts/{id}` | Hesap güncelle |
| DELETE | `/api/accounts/{id}` | Hesap sil |

### İşlemler

| Method | URL | Açıklama |
|--------|-----|----------|
| GET | `/api/transactions` | Tüm işlemleri listele |
| GET | `/api/transactions/{id}` | Tek işlem getir |
| GET | `/api/transactions/filter` | Filtrele (`minAmount`, `maxAmount`, `startDate`, `endDate`) |
| POST | `/api/transactions/transfer` | Para transferi yap |

### Transfer örneği

```json
POST /api/transactions/transfer
{
  "senderAccountId": 1,
  "receiverAccountId": 2,
  "amount": 500
}
```

```json
{
  "id": 4,
  "senderName": "Alara Köksal",
  "senderAccountNumber": "TR10001",
  "receiverName": "Ege",
  "receiverAccountNumber": "TR10002",
  "amount": 500,
  "date": "2026-06-05T20:00:00Z"
}
```
