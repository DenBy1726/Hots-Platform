using HoTS_Forecaster_form.View.StatisticViewItem.StatisticViewPainter;
using HoTS_Service.Entity;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZedGraph;

namespace HoTS_Forecaster_form.View.StatisticViewItem
{
    public class SDPSStat : AbstractStatItem<HeroStatisticItemAvg[]>
    {
        private Func<string,int> heroGameCount;

        public const string name = "Количество нанесенного урона";
        public override string Name => name;

        public SDPSStat(Func<int,Hero> hero,Func<string,Image> image, 
            Func<string,int> heroGameCount)
        {
            this.heroGameCount = heroGameCount;
            this.Painter = new ImagePointPainter(hero, image);
            
        }

        public override void ComputePoints(HeroStatisticItemAvg[] points)
        {
            var val = points.Select((x, i) => new Tuple<double, int>(x.sdps*x.sec, x.heroId)).ToList();
            val.Sort();

            this.Y = val.Select((x) => x.Item1).ToList();
            this.X = val.Select((x) => (double)x.Item2).ToList();
        }

        public override void Draw(ZedGraphControl graph, HeroStatisticItemAvg[] points, Color c)
        {
            base.Draw(graph, points, c);
            graph.GraphPane.Title.Text = "График количества нанесенного урона фортам и юнитам проивника в секунду";
            graph.GraphPane.YAxis.Title.Text = "Количество нанесенного урона";

            graph.AxisChange();
            graph.Invalidate();

            // Обновляем график
            if (lastEvent != null)
                graph.PointValueEvent -= lastEvent;
            lastEvent = ZedGraphControl1_PointValueEvent1;
            graph.PointValueEvent += lastEvent;

        }

        private string ZedGraphControl1_PointValueEvent1(ZedGraphControl sender, GraphPane pane, CurveItem curve, int iPt)
        {
            PointPair point = curve[iPt];

            // Сформируем строку
            string result = string.Format("Герой: {0:F3} \nКоличества нанесенного урона: {1:F3} \nКоличетсво игр: {2} ",
                point.Tag, point.Y, heroGameCount(point.Tag as string));

            return result;
        }
    }
}
