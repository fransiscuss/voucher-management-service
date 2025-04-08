# Voucher Management Service

## Introduction 
The Voucher Management Service is a flexible service for generating, validating, and redeeming vouchers for various business use cases. It provides a robust API for managing the complete lifecycle of vouchers.

Common use cases for vouchers include:
* Promotional offers and discounts
* Gift certificates
* Loyalty program rewards
* Event tickets
* Access credentials

Vouchers are created with an expiry date. When a voucher is not used within its validity period, it will no longer be valid and cannot be redeemed.

## Architecture

This project follows Clean Architecture principles with:

- **Domain Layer**: Core business entities and logic
- **Application Layer**: Business use cases and interfaces
- **Infrastructure Layer**: External services implementation
- **API Layer**: HTTP endpoints and controllers

## Technologies

- .NET 8
- ASP.NET Core Web API
- Azure Cosmos DB
- Azure Service Bus
- MediatR for CQRS implementation
- FluentValidation
- Polly for resilience patterns

## Getting Started

### Prerequisites
- .NET 8 SDK
- Azure Cosmos DB Emulator (for local development)
- Azure Service Bus connection (or use a mock for development)

### Setup
1. Clone the repository
2. Configure your application settings in `appsettings.json`
3. Run `dotnet build` to build the solution
4. Run `dotnet run --project src/Api/Api.csproj` to start the API

## API Endpoints

The service exposes the following endpoints:

- `POST /api/v1/vouchers/batch` - Create a batch of vouchers
- `POST /api/v1/vouchers/{voucherCode}/redeem` - Redeem a voucher
- `POST /api/v1/vouchers/{voucherCode}/validate` - Validate a voucher
- `GET /api/v1/vouchers` - Get vouchers (with filtering)
- `GET /api/v1/vouchers/{voucherCode}` - Get a specific voucher

## License

This project is licensed under the MIT License - see the LICENSE file for details
