using Bunifu.Framework.UI;
using HoTS_Service.Entity;
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
        private Func<double[], double> compute;
        private Func<int,int,double> matchupWith;
        private Func<int, int, double> matchupAgainst;

        public Func<List<int>> GetPlayersPick { get => getPlayersPick; set => getPlayersPick = value; }
        public Func<HeroList> GetHeroes { get => getHeroes; set => getHeroes = value; }
        public Func<double[], double> Compute { get => compute; set => compute = value; }
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

            fTeam = pics.GetRange(0, 5).ToArray();
            sTeam = pics.GetRange(5, 5).ToArray();

            double neuroOutput = 0, statOutput;

            if (fTeam.SequenceEqual(sTeam))
            {
                neuroOutput = 0.5;
                statOutput = 0.5;
            }
            else
            {

                double[] nnInput = new double[18];

                var heroes = GetHeroes();

                for (int i = 0; i < 5; i++)
                {
                    nnInput[((int)heroes.Item[pics[i]].Hero.SubGroup) - 1]++;
                }

                for (int i = 0; i < 5; i++)
                {
                    nnInput[((int)heroes.Item[pics[i + 5]].Hero.SubGroup) - 1 + 9]++;
                }

                neuroOutput = Compute(nnInput);

                double fSummator, sSummator;


                fSummator = fTeam.SelectMany(x => fTeam, (x, y) => new Tuple<int, int>(x, y)).
                    Sum(x => MatchupWith(x.Item1, x.Item2));

                sSummator = sTeam.SelectMany(x => sTeam, (x, y) => new Tuple<int, int>(x, y)).
                    Sum(x => MatchupAgainst(x.Item1, x.Item2));

                statOutput = fSummator / (fSummator + sSummator);
            }



            ShowResult(neuroOutput, statOutput);

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

        public void ShowResult(params double[] result)
        {
            Show();
            double net = result[0];
            double stat = result[1];
            data[0].Text = (int)(net * 100) + " %";
            data[1].Text = (int)(stat * 100) + " %";

            data[2].Text = (int)((net + stat)/2 * 100) + " %";

        }
        
    }
}
