# Infrastructure as Code (IaC) for Voucher Management Service

This directory contains Terraform code to provision and manage the cloud infrastructure required for the Voucher Management Service.

## Resources Created

- **Resource Group**: Container for all resources
- **App Service Plan**: Hosting plan for the web application
- **App Service**: Web application hosting
- **Application Insights**: Monitoring and telemetry
- **Cosmos DB Account**: Main database with geo-replication for production
- **Cosmos DB Database and Container**: Data storage for vouchers
- **Service Bus Namespace**: Messaging platform
- **Service Bus Topic**: For voucher redemption events
- **Key Vault**: Secure storage for secrets

## Environment Support

The infrastructure supports three environments:
- **Development (dev)**: For development and testing
- **Staging (stg)**: For pre-production validation
- **Production (prod)**: For production deployment with enhanced reliability

## Usage

### Prerequisites

- Terraform v1.0.0+
- Azure CLI
- Proper Azure permissions

### Deployment

1. Initialize Terraform:
   ```bash
   terraform init
   ```

2. Create a `terraform.tfvars` file with your variables:
   ```
   environment        = "dev"
   location           = "East US"
   secondary_location = "West US"
   api_key            = "your-secure-api-key"
   ```

3. Plan the deployment:
   ```bash
   terraform plan -out=tfplan
   ```

4. Apply the deployment:
   ```bash
   terraform apply tfplan
   ```

### Destroying Infrastructure

To tear down the infrastructure:
```bash
terraform destroy
```

## Security Considerations

- API keys are stored securely in Key Vault
- App Service uses managed identity to access other resources
- All communications use HTTPS
- Network access is restricted where possible

## Scaling

- For production, the Service Bus uses Premium SKU for enhanced reliability
- Cosmos DB has geo-replication enabled in production
- App Service Plan can be scaled up or out based on load

## Monitoring

Application Insights is integrated for comprehensive monitoring, including:
- Request metrics
- Dependency tracking
- Exception logging
- Performance analysis
