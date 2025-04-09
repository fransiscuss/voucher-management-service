# Testing Approach

In this project, we follow a pragmatic approach to testing, focusing on business value over implementation details:

## What We Test

- **Business Rules and Logic**: We test the core business rules and logic that are critical to the domain, such as voucher validation and redemption rules.
- **Complex Behaviors**: We test behaviors that involve conditional logic, calculations, or state transitions.
- **Integration Points**: We test how our components interact with other parts of the system.

## What We Don't Test

- **Property Getters/Setters**: We don't test simple property getters and setters that don't contain business logic.
- **Trivial Implementations**: We don't test code that is so simple that the test would essentially duplicate the implementation.
- **Framework Features**: We don't test features provided by frameworks or libraries we use.

## Value-Driven Testing

Our approach focuses on the value that tests provide:

1. **Documentation**: Tests serve as executable documentation for our business rules.
2. **Safety Net**: Tests catch regressions when we change the code.
3. **Design Feedback**: Tests help us identify design issues early.

## Test Structure

We organize our tests to match the structure of our application, with a focus on:

- **Domain Tests**: Tests for domain entities and business rules
- **Application Tests**: Tests for application services and use cases
- **Infrastructure Tests**: Tests for infrastructure components when necessary

This approach gives us a balance of test coverage and maintenance cost, ensuring we have high confidence in our code without spending time writing tests that don't add much value.
