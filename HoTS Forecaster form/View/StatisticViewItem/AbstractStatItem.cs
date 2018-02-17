using HoTS_Forecaster_form.View.StatisticViewItem.StatisticViewPainter;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZedGraph;
using static ZedGraph.ZedGraphControl;

namespace HoTS_Forecaster_form.View.StatisticViewItem
{
    public abstract class AbstractStatItem<T>
    {
        IPointPainter painter;
        List<double> x;
        List<double> y;
        GraphPane pane;
        protected PointValueHandler lastEvent;

        public abstract String Name
        {
            get;
        }


        public List<double> X { get => x; set => x = value; }
        public List<double> Y { get => y; set => y = value; }
        public GraphPane Pane { get => pane; set => pane = value; }
        internal IPointPainter Painter { get => painter; set => painter = value; }

        public abstract void ComputePoints(T points);

        public void SetXInterval()
        {
            if (x.Count == 0)
                return;
            // !!!
            // Устанавливаем интересующий нас интервал по оси X
            Pane.XAxis.Scale.Min = 0;
            Pane.XAxis.Scale.Max = x.Count + 2;
            Pane.XAxis.IsVisible = false;
        }

        public void SetYInterval()
        {
            if (y.Count == 0)
                return;
            // !!!
            // Устанавливаем интересующий нас интервал по оси X
            Pane.YAxis.Scale.Min = y.Min() - y.Min() * 0.3;
            if (Pane.YAxis.Scale.Min == 0)
                Pane.YAxis.Scale.Min = -y.Max()*0.1;
            Pane.YAxis.Scale.Max = y.Max() + y.Max() * 0.3;
            Pane.YAxis.IsVisible = true;
        }


        public virtual void Draw(ZedGraphControl graph,T data,Color c)
        {
            this.pane = graph.GraphPane;

            pane.CurveList.Clear();
            
            ComputePoints(data);

            this.Painter.X = X;
            this.Painter.Y = Y;

            Painter.Draw(pane, c);
            SetXInterval();
            SetYInterval();
        }

        public void Draw(ZedGraphControl graph, T data, Color c, Color mid,double center)
        {
            this.pane = graph.GraphPane;

            pane.CurveList.Clear();

            ComputePoints(data);

            this.Painter.X = X;
            this.Painter.Y = Y;

            Painter.Draw(pane, c);
            SetXInterval();
            SetYInterval();
            Painter.DrawCenterLine(pane, mid, center);
        }

    }
}
