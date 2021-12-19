/* This is a listing of all SQL queries that appear in my C# code. 
* Note that many of them have parameters passed in from C# so the argument is wrapped in {} brackets
* Those queries would not run unless you replace those values with an actual argument
*/

SELECT * FROM Books NATURAL JOIN Genres;
SELECT * FROM Books NATURAL JOIN Genres WHERE Books.Title ='{search}';
SELECT * FROM Books NATURAL JOIN Genres WHERE Author='{search}';
SELECT * FROM Books NATURAL JOIN Genres WHERE ISBN='{search}';
SELECT * FROM users NATURAL JOIN billing_info WHERE UserName='{textBox8.Text}' AND Password='{textBox2.Text}';
SELECT * FROM billing_info WHERE user_id='{account.UserID}';
SELECT order_id FROM Orders WHERE user_id='{account.UserID}';
DELETE FROM billing_info WHERE user_id={account.UserID};

INSERT INTO billing_info (`user_id`, `BillingAddress`, `NameOnCard`, `CardNumber`, `CVC`, `ValidThrough`) VALUES( 
'{account.UserID}', 
'{billing.BillingAddress}', 
'{billing.NameOnCard}', 
'{billing.CardNumber}', 
'{billing.CVC}', 
'{billing.ValidThrough.ToString(yyyy-MM-dd HH:mm:ss)}');

INSERT INTO Users (`UserName`, `Password`) VALUES('{userAccount.UserName}', '{userAccount.Password}');
SELECT MAX(order_id) AS next FROM orders;

INSERT INTO Orders (`order_id`, `user_id`, `Date`) VALUES('{order.OrderNumber}', '{account.UserID}', '{DateTime.Now.ToString(yyyy-MM-dd HH:mm:ss)}');
INSERT INTO line_item (`order_id`, `ISBN`, `Quantity`) VALUES('{order.OrderNumber}', '{lineItem.Book.ISBN}', '{lineItem.Quantity}');

--subtract the quantity of a book sold from the available stock of books
UPDATE Books SET Stock=Stock-{lineItem.Quantity} WHERE ISBN='{lineItem.Book.ISBN}';

INSERT INTO billing_info (`user_id`, `BillingAddress`, `NameOnCard`, `CardNumber`, `CVC`, `ValidThrough`) VALUES( 
'{account.UserID}', 
'{billing.BillingAddress}', 
'{billing.NameOnCard}', 
'{billing.CardNumber}', 
'{billing.CVC}', 
'{billing.ValidThrough.ToString(yyyy-MM-dd HH:mm:ss)}');

--queries used to search books by different attributes
SELECT* FROM Books NATURAL JOIN Genres;
SELECT * FROM Books NATURAL JOIN Genres WHERE Books.Title ='{search}';
SELECT * FROM Books NATURAL JOIN Genres WHERE Author='{search}';


DELETE FROM Books WHERE ISBN='{selectedBook.ISBN}';

INSERT INTO books (`ISBN`, `Pages`, `Stock`, `Title`, `Author`, `publisher_id`, `genre_id`, `Price`, `Format`) VALUES( 
'{selectedBook.ISBN}', 
'{selectedBook.Pages}', 
'{selectedBook.Stock}', 
'{selectedBook.Title}', 
'{selectedBook.Author}', 
'{selectedBook.PublisherID}', 
'{selectedBook.GenreID}', 
'{selectedBook.Price}', 
'{selectedBook.Format}');

SELECT * FROM genres;
SELECT * FROM publishers;

SELECT * FROM Publishers LEFT JOIN Bank_Details ON Publishers.bank_details_id=Bank_Details.bank_details_id;
SELECT * FROM Publishers LEFT JOIN Bank_Details ON Publishers.bank_details_id=Bank_Details.bank_details_id WHERE PublisherName='{search}';
SELECT * FROM Books WHERE publisher_id={selectedPublisher.PublisherID};

UPDATE Publishers SET publisher_id = '{selectedPublisher.PublisherID}', 
PublisherName = '{selectedPublisher.PublisherName}', 
Address = '{selectedPublisher.Address}', 
EMailAddress = '{selectedPublisher.EmailAddress}', 
PhoneNumber = '{selectedPublisher.PhoneNumber}' WHERE publisher_id ='{selectedPublisher.PublisherID}';

SELECT * FROM Bank_Details WHERE bank_details_id='{selectedPublisher.BankDetailsID}';
UPDATE Bank_Details SET 
RoutingNumber = '{selectedPublisher.RoutingNumber}', 
AccountNumber = '{selectedPublisher.AccountNumber}' WHERE bank_details_id ='{selectedPublisher.BankDetailsID}';

INSERT INTO Bank_Details (`RoutingNumber`, `AccountNumber`) VALUES('{selectedPublisher.RoutingNumber}', '{selectedPublisher.AccountNumber}') ;
SELECT MAX(bank_details_id) AS next_id FROM Bank_Details;
UPDATE Publishers SET bank_details_id='{selectedPublisher.BankDetailsID}' WHERE publisher_id='{selectedPublisher.PublisherID}';

--Views for reports
SELECT * FROM SalesByMonth
SELECT * FROM SalesByAuthor
SELECT * FROM SalesByGenre