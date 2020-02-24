using System;

namespace OpenHab.UniFiProxy.Logging
{
    public interface ICounters
    {
        int BootstrapCalls { get; set; }
        int UniFiLogins { get; set; }
        int OpenHabCalls { get; set; }

        public ExecutionStats Stats { get; }

        void LogExecution(double elapsed);
        string ToString();
    }

    public class Counters : ICounters
    {
        public int BootstrapCalls { get; set; }
        public int UniFiLogins { get; set; }
        public int OpenHabCalls { get; set; }
        public ExecutionStats Stats { get; private set; }

        public Counters()
        {
            BootstrapCalls = 0;
            UniFiLogins = 0;
            OpenHabCalls = 0;
            Stats = new ExecutionStats();
            Log.Write("Counters initialized.");
        }

        public void LogExecution(double elapsed)
        {
            Stats.TotalExecutions++;
            Stats.TotalElapsed += elapsed;
            if (elapsed > Stats.Longest) { Stats.Longest = elapsed; }
            if (elapsed < Stats.Shortest) { Stats.Shortest = elapsed; }
        }


        public override string ToString()
        {
            var average = (Stats.Average == double.NaN ? "--" : Math.Ceiling(Stats.Average).ToString());
            return $"Unifi logins: {UniFiLogins} | "
                    + $"Unifi bootstrap calls: {BootstrapCalls} | "
                    + $"OpenHAB calls: {OpenHabCalls} | "
                    + $"Average execution: {average}ms"
                    // + $"Execution time (high/low/avg): "
                    // + $"{Math.Ceiling(Stats.Longest)}/"
                    // + $"{Math.Ceiling(Stats.Shortest)}/"
                    // + $"{Math.Ceiling(Stats.Average)}"
                    ;
        }

    }

    public class ExecutionStats
    {
        public double TotalExecutions { get; set; }
        public double TotalElapsed { get; set; }
        public double Longest { get; set; }
        public double Shortest { get; set; }
        public double Average { get { return TotalElapsed / TotalExecutions; } }

        public ExecutionStats()
        {
            TotalExecutions = 0;
            TotalElapsed = 0;
            Longest = 0;
            Shortest = double.MaxValue;
        }

    }


}