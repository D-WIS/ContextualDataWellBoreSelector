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
