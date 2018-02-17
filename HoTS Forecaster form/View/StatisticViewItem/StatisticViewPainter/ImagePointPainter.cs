using HoTS_Service.Entity;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZedGraph;

namespace HoTS_Forecaster_form.View.StatisticViewItem.StatisticViewPainter
{
    public class ImagePointPainter : IPointPainter
    {
        List<double> x;
        List<double> y;
        Func<int,Hero> hero;
        Func<string, Image> image;

        public ImagePointPainter(Func<int,Hero> hero, Func<string, Image> image)
        {
            this.hero = hero;
            this.image = image;
        }

        public List<double> X { get => x; set => x = value; }
        public List<double> Y { get => y; set => y = value; }
        public Func<int,Hero> Hero { get => hero; set => hero = value; }
        public Func<string, Image> Image { get => image; set => image = value; }

        public event Action<LineItem,int ,double> OnDrawPoint;

        public virtual void Draw(GraphPane pane,Color style)
        {
            // Создадим список точек
            PointPairList list = new PointPairList();

            for (int i = 0; i < y.Count; i++)
            {
                string heroName = Hero((int)x[i]).Name;
                list.Add(i + 1, y[i], heroName);
                var line = pane.AddCurve("Sinc" + i, list.Clone(), style, SymbolType.Circle);
                line.Symbol.Fill = new Fill(Image(heroName + ".png"),
                    System.Drawing.Drawing2D.WrapMode.TileFlipX);
                if (OnDrawPoint != null)
                    OnDrawPoint.Invoke(line,i+1,y[i]);
                line.Symbol.Size = 55 - y.Count/2;
                list.Clear();
            }
        }
        
        public virtual void DrawCenterLine(GraphPane pane, Color style,double center)
        {
            PointPairList list = new PointPairList();
            list.Add(pane.XAxis.Scale.Min, center);
            list.Add(pane.XAxis.Scale.Max, center);
            pane.AddCurve("middle", list, style, SymbolType.None);
        }
    }

   
}
