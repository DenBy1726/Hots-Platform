using Bunifu.Framework.UI;
using DenByComponent;
using HoTS_Forecaster_form.Properties;
using MetroFramework;
using MetroFramework.Components;
using MetroFramework.Controls;
using MetroFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HoTS_Forecaster_form.View
{
    class StyleManager
    {
        public Dictionary<string, Color> ColorMapper = new Dictionary<string, Color>
        {
            { "Black" ,MetroColors.Black },
            { "Blue" ,MetroColors.Blue },
            { "Brown" ,MetroColors.Brown },
            { "Dark" ,Color.FromArgb(15,15,15) },
            { "Green" ,MetroColors.Green },
            { "Lime" ,MetroColors.Lime },
            { "Magenta" ,MetroColors.Magenta },
            { "Orange" ,MetroColors.Orange },
            { "Pink" ,MetroColors.Pink },
            { "Purple" ,MetroColors.Purple },
            { "Red" ,MetroColors.Red },
            { "Silver" ,MetroColors.Silver },
            { "Teal" ,MetroColors.Teal },
            { "White" ,MetroColors.White },
            { "Light" ,MetroColors.White },
            { "Yellow" ,MetroColors.Yellow },

        };

        List<Control> controls;
        List<Component> menuItems = new List<Component>();
        MetroContextMenu menu;
        MetroStyleManager manager;
        Form1 basewnd;

        Color theme, style,fore;

        public Color Theme { get => theme; }
        public Color Style { get => style;}
        public Color Fore { get => fore;}

        public StyleManager(List<Control> controls,MetroContextMenu menu,MetroStyleManager
            manager,Form1 baseWnd)
        {
            this.controls = controls;
            this.menu = menu;
            this.manager = manager;
            this.basewnd = baseWnd;

            //GetMenu(menu, menuItems);

            ChangeTheme(Settings.Default.Theme);
            ChangeStyle(Settings.Default.Style);
        }

        public void SetBackground(Image obj)
        {
            var control = controls.Find((x) => x.Name == "metroTabControl1") as TabControl;
            foreach (TabPage it in control.TabPages)
            {
                it.BackgroundImage = obj;
            }
        }

        public void TransparentChange(Color c)
        {
            var cont_Filter = controls.Find((x) => x.Name == "cont_Filter") as SplitContainer;
            var metroLabel47 = controls.Find((x) => x.Name == "metroLabel47");
            var panel3 = controls.Find((x) => x.Name == "panel3");
            var tableLayoutPanel4 = controls.Find((x) => x.Name == "tableLayoutPanel4");
            var panel4 = controls.Find((x) => x.Name == "panel4");
            var splitContainer2 = controls.Find((x) => x.Name == "splitContainer2") as SplitContainer;

            cont_Filter.BackColor = c;
            cont_Filter.Panel1.BackColor = c;
            cont_Filter.Panel2.BackColor = c;
            splitContainer2.BackColor = c;
            splitContainer2.Panel1.BackColor = c;
            splitContainer2.Panel2.BackColor = c;
            metroLabel47.BackColor = c;
            panel3.BackColor = c;
            panel4.BackColor = c;
            tableLayoutPanel4.BackColor = c;
            tableLayoutPanel4.BackColor = c;
        }

        public void ChangeTheme(string color)
        {
            if (ColorMapper.ContainsKey(color) == false)
                return;
            theme = ColorMapper[color];

            manager.Theme = (MetroThemeStyle)Enum.Parse(typeof(MetroThemeStyle),color);

            if (color == "Dark")
            {
                fore = ColorMapper["White"];
            }
            else
            {
                fore = ColorMapper["Dark"];
            }

            Settings.Default.Theme = manager.Theme.ToString();
            Settings.Default.Save();
            Render();
        }

        public void ChangeStyle(string color)
        {
            if (ColorMapper.ContainsKey(color) == false)
                return;
            style = ColorMapper[color];

            manager.Style = (MetroColorStyle)Enum.Parse(typeof(MetroColorStyle), color);

            Settings.Default.Style = manager.Style.ToString();
            Settings.Default.Save();
            Render();
        }

        public void Render()
        {
            manager.Theme = manager.Theme;
            manager.Style = manager.Style;
            basewnd.StyleManager = manager;
            menu.StyleManager = manager;
            basewnd.Refresh();
            menu.Refresh();

            foreach (var it in controls)
            {
                RenderControl(it);
            }
            RenderControl(menu);
        }

        void RenderControl(Control it)
        {

            if (it is IMetroControl)
            {
                var control = it as IMetroControl;
                control.Theme = manager.Theme;
                control.Style = manager.Style;

            }


            else if (it is LeftSideTabControl)
            {
                var control = it as LeftSideTabControl;

                control.BackColor = Theme;
                control.TabColor = Theme;
                control.PressedTextColor = Theme;

                control.PressedTabColor1 = Style;
                control.PressedTabColor2 = Style;

                control.TextColor = Fore;


            }

            else if(it is SplitContainer){

            }

            else if(it is SplitterPanel){

            }

            else if(it is Panel)
            {

            }

            else
            {
               // it.BackColor = Theme;
                it.ForeColor = Style;
            }

        }

        void RenderControl(List<ToolStripItem> it)
        {
            foreach (ToolStripItem control in it)
            {
                control.BackColor = Theme;
                control.ForeColor = Fore;
            }

        }

       /* void GetMenu(MetroContextMenu control, List<Component> controls)
        {
            foreach (ToolStripMenuItem it in control.Items)
            {
                controls.Add(it);
                foreach (ToolStripMenuItem it2 in it.DropDownItems)
                {
                    controls.Add(it2);
                }
            }
        }*/

    }
}
