using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

await Task.Delay(1000); // Allow time for server to start

IPAddress[] addresses = await Dns.GetHostAddressesAsync("address.test");
foreach (IPAddress address in addresses)
{
    Console.WriteLine($"Resolved address: {address}");
}

const int Port = 5000;
string[] addressStrings = ["172.20.0.10", "172.20.0.11", "2001:db8:1::10", "2001:db8:1::11"];
foreach (IPAddress a in addressStrings.Select(IPAddress.Parse))
{
    Socket socket = new Socket(a.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
    IPEndPoint endpoint = new IPEndPoint(a, Port);
    System.Console.WriteLine($"Connecting to {endpoint}...");
    Stopwatch sw = Stopwatch.StartNew();
    await socket.ConnectAsync(endpoint);
    System.Console.WriteLine($"Connected to {endpoint} in {sw.ElapsedMilliseconds} ms");
}