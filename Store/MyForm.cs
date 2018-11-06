using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Threading;

namespace Store
{
    class Product
    {
        public string Serialnumber;
        public string Name;
        public string Description;
        public double Price;
    }
    class CartInfo : Product
    {
        private int cartQuantity = 0;
        public int CartQuantity
        {
            get
            {
                return cartQuantity;
            }
            set
            {
                if (value >= 0)
                {
                    cartQuantity = value;
                }
            }
        }
        public bool BeenClicked = false;
        public Label cartProduct = new Label
        {
            TextAlign = ContentAlignment.MiddleLeft,
            //Dock = DockStyle.Fill,
            Text = ""
        };
        public Button subtractQuantity = new Button
        {
            Text = "-",
            //Dock = DockStyle.Fill,
        };
        public Label quantity = new Label
        {
            //Dock = DockStyle.Fill,
            Text = "0",
            TextAlign = ContentAlignment.MiddleCenter,
        };
        public Button addQuantity = new Button
        {
            //Dock = DockStyle.Fill,
            Text = "+"
        };
        public Label productPriceCart = new Label
        {
            TextAlign = ContentAlignment.MiddleRight,
            //Dock = DockStyle.Fill,
            Text = " SEK"
        };
        public Label sumProduct = new Label
        {
            TextAlign = ContentAlignment.MiddleRight,
            //Dock = DockStyle.Fill,
            Text = ""
        };
        public TableLayoutPanel articlePanel;

        public TableLayoutPanel ArticlePanel()
        {
            articlePanel = new TableLayoutPanel
            {
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single,
                ColumnCount = 6,
                RowCount = 1,
                Dock = DockStyle.Top,
                BackColor = Color.Aqua,
                Height = 30
            };
            articlePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10));
            articlePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 3));
            articlePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 3));
            articlePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 3));
            articlePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10));
            articlePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10));

            cartProduct.Text = Name;
            productPriceCart.Text = Price + " SEK";
            sumProduct.Text = (CartQuantity * Price).ToString() + " SEK";

            articlePanel.Controls.Add(cartProduct);
            articlePanel.Controls.Add(subtractQuantity);
            articlePanel.Controls.Add(quantity);
            articlePanel.Controls.Add(addQuantity);
            articlePanel.Controls.Add(productPriceCart);
            articlePanel.Controls.Add(sumProduct);

            articlePanel.Tag = this;

            return articlePanel;
        }
    }
    class Discount
    {
        public string code;
        public string description;
        public double discount;
        public bool percent;
    }
    class Cart
    {
        public Dictionary<string, int> cart = new Dictionary<string, int>();

        public void ReadToCVSFile()
        {
            string[] cartString = new string[cart.Count()];
            int i = 0;
            foreach (KeyValuePair<string, int> pair in cart)
            {
                cartString[i] = pair.Key + ", " + pair.Value.ToString();
                i++;
            }
            File.WriteAllLines("Cart.csv", cartString, Encoding.UTF8);
        }
    }
    class MyForm : Form
    {
        #region instance variables
        TableLayoutPanel basePanel;
        Label storeName;
        TableLayoutPanel productPanel;
        Label productsText;
        TableLayoutPanel descriptionPanel;
        Label productName;
        Label productDescription;
        Label productPrice;
        Button addToCart;
        public TableLayoutPanel checkoutPanel;
        Label cart;
        TableLayoutPanel articleListHeadingPanel;
        Label nameHeading;
        Label quantityHeading;
        Label aPriceHeading;
        Label totPriceHeading;
        TableLayoutPanel articleListPanel;
        TableLayoutPanel articlePanel;
        TableLayoutPanel discountPanel;
        Label discountLabel;
        Label discountDescription;
        Label sumDiscountCart;
        TextBox discountText;
        Label totCart;
        TableLayoutPanel cartButtonPanel;
        Button emptyCart;
        Button saveCart;
        Button pay;        

        CartInfo emptyCartInfo;
        Product[] products;
        Discount[] discounts;
        CartInfo[] cartInfos;

        #endregion
        public MyForm()
        {
            MinimumSize = new Size(750, 500);
            emptyCartInfo = new CartInfo();

            #region Tables
            basePanel = BasePanel();
            productPanel = ProductPanel();
            descriptionPanel = DescriptionPanel(emptyCartInfo);
            checkoutPanel = CheckoutPanel();
            articleListHeadingPanel = ArticleListHeadingPanel();
            articleListPanel = ArticleListPanel();
            products = ReadCSVFileProducts("TextFile1.csv");
            discounts = ReadCSVFileDiscounts("discounts.csv");
            cartInfos = MakeArticlePanels();
            discountPanel = DiscountPanel();
            SumAndDiscountCart();
            cartButtonPanel = CartButtonPanel();
            #endregion

            #region Things to put in the panels
            storeName = new Label
            {
                Text = "Fantastic Beasts, here you'll find them",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.TopCenter,
            };
            productsText = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "Products"
            };
            cart = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "Your Cart",
                BorderStyle = BorderStyle.FixedSingle,
            };
            nameHeading = new Label
            {
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "Product"
            };
            quantityHeading = new Label
            {
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "Quantity"
            };
            aPriceHeading = new Label
            {
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "á Price"
            };
            totPriceHeading = new Label
            {
                Text = "Total Price",
                TextAlign = ContentAlignment.MiddleCenter
            };
            #endregion

            #region Panel Layouts
            Controls.Add(basePanel);
            basePanel.Controls.Add(storeName);
            basePanel.SetColumnSpan(storeName, 3);

            basePanel.Controls.Add(productPanel);
            productPanel.Controls.Add(productsText);
            MakeProductButtons();

            basePanel.Controls.Add(descriptionPanel, 1, 1);
            descriptionPanel.Controls.Add(productName);
            descriptionPanel.Controls.Add(productDescription);
            descriptionPanel.Controls.Add(productPrice);
            descriptionPanel.Controls.Add(addToCart);

            basePanel.Controls.Add(checkoutPanel, 2, 1);
            checkoutPanel.Controls.Add(cart);
            checkoutPanel.Controls.Add(articleListHeadingPanel);
            articleListHeadingPanel.Controls.Add(nameHeading);
            articleListHeadingPanel.Controls.Add(quantityHeading);
            articleListHeadingPanel.Controls.Add(aPriceHeading);
            articleListHeadingPanel.Controls.Add(totPriceHeading);

            checkoutPanel.Controls.Add(articleListPanel);
            checkoutPanel.Controls.Add(discountPanel);
            checkoutPanel.Controls.Add(cartButtonPanel);
            cartButtonPanel.Controls.Add(emptyCart);
            cartButtonPanel.Controls.Add(saveCart);
            cartButtonPanel.Controls.Add(pay);
            #endregion
        }
        #region Panel Methods
        private TableLayoutPanel BasePanel()
        {
            basePanel = new TableLayoutPanel
            {
                RowCount = 2,
                ColumnCount = 3,
                Dock = DockStyle.Fill,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single
            };
            basePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
            basePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 200));
            basePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 400));
            return basePanel;
        }

        private TableLayoutPanel ProductPanel()
        {
            productPanel = new TableLayoutPanel
            {
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single,
                Dock = DockStyle.Fill,
                AutoScroll = true,
            };
            // Följande 4 rader text hittades på https://stackoverflow.com/questions/5489273/how-do-i-disable-the-horizontal-scrollbar-in-a-panel, och med lite modifiering funkade det som önskat.
            productPanel.HorizontalScroll.Maximum = 0;
            productPanel.AutoScroll = false;
            productPanel.HorizontalScroll.Visible = false;
            productPanel.AutoScroll = true;
            return productPanel;
        }

        private TableLayoutPanel DescriptionPanel(CartInfo cartInfo)
        {
            descriptionPanel = new TableLayoutPanel
            {
                RowCount = 4,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single,
                Dock = DockStyle.Fill
            };
            productName = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = cartInfo.Name
            };
            descriptionPanel.Controls.Add(productName);
            productDescription = new Label
            {
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.Fill,
                Text = cartInfo.Description
            };
            descriptionPanel.Controls.Add(productDescription);
            productPrice = new Label
            {
                TextAlign = ContentAlignment.MiddleRight,
                Dock = DockStyle.Fill,
                Text = cartInfo.Price + " SEK"
            };
            descriptionPanel.Controls.Add(productPrice);

            addToCart = new Button
            {
                Dock = DockStyle.Fill,
                Text = "Add To Cart"
            };
            descriptionPanel.Controls.Add(addToCart);

            addToCart.Tag = cartInfo;
            if (cartInfo != emptyCartInfo)
            {
                addToCart.Click += AddToCartClick;
            }

            descriptionPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 35));
            descriptionPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            descriptionPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 35));
            descriptionPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 35));
            return descriptionPanel;
        }

        private TableLayoutPanel CheckoutPanel()
        {
            checkoutPanel = new TableLayoutPanel
            {
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single,
                RowCount = 5,
                Dock = DockStyle.Fill
            };
            checkoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            checkoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            checkoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            checkoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));
            checkoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 35));
            return checkoutPanel;
        }

        private TableLayoutPanel ArticleListHeadingPanel()
        {
            articleListHeadingPanel = new TableLayoutPanel
            {
                ColumnCount = 4,
                Dock = DockStyle.Fill
            };
            articleListHeadingPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10));
            articleListHeadingPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 9));
            articleListHeadingPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10));
            articleListHeadingPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10));

            return articleListHeadingPanel;
        }

        private TableLayoutPanel ArticleListPanel()
        {
            articleListPanel = new TableLayoutPanel
            {
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single,
                ColumnCount = 1,
                RowCount = 1,
                Dock = DockStyle.Fill,
                AutoScroll = true
            };
            articleListPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 35));
            return articleListPanel;
        }
    
        private TableLayoutPanel DiscountPanel()
        {
            discountPanel = new TableLayoutPanel
            {
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single,
                ColumnCount = 4,
                RowCount = 2,
                Dock = DockStyle.Bottom
            };
            discountPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
            discountPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
            discountPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 80));
            discountPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
            discountPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            discountPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));

            discountLabel = new Label
            {
                Dock = DockStyle.Fill,
                Text = "Discount code",
                TextAlign = ContentAlignment.MiddleLeft
            };
            discountText = new TextBox
            {
                Dock = DockStyle.Fill,
            };
            discountText.KeyDown += new KeyEventHandler(DiscountTextEnter);

            discountDescription = new Label
            {
                Dock = DockStyle.Fill,
                Text = "",
                TextAlign = ContentAlignment.MiddleRight
            };
            sumDiscountCart = new Label
            {
                Dock = DockStyle.Fill,
                Text = "",
                TextAlign = ContentAlignment.MiddleRight
            };
            Label sumText = new Label
            {
                Text = "Sum",
                TextAlign = ContentAlignment.MiddleRight
            };
            totCart = new Label
            {
                TextAlign = ContentAlignment.MiddleRight,
                Text = "0 SEK",
            };
            SumAndDiscountCart();

            discountPanel.Controls.Add(discountLabel);
            discountPanel.Controls.Add(discountText);
            discountPanel.Controls.Add(discountDescription);
            discountPanel.Controls.Add(sumDiscountCart);
            discountPanel.Controls.Add(sumText, 2, 1);
            discountPanel.Controls.Add(totCart, 3, 1);

            return discountPanel;
        }

        private TableLayoutPanel CartButtonPanel()
        {
            cartButtonPanel = new TableLayoutPanel
            {
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single,
                ColumnCount = 3,
                Dock = DockStyle.Bottom,
                Height = 30
            };
            cartButtonPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 1));
            cartButtonPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 1));
            cartButtonPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 1));
            emptyCart = new Button
            {
                Dock = DockStyle.Fill,
                Text = "Empty Cart"
            };
            emptyCart.Click += EmptyCartClick;
            saveCart = new Button
            {
                Dock = DockStyle.Fill,
                Text = "Save Cart"
            };
            saveCart.Click += SaveCartClick;
            pay = new Button
            {
                Dock = DockStyle.Fill,
                Text = "Pay"
            };
            pay.Click += PayClick;
            return cartButtonPanel;
        }
        #endregion

        #region Enstaka klassmetoder
        private void MakeProductButtons()
        {
            foreach (CartInfo c in cartInfos)
            {
                Button button = new Button
                {
                    Text = c.Name,
                    Dock = DockStyle.Top,
                    Height = 30,
                };
                productPanel.Controls.Add(button);
                button.Tag = c;
                button.Click += ProductButtonClick;
            }
        }

        private Product[] ReadCSVFileProducts(string fileName)
        {
            string[] inventoryString = File.ReadAllLines(fileName);
            Product[] products = new Product[inventoryString.Length];
            int i = 0;
            foreach (string line in inventoryString)
            {
                Product product = new Product { };
                string[] words = line.Split(new char[] { '|' });
                product.Serialnumber = words[0].Trim(' ');
                product.Name = words[1].Trim(' ');
                product.Description = words[2].Trim(' ');
                product.Price = double.Parse(words[3].Trim(' '));
                products[i] = product;
                i++;
            }
            return products;
        }


        private Discount[] ReadCSVFileDiscounts(string fileName)
        {
            {
                string[] discountString = File.ReadAllLines(fileName);
                Discount[] discounts = new Discount[discountString.Length];
                int i = 0;
                foreach (string line in discountString)
                {
                    Discount discount = new Discount { };
                    string[] words = line.Split(new char[] { ',' });
                    discount.code = words[0].Trim(' ');
                    discount.description = words[1].Trim(' ');
                    discount.discount = double.Parse(words[2].Trim(' '));
                    discount.percent = bool.Parse(words[3].Trim(' '));
                    discounts[i] = discount;
                    i++;
                }
                return discounts;
            }
        }

        private CartInfo[] MakeArticlePanels()
        {
            int i = 0;
            CartInfo[] cartInfos = new CartInfo[products.Length];
            foreach (Product p in products)
            {
                CartInfo cartInfo = new CartInfo();
                cartInfo.Serialnumber = p.Serialnumber;
                cartInfo.Name = p.Name;
                cartInfo.Description = p.Description;
                cartInfo.Price = p.Price;
                cartInfos[i] = cartInfo;
                i++;
            }
            return cartInfos;
        }

        public void SumAndDiscountCart()
        {
            bool validDiscount = false;
            double sum = 0;
            foreach (CartInfo c in cartInfos)
            {
                sum += c.Price * c.CartQuantity;
            }
            double totDiscount = 0;

            if (discountText.Text == "")
            {
                totDiscount = 0;
                validDiscount = true;
            }
            else
            {
                foreach (Discount d in discounts)
                {
                    if (discountText.Text == d.code)
                    {
                        discountDescription.Text = d.description;
                        if (d.percent)
                        {
                            totDiscount = sum * d.discount / 100;
                        }
                        else
                        {
                            totDiscount = d.discount;
                        }
                        validDiscount = true;
                    }
                }
            }
            if (!validDiscount)
            {
                discountDescription.Text = "";
                totDiscount = 0;
                discountText.Text = "";
                MessageBox.Show("Ej giltig kod");
            }
            sumDiscountCart.Text = "- " + totDiscount + " SEK";
            sum -= totDiscount;
            if (sum >= 0)
            {
                totCart.Text = sum + " SEK";
            }
            else
            {
                totCart.Text = "0 SEK";
            }
        }
        #endregion

        #region Events
        private void ProductButtonClick(object sender, EventArgs e)
        {
            basePanel.Controls.Remove(descriptionPanel);
            Button button = (Button)sender;
            CartInfo cartInfo = (CartInfo)button.Tag;
            descriptionPanel = DescriptionPanel(cartInfo);
            basePanel.Controls.Add(descriptionPanel, 1, 1);
        }

        private void AddToCartClick(object sender, EventArgs e)
        {

            Button button = (Button)sender;
            CartInfo cartInfo = (CartInfo)button.Tag;
            cartInfo.CartQuantity++;
            cartInfo.quantity.Text = cartInfo.CartQuantity.ToString();
            if (cartInfo.CartQuantity == 1)
            {
                articlePanel = cartInfo.ArticlePanel();
                articleListPanel.Controls.Add(articlePanel);
                cartInfo.subtractQuantity.Tag = cartInfo;
                cartInfo.addQuantity.Tag = cartInfo;
            }
            else
            {
                cartInfo.sumProduct.Text = (cartInfo.CartQuantity * cartInfo.Price).ToString() + " SEK";
                cartInfo.subtractQuantity.KeyDown += new KeyEventHandler(DiscountTextEnter);
                cartInfo.addQuantity.KeyDown += new KeyEventHandler(DiscountTextEnter);
            }
            if (!cartInfo.BeenClicked)
            {
                cartInfo.subtractQuantity.Click += SubtractQuantityClick;
                cartInfo.addQuantity.Click += AddQuantityClick;
            }
            cartInfo.BeenClicked = true;
            SumAndDiscountCart();
        }

        private void AddQuantityClick(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            CartInfo cartInfo = (CartInfo)button.Tag;
            cartInfo.CartQuantity++;
            cartInfo.quantity.Text = cartInfo.CartQuantity.ToString();
            cartInfo.sumProduct.Text = (cartInfo.CartQuantity * cartInfo.Price).ToString() + " SEK";
            SumAndDiscountCart();
        }

        private void SubtractQuantityClick(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            CartInfo cartInfo = (CartInfo)button.Tag;

            if (cartInfo.CartQuantity > 0)
            {
                cartInfo.CartQuantity--;
                cartInfo.quantity.Text = cartInfo.CartQuantity.ToString();
                cartInfo.sumProduct.Text = cartInfo.CartQuantity * cartInfo.Price + " SEK";
            }
            if (cartInfo.CartQuantity <= 0)
            {
                articleListPanel.Controls.Remove(cartInfo.articlePanel);
            }
            SumAndDiscountCart();
        }

        private void DiscountTextEnter(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SumAndDiscountCart();
            }
        }

        private void SaveCartClick(object sender, EventArgs e)
        {
            Cart car = new Cart { };
            foreach (CartInfo c in cartInfos)
            {
                if (c.CartQuantity != 0)
                {
                    car.cart[c.Serialnumber] = c.CartQuantity;
                }
            }
            car.ReadToCVSFile();
        } 

        
        private void EmptyCartClick(object sender, EventArgs e)
        {
            foreach (CartInfo c in cartInfos)
            {
                c.CartQuantity = 0;
                c.quantity.Text = c.CartQuantity.ToString();
                articleListPanel.Controls.Remove(c.articlePanel);
            }
        }

        private void PayClick(object sender, EventArgs e)
        {
            SumAndDiscountCart();
            string reciept = "";
            foreach(CartInfo c in cartInfos)
            {
                if (c.CartQuantity == 0)
                {
                }
                else if (c.CartQuantity == 1)
                {
                    string inventory = c.Name + "             " + c.Price + " SEK";
                    reciept += "\n" + inventory;
                }
                else
                {
                    string inventory = c.Name + "      " + c.CartQuantity + "*" + c.Price + "   " + c.CartQuantity * c.Price + " SEK";
                    reciept += "\n" + inventory;
                }
            }
            string disc = "Rabatt                           " + sumDiscountCart.Text;
            reciept += "\n" + disc;

            string total = "                      Summa: " + totCart.Text;
            reciept += "\n" + total;
            MessageBox.Show(reciept, "Kvitto");
        }
        #endregion
    }
}


//Felhantering(lektion 10)
//Funktionell programmering(lektion 11)
//Polymorfism(lektion 22)


// static - klassmetod, EN GÅNG
// ej static - instansmetod, en gång per instans.

    // Dela data
// 1. Metodanrop
// 2. Instansvariabler
// 3. Klassvariabler


// viktiga klasser/ metoder
// Vilken data finns
// Hur delas data

    // Man kan dela beteende med LINQ/lambdauttryck. Lägga till rabatt/summa??
    // Action MetodNamn = () =>
    // {
    //     MeeasgeBox("NÅT");
    // };


    // ctrl + r, ctrl + r