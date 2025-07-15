using System.Net;
using System.Net.Sockets;
using System.Text;

const int Port = 8080;
IPEndPoint endpoint = new IPEndPoint(IPAddress.Loopback, Port);

using Socket socket1 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
socket1.Bind(endpoint);
socket1.Listen();
using Socket client = socket1.Accept();

