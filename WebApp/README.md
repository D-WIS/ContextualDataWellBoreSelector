# A Web Application to Select the Current Wellbore
This Blazor Server web app is the user-facing selector for the DWIS contextual data pipeline. It connects to
the OSDC microservice framework to discover fields, clusters, wells, and wellbores, and allows the user to
choose the active wellbore for downstream DWIS consumers.

## Purpose
- Provide a simple UI for navigating field/cluster/well/wellbore hierarchies.
- Centralize the “current wellbore” selection used by other DWIS components.
- Serve as the front-end for the WellBoreSelector and related microservices.

## How It Works
- The app queries the external microservices (Field, Cluster, Well and WellBore).
- The user selects a wellbore through a cascading selector UI.
- The selection is published to the DWIS Blackboard for other services to consume.

## Notes
- Configuration values are loaded from `../home/Config.json` (see `WebApp/Configuration.cs` for details).
- This project does not define domain models; it depends on `ModelSharedOut` for generated types.

# Deployment
Run the following command to create a replicated DWIS Blackboard: 
```sh
docker run  -dit --name blackboard -P -p 48030:48030/tcp --hostname localhost  digiwells/ddhubserver:latest --useHub --hubURL https://dwis.digiwells.no/blackboard/applications
```
Run the following command to create a new docker container:
```docker
docker run -dit --name WellBoreSelector -p 5001:8080 -p 5002:443 -v c:\Volumes\DWISContextualDataWellBoreSelector:/home digiwells/dwiscontextualdatawellboreselectorwebapp:stable
```
The web application is then accessible here: http://localhost:5001/WellBoreSelector/webapp/Selector

## DWIS package versions
This host uses DWIS.RigOS.Common.Model 1.0.7 and explicitly pins
DWIS.Client.ReferenceImplementation.OPCFoundation 2.2.54. Restore packages from nuget.org.

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
also match their UUID. Searches run against already loaded choices; typing does not call
external services. Choose a listed result; arbitrary text is not a published selection.
Use Apply selections to publish the selected UUIDs. Existing field/cluster/well filters apply
before text filtering, including the field restriction on single wells.
