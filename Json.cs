using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Pocoyo
{
    public static class Json
    {
        public static string Serialize<T>(T data, bool formatted = true)
        {
            if (formatted)
            {
                return JsonConvert.SerializeObject(data, new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                });
            }

            return JsonConvert.SerializeObject(data);
        }

        public static string SerializeType<T>(T data, bool formatted = true)
        {
            var options = new JsonSerializerSettings
            {
                ContractResolver = new TypeOnlyContractResolver<T>(),
                Formatting = formatted ? Formatting.Indented : Formatting.None
            };

            return JsonConvert.SerializeObject(data, options);
        }

        public static string SerializeListType<T>(List<T> data, bool formatted = true)
        {
            var options = new JsonSerializerSettings
            {
                ContractResolver = new ListTypeOnlyContractResolver<T>(),
                Formatting = formatted ? Formatting.Indented : Formatting.None
            };

            return JsonConvert.SerializeObject(data, options);
        }

        public static bool SerializeToFile<T>(T data, string filePath, bool formatted = true)
        {
            try
            {
                var json = Serialize(data, formatted);
                return SharedFile.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                Log.Exception(filePath, ex);
                //throw new Exception(string.Format("Json parse error: {0}\r\n{1}\r\n", ex.Message, filePath));
            }
            return false;
        }

        public static T Deserialize<T>(string json, bool formatted = true, bool camelCase = false)
        {
            var idxNull = json.IndexOf('\0');
            if (idxNull != -1)
                json = json.Substring(0, idxNull);

            var jsonSettings = new JsonSerializerSettings();
            if (formatted)
                jsonSettings.Formatting = Formatting.Indented;
            if (camelCase)
                jsonSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            return JsonConvert.DeserializeObject<T>(json, jsonSettings);
        }

        public static T Deserialize<T>(string json, string typeName, bool formatted = true, bool camelCase = false) where T: class
        {
            var idxNull = json.IndexOf('\0');
            if (idxNull != -1)
                json = json.Substring(0, idxNull);

            var type = Type.GetType(typeName);

            var jsonSettings = new JsonSerializerSettings();
            if (formatted)
                jsonSettings.Formatting = Formatting.Indented;
            if (camelCase)
                jsonSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            return JsonConvert.DeserializeObject(json, type, jsonSettings) as T;
        }

        public static object Deserialize(string json, string typeName, bool formatted = true, bool camelCase = false)
        {
            var idxNull = json.IndexOf('\0');
            if (idxNull != -1)
                json = json.Substring(0, idxNull);

            var type = Type.GetType(typeName);

            var jsonSettings = new JsonSerializerSettings();
            if (formatted)
                jsonSettings.Formatting = Formatting.Indented;
            if (camelCase)
                jsonSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            return JsonConvert.DeserializeObject(json, type, jsonSettings);
        }

        public static T DeserializeType<T>(string json, bool formatted = true)
        {
            var idxNull = json.IndexOf('\0');
            if (idxNull != -1)
                json = json.Substring(0, idxNull);

            var options = new JsonSerializerSettings
            {
                ContractResolver = new TypeOnlyContractResolver<T>(),
                Formatting = formatted ? Formatting.Indented : Formatting.None
            };

            return JsonConvert.DeserializeObject<T>(json, options);
        }

        public static dynamic DeserializeFromFile(string filePath)
        {
            var json = "";
            try
            {
                if (!File.Exists(filePath))
                    return new JObject();

                json = SharedFile.ReadAllText(filePath);
                if (string.IsNullOrEmpty(json))
                    return new JObject();

                var idxNull = json.IndexOf('\0');
                if (idxNull != -1)
                    json = json.Substring(0, idxNull);

                dynamic data = JObject.Parse(json);
                return data;
            }
            catch (Exception ex)
            {
                Log.Warn("Json parse error: {0}\r\n{1}\r\n{2}\r\n", ex.Message, filePath, json, ex);
                //throw new Exception(string.Format("Json parse error: {0}\r\n{1}\r\n", ex.Message, filePath));
                return new JObject();
            }
        }

        public static T DeserializeFromFile<T>(string filePath)
        {
            var json = "";
            try
            {
                json = SharedFile.ReadAllText(filePath);
                return Deserialize<T>(json);
            }
            catch (Exception ex)
            {
                Log.Error("Json parse error: {0}\r\n{1}\r\n{2}\r\n", ex.Message, filePath, json, ex);
                throw;
            }
        }

        public static T TryDeserializeFromFile<T>(string filePath) where T : class
        {
            var json = "";
            try
            {
                json = SharedFile.ReadAllText(filePath);
                return Deserialize<T>(json);
            }
            catch (Exception ex)
            {
                Log.Warn("Json parse error: {0}\r\n{1}\r\n{2}\r\n", ex.Message, filePath, json, ex);
                return null;
            }
        }
        
    }

    public class TypeOnlyContractResolver<T> : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
#if Diagnostic
            var serialize = property.DeclaringType == typeof(T) || property.DeclaringType.DeclaringType == typeof(T);
            if (serialize)
                Log.Info("Serialize {0} {1}", member.Name, property.DeclaringType.FullName);
            else
                Log.Warn("Not serializing {0} {1}", member.Name, property.DeclaringType.FullName);
#endif
            property.ShouldSerialize = instance => property.DeclaringType == typeof(T) || property.DeclaringType.DeclaringType == typeof(T);
            return property;
        }
    }

    public class ListTypeOnlyContractResolver<T> : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
#if Diagnostic
            var serialize = property.DeclaringType == typeof(T) || property.DeclaringType.DeclaringType == typeof(T);
            if (serialize)
                Log.Info("Serialize {0} {1}", member.Name, property.DeclaringType.FullName);
            else
                Log.Warn("Not serializing {0} {1}", member.Name, property.DeclaringType.FullName);
#endif
            property.ShouldSerialize = instance => property.DeclaringType == typeof(T) || property.DeclaringType.DeclaringType == typeof(T) || property.DeclaringType == typeof(List<T>) || property.DeclaringType.DeclaringType == typeof(List<T>);
            return property;
        }
    }

}
