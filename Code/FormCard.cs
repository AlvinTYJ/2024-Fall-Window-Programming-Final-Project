using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Final_Project
{
    public partial class FormCard : Form
    {
        public List<TextBoxData> dataList = new List<TextBoxData>();
        public FormCard()
        {
            InitializeComponent();
            this.BackColor = Form1.backgroundColor;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            // 檢查是否已包含相同的 vocabulary
            if (dataList.Any(data => data.vocabulary.Equals(textBox1.Text, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("此單字已存在！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox1.Clear();
                textBox2.Clear();
                foreach (RadioButton rb in panelPartOfSpeech.Controls.OfType<RadioButton>())
                    rb.Checked = false;
                return;
            }
            string partOfSpeech = GetSelectedPartOfSpeech();
            if (string.IsNullOrEmpty(partOfSpeech))
            {
                MessageBox.Show("請選擇詞性！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (textBox1.Text != "" && textBox2.Text != "")
            {
                TextBoxData data = new TextBoxData
                {
                    vocabulary = textBox1.Text,
                    translation = textBox2.Text,
                    PartOfSpeech = partOfSpeech
                };
                dataList.Add(data);   // 將資料加入清單

                textBox1.Clear();
                textBox2.Clear();
                foreach (RadioButton rb in panelPartOfSpeech.Controls.OfType<RadioButton>())
                    rb.Checked = false;
                textBox1.Focus();
            }
            else
            {
                MessageBox.Show("請填寫完整的單字與翻譯內容！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }


        private string GetSelectedPartOfSpeech()
        {
            foreach (RadioButton rb in panelPartOfSpeech.Controls.OfType<RadioButton>())
            {
                if (rb.Checked)
                {
                    return rb.Text; // 返回選中的詞性文本
                }
            }
            return null; // 沒有選中任何選項
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            foreach (RadioButton rb in panelPartOfSpeech.Controls.OfType<RadioButton>())
                rb.Checked = false;
            textBox1.Focus();
            this.Close();
        }
    }

    // 用於儲存 TextBox 資料的類別
    public class TextBoxData
    {
        public string vocabulary { get; set; }
        public string translation { get; set; }
        public string PartOfSpeech { get; set; }
    }
}
