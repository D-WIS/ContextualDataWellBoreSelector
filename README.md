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
DWIS.Client.ReferenceImplementation.OPCFoundation 2.2.55. Restore packages from nuget.org.

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

## Current production APIs
The configuration uses https://app.digiwells.no/; the Blackboard address remains independently
configurable. Generator inputs were refreshed from each service's /api/swagger/merged/swagger.json
export, retaining its own resource routes and the transitive schema references, including
discriminator mappings. This avoids importing stale routes from bundled dependencies.
Run the ModelSharedOut generator from the repository root and answer Y to overwrite.
NSwag owns the generated C# and merged JSON. Collections are mutable lists. Unused reflection
pseudo constructors are no longer generated; NSwag supplies DTO defaults.
The selector follows NextOffset on LightData discovery pages and uses light hierarchy records
for its menus. It fetches the complete wellbore by UUID when publishing the selection. Service
load failures and Blackboard write outcomes are displayed in the UI.

## Explicit contextual selections
The WellBore Selector publishes SelectedWellBoreArchitectureUUID, SelectedTrajectoryUUID and
SelectedDrillStringUUID as separate string signals, semantically classified as ConfigurationData
and the corresponding description kind. Values are UUID strings; an empty string clears a choice.
The existing JSON description signals carry the selected full records, not these UUIDs.
Each bridge requires an explicit UUID, fetches that record by ID and verifies its WellBoreID
before publishing. Missing or incompatible selections clear the description to JSON null.
There is no first-record fallback. Temporary service failures leave the output cleared and retry.
Selections are retained on the Blackboard and restored when reopening the selector page;
restoration requires the Blackboard to retain its state. Changing the selected wellbore resets
incompatible choices. Apply selections writes the wellbore and all three UUIDs in one OPC-UA call.

## Well menu field scoping
A single well belongs to its field through its proxy ClusterID, even though that proxy is
not shown in the Cluster menu. The Well menu requires the proxy cluster's FieldID to match
the selected field. Clustered wells additionally require the selected cluster. Unlinked wells
and wells with unknown cluster ownership are omitted. WebApp.Tests covers these cases.

## Searchable selectors
All seven selectors accept typed, case-insensitive substring searches. Empty text shows all
choices permitted by the current hierarchy. Architecture, trajectory and drill-string choices
display only their names and also match their UUID when searching. Searches run against already loaded choices; typing does not call
external services. Choose a listed result; arbitrary text is not a published selection.
Use Apply selections to publish the selected UUIDs. Existing field/cluster/well filters apply
before text filtering, including the field restriction on single wells.

The query lifecycle dependency release uses OPC Foundation client 2.2.55 (directly or through mechanical bridge 1.0.6) and Common.Worker 1.0.8 where applicable. Repeated query registrations are deduplicated and released when their session closes. Existing configuration and persistent volumes are preserved.

## Blackboard reconnection
The host retries an unavailable or disconnected Blackboard every five seconds and disposes
replaced sessions. Each open selector page retries registration of the wellbore and three
context-selection signals, including after reconnect. A warning remains visible until all
bindings are ready. No selections are published automatically. Unsaved dropdown choices
survive connection loss; Blackboard selections are restored only on the first successful
binding when the user has not already edited the page. Closing the page cancels its monitor.
Regression tests cover startup failures, lost connections, disposal, registration failures,
re-binding and preservation of unsaved choices.
