namespace Entity
{
    public class TransmissionPacket:WriteOffEntity
    {
        /// <summary>
        /// 发送人Id
        /// </summary>
        public int? SendID { get; set; }
        /// <summary>
        /// 接收人Id
        /// </summary>
        public int? accepterID { get; set; }
        /// <summary>
        /// 消息体
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public int? Type { get; set; }
        /// <summary>
        /// 日期
        /// </summary>
        public DateTime DateTime { get; set; }
    }
}