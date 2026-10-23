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
