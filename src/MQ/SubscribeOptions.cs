using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MQ
{
    public class SubscribeOptions
    {
        public MessageModel Model { get; set; }

        public static SubscribeOptions Default { get; } = new SubscribeOptions()
        {
            Model = MessageModel.Clustering
        };
    }
    /// <summary>
    /// 消息消费方式
    /// </summary>
    public enum MessageModel
    {
        /// <summary>
        /// 广播消费
        /// </summary>
        Broadcasting = 1,
        /// <summary>
        /// 集群消费,默认方式
        /// </summary>
        Clustering = 2
    }
}
