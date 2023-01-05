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
        static readonly List<IPAddress> iPAddresses = GetIPAddresses();
        //发送消息需要提供
        public IPEndPoint sendPoint = new(iPAddresses[0], 12345);
        //public IPEndPoint sendPoint = new(IPAddress.Broadcast, 12345);

        public MainWindow()
        {
            InitializeComponent();
            Initenter();
        }

        private async void Initenter()
        {
            try
            {

                //创建负责通信的Socket
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)
                {
                    SendTimeout = 10000
                };
                IPEndPoint point = new(iPAddresses[0], 12345);
                await socket.ConnectAsync(point);
                await Task.Run(async () => await ForRecive(socket));

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        /// <summary>  
        /// 发送消息  
        /// </summary>  
        /// <param name="socket"></param>  
        /// <param name="message"></param>  
        private async Task<int> AsynSend(Socket socket, string message)
        {
            if (socket == null || message == string.Empty) return 0;
            //编码  
            byte[] data = Encoding.UTF8.GetBytes(message);
            try
            {
                return await socket.SendAsync(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine("异常信息：{0}", ex.Message);
                return 0;
            }
        }

        /// <summary>  
        /// 接收消息  
        /// </summary>  
        /// <param name="socket"></param>  
        private async Task<string> AsynRecive(Socket socket)
        {
            byte[] data = new byte[1024];
            try
            {
                int length = await socket.ReceiveAsync(data);
                if (length > 0)
                {
                    string messgae = Encoding.UTF8.GetString(data);
                    return messgae;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("异常信息：{0}", ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 循环接受消息
        /// </summary>
        /// <param name="_socket"></param>
        /// <returns></returns>
        private async Task ForRecive(Socket _socket)
        {
            Action<string, string> action = new(UIupdate);
            string messgae = await AsynRecive(_socket);
            if (messgae != null)
            {
                //UI界面的内容和获取到的内容在不同的线程中，所以要跨线程调用
                Dispatcher.Invoke(action, DateTime.Now.ToString(), messgae);//在这里执行action，获取内容
            }
            //递归
            await ForRecive(socket);
        }

        #region
        /// <summary>
        /// 对ui线程进行操作
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
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

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            //发送消息
            int i = await AsynSend(socket, MsgBox.Text);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            //释放socket
            socket?.Close();
            Application.Current.Shutdown(0);
        }
        #endregion
    }
}
