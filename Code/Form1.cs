using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Final_Project.Extensions;

namespace Final_Project
{
    public partial class Form1 : Form
    {
        public Panel mainPanel;
        private CustomLabel moneyCountLabel;
        private PictureBox moneyCountPictureBox;
        private CustomLabel focusTimeLabel;
        private PictureBoxButton storeButton;
        public TableLayoutPanel ForestPanel;
        private PictureBoxButton startTreeButton;
        private PictureBoxButton leftArrowButton;
        private PictureBoxButton rightArrowButton;
        private PictureBoxButton todoButton;
        private PictureBoxButton vocabButton;
        private CustomLabel currentTreeLabel;
        private UserInfo userInfo;
        private PictureBoxButton musicButton;

        //Colors
        public static Color backgroundColor, foregroundColor, mainTextColor, invertTextColor, forestPanelColor;

        //Images
        public static Image startButtonImage, todoButtonImage, storeButtonImage, cardButtonImage, coinImage, homeImage, returnImage, addButtonImage, deleteButtonImage, rightButtonImage, leftButtonImage;
        public static Image forestImage, decorationImage, seedButtonImage, playImage, restartImage;
        public static Image daisyImage, benchImage, windmillImage;
        public static Image musicImage, folderImage, mplayImage, mpauseImage;

        //ToDoList
        private int taskCount = 1;
        private Panel pnlToDoList;
        private CustomLabel lblToDoTitle;
        private PictureBoxButton btnAddTask;
        private PictureBoxButton btnReturn;
        private Panel pnlTaskContainer;
        private PictureBox backgroundImage;

        //Vocab
        private FormVocab formVocab;

        public Form1()
        {
            InitializeComponent();

            userInfo = new UserInfo(400);
            Init();
            //ToDoList
            CreateUI();

            this.Resize += Form1_Resize;
            Form1_Resize(null, null); // 初始定位

            formVocab = new FormVocab(userInfo);
        }

        private void Init()
        {
            backgroundColor = ColorHelper.FromHEX("#f3c893");
            foregroundColor = ColorHelper.FromHEX("#F3C893");
            forestPanelColor = ColorHelper.FromHEX("#52675d");
            mainTextColor = ColorHelper.FromHEX("#1f2025");
            invertTextColor = ColorHelper.FromHEX("e2d9e4");

            startButtonImage = LoadEmbeddedImage("clock.png");
            coinImage = LoadEmbeddedImage("coin.png");
            todoButtonImage = LoadEmbeddedImage("check.png");
            cardButtonImage = LoadEmbeddedImage("menu.png");
            storeButtonImage = LoadEmbeddedImage("shopping-bag.png");
            addButtonImage = LoadEmbeddedImage("add.png");
            homeImage = LoadEmbeddedImage("home.png");
            deleteButtonImage = LoadEmbeddedImage("trash.png");
            rightButtonImage = LoadEmbeddedImage("right-arrow.png");
            leftButtonImage = LoadEmbeddedImage("left-arrow.png");
            forestImage = LoadEmbeddedImage("forest.png");
            decorationImage = LoadEmbeddedImage("daisy.png");
            seedButtonImage = LoadEmbeddedImage("growing.png");
            playImage = LoadEmbeddedImage("play.png");
            restartImage = LoadEmbeddedImage("restart.png");
            daisyImage = LoadEmbeddedImage("daisy.png");
            benchImage = LoadEmbeddedImage("park.png");
            windmillImage = LoadEmbeddedImage("windmill.png");
            musicImage = LoadEmbeddedImage("music.png");
            folderImage = LoadEmbeddedImage("folder.png");
            mplayImage = LoadEmbeddedImage("mplay.png");
            mpauseImage = LoadEmbeddedImage("mpause.png");

            // 視窗設定
            this.Text = "Grow with Time";
            this.Size = new Size(1000, 600);
            this.BackColor = backgroundColor;

            // Main Panel (0.7 of Form width, Centered)
            mainPanel = new Panel()
            {
                Size = new Size((int)(this.ClientSize.Width * 0.7), this.ClientSize.Height),
                BackColor = Color.Transparent
            };
            this.Controls.Add(mainPanel);

            // 金幣數量標籤
            moneyCountLabel = new CustomLabel(20)
            {
                Text = "" + userInfo.Money,
                TextAlign = ContentAlignment.MiddleRight,
                ForeColor = mainTextColor,
                AutoSize = true
            };
            mainPanel.Controls.Add(moneyCountLabel);

            //coin icon
            moneyCountPictureBox = new PictureBox()
            {
                Image = coinImage,
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(30, 30),
                BackColor = Color.Transparent
            };
            mainPanel.Controls.Add(moneyCountPictureBox);

            // 專注時數之類的 Stats
            focusTimeLabel = new CustomLabel
            {
                Text = "專注時數：" + userInfo.TotalFocusTime + " 分鐘",
                Size = new Size(200, 50),
                ForeColor = mainTextColor,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter
            };
            mainPanel.Controls.Add(focusTimeLabel);

            // 當前種的樹 & 剩餘所需時數
            currentTreeLabel = new CustomLabel
            {
                Text = GetCurrentTreeStatus(),
                Size = new Size(200, 50),
                ForeColor = mainTextColor,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter
            };
            mainPanel.Controls.Add(currentTreeLabel);

            // 開始種樹按鈕
            startTreeButton = new PictureBoxButton
            {
                Text = "開始種樹",
                Font = new Font("Microsoft YaHei", 10),
            };
            startTreeButton.SetOverlayImage(startButtonImage);
            startTreeButton.Click += StartTreeButton_Click;
            mainPanel.Controls.Add(startTreeButton);

            // 森林 TableLayoutPanel
            ForestPanel = new TableLayoutPanel
            {
                BackColor = forestPanelColor,
                ColumnCount = 10,
                RowCount = 5
            };
            for (int i = 0; i < ForestPanel.ColumnCount; i++)
            {
                ForestPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10)); // Each column gets 10% of the width
            }

            for (int i = 0; i < ForestPanel.RowCount; i++)
            {
                ForestPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 20));      // Each row gets 20% of the height
            }


            mainPanel.Controls.Add(ForestPanel);

            // 左箭頭按鈕 (Outside Main Panel)
            leftArrowButton = new PictureBoxButton
            {
                Text = "◀",
                Font = new Font("Microsoft YaHei", 12),
                Size = new Size(50, 50),
                Visible = false
            };
            leftArrowButton.SetOverlayImage(leftButtonImage);
            this.Controls.Add(leftArrowButton);

            // 右箭頭按鈕 (Outside Main Panel)
            rightArrowButton = new PictureBoxButton
            {
                Text = "▶",
                Font = new Font("Microsoft YaHei", 12),
                Size = new Size(50, 50),
                BackColor = backgroundColor,
                ForeColor = foregroundColor,
                Visible = false
            };
            rightArrowButton.SetOverlayImage(rightButtonImage);
            this.Controls.Add(rightArrowButton);

            // 商店按鈕 (Right Side Outside Main Panel)
            storeButton = new PictureBoxButton
            {
                Text = "商店",
                Font = new Font("Microsoft YaHei", 10),
            };
            storeButton.SetOverlayImage(storeButtonImage);
            storeButton.Click += StoreButton_Click;
            this.Controls.Add(storeButton);

            // To-Do List 按鈕 (Right Side Outside Main Panel)
            todoButton = new PictureBoxButton
            {
                Text = "To-Do List",
                Font = new Font("Microsoft YaHei", 10),
            };
            todoButton.SetOverlayImage(todoButtonImage);
            todoButton.Click += BtnShowToDoList_Click;
            this.Controls.Add(todoButton);

            // 背單字按鈕 (Right Side Outside Main Panel)
            vocabButton = new PictureBoxButton
            {
                Text = "背單字",
                Font = new Font("Microsoft YaHei", 10),
            };
            vocabButton.SetOverlayImage(cardButtonImage);
            vocabButton.Click += VocabButton_Click;
            this.Controls.Add(vocabButton);

            musicButton = new PictureBoxButton
            {
                Text = "播放音樂",
                Font = new Font("Microsoft YaHei", 10),
            };
            musicButton.SetOverlayImage(musicImage);
            musicButton.Click += MusicButton_Click;  // Attach event handler
            this.Controls.Add(musicButton);

            /*
            backgroundImage = new PictureBox
            {
                SizeMode = PictureBoxSizeMode.StretchImage,
                Size = this.Size,
                Image = LoadEmbeddedImage("Final_Project.Resources.background.png")
            };
            this.Controls.Add(backgroundImage);
            backgroundImage.SendToBack();
            */
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            // Main Panel Resize (0.7 of Form Width, Centered)
            mainPanel.Size = new Size((int)(this.ClientSize.Width * 0.7), this.ClientSize.Height);
            mainPanel.Location = new Point((int)(this.ClientSize.Width * 0.15), 0);

            // Resize ForestPanel to maintain aspect ratio (Width = 0.9 of Main Panel Width)
            int forestWidth = (int)(mainPanel.Width * 0.97);
            int forestHeight = (int)(forestWidth / 2.0); // Maintain 3:1 aspect ratio
            ForestPanel.Size = new Size(forestWidth, forestHeight);

            // Position components in Main Panel
            ControlPositionHelper.PositionControl(moneyCountLabel, mainPanel, ControlPosition.TopRight, horizontalMargin: 40, verticalMargin: 23);
            ControlPositionHelper.PositionControl(moneyCountPictureBox, mainPanel, ControlPosition.TopRight, verticalMargin: 22);
            ControlPositionHelper.PositionControl(focusTimeLabel, mainPanel, ControlPosition.TopLeft, verticalMargin: 23);
            ControlPositionHelper.PositionControl(currentTreeLabel, mainPanel, ControlPosition.TopCenter, verticalMargin: 23);
            ControlPositionHelper.PositionControl(startTreeButton, mainPanel, ControlPosition.TopCenter, verticalMargin: startTreeButton.Height);
            ControlPositionHelper.PositionControl(ForestPanel, mainPanel, ControlPosition.BottomCenter);

            // Position outside buttons on the right side
            ControlPositionHelper.PositionControl(leftArrowButton, this, ControlPosition.BottomLeft, verticalMargin: forestHeight / 2);
            ControlPositionHelper.PositionControl(rightArrowButton, this, ControlPosition.BottomRight, verticalMargin: forestHeight / 2);
            ControlPositionHelper.PositionControl(storeButton, this, ControlPosition.TopRight, 10, 20);
            ControlPositionHelper.PositionControl(todoButton, this, ControlPosition.TopRight, 10, 110);
            ControlPositionHelper.PositionControl(vocabButton, this, ControlPosition.TopRight, 10, 200);
            ControlPositionHelper.PositionControl(musicButton, this, ControlPosition.TopRight, 10, 290);

            //backgroundImage.Size = this.Size;

            //ToDoList
            CenterPnlToDoList();
        }

        private void StoreButton_Click(object sender, EventArgs e)
        {
            StoreForm store = new StoreForm(userInfo, this);
            store.ShowDialog();
            UpdateInterface();
        }

        private void VocabButton_Click(object sender, EventArgs e)
        {
            formVocab.ShowDialog();
            UpdateInterface();
        }

        private void MusicButton_Click(object sender, EventArgs e)
        {
            MusicForm musicForm = new MusicForm();
            musicForm.ShowDialog();
            UpdateInterface();
        }

        private void StartTreeButton_Click(object sender, EventArgs e)
        {
            var focusTimerForm = new FocusTimerForm(userInfo, this);
            focusTimerForm.ShowDialog();
            UpdateInterface();
        }

        private void UpdateInterface()
        {
            moneyCountLabel.Text = "" + userInfo.Money;
            focusTimeLabel.Text = "專注時數：" + userInfo.TotalFocusTime + " 分鐘";
            currentTreeLabel.Text = GetCurrentTreeStatus();
            Form1_Resize(null, null);
        }

        private string GetCurrentTreeStatus()
        {
            if (userInfo.TreesPlanting.Count == 0)
                return "目前沒有種植中的樹。";

            var tree = userInfo.TreesPlanting[0];
            return $"{tree.Name} - 剩餘 {tree.RemainingTime} 分鐘";
        }

        private void CreateUI()
        {
            // To Do List Panel
            pnlToDoList = new Panel();
            pnlToDoList.Size = new Size(400, 500);
            pnlToDoList.BorderStyle = BorderStyle.FixedSingle;
            //pnlToDoList.AutoScroll = false;
            pnlToDoList.Visible = false;
            this.Controls.Add(pnlToDoList);

            // To Do Panel Title
            lblToDoTitle = new CustomLabel(14, FontStyle.Bold);
            lblToDoTitle.Text = "To Do List";
            lblToDoTitle.AutoSize = true;
            pnlToDoList.Controls.Add(lblToDoTitle);
            lblToDoTitle.Location = new Point((pnlToDoList.Width / 2) - (lblToDoTitle.Width / 2), 10);

            // Add Task Button
            btnAddTask = new PictureBoxButton();
            btnAddTask.Text = "Add Task";
            btnAddTask.Size = new Size(50, 50);
            btnAddTask.Location = new Point((pnlToDoList.Width / 2) - 20 - btnAddTask.Width, lblToDoTitle.Height + 20);
            btnAddTask.Click += BtnAddTask_Click;
            btnAddTask.SetOverlayImage(addButtonImage);
            pnlToDoList.Controls.Add(btnAddTask);

            // Return Button
            btnReturn = new PictureBoxButton();
            btnReturn.Text = "Return";
            btnReturn.Size = new Size(50, 50);
            btnReturn.Location = new Point((pnlToDoList.Width / 2) + 20, lblToDoTitle.Height + 20);
            btnReturn.Click += BtnReturn_Click;
            btnReturn.SetOverlayImage(homeImage);
            pnlToDoList.Controls.Add(btnReturn);

            // Panel Task Container
            pnlTaskContainer = new Panel();
            pnlTaskContainer.Size = new Size(pnlToDoList.Width - 20, pnlToDoList.Height - lblToDoTitle.Height - btnAddTask.Height - 40);
            pnlTaskContainer.Location = new Point(10, lblToDoTitle.Height + btnAddTask.Height + 30);
            pnlTaskContainer.BackColor = ColorHelper.FromHEX("#2e334d");
            pnlTaskContainer.BorderStyle = BorderStyle.None;
            pnlTaskContainer.AutoScroll = true;
            pnlToDoList.Controls.Add(pnlTaskContainer);
        }

        private void CenterPnlToDoList()
        {
            pnlToDoList.Location = new Point(
                (this.ClientSize.Width / 2) - (pnlToDoList.Width / 2),
                (this.ClientSize.Height / 2) - (pnlToDoList.Height / 2)
            );
        }

        private void BtnShowToDoList_Click(object sender, EventArgs e)
        {
            pnlToDoList.Visible = !pnlToDoList.Visible;
            pnlToDoList.BringToFront();
            foreach (Control control in this.Controls)
            {
                if (control != pnlToDoList)
                {
                    control.Enabled = false;
                }
            }
        }

        private void BtnReturn_Click(object sender, EventArgs e)
        {
            pnlToDoList.Visible = false;
            foreach (Control control in this.Controls)
            {
                control.Enabled = true;
            }
        }

        private void BtnAddTask_Click(object sender, EventArgs e)
        {
            AddNewTask();
        }

        private void AddNewTask()
        {
            int lastControlBottom = 0;
            if (pnlTaskContainer.Controls.Count > 0)
            {
                Control lastControl = pnlTaskContainer.Controls[pnlTaskContainer.Controls.Count - 1];
                lastControlBottom = lastControl.Bottom;
            }

            Panel taskPanel = new Panel();
            taskPanel.Size = new Size(360, 30);
            taskPanel.Name = "taskPanel_" + taskCount;
            pnlTaskContainer.Controls.Add(taskPanel);
            taskPanel.Location = new Point(10, lastControlBottom + 10);

            CheckBox chkTask = new CheckBox();
            chkTask.AutoSize = false;
            chkTask.Size = new Size(25, 25);
            chkTask.Location = new Point(10, 5);

            TextBox txtTask = new TextBox();
            txtTask.Text = "Task " + taskCount;
            txtTask.Location = new Point(40, 10);
            txtTask.Size = new Size(210, 25);
            txtTask.ForeColor = invertTextColor;
            txtTask.BackColor = taskPanel.BackColor;
            txtTask.BorderStyle = BorderStyle.None;

            chkTask.CheckedChanged += (s, ev) =>
            {
                if (chkTask.Checked)
                {
                    txtTask.ForeColor = Color.Gray;
                }
                else
                {
                    txtTask.ForeColor = Color.Black;
                }
            };

            PictureBoxButton btnDelete = new PictureBoxButton();
            btnDelete.Size = new Size(25, 25);
            btnDelete.Location = new Point(330, 5);
            btnDelete.SetOverlayImage(deleteButtonImage);

            btnDelete.Click += (s, ev) =>
            {
                int originalScrollPosition = pnlTaskContainer.VerticalScroll.Value;
                pnlTaskContainer.VerticalScroll.Value = 0;
                pnlTaskContainer.PerformLayout();

                pnlTaskContainer.Controls.Remove(taskPanel);
                UpdateTaskPanelLocations();

                pnlTaskContainer.VerticalScroll.Value = Math.Min(originalScrollPosition, pnlTaskContainer.VerticalScroll.Maximum);
                pnlTaskContainer.PerformLayout();
            };

            taskPanel.Controls.Add(chkTask);
            taskPanel.Controls.Add(txtTask);
            taskPanel.Controls.Add(btnDelete);
            taskCount++;
        }

        private void UpdateTaskPanelLocations()
        {
            int newY = 10;
            foreach (Control control in pnlTaskContainer.Controls)
            {
                if (control is Panel)
                {
                    if (control.Location.Y != newY)
                    {
                        control.Location = new Point(10, newY);
                    }
                    newY = control.Bottom + 10;
                }
            }
        }
        private static Image LoadEmbeddedImage(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            string fullResourceName = "Final_Project.Resources." + resourceName;

            using (Stream stream = assembly.GetManifestResourceStream(fullResourceName))
            {
                if (stream != null)
                {
                    return Image.FromStream(stream);
                }
                else
                {
                    throw new Exception($"Resource not found: {fullResourceName}");
                }
            }
        }
    }
}
