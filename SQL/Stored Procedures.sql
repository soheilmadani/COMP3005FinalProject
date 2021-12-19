--Restocks the book specified by ISBN by increasing the stock by a specified amount
CREATE PROCEDURE `PlaceOrderToPublisher`(IN ISBN VARCHAR(50), IN quantity INT)
BEGIN
UPDATE books SET books.stock=books.stock+quantity WHERE books.ISBN=ISBN;
END