using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using OpenHab.UniFiProxy.Logging;
using OpenHab.UniFiProxy.Model;

namespace OpenHab.UniFiProxy.Clients
{
    public interface IUniFiClient
    {
        UserInfo AuthenticatedUser { get; }

        Bootstrap GetBootstrap();
    }

    public class UniFiClient : IUniFiClient
    {
        private HttpClient _client { get; set; }
        private IAppConfiguration _config { get; set; }
        private ICounters _counters { get; set; }
        private string _bearerToken { get; set; }

        public UserInfo AuthenticatedUser { get; private set; }

        public UniFiClient(ICounters counters, IAppConfiguration config, HttpClient client)
        {
            _counters = counters;
            _client = client;
            _config = config;
            Log.Write("UniFiClient initialized.");
        }

        public Bootstrap GetBootstrap()
        {
            var url = $"{_config.UnifiApiUrl}/bootstrap";
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            var response = CallUniFiApi(req);
            if (response == null || !response.IsSuccessStatusCode)
            {
                Log.Write(
                    (response != null)
                    ? $"Bootstrap failed. {response?.ReasonPhrase} ({response?.StatusCode})"
                    : "Bootstrap failed."
                    );
                return null;
            }

            var json = response.Content.ReadAsStringAsync().Result;
            try { File.WriteAllText(Path.Join(_config.AppPath, "bootstrap.json"), json); } catch { }

            _counters.BootstrapCalls++;

            return json.ParseJson<Bootstrap>();
        }

        protected HttpResponseMessage CallUniFiApi(HttpRequestMessage request, bool AddToken = true)
        {
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (string.IsNullOrWhiteSpace(_bearerToken))
            {
                if (!Authenticate())
                {
                    //Log.Write("Cannot call UniFi API. Could not authenticate.");
                    return null;
                }
            }

            if (AddToken)
            {
                _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_bearerToken}");
            }

            //Log.Write($"UniFi API call: {request.RequestUri.AbsoluteUri}");
            var result = _client.SendAsync(request).Result;
            if (!result.IsSuccessStatusCode)
            {
                if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    // Try one more time ... token may be stale 
                    Log.Write($"Failed. Reauthenticating.");
                    if (!Authenticate())
                    {
                        Log.Write("Cannot call UniFi API. Could not authenticate.");
                        return null;
                    }
                    Log.Write($"Retry API Request: {request.RequestUri.AbsoluteUri}");
                    result = _client.SendAsync(request).Result;
                }
            }

            return result;
        }

        protected bool Authenticate()
        {
            Log.Write($"UniFi client authenticating.");
            _bearerToken = string.Empty;

            var url = $"{_config.UnifiApiUrl}/auth";
            var postParams = new List<KeyValuePair<string, string>>();
            postParams.Add(new KeyValuePair<string, string>("username", _config.UnifiUserName));
            postParams.Add(new KeyValuePair<string, string>("password", _config.UnifiPassword));
            var req = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new FormUrlEncodedContent(postParams)
            };
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Log.Write($"API Request: {req.RequestUri.AbsoluteUri}");
            HttpResponseMessage response = _client.SendAsync(req).Result;

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    var error = response.ParseResponseJson<UniFiApiError>();
                    Log.Write($"UniFi client authentication failed. {error.error}");
                    return false;
                }
                var errMsg = response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                    ? $"UniFi client could not login as '{_config.UnifiUserName}'. {response.ReasonPhrase}"
                    : $"UniFi client authentication failed. {response.ReasonPhrase} ({response.StatusCode})"
                    ;
                Log.Write(errMsg);
                return false;
            }

            HttpHeaders headers = response.Headers;
            IEnumerable<string> values;
            if (!headers.TryGetValues("Authorization", out values))
            {
                Log.Write($"UniFi client authentication failed. Bearer token not returned.");
                return false;
            }

            Log.Write($"UniFi authentication succeeded.");
            _bearerToken = values.First();
            AuthenticatedUser = response.ParseResponseJson<UserInfo>();

            _counters.UniFiLogins++;

            return true;
        }


    }
}