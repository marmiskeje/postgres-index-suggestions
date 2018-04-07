using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.DAL
{
    internal static class JsonSerializationUtility
    {
        public static T Deserialize<T>(string str)
        {
            return (T)JsonConvert.DeserializeObject(str, typeof(T));
        }
        public static string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}
