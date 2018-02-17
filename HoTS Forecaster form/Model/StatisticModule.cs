using HoTS_Service.Service;
using System.Linq;
using System.IO;
using System;

namespace HoTS_Forecaster_form
{
    public class StatisticModule
    {
        public StatisticService Statistic = new StatisticService();
        public HeroStatisticService HeroStatistic = new HeroStatisticService();
        public MatchupService MatchUp = new MatchupService();
        public StatisticModule()
        {
            Validate("./Data/Statistic.json");
            Validate("./Data/Statistic_sho.json");
            Validate("./Data/MatchupTable.json");
            try
            {
                Statistic.Load("./Data/Statistic.json");
            }
            catch (Exception e)
            {
                ExceptionThrower.Throw(405,e, "Statistic.json");
            }
            try
            {
                HeroStatistic.Load("./Data/Statistic_sho.json");
            }
            catch (Exception e)
            {
                ExceptionThrower.Throw(405, e,"Statistic_sho.json");
            }
            try
            {
                MatchUp.Load("./Data/MatchupTable.json");
            }
            catch (Exception e)
            {
                ExceptionThrower.Throw(405, e, "MatchupTable.json");
            }
        }

        /// <summary>
        /// Вовзращает массив, где i соответствует количество игр на i карте
        /// </summary>
        /// <returns></returns>
        public int[] MapPopularity()
        {
            return Statistic.Select((x) => x.Statictic.Ammount).ToArray();
        }


        private void Validate(string file)
        {
            if (File.Exists(file) == false)
                ExceptionThrower.Throw(404, null,file);
        }
    }


}

    

