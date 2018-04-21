using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.DAL
{
    internal static class JsonSerializationUtility
    {
        private static JsonSerializerSettings settings = new JsonSerializerSettings();
        static JsonSerializationUtility()
        {
            settings.TypeNameHandling = TypeNameHandling.Auto;
        }
        public static T Deserialize<T>(string str)
        {
            return (T)JsonConvert.DeserializeObject(str, typeof(T), settings);
        }
        public static string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, settings);
        }
    }
}
