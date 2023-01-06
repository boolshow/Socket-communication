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
        Open= 1,//申请创建连接
        Miss=2,//连接失败
        OK=3,//主机握手成功
        OutAge=5,//断开链接
        OverTime=9,//超时
        MeSu =10,//消息发送成功
    }
}
