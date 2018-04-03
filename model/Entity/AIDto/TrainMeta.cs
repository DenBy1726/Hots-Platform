using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HoTS_Service.Entity.AIDto
{
    [DataContract]
    public class TrainMeta
    {
        [DataMember]
        public string Name;
        [DataMember]
        public string ClusterPath;
        [DataMember]
        public string Alias;
        [DataMember]
        public string Description;

        public override string ToString()
        {
            return HoTS_Service.Util.ToString.ReflexString(this);
        }
    }
}
