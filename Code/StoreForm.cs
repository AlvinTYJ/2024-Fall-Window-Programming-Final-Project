using Final_Project.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Final_Project
{
    
    public class StoreForm : Form
    {
        private Form1 parentForm;
        private CustomLabel moneyCountLabel;
        private FlowLayoutPanel itemsPanel;
        private PictureBoxButton backButton;
        private PictureBoxButton decorButton, seedsButton, forestButton;
        private PictureBox moneyPictureBox;
        private UserInfo userInfo;
        private Dictionary<string, List<Product>> categorizedProducts;
        private string currentCategory = "裝飾品";


        public StoreForm(UserInfo user, Form1 parent)
        {
            userInfo = user;
            InitializeStoreInterface();
            InitializeProducts();
            LoadStoreItems(currentCategory);
            this.Resize += StoreForm_Resize;
            parentForm = parent;
        }

        private void InitializeStoreInterface()
        {
            // 視窗設定
            this.Text = "商店介面";
            this.Size = new Size(1000, 600);
            this.BackColor = Form1.backgroundColor;

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
                Size = new Size(25,25),
                SizeMode = PictureBoxSizeMode.Zoom
            };
            this.Controls.Add(moneyPictureBox);

            // 返回按鈕
            backButton = new PictureBoxButton
            {
                Text = "返回",
                Font = new Font("Microsoft YaHei", 12),
                Size = new Size(40, 40)
            };
            backButton.SetOverlayImage(Form1.homeImage);
            backButton.Click += (s, e) => this.Close();
            this.Controls.Add(backButton);

            // 左側圓形按鈕
            decorButton = CreateCircularButton("裝飾品", 20, 150, "裝飾品");
            seedsButton = CreateCircularButton("種子", 20, 250, "種子");
            forestButton = CreateCircularButton("森林", 20, 350, "森林");

            //
            forestButton.Visible = false;
            //

            this.Controls.Add(decorButton);
            this.Controls.Add(seedsButton);
            this.Controls.Add(forestButton);

            // 商品展示區
            itemsPanel = new FlowLayoutPanel
            {
                AutoScroll = true,
                Size = new Size(this.ClientSize.Width - 200, this.ClientSize.Height - 80),
                Location = new Point(150, 60)
            };
            this.Controls.Add(itemsPanel);

            StoreForm_Resize(null, null); // 初始定位
        }

        private void InitializeProducts()
        {
            categorizedProducts = new Dictionary<string, List<Product>>
            {
                ["裝飾品"] = new List<Product>
                {
                    new Product("Daisy", 400, "清新自然的小花，為你的森林增添活力與色彩。\n持續性收入+10%", Form1.daisyImage, imageSize: 50, isDecor: true, boostPercentage: 10),
                    new Product("Windmill", 500, "旋轉的風車，為森林帶來動感與詩意。\n持續性收入+15%", Form1.windmillImage, imageSize: 50, isDecor: true, boostPercentage: 15),
                    new Product("Bench", 1000, "簡約舒適的長椅，打造溫馨的森林角落。\n持續性收入+50%",Form1.benchImage, imageSize: 50, isDecor : true, boostPercentage : 50)
                },
                ["種子"] = new List<Product>
                {
                    new Product("橡樹種子", 50, "一顆用來種植橡樹的種子", Tree.TreeImageLoader.BasicTreeSeed, TreeType.BasicTree),
                    new Product("楓樹種子", 100, "一顆用來種植楓樹的種子", Tree.TreeImageLoader.MapleTreeSeed, TreeType.MapleTree),
                    new Product("松樹種子", 150, "一顆用來種植松樹的種子", Tree.TreeImageLoader.PineTreeSeed, TreeType.PineTree),
                    new Product("櫻花樹種子", 200, "一顆用來種植櫻花樹的種子", Tree.TreeImageLoader.SakuraTreeSeed, TreeType.SakuraTree)
                },
                ["森林"] = new List<Product>
                {
                    new Product("森林套件 1", 300, "這是一個森林套件。"),
                    new Product("森林套件 2", 350, "另一個森林套件。"),
                    new Product("森林套件 3", 400, "更多森林內容。")
                }
            };
        }

        private void LoadStoreItems(string category)
        {
            itemsPanel.Controls.Clear();

            if (!categorizedProducts.ContainsKey(category)) return;

            foreach (var product in categorizedProducts[category])
            {
                Panel itemPanel = new Panel
                {
                    Size = new Size(itemsPanel.Width - 40, 120),
                    BorderStyle = BorderStyle.FixedSingle,
                    Margin = new Padding(10),
                    BackColor = ColorHelper.FromHEX("52675d")
                };

                // 商品圖片
                PictureBox itemImage = new PictureBox
                {
                    Size = new Size(product.ImageSize, product.ImageSize),
                    Image = product.ProductImage,
                    SizeMode = PictureBoxSizeMode.StretchImage,
                };
                ControlPositionHelper.PositionControl(itemImage, itemPanel, ControlPosition.CenterLeft, 60 - product.ImageSize/2);

                // 商品敘述
                CustomLabel itemDescription = new CustomLabel(10)
                {
                    Text = product.Description,
                    AutoSize = false,
                    Size = new Size(200, 80),
                    Location = new Point(120, 20),
                    ForeColor = Form1.invertTextColor,
                    TextAlign = ContentAlignment.MiddleLeft
                };

                // 價格按鈕
                PictureBoxButton priceButton = new PictureBoxButton
                {
                    Text = "$" + product.Price,
                    Font = new Font("Microsoft YaHei", 12),
                    Size = new Size(80, 80),
                    Location = new Point(itemPanel.Width - 100, 20),
                };
                priceButton.SetText("$" + product.Price);
                priceButton.Click += (s, e) => BuyItem(product);

                itemPanel.Controls.Add(itemImage);
                itemPanel.Controls.Add(itemDescription);
                itemPanel.Controls.Add(priceButton);

                itemsPanel.Controls.Add(itemPanel);
            }
        }

        private void BuyItem(Product product)
        {
            try
            {
                userInfo.BuyProduct(product);
                if(product.IsDecor)
                {
                    userInfo.Decorations.Add(product);
                    AddDecorationToForestPanelInForm1(product);
                    userInfo.UpdateBoostPercentage();
                }
                moneyCountLabel.Text = "" + userInfo.Money;
                MessageBox.Show($"成功購買 {product.Name}！剩餘金幣: {userInfo.Money}", "購買成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            StoreForm_Resize(null, null);
        }
        private void AddDecorationToForestPanelInForm1(Product product)
        {
            // Create a PictureBox for the tree
            PictureBox productPictureBox = new PictureBox
            {
                Image = product.ProductImage,
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(100, 100),
                BorderStyle = BorderStyle.None,
                BackColor = Color.Transparent
            };

            // Add a tooltip for tree details
            ToolTip treeToolTip = new ToolTip();
            treeToolTip.SetToolTip(productPictureBox, $"+{product.BoostPercentage}%");

            // Add the PictureBox to the forest panel in Form1
            parentForm.ForestPanel.Controls.Add(productPictureBox);
        }

        private PictureBoxButton CreateCircularButton(string text, int x, int y, string category)
        {
            PictureBoxButton button = new PictureBoxButton
            {
                Text = text,
                Font = new Font("Microsoft YaHei", 10),
                Size = new Size(80, 80),
                Location = new Point(x, y),
            };
            switch (text)
            {
                case "裝飾品":
                    button.SetOverlayImage(Form1.decorationImage);
                    break;
                case "種子":
                    button.SetOverlayImage(Form1.seedButtonImage);
                    break;
                case "森林":
                    button.SetOverlayImage(Form1.forestImage);
                    break;
            }
            button.Click += (s, e) => LoadStoreItems(category);
            return button;
        }

        private void StoreForm_Resize(object sender, EventArgs e)
        {
            itemsPanel.Size = new Size(this.ClientSize.Width - 200, this.ClientSize.Height - 80);
            itemsPanel.Location = new Point(150, 60);

            decorButton.Location = new Point(20, 150);
            seedsButton.Location = new Point(20, 250);
            forestButton.Location = new Point(20, 350);

            backButton.Location = new Point(40, 20);
            ControlPositionHelper.PositionControl(moneyCountLabel, this, ControlPosition.TopRight, 50, 20);
            ControlPositionHelper.PositionControl(moneyPictureBox, this, ControlPosition.TopRight, 20, 20);
        }
    }
}
