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
            public int? phyRate { get; set; }
        }

        public class Channel
        {
            public int id { get; set; }
            public string name { get; set; }
            public bool enabled { get; set; }
            public bool isRtspEnabled { get; set; }
            public object rtspAlias { get; set; }
            public int width { get; set; }
            public int height { get; set; }
            public int fps { get; set; }
            public int bitrate { get; set; }
            public int minBitrate { get; set; }
            public int maxBitrate { get; set; }
            public IList<int> fpsValues { get; set; }
            public int idrInterval { get; set; }
        }

        public class IspSettings
        {
            public string aeMode { get; set; }
            public string irLedMode { get; set; }
            public int irLedLevel { get; set; }
            public int wdr { get; set; }
            public int icrSensitivity { get; set; }
            public int brightness { get; set; }
            public int contrast { get; set; }
            public int hue { get; set; }
            public int saturation { get; set; }
            public int sharpness { get; set; }
            public int denoise { get; set; }
            public bool isFlippedVertical { get; set; }
            public bool isFlippedHorizontal { get; set; }
            public bool isAutoRotateEnabled { get; set; }
            public bool isLdcEnabled { get; set; }
            public bool is3dnrEnabled { get; set; }
            public bool isExternalIrEnabled { get; set; }
            public bool isAggressiveAntiFlickerEnabled { get; set; }
            public bool isPauseMotionEnabled { get; set; }
            public int dZoomCenterX { get; set; }
            public int dZoomCenterY { get; set; }
            public int dZoomScale { get; set; }
            public int dZoomStreamId { get; set; }
            public string focusMode { get; set; }
            public int focusPosition { get; set; }
            public int touchFocusX { get; set; }
            public int touchFocusY { get; set; }
            public int zoomPosition { get; set; }
        }

        public class TalkbackSettings
        {
            public string typeFmt { get; set; }
            public string typeIn { get; set; }
            public string bindAddr { get; set; }
            public int bindPort { get; set; }
            public string filterAddr { get; set; }
            public int? filterPort { get; set; }
            public int channels { get; set; }
            public int samplingRate { get; set; }
            public int bitsPerSample { get; set; }
            public int quality { get; set; }
        }

        public class OsdSettings
        {
            public bool isNameEnabled { get; set; }
            public bool isDateEnabled { get; set; }
            public bool isLogoEnabled { get; set; }
            public bool isDebugEnabled { get; set; }
        }

        public class LedSettings
        {
            public bool isEnabled { get; set; }
            public int blinkRate { get; set; }
        }

        public class SpeakerSettings
        {
            public bool isEnabled { get; set; }
            public bool areSystemSoundsEnabled { get; set; }
            public int volume { get; set; }
        }

        public class RecordingSettings
        {
            public int prePaddingSecs { get; set; }
            public int postPaddingSecs { get; set; }
            public int minMotionEventTrigger { get; set; }
            public int endMotionEventDelay { get; set; }
            public bool suppressIlluminationSurge { get; set; }
            public string mode { get; set; }
            public string geofencing { get; set; }
            public bool useNewMotionAlgorithm { get; set; }
            public bool enablePirTimelapse { get; set; }
        }

        public class MotionZone
        {
            public string name { get; set; }
            public string color { get; set; }
            public IList<IList<int>> points { get; set; }
            public int sensitivity { get; set; }
        }

        public class Wifi
        {
            public int? channel { get; set; }
            public int? frequency { get; set; }
            public int? linkSpeedMbps { get; set; }
            public int? signalQuality { get; set; }
            public int? signalStrength { get; set; }
        }

        public class Battery
        {
            public int? percentage { get; set; }
            public bool isCharging { get; set; }
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
            public long rxBytes { get; set; }
            public long? txBytes { get; set; }
            public Wifi wifi { get; set; }
            public Battery battery { get; set; }
            public Video video { get; set; }
            public int wifiQuality { get; set; }
            public int wifiStrength { get; set; }
        }

        public class NvrFeatureFlags
        {
            public bool beta { get; set; }
            public bool dev { get; set; }
        }

        public class CameraFeatureFlags
        {
            public bool canAdjustIrLedLevel { get; set; }
            public bool canMagicZoom { get; set; }
            public bool canOpticalZoom { get; set; }
            public bool canTouchFocus { get; set; }
            public bool hasAccelerometer { get; set; }
            public bool hasAec { get; set; }
            public bool hasBattery { get; set; }
            public bool hasBluetooth { get; set; }
            public bool hasChime { get; set; }
            public bool hasExternalIr { get; set; }
            public bool hasIcrSensitivity { get; set; }
            public bool hasLdc { get; set; }
            public bool hasLedIr { get; set; }
            public bool hasLedStatus { get; set; }
            public bool hasLineIn { get; set; }
            public bool hasMic { get; set; }
            public bool hasPrivacyMask { get; set; }
            public bool hasRtc { get; set; }
            public bool hasSdCard { get; set; }
            public bool hasSpeaker { get; set; }
            public bool hasWifi { get; set; }
            public bool hasHdr { get; set; }
            public bool hasAutoICROnly { get; set; }
            public bool hasMotionZones { get; set; }
        }

        public class PirSettings
        {
            public int pirSensitivity { get; set; }
            public int pirMotionClipLength { get; set; }
            public int timelapseFrameInterval { get; set; }
            public int timelapseTransferInterval { get; set; }
        }

        public class WifiConnectionState
        {
            public int? channel { get; set; }
            public int? frequency { get; set; }
            public int? phyRate { get; set; }
            public int? signalQuality { get; set; }
            public int? signalStrength { get; set; }
        }

        public class Camera
        {
            public bool isDeleting { get; set; }
            public string mac { get; set; }
            public string host { get; set; }
            public string connectionHost { get; set; }
            public string type { get; set; }
            public string name { get; set; }
            public long upSince { get; set; }
            public long? lastSeen { get; set; }
            public long connectedSince { get; set; }
            public string state { get; set; }
            public string hardwareRevision { get; set; }
            public string firmwareVersion { get; set; }
            public string firmwareBuild { get; set; }
            public bool isUpdating { get; set; }
            public bool isAdopting { get; set; }
            public bool isManaged { get; set; }
            public bool isProvisioned { get; set; }
            public bool isRebooting { get; set; }
            public bool isSshEnabled { get; set; }
            public bool canManage { get; set; }
            public bool isHidden { get; set; }
            public long lastMotion { get; set; }
            public int micVolume { get; set; }
            public bool isMicEnabled { get; set; }
            public bool isRecording { get; set; }
            public bool isMotionDetected { get; set; }
            public bool isAttemptingToConnect { get; set; }
            public int? phyRate { get; set; }
            public bool hdrMode { get; set; }
            public bool? isProbingForWifi { get; set; }
            public object apMac { get; set; }
            public object apRssi { get; set; }
            public object elementInfo { get; set; }
            public int chimeDuration { get; set; }
            public bool isDark { get; set; }
            public int motionStartCalls { get; set; }
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
            public bool isConnected { get; set; }
            public string platform { get; set; }
            public bool hasSpeaker { get; set; }
            public bool hasWifi { get; set; }
            public long audioBitrate { get; set; }
            public string modelKey { get; set; }
        }

        public class Flags
        {
        }

        public class Web
        {
            [JsonPropertyName("liveview.includeGlobal")]
            public bool liveview_includeGlobal { get; set; }
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
            public bool isAway { get; set; }
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
            public long lastLoginTime { get; set; }
            public bool isOwner { get; set; }
            public string localUsername { get; set; }
            public bool enableNotifications { get; set; }
            public bool syncSso { get; set; }
            public Settings settings { get; set; }
            public IList<string> groups { get; set; }
            public CloudAccount cloudAccount { get; set; }
            public IList<AlertRule> alertRules { get; set; }
            public string id { get; set; }
            public bool hasAcceptedInvite { get; set; }
            public string role { get; set; }
            public IList<string> allPermissions { get; set; }
            public string modelKey { get; set; }
        }

        public class Group
        {
            public string name { get; set; }
            public IList<string> permissions { get; set; }
            public string type { get; set; }
            public bool isDefault { get; set; }
            public string id { get; set; }
            public string modelKey { get; set; }
        }

        public class Slot
        {
            public IList<string> cameras { get; set; }
            public string cycleMode { get; set; }
            public int cycleInterval { get; set; }
        }

        public class Liveview
        {
            public string name { get; set; }
            public bool isGlobal { get; set; }
            public int layout { get; set; }
            public IList<Slot> slots { get; set; }
            public string owner { get; set; }
            public string id { get; set; }
            public string modelKey { get; set; }
        }

        public class Ports
        {
            public int ump { get; set; }
            public int http { get; set; }
            public int https { get; set; }
            public int rtsp { get; set; }
            public int rtmp { get; set; }
            public int elementsWss { get; set; }
            public int cameraHttps { get; set; }
            public int cameraTcp { get; set; }
            public int liveWs { get; set; }
            public int liveWss { get; set; }
            public int tcpStreams { get; set; }
            public int emsCLI { get; set; }
            public int emsLiveFLV { get; set; }
            public int cameraEvents { get; set; }
            public int discoveryClient { get; set; }
        }

        public class WifiSettings
        {
            public bool useThirdPartyWifi { get; set; }
            public object ssid { get; set; }
            public object password { get; set; }
        }

        public class LocationSettings
        {
            public bool isAway { get; set; }
            public bool isGeofencingEnabled { get; set; }
            public double latitude { get; set; }
            public double longitude { get; set; }
            public int radius { get; set; }
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
            public int RPM { get; set; }
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
            public bool canAutoUpdate { get; set; }
            public bool isStatsGatheringEnabled { get; set; }
            public string timezone { get; set; }
            public string version { get; set; }
            public string firmwareVersion { get; set; }
            public object uiVersion { get; set; }
            public string hardwarePlatform { get; set; }
            public Ports ports { get; set; }
            public object setupCode { get; set; }
            public int uptime { get; set; }
            public long lastSeen { get; set; }
            public bool isUpdating { get; set; }
            public long lastUpdateAt { get; set; }
            public bool isConnectedToCloud { get; set; }
            public object cloudConnectionError { get; set; }
            public bool isStation { get; set; }
            public bool enableAutomaticBackups { get; set; }
            public bool enableStatsReporting { get; set; }
            public bool isSshEnabled { get; set; }
            public object errorCode { get; set; }
            public string releaseChannel { get; set; }
            public object availableUpdate { get; set; }
            public IList<string> hosts { get; set; }
            public string hardwareId { get; set; }
            public string hardwareRevision { get; set; }
            public int hostType { get; set; }
            public bool isHardware { get; set; }
            public string timeFormat { get; set; }
            public object recordingRetentionDurationMs { get; set; }
            public bool enableCrashReporting { get; set; }
            public bool disableAudio { get; set; }
            public WifiSettings wifiSettings { get; set; }
            public LocationSettings locationSettings { get; set; }
            public NvrFeatureFlags featureFlags { get; set; }
            public StorageInfo storageInfo { get; set; }
            public string id { get; set; }
            public bool isAdopted { get; set; }
            public bool isAway { get; set; }
            public bool isSetup { get; set; }
            public string network { get; set; }
            public string type { get; set; }
            public long upSince { get; set; }
            public string modelKey { get; set; }
        }

    }

}