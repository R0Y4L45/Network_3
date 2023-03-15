using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Threading;

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
        int lenght, wholeNum, a = 0;
        if (client != null && server != null)
        {
            await client.SendAsync(pulse, pulse.Length, server);

            try
            {
                btn.IsEnabled = false;
                UdpReceiveResult rec_Bytes, rec_lenght, rec_wholeNum;

                await Task.Run(async () =>
                {
                    while (true)
                    {
                        rec_lenght = await client.ReceiveAsync();
                        rec_wholeNum = await client.ReceiveAsync();

                        lenght = int.Parse(Encoding.Default.GetString(rec_lenght.Buffer));
                        wholeNum = int.Parse(Encoding.Default.GetString(rec_wholeNum.Buffer));
                        a = 0;
                        byte[] arr = new byte[lenght];

                        for (int i = 0; i < wholeNum; i++)
                        {
                            rec_Bytes = await client.ReceiveAsync();
                            for (int k = 0; k < rec_Bytes.Buffer.Length; k++)
                                arr[a++] = rec_Bytes.Buffer[k];
                        }

                        Dispatcher.Invoke(() =>
                        {
                            BitmapImage bitmap = new BitmapImage();

                            bitmap.BeginInit();
                            bitmap.StreamSource = new MemoryStream(arr);
                            bitmap.EndInit();

                            img.Source = bitmap;
                        });
                    }
                });
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
