# Bank Transaction Tracker API

A banking REST API for account management and money transfers.

## Technologies

- .NET 8 Web API
- Entity Framework Core
- SQLite
- Swagger / OpenAPI

## Features

- Create, list, update, and delete accounts
- Money transfers between accounts (atomic — DB transaction)
- Transaction history listing and filtering (by amount, date range)
- Transaction history per account
- Duplicate account number prevention (DB unique constraint)
- Negative balance and insufficient funds protection

## Getting Started

```bash
git clone https://github.com/alarakoksal/BankTransactionAPI.git
cd BankTransactionAPI
dotnet ef database update
dotnet run
```

Swagger UI: `http://localhost:5299/swagger`

## Endpoints

### Accounts

| Method | URL | Description |
|--------|-----|-------------|
| GET | `/api/accounts` | List all accounts |
| GET | `/api/accounts/{id}` | Get account by ID |
| GET | `/api/accounts/{id}/transactions` | Get transaction history for an account |
| POST | `/api/accounts` | Create a new account |
| PUT | `/api/accounts/{id}` | Update an account |
| DELETE | `/api/accounts/{id}` | Delete an account |

### Transactions

| Method | URL | Description |
|--------|-----|-------------|
| GET | `/api/transactions` | List all transactions |
| GET | `/api/transactions/{id}` | Get transaction by ID |
| GET | `/api/transactions/filter` | Filter by `minAmount`, `maxAmount`, `startDate`, `endDate` |
| POST | `/api/transactions/transfer` | Make a transfer |

### Transfer Example

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
