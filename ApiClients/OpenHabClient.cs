using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using OpenHab.UniFiProxy.Logging;
using OpenHab.UniFiProxy.Model;

namespace OpenHab.UniFiProxy.Clients
{
    public interface IOpenHabClient
    {
        HttpResponseMessage PostOpenHabApi(string item, string value);
        void RunMotion(JobSettings.Job job, Bootstrap data);
        void RunStorage(JobSettings.Job job, Bootstrap data);
        void RunUptime(JobSettings.Job job, Bootstrap data);
    }

    public class OpenHabClient : IOpenHabClient
    {
        private HttpClient _client { get; set; }
        private IAppConfiguration _config { get; set; }
        private ICounters _counters;

        public OpenHabClient(ICounters counters, IAppConfiguration config, HttpClient client)
        {
            _counters = counters;
            _client = client;
            _config = config;
            Log.Write("OpenHabClient initialized.");
        }

        public void RunMotion(JobSettings.Job job, Bootstrap data)
        {
            var oldValue = DateTime.MinValue;
            DateTime.TryParse(job.LastValue, out oldValue);

            var camera = data.cameras.FirstOrDefault(c => c.id == job.Id);
            if (camera == null)
            {
                return;
            }

            var newValue = camera.lastMotion.FromUnixTime();
            var newVal = newValue.ToString("yyyy-MM-ddThh:mm:ss");

            if (newVal == oldValue.ToString("yyyy-MM-ddThh:mm:ss"))
            {
                return;
            }

            var response = PostOpenHabApi(job.Item, newVal);
            if (!response.IsSuccessStatusCode)
            {
                var error = response.ParseResponseJson<OpenHabError>();
                Log.Write($"Update to {job.Item} failed. {error.Error.Message}");
                return;
            }
            job.LastRun = DateTime.Now;
            job.LastValue = newVal;
            Log.Write($"{job.Item} updated to {newVal}.");
        }

        // public void RunSnap(JobSettings.Job job, Bootstrap data)
        // {
        //     // Yeah, I'm stuck here.
        //     var url = $"{config["unifiApiUrl"]}/cameras/{job.Id}/snapshot?accessKey={data.accessKey}";
        //     var req = new HttpRequestMessage(HttpMethod.Post, url);
        //     var response = CallUniFiApi(req);

        //     if (response == null || !response.IsSuccessStatusCode)
        //     {
        //         Log.Write(
        //             (response != null)
        //             ? $"Camera Snapshot failed. {response?.ReasonPhrase} ({response?.StatusCode})"
        //             : "Camera Snapshot failed."
        //             );
        //     }
        //     var responseData = response.Content.ReadAsByteArrayAsync();

        // }

        public void RunUptime(JobSettings.Job job, Bootstrap data)
        {
            var oldValue = DateTime.MinValue;
            DateTime.TryParse(job.LastValue, out oldValue);

            var camera = data.cameras.FirstOrDefault(c => c.id == job.Id);
            if (camera == null)
            {
                return;
            }

            var newValue = camera.upSince.FromUnixTime();
            var newVal = newValue.ToString("yyyy-MM-ddThh:mm:ss");
            if (newVal == oldValue.ToString("yyyy-MM-ddThh:mm:ss"))
            {
                // Log.Write($"{job.Item} unchanged.");
                return;
            }

            var response = PostOpenHabApi(job.Item, newVal);
            if (!response.IsSuccessStatusCode)
            {
                var error = response.ParseResponseJson<OpenHabError>();
                Log.Write($"Update to {job.Item} failed. {error.Error.Message}");
                return;
            }
            job.LastRun = DateTime.Now;
            job.LastValue = newVal;
            Log.Write($"{job.Item} updated to {newVal}.");
        }

        public void RunStorage(JobSettings.Job job, Bootstrap data)
        {
            // WriteConsole($"Sending update to {job.Item} ");
            var totStorage = Math.Round(data.nvr.storageInfo.totalSize / 1e+9, 1);
            var usedStorage = Math.Round(data.nvr.storageInfo.totalSpaceUsed / 1e+9, 1);
            var newValue = Math.Round(((usedStorage * 100) / totStorage), 1);

            double oldValue = 0;
            double.TryParse(job.LastValue, out oldValue);

            if (newValue == oldValue)
            {
                // WriteConsole($"{job.Item} unchanged.");
                return;
            }
            var response = PostOpenHabApi(job.Item, newValue.ToString());
            if (!response.IsSuccessStatusCode)
            {
                var error = response.ParseResponseJson<OpenHabError>();
                Log.Write($"Update to {job.Item} failed. {error.Error.Message}");
                return;
            }
            job.LastRun = DateTime.Now;
            job.LastValue = newValue.ToString();
            Log.Write($"{job.Item} updated to {newValue}.");
        }


        public HttpResponseMessage PostOpenHabApi(string item, string value)
        {
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var url = $"{_config.OpenHabApiUrl}/items/{item}";
            var req = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(value)
            };

            _counters.OpenHabCalls++;

            return _client.SendAsync(req).Result;
        }

    }
}