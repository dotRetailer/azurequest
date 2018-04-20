using System.IO;
using System.Text;

namespace AzureQuest.Common
{
    public static class NativeSerializer
    {
        public static string JsonSerialize(this object obj)
        {
            using (var ms = new MemoryStream())
            {
                var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(obj.GetType());
                serializer.WriteObject(ms, obj);
                string json = Encoding.Default.GetString(ms.ToArray());
                return json;
            }
        }

        public static T JsonDeserialize<T>(this string json) where T : class, new()
        {
            using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(json)))
            {
                var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(T));
                var result = serializer.ReadObject(ms) as T;
                return result;
            }
        }
    }
}
