using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Text;
using System.Threading;

namespace NetCoreApp.Logger.File
{
    public class FileLogger : ILogger
    {
        static protected string delimiter = new string(new char[] { (char)1 });
        public FileLogger(string categoryName)
        {
            this.Name = categoryName;
        }
        class Disposable : IDisposable
        {
            public void Dispose()
            {
            }
        }
        Disposable _DisposableInstance = new Disposable();
        public IDisposable BeginScope<TState>(TState state)
        {
            return _DisposableInstance;
        }
        public bool IsEnabled(LogLevel logLevel)
        {
            return this.MinLevel <= logLevel;
        }
        public void Reload()
        {
            _Expires = true;
        }

        public string Name { get; private set; }

        public LogLevel MinLevel { get; set; }
        public string FileDiretoryPath { get; set; }
        public string FileNameTemplate { get; set; }
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!this.IsEnabled(logLevel))
                return;
            var msg = formatter(state, exception);
            this.Write(logLevel, eventId, msg, exception);
        }
        void Write(LogLevel logLevel, EventId eventId, string message, Exception ex)
        {
            EnsureInitFile();

            //TODO 提高效率 队列写！！！
            var log = String.Concat(DateTime.Now.ToString("HH:mm:ss"), '[', logLevel.ToString(), ']', '[',
                  Thread.CurrentThread.ManagedThreadId.ToString(), ',', eventId.Id.ToString(), ',', eventId.Name, ']',
                  delimiter, message, delimiter, ex?.ToString());
            lock (this)
            {
                this._sw.WriteLine(log);
            }
        }

        bool _Expires = true;
        string _FileName;
        protected StreamWriter _sw;
        void EnsureInitFile()
        {
            if (CheckNeedCreateNewFile())
            {
                lock (this)
                {
                    if (CheckNeedCreateNewFile())
                    {
                        InitFile();
                        _Expires = false;
                    }
                }
            }
        }
        bool CheckNeedCreateNewFile()
        {
            if (_Expires)
            {
                return true;
            }
            //TODO 使用 RollingType判断是否需要创建文件。提高效率！！！
            if (_FileName != DateTime.Now.ToString(this.FileNameTemplate))
            {
                return true;
            }
            return false;
        }
        void InitFile()
        {
            if (!Directory.Exists(this.FileDiretoryPath))
            {
                Directory.CreateDirectory(this.FileDiretoryPath);
            }
            var path = "";
            int i = 0;
            do
            {
                _FileName = DateTime.Now.ToString(this.FileNameTemplate);
                path = Path.Combine(this.FileDiretoryPath, _FileName + "_" + i + ".log");
                i++;
            } while (System.IO.File.Exists(path));
            var oldsw = _sw;
            _sw = new StreamWriter(new FileStream(path, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read), Encoding.UTF8);
            _sw.AutoFlush = true;
            if (oldsw != null)
            {
                try
                {
                    _sw.Flush();
                    _sw.Dispose();
                }
                catch
                {
                }
            }
        }
    }
}
