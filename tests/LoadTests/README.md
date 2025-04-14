# Load Testing for Voucher Management Service

This directory contains load test scripts for the Voucher Management Service API.

## Tools Used

- **k6**: Modern load testing tool that makes performance testing easy and productive
- **Azure Load Testing**: For cloud-based load testing

## Test Scenarios

The load tests cover the following scenarios:

1. **Create Voucher Batch**: Test the performance of creating voucher batches
2. **Validate Voucher**: Test the performance of voucher validation
3. **Redeem Voucher**: Test the performance of voucher redemption
4. **End-to-End Flow**: Test the complete voucher lifecycle

## Running the Tests Locally

### Prerequisites

- Install k6: https://k6.io/docs/getting-started/installation/

### Running the Tests

```bash
# Run with default settings
k6 run voucher-api-load-test.js

# Run with custom settings
k6 run \
  -e API_BASE_URL=https://your-api-url/api/v1 \
  -e API_KEY=your-api-key \
  voucher-api-load-test.js
```

## Running in Azure Load Testing

1. Create an Azure Load Testing resource
2. Upload the test script
3. Configure test parameters:
   - Duration: 5 minutes
   - Virtual users: 50
   - Ramp-up time: 30 seconds
4. Set the test environment variables:
   - API_BASE_URL
   - API_KEY

## Performance Targets

The service should meet the following performance targets:

- **Response Time**: 95% of requests should complete in under 500ms
- **Throughput**: Support at least 100 requests per second
- **Error Rate**: Less than 1% error rate under load
- **Resource Utilization**: CPU < 80%, Memory < 70%

## Test Results Analysis

After running the tests, analyze the following metrics:

1. **Response Time**: Check p50, p95, and p99 percentiles
2. **Throughput**: Requests per second
3. **Error Rate**: Percentage of failed requests
4. **Resource Utilization**: CPU, memory, and network usage

## CI/CD Integration

The load tests can be integrated into the CI/CD pipeline:

```yaml
- task: AzureLoadTest@1
  inputs:
    azureSubscription: 'Your Azure Service Connection'
    loadTestConfigFile: 'tests/LoadTests/config.yaml'
    resourceGroup: '$(resourceGroupName)'
    loadTestResource: '$(loadTestResourceName)'
```

## Notes

- For best results, run tests against an environment that matches production
- Consider testing during off-peak hours to avoid impact on real users
- Reset test data before and after load tests
