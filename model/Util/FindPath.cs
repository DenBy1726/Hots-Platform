using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HoTS_Service.Util
{
    public class FindPath
    {
        public static T GetDeepPropertyValue<T>(object instance, string path)
        {
            var pp = path.Split('.');
            Type t = instance.GetType();
            foreach (var prop in pp)
            {
                PropertyInfo propInfo = t.GetProperty(prop);
                if (propInfo != null)
                {
                    instance = propInfo.GetValue(instance, null);
                    t = propInfo.PropertyType;
                }
                else throw new ArgumentException("Properties path is not correct");
            }
            return (T)instance;
        }

        public static object GetDeepPropertyValue(object instance, string path)
        {
            return GetDeepPropertyValue<object>(instance, path);
        }
    }
}
