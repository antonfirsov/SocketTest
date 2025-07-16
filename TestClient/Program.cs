using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.IO;
using System.Text;
using Xunit;
using System.Threading.Tasks;

if (args.Length > 0 && args[0].ToLower() == "init")
{
    System.Console.WriteLine("Generating Corefile and updating resolv.conf...");

    string corefileContent = $@"
.:53 {{
    hosts {{
        {GetHosts("HOST_A", "HOST_B")}
    }}
    errors
    log
}}";

    Directory.CreateDirectory("/etc/coredns");
    File.WriteAllText("/etc/coredns/Corefile", corefileContent);
    Console.WriteLine("Corefile generated at /etc/coredns/Corefile");

    // Read original resolv.conf and prepend CoreDNS nameserver
    string resolvConfOriginal = File.ReadAllText("/etc/resolv.conf");
    string newContent = "nameserver 127.0.0.1" + Environment.NewLine + resolvConfOriginal;
    File.WriteAllText("/etc/resolv.conf", newContent);
    System.Console.WriteLine("Updated /etc/resolv.conf to use CoreDNS.");

    return;
    
    static string GetHosts(params string[] hostVars)
    {
        StringBuilder sb = new StringBuilder();
        foreach (string hostVar in hostVars)
        {
            string host = hostVar.Replace("_", "-").ToLower();
            foreach (string ipStr in Environment.GetEnvironmentVariable(hostVar).Split())
            {
                sb.AppendLine($"{ipStr} {host}");
            }
        }
        return sb.ToString();
    }
}

System.Console.WriteLine("Starting the client test");

await Task.Delay(1000); // Allow time for server to start



public class ConnectTests
{
    const int Port = 5000;

    [Theory]
    [InlineData("V4_SLOW")]
    [InlineData("V6_SLOW")]
    public async Task ConnectAsync_IPEndPoint_Cancellable(string ipName)
    {
        IPAddress address = IPAddress.Parse(Environment.GetEnvironmentVariable(ipName));
        using Socket socket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        CancellationTokenSource cts = new CancellationTokenSource();
        Stopwatch stopwatch = Stopwatch.StartNew();
        Task connectTask = socket.ConnectAsync(new IPEndPoint(address, Port), cts.Token).AsTask();
        cts.CancelAfter(50);
        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await connectTask);
        stopwatch.Stop();
        Console.WriteLine($"ConnectAsync with {ipName} cancelled after {stopwatch.ElapsedMilliseconds} ms");
        Assert.True(stopwatch.ElapsedMilliseconds < 100, "Connect should complete quickly after cancellation");
    }

    [Fact]
    public async Task BasicConnect()
    {
        using Socket socketA = new Socket(SocketType.Stream, ProtocolType.Tcp);
        await socketA.ConnectAsync(new DnsEndPoint("host-a", Port));
        Console.WriteLine($"socketA (host-a) connected to {socketA.RemoteEndPoint}");

        using Socket socketB = new Socket(SocketType.Stream, ProtocolType.Tcp);
        await socketB.ConnectAsync(new DnsEndPoint("host-b", Port));
        Console.WriteLine($"socketB (host-b) connected to {socketB.RemoteEndPoint}");
    }
}