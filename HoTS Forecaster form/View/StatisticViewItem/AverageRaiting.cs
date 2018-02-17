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
    public class AverageRaiting : AbstractStatItem<HeroStatisticItemAvg[]>
    {
        private Func<string,int> heroGameCount;
        private HeroStatisticItemAvg[] points;

        public const string name = "Средний рейтинг";
        public override string Name => name;

        public AverageRaiting(Func<int,Hero> hero,Func<string,Image> image, 
            Func<string,int> heroGameCount)
        {
            this.heroGameCount = heroGameCount;
            this.Painter = new ImagePointPainter(hero, image);
            
        }

        public override void ComputePoints(HeroStatisticItemAvg[] points)
        {
            this.points = points;
            var val = points.Select((x, i) => new Tuple<double, int>
            ((x.specialistRating + x.assassinRating + x.warriorRating + x.supportRating)/4, x.heroId)).ToList();
            val.Sort();

            this.Y = val.Select((x) => x.Item1).ToList();
            this.X = val.Select((x) => (double)x.Item2).ToList();
        }

        public override void Draw(ZedGraphControl graph, HeroStatisticItemAvg[] points, Color c)
        {
            base.Draw(graph, points, c);
            graph.GraphPane.Title.Text = "Средний рейтинг";
            graph.GraphPane.YAxis.Title.Text = "Средний рейтинг";

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
            var item = points[(int)this.X[(int)point.X-1]];
            // Сформируем строку
            string result = string.Format("Герой: {0:F3} \nКоличетсво игр: {1}" +
                "\nСредний рейтинг: {2:F3}" +
                "\nРейтинг убийцы: {3:F3}" +
                "\nРейтинг бойца: {4:F3}" +
                "\nРейтинг поддержки: {5:F3}" +
                "\nРейтинг специалиста: {6:F3}",
                point.Tag, heroGameCount(point.Tag as string),point.Y,item.assassinRating,
                item.warriorRating,item.supportRating,item.specialistRating);
            return result;
        }
    }
}
