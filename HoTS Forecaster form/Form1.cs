using Bunifu.Framework.UI;
using DenByComponent;
using HoTS_Forecaster_form.Properties;
using HoTS_Forecaster_form.View;
using MetroFramework;
using MetroFramework.Controls;
using MetroFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;
using static MetroFramework.Controls.MetroTextBox;


namespace HoTS_Forecaster_form
{
    public partial class Form1 : MetroFramework.Forms.MetroForm
    {
        private Model Model;

        private HeroPicker heroPicker;//
        private HeroFilter heroFilter;//
        private HeroSelector heroSelector;//
        private ImageResource imageResource;
        private ImageResource largeImageResource;
        private FilterTab filterTab;//
        private ContextMenuComponent contextMenu;//
        private PlayerPickInfo playerPick;//
        private ForecastResultComponent forecastResultComponent;//
        private StyleManager styleManager;
        private ThemeSelector themeSelector;
        private HeroComponent heroComponent;//
        private RenderSettings renderSettings;
        private StatGraph statGraph;
        private StatComponent statComponent;

        private HeroFilter heroStatFilter;
        private FilterTab statfilterTab;

        private List<MetroCheckBox> groupCheckBox;
        private List<MetroCheckBox> franchiseCheckBox;
        private List<MetroCheckBox> subgroupCheckBox;

        private List<MetroCheckBox> groupStatCheckBox;
        private List<MetroCheckBox> franchiseStatCheckBox;
        private List<MetroCheckBox> subgroupStatCheckBox;

        private List<BunifuFlatButton> playerpickButton;
        private List<BunifuCustomLabel> textLabel;
        private List<BunifuCustomLabel> resultLabel;
        private List<Control> controls = new List<Control>();
        private List<ToolStripItem> menu = new List<ToolStripItem>();
        




        public Form1()
        {
            Model = new Model();
            InitializeComponent();


            Groups();

            imageResource = new ImageResource(this.imageList1)
            {
                Resource = "Source/Icons/"
            };
            imageResource.ImageSizeChange(Settings.Default.IconSize);
            largeImageResource = new ImageResource(this.imageList2)
            {
                Resource = "Source/Images/"
            };
            largeImageResource.Load(largeImageResource.Resource);

            contextMenu = new ContextMenuComponent(metroContextMenu1)
            {
                GetStatistic = (x) => Model.Hero.Statistic(x)
            };

            playerPick = new PlayerPickInfo(playerpickButton)
            {
                GetImage = imageResource.Bitmap,
                GetHeroId = (x) => Model.Hero.Select(x).Item[0].Id
            };

            forecastResultComponent = new ForecastResultComponent(textLabel, resultLabel, btn_forecast)
            {
                Compute = Model.ForecastService.Compute,
                GetHeroes = () => Model.Hero,
                GetPlayersPick = playerPick.GetPlayerPics,
                MatchupWith = Model.Statistic.MatchUp.With,
                MatchupAgainst = Model.Statistic.MatchUp.Against
            };

            filterTab = new FilterTab(cont_Filter, btn_FilterVisibleChange);
            filterTab.Collapse();

            statfilterTab = new FilterTab(splitContainer2, bunifuTileButton1);
            statfilterTab.Collapse();

            heroFilter = new HeroFilter(groupCheckBox, subgroupCheckBox, franchiseCheckBox, metroTextBox9);
            heroStatFilter = new HeroFilter(groupStatCheckBox, subgroupStatCheckBox, 
                franchiseStatCheckBox, metroTextBox1);

            heroPicker = new HeroPicker(listView1)
            {
                GetImageId = imageResource.Index,
                GetHeroes = () => Model.Hero
            };

            heroSelector = new HeroSelector(listView2)
            {
                GetImageId = imageResource.Index,
                GetHeroes = () => Model.Hero,
                GetFranchiseFilter = heroFilter.GetFranchiseFilter,
                GetGroupFilter = heroFilter.GetGroupsFilter,
                GetSubGroupFilter = heroFilter.GetSubGroupFilter,
                GetTextFilter = heroFilter.GetTextFilter
            };

            heroComponent = new HeroComponent(controls)
            {
                GetImage = imageResource.Bitmap,
                GetLargeImage = largeImageResource.Bitmap,
                GetHero = (x) => Model.Hero.Select(x).Item[0]
            };

            renderSettings = new RenderSettings(metroComboBox1, metroTextBox15, metroCheckBox2, numericUpDown1)
            {
                GetRenderType = () => imageResource.RenderType.ToArray()
            };
            renderSettings.Init();

            styleManager = new StyleManager(controls, metroContextMenu1, metroStyleManager1, this);

            themeSelector = new ThemeSelector(metroComboBox2, metroComboBox3);

            statComponent = new StatComponent(zedGraphControl1)
            {
                Hero = x => Model.Hero.Item[x].Hero,
                HeroesAvgStatistic = () => Model.Statistic.HeroStatistic.All().Item1,
                HeroGameCount = x => Model.Hero.Select(x).Item[0].Statistic.count,
                Image = imageResource.Bitmap,
                Style = () => styleManager.Style,
                Filter = () =>
                {
                    return Model.Hero.
                    Select(heroStatFilter.GetGroupsFilter()).
                    Select(heroStatFilter.GetSubGroupFilter()).
                    Select(heroStatFilter.GetFranchiseFilter()).
                    Select(heroStatFilter.GetTextFilter()).Select(X => X.Id).ToList();
                }
            };
            statComponent.Init();

            statGraph = new StatGraph(metroComboBox4)
            {
                GetSupportHeroStatistic = () => statComponent.HeroStat
            };
            statGraph.HeroesSelectionChanged += statComponent.ComputeHeroesStat;
            statGraph.Init();

            heroSelector.SelectionChanged += contextMenu.SelectPlayer;

            heroPicker.SelectionChanged += heroComponent.Render;

            playerPick.PickChanged += contextMenu.ChangeElement;
            contextMenu.OnPlayerPicked += playerPick.SetPick;

            heroFilter.OnGroupChanged += heroSelector.Render;
            heroFilter.OnSubGroupChanged += heroSelector.Render;
            heroFilter.OnFranchiseChanged += heroSelector.Render;
            heroFilter.OnTextСhanged += heroSelector.Render;

            heroStatFilter.OnGroupChanged += statComponent.ComputeHeroesStat;
            heroStatFilter.OnSubGroupChanged += statComponent.ComputeHeroesStat;
            heroStatFilter.OnFranchiseChanged += statComponent.ComputeHeroesStat;
            heroStatFilter.OnTextСhanged += statComponent.ComputeHeroesStat;

            themeSelector.OnThemeChanged += styleManager.ChangeTheme;
            themeSelector.OnStyleChanged += styleManager.ChangeStyle;

            renderSettings.OnBackgroundChanged += styleManager.SetBackground;
            renderSettings.OnTransparentChanged += styleManager.TransparentChange;
            renderSettings.OnBackgroundChanged += heroPicker.ChangeBackGround;
            renderSettings.OnRenderModeChanged += heroPicker.RenderModeChange;
            renderSettings.OnImageSizeChanged += heroPicker.ItemSizeChange;
            renderSettings.OnBackgroundChanged += heroSelector.ChangeBackGround;
            renderSettings.OnRenderModeChanged += heroSelector.RenderModeChange;
            renderSettings.OnImageSizeChanged += heroSelector.ItemSizeChange;
            renderSettings.OnImageSizeChanged += imageResource.ImageSizeChange;

            LoadToolTip();
            Render();
        }

       

        private void Groups()
        {
            groupCheckBox = new List<MetroCheckBox>
            {
                this.metroCheckBox1,this.cb_specialist,this.cb_support,
                this.cb_assassin,this.cb_warrior
            };

            groupStatCheckBox = new List<MetroCheckBox>
            {
                this.metroCheckBox35,this.metroCheckBox38,this.metroCheckBox37,
                this.metroCheckBox36,this.metroCheckBox39
            };

            franchiseCheckBox = new List<MetroCheckBox>
            {
                this.metroCheckBox6,this.metroCheckBox7,this.metroCheckBox8,this.metroCheckBox9,
                this.metroCheckBox10,this.metroCheckBox11
            };

            franchiseStatCheckBox = new List<MetroCheckBox>
            {
                this.metroCheckBox34,this.metroCheckBox33,this.metroCheckBox32,this.metroCheckBox31,
                this.metroCheckBox30,this.metroCheckBox29
            };

            subgroupCheckBox = new List<MetroCheckBox>
            {
                metroCheckBox12,metroCheckBox13,metroCheckBox14,metroCheckBox15,
                metroCheckBox16,metroCheckBox17,metroCheckBox18,metroCheckBox19,
                metroCheckBox20,metroCheckBox21,
            };

            subgroupStatCheckBox = new List<MetroCheckBox>
            {
                metroCheckBox28,metroCheckBox27,metroCheckBox26,metroCheckBox25,
                metroCheckBox24,metroCheckBox23,metroCheckBox22,metroCheckBox5,
                metroCheckBox4,metroCheckBox3,
            };

            playerpickButton = new List<BunifuFlatButton>()
            {
                bunifuFlatButton1,bunifuFlatButton2,bunifuFlatButton3,bunifuFlatButton4,
                bunifuFlatButton5,bunifuFlatButton6,bunifuFlatButton7,bunifuFlatButton8,
                bunifuFlatButton9,bunifuFlatButton10
            };

            textLabel = new List<BunifuCustomLabel>()
            {
                bunifuCustomLabel1,bunifuCustomLabel2,bunifuCustomLabel3,bunifuCustomLabel6
            };

            resultLabel = new List<BunifuCustomLabel>()
            {
                bunifuCustomLabel4,bunifuCustomLabel5,bunifuCustomLabel7
            };

            GetControls(this.Controls, controls);
        }

        private void LoadToolTip()
        {
            var tip = metroToolTip1;
            tip.SetToolTip(cb_specialist, Resources.SpecialistText);
            tip.SetToolTip(cb_support, Resources.SupportText);
            tip.SetToolTip(cb_assassin, Resources.AssassinText);
            tip.SetToolTip(cb_warrior, Resources.WarriorText);
        }

        private void Render()
        {
            styleManager.Render();
            this.Refresh();

            metroCheckBox2.Checked = Settings.Default.SimpleMode;

            heroPicker.Render();
            heroSelector.Render();
        } 
    
        public void GetControls(Control.ControlCollection control, List<Control> controls)
        {
            foreach (Control it in control)
            {
                controls.Add(it);
                GetControls(it.Controls, controls);
            }
        }

    }
}

