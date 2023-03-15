using Server;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;

Size size = ScreenResolution.GetScreenResolution();
byte[] sendbuf;

IPAddress ip = IPAddress.Parse("127.0.0.1");
int port = 45678;
IPEndPoint listenerEP = new IPEndPoint(ip, port);
IPEndPoint clientEP = new IPEndPoint(IPAddress.Parse("127.1.1.1"), 12);

UdpClient server = new UdpClient(listenerEP);

byte a = 0;
UdpReceiveResult result;
string? resultStr = null;
result = await server.ReceiveAsync();
resultStr = Encoding.Default.GetString(result.Buffer);

if (resultStr is not null)
{
    while (true)
    {
        sendbuf = ScreenShot.ScreenShotMethod(size).ToArray();

        int whole_num = ((int)(sendbuf.Length / 65506) + 1);

        await server.SendAsync(Encoding.Default.GetBytes(sendbuf.Length.ToString()), clientEP);
        await server.SendAsync(Encoding.Default.GetBytes(whole_num.ToString()), clientEP);

        Thread.Sleep(1);
        foreach (var item in sendbuf.Chunk(65506))
        {
            await server.SendAsync(item, clientEP);
            Thread.Sleep(3);
        }
    }
}