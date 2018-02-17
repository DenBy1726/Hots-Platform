using HoTS_Forecaster_form.Properties;
using HoTS_Forecaster_form.View.StatisticViewItem;
using HoTS_Service.Entity;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ZedGraph;
using static ZedGraph.ZedGraphControl;

namespace HoTS_Forecaster_form.View
{
    class StatComponent
    {
        ZedGraphControl graph;

        private Func<HeroStatisticItemAvg[]> heroesAvgStatistic;
        private Func<int, Hero> hero;
        private Func<string, int> heroGameCount;
        private Func<string, Image> image;
        private Func<Color> style;
        private Func<List<int>> filter;
        private Func<HeroList> getHeroes;
        int id = -1;

        public Func<HeroStatisticItemAvg[]> HeroesAvgStatistic { get => heroesAvgStatistic; set => heroesAvgStatistic = value; }
        public Func<int, Hero> Hero { get => hero; set => hero = value; }
        public Func<string, Image> Image { get => image; set => image = value; }
        public Func<string, int> HeroGameCount { get => heroGameCount; set => heroGameCount = value; }
        public Func<Color> Style { get => style; set => style = value; }
        public Func<List<int>> Filter { get => filter; set => filter = value; }
        public Func<HeroList> GetHeroes { get => getHeroes; set => getHeroes = value; }
        public List<AbstractStatItem<HeroStatisticItemAvg[]>> HeroStat { get => heroStat; set => heroStat = value; }

        List<AbstractStatItem<HeroStatisticItemAvg[]>> heroStat;


        public StatComponent(ZedGraphControl graph)
        {
            this.graph = graph;
           
            CustomizeGraph();
        }

        public void Init()
        {
            HeroStat = new List<AbstractStatItem<HeroStatisticItemAvg[]>>()
            {
                new AssasinRaitingStat(Hero, Image, HeroGameCount),
                new AssistPerSecStat(Hero, Image, HeroGameCount),
                new AverageRaiting(Hero, Image, HeroGameCount),
                new DeathStat(Hero, Image, HeroGameCount),
                new DPSStat(Hero, Image, HeroGameCount),
                new DTPSStat(Hero, Image, HeroGameCount),
                new EXPStat(Hero, Image, HeroGameCount),
                new FrequencyStat(Hero, Image, HeroGameCount),
                new HPSStat(Hero, Image, HeroGameCount),
                new KillPerSecStat(Hero, Image, HeroGameCount),
                new SDPSStat(Hero, Image, HeroGameCount),
                new SpecialistRaiting(Hero, Image, HeroGameCount),
                new SupportRaitingStat(Hero, Image, HeroGameCount),
                new WarriorRaitingStat(Hero, Image, HeroGameCount),
                new WinrateStat(Hero, Image, HeroGameCount)
            };

        }

        private void CustomizeGraph()
        {

            System.Drawing.Image image = System.Drawing.Image.FromFile(Settings.Default.background);
            graph.GraphPane.Chart.Fill = new Fill(Color.Transparent);
            graph.GraphPane.Fill = new Fill(image, System.Drawing.Drawing2D.WrapMode.TileFlipXY);

            graph.GraphPane.XAxis.Title.FontSpec.FontColor = Color.White;
            graph.GraphPane.XAxis.Color = Color.White;
            graph.GraphPane.XAxis.Scale.FontSpec.FontColor = Color.White;

            graph.GraphPane.YAxis.Title.FontSpec.FontColor = Color.White;
            graph.GraphPane.YAxis.Color = Color.White;
            graph.GraphPane.YAxis.Scale.FontSpec.FontColor = Color.White;

            graph.GraphPane.Title.FontSpec.FontColor = Color.White;

            graph.GraphPane.Legend.IsVisible = false;

            graph.IsShowCursorValues = false;
            graph.IsShowPointValues = true;

            graph.GraphPane.YAxis.MajorGrid.IsZeroLine = false;
        }

        public HeroStatisticItemAvg[] GetHeroStatistic()
        {
            var filter = new HashSet<int>(Filter());
            var stat = heroesAvgStatistic();

            return stat.Where((x, i) => filter.Contains(i)).ToArray();
        }

        public void ComputeHeroesStat(string text)
        {
            if (text != null)
                this.id = HeroStat.FindIndex(x=>x.Name == text);
            ComputeHeroesStat();
        }

        public void ComputeHeroesStat()
        {
            HeroStat[id].Draw(graph, GetHeroStatistic(), Style());
        }       

    }

   
}

