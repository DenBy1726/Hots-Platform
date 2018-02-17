using HoTS_Service.Entity.Enum;
using MetroFramework.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoTS_Forecaster_form.View
{

    /// <summary>
    /// компонент для фильтрации списка героев ( получения множества допустимых значений
    /// по категориям)
    /// </summary>
    public class HeroFilter
    {
        //дочерние флажка фильтров
        List<MetroCheckBox> group, franchise,subgroup;

        // текствоый фильтр
        MetroTextBox textFilter;

        /// <summary>
        /// Событие изменения фильтра группы
        /// </summary>
        public event Action OnGroupChanged;

        /// <summary>
        /// Событие изменения фильтра франшизы
        /// </summary>
        public event Action OnFranchiseChanged;

        /// <summary>
        /// Событие изменения фильтра подгруппы
        /// </summary>
        public event Action OnSubGroupChanged;

        /// <summary>
        /// Событие изменения текстового фильтра
        /// </summary>
        public event Action OnTextСhanged;

        public HeroFilter(List<MetroCheckBox> group, List<MetroCheckBox> subgroup,
            List<MetroCheckBox> franchise,MetroTextBox textFilter)
        {
            foreach(var it in group)
            {
                it.CheckStateChanged += OnGroupCheckChanged;
            }

            this.group = group;

            foreach (var it in franchise)
            {
                it.CheckStateChanged += OnFranchiseCheckChanged;
            }

            this.franchise = franchise;

            foreach (var it in subgroup)
            {
                it.CheckStateChanged += OnSubGroupCheckChanged;
            }

            this.subgroup = subgroup;

            this.textFilter = textFilter;
            textFilter.TextChanged += TextFilter_TextChanged;
        }

        private void TextFilter_TextChanged(object sender, EventArgs e)
        {
            if (OnTextСhanged != null)
                OnTextСhanged.Invoke();
        }

        private void OnGroupCheckChanged(object sender, EventArgs e)
        {
            var control = sender as MetroCheckBox;
            if (control.Tag.Equals("All"))
            {
                foreach(var it in group)
                {
                    it.CheckState = control.CheckState;
                }
            }
            if (OnGroupChanged != null)
                OnGroupChanged.Invoke();
        }

        private void OnFranchiseCheckChanged(object sender, EventArgs e)
        {
            var control = sender as MetroCheckBox;
            if (control.Tag.Equals("All"))
            {
                foreach (var it in franchise)
                {
                    it.CheckState = control.CheckState;
                }
            }
            if (OnFranchiseChanged != null)
                OnFranchiseChanged.Invoke();
        }

        private void OnSubGroupCheckChanged(object sender, EventArgs e)
        {
            var control = sender as MetroCheckBox;
            if (control.Tag.Equals("All"))
            {
                foreach (var it in subgroup)
                {
                    it.CheckState = control.CheckState;
                }
            }
            if (OnSubGroupChanged != null)
                OnSubGroupChanged.Invoke();
        }

        /// <summary>
        /// возвращает список групп согласно фильтру
        /// </summary>
        /// <returns></returns>
        public HashSet<HeroGroup> GetGroupsFilter()
        {
            HashSet<HeroGroup> rez = new HashSet<HeroGroup>();
            var checkArray = group.Where(x => x.CheckState == System.Windows.Forms.CheckState.Checked
                && x.Tag.Equals("All") == false).Select(x => x.Tag).ToList();

            foreach(var it in checkArray)
            {
                rez.Add((HeroGroup)Enum.Parse(typeof(HeroGroup), it.ToString()));
            }
            return rez;
        }

        /// <summary>
        /// возвращает список франшиз согласно фильтру
        /// </summary>
        /// <returns></returns>
        public HashSet<Franchise> GetFranchiseFilter()
        {
            HashSet<Franchise> rez = new HashSet<Franchise>();
            var checkArray = franchise.Where(x => x.CheckState == System.Windows.Forms.CheckState.Checked
                && x.Tag.Equals("All") == false).Select(x => x.Tag).ToList();

            foreach (var it in checkArray)
            {
                rez.Add((Franchise)Enum.Parse(typeof(Franchise), it.ToString()));
            }
            return rez;
        }

        /// <summary>
        /// фозвращает список подгрупп согласно фильтру
        /// </summary>
        /// <returns></returns>
        public HashSet<HeroSubGroup> GetSubGroupFilter()
        {
            HashSet<HeroSubGroup> rez = new HashSet<HeroSubGroup>();
            var checkArray = subgroup.Where(x => x.CheckState == System.Windows.Forms.CheckState.Checked
                && x.Tag.Equals("All") == false).Select(x => x.Tag).ToList();

            foreach (var it in checkArray)
            {
                rez.Add((HeroSubGroup)Enum.Parse(typeof(HeroSubGroup), it.ToString()));
            }
            return rez;
        }

        /// <summary>
        /// фозвращает значение текстового фильтра
        /// </summary>
        /// <returns></returns>
        public string GetTextFilter()
        {
            return textFilter.Text;
        }
    }
}
