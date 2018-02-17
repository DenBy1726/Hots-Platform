using HoTS_Forecaster_form.Properties;
using MetroFramework.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HoTS_Forecaster_form.View
{
    class RenderSettings
    {
        MetroComboBox renderCombobox;
        MetroTextBox textBox;
        MetroCheckBox checkbox;
        NumericUpDown numeric;

        Func<string[]> getRenderType;

        public event Action<Image> OnBackgroundChanged;
        public event Action<Color> OnTransparentChanged;
        public event Action<string> OnRenderModeChanged;
        public event Action<int> OnImageSizeChanged;

        public Func<string[]> GetRenderType { get => getRenderType; set => getRenderType = value; }

        public RenderSettings(MetroComboBox renderCombobox,MetroTextBox textBox,
            MetroCheckBox checkbox,NumericUpDown numeric)
        {
            this.renderCombobox = renderCombobox;
            this.textBox = textBox;
            this.checkbox = checkbox;
            this.numeric = numeric;

            this.checkbox.CheckStateChanged += Checkbox_CheckStateChanged;
            this.textBox.ButtonClick += TextBox_ButtonClick;
            this.renderCombobox.SelectedValueChanged += RenderCombobox_SelectedValueChanged;
            this.numeric.ValueChanged += Numeric_ValueChanged;
        }

        private void Numeric_ValueChanged(object sender, EventArgs e)
        {
            int value = (int)numeric.Value;
            Settings.Default.IconSize = value;
            Settings.Default.Save();

            if (OnImageSizeChanged != null)
                OnImageSizeChanged.Invoke(value);
        }

        private void RenderCombobox_SelectedValueChanged(object sender, EventArgs e)
        {
            string text = renderCombobox.SelectedItem as string;
            if (text == "По умолчанию" || text == "")
                text = "";
            else
                text = "_" + text;

            Settings.Default.RenderType = text;
            Settings.Default.Save();

            if (OnRenderModeChanged != null)
                OnRenderModeChanged.Invoke(text);
        }

        private void Checkbox_CheckStateChanged(object sender, EventArgs e)
        {
            Color c;
            if (checkbox.Checked == false)
            {
                c = Color.Transparent;
               
            }
            else
            {
                c = Color.FromArgb(15, 15, 15);
            }

            if (OnTransparentChanged != null)
                OnTransparentChanged.Invoke(c);

            Settings.Default.SimpleMode = checkbox.Checked;
            Settings.Default.Save();

        }

        public void Init()
        {
            renderCombobox.Items.Add("По умолчанию");
            renderCombobox.Items.AddRange(getRenderType());

            SetBackground(Settings.Default.background);
        }

        private void TextBox_ButtonClick(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog()
            {
                Filter = "|*.jpg;*.png"
            };
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                Settings.Default.background = dlg.FileName;
                Settings.Default.Save();
                textBox.Text = Settings.Default.background;
                SetBackground(Settings.Default.background);
            }
        }

        public void SetBackground(string path)
        {
            Image background = new Bitmap(path);
            if (background != null)
            {
                Settings.Default.background = path;
                if (OnBackgroundChanged != null)
                    OnBackgroundChanged.Invoke(background);
            }
        }
    }
}
