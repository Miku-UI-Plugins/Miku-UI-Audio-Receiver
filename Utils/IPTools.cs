using System.Net.Sockets;
using System.Net;
using System.Linq;

namespace Miku_UI_Music_Center.Utils
{
    internal class IPTools
    {
        public static string GetLocalIPv4()
        {
            var ipv4s = Dns.GetHostAddresses(Dns.GetHostName())
                        .Where(p => p.AddressFamily == AddressFamily.InterNetwork)
                        .Select(p => p.MapToIPv4().ToString());
            if (ipv4s.Count() > 1)
                return ipv4s.Where(p => p.StartsWith("192.168.") || p.StartsWith("10.") || p.StartsWith("172.16.")).First();
            else
                return ipv4s.First();
        }
    }
}
