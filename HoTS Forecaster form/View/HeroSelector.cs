using HoTS_Service.Entity;
using HoTS_Service.Entity.Enum;
using HoTS_Service.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HoTS_Forecaster_form.View
{
    public class HeroSelector : BaseHeroPicker
    {
        Func<HashSet<HeroGroup>> getGroupFilter;
        Func<HashSet<HeroSubGroup>> getSubGroupFilter;
        Func<HashSet<Franchise>> getFranchiseFilter;
        Func<string> getTextFilter;

        public Func<HashSet<HeroGroup>> GetGroupFilter { get => getGroupFilter; set => getGroupFilter = value; }
        public Func<HashSet<HeroSubGroup>> GetSubGroupFilter { get => getSubGroupFilter; set => getSubGroupFilter = value; }
        public Func<HashSet<Franchise>> GetFranchiseFilter { get => getFranchiseFilter; set => getFranchiseFilter = value; }
        public Func<string> GetTextFilter { get => getTextFilter; set => getTextFilter = value; }

        public HeroSelector(ListView control)
            :base(control)
        {
            this.BeforeRender += HeroSelector_BeforeRender;
        }

       
        private HeroList HeroSelector_BeforeRender(HeroList model)
        {
            return model.
                Select(GetGroupFilter()).
                Select(GetSubGroupFilter()).
                Select(GetFranchiseFilter()).
                Select(GetTextFilter());
        }
    }
}
