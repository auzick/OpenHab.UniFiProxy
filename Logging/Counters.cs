using System;

namespace OpenHab.UniFiProxy.Logging
{
    public interface ICounters
    {
        int BootstrapCalls { get; set; }
        int UniFiLogins { get; set; }
        int OpenHabCalls { get; set; }

        string ToString();
    }

    public class Counters : ICounters
    {
        public int BootstrapCalls { get; set; }
        public int UniFiLogins { get; set; }
        public int OpenHabCalls { get; set; }


        public Counters()
        {
            BootstrapCalls = 0;
            UniFiLogins = 0;
            OpenHabCalls = 0;
            Log.Write("Counters initialized.");
        }

        public override string ToString()
        {
            return $"Unifi logins: {UniFiLogins} / "
                    + $"Unifi bootstrap calls: {BootstrapCalls} / "
                    + $"OpenHAB calls: {OpenHabCalls} ";
        }

    }
}