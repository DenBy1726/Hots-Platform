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
    /// <summary>
    /// компонент для скрытия и отображения панели фильтра
    /// </summary>
    class FilterTab
    {
        SplitContainer cont_Filter;
        BunifuTileButton btn_FilterVisibleChange;

        public FilterTab(SplitContainer container, BunifuTileButton button)
        {
            cont_Filter = container;
            btn_FilterVisibleChange = button;
            btn_FilterVisibleChange.Click += FilterVisibleChange;
            
        }

        private void FilterVisibleChange(object sender, EventArgs e)
        {
            if (cont_Filter.Panel2Collapsed == true)
            {
                Show();
            }
            else
            {
                Collapse();
            }
            cont_Filter.SplitterDistance = 115;
        }

        public void Collapse()
        {
            cont_Filter.Panel2Collapsed = true;
            cont_Filter.Size = new Size(115, 45);
            btn_FilterVisibleChange.LabelText = "Показать";
        }

        public void Show()
        {
            cont_Filter.Panel2Collapsed = false;
            cont_Filter.Size = new Size(600, 91);
            btn_FilterVisibleChange.LabelText = "Скрыть";
        }

    }
}
