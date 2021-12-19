--If, upon updating a book in the table, the stock is now less then 10, increase the stock by the number of copies sold the previous month.
CREATE TRIGGER `books_BEFORE_UPDATE` BEFORE UPDATE ON `books` FOR EACH ROW BEGIN
IF new.stock < 10 THEN
	SET new.stock = new.stock+NumberSoldLastMonth(new.ISBN);
END IF;
END