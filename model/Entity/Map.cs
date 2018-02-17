using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HoTS_Service.Entity
{
    [DataContract]
    public class Map : StormObject, IComparable<Map>
    {
        [DataMember]
        public int Id
        {
            get;private set;
        }

        [DataMember]
        public string Name
        {
            get;private set;
        }

        public Map(int id,string name)
        {
            this.Id = id;
            this.Name = name;

        }

        public int CompareTo(Map other)
        {
            return Id.CompareTo(other.Id);
        }

        public override bool Equals(object o)
        {
            Map other = o as Map;
            if (other == null)
            {
                return false;
            }
            return (Name == other.Name) && (Id == other.Id);
        }

        public override int GetHashCode()
        {
            return 37 * Name.GetHashCode() ^ 33 * Id.GetHashCode() ^ 19;
        }
    }
}
