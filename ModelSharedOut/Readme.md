# Access to External Micro-service Data Structures
This project produces shared model types used by the WebApp and any other consumers that need to talk to the
external DWIS micro-services (Field, Cluster, Well and WellBore).

## Purpose
- Provide a single, consistent set of C# data structures generated from the external JSON schemas.
- Ensure serialization and deserialization are aligned across services and clients.
- Avoid duplicating model definitions in multiple projects.

## How It Works
- Place or update the JSON schemas of the external data in `json-schemas/`.
- Run the generator program in this project.
- The generated model types are then referenced by the WebApp for API clients and payloads.

## Notes
- This project does not host a server or define its own domain model.
- The `Server/` folder exists only to satisfy tooling/solution structure expectations.

