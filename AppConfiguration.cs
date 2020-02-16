using System;
using System.IO;
using System.Reflection;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using OpenHab.UniFiProxy.Model;

namespace OpenHab.UniFiProxy
{
    public interface IAppConfiguration
    {
        JobSettings Jobs { get; }
        string AppPath { get; }
        string UnifiApiUrl { get; }
        string UnifiUserName { get; }
        string UnifiPassword { get; }
        string OpenHabApiUrl { get; }
        int StatsFrequency { get; }
    }

    public class AppConfiguration : IAppConfiguration
    {
        public static readonly string[] RequiredConfigSettings = new string[]{
            "unifiApiUrl", "unifiUserName", "unifiPassword", "openHabApiUrl", "statsFrequency"
            };

        public string AppPath => Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        public JobSettings Jobs { get; private set; }
        public string UnifiApiUrl { get; private set; }
        public string UnifiUserName { get; private set; }
        public string UnifiPassword { get; private set; }
        public string OpenHabApiUrl { get; private set; }
        public int StatsFrequency { get; private set; }

        public AppConfiguration()
        {
            LoadConfig();
            LoadJobs();
        }

        private void LoadConfig()
        {
            Log.Write("Loading configuration.");

            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            ValidateAppSettings(config);

            UnifiApiUrl = config["unifiApiUrl"];
            UnifiUserName = config["unifiUserName"];
            UnifiPassword = config["unifiPassword"];
            OpenHabApiUrl = config["openHabApiUrl"];
            StatsFrequency = int.Parse(config["statsFrequency"]);

            Log.Write("App settings loaded.");


        }

        private void LoadJobs()
        {
            Log.Write("Loading job settings.");
            Jobs = new JobSettings();
            var jobsPath = Path.Join(AppPath, "jobs.json");
            if (File.Exists(jobsPath))
            {
                var jobsJson = File.ReadAllText(jobsPath);
                try
                {
                    Jobs = JsonSerializer.Deserialize<JobSettings>(jobsJson);
                }
                catch { }
                Log.Write($"Job poll interval: {Jobs.PollInterval}");
                Log.Write("Jobs:");
                foreach (var job in Jobs.Jobs)
                {
                    var msg = $"  {job.Type} {job.Id}: {job.Item}";
                    Log.Write(msg);
                }
            }
            if (Jobs.Jobs.Count == 0)
            {
                Log.Write($"No jobs defined.");
            }
            Log.Write($"Job settings loaded.");

        }

        private void ValidateAppSettings(IConfiguration config)
        {
            foreach (string requiredConfig in RequiredConfigSettings)
            {
                if (string.IsNullOrWhiteSpace(config[requiredConfig]))
                {
                    throw new Exception($"Required configuration not defined: {requiredConfig}");
                }
            }

            if (!int.TryParse(config["statsFrequency"], out var freq))
            {
                throw new Exception($"Invalid value for statsFrequency: {config["statsFrequency"]}");
            }
        }


    }
}