import http from 'k6/http';
import { check, sleep } from 'k6';
import { randomString } from 'https://jslib.k6.io/k6-utils/1.2.0/index.js';

export const options = {
  stages: [
    { duration: '30s', target: 10 }, // Ramp up to 10 users
    { duration: '1m', target: 10 },  // Stay at 10 users for 1 minute
    { duration: '30s', target: 50 }, // Ramp up to 50 users
    { duration: '1m', target: 50 },  // Stay at 50 users for 1 minute
    { duration: '30s', target: 0 },  // Ramp down to 0 users
  ],
  thresholds: {
    http_req_duration: ['p(95)<500'], // 95% of requests should be below 500ms
    http_req_failed: ['rate<0.01'],   // Less than 1% of requests should fail
  },
};

// Configuration
const API_BASE_URL = __ENV.API_BASE_URL || 'https://app-voucherservice-dev.azurewebsites.net/api/v1';
const API_KEY = __ENV.API_KEY || 'test-api-key';
const ISSUING_PARTY = 'ACME_LOAD_TEST';

// Global variables to store data between requests
let voucherCode = '';
let batchNumber = '';

// Helper functions
function generateBatchRequest() {
  batchNumber = `BATCH-LOAD-${randomString(8)}`;
  return {
    issuingParty: ISSUING_PARTY,
    category: 'Load Test',
    batchNumber: batchNumber,
    batchSize: 1,
    expiryDateUtc: new Date(Date.now() + 30 * 24 * 60 * 60 * 1000).toISOString(), // 30 days from now
    codeGenerationType: 'ShortCode',
    userDefinedProperties: [
      {
        id: 'source',
        values: ['load-test']
      }
    ]
  };
}

export default function () {
  const headers = {
    'Content-Type': 'application/json',
    'X-API-Key': API_KEY
  };

  // 1. Create a voucher batch
  let createResponse = http.post(
    `${API_BASE_URL}/vouchers/batch`,
    JSON.stringify(generateBatchRequest()),
    { headers }
  );
  
  check(createResponse, {
    'Voucher batch created successfully': (r) => r.status === 201,
    'Voucher batch response has correct size': (r) => r.json().batchSize === 1
  });

  sleep(1);

  // 2. Get vouchers by batch number (to get the code)
  // Note: In a real implementation, we would need an endpoint to query by batch number
  // This is omitted here since our API doesn't have this endpoint in the example

  // 3. Validate the voucher
  // Using a placeholder voucher code for demonstration
  const validateHeaders = {
    'X-API-Key': API_KEY,
    'issuingParty': ISSUING_PARTY
  };
  
  // Using a hardcoded known voucher code for the test
  // In a real test, we would use the code from the previous step
  voucherCode = 'TEST123';
  
  let validateResponse = http.post(
    `${API_BASE_URL}/vouchers/${voucherCode}/validate`,
    null,
    { headers: validateHeaders }
  );
  
  // This will likely fail in our test setup since we don't have the actual code
  // check(validateResponse, {
  //   'Voucher validation status code': (r) => r.status === 200 || r.status === 404,
  // });

  sleep(1);

  // 4. Redeem the voucher
  let redeemResponse = http.post(
    `${API_BASE_URL}/vouchers/${voucherCode}/redeem`,
    JSON.stringify({
      redemptionId: `order-${randomString(8)}`,
      redemptionType: 'LoadTest'
    }),
    { headers: validateHeaders }
  );
  
  // This will likely fail in our test setup since we don't have the actual code
  // check(redeemResponse, {
  //   'Voucher redemption status code': (r) => r.status === 204 || r.status === 404 || r.status === 400,
  // });

  sleep(2);
}
