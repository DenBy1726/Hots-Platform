using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace HoTS_Service.Util
{
 
    public static class JSONWebParser
    {
        /// <summary>
        /// возвращает граф объекта, созданный из json
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static object Load(string json)
        {
            var ser = new JavaScriptSerializer();
            return ser.DeserializeObject(json);
        }
    }

    public static class JSonParser
    {

        public static object Load(string json,Type objectType)
        {
            object deser;
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json));
            DataContractJsonSerializer ser = new DataContractJsonSerializer(objectType);
            deser = ser.ReadObject(ms);
            ms.Close();
            return deser;
        }

        public static string Save(object obj,Type objectType)
        {
            MemoryStream stream1 = new MemoryStream();
            DataContractJsonSerializer ser = new DataContractJsonSerializer(objectType);
            ser.WriteObject(stream1, obj);
            stream1.Position = 0;
            StreamReader sr = new StreamReader(stream1);
            return sr.ReadToEnd();

        }
    }
}
