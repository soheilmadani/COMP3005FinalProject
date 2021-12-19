CREATE DATABASE  IF NOT EXISTS `look_inna_book` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `look_inna_book`;
-- MySQL dump 10.13  Distrib 8.0.26, for Win64 (x86_64)
--
-- Host: localhost    Database: look_inna_book
-- ------------------------------------------------------
-- Server version	8.0.26

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `bank_details`
--

DROP TABLE IF EXISTS `bank_details`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `bank_details` (
  `bank_details_id` int NOT NULL AUTO_INCREMENT,
  `AccountNumber` varchar(45) NOT NULL,
  `RoutingNumber` varchar(45) NOT NULL,
  PRIMARY KEY (`bank_details_id`)
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `bank_details`
--

LOCK TABLES `bank_details` WRITE;
/*!40000 ALTER TABLE `bank_details` DISABLE KEYS */;
INSERT INTO `bank_details` VALUES (8,'234','256'),(9,'321321321321','3213213213213'),(10,'23423423423','23423423'),(11,'53453453','534534534345'),(12,'fsdaf','asdf');
/*!40000 ALTER TABLE `bank_details` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `billing_info`
--

DROP TABLE IF EXISTS `billing_info`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `billing_info` (
  `user_id` int NOT NULL,
  `BillingAddress` varchar(45) NOT NULL,
  `NameOnCard` varchar(45) NOT NULL,
  `ValidThrough` date NOT NULL,
  `CardNumber` bigint NOT NULL,
  `CVC` smallint NOT NULL,
  PRIMARY KEY (`user_id`),
  CONSTRAINT `user_billing` FOREIGN KEY (`user_id`) REFERENCES `users` (`user_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `billing_info`
--

LOCK TABLES `billing_info` WRITE;
/*!40000 ALTER TABLE `billing_info` DISABLE KEYS */;
INSERT INTO `billing_info` VALUES (1,'5555 W MyStreet','John LastName','2024-01-10',4444555566669990,456),(3,'2424 Street Street','Eustace DaMan','2021-12-10',1111111111111111,123);
/*!40000 ALTER TABLE `billing_info` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `books`
--

DROP TABLE IF EXISTS `books`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `books` (
  `ISBN` varchar(45) NOT NULL,
  `Pages` int DEFAULT NULL,
  `Stock` int NOT NULL,
  `Title` varchar(200) DEFAULT NULL,
  `Author` varchar(200) DEFAULT NULL,
  `publisher_id` int DEFAULT NULL,
  `Publisher` varchar(200) DEFAULT NULL,
  `genre_id` int DEFAULT NULL,
  `Price` decimal(4,2) DEFAULT NULL,
  `Format` int DEFAULT NULL,
  PRIMARY KEY (`ISBN`),
  KEY `book_genre_idx` (`genre_id`),
  KEY `publisher_id_idx` (`publisher_id`),
  CONSTRAINT `book_genre` FOREIGN KEY (`genre_id`) REFERENCES `genres` (`genre_id`),
  CONSTRAINT `book_publisher` FOREIGN KEY (`publisher_id`) REFERENCES `publishers` (`publisher_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `books`
--

LOCK TABLES `books` WRITE;
/*!40000 ALTER TABLE `books` DISABLE KEYS */;
INSERT INTO `books` VALUES ('0385546025',368,21,'The Judge\'s List','John Grisham',4,'Doubleday',4,14.97,0),('0544668251',64,25,'Thing Explainer: Complicated Stuff in Simple Words','Randall Munroe',1,'Mariner Books',1,15.49,0),('0553418025',387,6,'The Martian','Andy Weir',2,'Broadway Books',2,10.41,1),('1982173610',528,11,'Billy Summers','Stephen King',5,'Scribner',4,15.00,0),('9781451673319',249,5,'Fahrenheit 451','Ray Bradbury',3,'Simon & Schuster',3,8.29,1);
/*!40000 ALTER TABLE `books` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `books_BEFORE_UPDATE` BEFORE UPDATE ON `books` FOR EACH ROW BEGIN
IF new.stock < 10 THEN
	SET new.stock = new.stock+NumberSoldLastMonth(new.ISBN);
END IF;
END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `genres`
--

DROP TABLE IF EXISTS `genres`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `genres` (
  `genre_id` int NOT NULL AUTO_INCREMENT,
  `GenreName` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`genre_id`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `genres`
--

LOCK TABLES `genres` WRITE;
/*!40000 ALTER TABLE `genres` DISABLE KEYS */;
INSERT INTO `genres` VALUES (0,'General'),(1,'Science and Technology'),(2,'Science Fiction'),(3,'Classics'),(4,'Mystery, Thriller, & Suspense');
/*!40000 ALTER TABLE `genres` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `line_item`
--

DROP TABLE IF EXISTS `line_item`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `line_item` (
  `order_id` int NOT NULL,
  `ISBN` varchar(45) NOT NULL,
  `Quantity` int DEFAULT NULL,
  PRIMARY KEY (`order_id`,`ISBN`),
  CONSTRAINT `order_line_item` FOREIGN KEY (`order_id`) REFERENCES `orders` (`order_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `line_item`
--

LOCK TABLES `line_item` WRITE;
/*!40000 ALTER TABLE `line_item` DISABLE KEYS */;
INSERT INTO `line_item` VALUES (0,'0544668251',72),(0,'1982173610',27),(1,'0544668251',12),(1,'0553418025',10),(2,'0385546025',16),(2,'9781451673319',5),(3,'0385546025',5),(4,'0553418025',4);
/*!40000 ALTER TABLE `line_item` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `orders`
--

DROP TABLE IF EXISTS `orders`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `orders` (
  `order_id` int NOT NULL,
  `user_id` int NOT NULL,
  `Date` date NOT NULL,
  `Status` varchar(45) NOT NULL DEFAULT 'Pending',
  PRIMARY KEY (`order_id`),
  KEY `user_id_idx` (`user_id`),
  CONSTRAINT `order_user` FOREIGN KEY (`user_id`) REFERENCES `users` (`user_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `orders`
--

LOCK TABLES `orders` WRITE;
/*!40000 ALTER TABLE `orders` DISABLE KEYS */;
INSERT INTO `orders` VALUES (0,1,'2021-11-12','Shipped'),(1,1,'2021-12-11','In Transit'),(2,1,'2021-11-16','Pending'),(3,1,'2021-12-13','Pending'),(4,1,'2021-12-13','Pending');
/*!40000 ALTER TABLE `orders` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `publishers`
--

DROP TABLE IF EXISTS `publishers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `publishers` (
  `publisher_id` int NOT NULL,
  `PublisherName` varchar(45) NOT NULL,
  `Address` varchar(45) DEFAULT NULL,
  `EMailAddress` varchar(45) DEFAULT NULL,
  `PhoneNumber` varchar(45) DEFAULT NULL,
  `bank_details_id` int DEFAULT NULL,
  PRIMARY KEY (`publisher_id`),
  KEY `publisher_bank_details_idx` (`bank_details_id`),
  CONSTRAINT `publisher_bank_details` FOREIGN KEY (`bank_details_id`) REFERENCES `bank_details` (`bank_details_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `publishers`
--

LOCK TABLES `publishers` WRITE;
/*!40000 ALTER TABLE `publishers` DISABLE KEYS */;
INSERT INTO `publishers` VALUES (1,'Mariner Books','','','',NULL),(2,'Broadway Books','','','',NULL),(3,'Simon & Schuster','222 street street','','',8),(4,'Doubleday',NULL,NULL,NULL,NULL),(5,'Scribner',NULL,NULL,NULL,NULL);
/*!40000 ALTER TABLE `publishers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Temporary view structure for view `salesbyauthor`
--

DROP TABLE IF EXISTS `salesbyauthor`;
/*!50001 DROP VIEW IF EXISTS `salesbyauthor`*/;
SET @saved_cs_client     = @@character_set_client;
/*!50503 SET character_set_client = utf8mb4 */;
/*!50001 CREATE VIEW `salesbyauthor` AS SELECT 
 1 AS `Author`,
 1 AS `TotalSales`*/;
SET character_set_client = @saved_cs_client;

--
-- Temporary view structure for view `salesbygenre`
--

DROP TABLE IF EXISTS `salesbygenre`;
/*!50001 DROP VIEW IF EXISTS `salesbygenre`*/;
SET @saved_cs_client     = @@character_set_client;
/*!50503 SET character_set_client = utf8mb4 */;
/*!50001 CREATE VIEW `salesbygenre` AS SELECT 
 1 AS `GenreName`,
 1 AS `TotalSales`*/;
SET character_set_client = @saved_cs_client;

--
-- Temporary view structure for view `salesbymonth`
--

DROP TABLE IF EXISTS `salesbymonth`;
/*!50001 DROP VIEW IF EXISTS `salesbymonth`*/;
SET @saved_cs_client     = @@character_set_client;
/*!50503 SET character_set_client = utf8mb4 */;
/*!50001 CREATE VIEW `salesbymonth` AS SELECT 
 1 AS `MonthYear`,
 1 AS `TotalSales`*/;
SET character_set_client = @saved_cs_client;

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `users` (
  `user_id` int NOT NULL AUTO_INCREMENT,
  `UserName` varchar(45) NOT NULL,
  `Password` varchar(45) NOT NULL,
  PRIMARY KEY (`user_id`),
  UNIQUE KEY `UserName_UNIQUE` (`UserName`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `users`
--

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` VALUES (1,'John','pass'),(2,'asdf','asdf'),(3,'Eustace','pass');
/*!40000 ALTER TABLE `users` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping routines for database 'look_inna_book'
--
/*!50003 DROP FUNCTION IF EXISTS `NumberSoldLastMonth` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` FUNCTION `NumberSoldLastMonth`(ISBN varchar(50)) RETURNS int
BEGIN
RETURN(SELECT IFNULL((SELECT SUM(quantity) AS NumberSold FROM orders INNER JOIN line_item ON line_item.order_id = orders.order_id WHERE line_item.ISBN=ISBN AND Date BETWEEN DATE_FORMAT(NOW() - INTERVAL 1 MONTH, '%Y-%m-01 00:00:00')
AND DATE_FORMAT(LAST_DAY(NOW() - INTERVAL 1 MONTH), '%Y-%m-%d 23:59:59')), 0));
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `PlaceOrderToPublisher` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `PlaceOrderToPublisher`(IN ISBN VARCHAR(50), IN quantity INT)
BEGIN
UPDATE books SET books.stock=books.stock+quantity WHERE books.ISBN=ISBN;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Final view structure for view `salesbyauthor`
--

/*!50001 DROP VIEW IF EXISTS `salesbyauthor`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = utf8mb4 */;
/*!50001 SET character_set_results     = utf8mb4 */;
/*!50001 SET collation_connection      = utf8mb4_0900_ai_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`root`@`localhost` SQL SECURITY DEFINER */
/*!50001 VIEW `salesbyauthor` AS select `books`.`Author` AS `Author`,sum((`line_item`.`Quantity` * `books`.`Price`)) AS `TotalSales` from ((`line_item` join `orders` on((`line_item`.`order_id` = `orders`.`order_id`))) join `books` on((`books`.`ISBN` = `line_item`.`ISBN`))) group by `books`.`Author` */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;

--
-- Final view structure for view `salesbygenre`
--

/*!50001 DROP VIEW IF EXISTS `salesbygenre`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = utf8mb4 */;
/*!50001 SET character_set_results     = utf8mb4 */;
/*!50001 SET collation_connection      = utf8mb4_0900_ai_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`root`@`localhost` SQL SECURITY DEFINER */
/*!50001 VIEW `salesbygenre` AS select `genres`.`GenreName` AS `GenreName`,sum((`line_item`.`Quantity` * `books`.`Price`)) AS `TotalSales` from (((`line_item` join `orders` on((`line_item`.`order_id` = `orders`.`order_id`))) join `books` on((`books`.`ISBN` = `line_item`.`ISBN`))) join `genres` on((`books`.`genre_id` = `genres`.`genre_id`))) group by `genres`.`GenreName` */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;

--
-- Final view structure for view `salesbymonth`
--

/*!50001 DROP VIEW IF EXISTS `salesbymonth`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = utf8mb4 */;
/*!50001 SET character_set_results     = utf8mb4 */;
/*!50001 SET collation_connection      = utf8mb4_0900_ai_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`root`@`localhost` SQL SECURITY DEFINER */
/*!50001 VIEW `salesbymonth` AS select date_format(`orders`.`Date`,'%M %Y') AS `MonthYear`,sum((`line_item`.`Quantity` * `books`.`Price`)) AS `TotalSales` from ((`line_item` join `orders` on((`line_item`.`order_id` = `orders`.`order_id`))) join `books` on((`books`.`ISBN` = `line_item`.`ISBN`))) group by `MonthYear` order by `MonthYear` desc */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2021-12-13  9:16:46
