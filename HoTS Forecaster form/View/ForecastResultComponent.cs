using Bunifu.Framework.UI;
using HoTS_Service.Entity;
using HoTS_Service.Service;
using MetroFramework;
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
        List<BunifuCustomLabel> text;
        List<BunifuCustomLabel> data;
        BunifuFlatButton button;

        private Func<List<int>> getPlayersPick;
        private Func<HeroList> getHeroes;
        private List<NeuralNetworkForecast> compute;
        private Func<int,int,double> matchupWith;
        private Func<int, int, double> matchupAgainst;

        public Func<List<int>> GetPlayersPick { get => getPlayersPick; set => getPlayersPick = value; }
        public Func<HeroList> GetHeroes { get => getHeroes; set => getHeroes = value; }
        public List<NeuralNetworkForecast> Compute { get => compute; set => compute = value; }
        public Func<int, int, double> MatchupWith { get => matchupWith; set => matchupWith = value; }
        public Func<int, int, double> MatchupAgainst { get => matchupAgainst; set => matchupAgainst = value; }

        public ForecastResultComponent(List<BunifuCustomLabel> text,List<BunifuCustomLabel> data,
            BunifuFlatButton btn)
        {
            this.text = text;
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

            HeroData[] heroes = GetHeroes().ToArray();

            HeroData[] taken = heroes.Where(x => pics.Contains(x.Id)).ToArray();


            fTeam = pics.GetRange(0, 5).ToArray();
            sTeam = pics.GetRange(5, 5).ToArray();

            Dictionary<string, double> result = new Dictionary<string, double>();
            foreach (var func in Compute)
            {
                if (fTeam.SequenceEqual(sTeam))
                {
                    result.Add(func.Method, 0.5);
                }
                else
                {
                    result.Add(func.Method, func.Compute(taken));
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

            foreach (var it in text)
            {
                it.Visible = vis;
            }

            foreach (var it in data)
            {
                it.Visible = vis;
            }
        }

        public void ShowResult(Dictionary<string, double> result)
        {
            Show();
            data[0].Text = (int)(result["NNData"] * 100) + " %";
            data[1].Text = (int)(result["NNDataGauss"] * 100) + " %";

            data[2].Text = (int)((result["NNData"] + result["NNDataGauss"]) /2 * 100) + " %";

        }
        
    }
}
