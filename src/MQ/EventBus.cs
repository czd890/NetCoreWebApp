using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MQ.Internal;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.DependencyInjection;
namespace MQ
{
    public class EventBus
    {
        static SpinLock sl = new SpinLock();
        static JsonSerializerSettings setting = new Newtonsoft.Json.JsonSerializerSettings();
        static EventBusSettings settings;
        static ILogger _logger;
        static CacheManager _CacheManager;
        static EventBus()
        {
            setting.DateFormatString = "yyyy/MM/dd HH:mm:ss.fff";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="productId"></param>
        /// <param name="topic">exchange</param>
        /// <param name="tag">routingKey</param>
        /// <param name="id"></param>
        /// <param name="message"></param>
        public static ulong Publish<T>(string productId, string topic, string tag, string id, T message)
        {
            var t = typeof(T);
            string msgStr;
            if (t == typeof(string))
                msgStr = message.ToString();
            else
                msgStr = Newtonsoft.Json.JsonConvert.SerializeObject(message, t, setting);

            var msg = Encoding.UTF8.GetBytes(msgStr);
            bool reflock = false;
            sl.Enter(ref reflock);
            try
            {
                var channel = _CacheManager.GetChannel(productId);
                var pubNo = channel.NextPublishSeqNo;
                channel.BasicPublish(topic, tag, false, null, msg);
                return pubNo;
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (reflock)
                    sl.Exit();
            }
            return 0;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="productId"></param>
        /// <param name="topic">集群消费：queuename;广播消费:exchange</param>
        /// <param name="action"></param>
        /// <param name="options"></param>
        public static async void Subscribe<T>(string productId, string topic, Func<T, bool> action, SubscribeOptions options = null)
        {
            options = options ?? SubscribeOptions.Default;
            var queueName = topic;
            var channel = _CacheManager.GetChannel(productId);

            if (options.Model == MessageModel.Broadcasting)
            {
                var hostName = System.Net.Dns.GetHostName();
                var ipaddress = await System.Net.Dns.GetHostEntryAsync(hostName).ConfigureAwait(false);
                var ip = ipaddress.AddressList.FirstOrDefault(p => p.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)?.ToString() ?? hostName;
                var fanoutQueueName = ip + "." + productId + "." + topic + "." + DateTime.UtcNow.Ticks;
                channel.QueueDeclare(queue: fanoutQueueName);
                channel.QueueBind(fanoutQueueName, topic, "", null);
                queueName = fanoutQueueName;
            }

            var consumer = new EventingBasicConsumer(channel);
            consumer.Shutdown += (obj, ea) =>
            {
                _logger.LogError("eventbus.consumer.shutdown:" + ea.ToJson());
            };
            channel.BasicQos(0, 1, false);
            var tx = typeof(T) == typeof(string);
            consumer.Received += (obj, ea) =>
            {
                try
                {
                    var body = Encoding.UTF8.GetString(ea.Body);
                    var result = false;
                    if (tx)
                        result = action((T)((object)body));
                    else
                    {
                        var msg = JsonConvert.DeserializeObject<T>(body, setting);
                        result = action(msg);
                    }
                    if (result)
                    {
                        consumer.Model.BasicAck(ea.DeliveryTag, true);
                    }
                    else
                    {
                        consumer.Model.BasicReject(ea.DeliveryTag, false);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(0, ex, "eventbus.subscribe.consumer");
                }
            };
            _logger.LogInformation("eventbus.subscribe." + queueName);
            channel.BasicConsume(queueName, false, consumer);
        }

        public static void Exit()
        {
            ((IDisposable)_CacheManager).Dispose();
        }

        public static void Configure(IConfigurationRoot config, IServiceProvider internalServiceProvider = null)
        {
            var logger = internalServiceProvider?.GetService<ILogger<EventBus>>();
            _logger = logger;
            _CacheManager = new CacheManager();
            _CacheManager.Configure(new EventBusSettings(config, logger));
        }

    }
}
