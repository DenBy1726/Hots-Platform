using Bunifu.Framework.UI;
using HoTS_Service.Entity;
using HoTS_Service.Service;
using MetroFramework;
using MetroFramework.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HoTS_Forecaster_form.View
{
    class ForecastResultComponent
    {
        MetroListView data;
        BunifuFlatButton button;

        private Func<List<int>> getPlayersPick;
        private Func<HeroList> getHeroes;
        private List<NeuralNetworkForecast> compute;
        private Func<int, int, double> matchupWith;
        private Func<int, int, double> matchupAgainst;

        public Func<List<int>> GetPlayersPick { get => getPlayersPick; set => getPlayersPick = value; }
        public Func<HeroList> GetHeroes { get => getHeroes; set => getHeroes = value; }
        public List<NeuralNetworkForecast> Compute { get => compute; set => compute = value; }
        public Func<int, int, double> MatchupWith { get => matchupWith; set => matchupWith = value; }
        public Func<int, int, double> MatchupAgainst { get => matchupAgainst; set => matchupAgainst = value; }

        public ForecastResultComponent(MetroListView data,
            BunifuFlatButton btn)
        {
            this.data = data;
            this.button = btn;

            this.button.Click += Button_Click;
            Hide();
        }

        private void Button_Click(object sender, EventArgs e)
        {
            var pics = GetPlayersPick();
            int noPickIndex = pics.IndexOf(-1);
            if (noPickIndex != -1)
            {
                MetroMessageBox.Show(Application.OpenForms[0], $"{(noPickIndex % 5) + 1 } Игрок " +
                    $"{(noPickIndex / 5) + 1 } Команды не выбрал героя");
                return;
            }

            int[] fTeam, sTeam;

            fTeam = pics.GetRange(0, 5).ToArray();
            sTeam = pics.GetRange(5, 5).ToArray();

            HeroData[] heroes = GetHeroes().ToArray();

            HeroData[] taken = heroes.Where(x => fTeam.Contains(x.Id))
                .Concat(heroes.Where(x => sTeam.Contains(x.Id))).ToArray();

            if (taken.Length != 10)
            {
                MetroMessageBox.Show(Application.OpenForms[0], "Герои должны быть уникальны в рамках одной команды");
                return;
            }

            Dictionary<string, double> result = new Dictionary<string, double>();
            foreach (var func in Compute)
            {
                if (fTeam.SequenceEqual(sTeam))
                {
                    result.Add(func.Meta.Alias, 0.5);
                }
                else
                {
                    result.Add(func.Meta.Alias, func.Compute(taken));
                }
            }
            ShowResult(result);
        }

        public void Hide()
        {
            SetVisibility(false);
        }

        public void Show()
        {
            SetVisibility(true);
        }

        public void SetVisibility(bool vis)
        {
            data.Visible = vis;
        }

        public void ShowResult(Dictionary<string, double> result)
        {
            Show();
            data.Items.Clear();
            foreach (var it in result)
            {
                var lv = new ListViewItem(new string[] { it.Key,
                    (Math.Round(it.Value,2)*100).ToString() + " %" },
                    -1, System.Drawing.Color.Empty, System.Drawing.Color.Empty,
                    new System.Drawing.Font("Segoe UI", 25F));
                data.Items.Add(lv);
            }
            data.Refresh();
        }

    }
}
