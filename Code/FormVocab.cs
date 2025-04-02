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
    public partial class FormVocab : Form
    {
        private int yOffset = 0;
        private FormCard formCard;
        private Panel scrollableContainer;
        private UserInfo userInfo;
        public FormVocab(UserInfo user)
        {
            InitializeComponent();
            InitializeScrollableContainer();
            panel1.Visible = false;
            pictureBox1.Visible = false;
            userInfo = user;
            formCard = new FormCard();
            this.BackColor = Form1.backgroundColor;
        }

        private void InitializeScrollableContainer()
        {
            scrollableContainer = new Panel
            {
                AutoScroll = true,
                Size = new Size(390, 410),
                Location = new Point(10, 50)
            };

            this.Controls.Add(scrollableContainer);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            formCard.ShowDialog();
            scrollableContainer.Controls.Clear();
            yOffset = 0;
            foreach (var data in formCard.dataList)
            {
                Panel newPanel = new Panel
                {
                    Size = panel1.Size,
                    Location = new Point(10, yOffset),
                    BackColor = panel1.BackColor,
                };

                Label label1 = new Label
                {
                    Text = data.vocabulary,
                    Location = panel1.Controls["label1"].Location,
                    Font = panel1.Controls["label1"].Font,
                    AutoSize = true
                };

                Label label2 = new Label
                {
                    Text = "(" + data.PartOfSpeech + ")  " + data.translation,
                    Location = panel1.Controls["label2"].Location,
                    Font = panel1.Controls["label2"].Font,
                    AutoSize = true
                };

                Button btnDelete = new Button
                {
                    BackgroundImage = panel1.Controls["btnDelete"].BackgroundImage,
                    BackgroundImageLayout = ImageLayout.Zoom,
                    Size = panel1.Controls["btnDelete"].Size,
                    Location = panel1.Controls["btnDelete"].Location,
                };
                btnDelete.Click += (s, args) =>
                {
                    formCard.dataList.Remove(data);
                    scrollableContainer.Controls.Remove(newPanel);
                    UpdatePanels();

                };

                // 新增隱藏按鈕
                Button btnHide = new Button
                {
                    BackgroundImage = panel1.Controls["btnHide"].BackgroundImage,
                    BackgroundImageLayout = ImageLayout.Zoom,
                    Size = panel1.Controls["btnHide"].Size,
                    Location = panel1.Controls["btnHide"].Location
                };
                btnHide.Click += (s, args) =>
                {
                    foreach (Control control in newPanel.Controls)
                    {
                        if (control is Label lbl && lbl.Text.Contains(data.translation))
                        {
                            lbl.Visible = lbl.Visible ? false : true;
                            btnHide.BackgroundImage = lbl.Visible ? panel1.Controls["btnHide"].BackgroundImage : pictureBox1.Image;
                            break;
                        }
                    }
                };

                newPanel.Controls.Add(label1);
                newPanel.Controls.Add(label2);
                newPanel.Controls.Add(btnDelete);
                newPanel.Controls.Add(btnHide);

                scrollableContainer.Controls.Add(newPanel);
                yOffset += newPanel.Height + 10;
            }
        }

        private void UpdatePanels()
        {
            Point pointNow = scrollableContainer.AutoScrollPosition;
            Point savedPosition = new Point(-pointNow.X, -pointNow.Y);
            scrollableContainer.AutoScrollPosition = new Point(0, 0);
            yOffset = 0;
            foreach (Control panel in scrollableContainer.Controls)
            {
                panel.Location = new Point(10, yOffset);
                yOffset += panel.Height + 10;
            }
            scrollableContainer.AutoScrollPosition = savedPosition;
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            if (formCard.dataList.Count >= 6)
            {
                FormTest formTest = new FormTest(formCard.dataList, userInfo);
                formTest.ShowDialog();
            }
            else
            {
                MessageBox.Show("多背一點單字再來玩吧！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }
    }
}
