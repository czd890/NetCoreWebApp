using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace NetCoreApp.Logger.File
{
    public class FileLoggerSettings
    {
        IConfiguration _configuration;
        public IChangeToken ChangeToken { get; private set; }
        public FileLoggerSettings(IConfiguration configuration)
        {
            _configuration = configuration;
            this.ChangeToken = _configuration.GetReloadToken();
        }

        public string DefaultPath
        {
            get
            {
                return this._configuration["DefaultPath"];
            }
        }

        public int DefaultMaxMB
        {
            get
            {
                return int.Parse(this._configuration["DefaultMaxMB"]);
            }
        }
        public string DefaultFileName
        {
            get { return this._configuration["DefaultFileName"]; }
        }

        public void Reload()
        {
            //update cache settings
        }

        public Tuple<bool, LogLevel> GetSwitch(string name)
        {
            var section = this._configuration.GetSection("LogLevel");
            if (section != null)
            {
                LogLevel level;
                if (Enum.TryParse(section[name], true, out level))
                    return new Tuple<bool, LogLevel>(true, level);
            }
            return new Tuple<bool, LogLevel>(false, LogLevel.None);
        }
        public Tuple<bool,string> GetDiretoryPath(string name)
        {
            var section = this._configuration.GetSection("Path");
            if (section!=null)
            {
                var path = section[name];
                if (!String.IsNullOrEmpty(path))
                {
                    return new Tuple<bool, string>(true, path);
                }
            }
            return new Tuple<bool, string>(false, this.DefaultPath);
        }
        public Tuple<bool, string> GetFileName(string name)
        {
            var section = this._configuration.GetSection("FileName");
            if (section != null)
            {
                var path = section[name];
                if (!String.IsNullOrEmpty(path))
                {
                    return new Tuple<bool, string>(true, path);
                }
            }
            return new Tuple<bool, string>(false, this.DefaultFileName);
        }
    }
}
