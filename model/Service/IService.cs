
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoTS_Service.Service
{
    public interface IService<StormObject>
    {

        StormObject Find(int id);
        void Load(string file);
        int Count();
        StormObject[] All();
        

    }
}
