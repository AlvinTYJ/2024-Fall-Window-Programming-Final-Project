using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Drawing;
using System.IO;
using System.Reflection;
using Final_Project.Properties;

namespace Final_Project
{
    public class UserInfo
    {
        public int Money { get; set; }
        public int TotalFocusTime { get; set; }
        public List<Product> PurchasedProducts { get; set; }

        public List<Tree> PurchasedSeeds { get; set; }
        public List<string> ToDoList { get; set; }
        public List<string> WordCards { get; set; }
        public List<Tree> TreesPlanting { get; set; }
        public List<Product> Decorations { get; set; }

        public int boostPercentage = 0;

        private List<Tree> plantedTrees;
        public float moneyPerMinute = 0;
        public float moneyAdder = 0;

        public UserInfo(int initialMoney)
        {
            Money = initialMoney;
            TotalFocusTime = 0;
            PurchasedProducts = new List<Product>();
            ToDoList = new List<string>();
            WordCards = new List<string>();
            plantedTrees = new List<Tree>();
            TreesPlanting = new List<Tree>();
            Decorations = new List<Product>();
        }

        public void BuyProduct(Product product)
        {
            if (Money >= product.Price)
            {
                Money -= product.Price;
                PurchasedProducts.Add(product);
            }
            else
            {
                throw new InvalidOperationException("金幣不足！");
            }
        }

        public void UseSeed(Tree tree)
        {
            foreach (Product product in PurchasedProducts)
            {
                if(tree.Type == product.AssociatedTreeType)
                {
                    PurchasedProducts.Remove(product);
                    return;
                }
            }
        }

        public void UpdateBoostPercentage()
        {
            boostPercentage = 0;
            foreach (Product product in Decorations)
            {
                boostPercentage += product.BoostPercentage;
            }
        }

        public void UpdatePassiveIncome()
        {
            float totalMoneyToAdd = moneyPerMinute * (1 + boostPercentage/100) ;
            Money += (int)totalMoneyToAdd;
            moneyAdder += totalMoneyToAdd - (int)totalMoneyToAdd;
            if (moneyAdder > 1)
            {
                Money += (int)moneyAdder;
                moneyAdder--;
            }
        }

        public void AddFocusTime(int minutes)
        {
            TotalFocusTime += minutes;
        }

        public void AddToDo(string task)
        {
            ToDoList.Add(task);
        }

        public void AddWordCard(string word)
        {
            WordCards.Add(word);
        }

        public void PlantTree(Tree tree)
        {
            TreesPlanting.Add(tree);
        }

        public void PutTreeInForest(Tree tree)
        {
            plantedTrees.Add(tree);
            moneyPerMinute += (float)tree.ForestIncome/60;
        }

        public void UpdateTreeGrowth(string treeName, int minutesReduced)
        {
            var tree = TreesPlanting.Find(t => t.Name == treeName);
            if (tree != null)
            {
                tree.RemainingTime -= minutesReduced;
                if (tree.RemainingTime <= 0)
                {
                    TreesPlanting.Remove(tree);
                }
            }
        }
    }

    public enum TreeType
    {
        BasicTree,
        MapleTree,
        PineTree,
        SakuraTree
    }

    public class Tree
    {
        public TreeType Type { get; set; }
        public string Name { get; set; }
        public int RemainingTime { get; set; }
        public int GrowthTime { get; set; }
        public Image SeedImage { get; set; }
        public Image SproutImage { get; set; }
        public Image SaplingImage { get; set; }
        public Image GrownTreeImage { get; set; }
        public int SellPrice { get; set; } // 賣掉的金額
        public int GrowthRewardRate { get; set; } // 成長超過時間後，每分鐘的金幣收益
        public int ForestIncome { get; set; } // 放進森林每小時賺取的金額
        public bool IsBasic { get; set; }

        public Tree(TreeType type, string name, int growthTime, int sellPrice, int growthRewardRate, int forestIncome, Image seedImage, Image sproutImage, Image saplingImage, Image grownTreeImage, bool isBasic = false)
        {
            Type = type;
            Name = name;
            GrowthTime = growthTime;
            RemainingTime = growthTime;
            SellPrice = sellPrice;
            GrowthRewardRate = growthRewardRate;
            ForestIncome = forestIncome;
            SeedImage = seedImage;
            SproutImage = sproutImage;
            SaplingImage = saplingImage;
            GrownTreeImage = grownTreeImage;
            IsBasic = isBasic;
        }

        public Image GetCurrentStageImage(int remainingTime)
        {
            if (remainingTime == GrowthTime)
                return SeedImage;
            else if (remainingTime > GrowthTime * 0.7)
                return TreeImageLoader.Dirt;
            else if (remainingTime > GrowthTime * 0.33)
                return SproutImage;
            else if (remainingTime > GrowthTime * 0)
                return SaplingImage;
            else
                return GrownTreeImage;
        }
        public static Tree CreateTree(TreeType type)
        {

            switch (type)
            {
                case TreeType.BasicTree:
                    return new Tree(type, "橡樹", 30, 50, 10, 30,
                        TreeImageLoader.BasicTreeSeed,
                        TreeImageLoader.BasicTreeSprout,
                        TreeImageLoader.BasicTreeSapling,
                        TreeImageLoader.BasicTreeGrownInDirt, false);
                case TreeType.MapleTree:
                    return new Tree(type, "楓樹", 60, 100, 15, 70,
                        TreeImageLoader.MapleTreeSeed,
                        TreeImageLoader.MapleTreeSprout,
                        TreeImageLoader.MapleTreeSapling,
                        TreeImageLoader.MapleTreeGrownInDirt, false);
                case TreeType.PineTree:
                    return new Tree(type, "松樹", 90, 300, 20, 60,
                        TreeImageLoader.PineTreeSeed,
                        TreeImageLoader.PineTreeSprout,
                        TreeImageLoader.PineTreeSapling,
                        TreeImageLoader.PineTreeGrownInDirt, false);
                case TreeType.SakuraTree:
                    return new Tree(type, "櫻花樹", 90, 200, 25, 100,
                        TreeImageLoader.SakuraTreeSeed,
                        TreeImageLoader.SakuraTreeSprout,
                        TreeImageLoader.SakuraTreeSapling,
                        TreeImageLoader.SakuraTreeGrownInDirt,
                        false);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), "Invalid tree type");
            }
        }

        public static class TreeImageLoader
        {
            // Helper method to load embedded images
            private static Image LoadEmbeddedImage(string resourceName)
            {
                var assembly = Assembly.GetExecutingAssembly();
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream != null)
                    {
                        return Image.FromStream(stream);
                    }
                    else
                    {
                        throw new Exception($"Resource not found: {resourceName}");
                    }
                }
            }

            // Methods to load images for each tree type and stage
            public static Image Dirt => LoadEmbeddedImage("Final_Project.Resources.dirt.png");

            public static Image BasicTreeSeed => LoadEmbeddedImage("Final_Project.Resources.oak_tree_seed.png");
            public static Image BasicTreeSprout => LoadEmbeddedImage("Final_Project.Resources.sprout.png");
            public static Image BasicTreeSapling => LoadEmbeddedImage("Final_Project.Resources.sapling.png");
            public static Image BasicTreeGrownInDirt => LoadEmbeddedImage("Final_Project.Resources.oak.png");

            public static Image MapleTreeSeed => LoadEmbeddedImage("Final_Project.Resources.maple_tree_seed.png");
            public static Image MapleTreeSprout => LoadEmbeddedImage("Final_Project.Resources.sprout.png");
            public static Image MapleTreeSapling => LoadEmbeddedImage("Final_Project.Resources.sapling.png");
            public static Image MapleTreeGrown => LoadEmbeddedImage("Final_Project.Resources.maple_tree_grown.png");
            public static Image MapleTreeGrownInDirt => LoadEmbeddedImage("Final_Project.Resources.maple.png");

            public static Image PineTreeSeed => LoadEmbeddedImage("Final_Project.Resources.pine_tree_seed.png");
            public static Image PineTreeSprout => LoadEmbeddedImage("Final_Project.Resources.sprout.png");
            public static Image PineTreeSapling => LoadEmbeddedImage("Final_Project.Resources.sapling.png");
            public static Image PineTreeGrown => LoadEmbeddedImage("Final_Project.Resources.pine_tree_grown.png");
            public static Image PineTreeGrownInDirt => LoadEmbeddedImage("Final_Project.Resources.pine.png");

            public static Image SakuraTreeSeed => LoadEmbeddedImage("Final_Project.Resources.sakura_tree_seed.png");
            public static Image SakuraTreeSprout => LoadEmbeddedImage("Final_Project.Resources.sprout.png");
            public static Image SakuraTreeSapling => LoadEmbeddedImage("Final_Project.Resources.sapling.png");
            public static Image SakuraTreeGrown => LoadEmbeddedImage("Final_Project.Resources.sakura_grown.png");
            public static Image SakuraTreeGrownInDirt => LoadEmbeddedImage("Final_Project.Resources.sakura.png");
        }
    }
    public class Forest
    {
        public string Name { get; set; }

        public Forest(string name)
        {
            Name = name;
        }
    }

    public class Product
    {
        public string Name { get; set; }
        public int Price { get; set; }
        public string Description { get; set; }
        public Image ProductImage { get; set; }
        public TreeType? AssociatedTreeType { get; set; } // Optional, only for seeds

        public bool IsDecor;

        public int BoostPercentage { get; set; }

        public int ImageSize { get; set; }

        public Product(string name, int price, string description, Image productImage = null, TreeType? associatedTreeType = null, int imageSize = 100, bool isDecor = false, int boostPercentage = 0)
        {
            Name = name;
            Price = price;
            Description = description;
            ProductImage = productImage ?? new Bitmap(100, 100);
            AssociatedTreeType = associatedTreeType;
            ImageSize = imageSize;
            IsDecor = isDecor;
            BoostPercentage = boostPercentage;
        }
    }

}
