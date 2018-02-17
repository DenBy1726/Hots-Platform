using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoTS_Service.Entity
{
    public class ReplaySchema : IComparable<ReplaySchema>
    {
        public int id;
        public Enum.GameMode gameMode;
        public int mapId;
        /// в секундах
        public int length;

        public int CompareTo(ReplaySchema other)
        {
            return id.CompareTo(other.id);
        }

        public override bool Equals(object o)
        {
            ReplaySchema other = o as ReplaySchema;
            if (other == null)
            {
                return false;
            }
            return (id == other.id) && (gameMode == other.gameMode) && (mapId == other.mapId)
                && (length == other.length);
        }

        public override int GetHashCode()
        {
            return 37 * id.GetHashCode() ^ 33 * gameMode.GetHashCode() ^ 
                27 * mapId.GetHashCode() ^ 23 * length.GetHashCode();
        }
    }
}
