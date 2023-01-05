// See https://aka.ms/new-console-template for more information
using System.Net.Sockets;
using System.Net;
using System.Text;

int _port = 12345;
int usernumber = 0;
Socket clientSocket = null;
//服务器端Socket对象
Socket _socket;
byte[] data = new byte[1024];
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
        _socket = new(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        //创建IP对象
        List<IPAddress> iPAddresses = GetIPAddresses();
        //创建IP地址和端口号对象
       // IPAddress ip = IPAddress.Any; 
        IPEndPoint point = new(iPAddresses[0], _port);

        //_socket.Bind(new IPEndPoint(iPAddresses[0], _port));
        _socket.Bind(point);
        //设置最大连接数
        //_socket.Listen(int.MaxValue);
        Console.WriteLine("监听{0}消息成功", _socket.LocalEndPoint);
        //连接客户端  
        //AsyncAccept(_socket);
        //开始监听
        Task.Run(() => udpReceiveMessage(_socket));

    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
    }
}

void SendMsg(Socket _socket,IPEndPoint remoteEndPoint,string msg)
{
    _socket.SendTo(Encoding.UTF8.GetBytes(msg), remoteEndPoint);
}

void udpReceiveMessage(Socket _socket)
{
    //while (true)
    //{
        EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
        //int length = _socket.ReceiveFrom(data, ref remoteEndPoint);//这个方法会把数据的来源(ip:port)放到第二个参数上
        //string message = Encoding.UTF8.GetString(data, 0, length);
        _socket.BeginReceiveFrom(data, 0, data.Length, SocketFlags.None,
                ref remoteEndPoint, new AsyncCallback(ReceiveData), remoteEndPoint);
       // _socket.BeginReceive(data, 0, data.Length, SocketFlags.None,
        //asyncResult =>
        //{
        //    int length = _socket.EndReceive(asyncResult);
        //    Console.WriteLine(string.Format("客户端发送消息:{0}", Encoding.UTF8.GetString(data)));
        //    string code = "200";
        //}, null);
    //}
}
void ReceiveData(IAsyncResult iar)
{
    //客户端的IP和端口，端口 0 表示任意端口
    IPEndPoint client = new IPEndPoint(IPAddress.Any, 0);
    //实例化客户端 终点
    EndPoint epSender = client;
    //结束异步接收消息  recv 表示接收到的字符数
    int recv = _socket.EndReceiveFrom(iar, ref epSender);
    //将接收到的数据打印出来，发送方采用什么编码方式，此处就采用什么编码方式 转换成字符串
    Console.WriteLine("IP{0}:{1}" , (epSender as IPEndPoint).Port, Encoding.UTF8.GetString(data, 0, recv));
    //定义要发送回客户端的消息，采用ASCII编码，
    //如果要发送汉字或其他特殊符号，可以采用UTF-8           
    byte[] sendData = Encoding.UTF8.GetBytes("hello");
    //开始异步发送消息  epSender是上次接收消息时的客户端IP和端口信息
    _socket.BeginSendTo(sendData, 0, sendData.Length, SocketFlags.None,
        epSender, new AsyncCallback(SendData), epSender);
    //重新实例化接收数据字节数组
    data = new byte[1024];
    //开始异步接收消息，此处的委托函数是这个函数本身，递归
    _socket.BeginReceiveFrom(data, 0, data.Length, SocketFlags.None,
        ref epSender, new AsyncCallback(ReceiveData), epSender);
}
void SendData(IAsyncResult iar)
{
    _socket.EndSend(iar);
}

#region tcp

#region 异步方式
/// <summary>  
/// 连接到客户端  
/// </summary>  
/// <param name="socket"></param>  
void AsyncAccept(Socket socket)
{
    socket.BeginAccept(asyncResult =>
    {
        //获取客户端套接字  
        Socket client = socket.EndAccept(asyncResult);
        Console.WriteLine(string.Format("客户端{0}请求连接...", client.RemoteEndPoint));
        AsyncSend(client, "服务器收到连接请求");
        AsyncSend(client, string.Format("欢迎你{0}", client.RemoteEndPoint));
        AsyncReveive(client);
    }, null);
}

/// <summary>  
/// 接收消息  
/// </summary>  
/// <param name="client"></param>  
void AsyncReveive(Socket socket)
{
    byte[] data = new byte[1024];
    try
    {
        //开始接收消息  
        socket.BeginReceive(data, 0, data.Length, SocketFlags.None,
        asyncResult =>
        {
            int length = socket.EndReceive(asyncResult);
            Console.WriteLine(string.Format("客户端发送消息:{0}", Encoding.UTF8.GetString(data)));
        }, null);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
}

/// <summary>  
/// 发送消息  
/// </summary>  
/// <param name="client"></param>  
/// <param name="p"></param>  
void AsyncSend(Socket client, string p)
{
    if (client == null || p == string.Empty) return;
    //数据转码  
    byte[] data = new byte[1024];
    data = Encoding.UTF8.GetBytes(p);
    try
    {
        //开始发送消息  
        client.BeginSend(data, 0, data.Length, SocketFlags.None, asyncResult =>
        {
            //完成消息发送  
            int length = client.EndSend(asyncResult);
            //输出消息  
            Console.WriteLine(string.Format("服务器发出消息:{0}", p));
        }, null);
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
    }
}
#endregion
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
            string remoteEndPoint = clientSocket.RemoteEndPoint?.ToString();
            //将远程连接的客户端的IP和Socket存入集合中
            dicSocket.Add(remoteEndPoint, clientSocket);
            //获取客户端的IP和端口号  
            IPAddress clientIP = (clientSocket.RemoteEndPoint as IPEndPoint)?.Address;
            int clientPort = (clientSocket.RemoteEndPoint as IPEndPoint).Port;
            usernumber++;
            string text = string.Format("服务端连接成功!当前客户端在线用户数量{0}-----{1}", usernumber,DateTime.Now);
            string sendmsg = string.Format("本地IP:{0},clientIP：{1}", clientIP?.ToString(), clientPort);
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

#endregion

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
            dicSocket.Remove(clientSocket.RemoteEndPoint?.ToString());
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