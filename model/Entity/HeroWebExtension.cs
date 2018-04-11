using HoTS_Service.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HoTS_Service.Entity
{
    [DataContract]
    public class HeroWebExtension : StormObject
    {
        [DataMember]
        private int id;

        [DataMember]
        public string ImageUrl;

        [DataMember]
        public string IconUrl;

        [DataMember]
        public string PortraitUrl;

        [DataMember]
        public string DetailsUrl;

        public int Id { get => id; }

        public HeroWebExtension(int id)
        {
            this.id = id;
        }

    }
}
