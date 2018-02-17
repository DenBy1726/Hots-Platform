using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZedGraph;

namespace HoTS_Forecaster_form.View.StatisticViewItem.StatisticViewPainter
{
    interface IPointPainter
    {
        event Action<LineItem,int ,double> OnDrawPoint;
        List<double> X { get ; set ; }
        List<double> Y { get ; set ; }
        void Draw(GraphPane pane, Color style);
        void DrawCenterLine(GraphPane pane, Color style,double center);
    }

}
