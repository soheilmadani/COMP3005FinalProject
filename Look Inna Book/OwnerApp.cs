using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Look_Inna_Book
{
    public partial class OwnerApp : Form
    {
        private MySqlConnection connection;
        private string server;
        private string database;
        private string uid;
        private string password;

        List<Book> searched_books = new List<Book>();
        Book selectedBook;

        public OwnerApp()
        {
            InitializeComponent();
            GetSQLConnection();
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


        private void searchButton_Click(object sender, EventArgs e)
        {
            SearchBooks();
        }

        private void SearchBooks()
        {
            string search = searchTextInput.Text;
            string query = "";
            if (string.IsNullOrEmpty(search))
                query = "SELECT* FROM Books NATURAL JOIN Genres";
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

            UpdateBookListView();
        }

        void UpdateBookListView()
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

        void UpdateBookPreview()
        {
            if (selectedBook != null)
            {
                previewPanel.Visible = true;
                label4.Text = selectedBook?.Title;
                authorLabel.Text = "by " + selectedBook?.Author;
                ISBNLabel.Text = "ISBN: " + selectedBook?.ISBN;
                stockLabel.Text = selectedBook?.Stock + " in stock";
                pagesLabel.Text = selectedBook?.Pages + " pages";
                genreLabel.Text = selectedBook?.Genre;
                publisherLabel.Text = "published by " + selectedBook?.Publisher;
                priceLabel.Text = "$" + selectedBook?.Price.ToString();
            }
            else
            {
                previewPanel.Visible = false;
            }
        }

        private void listView1_Click(object sender, EventArgs e)
        {
            int i = listView1.SelectedIndices[0];
            selectedBook = null;
            if (i >= 0)
                selectedBook = searched_books[i];
            UpdateBookPreview();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show("Are you sure to delete this book from the database?", "Delete Selected Book?", MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                OpenConnection();
                string query = $"DELETE FROM Books WHERE ISBN=\"{selectedBook.ISBN}\"";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                CloseConnection();

                selectedBook = null;
                UpdateBookPreview();
                searched_books.Clear();
                UpdateBookListView();
            }
        }

        private void newButton_Click(object sender, EventArgs e)
        {
            selectedBook = new Book();
            panel1.Visible = true;
            panel1.BringToFront();
            UpdateBookEditView();
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            panel1.Visible = true;
            panel1.BringToFront();
            UpdateBookEditView();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            selectedBook.Title = titleTextInput.Text;
            selectedBook.Author = authorTextInput.Text;
            selectedBook.ISBN = ISBNTextInput.Text;
            selectedBook.Pages = (int)pagesNumericUpDown.Value;
            selectedBook.GenreID = (int)genreDropDown.SelectedValue;
            selectedBook.Genre = ((Genre)genreDropDown.SelectedItem).GenreName;
            selectedBook.PublisherID = (int)publisherDropDown.SelectedValue;
            selectedBook.Publisher = ((Publisher)publisherDropDown.SelectedItem).PublisherName;
            selectedBook.Price = priceNumbericUpDown.Value;
            selectedBook.PublisherCut = publisherCutNumericUpDown.Value;

            OpenConnection();
            string query = $"DELETE FROM books WHERE ISBN={selectedBook.ISBN}";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.ExecuteNonQuery();

            query = $"INSERT INTO books (`ISBN`, `Pages`, `Stock`, `Title`, `Author`, `publisher_id`, `genre_id`, `Price`, `Format`) VALUES(" +
                $"'{selectedBook.ISBN}'," +
                $"'{selectedBook.Pages}'," +
                $"'{selectedBook.Stock}'," +
                $"'{selectedBook.Title}'," +
                $"'{selectedBook.Author}'," +
                $"'{selectedBook.PublisherID}'," +
                $"'{selectedBook.GenreID}'," +
                $"'{selectedBook.Price}'," +
                $"'{(int)selectedBook.Format}')";

            cmd = new MySqlCommand(query, connection);
            cmd.ExecuteNonQuery();
            CloseConnection();
            panel1.Visible = false;


        }

        void UpdateBookEditView()
        {
            titleTextInput.Text = selectedBook.Title;
            authorTextInput.Text = selectedBook.Author;
            ISBNTextInput.Text = selectedBook.ISBN;
            pagesNumericUpDown.Value = selectedBook.Pages;
            publisherDropDown.SelectedValue = selectedBook.Publisher;
            priceNumbericUpDown.Value = selectedBook.Price;
            publisherCutNumericUpDown.Value = selectedBook.PublisherCut;

            List<Genre> genres = new List<Genre>();
            List<Publisher> publishers = new List<Publisher>();

            OpenConnection();
            string query = $"SELECT * FROM genres;";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Genre genre = new Genre()
                {
                    GenreID = int.Parse(reader["genre_id"] + ""),
                    GenreName = (reader["GenreName"] + "")
                };
                genres.Add(genre);
            }
            reader.Close();


            query = $"SELECT * FROM publishers;";
            cmd = new MySqlCommand(query, connection);
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Publisher pub = new Publisher()
                {
                    PublisherID = int.Parse(reader["publisher_id"] + ""),
                    PublisherName = (reader["PublisherName"] + "")
                };
                publishers.Add(pub);
            }
            reader.Close();
            CloseConnection();
            genreDropDown.DataSource = genres;
            genreDropDown.ValueMember = "GenreID";
            genreDropDown.DisplayMember = "GenreName";
            genreDropDown.SelectedValue = selectedBook.GenreID;

            publisherDropDown.DataSource = publishers;
            publisherDropDown.ValueMember = "PublisherID";
            publisherDropDown.DisplayMember = "PublisherName";
            publisherDropDown.SelectedValue = selectedBook.PublisherID;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            selectedBook = null;
            panel1.Visible = false;
            UpdateBookPreview();
        }


        Publisher selectedPublisher;
        void UpdatePublisherEditView()
        {
            textBox1.Text = selectedPublisher.PublisherName;
            textBox3.Text = selectedPublisher.Address;
            textBox2.Text = selectedPublisher.EmailAddress;
            textBox5.Text = selectedPublisher.PhoneNumber;
            textBox6.Text = selectedPublisher.RoutingNumber;
            textBox7.Text = selectedPublisher.AccountNumber;
        }

        List<Publisher> searched_publishers;

        private void button7_Click(object sender, EventArgs e)
        {
            string search = textBox4.Text;
            string query = "";
            if (string.IsNullOrEmpty(search))
                query = "SELECT * FROM Publishers LEFT JOIN Bank_Details ON Publishers.bank_details_id=Bank_Details.bank_details_id;";
            else
                query = $"SELECT * FROM Publishers LEFT JOIN Bank_Details ON Publishers.bank_details_id=Bank_Details.bank_details_id WHERE PublisherName=\"{search}\";";

            OpenConnection();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            var reader = cmd.ExecuteReader();
            searched_publishers = new List<Publisher>();
            while (reader.Read())
            {
                Publisher pub = new Publisher();
                pub.PublisherID = int.Parse(reader["publisher_id"] + "");
                pub.PublisherName = reader["PublisherName"] + "";
                pub.Address = reader["Address"] + "";
                pub.EmailAddress = reader["EMailAddress"] + "";
                int.TryParse(reader["bank_details_id"] + "", out pub.BankDetailsID);
                pub.PhoneNumber = reader["PhoneNumber"] + "";
                pub.RoutingNumber = reader["RoutingNumber"] + "";
                pub.AccountNumber = (reader["AccountNumber"] + "");
                searched_publishers.Add(pub);
            }
            reader.Close();
            CloseConnection();

            UpdatePublisherListView();
        }

        void UpdatePublisherListView()
        {
            listView2.View = View.Details;
            listView2.GridLines = true;
            listView2.FullRowSelect = true;
            listView2.Columns.Clear();
            listView2.Items.Clear();
            //Add column header
            listView2.Columns.Add("Name", 200);
            listView2.Columns.Add("E-Mail)", 100);
            listView2.Columns.Add("Phone Number", 100);


            //Add items in the listview
            string[] arr = new string[3];
            ListViewItem item;
            for (int i = 0; i < searched_publishers.Count; i++)
            {
                //Add first item
                arr[0] = searched_publishers[i].PublisherName;
                arr[1] = searched_publishers[i].EmailAddress;
                arr[2] = searched_publishers[i].PhoneNumber;

                item = new ListViewItem(arr);
                listView2.Items.Add(item);

            }
        }

        private void listView2_Click(object sender, EventArgs e)
        {
            int i = listView2.SelectedIndices[0];
            selectedPublisher = null;
            if (i >= 0)
                selectedPublisher = searched_publishers[i];
            UpdatePublisherPreview();
        }

        private void UpdatePublisherPreview()
        {
            if (selectedPublisher != null)
            {
                panel4.Visible = true;
                label20.Text = selectedPublisher.PublisherName;
                label24.Text = $"Address: {selectedPublisher.Address}";
                label22.Text = $"E-Mail:{selectedPublisher.EmailAddress}";
                label25.Text = $"Phone Number:{selectedPublisher.PhoneNumber}";
            }
            else
            {
                panel4.Visible = false;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show("Are you sure to delete this publisher from the database?", "Delete Selected Publisher?", MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                OpenConnection();

                string query = $"SELECT * FROM Books WHERE publisher_id={selectedPublisher.PublisherID}";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                var reader = cmd.ExecuteReader();

                if (!reader.HasRows) //if there are no books under this publisher connected via foreign key, we are free to delete it.
                {
                    query = $"DELETE FROM Publishers WHERE publisher_id=\"{selectedPublisher.PublisherID}\"";
                    cmd = new MySqlCommand(query, connection);
                    cmd.ExecuteNonQuery();
                    selectedPublisher = null;
                    UpdatePublisherPreview();
                    searched_publishers.Clear();
                    UpdatePublisherListView();
                }
                else
                    MessageBox.Show("You can't delete this publisher because there are still associated books in the database", "Warning");
                reader.Close();
                CloseConnection();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            panel3.Visible = true;
            panel3.BringToFront();
            UpdatePublisherEditView();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            panel3.Visible = false;
            UpdatePublisherPreview();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            selectedPublisher.PublisherName = textBox1.Text;
            selectedPublisher.Address = textBox3.Text;
            selectedPublisher.EmailAddress = textBox2.Text;
            selectedPublisher.PhoneNumber = textBox5.Text;
            selectedPublisher.RoutingNumber = textBox6.Text;
            selectedPublisher.AccountNumber = textBox7.Text;

            OpenConnection();

            string query = $"UPDATE Publishers SET " +
                $"publisher_id = '{selectedPublisher.PublisherID}', " +
                $"PublisherName = '{selectedPublisher.PublisherName}', " +
                $"Address = '{selectedPublisher.Address}', " +
                $"EMailAddress = '{selectedPublisher.EmailAddress}', " +
                $"PhoneNumber = '{selectedPublisher.PhoneNumber}' WHERE publisher_id ='{selectedPublisher.PublisherID}';";

            var cmd = new MySqlCommand(query, connection);
            cmd.ExecuteNonQuery();


            //if we've added banking details, save them
            if (!string.IsNullOrEmpty(selectedPublisher.RoutingNumber) && !string.IsNullOrEmpty(selectedPublisher.AccountNumber))
            {
                query = $"SELECT * FROM Bank_Details WHERE bank_details_id='{selectedPublisher.BankDetailsID}'";
                cmd = new MySqlCommand(query, connection);
                var reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Close();
                    query = $"UPDATE Bank_Details SET " +
                        $"RoutingNumber = '{selectedPublisher.RoutingNumber}', " +
                        $"AccountNumber = '{selectedPublisher.AccountNumber}' WHERE bank_details_id ='{selectedPublisher.BankDetailsID}';";
                    cmd = new MySqlCommand(query, connection);
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    reader.Close();
                    query = $"INSERT INTO Bank_Details (`RoutingNumber`, `AccountNumber`) VALUES('{selectedPublisher.RoutingNumber}', '{selectedPublisher.AccountNumber}') ;";
                    cmd = new MySqlCommand(query, connection);
                    cmd.ExecuteNonQuery();

                    query = "SELECT MAX(bank_details_id) AS next_id FROM Bank_Details;";
                    cmd = new MySqlCommand(query, connection);
                    reader = cmd.ExecuteReader();
                    reader.Read();
                    selectedPublisher.BankDetailsID = int.Parse(reader["next_id"] + "");
                    reader.Close();

                    query = $"UPDATE Publishers SET bank_details_id='{selectedPublisher.BankDetailsID}' WHERE publisher_id='{selectedPublisher.PublisherID}'";
                    cmd = new MySqlCommand(query, connection);
                    cmd.ExecuteNonQuery();
                }
            }

            CloseConnection();
            panel3.Visible = false;
            panel4.Visible = false;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            selectedPublisher = new Publisher();
            panel3.Visible = true;
            panel3.BringToFront();
            UpdatePublisherEditView();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
            {
                chart1.Visible = false;
            }
            else if (comboBox1.SelectedIndex == 1)
            {
                chart1.Series.Clear();
                string query = "SELECT * FROM SalesByMonth";
                connection.Open();
                var cmd = new MySqlCommand(query, connection);
                var reader = cmd.ExecuteReader();
                var series = chart1.Series.Add("Sales By Month");

                while(reader.Read())
                {
                    series.Points.AddXY(reader["MonthYear"] + "", decimal.Parse(reader["TotalSales"] + ""));
                }
                connection.Close();
                chart1.Visible = true;
                
            }
            else if (comboBox1.SelectedIndex == 2)
            {
                chart1.Series.Clear();
                string query = "SELECT * FROM SalesByAuthor";
                connection.Open();
                var cmd = new MySqlCommand(query, connection);
                var reader = cmd.ExecuteReader();
                var series = chart1.Series.Add("Sales By Author");

                while (reader.Read())
                {
                    series.Points.AddXY(reader["Author"] + "", decimal.Parse(reader["TotalSales"] + ""));
                }
                connection.Close();
                chart1.Visible = true;

            }
            else if (comboBox1.SelectedIndex == 3)
            {
                chart1.Series.Clear();
                string query = "SELECT * FROM SalesByGenre";
                connection.Open();
                var cmd = new MySqlCommand(query, connection);
                var reader = cmd.ExecuteReader();
                var series = chart1.Series.Add("Sales By Genre");

                while (reader.Read())
                {
                    series.Points.AddXY(reader["GenreName"] + "", decimal.Parse(reader["TotalSales"] + ""));
                }
                connection.Close();
                chart1.Visible = true;

            }
        }
    }
}