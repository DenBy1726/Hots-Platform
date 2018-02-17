using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoTS_Service.Util
{
    public static class ToString
    {
        public static string ReflexString(object obj)
        {
            var flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.FlattenHierarchy;
            System.Reflection.PropertyInfo[] infos = obj.GetType().GetProperties(flags);

            StringBuilder sb = new StringBuilder();

            string typeName = obj.GetType().Name;
            sb.AppendLine(typeName);
            sb.AppendLine(" ");

            foreach (var info in infos)
            {
                object value = info.GetValue(obj, null);
                sb.AppendFormat(" {0}: {1}{2}", info.Name, value != null ? value : "null", Environment.NewLine);
            }

            return sb.ToString();
        }
    }
}
