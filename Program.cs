using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using OpenHab.UniFiProxy.Model;
using System.Threading;

namespace OpenHab.UniFiProxy
{
    class Program
    {
        static IConfiguration config;
        static readonly HttpClient client = new HttpClient();
        static string bearerToken;
        static UserInfo authenticatedUserInfo;
        static JobSettings jobSettings;
        static int statsFrequency;
        static string appPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        static DateTime lastReport = DateTime.MinValue;
        static Dictionary<string, int> counters = new Dictionary<string, int>();

        static void Main(string[] args)
        {
            if (!LoadConfig()) { return; }
            if (!CheckConnection()) { return; }

            if (jobSettings.Jobs.Count() == 0)
            {
                Console.WriteLine("");
                Console.WriteLine("Nothing to do. No jobs defined in jobs.json.");
                return;
            }
            if (jobSettings.PollInterval == 0)
            {
                Console.WriteLine("");
                Console.WriteLine("Nothing to do. Poll interval set to zero.");
                return;
            }


            Console.WriteLine("");
            Console.WriteLine(new string('=', 80));
            Console.WriteLine("Running");
            Console.WriteLine(new string('-', 80));

            TimerCallback tmCallback = RunJobs;
            Timer timer = new Timer(tmCallback, null, jobSettings.PollInterval * 1000, jobSettings.PollInterval * 1000);
            Console.WriteLine("Press the enter key to stop.");
            Console.ReadLine();
        }

        static void RunJobs(object stateInfo)
        {
            var thisRun = DateTime.Now;
            Bootstrap data = null;
            foreach (var job in jobSettings.Jobs)
            {
                if (job.Frequency > 0 && job.LastRun.AddSeconds(job.Frequency) <= thisRun)
                {
                    if (data == null)
                    {
                        data = GetBootstrap();
                    }
                    switch (job.Type.ToLower())
                    {
                        case "motion":
                            RunMotion(job, data);
                            break;
                        case "uptime":
                            RunUptime(job, data);
                            break;
                        case "storage":
                            RunStorage(job, data);
                            break;
                        default:
                            WriteConsole($"Unknown job type: {job.Type}");
                            break;
                    }
                }
            }
            if (DateTime.Now > lastReport.AddSeconds(statsFrequency))
            {
                WriteConsole($"Unifi logins: {counters["unifiLogins"]} / "
                    + $"Unifi bootstrap calls: {counters["bootstrapCalls"]} / "
                    + $"OpenHAB calls: {counters["openhabCalls"]} "
                );
                lastReport = DateTime.Now;
            }
        }

        private static void RunMotion(JobSettings.Job job, Bootstrap data)
        {
            var oldValue = DateTime.MinValue;
            DateTime.TryParse(job.LastValue, out oldValue);

            var camera = data.cameras.FirstOrDefault(c => c.id == job.Id);
            if (camera == null)
            {
                // WriteConsole($"Camera {job.Id} unchanged.");
                return;
            }

            var newValue = FromUnixTime(camera.lastMotion);
            var newVal = newValue.ToString("yyyy-MM-ddThh:mm:ss");

            if (newVal == oldValue.ToString("yyyy-MM-ddThh:mm:ss"))
            {
                // WriteConsole($"{job.Item} unchanged.");
                return;
            }

            var response = CallOpenHabApi(job.Item, newVal);
            if (!response.IsSuccessStatusCode)
            {
                var error = ParseResponseJson<OpenHabError>(response);
                WriteConsole($"Update to {job.Item} failed. {error.Error.Message}");
                return;
            }
            job.LastRun = DateTime.Now;
            job.LastValue = newVal;
            WriteConsole($"{job.Item} updated to {newVal}.");
        }

        private static void RunUptime(JobSettings.Job job, Bootstrap data)
        {
            var oldValue = DateTime.MinValue;
            DateTime.TryParse(job.LastValue, out oldValue);

            var camera = data.cameras.FirstOrDefault(c => c.id == job.Id);
            if (camera == null)
            {
                // WriteConsole($"Camera {job.Id} unchanged.");
                return;
            }

            var newValue = FromUnixTime(camera.upSince);
            var newVal = newValue.ToString("yyyy-MM-ddThh:mm:ss");
            if (newVal == oldValue.ToString("yyyy-MM-ddThh:mm:ss"))
            {
                // WriteConsole($"{job.Item} unchanged.");
                return;
            }

            var response = CallOpenHabApi(job.Item, newVal);
            if (!response.IsSuccessStatusCode)
            {
                var error = ParseResponseJson<OpenHabError>(response);
                WriteConsole($"Update to {job.Item} failed. {error.Error.Message}");
                return;
            }
            job.LastRun = DateTime.Now;
            job.LastValue = newVal;
            WriteConsole($"{job.Item} updated to {newVal}.");
        }

        private static void RunStorage(JobSettings.Job job, Bootstrap data)
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
            var response = CallOpenHabApi(job.Item, newValue.ToString());
            if (!response.IsSuccessStatusCode)
            {
                var error = ParseResponseJson<OpenHabError>(response);
                WriteConsole($"Update to {job.Item} failed. {error.Error.Message}");
                return;
            }
            job.LastRun = DateTime.Now;
            job.LastValue = newValue.ToString();
            WriteConsole($"{job.Item} updated to {newValue}.");
        }

        private static HttpResponseMessage CallOpenHabApi(string item, string value)
        {
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var url = $"{config["openHabApiUrl"]}/items/{item}";
            var req = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(value)
            };

            counters["openhabCalls"]++;

            return client.SendAsync(req).Result;
        }

        private static bool LoadConfig()
        {
            appPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            lastReport = DateTime.Now;
            counters.Add("bootstrapCalls", 0);
            counters.Add("unifiLogins", 0);
            counters.Add("openhabCalls", 0);

            WriteConsole("Loading configuration");

            config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            foreach (string requiredConfig in new string[]{
                "unifiApiUrl", "unifiUserName", "unifiPassword", "openHabApiUrl", "statsFrequency"
                })
            {
                if (string.IsNullOrWhiteSpace(config[requiredConfig]))
                {
                    WriteConsole($"Required configuration not defined: {requiredConfig}");
                    return false;
                }
            }

            if (!int.TryParse(config["statsFrequency"], out statsFrequency))
            {
                WriteConsole($"Invalid value for statsFrequency: {config["statsFrequency"]}");
                return false;
            }

            WriteConsole("App settings loaded");

            WriteConsole("Loading job settings");
            jobSettings = new JobSettings();
            var jobsPath = Path.Join(appPath, "jobs.json");
            if (File.Exists(jobsPath))
            {
                var jobsJson = File.ReadAllText(jobsPath);
                try
                {
                    jobSettings = JsonSerializer.Deserialize<JobSettings>(jobsJson);
                }
                catch { }
                WriteConsole($"Job poll interval: {jobSettings.PollInterval}");
                WriteConsole("Jobs:");
                foreach (var job in jobSettings.Jobs)
                {
                    WriteConsole($"  {job.Type} {job.Id}: {job.Item}");
                }
            }
            if (jobSettings.Jobs.Count() == 0)
            {
                WriteConsole($"No jobs loaded.");
            }
            return true;
        }

        private static bool CheckConnection()
        {
            WriteConsole("Connecting to NVR");

            var nvrData = GetBootstrap();

            if (nvrData == null)
            {
                WriteConsole("Could not communicate with NVR");
                return false;
            }

            var totStorage = Math.Round(nvrData.nvr.storageInfo.totalSize / 1e+9, 1);
            var usedStorage = Math.Round(nvrData.nvr.storageInfo.totalSpaceUsed / 1e+9, 1);
            var usedStoragePct = Math.Round(((usedStorage * 100) / totStorage), 1);

            WriteConsole("NVR Info:");

            WriteConsole($"  NVR name:      {nvrData.nvr.name}");
            WriteConsole($"  NVR IP:        {nvrData.nvr.host}");
            WriteConsole($"  NVR MAC:       {nvrData.nvr.mac}");
            WriteConsole($"  NVR firmware:  {nvrData.nvr.firmwareVersion}");
            WriteConsole($"  NVR time zone: {nvrData.nvr.timezone}");
            WriteConsole($"  NVR storage:   {usedStorage} used of {totStorage} gb ({usedStoragePct}%)");

            WriteConsole("Cameras:");

            Console.WriteLine(nvrData.cameras.ToStringTable(
                new string[] { "Name", "ID", "IP", "MAC address", "Type", "State", "Last motion", "Wifi" },
                c => c.name,
                c => c.id,
                c => c.host,
                c => c.mac,
                c => c.type,
                c => c.state,
                c => FromUnixTime(c.lastMotion).ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"),
                c => c.stats.wifiStrength
                ));

            return true;
        }

        protected static Bootstrap GetBootstrap()
        {
            var url = $"{config["unifiApiUrl"]}/bootstrap";
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            var response = CallUniFiApi(req);
            if (response == null || !response.IsSuccessStatusCode)
            {
                WriteConsole(
                    (response != null)
                    ? $"Bootstrap failed. {response?.ReasonPhrase} ({response?.StatusCode})"
                    : "Bootstrap failed."
                    );
                return null;
            }
            // WriteConsole("Got bootstrap data");

            var json = response.Content.ReadAsStringAsync().Result;
            File.WriteAllText(Path.Join(appPath, "bootstrap.json"), json);

            counters["bootstrapCalls"]++;

            return ParseJson<Bootstrap>(json);
        }

        protected static HttpResponseMessage CallUniFiApi(HttpRequestMessage request, bool AddToken = true)
        {
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (string.IsNullOrWhiteSpace(bearerToken))
            {
                if (!Authenticate())
                {
                    //WriteConsole("Cannot call UniFi API. Could not authenticate.");
                    return null;
                }
            }

            if (AddToken)
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {bearerToken}");
            }

            //WriteConsole($"UniFi API call: {request.RequestUri.AbsoluteUri}");
            var result = client.SendAsync(request).Result;
            if (!result.IsSuccessStatusCode)
            {
                if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    // Try one more time ... token may be stale 
                    WriteConsole($"Failed. Reauthenticating.");
                    if (!Authenticate())
                    {
                        WriteConsole("Cannot call UniFi API. Could not authenticate.");
                        return null;
                    }
                    WriteConsole($"Retry API Request: {request.RequestUri.AbsoluteUri}");
                    result = client.SendAsync(request).Result;
                }
            }

            return result;
        }

        protected static bool Authenticate()
        {
            WriteConsole($"Authenticating.");
            bearerToken = string.Empty;

            var url = $"{config["unifiApiUrl"]}/auth";
            var postParams = new List<KeyValuePair<string, string>>();
            postParams.Add(new KeyValuePair<string, string>("username", config["unifiUserName"]));
            postParams.Add(new KeyValuePair<string, string>("password", config["unifiPassword"]));
            var req = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new FormUrlEncodedContent(postParams)
            };
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // WriteConsole($"API Request: {req.RequestUri.AbsoluteUri}");
            HttpResponseMessage response = client.SendAsync(req).Result;

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    var error = ParseResponseJson<UniFiApiError>(response);
                    WriteConsole($"Authentication failed. {error.error}");
                    return false;
                }
                var errMsg = response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                    ? $"Could not login as '{config["unifiUserName"]}/{config["UnifiPassword"]}'. {response.ReasonPhrase}"
                    : "Authentication failed. {response.ReasonPhrase} ({response.StatusCode})"
                    ;
                WriteConsole(errMsg);
                return false;
            }

            HttpHeaders headers = response.Headers;
            IEnumerable<string> values;
            if (!headers.TryGetValues("Authorization", out values))
            {
                WriteConsole($"Authentication failed. Bearer token not returned.");
                return false;
            }

            WriteConsole($"Authentication succeeded.");
            bearerToken = values.First();
            authenticatedUserInfo = ParseResponseJson<UserInfo>(response);

            counters["unifiLogins"]++;

            return true;
        }

        private static T ParseJson<T>(string json)
        {
            var options = new JsonSerializerOptions
            {
                AllowTrailingCommas = true
            };
            return JsonSerializer.Deserialize<T>(json);
        }

        private static T ParseResponseJson<T>(HttpResponseMessage response)
        {
            var json = response.Content.ReadAsStringAsync().Result;
            return ParseJson<T>(json);
        }

        private static void WriteConsole(string message)
        {
            Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}] {message}");
        }

        private static DateTime FromUnixTime(double unixTime)
        {
            return epoch.AddMilliseconds(unixTime).ToLocalTime();
        }

        private static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    }
}