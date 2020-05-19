# Getting started

### Secrets 
[Configure](covi_configuration.md) application using your environment parameters

Mandatory fields to set:

##### 1. Server endpoint
```EndpointUrl```

Put here the url of your server instance.

##### 2. Push notifications
Configure azure notifications hub (it is the way back-end notifies the user about their infected status cahge) according to their guidelines and provide the following keys:
```
"PushNotifications_NotificationHubName": "{hub_name}",
"PushNotifications_ListenConnectionString": "{connection_string_debug}",
"PushNotifications_NotificationHubName_Prod": "{hub_name_prod}",
"PushNotifications_ListenConnectionString_Prod": "{connection_string_prod}"
```

### Tracing configuration
Have a look on possible configuration options in platform app.config file:

##### 1. Data persistence
Configure bluetooth tracing parameters depending on your needs:

```BluetoothTracing_DataExpirationTimeDays```

Defines the amount of time the device contact information being persisted on the device (data sync with back-end happens only on user status change)

```BluetoothTracing_DeviceThrottleTimeMinutes```

Defines amount of time, the device will be ignored after being processed for the first time (to avoid multiple device advertising processing while devices are nearby).

##### 2. Bluetooth parameters
```BluetoothTracing_ServiceUUID```

Defines the identifier of bluetooth service UUID the device is going to broadcast

```BluetoothTracing_CharacteristicUUID```

Defines the identifier of bluetooth service characteristic UUID the device is going to try read from to get device identifier.

### Debugging

In production mode app has all logs and crash reporting disabled for privacy concerns, for details about debugging, please [see](debugging.md).
