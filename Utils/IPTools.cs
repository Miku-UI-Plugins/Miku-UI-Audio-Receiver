﻿using System.Net.Sockets;
using System.Net;

namespace Miku_UI_Audio_Receiver.Utils
{
    internal class IPTools
    {
        public static string GetLocalIPv4()
        {
            var ipv4s = Dns.GetHostAddresses(Dns.GetHostName())
                        .Where(p => p.AddressFamily == AddressFamily.InterNetwork)
                        .Select(p => p.MapToIPv4().ToString());
            if (ipv4s.Count() > 1)
                return ipv4s.Where(p => p.StartsWith("192")).First();
            else
                return ipv4s.First();
        }
    }
}