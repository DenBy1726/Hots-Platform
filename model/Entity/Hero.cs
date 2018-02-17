using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HoTS_Service.Entity
{
    [DataContract]
    public class Hero : StormObject, IComparable<Hero>
    {
        [DataMember]
        public int Id
        {
            get; private set;
        }

        [DataMember]
        public string Name
        {
            get; private set;
        }

        [DataMember]
        public Enum.HeroGroup Group
        {
            get; private set;
        }

        [DataMember]
        public Enum.HeroSubGroup SubGroup
        {
            get; private set;
        }

        public Hero(int id, string name, Enum.HeroGroup group, Enum.HeroSubGroup subgroup)
        {
            this.Id = id;
            this.Name = name;
            this.Group = group;
            this.SubGroup = subgroup;
        }

        public int CompareTo(Hero other)
        {
            return Id.CompareTo(other.Id);
        }


        public override bool Equals(object o)
        {
            Hero other = o as Hero;
            if (other == null)
            {
                return false;
            }
            return (Name == other.Name) && (Id == other.Id) && (Group == other.Group)
                && (SubGroup == other.SubGroup);
        }

        public override int GetHashCode()
        {
            return 37 * Name.GetHashCode() ^ 33 * Id.GetHashCode() ^ 19 * Group.GetHashCode()
                ^ 17 * SubGroup.GetHashCode();
        }
    }
}
