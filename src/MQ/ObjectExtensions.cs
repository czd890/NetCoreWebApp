using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace System
{
    public static class ObjectExtensions
    {
        public static string ToJson<T>(this T obj, JsonSerializerSettings setting = null)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj, typeof(T), setting);
        }
    }
}
