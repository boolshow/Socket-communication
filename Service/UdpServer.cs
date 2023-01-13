using Entity;
using Newtonsoft.Json;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Service
{
    public class UdpServer
    {
        private List<IPAddress> _ipList;
        public UdpServer()
        {
            _ipList = GetIPAddresses();
        }
        /// <summary>
        /// 创建链接
        /// </summary>
        /// <returns></returns>
        public async Task<Socket> GetSocketConnect()
        {
            try
            {
                WriteOffEntity writeOff = new()
                {
                    CheckOffCode = CheckOffCodeEn.Open,
                    CheckOffInformation = "创建连接!"
                };
                //创建负责通信的Socket
                Socket socket = new(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)
                {
                    SendTimeout = 10000
                };
                IPEndPoint point = new(_ipList[0], 12345);
                await socket.ConnectAsync(point);

                string conntext = System.Text.Json.JsonSerializer.Serialize(writeOff);
                //发送连接信息
                await AsynSend(socket, conntext);
                //获取响应信息
                bool ispoll = true;
                //定义一个超时时间(单位毫秒)
                int timeout = 10000;
                DateTime starttime = DateTime.Now;
                while (ispoll)
                {
                    DateTime currentTime = DateTime.Now;
                    if ((currentTime - starttime).Milliseconds > timeout)
                        break;
                    string messgae = await AsynRecive(socket);
                    var retjson = JsonConvert.DeserializeObject<dynamic>(messgae);
                    int code = retjson["CheckOffCode"] ?? 0;
                    if (Enum.IsDefined(typeof(CheckWinCodeEn), code))
                    {
                        ispoll = false;
                        break;
                    }
                }
                return socket;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>  
        /// 发送消息  
        /// </summary>  
        /// <param name="socket"></param>   
        /// <param name="message"></param>  
        public async Task<int> AsynSend(Socket socket, string message)
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
                throw;
            }
        }

        /// <summary>  
        /// 接收消息  
        /// </summary>  
        /// <param name="socket"></param>  
        public async Task<string> AsynRecive(Socket socket)
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
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 获取ip地址
        /// </summary>
        /// <returns></returns>
        public static List<IPAddress> GetIPAddresses()
        {
            List<IPAddress> iPAddresses = new();

            foreach (IPAddress ip in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    iPAddresses.Add(ip);
                }
            }
            return iPAddresses;
        }
    }
}
