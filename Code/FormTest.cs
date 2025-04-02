using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Final_Project
{
    public partial class FormTest : Form
    {
        public List<TextBoxData> testList = new List<TextBoxData>();
        private List<TextBoxData> selectedData = new List<TextBoxData>();
        private List<TextBoxData> wrongAns = new List<TextBoxData>();
        private List<Button> buttons = new List<Button>();
        private Button firstSelectedButton = null;
        private Button secondSelectedButton = null;
        private Image cross = null;
        private int wrong = 0;
        private float opacity = 1.0f; // 初始透明度
        private UserInfo userInfo;
        public FormTest(List<TextBoxData> formTesttestList, UserInfo user)
        {
            InitializeComponent();
            this.AutoSize = false;
            testList = formTesttestList;
            buttons = this.Controls.OfType<Button>().ToList();
            RandomizeVocabulary();
            pictureBox1.Visible = false;
            pictureBox2.Visible = false;
            cross = pictureBox1.Image;
            userInfo = user;
            this.BackColor = Form1.backgroundColor;
        }

        private void RandomizeVocabulary()
        {
            Random rnd = new Random();
            selectedData = testList.OrderBy(x => rnd.Next()).Take(6).ToList();
            List<int> buttonIndices = Enumerable.Range(0, buttons.Count).OrderBy(x => rnd.Next()).ToList();
            for (int i = 0; i < 6; i++)
                buttons[buttonIndices[i]].Text = selectedData[i].vocabulary;
            for (int i = 6; i < buttons.Count; i++)
                buttons[buttonIndices[i]].Text = selectedData[i - 6].translation;
        }

        private void Button_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;

            if (firstSelectedButton == null)
            {
                firstSelectedButton = clickedButton;
                firstSelectedButton.BackColor = Color.LightBlue;
            }
            else if (secondSelectedButton == null && clickedButton != firstSelectedButton)
            {
                secondSelectedButton = clickedButton;
                secondSelectedButton.BackColor = Color.LightBlue;

                // 檢查兩個按鈕是否符合條件
                CheckMatchingPair();
            }
        }

        private void CheckMatchingPair()
        {
            // 檢查選擇的兩個按鈕內容是否符合 testList 中的 vocabulary 和 translation
            string text1 = firstSelectedButton.Text;
            string text2 = secondSelectedButton.Text;

            bool isMatch = selectedData.Any(data =>
                (data.vocabulary == text1 && data.translation == text2) ||
                (data.vocabulary == text2 && data.translation == text1));

            if (isMatch)//配對正確
            {
                // 如果匹配，從 Form 中移除這兩個按鈕
                this.Controls.Remove(firstSelectedButton);
                this.Controls.Remove(secondSelectedButton);

                buttons.Remove(firstSelectedButton);
                buttons.Remove(secondSelectedButton);

                if (buttons.Count == 0)
                {
                    finalPicture();
                }
            }
            else//配對錯誤
            {
                wrong++;
                //var wrongItem = selectedData.FirstOrDefault(data =>
                //    data.vocabulary == text1 || data.vocabulary == text2);
                var wrongItem = selectedData.FirstOrDefault(data => data.vocabulary == text1);

                // 如果 text1 不存在，則找 text2（保證錯誤記錄至少選擇一個）
                if (wrongItem == null)
                {
                    wrongItem = selectedData.FirstOrDefault(data => data.vocabulary == text2);
                }
                if (wrongItem == null)
                {
                    wrongItem = selectedData.FirstOrDefault(data => data.translation == text1);
                }
                if (wrongItem != null && !wrongAns.Contains(wrongItem))
                {
                    wrongAns.Add(wrongItem);
                }
                opacity = 1.0f; // 重置透明度
                pictureBox1.Visible = true;
                pictureBox2.Visible = true;
                pictureBox1.Location = new Point(firstSelectedButton.Left + 40, firstSelectedButton.Top + 10);
                pictureBox2.Location = new Point(secondSelectedButton.Left + 40, secondSelectedButton.Top + 10);
                timer1.Enabled = true;
            }

            // 重置選擇狀態
            ResetSelection();
        }

        private void finalPicture()
        {
            this.Controls.Clear();

            Label label = new Label();

            label.Font = new Font("新細明體", 16f);
            if (wrong == 0)
            {
                label.Location = new Point(165, 30);
                label.Text = "全對!"; //給金幣
                userInfo.Money += 10;
            }
            else
            {
                label.SetBounds(55, 30, 300, 40);
                label.Text = $"錯了 {wrong} 次，答錯的單字如下:";
                RichTextBox richTextBox = new RichTextBox();
                richTextBox.SetBounds(50, 70, 300, 315);
                richTextBox.BorderStyle = BorderStyle.None;
                richTextBox.BackColor = Color.White;
                richTextBox.SelectionAlignment = HorizontalAlignment.Center;
                richTextBox.Font = new Font("新細明體", 16f);
                richTextBox.ReadOnly = true;

                if (wrongAns != null && wrongAns.Any())
                {
                    int count = wrongAns.Count;
                    int currentIndex = 0;

                    foreach (var data in wrongAns)
                    {
                        currentIndex++;

                        // 如果是最後一個項目，則不加換行
                        if (currentIndex == count)
                        {
                            richTextBox.AppendText($"{data.vocabulary}   ({data.PartOfSpeech}) {data.translation}");
                        }
                        else
                        {
                            richTextBox.AppendText($"{data.vocabulary}   ({data.PartOfSpeech}) {data.translation}\n\n");
                        }
                    }
                }
                this.Controls.Add(richTextBox);
                richTextBox.BackColor = SystemColors.Control;

            }

            Button button = new Button();
            button.SetBounds(145, 390, 110, 50);
            button.Font = new Font("新細明體", 16f);
            button.Text = "確定";

            this.Controls.Add(label);
            this.Controls.Add(button);
            button.Click += new EventHandler(button_Click);
            this.Size = new Size(421, 506);
        }

        private void button_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ResetSelection()
        {
            if (firstSelectedButton != null) firstSelectedButton.BackColor = SystemColors.ControlLight;
            if (secondSelectedButton != null) secondSelectedButton.BackColor = SystemColors.ControlLight;

            firstSelectedButton = null;
            secondSelectedButton = null;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (opacity > 0) // 透明度尚未到 0
            {
                opacity -= 0.1f; // 每次遞減 0.05，可根據需求調整速度
                if (opacity < 0) opacity = 0; // 確保透明度不小於 0

                // 更新 PictureBox 圖片的透明度
                pictureBox1.Image = SetImageOpacity(pictureBox1.Image, opacity);
                pictureBox2.Image = SetImageOpacity(pictureBox2.Image, opacity);
            }
            else
            {
                timer1.Enabled = false;
                pictureBox1.Image = cross;
                pictureBox2.Image = cross;
                pictureBox1.Visible = false;
                pictureBox2.Visible = false;
            }
        }

        private Image SetImageOpacity(Image image, float opacity)
        {
            // 創建一個 Bitmap 來存儲新的圖片
            Bitmap bmp = new Bitmap(image.Width, image.Height);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                // 設置透明度的顏色矩陣
                ColorMatrix matrix = new ColorMatrix();
                matrix.Matrix33 = opacity; // Matrix33 控制 Alpha 透明度

                // 設置圖像屬性
                ImageAttributes attributes = new ImageAttributes();
                attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                // 繪製圖片
                g.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);
            }

            return bmp;
        }
    }
}
