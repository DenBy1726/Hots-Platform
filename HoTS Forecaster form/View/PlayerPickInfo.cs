using Bunifu.Framework.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HoTS_Forecaster_form.View
{
    class PlayerPickInfo
    {
        List<BunifuFlatButton> buttons;

        //интерфейс получения изображения через делегат
        private Func<string, Image> getImageId;
        private Func<string, int> getHeroId;

        public Func<string, Image> GetImage { get => getImageId; set => getImageId = value; }
        public Func<string, int> GetHeroId { get => getHeroId; set => getHeroId = value; }

        public event Action<EventPickChangedArgs> PickChanged;

        public class EventPickChangedArgs : EventArgs
        {
            public int heroId;
            public int playerId;
            public string heroName;
            public Image heroIcon;
        }

        public PlayerPickInfo(List<BunifuFlatButton> buttons)
        {
            this.buttons = buttons;

            SetStyle();
        }

        public void SetStyle()
        {
            foreach(var it in buttons)
            {
                it.Activecolor = Color.Transparent;
                it.BackColor = Color.Transparent;
                it.ButtonText = "     Герой не выбран";
                it.Iconcolor = Color.Transparent;
                it.Normalcolor = Color.Transparent;
                it.OnHovercolor = Color.Transparent;
                it.OnHoverTextColor = Color.Purple;
                it.Iconimage = null;
            }
        }

        public void SetPick(string heroName, int playerId)
        {
            if (heroName == "" || playerId == -1)
                return;

            int heroId = getHeroId(heroName);

            if (heroId == -1)
                return;

            var control = buttons[playerId];
            control.ButtonText = $"     {heroName}";
            control.Iconimage = GetImage($"{heroName}_corner.png");
            control.Tag = heroId;

            if (PickChanged != null)
                PickChanged.Invoke(
                    new EventPickChangedArgs()
                    {
                        heroId = heroId,
                        playerId = playerId,
                        heroName = heroName,
                        heroIcon = control.Iconimage
                    });
        }

        public List<int> GetPlayerPics()
        {
            List<int> picks = new List<int>();
            foreach(var but in buttons)
            {
                int tag;
                try
                {
                    tag = (int)but.Tag;

                }
                catch
                {
                    picks.Add(-1);
                    continue;
                }
                picks.Add(tag);
            }
            return picks;
        }
    }
}
