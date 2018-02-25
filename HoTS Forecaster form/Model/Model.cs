using HoTS_Service.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using HoTS_Service.Entity;
using HoTS_Service.Entity.Enum;

namespace HoTS_Forecaster_form
{
    public class Model
    {
        private HeroService hero = new HeroService();

        /// <summary>
        /// сервис для работы с картами
        /// </summary>
        public MapService Map = new MapService();

        private HeroDetailsService detail = new HeroDetailsService();

        /// <summary>
        /// сервис для работы со статистикой
        /// </summary>
        public StatisticModule Statistic = new StatisticModule();

        public NeuralNetworkForecast ForecastService = new NeuralNetworkForecast();

        /// <summary>
        /// данные о героях
        /// </summary>
        public HeroList Hero { get => new HeroList(hero.All().ToList(),detail.All().ToList()
            ,Statistic.HeroStatistic.All().Item1.ToList());}

        public Model()
        {
            Validate("./Source/Hero/Hero.json");
            Validate("./Source/Hero/HeroDetails.json");
            Validate("./Source/Map/Map.json");

            try
            {
                hero.Load("./Source/Hero/Hero.json");
            }
            catch(Exception e)
            {
                ExceptionThrower.Throw(405,e, "Hero.json");
            }
            try
            {
                Map.Load("./Source/Map/Map.json");
            }
            catch (Exception e)
            {
                ExceptionThrower.Throw(405,e, "Map.json");
            }
            try
            {
                detail.Load("./Source/Hero/HeroDetails.json");
            }
            catch (Exception e)
            {
                ExceptionThrower.Throw(405,e, "HeroDetails.json");
            }
            
        }

        /// <summary>
        /// Проверка пути файла на корректность
        /// </summary>
        /// <param name="file"></param>
        private void Validate(string file)
        {
            //если файла нету, кинуть исключение
            if (File.Exists(file) == false)
                ExceptionThrower.Throw(404,null,file);
        }
 
    }
    
}
