﻿# OpenHab.UniFiProxy
This .NET Core console application polls the UniFi CloudKey api (undocumented), monitoring for things like motion events, and updates OpenHAB items.

The app can poll for motion detected on cameras, camera uptime, and NVR disk space available. 

The app periodically polls the CloudKey. Based on your configuration (`jobs.json`), it pushes updates to OpenHAB items using the REST api.

### Yes, it's a c# .NET Core console app.

I know I'm in the minority, running OpenHAB on Windows. I'm also in the minority by developing for HA using c# and .NET Core. What can I say, I'm staying in my technology comfort zone. But these days, .NET Core is cross-platform, so why not?

Worry not, there are release builds for both Windows and Linux x64 systems.

## Downloading

The Release directory contains zip files with releases for various systems

- `OpenHabUnifiProxy-win-x64.zip` for Windows systems with [.NET Core 3.1](https://dotnet.microsoft.com/download/dotnet-core/3.1) framework installed
- `OpenHabUnifiProxy-win-x64-standalone.zip` for Windows systems that don't have .NET Core 3.1
- `OpenHabUnifiProxy-linux-x64.zip` for Linux systems with [.NET Core 3.1](https://dotnet.microsoft.com/download/dotnet-core/3.1) framework installed
- `OpenHabUnifiProxy-linux-x64-standalone.zip` for Linux systems that don't have .NET Core 3.1


## Usage

Once you've configured `appsettings.config`, you can run the app. It will connect to the CloudKey and display information about the CloudKey and attached cameras (including the camera ID's). 

Sample output:
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
The first four settings should be self-explanatory. The `statsFrequency` setting sets how often (in seconds) the app will update statistics in the console window. 

Once you've done this, you can run the app to get info about the NVR, including the camera ID's which you'll need for the next step.

## `jobs.json`
This is where you define the "tasks" that define the data from the CloudKey that you want to push to OpenHAB.

There are three types of job:
- **motion**: Updates an OpenHAB item when motion is detected on a camera.
- **state**: Updates an OpenHAB item with the current camera state (CONNECTED or DISCONNECTED).
- **uptime**: Updates an OpenHAB item with the current uptime for a camera. 
- **storage**: Updates an OpenHAB item with the current available disk space (%).

For all jobs, you define the `type`, as well as the name of the OpenHAB `item`, and the `frequency` (in seconds) with which to check for changes. 

When the program starts, every job will update OpenHAB the first time it's interval expires. After that, the OpenHAB item will not updated unless the value has changed since the last time it was polled.

For camera events, the `id` value can be either the camera's id or it's name. Using the id avoids issues if you ever happen to rename the camera. The id can be found by looking at the table that is emitted when the app launches.

Example `jobs.json`:
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
### `jobs.json` settings:

#### pollingInterval
This defines the interval (in seconds) at which the app will poll the CloudKey API. On each poll, new data is fetched from the CloudKey. Then, each `job` is examined to see if its frequency has timed out, and if so, OpenHab will be updated if that value has changed.

In other words, no matter what the setting for a `job`'s frequency, it will not be updated more often than the setting for `pollingInterval`. The maximum time between updates would be the `job`'s update frequency + the `pollingInterval`. Note that OpenHab will not be updated unless the value has changed since the last time the `job` ran.

This may seen confusing, but it is done that way to minimize chatter with the CloudKey and OpenHAB.

#### **motion**
This is for monitoring motion detection on the camera. The app will update an OpenHAB item with the last time motion was detected. You can define as many of these as needed for multiple cameras.

#### **state**
This is for monitoring state of the camera. The app will update an OpenHAB item with the current state of the camera (CONNECTED or DISCONNECTED). You can define as many of these as needed for multiple cameras.

#### **uptime**
This is for monitoring uptime for the camera. the app will update an OpenHAB item with the date the camera last came online. You can define as many of these as needed for multiple cameras.

#### **storage**
This is for monitoring the available storage on the CloudKey, as a number representing the percent available. Note that this job definition does not require the `id` to be set, since it refers to the CloudKey and not a camera.

## OpenHAB configuration

Here's an example of how to configure OpenHAB:

```
Group   UnifiVideo "UniFi Video" <camera>

Number CloudKeyStorage "Cloud Key storage used: [%.0f %%]" <pie> (UnifiVideo)

DateTime Camera1Motion "Camera 1 last motion" <motion> (UnifiVideo)
DateTime Camera1UpSince "Camera 1 up since" <time> (UnifiVideo)
String Camera1State "Camera 1 state" <network> (UnifiVideo)

DateTime Camera2Motion "Camera 2 last motion" <motion> (UnifiVideo)
DateTime Camera2UpSince "Camera 2 up since" <time> (UnifiVideo)
String Camera2State "Camera 2 state" <network> (UnifiVideo)
```

