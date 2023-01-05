using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using static System.Collections.Specialized.BitVector32;

namespace 客户端
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //循环用到socket
        public Socket socket = null;
        //发送消息需要提供
        public IPEndPoint sendPoint = new(IPAddress.Broadcast, 12345);

        public MainWindow()
        {
            InitializeComponent();
            Initenter();
        }

        private void Initenter()
        {
            try
            {
                List<IPAddress> iPAddresses = GetIPAddresses();
                //创建负责通信的Socket
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Udp)
                {
                    SendTimeout = 10000
                };
                IPEndPoint point = new(iPAddresses[0], 12345);

                //获得要链接的远程服务器应用程序的IP地址和端口号
                socket.Connect(point);

                Console.WriteLine("连接成功");
                Task.Run(Recive);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// 不停的接受服务器发来的消息
        /// </summary>
        public void Recive()
        {
            Action<string, string> action = new(UIupdate);
            EndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, 12345);
            while (true)
            {
                //缓冲区
                byte[] vs = new byte[1024];
                //参数  缓冲区   获取到的内容保存到了iPEndPoint
                if (!socket.Connected)
                {
                    MessageBox.Show("与服务器断开链接");
                }
                else
                {
                    int r = socket.Receive(vs);
                    string p = Encoding.UTF8.GetString(vs);
                    //UI界面的内容和获取到的内容在不同的线程中，所以要跨线程调用
                    Dispatcher.Invoke(action, DateTime.Now.ToString(), p);//在这里执行action，获取ip和内容
                }
            }
        }

        //对ui线程进行操作
        private void UIupdate(string arg1, string arg2)
        {
            StringBuilder stringBuilder = new();
            stringBuilder.Append(arg1);
            stringBuilder.Append(':');
            stringBuilder.Append(arg2);
            Label label = new()
            {
                Content = stringBuilder.ToString()
            };
            MsgWindow.Children.Add(label);
        }

        //获取ip地址
        public static List<IPAddress> GetIPAddresses()
        {
            List<IPAddress> iPAddresses = new();

            foreach (IPAddress ip in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (ip.AddressFamily.ToString() == "InterNetwork")
                {
                    iPAddresses.Add(ip);
                }

            }
            return iPAddresses;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //发送消息

            byte[] vs = Encoding.UTF8.GetBytes(MsgBox.Text);
            socket.SendTo(vs, sendPoint);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            //释放socket
            socket?.Close();
            Application.Current.Shutdown(0);
        }
    }
}
