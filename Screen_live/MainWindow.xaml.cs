using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Threading;
using System.Collections.Generic;

namespace Screen_live;

public partial class MainWindow : Window
{
    IPEndPoint? clientEP = null;
    IPEndPoint? server = null;
    UdpClient? client = null;
    public MainWindow()
    {
        InitializeComponent();

        clientEP = new IPEndPoint(IPAddress.Parse("127.1.1.1"), 12);
        server = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 45678);
        client = new UdpClient(clientEP);
    }

    private async void btn_Click(object sender, RoutedEventArgs e)
    {
        var pulse = Encoding.Default.GetBytes("hello");
        int wholeNum;
        var list = new List<byte>();
        if (client != null && server != null)
        {
            await client.SendAsync(pulse, pulse.Length, server);

            try
            {
                btn.IsEnabled = false;
                UdpReceiveResult rec_Bytes;
                
                while (true)
                {
                    rec_Bytes = await client.ReceiveAsync();

                    if (rec_Bytes.Buffer.Length != 65506)
                    {
                        list.AddRange(rec_Bytes.Buffer);

                        Dispatcher.Invoke(() =>
                        {
                            BitmapImage bitmap = new BitmapImage();

                            bitmap.BeginInit();
                            bitmap.StreamSource = new MemoryStream(list.ToArray());
                            bitmap.EndInit();

                            img.Source = bitmap;
                        });

                        list.Clear();
                    }
                    else
                        list.AddRange(rec_Bytes.Buffer);
                }
            }
            catch (SocketException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                btn.IsEnabled = true;
            }

        }
    }
}
