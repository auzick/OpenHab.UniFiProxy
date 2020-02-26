using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using OpenHab.UniFiProxy.Logging;
using OpenHab.UniFiProxy.Model;

namespace OpenHab.UniFiProxy.Clients
{
    public interface IUniFiClient
    {
        UserInfo AuthenticatedUser { get; }
        string AccessKey { get; }

        Task<Bootstrap> GetBootstrap();
    }

    public class UniFiClient : IUniFiClient
    {
        private HttpClient _client { get; set; }
        private IAppConfiguration _config { get; set; }
        private ICounters _counters { get; set; }
        private string _bearerToken { get; set; }

        public UserInfo AuthenticatedUser { get; private set; }

        public string AccessKey { get; private set; }

        public UniFiClient(ICounters counters, IAppConfiguration config, HttpClient client)
        {
            _counters = counters;
            _client = client;
            _config = config;
            Log.Write("UniFiClient initialized.");
        }

        public async Task<Bootstrap> GetBootstrap()
        {
            var url = $"{_config.UnifiApiUrl}/bootstrap";
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            var response = await CallUniFiApi(req);
            if (response == null || !response.IsSuccessStatusCode)
            {
                Log.Write(
                    (response != null)
                    ? $"Bootstrap failed. {response?.ReasonPhrase} ({response?.StatusCode})"
                    : "Bootstrap failed."
                    );
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            try { File.WriteAllText(Path.Join(_config.AppPath, "bootstrap.json"), json); } catch { }

            var bootstrap = json.ParseJson<Bootstrap>();
            AccessKey = bootstrap.accessKey;

            _counters.BootstrapCalls++;
            return bootstrap;
        }

        public async Task<Stream> GetSnapshot(string cameraId)
        {

            var url = $"{_config.UnifiApiUrl}/api/cameras/{cameraId}/snapshot";
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            var response = await CallUniFiApiWithAccessKey(req);
            if (response == null || !response.IsSuccessStatusCode)
            {
                Log.Write(
                    (response != null)
                    ? $"Camera shapshot failed. {response?.ReasonPhrase} ({response?.StatusCode})"
                    : "Camera shapshot."
                    );
                return null;
            }

            return await response.Content.ReadAsStreamAsync();

            // var data = response.Content.ReadAsStreamAsync ReadAsByteArrayAsync().Result;
            // using (var ms = new MemoryStream(data))
            // {
            //     using (var fs = File.Create(path))
            //     {
            //         ms.CopyTo(fs);
            //     }
            // }

        }

        protected async Task<HttpResponseMessage> CallUniFiApi(HttpRequestMessage request)
        {
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (string.IsNullOrWhiteSpace(_bearerToken))
            {
                if (!await Authenticate())
                {
                    //Log.Write("Cannot call UniFi API. Could not authenticate.");
                    return null;
                }
            }

            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_bearerToken}");

            //Log.Write($"UniFi API call: {request.RequestUri.AbsoluteUri}");
            var result = await _client.SendAsync(request);
            if (!result.IsSuccessStatusCode)
            {
                if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    // Try one more time ... token may be stale 
                    Log.Write($"Failed. Reauthenticating.");
                    if (!await Authenticate())
                    {
                        Log.Write("Cannot call UniFi API. Could not authenticate.");
                        return null;
                    }
                    Log.Write($"Retry API Request: {request.RequestUri.AbsoluteUri}");
                    result = await _client.SendAsync(request);
                }
            }

            return result;
        }

        protected async Task<HttpResponseMessage> CallUniFiApiWithAccessKey(HttpRequestMessage request, bool recurse = false)
        {
            request.RequestUri = request.RequestUri.SetQueryValue("accessKey", AccessKey);
            var result = await CallUniFiApi(request);
            if (result.IsSuccessStatusCode || result.StatusCode != System.Net.HttpStatusCode.Unauthorized)
                return result;
            // Maybe the access key is stale? Refresh it and try one more time
            if (recurse)
                return result;
            await RefreshAccessKey();
            return await CallUniFiApiWithAccessKey(request, true);
        }

        protected async Task<bool> RefreshAccessKey()
        {
            var url = $"{_config.UnifiApiUrl}/auth/access-key";
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            var response = await CallUniFiApi(req);
            if (response == null || !response.IsSuccessStatusCode)
            {
                Log.Write(
                    (response != null)
                    ? $"Could not refresh access key. {response?.ReasonPhrase} ({response?.StatusCode})"
                    : "Could not refresh access key."
                    );
                return false;
            }
            var json = await response.Content.ReadAsStringAsync();
            var bootstrap = json.ParseJson<UniFiAccessKey>();
            AccessKey = bootstrap.accessKey;
            return true;
        }

        protected async Task<bool> Authenticate()
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
            HttpResponseMessage response = await _client.SendAsync(req);

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