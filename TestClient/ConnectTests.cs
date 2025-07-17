using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Xunit;

namespace TestClient;

public class ConnectTests
{
    const int Port = 5000;

    private static string GetHostName(string hostNameVar)
        => hostNameVar.Replace("_", "-").ToLower();

    private static IPAddress GetAddress(string ipNameVar)
        => IPAddress.Parse(Environment.GetEnvironmentVariable(ipNameVar));

    [Theory]
    [InlineData("V4_SLOW")]
    [InlineData("V6_SLOW")]
    public async Task ConnectAsync_IPEndPoint_Cancellable(string ipNameVar)
    {
        IPAddress address = GetAddress(ipNameVar);
        using Socket socket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        CancellationTokenSource cts = new CancellationTokenSource();
        Stopwatch stopwatch = Stopwatch.StartNew();
        Task connectTask = socket.ConnectAsync(new IPEndPoint(address, Port), cts.Token).AsTask();
        cts.CancelAfter(50);
        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await connectTask);
        Assert.True(stopwatch.ElapsedMilliseconds < 100, "Connect should complete quickly after cancellation");
    }

    public static TheoryData<string> AllHostNames = new TheoryData<string>()
    {
        "HOST_V4_SINGLE_SLOW",
        "HOST_V6_SINGLE_SLOW",
        "HOST_V4_WINS0",
        "HOST_V6_WINS0",
        "HOST_V4_WINS1",
        "HOST_V6_WINS1"
    };

    [Theory]
    [MemberData(nameof(AllHostNames))]
    public void Prerequisite_CanResolveEachHost(string hostNameVar)
    {
        string host = GetHostName(hostNameVar);
        try
        {
            var addresses = Dns.GetHostAddresses(host);
            Assert.NotEmpty(addresses);
        }
        catch (SocketException ex)
        {
            Assert.Fail($"Failed to resolve host {host}: {ex.Message}");
        }
        
    }

    [Theory]
    [InlineData("HOST_V4_SINGLE_SLOW")]
    [InlineData("HOST_V6_SINGLE_SLOW")]
    public async Task ConnectAsync_HostName_Cancellable(string hostNameVar)
    {
        string host = GetHostName(hostNameVar);
        using Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        CancellationTokenSource cts = new CancellationTokenSource();
        Stopwatch stopwatch = Stopwatch.StartNew();
        Task connectTask = socket.ConnectAsync(host, Port, cts.Token).AsTask();
        cts.CancelAfter(50);
        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await connectTask);
        Assert.True(stopwatch.ElapsedMilliseconds < 100, "Connect should complete quickly after cancellation");
    }    

    [Theory]
    [InlineData("HOST_V4_SINGLE_SLOW", "V4_SLOW")]
    [InlineData("HOST_V6_SINGLE_SLOW", "V6_SLOW")]
    [InlineData("HOST_V4_WINS1", "V4_FAST")]
    [InlineData("HOST_V4_WINS0", "V4_FAST")]
    [InlineData("HOST_V6_WINS0", "V6_FAST")]
    [InlineData("HOST_V6_WINS1", "V6_FAST")]
    public void ConnectAsync_Parallel_FastestServerWins(string hostNameVar, string expectedWinnerIpVar)
    {
        string host = GetHostName(hostNameVar);
        IPAddress expectedWinner = GetAddress(expectedWinnerIpVar);

        var mres = new ManualResetEventSlim();
        SocketAsyncEventArgs saea = new SocketAsyncEventArgs();
        saea.RemoteEndPoint = new DnsEndPoint(host, Port);
        saea.Completed += (_, _) => mres.Set();

        Stopwatch stopwatch = Stopwatch.StartNew();
        if (ParallelConnect(saea))
        {
            System.Console.WriteLine("pending");
            mres.Wait(5000);
        }

        Console.WriteLine($"Connect({hostNameVar}) completed in {stopwatch.ElapsedMilliseconds} ms");
        Assert.Equal(SocketError.Success, saea.SocketError);
        Socket socket = saea.ConnectSocket;

        Assert.NotNull(socket);
        Assert.True(socket.Connected);
        Assert.NotNull(socket.RemoteEndPoint);
        Assert.Equal(expectedWinner, ((IPEndPoint)socket.RemoteEndPoint).Address);
    }

    private static bool ParallelConnect(SocketAsyncEventArgs saea)
    {
        // Use reflection to discover ConnectAlgorithm.Parallel
        var socketAssembly = typeof(Socket).Assembly;
        var connectAlgorithmType = socketAssembly.GetType("System.Net.Sockets.ConnectAlgorithm");
        Assert.NotNull(connectAlgorithmType);
        
        var parallelValue = Enum.Parse(connectAlgorithmType, "Parallel");
        
        // Use reflection to find Socket.ConnectAsync method with ConnectAlgorithm parameter
        var connectAsyncMethod = typeof(Socket).GetMethod("ConnectAsync", 
            new[] { typeof(SocketType), typeof(ProtocolType), typeof(SocketAsyncEventArgs), connectAlgorithmType });
        
        Assert.NotNull(connectAsyncMethod);
        
        // Invoke the method
        return (bool)connectAsyncMethod.Invoke(null, new object[] 
        { 
            SocketType.Stream, 
            ProtocolType.Tcp, 
            saea, 
            parallelValue 
        });
    }
}