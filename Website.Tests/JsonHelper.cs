using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Website.Tests
{
    namespace Opuno.Brenn.WindowsPhone.Helpers
    {
        using System;
        using System.IO;
        using System.Text;

        public class JsonHelper
        {
            public static string Serialize<T>(T obj)
            {
                var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(obj.GetType());
                var ms = new MemoryStream();
                serializer.WriteObject(ms, obj);
                var arr = ms.ToArray();
                string retVal = Encoding.UTF8.GetString(arr, 0, arr.Length);
                ms.Dispose();
                return retVal;
            }

            public static T Deserialize<T>(string json)
            {
                T obj = Activator.CreateInstance<T>();
                var ms = new MemoryStream(Encoding.Unicode.GetBytes(json));
                var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(obj.GetType());
                obj = (T)serializer.ReadObject(ms);
                ms.Close();
                ms.Dispose();
                return obj;
            }
        }
    }

}
