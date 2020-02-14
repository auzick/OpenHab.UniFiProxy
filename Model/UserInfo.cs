using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenHab.UniFiProxy.Model
{
    public class UserInfo
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

        public class Camera
        {
            public IList<object> connectDisconnect { get; set; }
            public IList<string> motion { get; set; }
            public object camera { get; set; }
        }

        public class User
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
            public IList<Camera> cameras { get; set; }
            public IList<User> users { get; set; }
            public string geofencing { get; set; }
        }

    }

}