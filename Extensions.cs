using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Web;
using OpenHab.UniFiProxy.Model;

namespace OpenHab.UniFiProxy
{
    public static class Extensions
    {
        public static T ParseResponseJson<T>(this HttpResponseMessage response)
        {
            return response.Content.ReadAsStringAsync().Result.ParseJson<T>();
        }

        public static T ParseJson<T>(this string json)
        {
            var options = new JsonSerializerOptions
            {
                AllowTrailingCommas = true
            };
            return JsonSerializer.Deserialize<T>(json);
        }

        // Assumes unix time is in milliseconds
        public static DateTime FromUnixTime(this double unixTime)
        {
            return FromUnixTime((long)unixTime);
            // return epoch.AddMilliseconds(unixTime).ToLocalTime();
        }

        public static DateTime FromUnixTime(this long unixTime)
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(unixTime).LocalDateTime;
        }

        public static DateTime FromUnixTime(this long? unixTime)
        {
            return ((long)unixTime).FromUnixTime();
        }

        public static long ToUnixTimestamp(this DateTime date)
        {
            return new DateTimeOffset(date).ToUnixTimeMilliseconds();
        }

        public static string ToOpenHabTimeString(this long unixDate)
        {
            return FromUnixTime(unixDate).ToOpenHabTimeString();
        }

        public static string ToOpenHabTimeString(this DateTime date)
        {
            return date.ToString("yyyy-MM-ddThh:mm:ss");
        }

        public static bool TryGetCamera(this Bootstrap bootstrap, string identifier, out Bootstrap.Camera camera)
        {
            if (!bootstrap.cameras.Any(c =>
                c.id == identifier || c.name == identifier
                ))
            {
                camera = null;
                return false;
            }
            camera = bootstrap.cameras.First(c =>
                c.id == identifier || c.name == identifier
                );
            return true;
        }

        public static Uri SetQueryValue(this Uri uri, string name, string value)
        {
            var builder = new UriBuilder(uri);
            var nameValues = HttpUtility.ParseQueryString(uri.Query);
            nameValues.Set(name, value);
            string url = uri.AbsolutePath;
            builder.Query = nameValues.ToString();
            return builder.Uri;
        }

    }
}