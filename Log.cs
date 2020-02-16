using System;

// I"m aware that this is sloppy
namespace OpenHab.UniFiProxy
{
    public static class Log
    {
        public static void Write(string message)
        {
            Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}] {message}");
        }

    }
}