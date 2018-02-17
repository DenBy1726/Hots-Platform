using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;


namespace HoTS_Service.Entity
{
    [DataContract]
    /// <summary>
    /// Класс объекта игры ( герой или карта)
    /// </summary>
    public abstract class StormObject
    {

        public override string ToString()
        {
            return Util.ToString.ReflexString(this);
        }
    }

    public enum StormObjectType
    {
        Hero, Map
    }
}
