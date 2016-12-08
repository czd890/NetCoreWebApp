using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MQ;

namespace MSServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var factory = new RabbitMQ.Client.ConnectionFactory();
            //factory.HostName = "192.168.50.2";
            //factory.UserName = "user";
            //factory.Password = "user";
            //factory.VirtualHost = "/";

            //using (var connection = factory.CreateConnection())
            //{
            //    using (var channel = connection.CreateModel())
            //    {
            //        //channel.ExchangeDeclare("ext1", "direct", true, false, null);
            //        //channel.QueueDeclare("queue_1", true, false, false, null);
            //        //channel.QueueBind("queue_1", "ext1", "route", null);

            //        var prop = channel.CreateBasicProperties();
            //        prop.DeliveryMode = 2;
            //        prop.Headers = new Dictionary<string, object>();
            //        prop.Headers["msgid"] = Guid.NewGuid().ToString("N");
            //        var t = true;
            //        int i = 0;
            //        while (true)
            //        {
            //            i++;
            //            channel.BasicPublish("amq.topic", ((t = !t) ? "route" : "route.a"),
            //                true, prop, Encoding.UTF8.GetBytes(DateTime.Now.ToString()));
            //        }
            //    }
            //}

            for (int i = 0; i < 100; i++)
            {
                //EventBus.Publish<DateTime>("amq.topic", "PPP", "", DateTime.Now);
                EventBus.Publish<DateTime>("productid","guangbo", "", "", DateTime.Now);
            }

        }
    }
}
