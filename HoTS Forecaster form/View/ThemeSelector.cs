using HoTS_Forecaster_form.Properties;
using MetroFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HoTS_Forecaster_form.View
{
    class ThemeSelector
    {
        private ComboBox theme;

        private ComboBox style;

        public event Action<string> OnThemeChanged;
        public event Action<string> OnStyleChanged;

        public ThemeSelector(ComboBox theme,ComboBox style)
        {
            this.theme = theme;
            this.style = style;

            var themes = Enum.GetNames(typeof(MetroThemeStyle)).ToList();
            themes.RemoveAt(0);
            var styles = Enum.GetNames(typeof(MetroColorStyle)).ToList();
            styles.RemoveAt(0);

            foreach (var it in themes)
            {
                theme.Items.Add(it);
            }

            theme.SelectedItem = Settings.Default.Theme;

            foreach (var it in styles)
            {
                style.Items.Add(it);
            }

            style.SelectedItem = Settings.Default.Style;

            theme.SelectedIndexChanged += Theme_SelectedIndexChanged;
            style.SelectedIndexChanged += Style_SelectedIndexChanged;
        }

        private void Style_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (OnStyleChanged != null)
            {
                OnStyleChanged.Invoke(style.SelectedItem as string);
            }
        }

        private void Theme_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (OnThemeChanged != null)
            {
                OnThemeChanged.Invoke(theme.SelectedItem as string);
            }
        }

       
    }
}
