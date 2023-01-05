// See https://aka.ms/new-console-template for more information
using System.Net.Sockets;
using System.Net;
using System.Text;

int _port = 12345;
int usernumber = 0;
Socket clientSocket = null;
//将远程客户端的IP地址和Socket存入集合中
Dictionary<string, Socket> dicSocket = new();

Console.WriteLine("Hello, World!");
StartListen();
Console.ReadLine();

void StartListen()
{
    try
    {
        // 实例化套接字(IP4寻找协议,流式协议,TCP协议)
        Socket _socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Udp);
        //创建IP对象
        List<IPAddress> iPAddresses = GetIPAddresses();
        foreach (IPAddress ip in iPAddresses)
        {
            Console.WriteLine(ip.ToString());
        }
        //创建IP地址和端口号对象
       // IPAddress ip = IPAddress.Any; 
        IPEndPoint point = new(iPAddresses[0], _port);

        //_socket.Bind(new IPEndPoint(iPAddresses[0], _port));
        _socket.Bind(point);
        //设置最大连接数
        _socket.Listen(int.MaxValue);
        Console.WriteLine("监听{0}消息成功", _socket.LocalEndPoint);
        //开始监听
        Task.Run(() => ListenClientConnect(_socket));

    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
    }
}

    /// <summary>
    /// 监听客户端连接
    /// </summary>
void ListenClientConnect(Socket _socket)
{
    try
    {
        while (true)
        {
            try
            {
                //Socket创建的新连接
                clientSocket = _socket.Accept();
            } catch (Exception ex) {
                //提示套接字监听异常     
                Console.WriteLine(ex.Message);
                break;
            }
            //客户端网络结点号  
            string remoteEndPoint = clientSocket.RemoteEndPoint.ToString();
            //将远程连接的客户端的IP和Socket存入集合中
            dicSocket.Add(remoteEndPoint, clientSocket);
            //获取客户端的IP和端口号  
            IPAddress clientIP = (clientSocket.RemoteEndPoint as IPEndPoint).Address;
            int clientPort = (clientSocket.RemoteEndPoint as IPEndPoint).Port;
            usernumber++;
            string text = string.Format("服务端连接成功!当前客户端在线用户数量{0}-----{1}", usernumber,DateTime.Now);
            string sendmsg = string.Format("本地IP:{0},clientIP：{1}", clientIP.ToString(), clientPort);
            clientSocket.Send(Encoding.UTF8.GetBytes(text));
            clientSocket.Send(Encoding.UTF8.GetBytes(sendmsg));
            //接收客户端消息线程
            Task.Run(() => ReceiveMessage(clientSocket));
        }
    }
    catch (Exception)
    {
    }
}

/// <summary>
/// 接收客户端消息
/// </summary>
/// <param name="socket">来自客户端的socket</param>
void ReceiveMessage(Socket clientSocket)
{
    while (true)
    {
        try
        {
            byte[] vs = new byte[1024];
            //获取从客户端发来的数据
            int length = clientSocket.Receive(vs);
            Console.WriteLine("接收客户端{0},消息{1}", clientSocket.RemoteEndPoint, Encoding.UTF8.GetString(vs, 0, length));
            clientSocket.Send(vs);
        }
        catch (Exception ex)
        {
            dicSocket.Remove(clientSocket.RemoteEndPoint.ToString());
            usernumber--;
            Console.WriteLine(ex.Message);
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
            break;
        }
    }
}

//获取ip地址
List<IPAddress> GetIPAddresses()
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