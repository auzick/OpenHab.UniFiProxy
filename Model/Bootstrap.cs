using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenHab.UniFiProxy.Model
{
    public class Bootstrap
    {
        public string authUserId { get; set; }
        public string accessKey { get; set; }
        public IList<Camera> cameras { get; set; }
        public IList<User> users { get; set; }
        public IList<Group> groups { get; set; }
        public IList<Liveview> liveviews { get; set; }
        public Nvr nvr { get; set; }
        public string lastUpdateId { get; set; }
        public string cloudPortalUrl { get; set; }
        public IList<object> viewers { get; set; }
        public IList<object> lights { get; set; }
        public IList<object> beacons { get; set; }
        public IList<object> sensors { get; set; }

        public class WiredConnectionState
        {
            public long? phyRate { get; set; }
        }

        public class Channel
        {
            public long? id { get; set; }
            public string name { get; set; }
            public bool? enabled { get; set; }
            public bool? isRtspEnabled { get; set; }
            public object rtspAlias { get; set; }
            public long? width { get; set; }
            public long? height { get; set; }
            public long? fps { get; set; }
            public long? bitrate { get; set; }
            public long? minBitrate { get; set; }
            public long? maxBitrate { get; set; }
            public IList<int> fpsValues { get; set; }
            public long? idrInterval { get; set; }
        }

        public class IspSettings
        {
            public string aeMode { get; set; }
            public string irLedMode { get; set; }
            public long? irLedLevel { get; set; }
            public long? wdr { get; set; }
            public long? icrSensitivity { get; set; }
            public long? brightness { get; set; }
            public long? contrast { get; set; }
            public long? hue { get; set; }
            public long? saturation { get; set; }
            public long? sharpness { get; set; }
            public long? denoise { get; set; }
            public bool? isFlippedVertical { get; set; }
            public bool? isFlippedHorizontal { get; set; }
            public bool? isAutoRotateEnabled { get; set; }
            public bool? isLdcEnabled { get; set; }
            public bool? is3dnrEnabled { get; set; }
            public bool? isExternalIrEnabled { get; set; }
            public bool? isAggressiveAntiFlickerEnabled { get; set; }
            public bool? isPauseMotionEnabled { get; set; }
            public long? dZoomCenterX { get; set; }
            public long? dZoomCenterY { get; set; }
            public long? dZoomScale { get; set; }
            public long? dZoomStreamId { get; set; }
            public string focusMode { get; set; }
            public long? focusPosition { get; set; }
            public long? touchFocusX { get; set; }
            public long? touchFocusY { get; set; }
            public long? zoomPosition { get; set; }
        }

        public class TalkbackSettings
        {
            public string typeFmt { get; set; }
            public string typeIn { get; set; }
            public string bindAddr { get; set; }
            public long? bindPort { get; set; }
            public string filterAddr { get; set; }
            public long? filterPort { get; set; }
            public long? channels { get; set; }
            public long? samplingRate { get; set; }
            public long? bitsPerSample { get; set; }
            public long? quality { get; set; }
        }

        public class OsdSettings
        {
            public bool? isNameEnabled { get; set; }
            public bool? isDateEnabled { get; set; }
            public bool? isLogoEnabled { get; set; }
            public bool? isDebugEnabled { get; set; }
        }

        public class LedSettings
        {
            public bool? isEnabled { get; set; }
            public long? blinkRate { get; set; }
        }

        public class SpeakerSettings
        {
            public bool? isEnabled { get; set; }
            public bool? areSystemSoundsEnabled { get; set; }
            public long? volume { get; set; }
        }

        public class RecordingSettings
        {
            public long? prePaddingSecs { get; set; }
            public long? postPaddingSecs { get; set; }
            public long? minMotionEventTrigger { get; set; }
            public long? endMotionEventDelay { get; set; }
            public bool? suppressIlluminationSurge { get; set; }
            public string mode { get; set; }
            public string geofencing { get; set; }
            public bool? useNewMotionAlgorithm { get; set; }
            public bool? enablePirTimelapse { get; set; }
        }

        public class MotionZone
        {
            public string name { get; set; }
            public string color { get; set; }
            public IList<IList<int>> points { get; set; }
            public long? sensitivity { get; set; }
        }

        public class Wifi
        {
            public long? channel { get; set; }
            public long? frequency { get; set; }
            public long? linkSpeedMbps { get; set; }
            public long? signalQuality { get; set; }
            public long? signalStrength { get; set; }
        }

        public class Battery
        {
            public long? percentage { get; set; }
            public bool? isCharging { get; set; }
            public string sleepState { get; set; }
        }

        public class Video
        {
            public long? recordingStart { get; set; }
            public long? recordingEnd { get; set; }
            public long? recordingStartLQ { get; set; }
            public long? recordingEndLQ { get; set; }
            public long? timelapseStart { get; set; }
            public long? timelapseEnd { get; set; }
            public long? timelapseStartLQ { get; set; }
            public long? timelapseEndLQ { get; set; }
        }

        public class Stats
        {
            public long? rxBytes { get; set; }
            public long? txBytes { get; set; }
            public Wifi wifi { get; set; }
            public Battery battery { get; set; }
            public Video video { get; set; }
            public long? wifiQuality { get; set; }
            public long? wifiStrength { get; set; }
        }

        public class NvrFeatureFlags
        {
            public bool? beta { get; set; }
            public bool? dev { get; set; }
        }

        public class CameraFeatureFlags
        {
            public bool? canAdjustIrLedLevel { get; set; }
            public bool? canMagicZoom { get; set; }
            public bool? canOpticalZoom { get; set; }
            public bool? canTouchFocus { get; set; }
            public bool? hasAccelerometer { get; set; }
            public object hasAec { get; set; }
            public bool? hasBattery { get; set; }
            public bool? hasBluetooth { get; set; }
            public bool? hasChime { get; set; }
            public bool? hasExternalIr { get; set; }
            public bool? hasIcrSensitivity { get; set; }
            public bool? hasLdc { get; set; }
            public bool? hasLedIr { get; set; }
            public bool? hasLedStatus { get; set; }
            public bool? hasLineIn { get; set; }
            public bool? hasMic { get; set; }
            public bool? hasPrivacyMask { get; set; }
            public bool? hasRtc { get; set; }
            public bool? hasSdCard { get; set; }
            public bool? hasSpeaker { get; set; }
            public bool? hasWifi { get; set; }
            public bool? hasHdr { get; set; }
            public bool? hasAutoICROnly { get; set; }
            public bool? hasMotionZones { get; set; }
        }

        public class PirSettings
        {
            public long? pirSensitivity { get; set; }
            public long? pirMotionClipLength { get; set; }
            public long? timelapseFrameInterval { get; set; }
            public long? timelapseTransferInterval { get; set; }
        }

        public class WifiConnectionState
        {
            public long? channel { get; set; }
            public long? frequency { get; set; }
            public long? phyRate { get; set; }
            public long? signalQuality { get; set; }
            public long? signalStrength { get; set; }
        }

        public class Camera
        {
            public bool? isDeleting { get; set; }
            public string mac { get; set; }
            public string host { get; set; }
            public string connectionHost { get; set; }
            public string type { get; set; }
            public string name { get; set; }
            public long? upSince { get; set; }
            public long? lastSeen { get; set; }
            public long? connectedSince { get; set; }
            public string state { get; set; }
            public string hardwareRevision { get; set; }
            public string firmwareVersion { get; set; }
            public string firmwareBuild { get; set; }
            public bool? isUpdating { get; set; }
            public bool? isAdopting { get; set; }
            public bool? isManaged { get; set; }
            public bool? isProvisioned { get; set; }
            public bool? isRebooting { get; set; }
            public bool? isSshEnabled { get; set; }
            public bool? canManage { get; set; }
            public bool? isHidden { get; set; }
            public long? lastMotion { get; set; }
            public long? micVolume { get; set; }
            public bool? isMicEnabled { get; set; }
            public bool? isRecording { get; set; }
            public bool? isMotionDetected { get; set; }
            public bool? isAttemptingToConnect { get; set; }
            public long? phyRate { get; set; }
            public bool? hdrMode { get; set; }
            public bool? isProbingForWifi { get; set; }
            public object apMac { get; set; }
            public object apRssi { get; set; }
            public object elementInfo { get; set; }
            public long? chimeDuration { get; set; }
            public bool? isDark { get; set; }
            public long? motionStartCalls { get; set; }
            public object lastRing { get; set; }
            public WiredConnectionState wiredConnectionState { get; set; }
            public IList<Channel> channels { get; set; }
            public IspSettings ispSettings { get; set; }
            public TalkbackSettings talkbackSettings { get; set; }
            public OsdSettings osdSettings { get; set; }
            public LedSettings ledSettings { get; set; }
            public SpeakerSettings speakerSettings { get; set; }
            public RecordingSettings recordingSettings { get; set; }
            public object recordingSchedule { get; set; }
            public IList<MotionZone> motionZones { get; set; }
            public IList<object> privacyZones { get; set; }
            public Stats stats { get; set; }
            public CameraFeatureFlags featureFlags { get; set; }
            public PirSettings pirSettings { get; set; }
            public WifiConnectionState wifiConnectionState { get; set; }
            public string id { get; set; }
            public bool? isConnected { get; set; }
            public string platform { get; set; }
            public bool? hasSpeaker { get; set; }
            public bool? hasWifi { get; set; }
            public long? audioBitrate { get; set; }
            public string modelKey { get; set; }
        }

        public class Flags
        {
        }

        public class Web
        {
            [JsonPropertyName("liveview.includeGlobal")]
            public bool? liveview_includeGlobal { get; set; }
            [JsonPropertyName("elements.viewmode")]
            public string elements_viewmode { get; set; }
        }

        public class Settings
        {
            public Flags flags { get; set; }
            public Web web { get; set; }
        }

        public class Location
        {
            public bool? isAway { get; set; }
            public object latitude { get; set; }
            public object longitude { get; set; }
        }

        public class CloudAccount
        {
            public string name { get; set; }
            public string email { get; set; }
            public string cloudId { get; set; }
            public object profileImg { get; set; }
            public Location location { get; set; }
        }

        public class Schedule
        {
            public IList<object> items { get; set; }
        }

        public class System
        {
            public IList<string> connectDisconnect { get; set; }
            public IList<string> update { get; set; }
        }

        public class AlertRulesCamera
        {
            public IList<object> connectDisconnect { get; set; }
            public IList<string> motion { get; set; }
            public object camera { get; set; }
        }

        public class AlertRuleUser
        {
            public IList<string> arrive { get; set; }
            public IList<string> depart { get; set; }
            public string user { get; set; }
        }

        public class AlertRule
        {
            public string id { get; set; }
            public string name { get; set; }
            public string when { get; set; }
            public Schedule schedule { get; set; }
            public System system { get; set; }
            public IList<AlertRulesCamera> cameras { get; set; }
            public IList<AlertRuleUser> users { get; set; }
            public string geofencing { get; set; }
        }

        public class User
        {
            public IList<string> permissions { get; set; }
            public string lastLoginIp { get; set; }
            public long? lastLoginTime { get; set; }
            public bool? isOwner { get; set; }
            public string localUsername { get; set; }
            public bool? enableNotifications { get; set; }
            public bool? syncSso { get; set; }
            public Settings settings { get; set; }
            public IList<string> groups { get; set; }
            public CloudAccount cloudAccount { get; set; }
            public IList<AlertRule> alertRules { get; set; }
            public string id { get; set; }
            public bool? hasAcceptedInvite { get; set; }
            public string role { get; set; }
            public IList<string> allPermissions { get; set; }
            public string modelKey { get; set; }
        }

        public class Group
        {
            public string name { get; set; }
            public IList<string> permissions { get; set; }
            public string type { get; set; }
            public bool? isDefault { get; set; }
            public string id { get; set; }
            public string modelKey { get; set; }
        }

        public class Slot
        {
            public IList<string> cameras { get; set; }
            public string cycleMode { get; set; }
            public long? cycleInterval { get; set; }
        }

        public class Liveview
        {
            public string name { get; set; }
            public bool? isGlobal { get; set; }
            public long? layout { get; set; }
            public IList<Slot> slots { get; set; }
            public string owner { get; set; }
            public string id { get; set; }
            public string modelKey { get; set; }
        }

        public class Ports
        {
            public long? ump { get; set; }
            public long? http { get; set; }
            public long? https { get; set; }
            public long? rtsp { get; set; }
            public long? rtmp { get; set; }
            public long? elementsWss { get; set; }
            public long? cameraHttps { get; set; }
            public long? cameraTcp { get; set; }
            public long? liveWs { get; set; }
            public long? liveWss { get; set; }
            public long? tcpStreams { get; set; }
            public long? emsCLI { get; set; }
            public long? emsLiveFLV { get; set; }
            public long? cameraEvents { get; set; }
            public long? discoveryClient { get; set; }
        }

        public class WifiSettings
        {
            public bool? useThirdPartyWifi { get; set; }
            public object ssid { get; set; }
            public object password { get; set; }
        }

        public class LocationSettings
        {
            public bool? isAway { get; set; }
            public bool? isGeofencingEnabled { get; set; }
            public double latitude { get; set; }
            public double longitude { get; set; }
            public long? radius { get; set; }
        }

        public class StorageUtilization
        {
            public string type { get; set; }
            public long spaceUsed { get; set; }
        }

        public class HardDrive
        {
            public string status { get; set; }
            public string name { get; set; }
            public string serial { get; set; }
            public string firmware { get; set; }
            public long size { get; set; }
            public long? RPM { get; set; }
            public string ataVersion { get; set; }
            public string sataVersion { get; set; }
            public string health { get; set; }
        }

        public class StorageInfo
        {
            public long totalSize { get; set; }
            public long totalSpaceUsed { get; set; }
            public IList<StorageUtilization> storageUtilization { get; set; }
            public IList<HardDrive> hardDrives { get; set; }
        }

        public class Nvr
        {
            public string mac { get; set; }
            public string host { get; set; }
            public string name { get; set; }
            public bool? canAutoUpdate { get; set; }
            public bool? isStatsGatheringEnabled { get; set; }
            public string timezone { get; set; }
            public string version { get; set; }
            public string firmwareVersion { get; set; }
            public object uiVersion { get; set; }
            public string hardwarePlatform { get; set; }
            public Ports ports { get; set; }
            public object setupCode { get; set; }
            public long? uptime { get; set; }
            public long lastSeen { get; set; }
            public bool? isUpdating { get; set; }
            public long lastUpdateAt { get; set; }
            public bool? isConnectedToCloud { get; set; }
            public object cloudConnectionError { get; set; }
            public bool? isStation { get; set; }
            public bool? enableAutomaticBackups { get; set; }
            public bool? enableStatsReporting { get; set; }
            public bool? isSshEnabled { get; set; }
            public object errorCode { get; set; }
            public string releaseChannel { get; set; }
            public object availableUpdate { get; set; }
            public IList<string> hosts { get; set; }
            public string hardwareId { get; set; }
            public string hardwareRevision { get; set; }
            public long? hostType { get; set; }
            public bool? isHardware { get; set; }
            public string timeFormat { get; set; }
            public object recordingRetentionDurationMs { get; set; }
            public bool? enableCrashReporting { get; set; }
            public bool? disableAudio { get; set; }
            public WifiSettings wifiSettings { get; set; }
            public LocationSettings locationSettings { get; set; }
            public NvrFeatureFlags featureFlags { get; set; }
            public StorageInfo storageInfo { get; set; }
            public string id { get; set; }
            public bool? isAdopted { get; set; }
            public bool? isAway { get; set; }
            public bool? isSetup { get; set; }
            public string network { get; set; }
            public string type { get; set; }
            public long upSince { get; set; }
            public string modelKey { get; set; }
        }

    }

}