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

## DWIS package versions
This host uses DWIS.RigOS.Common.Model 1.0.7 and explicitly pins
DWIS.Client.ReferenceImplementation.OPCFoundation 2.2.54. Restore packages from nuget.org.

## Docker publication
Manually dispatch docker-build-push-interpreter.yml to build/test the solution and publish
digiwells/dwiscontextualdatawellboreselectorwebapp with stable and immutable sha-<commit> tags. Build from the repository root;
the Docker restore layer includes the referenced shared model project. Mount a persistent
/home directory containing the existing configuration; do not overwrite it during upgrades.

## OSDC package versions
DrillingProperties 10.2.11 is supplied by Common Model. General.Statistics is explicitly pinned
to 3.3.6, which supplies General.Math 1.4.2 transitively; no direct Math reference is needed.
The selector uses General.DataManagement 2.2.0 and DrillingRazorMudComponents 3.4.9,
which resolves the Conversion, DrillingEngineering and UnitSystem packages at 3.4.5.
