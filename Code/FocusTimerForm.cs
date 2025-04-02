using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace Final_Project
{
    public class FocusTimerForm : Form
    {
        private Form1 parentForm;
        private UserInfo userInfo;
        private Timer focusTimer;
        private int timerDuration; // 計時器總時長（秒）
        private int remainingTime; // 剩餘時間
        private int minuteCounter;
        private Tree selectedTree; // 當前選擇的樹
        private PictureBox treeImageBox;
        private CustomLabel timerLabel;
        private CustomLabel remainingGrowthLabel; // 樹的剩餘成長時間顯示
        //private Label plantedTreeLabel; // 顯示已種植的樹
        private PictureBoxButton leftButton, rightButton, startButton, sellButton, forestButton, restartButton, exitButton;
        private CustomTrackBar timerSlider; // 新增滑桿選擇時間
        private List<Tree> availableSeeds;
        private int selectedSeedIndex;
        private Tree basicTree; // 基本樹
        private CustomLabel timerSliderLabel;
        private int timerSliderValue = 25;
        private CustomLabel moneyCountLabel;
        private PictureBox moneyPictureBox;

        public FocusTimerForm(UserInfo user, Form1 parent)
        {
            userInfo = user;
            parentForm = parent;
            InitializeInterface();
            InitializeFocusTimer();
            InitializeBasicTree();
            CheckTreeStatus();
            this.Resize += FocusTimerForm_Resize;
            FocusTimerForm_Resize(null, null);
        }

        private void InitializeInterface()
        {
            this.Text = "專注計時器";
            this.Size = new Size(1000, 600);
            this.BackColor = Form1.backgroundColor;

            // 樹圖片
            treeImageBox = new PictureBox()
            {
                Size = new Size(400, 400),
                BorderStyle = BorderStyle.None,
                BackColor = Color.Transparent,
                SizeMode = PictureBoxSizeMode.Zoom // Ensures proper scaling
            };
            this.Controls.Add(treeImageBox);

            // 金幣標籤
            moneyCountLabel = new CustomLabel(18)
            {
                Text = "" + userInfo.Money,
                AutoSize = true
            };
            this.Controls.Add(moneyCountLabel);

            // 金幣圖案
            moneyPictureBox = new PictureBox()
            {
                Image = Form1.coinImage,
                Size = new Size(25, 25),
                SizeMode = PictureBoxSizeMode.Zoom
            };
            this.Controls.Add(moneyPictureBox);

            // 計時器標籤
            timerLabel = new CustomLabel(72, FontStyle.Bold)
            {
                Text = $"{timerSliderValue:D2}:00"　?? "25:00",
                Size = new Size(400, 120),
                TextAlign = ContentAlignment.MiddleRight
            };
            this.Controls.Add(timerLabel);

            // 樹剩餘成長時間顯示
            remainingGrowthLabel = new CustomLabel(12)
            {
                Text = "樹還需要 0 分鐘長大",
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleRight
            };
            this.Controls.Add(remainingGrowthLabel);
            timerLabel.TextChanged += (s, e) =>
            {
                ControlPositionHelper.PositionControl(remainingGrowthLabel, this, ControlPosition.TopRight, 50, 180);
            };

            // 設定時間滑桿
            timerSliderLabel = new CustomLabel()
            {
                Text = "設定時間（分鐘）:",
                AutoSize = true,
            };
            this.Controls.Add(timerSliderLabel);

            timerSlider = new CustomTrackBar()
            {
                Minimum = 5,
                Maximum = 90,
                Value = 25,
                TickFrequency = 5,
                LargeChange = 5,
                SmallChange = 5,
                Size = new Size(200, 30),
                TabStop = false
            };
            timerSlider.Scroll += TimerSlider_Scroll;
            this.Controls.Add(timerSlider);

            // 左右選擇按鈕
            leftButton = new PictureBoxButton() { Text = "◀", Size = new Size(50, 50), Enabled = true }; // 預設禁用
            leftButton.SetOverlayImage(Form1.leftButtonImage);
            rightButton = new PictureBoxButton() { Text = "▶", Size = new Size(50, 50), Enabled = true }; // 預設禁用
            rightButton.SetOverlayImage(Form1.rightButtonImage);
            leftButton.Click += (s, e) => ChangeSelectedSeed(-1);
            rightButton.Click += (s, e) => ChangeSelectedSeed(1);
            this.Controls.Add(leftButton);
            this.Controls.Add(rightButton);

            // 開始按鈕
            startButton = new PictureBoxButton()
            {
                Text = "開始計時",
                Size = new Size(100, 100)
            };
            startButton.SetOverlayImage(Form1.playImage);
            startButton.Click += StartButton_Click;
            this.Controls.Add(startButton);

            // 賣掉與放進森林按鈕
            sellButton = new PictureBoxButton() { Size = new Size(125, 125), Visible = false };
            forestButton = new PictureBoxButton() { Text = "放進森林 10金幣/hr", Size = new Size(125, 125), Visible = false };
            sellButton.Click += SellTree;
            forestButton.Click += PutTreeInForest;
            this.Controls.Add(sellButton);
            this.Controls.Add(forestButton);

            // 重新開始與退出按鈕
            restartButton = new PictureBoxButton() { Text = "重新開始", Size = new Size(50, 50), Visible = false };
            restartButton.SetOverlayImage(Form1.restartImage);
            exitButton = new PictureBoxButton() { Text = "退出", Size = new Size(50, 50), Visible = false };
            exitButton.SetOverlayImage(Form1.homeImage);
            restartButton.Click += RestartTimer;
            exitButton.Click += (s, e) => this.Close();
            this.Controls.Add(restartButton);
            this.Controls.Add(exitButton);
        }

        private void TimerSlider_Scroll(object sender, EventArgs e)
        {
            timerLabel.Text = $"{timerSlider.Value:D2}:00"; // 更新大數字顯示
            timerSliderValue = timerSlider.Value;
        }

        private void InitializeFocusTimer()
        {
            focusTimer = new Timer() { Interval = 10 }; // 1秒更新一次
            focusTimer.Tick += FocusTimer_Tick;
        }

        private void InitializeBasicTree()
        {
            // 初始化一個基本的樹
            basicTree = Tree.CreateTree(TreeType.BasicTree);
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            if (selectedTree == null)
            {
                MessageBox.Show("尚未選擇任何種子！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            userInfo.UseSeed(selectedTree);
            timerDuration = timerSlider.Value * 60; // 使用滑桿選擇的分鐘數轉換為秒
            remainingTime = timerDuration;
            focusTimer.Start();
            startButton.Visible = false;
            timerSlider.Visible = false; // 禁用時間滑桿
            timerSliderLabel.Visible = false;
            leftButton.Visible = false; // 隱藏種子選擇按鈕
            rightButton.Visible = false;
            remainingGrowthLabel.Text = $"目前選擇的樹: {selectedTree.Name}，還需要 {selectedTree.RemainingTime} 分鐘才會長大\n目前每分鐘可以賺 {userInfo.moneyPerMinute} + {userInfo.boostPercentage}% 個金幣";
            if (!userInfo.TreesPlanting.Contains(selectedTree))
            {
                userInfo.TreesPlanting.Add(selectedTree); // 添加到種植中的樹
            }
            //UpdatePlantedTreeStatus();
        }

        private void FocusTimer_Tick(object sender, EventArgs e)
        {
            remainingTime -= 10;
            int minutes = remainingTime / 60;
            int seconds = remainingTime % 60;
            timerLabel.Text = $"{minutes:D2}:{seconds:D2}";
            remainingGrowthLabel.Text = $"{selectedTree.Name} 還需要 {Math.Max(selectedTree.RemainingTime - (timerDuration / 60 - minutes), 0)} 分鐘才會長大\n目前每分鐘可以賺 {userInfo.moneyPerMinute} + {userInfo.boostPercentage}% 個金幣";

            treeImageBox.Image = selectedTree.GetCurrentStageImage(Math.Max(selectedTree.RemainingTime - (timerDuration / 60 - minutes), 0));

            if (minuteCounter >= 60)
            {
                userInfo.UpdatePassiveIncome();
                moneyCountLabel.Text = "" + userInfo.Money;
                FocusTimerForm_Resize(null, null);
                minuteCounter = 0;
            }

            if (remainingTime <= 0)
            {
                focusTimer.Stop();
                selectedTree.RemainingTime -= timerDuration / 60;
                UpdateTreeCompletion();
            }
            minuteCounter+= 10;
        }

        private void UpdateTreeCompletion()
        {
            if (selectedTree.RemainingTime <= 0)
            {
                treeImageBox.Image = selectedTree?.GetCurrentStageImage(selectedTree.RemainingTime);
                sellButton.SetText($"賣掉\n+{selectedTree.SellPrice}金幣");
                sellButton.Visible = true;
                forestButton.SetText($"放進森林\n{selectedTree.ForestIncome}金幣/小時");
                forestButton.Visible = true;
            }
            else
            {
                treeImageBox.Image = selectedTree?.GetCurrentStageImage(selectedTree.RemainingTime);
                restartButton.Visible = true;
                exitButton.Visible = true;
            }
            userInfo.TotalFocusTime += timerSliderValue;
        }

        private void RestartTimer(object sender, EventArgs e)
        {
            // Stop the timer
            focusTimer.Stop();

            // Clear all controls and reset the form state
            this.Controls.Clear();
            remainingTime = timerDuration;

            // Reinitialize the form components
            InitializeInterface();
            InitializeFocusTimer();
            InitializeBasicTree();
            CheckTreeStatus();

            timerSlider.Value = timerSliderValue;
            FocusTimerForm_Resize(null, null);
        }


        private void SellTree(object sender, EventArgs e)
        {
            userInfo.Money += selectedTree.SellPrice;
            userInfo.TreesPlanting.Remove(selectedTree);
            MessageBox.Show("樹成功賣掉了", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        private void PutTreeInForest(object sender, EventArgs e)
        {
            if (selectedTree == null)
            {
                MessageBox.Show("沒有樹可放入森林！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Add the tree to the user's forest
            userInfo.PutTreeInForest(selectedTree);
            userInfo.TreesPlanting.Remove(selectedTree);

            // Add the tree visually to the forest panel in Form1
            AddTreeToForestPanelInForm1(selectedTree);

            // Reset selected tree and hide action buttons
            selectedTree = null;
            sellButton.Visible = false;
            forestButton.Visible = false;

            MessageBox.Show("樹已成功放入森林！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        private void AddTreeToForestPanelInForm1(Tree tree)
        {
            // Create a PictureBox for the tree
            PictureBox treePictureBox = new PictureBox
            {
                Image = tree.GrownTreeImage,
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(100, 100),
                BorderStyle = BorderStyle.None,
                BackColor = Color.Transparent
            };

            // Add a tooltip for tree details
            ToolTip treeToolTip = new ToolTip();
            treeToolTip.SetToolTip(treePictureBox, $"{tree.Name}\n收入: {tree.ForestIncome}/小時");

            // Add the PictureBox to the forest panel in Form1
            parentForm.ForestPanel.Controls.Add(treePictureBox);
        }


        private void ChangeSelectedSeed(int direction)
        {
            if (userInfo.TreesPlanting.Any())
            {
                // 若有未完成的樹則禁用選擇功能
                leftButton.Enabled = false;
                rightButton.Enabled = false;
                selectedTree = userInfo.TreesPlanting.First();
                UpdateTreeSelectionDisplay();
                return;
            }

            if (availableSeeds == null || !availableSeeds.Any())
            {
                selectedTree = basicTree; // 預設為基本樹
                leftButton.Enabled = false;
                rightButton.Enabled = false;
                UpdateTreeSelectionDisplay();
                return;
            }

            selectedSeedIndex = (selectedSeedIndex + direction + availableSeeds.Count) % availableSeeds.Count;
            UpdateTreeSelection();
        }

        private void UpdateTreeSelection()
        {
            if (availableSeeds != null && availableSeeds.Any())
            {
                selectedTree = availableSeeds[selectedSeedIndex];
            }
            else
            {
                selectedTree = basicTree; // 選擇基本樹
            }
            UpdateTreeSelectionDisplay();
        }

        private void UpdateTreeSelectionDisplay()
        {
            treeImageBox.Image = selectedTree?.GetCurrentStageImage(selectedTree.RemainingTime);
            remainingGrowthLabel.Text = $"目前選擇的樹: {selectedTree.Name}，還需要 {selectedTree.RemainingTime} 分鐘才會長大\n目前每分鐘可以賺 {userInfo.moneyPerMinute} + {userInfo.boostPercentage}% 個金幣";
        }

        private void CheckTreeStatus()
        {
            if (userInfo.TreesPlanting.Any())
            {
                // 若有未完成的樹，直接使用第一個未完成的樹
                selectedTree = userInfo.TreesPlanting.First();
                leftButton.Enabled = false;
                rightButton.Enabled = false;
                UpdateTreeSelectionDisplay();
                return;
            }

            // 將購買的產品中與種子相關的商品轉換為樹
            availableSeeds = userInfo.PurchasedProducts
                .Where(p => p.AssociatedTreeType.HasValue) // 確保是種子並有對應的樹類型
                .Select(p => Tree.CreateTree(p.AssociatedTreeType.Value)) // 根據樹類型創建樹
                .ToList();

            if (availableSeeds.Any())
            {
                selectedSeedIndex = 0; // 預設選擇第一顆種子
                UpdateTreeSelection();
            }
            else
            {
                // 如果沒有購買種子，則顯示基本樹
                selectedTree = basicTree;
                UpdateTreeSelectionDisplay();
            }
        }

        private void FocusTimerForm_Resize(object sender, EventArgs e)
        {
            ControlPositionHelper.PositionControl(restartButton, this, ControlPosition.BottomRight, 70 + exitButton.Width, 50);
            ControlPositionHelper.PositionControl(exitButton, this, ControlPosition.BottomRight, 50, 50);
            ControlPositionHelper.PositionControl(sellButton, this, ControlPosition.BottomRight, 70 + forestButton.Width, 50);
            ControlPositionHelper.PositionControl(forestButton, this, ControlPosition.BottomRight, 50, 50);
            ControlPositionHelper.PositionControl(startButton, this, ControlPosition.BottomRight, 50, 80);
            ControlPositionHelper.PositionControl(leftButton, this, ControlPosition.CenterLeft, 50, 0);
            ControlPositionHelper.PositionControl(rightButton, this, ControlPosition.CenterLeft, 50 + leftButton.Width + treeImageBox.Width, 0);
            ControlPositionHelper.PositionControl(timerSlider, this, ControlPosition.TopRight, 50, 330);
            ControlPositionHelper.PositionControl(timerSliderLabel, this, ControlPosition.TopRight, 50, 300);
            ControlPositionHelper.PositionControl(remainingGrowthLabel, this, ControlPosition.TopRight, 50, 180);
            ControlPositionHelper.PositionControl(timerLabel, this, ControlPosition.TopRight, 20, 50);
            ControlPositionHelper.PositionControl(treeImageBox, this, ControlPosition.CenterLeft, 100);
            ControlPositionHelper.PositionControl(remainingGrowthLabel, this, ControlPosition.TopRight, 50, 180);

            ControlPositionHelper.PositionControl(moneyCountLabel, this, ControlPosition.TopLeft, 70, 40);
            ControlPositionHelper.PositionControl(moneyPictureBox, this, ControlPosition.TopLeft, 40, 40);
        }

        public class CustomTrackBar : TrackBar
        {
            private const int WM_SETFOCUS = 0x0007;
            private const int WM_KILLFOCUS = 0x0008;

            protected override void WndProc(ref Message m)
            {
                // Suppress the drawing of the focus rectangle
                if (m.Msg == WM_SETFOCUS || m.Msg == WM_KILLFOCUS)
                {
                    return;
                }
                base.WndProc(ref m);
            }
        }

    }
}
