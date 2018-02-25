using HoTS_Service.Entity.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HoTS_Service.Entity
{
    [DataContract]
    public class HeroDetails : StormObject
    {
        [DataMember]
        private int id;
        [DataMember]
        public DateTime Date;
        [DataMember]
        public int Price;
        [DataMember]
        private string title;
        [DataMember]
        public Franchise Franchise;
        [DataMember]
        public string Info;
        [DataMember]
        public string Lore;
        [DataMember]
        public Difficulty Difficulty;
        [DataMember]
        public bool Melee;
        [DataMember]
        public int Health;
        [DataMember]
        public double HealthRegen;
        [DataMember]
        public int Resource;
        [DataMember]
        public ResourceType ResourceType;
        [DataMember]
        public int SpellArmor = 0;
        [DataMember]
        public int PhysicalArmor = 0;
        [DataMember]
        public double AttackSpeed;
        [DataMember]
        public double AttackRange;
        [DataMember]
        public int AttackDamage;

        [DataMember]
        public string ImageUrl;

        [DataMember]
        public string IconUrl;

        [DataMember]
        public string DetailsUrl;


        public int Id { get => id;}
        public string Title { get => title; set => title = value; }

        public HeroDetails(int id)
        {
            this.id = id;
        }
    }
}
