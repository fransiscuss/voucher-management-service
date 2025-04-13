provider "azurerm" {
  features {}
}

locals {
  common_tags = {
    Project     = "Voucher Management Service"
    Environment = var.environment
    ManagedBy   = "Terraform"
  }
}

# Resource Group
resource "azurerm_resource_group" "main" {
  name     = "rg-voucherservice-${var.environment}"
  location = var.location
  tags     = local.common_tags
}

# App Service Plan
resource "azurerm_service_plan" "main" {
  name                = "plan-voucherservice-${var.environment}"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  os_type             = "Linux"
  sku_name            = var.app_service_plan_sku
  tags                = local.common_tags
}

# App Service
resource "azurerm_linux_web_app" "main" {
  name                = "app-voucherservice-${var.environment}"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  service_plan_id     = azurerm_service_plan.main.id
  https_only          = true
  tags                = local.common_tags

  site_config {
    always_on         = true
    app_command_line  = ""
    ftps_state        = "Disabled"
    
    application_stack {
      dotnet_version = "8.0"
    }
  }

  app_settings = {
    "WEBSITE_RUN_FROM_PACKAGE"                   = "1"
    "APPLICATIONINSIGHTS_CONNECTION_STRING"      = azurerm_application_insights.main.connection_string
    "CosmosDb:DocumentEndpoint"                  = azurerm_cosmosdb_account.main.endpoint
    "CosmosDb:DatabaseId"                        = azurerm_cosmosdb_sql_database.main.name
    "CosmosDb:VoucherContainerId"                = azurerm_cosmosdb_sql_container.vouchers.name
    "ServiceBus:VoucherRedeemConnectionString"   = azurerm_servicebus_namespace.main.default_primary_connection_string
    "ServiceBus:VoucherRedeemTopicName"          = azurerm_servicebus_topic.voucher_redeemed.name
    "Authentication:ApiKey"                      = var.api_key
  }

  identity {
    type = "SystemAssigned"
  }
}

# Application Insights
resource "azurerm_application_insights" "main" {
  name                = "appi-voucherservice-${var.environment}"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  application_type    = "web"
  tags                = local.common_tags
}

# Cosmos DB Account
resource "azurerm_cosmosdb_account" "main" {
  name                = "cosmos-voucherservice-${var.environment}"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  offer_type          = "Standard"
  kind                = "GlobalDocumentDB"
  tags                = local.common_tags

  consistency_policy {
    consistency_level = "Session"
  }

  geo_location {
    location          = azurerm_resource_group.main.location
    failover_priority = 0
  }

  # Additional geo_location for production
  dynamic "geo_location" {
    for_each = var.environment == "prod" ? [1] : []
    content {
      location          = var.secondary_location
      failover_priority = 1
    }
  }
}

# Cosmos DB Database
resource "azurerm_cosmosdb_sql_database" "main" {
  name                = var.environment == "dev" ? "voucher-service-dev" : "voucher-service"
  resource_group_name = azurerm_resource_group.main.name
  account_name        = azurerm_cosmosdb_account.main.name
}

# Cosmos DB Container
resource "azurerm_cosmosdb_sql_container" "vouchers" {
  name                = "vouchers"
  resource_group_name = azurerm_resource_group.main.name
  account_name        = azurerm_cosmosdb_account.main.name
  database_name       = azurerm_cosmosdb_sql_database.main.name
  partition_key_path  = "/id"
  throughput          = 400
}

# Service Bus Namespace
resource "azurerm_servicebus_namespace" "main" {
  name                = "sb-voucherservice-${var.environment}"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  sku                 = var.environment == "prod" ? "Premium" : "Standard"
  capacity            = var.environment == "prod" ? 1 : 0
  tags                = local.common_tags
}

# Service Bus Topic
resource "azurerm_servicebus_topic" "voucher_redeemed" {
  name                = "voucher-redeemed"
  resource_group_name = azurerm_resource_group.main.name
  namespace_name      = azurerm_servicebus_namespace.main.name
  enable_partitioning = true
  max_size_in_megabytes = 1024
}

# Service Bus Topic Authorization Rule
resource "azurerm_servicebus_topic_authorization_rule" "sender" {
  name                = "sender"
  resource_group_name = azurerm_resource_group.main.name
  namespace_name      = azurerm_servicebus_namespace.main.name
  topic_name          = azurerm_servicebus_topic.voucher_redeemed.name
  listen              = false
  send                = true
  manage              = false
}

# Key Vault
resource "azurerm_key_vault" "main" {
  name                = "kv-voucherservice-${var.environment}"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  tenant_id           = data.azurerm_client_config.current.tenant_id
  sku_name            = "standard"
  tags                = local.common_tags
  
  purge_protection_enabled = true
}

data "azurerm_client_config" "current" {}

# Key Vault Access Policy for the App Service
resource "azurerm_key_vault_access_policy" "app" {
  key_vault_id        = azurerm_key_vault.main.id
  tenant_id           = data.azurerm_client_config.current.tenant_id
  object_id           = azurerm_linux_web_app.main.identity[0].principal_id
  
  secret_permissions = [
    "Get",
    "List"
  ]
}

# Store API Key in Key Vault
resource "azurerm_key_vault_secret" "api_key" {
  name         = "ApiKey"
  value        = var.api_key
  key_vault_id = azurerm_key_vault.main.id
  
  depends_on = [
    azurerm_key_vault_access_policy.app
  ]
}
