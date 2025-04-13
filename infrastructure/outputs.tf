output "app_service_url" {
  description = "The URL of the App Service"
  value       = "https://${azurerm_linux_web_app.main.default_hostname}"
}

output "app_service_name" {
  description = "The name of the App Service"
  value       = azurerm_linux_web_app.main.name
}

output "cosmos_db_endpoint" {
  description = "The endpoint of the Cosmos DB"
  value       = azurerm_cosmosdb_account.main.endpoint
}

output "service_bus_namespace" {
  description = "The name of the Service Bus namespace"
  value       = azurerm_servicebus_namespace.main.name
}

output "application_insights_connection_string" {
  description = "The connection string for Application Insights"
  value       = azurerm_application_insights.main.connection_string
  sensitive   = true
}

output "application_insights_instrumentation_key" {
  description = "The instrumentation key for Application Insights"
  value       = azurerm_application_insights.main.instrumentation_key
  sensitive   = true
}
