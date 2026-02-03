# Contextual Data: Wellbore Selector
This solution provides a web-based selector for choosing the active wellbore in the DWIS contextual data
pipeline, and a shared model project that keeps API contracts aligned with external OSDC microservices.

## Purpose
- Offer a UI for browsing Field → Cluster → Well → Wellbore and selecting the current wellbore.
- Publish the selection so DWIS consumers can use a consistent context.
- Keep generated model types in sync with external microservice JSON schemas.

## Projects
### `ModelSharedOut`
Generates and hosts shared C# data structures derived from external JSON schemas. These types are used by
the WebApp to serialize and deserialize microservice API payloads.

### `WebApp`
Blazor Server application that queries the external microservices, provides the selection UI, and publishes
the selected wellbore to the DWIS Blackboard.

## How They Link Together
- `WebApp` references the `ModelSharedOut` project directly.
- `ModelSharedOut` supplies the generated DTOs and API client types used by the WebApp.
- This ensures the WebApp stays aligned with schema changes without duplicating model definitions.
