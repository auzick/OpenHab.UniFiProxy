using System;
using System.Net.Http;
using System.Net.Http.Headers;
using OpenHab.UniFiProxy.Logging;
using OpenHab.UniFiProxy.Model;

namespace OpenHab.UniFiProxy.Clients
{
    public interface IOpenHabClient
    {
        HttpResponseMessage PostOpenHabApi(string item, string value);
        void RunMotion(JobSettings.Job job, Bootstrap data);
        void RunState(JobSettings.Job job, Bootstrap bootstrap);
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

        public void RunMotion(JobSettings.Job job, Bootstrap bootstrap)
        {
            if (!bootstrap.TryGetCamera(job.Id, out var camera))
            {
                Log.Write($"Camera {job.Id} not found.");
                return;
            }
            PostChangedValue(job, camera.lastMotion.FromUnixTime());
        }

        public void RunUptime(JobSettings.Job job, Bootstrap bootstrap)
        {
            if (!bootstrap.TryGetCamera(job.Id, out var camera))
            {
                Log.Write($"Camera {job.Id} not found.");
                return;
            }
            PostChangedValue(job, camera.upSince.FromUnixTime());
        }

        public void RunState(JobSettings.Job job, Bootstrap bootstrap)
        {
            if (!bootstrap.TryGetCamera(job.Id, out var camera))
            {
                Log.Write($"Camera {job.Id} not found.");
                return;
            }
            PostChangedValue(job, camera.state);
        }

        public void RunStorage(JobSettings.Job job, Bootstrap bootstrap)
        {
            var totStorage = Math.Round(bootstrap.nvr.storageInfo.totalSize / 1e+9, 1);
            var usedStorage = Math.Round(bootstrap.nvr.storageInfo.totalSpaceUsed / 1e+9, 1);
            var currentSpaceUsed = Math.Round(((usedStorage * 100) / totStorage), 1);

            PostChangedValue(job, currentSpaceUsed);
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

        private bool PostChangedValue(JobSettings.Job job, DateTime value)
        {
            return PostChangedValue(job, value.ToOpenHabTimeString());
        }

        private bool PostChangedValue(JobSettings.Job job, long value)
        {
            return PostChangedValue(job, value.ToString());
        }

        private bool PostChangedValue(JobSettings.Job job, double value)
        {
            return PostChangedValue(job, value.ToString());
        }

        private bool PostChangedValue(JobSettings.Job job, string value)
        {
            if (value == job.LastValue)
            {
                return false;
            }
            var response = PostOpenHabApi(job.Item, value);
            if (!response.IsSuccessStatusCode)
            {
                var error = response.ParseResponseJson<OpenHabError>();
                Log.Write($"Update to {job.Item} failed. {error.Error.Message}");
                return false;
            }
            job.LastRun = DateTime.Now;
            job.LastValue = value;
            Log.Write($"{job.Item} updated to {value}.");
            return true;
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