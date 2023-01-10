// See https://aka.ms/new-console-template for more information
using Entity;
using Newtonsoft.Json;
using System.Net;
using System.Net.Sockets;
using System.Text;

int Udp_port = 12345;
int Tcp_port = 12346;
//创建IP对象
List<IPAddress> iPAddresses = GetIPAddresses();
byte[] data = new byte[1024];
//将远程客户端的IP地址和Socket存入集合中
Dictionary<string, Socket> dicSocket = new();

Console.WriteLine("Hello, World!");
TcpListen();
udpListen();

SocketConnect socketConnect=new SocketConnect();
var a=socketConnect;

Console.ReadKey();

//socket udp
void udpListen()
{
    try
    {
        // 实例化套接字(IP4寻找协议,流式协议,Udp协议)
        Socket _socket = new(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        //创建IP地址和端口号对象
        IPEndPoint point = new(iPAddresses[0], Udp_port);
        _socket.Bind(point);
        Console.WriteLine("Udp协议,监听{0}成功", _socket.LocalEndPoint);
        //开始监听
        Task.Run(() => udpReceiveMessage(_socket));
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
    }
}

//socket udp
void TcpListen()
{
    try
    {
        // 实例化套接字(IP4寻找协议,流式协议,TCP协议)
        Socket _socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPEndPoint point = new(iPAddresses[0], Tcp_port);
        _socket.Bind(point);
        //设置最大连接数
        _socket.Listen(int.MaxValue);
        Console.WriteLine("Tcp协议,监听{0}成功", _socket.LocalEndPoint);
        //开始监听
        Task.Run(() => TcpReceiveMessage(_socket));
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
    }
}

//tcp消息处理
async Task TcpReceiveMessage(Socket _socket)
{
    EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
    //接收数据并返回发送主机的端点
    var SocketReceiveFromResult = await _socket.ReceiveFromAsync(data, remoteEndPoint);
    //将接收到的数据打印出来，发送方采用什么编码方式，此处就采用什么编码方式 转换成字符串
    string messgae = Encoding.UTF8.GetString(data, 0, SocketReceiveFromResult.ReceivedBytes);
    IPEndPoint iPEndPoint = (IPEndPoint)SocketReceiveFromResult.RemoteEndPoint;
    string host = $"{iPEndPoint.Address}:{iPEndPoint.Port}";
    Console.WriteLine("客户端IP地址:{0} 消息内容:{1}", host, messgae);
    //定义要发送回客户端的消息，采用UTF-8 编码，    
    byte[] sendData = Encoding.UTF8.GetBytes("200");
    //开始异步发送消息  epSender是上次接收消息时的客户端IP和端口信息
    int number = await _socket.SendToAsync(sendData, SocketReceiveFromResult.RemoteEndPoint);
    //重新实例化接收数据字节数组
    data = new byte[1024];
    //递归
    await TcpReceiveMessage(_socket);
}
//udp消息处理
async Task udpReceiveMessage(Socket _socket)
{
    EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
    //接收数据并返回发送主机的端点
    var SocketReceiveFromResult = await _socket.ReceiveFromAsync(data, remoteEndPoint);
    //将接收到的数据打印出来，发送方采用什么编码方式，此处就采用什么编码方式 转换成字符串
    string messgae = Encoding.UTF8.GetString(data, 0, SocketReceiveFromResult.ReceivedBytes);
    IPEndPoint iPEndPoint = (IPEndPoint)SocketReceiveFromResult.RemoteEndPoint;
    string host = $"{iPEndPoint.Address}:{iPEndPoint.Port}";
    Console.WriteLine("客户端IP地址:{0} 消息内容：{1}", host, messgae);
    if (!string.IsNullOrWhiteSpace(messgae))
    {
        var obj =JsonConvert.DeserializeObject<dynamic>(messgae);
        int code = obj?["CheckOffCode"] ?? 0;
        int retcode = Returnclientinformation(code);

        TransmissionPacket writeOffEntity = new()
        {
            CheckOffCode = (CheckOffCodeEn)retcode,
            Message = obj?["Message"]
        };
        //定义要发送回客户端的消息，采用UTF-8 编码，    
        byte[] sendData = Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(writeOffEntity));
        //开始异步发送消息  epSender是上次接收消息时的客户端IP和端口信息
        int number = await _socket.SendToAsync(sendData, SocketReceiveFromResult.RemoteEndPoint);
    }
    //重新实例化接收数据字节数组
    data = new byte[1024];
    //递归
    await udpReceiveMessage(_socket);
}

int Returnclientinformation(int code)
{
    return code switch
    {
        0 => (int)CheckOffCodeEn.None,
        1 => (int)CheckOffCodeEn.OK,
        _ => (int)CheckOffCodeEn.None,
    };
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