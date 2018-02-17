using HoTS_Forecaster_form.Properties;
using HoTS_Forecaster_form.View.StatisticViewItem;
using HoTS_Service.Entity;
using MetroFramework.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZedGraph;

namespace HoTS_Forecaster_form.View
{
    class StatGraph
    {
        MetroComboBox heroesCombo;
        public Action<string> HeroesSelectionChanged;
        private Func<List<AbstractStatItem<HeroStatisticItemAvg[]>>> getSupportHeroStatistic;

        public Func<List<AbstractStatItem<HeroStatisticItemAvg[]>>> GetSupportHeroStatistic { get => getSupportHeroStatistic; set => getSupportHeroStatistic = value; }

        public StatGraph(MetroComboBox metroComboBox4)
        {
            this.heroesCombo = metroComboBox4;
            heroesCombo.Sorted = true;
            this.heroesCombo.SelectedValueChanged += metroComboBox4_SelectedIndexChanged;
        }

        public void Init()
        {
            var heroStatistic = GetSupportHeroStatistic();
            foreach(var it in  heroStatistic)
            {
                heroesCombo.Items.Add(it.Name);
            }
            heroesCombo.SelectedIndex = 0;
            
        }

        private void metroComboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            string text = heroesCombo.SelectedItem as string;
            if (HeroesSelectionChanged != null)
                HeroesSelectionChanged.Invoke(text);
        }

    }
}
