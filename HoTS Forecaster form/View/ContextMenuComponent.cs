using HoTS_Service.Entity;
using MetroFramework.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static HoTS_Forecaster_form.View.PlayerPickInfo;

namespace HoTS_Forecaster_form.View
{
    //контекстное меню. Служит для выбора к какому игроку отнести героя после
    //выбора героя в HeroPicker
    class ContextMenuComponent
    {
        MetroContextMenu menu;
        public event Action<string, int> OnPlayerPicked;
        string hero = "";

        //HeroList heroes;
        Func<string,HeroStatisticItemAvg> getStatistic;

        public Func<string, HeroStatisticItemAvg> GetStatistic { get => getStatistic;
            set => getStatistic = value; }

        public ContextMenuComponent(MetroContextMenu menu)
        {
            this.menu = menu;

            //подписка меню на событие клика
            foreach (object it in this.menu.Items)
            {
                if (it is ToolStripMenuItem)
                {
                    var control = it as ToolStripMenuItem;
                    control.DropDownItemClicked += Menu_ItemClicked;
                }
            }
        }

        private void Menu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            //запоминаем кликнутого игрока, и кидаем событие
            string clicked = e.ClickedItem.Tag as string;
            if (clicked == null)
                return;
            int item = Int32.Parse(clicked);

            if (OnPlayerPicked != null)
                OnPlayerPicked.Invoke(hero, item);
        }

        public void Show()
        {
            var heroes = GetStatistic(hero);
            if (heroes != null)
            {

                if (menu.Items["nameMenu"] is ToolStripMenuItem namebar)
                {
                    namebar.Text = hero;
                }

                if (menu.Items["indicesMenu"] is ToolStripComboBox control)
                {
                    control.Items.Clear();
                    control.Items.
                        AddRange(new object[]{
                    $"Индекс убийцы: {Math.Round(heroes.assassinRating,2)}" ,
                         $"Индейкс воина: {Math.Round(heroes.warriorRating,2)}" ,
                         $"Индекс поддежки: {Math.Round(heroes.supportRating,2)}" ,
                         $"Индекс специалиста: {Math.Round(heroes.specialistRating,2)}"
                    });
                }

            }
            menu.Show(Cursor.Position);
        }

        public void ChangeElement(EventPickChangedArgs e)
        {
            ToolStripMenuItem container;
            if (e.playerId < 5)
            {
                container = menu.Items[1] as ToolStripMenuItem;
            }
            else
            {
                container = menu.Items[2] as ToolStripMenuItem;
            }

            container.DropDownItems[e.playerId % 5].Text = e.heroName;
            container.DropDownItems[e.playerId % 5].Image = e.heroIcon;
        }

        /// <summary>
        /// Обработка выбора героя
        /// </summary>
        /// <param name="id">ид героя</param>
        public void SelectPlayer(string id)
        {
            hero = id;
            Show();
        }
    }
}
