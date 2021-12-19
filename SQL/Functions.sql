--Returns how many of the specified book were sold the previous month. Uses an IFNULL select to prevent from ever returning a NULL value
CREATE FUNCTION `NumberSoldLastMonth`(ISBN varchar(50)) RETURNS int
BEGIN
RETURN(SELECT IFNULL((SELECT SUM(quantity) AS NumberSold FROM orders INNER JOIN line_item ON line_item.order_id = orders.order_id WHERE line_item.ISBN=ISBN AND Date BETWEEN DATE_FORMAT(NOW() - INTERVAL 1 MONTH, '%Y-%m-01 00:00:00')
AND DATE_FORMAT(LAST_DAY(NOW() - INTERVAL 1 MONTH), '%Y-%m-%d 23:59:59')), 0));
END