using HoTS_Forecaster_form.Properties;
using MetroFramework.Components;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace HoTS_Forecaster_form.View
{

    /// <summary>
    /// Базовый компонент для выбора героя. Предоставляет возможность
    /// выбирать героя и делать предобработку перед рендерингом через подписку на событие
    /// </summary>
    public class BaseHeroPicker
    {
        //управляемый компонент
        protected ListView list;

        //интерфейс получения изображения через делегат
        private Func<string,int> getImageId;

        private Func<HeroList> getHeroes;

        //тип отрисовки
        string renderMode = "";//_corner

        const string FORMAT = ".png";

        /// <summary>
        /// действия со списком героев перед отрисовкой
        /// </summary>
        public event Func<HeroList,HeroList> BeforeRender;
        /// <summary>
        /// возникает когда игрок выбрал героя
        /// </summary>
        public event Action<string> SelectionChanged;

        public string RenderMode { get => renderMode; set => renderMode = value; }
        public Func<string, int> GetImageId { get => getImageId; set => getImageId = value; }
        public Func<HeroList> GetHeroes { get => getHeroes; set => getHeroes = value; }

        ListViewItem lastHover = null;

        public BaseHeroPicker(ListView control)
        {
            this.list = control;
            this.list.MouseMove += List_MouseMove;
            this.list.MouseLeave += List_MouseLeave;
            this.list.SelectedIndexChanged += List_SelectedIndexChanged;

            this.RenderMode = Settings.Default.RenderType;
            ItemSizeChange(Settings.Default.IconSize);
        }

        public void ItemSizeChange(int value)
        {
            int tileValue = value + (value / 7);
            list.TileSize = new Size(tileValue, tileValue);
        }
        
        public void ChangeBackGround(Image img)
        {
            list.BackgroundImage = img;
        }

        private void List_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SelectionChanged != null)
            {
                if(this.list.SelectedItems.Count > 0)
                    SelectionChanged.Invoke(this.list.SelectedItems[0].Text);
            }
               
        }

        private void List_MouseLeave(object sender, EventArgs e)
        {
            if(lastHover != null)
            {
                lastHover.ImageIndex = GetImageId(lastHover.Text + RenderMode + FORMAT);
            }
            lastHover = null;
        }

        private void List_MouseMove(object sender, MouseEventArgs e)
        {
            var elem = this.list.GetItemAt(e.X, e.Y);
            if (elem != null)
            {
                if (lastHover != null)
                {
                    lastHover.ImageIndex = GetImageId(lastHover.Text + RenderMode + FORMAT);
                }
                    elem.ImageIndex = GetImageId(elem.Text + RenderMode + "_h" + FORMAT);
                lastHover = elem;
            }
        }

        /// <summary>
        /// отрисовка списка
        /// </summary>
        public void Render()
        {
            var heroes = GetHeroes();
            if (BeforeRender != null)
                heroes = BeforeRender.Invoke(heroes);

            list.Items.Clear();
            foreach (var it in heroes)
            {
                var lvItem = new ListViewItem(it.Hero.Name)
                {
                    Tag = it.Hero.Id
                };

                lvItem.ImageIndex = GetImageId(it.Hero.Name + renderMode + FORMAT);
                list.Items.Add(lvItem);
            }
        }

        public void RenderModeChange(string mode)
        {
            this.renderMode = mode;
            Render();
        }
    }
}