using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class Enumeration
    {
    }

    public enum CheckOffCodeEn 
    {
        None = 0,//无连接
        Open = 1,//申请创建连接   
        Miss = 2,//连接失败
        OK = 3,//主机握手成功
        OutAge = 5,//断开链接
        Send = 8,//发送消息
        OverTime = 9,//超时
        MeSu = 10,//消息发送成功
    }

    public enum CheckOnCodeEn
    {
        Open = 1,//申请创建连接
        Send = 8,//发送消息
    }

    public enum CheckWinCodeEn
    {
        OK = 3,//主机握手成功
        MeSu = 10,//消息发送成功
    }

    public enum CheckErrorCodeEn
    {
        None = 0,//无连接
        Miss = 2,//连接失败
        OutAge = 5,//断开链接
        OverTime = 9,//超时
    }
}
