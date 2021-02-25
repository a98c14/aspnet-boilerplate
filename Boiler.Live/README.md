# Boiler Live

Provides various websocket endpoints using SignalR

## Usage

Add `endpoints.MapBoilerLive()` to startup endpoints and add `services.AddBoilerLive()`
for service configuration.

Endpoints are listed below

| Endpoint            | Description                                  |
| ------------------- | -------------------------------------------- |
| `notifications-hub` | Provides notification mechanism for clients. |
| `logging-hub`       | Logs the user behavior                       |