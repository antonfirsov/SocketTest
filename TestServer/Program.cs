using System.Net;
using System.Net.Sockets;

IPAddress address = IPAddress.Parse(Environment.GetEnvironmentVariable("CONTAINER_IP"));
const int Port = 5000;
IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

if (!host.AddressList.Contains(address))
{
    Console.WriteLine($"Container IP {address} not found in host address list.");
    return -1;    
}

IPEndPoint endpoint = new IPEndPoint(address, Port);
using Socket socket1 = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
socket1.Bind(endpoint);
socket1.Listen();

Console.WriteLine($"Listening on {endpoint}...");

while (true)
{
    Socket s = socket1.Accept();
    Console.WriteLine($"Accepted connection from {s.RemoteEndPoint}");
}