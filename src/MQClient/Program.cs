using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Practices.ServiceLocation;
using MQ;
using MQ.Internal;
using RabbitMQ.Client;
using RabbitMQ.Client.Content;
using RabbitMQ.Client.Events;

namespace MQClient
{
    public class Program
    {
        static IServiceProvider sp;

        public static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var appsettings = new ConfigurationBuilder()
              .SetBasePath(System.IO.Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();

            var mqsettings = new ConfigurationBuilder()
               .SetBasePath(System.IO.Directory.GetCurrentDirectory())
               .AddJsonFile("mqsettings.json", optional: true, reloadOnChange: true).Build();

            //----------------------------------------------------------------------------------------


            ServiceCollection sc = new ServiceCollection();
            sc.AddLogging();

            sc.AddDbContext<DbContext>(ServiceLifetime.Transient);

            sp = sc.BuildServiceProvider();
            //----------------------------------------------------------------------------------------

            sp.GetService<ILoggerFactory>()
                .AddConsole()
                .AddDebug()
                .AddFile(appsettings.GetSection("FileLogging"));

            EventBus.Configure(mqsettings, sp);

            //主题发布订阅
            EventBus.Subscribe<DateTime>("produtid", "queue_3", time =>
            {
                //Console.WriteLine(time);
                return true;
            });


            //广播方式
            EventBus.Subscribe<DateTime>("produt", "guangbo", message =>
            {
                Console.WriteLine(message);
                return true;
            }, new SubscribeOptions() { Model = MessageModel.Broadcasting });

            EventBus.Subscribe<string>("produt", "queue_1", message =>
            {
                return true;
            });


            Console.WriteLine("开始监听");
            Console.ReadLine();
            EventBus.Exit();
        }
    }
}
