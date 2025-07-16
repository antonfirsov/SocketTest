using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using Xunit;

namespace TestClient;

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
