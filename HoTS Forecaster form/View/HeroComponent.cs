using HoTS_Service.Entity;
using HoTS_Service.Entity.Enum;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HoTS_Forecaster_form.View
{
    class HeroComponent
    {
        private Func<string,HeroData> getHero;
        //интерфейс получения изображения через делегат
        private Func<string, Image> getImage;
        private Func<string, Image> getLargeImage;

        public Func<string, HeroData> GetHero { get => getHero; set => getHero = value; }
        public Func<string, Image> GetImage { get => getImage; set => getImage = value; }
        public Func<string, Image> GetLargeImage { get => getLargeImage; set => getLargeImage = value; }

        List<Control> ctrl;

        public HeroComponent(List<Control> ctrl)
        {
            this.ctrl = ctrl;
        }

        

        public void Render(string heroName)
        {
            HeroData cur = getHero(heroName);

            ctrl.Find((x) => x.Name == "lbl_Name").Text = cur.Hero.Name + ", " +
                cur.Details.Title;

            ctrl.Find((x) => x.Name == "lbl_Info").Text = cur.Details.Info;

            ctrl.Find((x) => x.Name == "pb_Group").BackgroundImage 
                = GetImage(cur.Hero.Group.ToString() + ".png");

            if (cur.Hero.SubGroup == HeroSubGroup.Support)
                ctrl.Find((x) => x.Name == "pb_Subgroup").BackgroundImage 
                    = GetImage(cur.Hero.SubGroup.ToString() + "1.png");
            else
                ctrl.Find((x) => x.Name == "pb_Subgroup").BackgroundImage 
                    = GetImage(cur.Hero.SubGroup.ToString() + ".png");
            ctrl.Find((x) => x.Name == "pb_Avatar").BackgroundImage 
                = GetLargeImage(cur.Hero.Name + ".png");

            double time = cur.Statistic.sec;

            ctrl.Find((x) => x.Name == "lbl_Winpercent").Text =
                Math.Round((double)cur.Statistic.winrate, 3)*100 + " %";

            ctrl.Find((x) => x.Name == "lbl_Kill").Text =
               Math.Round((double)cur.Statistic.killPerSec * time, 2) + "";

            ctrl.Find((x) => x.Name == "lbl_Assist").Text =
               Math.Round((double)cur.Statistic.assistPerSec * time, 2) + "";

            ctrl.Find((x) => x.Name == "lbl_Death").Text =
               Math.Round((double)cur.Statistic.deathPerSec * time, 2) + "";

            ctrl.Find((x) => x.Name == "lbl_Damage").Text =
              Math.Round((double)cur.Statistic.dps * time, 2) + "";

            ctrl.Find((x) => x.Name == "lbl_Damage").Text =
             Math.Round((double)cur.Statistic.dps * time, 2) + "";

            ctrl.Find((x) => x.Name == "lbl_Heal").Text =
             Math.Round((double)cur.Statistic.hps * time, 2) + "";

            ctrl.Find((x) => x.Name == "lbl_Fort_damage").Text =
             Math.Round((double)cur.Statistic.sdps * time, 2) + "";

            ctrl.Find((x) => x.Name == "lbl_Taken_damage").Text =
             Math.Round((double)cur.Statistic.damageTakenPerSec * time, 2) + "";

            ctrl.Find((x) => x.Name == "lbl_Exp").Text =
             Math.Round((double)cur.Statistic.expPerSec * time, 2) + "";

            ctrl.Find((x) => x.Name == "lbl_Camps").Text =
             Math.Round((double)cur.Statistic.campTakenPerSec * time, 2) + "";

            ctrl.Find((x) => x.Name == "lbl_During").Text =
             new DateTime((long)time * 10000000).TimeOfDay.ToString().Split('.')[0];

            ctrl.Find((x) => x.Name == "lbl_Dps").Text =
             Math.Round((double)cur.Statistic.dps, 2) + "";

            ctrl.Find((x) => x.Name == "lbl_Hps").Text =
             Math.Round((double)cur.Statistic.hps, 2) + "";

            ctrl.Find((x) => x.Name == "lbl_Sdps").Text =
             Math.Round((double)cur.Statistic.sdps, 2) + "";

            ctrl.Find((x) => x.Name == "lbl_tps").Text =
             Math.Round((double)cur.Statistic.damageTakenPerSec, 2) + "";

            ctrl.Find((x) => x.Name == "lbl_exps").Text =
             Math.Round((double)cur.Statistic.expPerSec, 2) + "";

            ctrl.Find((x) => x.Name == "lbl_ik").Text =
             Math.Round((double)cur.Statistic.assassinRating, 2) + "";

            ctrl.Find((x) => x.Name == "lbl_iw").Text =
             Math.Round((double)cur.Statistic.warriorRating, 2) + "";

            ctrl.Find((x) => x.Name == "lbl_isup").Text =
             Math.Round((double)cur.Statistic.supportRating, 2) + "";

            ctrl.Find((x) => x.Name == "lbl_ispec").Text =
            Math.Round((double)cur.Statistic.specialistRating, 2) + "";

            ctrl.Find((x) => x.Name == "lbl_added").Text = cur.Details.Date.ToShortDateString();

            ctrl.Find((x) => x.Name == "lbl_cost").Text = cur.Details.Price.ToString();

            ctrl.Find((x) => x.Name == "lbl_title").Text = cur.Details.Title;

            ctrl.Find((x) => x.Name == "lbl_role").Text = cur.Hero.Group.ToString();

            ctrl.Find((x) => x.Name == "lbl_franchise").Text = cur.Details.Franchise.ToString();

            ctrl.Find((x) => x.Name == "lbl_difficult").Text = cur.Details.Difficulty.ToString();

            ctrl.Find((x) => x.Name == "lbl_range").Text = cur.Details.AttackRange.ToString();

            ctrl.Find((x) => x.Name == "tb_subscription").Text = cur.Details.Info;

            ctrl.Find((x) => x.Name == "tb_lore").Text = cur.Details.Lore;

            ctrl.Find((x) => x.Name == "lbl_hp").Text = cur.Details.Health.ToString();

            ctrl.Find((x) => x.Name == "lbl_regen").Text = cur.Details.HealthRegen.ToString();

            ctrl.Find((x) => x.Name == "lbl_resource").Text = cur.Details.ResourceType.ToString();

            ctrl.Find((x) => x.Name == "lbl_resource_n").Text = cur.Details.Resource.ToString();

            ctrl.Find((x) => x.Name == "lbl_defend").Text = "S = " + cur.Details.SpellArmor + " P = " + cur.Details.PhysicalArmor;

            ctrl.Find((x) => x.Name == "lbl_attack_speed").Text = cur.Details.AttackSpeed.ToString();

            ctrl.Find((x) => x.Name == "lbl_attack_range").Text = cur.Details.AttackRange.ToString();

            ctrl.Find((x) => x.Name == "lbl_attack").Text = cur.Details.AttackDamage.ToString();


        }
    }
}
