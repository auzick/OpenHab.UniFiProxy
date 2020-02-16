using System;
using System.Net.Http;
using System.Text.Json;

namespace OpenHab.UniFiProxy
{
    public static class Extensions
    {
        public static T ParseResponseJson<T>(this HttpResponseMessage response)
        {
            return response.Content.ReadAsStringAsync().Result.ParseJson<T>();
            // return ParseJson<T>(json);
        }

        public static T ParseJson<T>(this string json)
        {
            var options = new JsonSerializerOptions
            {
                AllowTrailingCommas = true
            };
            return JsonSerializer.Deserialize<T>(json);
        }

        public static DateTime FromUnixTime(this double unixTime)
        {
            return epoch.AddMilliseconds(unixTime).ToLocalTime();
        }

        public static DateTime FromUnixTime(this long unixTime)
        {
            return epoch.AddMilliseconds(unixTime).ToLocalTime();
        }

        private static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);


    }
}