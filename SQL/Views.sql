--Gets total all time sales for each author
CREATE VIEW `salesbyauthor` AS
    SELECT 
        `books`.`Author` AS `Author`,
        SUM((`line_item`.`Quantity` * `books`.`Price`)) AS `TotalSales`
    FROM
        ((`line_item`
        JOIN `orders` ON ((`line_item`.`order_id` = `orders`.`order_id`)))
        JOIN `books` ON ((`books`.`ISBN` = `line_item`.`ISBN`)))
    GROUP BY `books`.`Author`;

--Gets total all time sales for each genre of book
CREATE `salesbygenre` AS
    SELECT 
        `genres`.`GenreName` AS `GenreName`,
        SUM((`line_item`.`Quantity` * `books`.`Price`)) AS `TotalSales`
    FROM
        (((`line_item`
        JOIN `orders` ON ((`line_item`.`order_id` = `orders`.`order_id`)))
        JOIN `books` ON ((`books`.`ISBN` = `line_item`.`ISBN`)))
        JOIN `genres` ON ((`books`.`genre_id` = `genres`.`genre_id`)))
    GROUP BY `genres`.`GenreName`;

--Gets total sales for each month on record
CREATE VIEW `salesbymonth` AS
    SELECT 
        DATE_FORMAT(`orders`.`Date`, '%M %Y') AS `MonthYear`,
        SUM((`line_item`.`Quantity` * `books`.`Price`)) AS `TotalSales`
    FROM
        ((`line_item`
        JOIN `orders` ON ((`line_item`.`order_id` = `orders`.`order_id`)))
        JOIN `books` ON ((`books`.`ISBN` = `line_item`.`ISBN`)))
    GROUP BY `MonthYear`
    ORDER BY `MonthYear` DESC;

