using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Look_Inna_Book
{
    class Book
    {
        public int Pages, Stock;
        public string ISBN;
        public string Title, Author, Publisher, Genre;
        public int GenreID;
        public int PublisherID;
        public decimal Price;
        public decimal PublisherCut; //percentage of sales that goes to the publisher
        public Format Format;

        public Book()
        {
            Publisher = "";
        }
    }

    class BillingInfo
    {
        public string BillingAddress, NameOnCard;
        public DateTime ValidThrough;
        public long CardNumber;
        public int CVC;
    }

    class UserAccount
    {
        public int UserID;
        public string UserName, Password;
        public BillingInfo PrimaryBillingInfo;
    }

    class Publisher
    {
        public int PublisherID { get; set; }
        public string PublisherName { get; set; }
        public string Address, EmailAddress, PhoneNumber;
        public string AccountNumber, RoutingNumber;
        internal int BankDetailsID;

    }

    struct Order
    {
        public int OrderNumber;
        public string Status;
    }

    class Genre
    {
        public int GenreID { get; set; }
        public string GenreName { get; set; }
    }
    enum Format
    {
        Hardcover,
        PaperBack,
        Audiobook,
    }
}
