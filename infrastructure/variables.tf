variable "environment" {
  description = "The environment (dev, stg, prod)"
  type        = string
}

variable "location" {
  description = "The Azure region for resources"
  type        = string
  default     = "East US"
}

variable "secondary_location" {
  description = "The secondary Azure region for geo-redundant resources"
  type        = string
  default     = "West US"
}

variable "app_service_plan_sku" {
  description = "The SKU for the App Service Plan"
  type        = string
  default     = "P1v2"
}

variable "api_key" {
  description = "The API key for authentication"
  type        = string
  sensitive   = true
}
