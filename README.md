# OpenHab.UniFiProxy
This .net core console application polls the UniFi CloudKey api (undocumented), monitoring for things like motion events, and updating OpenHAB items.

The app can poll for motion detected on cameras, camera uptime, and NVR disk space available. 

The app periodically polls the CloudKey. Based on your configuration (`jobs.json`), it pushes updates to OpenHAB items using the REST api.

## Usage

Once you've configured `appsettings.config`, you can run the app. It will connect to the CloudKey and display information about the CloudKey and attached cameras (including the camera ID's). 

Some settings require the ID of UniFi cameras. The ID's are not available from the UniFi protect web api. Use the output from the app to get the ID's you need to configure "jobs" for the cameras.

```
[13:54:42] Loading configuration
[13:54:43] App settings loaded
[13:54:43] Loading job settings
[13:54:43] Job poll interval: 3
[13:54:43] Jobs:
[13:54:43]   motion 5e3704ac001bda03e700041a: MyCamera1Motion
[13:54:43]   uptime 5e3704ac001bda03e700041a: MyCamera1UpSince
[13:54:43]   motion 5e375ada006a9a03e7000528: MyCamera2Motion
[13:54:43]   uptime 5e375ada006a9a03e7000528: MyCamera2UpSince
[13:54:43]   storage : CloudKeyStorage
[13:54:43] Connecting to NVR
[13:54:43] Authenticating.
[13:54:43] Authentication succeeded.
[13:54:43] NVR Info:
[13:54:43]   NVR name:      MyCloudKeyName
[13:54:43]   NVR IP:        192.168.1.100
[13:54:43]   NVR MAC:       6373D141D5E1
[13:54:43]   NVR firmware:  1.1.6
[13:54:43]   NVR time zone: America/Chicago
[13:54:43]   NVR storage:   74.7 used of 942.3 gb (7.9%)
[13:54:43] Cameras:
 | Name           | ID                       | IP           | MAC address  | Type         | State     | Last motion         | Wifi | 
 |---------------------------------------------------------------------------------------------------------------------------------| 
 | MyCamera1      | 5e4804ac001ceb03e700031a | 192.168.1.24 | ECECDDA8EBCB | UVC G3 Micro | CONNECTED | 2020-02-12 21:22:07 | -59  | 
 | MyCamera2      | 5e31aadaac001ce3e7000528 | 192.168.1.25 | 7483C23F34B9 | UVC G3 Flex  | CONNECTED | 2020-02-14 10:01:55 | 0    | 
 ```

The app requires settings in `appconfig.json` to define how to access the CloudKey and OpenHAB apis.

You then define a set of "jobs". A job is a definition that maps a bit of CloudKey data to an OpenHAB item, and an interval at which to poll for changes. 

## `appconfig.json`
Modify the `appconfig.json` replacing the values in `<>` with your values:

```
{
    "unifiApiUrl": "http://<NVR IP ADDRESS>:7080/api",
    "unifiUserName": "<NVR USER NAME>",
    "unifiPassword": "<NVR PASSWORD>",
    "openHabApiUrl": "http://<OPENHAB IP ADDRESS>:8080/rest",
    "statsFrequency": 600
}
```
## `jobs.json`
This is where you define the "tasks" that define the data from the CloudKey that you want to push to OpenHAB.

There are three types of job:
- **motion**: Updates an OpenHAB item when motion is detected on a camera.
- **uptime**: Updates an OpenHAB item with the current uptime for a camera. 
- **storage**: Updates an OpenHAB item with the current available disk space (%).

For all jobs, you define the `type`, as well as the name of the OpenHAB `item`, and the frequency with which to check for changes. 

Every job will update OpenHAB the first time its interval expires. After that, the OpenHAB item will not updated unless the value has changed since the last time it was polled.

For `motion` and `uptime` jobs, you must supply the `id` of the camera. This `id` can be found by looking at the table that is emitted when the app launches.

```
{
    "pollInterval": 3,
    "jobs": [
        {
            "type": "motion",
            "id": "<CAMERA ID>",
            "item": "<OPENHAB ITEM>",
            "frequency": 3
        },
        {
            "type": "uptime",
            "id": "<CAMERA ID>",
            "item": "<OPENHAB ITEM>",
            "frequency": 10
        },
        {
            "type": "storage",
            "id": "",
            "item": "<OPENHAB ITEM>",
            "frequency": 20
        }
    ]
}
```

### pollingInterval
This defines the interval at which the app will poll the CloudKey API. On each poll, new data is fetched from the CloudKey. Then, each `job` is examined to see if its frequency has timed out, and if so, OpenHab will be updated if that value has changed.

In other words, no matter what the setting for a `job`'s frequency, it will not be updated more often than the setting for `pollingInterval`. The maximum time between updates would be the `job`'s update frequency + the `pollingInterval`. Note that OpenHab will not be updated unless the value has changed since the last time the `job` ran.

This may seen confusing, but it is done that way to minimize chatter with the CloudKey and OpenHAB.

### **motion**
This is for monitoring motion detection on the camera. the app will update an OpenHAB item with the last time motion was detected. You can define as many of these as needed for multiple cameras.

### **uptime**
This is for monitoring uptime for the camera. the app will update an OpenHAB item with the date the camera last came online. You can define as many of these as needed for multiple cameras.

### **storage**
This is for monitoring the available storage on the CloudKey, as a number representing the percent available. Note that this job definition does not require the `id` to be set, since it refers to the CloudKey and not a camera.

## OpenHAB configuration

Here's an example of how to configure OpenHAB:

```
Group   UnifiVideo "UniFi Video" <camera>

Number CloudKeyStorage "Cloud Key storage used: [%.0f %%]" <pie> (UnifiVideo)
DateTime Camera1Motion "Camera 1 last motion" <time> (UnifiVideo)
DateTime Camera1UpSince "Camera 1 up since" <time> (UnifiVideo)
DateTime Camera2Motion "Camera 2 last motion" <time> (UnifiVideo)
DateTime Camera2UpSince "Camera 2 Room camera up since" <time> (UnifiVideo)
```

