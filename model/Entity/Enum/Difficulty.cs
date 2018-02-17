using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HoTS_Service.Entity.Enum
{
    [DataContract]
    public enum Difficulty
    {
        Unknown,Easy,Medium,Hard,VeryHard
    }
}
