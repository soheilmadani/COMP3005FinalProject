using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Look_Inna_Book
{
    public partial class UserApp : Form
    {
        private MySqlConnection connection;
        private string server;
        private string database;
        private string uid;
        private string password;

        List<Book> searched_books = new List<Book>();

        UserAccount account = null;
        public UserApp()
        {
            InitializeComponent();
            cart = new Cart();
            ClearInterface();
            loginPanel.Visible = true;
            GetSQLConnection();
            UpdateListview();

        }

        void GetSQLConnection()
        {
            //read the connection string from a file and open the database
            using (StreamReader reader = new StreamReader("connectionstring.txt"))
            {
                connection = new MySqlConnection(reader.ReadToEnd());
            }
        }

        //open connection to database
        private bool OpenConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                switch (ex.Number)
                {
                    case 0:
                        MessageBox.Show("Cannot connect to server.  Contact administrator");
                        break;

                    case 1045:
                        MessageBox.Show("Invalid username/password, please try again");
                        break;
                }
                return false;
            }
        }

        private bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            textBox2.UseSystemPasswordChar = !textBox2.UseSystemPasswordChar;
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            Search();
        }

        private void Search()
        {
            string search = textBox3.Text;
            string query = "";
            if (string.IsNullOrEmpty(search))
                query = "SELECT * FROM Books NATURAL JOIN Genres";
            else
            {
                if (titleRadio.Checked)
                    query = $"SELECT * FROM Books NATURAL JOIN Genres WHERE Books.Title =\"{search}\";";
                else if (authorRadio.Checked)
                    query = $"SELECT * FROM Books NATURAL JOIN Genres WHERE Author=\"{search}\";";
                else if (ISBNRadio.Checked)
                    query = $"SELECT * FROM Books NATURAL JOIN Genres WHERE ISBN=\"{search}\";";
            }

            OpenConnection();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            var reader = cmd.ExecuteReader();
            searched_books = new List<Book>();
            while (reader.Read())
            {
                Book book = new Book();
                book.ISBN = reader["ISBN"] + "";
                book.Pages = int.Parse(reader["Pages"] + "");
                book.Stock = int.Parse(reader["Stock"] + "");
                book.Title = reader["Title"] + "";
                book.Author = reader["Author"] + "";
                book.Publisher = (reader["Publisher"] + "");
                book.GenreID = int.Parse(reader["genre_id"] + "");
                book.Genre = (reader["GenreName"] + "");
                book.Price = decimal.Parse(reader["Price"] + "");
                book.Format = (Format)int.Parse(reader["Format"] + "");
                searched_books.Add(book);
            }
            reader.Close();
            CloseConnection();

            UpdateListview();
        }

        void UpdateBookPreview()
        {
            if (currentBook != null)
            {
                previewPanel.Visible = true;
                label4.Text = currentBook?.Title;
                authorLabel.Text = "by " + currentBook?.Author;
                ISBNLabel.Text = "ISBN: " + currentBook?.ISBN;
                stockLabel.Text = currentBook?.Stock + " in stock";
                pagesLabel.Text = currentBook?.Pages + " pages";
                genreLabel.Text = currentBook?.Genre;
                publisherLabel.Text = "published by " + currentBook?.Publisher;
                priceLabel.Text = "$" + currentBook?.Price.ToString();
                desiredNumberOfCopies.Maximum = (decimal)currentBook?.Stock;
                desiredNumberOfCopies.Value = desiredNumberOfCopies.Maximum > 0 ? 1 : 0;
            }
            else
            {
                previewPanel.Visible = false;
            }
        }

        private void textBox3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                Search();

        }

        private void registerLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ClearInterface();
            registrationPanel.Visible = true;
            billingPanel.Visible = true;
            billingPanel.BringToFront();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        Cart cart;
        struct LineItem
        {
            public Book Book;
            public int Quantity;
        }

        class Cart
        {
            public Dictionary<string, LineItem> items = new Dictionary<string, LineItem>();

            public Cart()
            {

            }

            public decimal GetTotalPrice()
            {
                decimal total = 0;
                foreach (var item in items)
                {
                    total += item.Value.Book.Price * item.Value.Quantity;
                }

                return total;
            }

            public int GetTotalQuantity()
            {
                int total = 0;
                foreach (var item in items)
                {
                    total += item.Value.Quantity;
                }

                return total;
            }

            public bool IsEmpty()
            {
                return items.Count == 0;
            }

            internal void Clear()
            {
                items.Clear();
            }
        }

        Book currentBook;
        private void addToCartButton_Click(object sender, EventArgs e)
        {
            if (currentBook != null)
            {
                Book b = currentBook ?? new Book();
                if (cart.items.ContainsKey(b.ISBN))
                {
                    LineItem lineItem = cart.items[b.ISBN];
                    lineItem.Quantity += (int)desiredNumberOfCopies.Value;
                    if (lineItem.Quantity > b.Stock)
                    {
                        lineItem.Quantity = b.Stock;
                        MessageBox.Show("Cannot add more copies than we have in stock");
                    }

                    cart.items[b.ISBN] = lineItem;
                }
                else
                {
                    cart.items.Add(b.ISBN, new LineItem() { Book = b, Quantity = (int)desiredNumberOfCopies.Value });
                }

                cartCountLabel.Text = cart.GetTotalQuantity().ToString();
            }
        }

        private void listView1_Click(object sender, EventArgs e)
        {
            int i = listView1.SelectedIndices[0];
            currentBook = null;
            if (i >= 0)
                currentBook = searched_books[i];
            UpdateBookPreview();
        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            OpenConnection();
            string query = $"SELECT * FROM users NATURAL JOIN billing_info WHERE UserName=\"{textBox8.Text}\" AND Password=\"{textBox2.Text}\";";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            var reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();
                UserAccount user = new UserAccount();
                user.UserID = int.Parse(reader["user_id"] + "");
                user.UserName = reader["UserName"] + "";
                user.Password = reader["Password"] + "";

                if(reader["BillingAddress"] != null)
                {
                    user.PrimaryBillingInfo = new BillingInfo();
                    user.PrimaryBillingInfo.BillingAddress = reader["BillingAddress"] + "";
                    user.PrimaryBillingInfo.CardNumber = long.Parse(reader["CardNumber"] + "");
                    user.PrimaryBillingInfo.CVC = int.Parse(reader["CVC"] + "");
                    user.PrimaryBillingInfo.NameOnCard = reader["NameOnCard"] + "";
                    user.PrimaryBillingInfo.ValidThrough = DateTime.Parse(reader["ValidThrough"] + "");

                }
                account = user;
            }

            reader.Close();
            CloseConnection();

            if (account == null)
            {
                MessageBox.Show("Invalid username or password");
            }
            else
            {
                accountLink.Text = $"logged in as {account.UserName}";
                browsePanel.Visible = true;
                loginPanel.Visible = false;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            cartPanel.Visible = true;
            browsePanel.Visible = false;
            billingPanel.BringToFront();
            UpdateCartView();
        }

        void UpdateListview()
        {
            listView1.View = View.Details;
            listView1.GridLines = true;
            listView1.FullRowSelect = true;
            listView1.Columns.Clear();
            listView1.Items.Clear();
            //Add column header
            listView1.Columns.Add("Title", 200);
            listView1.Columns.Add("Author(s)", 100);
            listView1.Columns.Add("ISBN", 100);


            //Add items in the listview
            string[] arr = new string[3];
            ListViewItem item;
            for (int i = 0; i < searched_books.Count; i++)
            {
                //Add first item
                arr[0] = searched_books[i].Title;
                arr[1] = searched_books[i].Author;
                arr[2] = searched_books[i].ISBN.ToString();

                item = new ListViewItem(arr);
                listView1.Items.Add(item);

            }
        }

        void UpdateCartView()
        {
            checkBox1.Enabled = account != null && account.PrimaryBillingInfo != null;
            billingPanel.Visible = !checkBox1.Checked;
            UpdateCartContentsPanel();
            checkoutButton.Enabled = account != null && billingForOrder != null && !cart.IsEmpty() && (checkBox1.Checked || billingFilledOut());
        }

        BillingInfo billingForOrder;
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            billingPanel.Visible = !checkBox1.Checked;
            if (checkBox1.Checked)
            {
                billingForOrder = account.PrimaryBillingInfo;
            }
            else if (billingFilledOut())
            {
                billingForOrder = new BillingInfo()
                {
                    BillingAddress = billingInfoAddressInput.Text,
                    NameOnCard = billingInfoNameInput.Text,
                    CardNumber = long.Parse(billingInfoCardNumberInput.Text),
                    CVC = int.Parse(billingInfoCVCInput.Text),
                    ValidThrough = billingInfoValidThruPicker.Value
                };
            }
            else
                billingForOrder = null;
            checkoutButton.Enabled = account != null && billingForOrder != null && !cart.IsEmpty();
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            ClearInterface();
            browsePanel.Visible = true;

        }

        void UpdateCartContentsPanel()
        {
            cartContents.Controls.Clear();

            foreach (var item in cart.items)
            {
                Panel panel = new Panel();
                panel.Dock = DockStyle.Top;
                panel.Width = cartContents.Width;
                panel.Height = 45;
                panel.BorderStyle = BorderStyle.FixedSingle;
                LineItem val = item.Value;

                Label label = new Label();
                label.Text = val.Book.Title;
                label.Size = new Size(300, 20);
                label.Location = new Point(0, 0);
                panel.Controls.Add(label);

                label = new Label();
                label.Text = "quantity:";
                label.Location = new Point(0, 21);
                label.AutoSize = false;
                label.Size = new Size(60, 20);
                panel.Controls.Add(label);

                NumericUpDown quant = new NumericUpDown();
                quant.Minimum = 1;
                quant.Maximum = val.Book.Stock;
                quant.AutoSize = false;
                quant.Location = new Point(50, 18);
                quant.Size = new Size(45, 20);
                quant.Value = val.Quantity;
                quant.ValueChanged += (s, e) =>
                {
                    LineItem lineItem = item.Value;
                    lineItem.Quantity = (int)quant.Value;
                    cart.items[item.Key] = lineItem;
                    totalCostLabel.Text = "Total: $" + cart.GetTotalPrice();
                };
                
                panel.Controls.Add(quant);

                Button trash = new Button();
                trash.Text = "🗑";
                trash.Size = new Size(20, 20);
                trash.Location = new Point(100, 18);
                trash.Click += (s, e) =>
                {
                    cartContents.Controls.Remove(panel);
                    cart.items.Remove(item.Key);
                    totalCostLabel.Text = "Total: $" + cart.GetTotalPrice();
                    UpdateCartView();
                };
                panel.Controls.Add(trash);

                label = new Label();
                label.Text = "$" + item.Value.Book.Price;
                label.Location = new Point(510, 2);
                label.AutoSize = false;
                label.Size = new Size(60, 20);
                panel.Controls.Add(label);

                quant.BringToFront();
                trash.BringToFront();
                cartContents.Controls.Add(panel);


                totalCostLabel.Text = "Total: $" + cart.GetTotalPrice();
            }
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ClearInterface();
            browsePanel.Visible = true;
            loginPanel.Visible = false;
        }

        private void accountLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ClearInterface();
            if (account == null)
            {
                loginPanel.Visible = true;
            }
            else
            {
                accountPanel.Visible = true;
                billingPanel.Visible = true;
                accountUserNameLabel.Text = account.UserName;

                billingPanel.BringToFront();

                OpenConnection();
                string query = $"SELECT * FROM billing_info WHERE user_id=\"{account.UserID}\"";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                var reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();
                    BillingInfo billing = new BillingInfo();

                    billing.BillingAddress = reader["BillingAddress"] + "";
                    billing.CardNumber = long.Parse(reader["CardNumber"] + "");
                    billing.NameOnCard = reader["NameOnCard"] + "";
                    billing.ValidThrough = DateTime.Parse(reader["ValidThrough"] + "");
                    billing.CVC = int.Parse(reader["CVC"] + "");
                    account.PrimaryBillingInfo = billing;
                }
                reader.Close();
                CloseConnection();

                if (account.PrimaryBillingInfo != null)
                {
                    billingInfoAddressInput.Text = account.PrimaryBillingInfo?.BillingAddress;
                    billingInfoNameInput.Text = account.PrimaryBillingInfo?.NameOnCard;
                    billingInfoCardNumberInput.Text = account.PrimaryBillingInfo?.CardNumber.ToString();
                    billingInfoValidThruPicker.Value = account.PrimaryBillingInfo?.ValidThrough ?? DateTime.Now;
                    billingInfoCVCInput.Text = account.PrimaryBillingInfo?.CVC.ToString() ?? "";
                }

                ordersPanel.Controls.Clear();

                OpenConnection();
                query = $"SELECT order_id FROM Orders WHERE user_id=\"{account.UserID}\"";
                cmd = new MySqlCommand(query, connection);
                reader = cmd.ExecuteReader();
                List<Order> orders = new List<Order>();
                while (reader.Read())
                {
                    Order order = new Order();
                    order.OrderNumber = int.Parse(reader["order_id"] + "");
                    orders.Add(order);
                }
                reader.Close();
                CloseConnection();

                foreach (Order order in orders)
                {
                    Label label = new Label();
                    label.Text = order.OrderNumber.ToString() + ": In Transit";
                    label.BorderStyle = BorderStyle.FixedSingle;
                    label.AutoSize = false;
                    label.Size = new Size(ordersPanel.Width, 20);
                    label.Dock = DockStyle.Top;
                    ordersPanel.Controls.Add(label);
                }
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            ClearInterface();
            account = null;
            accountLink.Text = "log in";
            loginPanel.Visible = true;
        }

        void ClearInterface()
        {
            accountPanel.Visible = false;
            billingPanel.Visible = false;
            registrationPanel.Visible = false;
            cartPanel.Visible = false;
            loginPanel.Visible = false;
            browsePanel.Visible = false;
            orderConfirmationPanel.Visible = false;
            billingInfoAddressInput.Text = "";
            billingInfoCardNumberInput.Text = "";
            billingInfoNameInput.Text = "";
            billingInfoCVCInput.Text = "";
            billingInfoValidThruPicker.Value = DateTime.Now;
            registrationUserNameInput.Text = "";
            registrationPasswordInput.Text = "";
            registrationEMailInput.Text = "";
            textBox8.Text = "";
            textBox2.Text = "";
            cartCountLabel.Text = cart.GetTotalQuantity().ToString();
            label25.Text = "";
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            billingPanel.Visible = checkBox2.Checked;
        }

        private void registrationButton_Click(object sender, EventArgs e)
        {
            //todo validation
            UserAccount userAccount = new UserAccount();
            userAccount.UserName = registrationUserNameInput.Text;
            userAccount.Password = registrationPasswordInput.Text;
            if (checkBox2.Checked)
            {
                //todo insert new billing info into database
                if(billingFilledOut())
                {
                    userAccount.PrimaryBillingInfo = new BillingInfo()
                    {
                        BillingAddress = billingInfoAddressInput.Text,
                        NameOnCard = billingInfoNameInput.Text,
                        CardNumber = long.Parse(billingInfoCardNumberInput.Text),
                        CVC = int.Parse(billingInfoCVCInput.Text),
                        ValidThrough = billingInfoValidThruPicker.Value
                    };

                    OpenConnection();
                    BillingInfo billing = account.PrimaryBillingInfo;
                    string query1 = $"DELETE FROM billing_info WHERE user_id={account.UserID}";
                    MySqlCommand cmd1 = new MySqlCommand(query1, connection);
                    cmd1.ExecuteNonQuery();
                    query1 = $"INSERT INTO billing_info (`user_id`, `BillingAddress`, `NameOnCard`, `CardNumber`, `CVC`, `ValidThrough`) VALUES(" +
                        $"'{account.UserID}'," +
                        $"'{billing.BillingAddress}'," +
                        $"'{billing.NameOnCard}'," +
                        $"'{billing.CardNumber}'," +
                        $"'{billing.CVC}'," +
                        $"'{billing.ValidThrough.ToString("yyyy-MM-dd HH:mm:ss")}')";
                    cmd1 = new MySqlCommand(query1, connection);
                    cmd1.ExecuteNonQuery();
                    CloseConnection();
                }

            }

            OpenConnection();
            string query = $"INSERT INTO Users (`UserName`, `Password`) VALUES('{userAccount.UserName}', '{userAccount.Password}')";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.ExecuteNonQuery();
            CloseConnection();

            ClearInterface();

            loginPanel.Visible = true;
        }

        private void checkoutButton_Click(object sender, EventArgs e)
        {
            OpenConnection();
            string query = $"SELECT MAX(order_id) AS next FROM orders;";
            
            MySqlCommand cmd = new MySqlCommand(query, connection);
            var reader = cmd.ExecuteReader();
            reader.Read();
            Order order = new Order();
            order.OrderNumber = int.Parse(reader["next"]+"");
            order.OrderNumber++;
            reader.Close();

            query = $"INSERT INTO Orders (`order_id`, `user_id`, `Date`) VALUES('{order.OrderNumber}', '{account.UserID}', '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')";
            cmd = new MySqlCommand(query, connection);
            cmd.ExecuteNonQuery();

            foreach (var item in cart.items)
            {
                LineItem lineItem = item.Value;
                query = $"INSERT INTO line_item (`order_id`, `ISBN`, `Quantity`) VALUES('{order.OrderNumber}', '{lineItem.Book.ISBN}', '{lineItem.Quantity}')";
                cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();


                query = $"UPDATE Books SET Stock=Stock-{lineItem.Quantity} WHERE ISBN='{lineItem.Book.ISBN}'";
                cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
            }
            CloseConnection();

            cart.Clear();
            label19.Text = "";
            ClearInterface();
            orderConfirmationPanel.Visible = true;
            label25.Text = order.OrderNumber.ToString();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            ClearInterface();
            loginPanel.Visible = true;
        }

        private void billingInputs_Changed(object sender, EventArgs e)
        {
            if (cartPanel.Visible == true)
            {
                if (checkBox1.Checked)
                {
                    billingForOrder = account.PrimaryBillingInfo;
                }
                else if (billingFilledOut())
                {
                    billingForOrder = new BillingInfo()
                    {
                        BillingAddress = billingInfoAddressInput.Text,
                        NameOnCard = billingInfoNameInput.Text,
                        CardNumber = long.Parse(billingInfoCardNumberInput.Text),
                        CVC = int.Parse(billingInfoCVCInput.Text),
                        ValidThrough = billingInfoValidThruPicker.Value
                    };
                }
                else
                    billingForOrder = null;
            }
            else if (accountPanel.Visible == true)
            {
                if (billingFilledOut())
                {
                    billingForOrder = new BillingInfo()
                    {
                        BillingAddress = billingInfoAddressInput.Text,
                        NameOnCard = billingInfoNameInput.Text,
                        CardNumber = long.Parse(billingInfoCardNumberInput.Text),
                        CVC = int.Parse(billingInfoCVCInput.Text),
                        ValidThrough = billingInfoValidThruPicker.Value
                    };
                }
                else
                    billingForOrder = null;
            }

            checkoutButton.Enabled = billingForOrder != null && !cart.IsEmpty();
            saveBillingButton.Enabled = billingForOrder != null;
        }

        bool billingFilledOut()
        {
            return billingInfoAddressInput.Text != "" &&
            billingInfoCardNumberInput.Text != "" &&
            billingInfoCVCInput.Text != "" &&
            billingInfoNameInput.Text != "";
        }

        private void saveBillingButton_Click(object sender, EventArgs e)
        {
            if (billingFilledOut())
            {
                account.PrimaryBillingInfo = new BillingInfo()
                {
                    BillingAddress = billingInfoAddressInput.Text,
                    NameOnCard = billingInfoNameInput.Text,
                    CardNumber = long.Parse(billingInfoCardNumberInput.Text),
                    CVC = int.Parse(billingInfoCVCInput.Text),
                    ValidThrough = billingInfoValidThruPicker.Value
                };

                OpenConnection();
                BillingInfo billing = account.PrimaryBillingInfo;
                string query = $"DELETE FROM billing_info WHERE user_id={account.UserID}";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                query = $"INSERT INTO billing_info (`user_id`, `BillingAddress`, `NameOnCard`, `CardNumber`, `CVC`, `ValidThrough`) VALUES(" +
                    $"'{account.UserID}'," +
                    $"'{billing.BillingAddress}'," +
                    $"'{billing.NameOnCard}'," +
                    $"'{billing.CardNumber}'," +
                    $"'{billing.CVC}'," +
                    $"'{billing.ValidThrough.ToString("yyyy-MM-dd HH:mm:ss")}')";
                cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                CloseConnection();

                MessageBox.Show("billing info saved");
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            ClearInterface();
            browsePanel.Visible = true;
            searched_books.Clear();
            UpdateListview();
            UpdateBookPreview();
        }
    }
}
