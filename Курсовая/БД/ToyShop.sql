-- MySQL dump 10.13  Distrib 8.0.43, for Win64 (x86_64)
--
-- Host: 127.0.0.1    Database: toy_shop
-- ------------------------------------------------------
-- Server version	9.4.0

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
-- Table structure for table `category`
--

DROP TABLE IF EXISTS `category`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `category` (
  `CategoryID` int NOT NULL,
  `CategoryName` varchar(100) NOT NULL,
  PRIMARY KEY (`CategoryID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `category`
--

LOCK TABLES `category` WRITE;
/*!40000 ALTER TABLE `category` DISABLE KEYS */;
INSERT INTO `category` VALUES (1,'Конструкторы'),(2,'Куклы и пупсы'),(3,'Мягкие игрушки'),(4,'Настольные игры'),(5,'Машинки и техника'),(6,'Развивающие игрушки'),(7,'Уличные игрушки'),(8,'Творчество и наборы для рукоделия');
/*!40000 ALTER TABLE `category` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `methodpayment`
--

DROP TABLE IF EXISTS `methodpayment`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `methodpayment` (
  `MethodPaymentID` int NOT NULL,
  `MethodPaymentName` varchar(100) NOT NULL,
  PRIMARY KEY (`MethodPaymentID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `methodpayment`
--

LOCK TABLES `methodpayment` WRITE;
/*!40000 ALTER TABLE `methodpayment` DISABLE KEYS */;
INSERT INTO `methodpayment` VALUES (1,'Наличные'),(2,'Банковская карта'),(3,'СПБ'),(4,'Рассрочка');
/*!40000 ALTER TABLE `methodpayment` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `payment`
--

DROP TABLE IF EXISTS `payment`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `payment` (
  `PaymentID` int NOT NULL,
  `PaymentProduct` int NOT NULL,
  `PaymentCountProduct` int NOT NULL,
  `PaymentUser` int NOT NULL,
  `PaymentMethod` int NOT NULL,
  `PaymentSumma` double NOT NULL,
  `PaymentStatus` int NOT NULL,
  PRIMARY KEY (`PaymentID`),
  KEY `PaymentProduct` (`PaymentProduct`),
  KEY `PaymentUser` (`PaymentUser`),
  KEY `PaymentMethod` (`PaymentMethod`),
  KEY `PaymentStatus` (`PaymentStatus`),
  CONSTRAINT `payment_ibfk_1` FOREIGN KEY (`PaymentProduct`) REFERENCES `product` (`ProductID`),
  CONSTRAINT `payment_ibfk_2` FOREIGN KEY (`PaymentUser`) REFERENCES `user` (`UserID`),
  CONSTRAINT `payment_ibfk_3` FOREIGN KEY (`PaymentMethod`) REFERENCES `methodpayment` (`MethodPaymentID`),
  CONSTRAINT `payment_ibfk_4` FOREIGN KEY (`PaymentStatus`) REFERENCES `status` (`StatusID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `payment`
--

LOCK TABLES `payment` WRITE;
/*!40000 ALTER TABLE `payment` DISABLE KEYS */;
INSERT INTO `payment` VALUES (1,1,1,3,2,45990,1),(2,3,2,4,1,17980,3),(3,5,3,3,3,4770,2),(4,2,1,5,2,78990,1),(5,8,2,4,1,9180,1),(6,4,1,5,4,6490,2),(7,6,1,3,2,54990,3),(8,7,1,5,3,12990,3);
/*!40000 ALTER TABLE `payment` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `product`
--

DROP TABLE IF EXISTS `product`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `product` (
  `ProductID` int NOT NULL,
  `ProductName` varchar(255) NOT NULL,
  `ProductDecription` varchar(255) DEFAULT NULL,
  `ProductSale` double NOT NULL,
  `ProductCategory` int NOT NULL,
  `ProductSupplier` int NOT NULL,
  `ProductPhoto` varchar(255) NOT NULL,
  PRIMARY KEY (`ProductID`),
  KEY `ProductCategory` (`ProductCategory`),
  KEY `ProductSupplier` (`ProductSupplier`),
  CONSTRAINT `product_ibfk_1` FOREIGN KEY (`ProductCategory`) REFERENCES `category` (`CategoryID`),
  CONSTRAINT `product_ibfk_2` FOREIGN KEY (`ProductSupplier`) REFERENCES `supplier` (`SupplierID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `product`
--

LOCK TABLES `product` WRITE;
/*!40000 ALTER TABLE `product` DISABLE KEYS */;
INSERT INTO `product` VALUES (1,'Смартфон Galaxy X','Смартфон с 6.5\" экраном, 128 ГБ',45990,1,1,'galaxy_x.jpg'),(2,'Ноутбук ProBook 15','Ноутбук 15.6\", 16 ГБ ОЗУ, 512 ГБ SSD',78990,3,1,'probook15.jpg'),(3,'Наушники BassBoom','Беспроводные наушники с шумоподавлением',8990,5,4,'bassboom.jpg'),(4,'Микроволновка MW-200','Микроволновая печь 20 л, 700 Вт',6490,2,3,'mw200.jpg'),(5,'Мышь Wireless Pro','Беспроводная мышь, 1600 dpi',1590,4,2,'mouse_pro.jpg'),(6,'Телевизор SmartTV 55\"','4K UHD Smart TV, 55 дюймов',54990,1,3,'smarttv55.jpg'),(7,'Пылесос CleanHome','Вертикальный пылесос, 2000 Вт',12990,2,2,'cleanhome.jpg'),(8,'Клавиатура MechKey','Механическая клавиатура с подсветкой',4590,4,4,'mechkey.jpg');
/*!40000 ALTER TABLE `product` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `role`
--

DROP TABLE IF EXISTS `role`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `role` (
  `RoleID` int NOT NULL,
  `RoleName` varchar(100) NOT NULL,
  PRIMARY KEY (`RoleID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `role`
--

LOCK TABLES `role` WRITE;
/*!40000 ALTER TABLE `role` DISABLE KEYS */;
INSERT INTO `role` VALUES (1,'Администратор'),(2,'Менеджер'),(3,'Товаровед'),(4,'Директор'),(5,'Продавец');
/*!40000 ALTER TABLE `role` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `status`
--

DROP TABLE IF EXISTS `status`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `status` (
  `StatusID` int NOT NULL,
  `StatusName` varchar(100) NOT NULL,
  PRIMARY KEY (`StatusID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `status`
--

LOCK TABLES `status` WRITE;
/*!40000 ALTER TABLE `status` DISABLE KEYS */;
INSERT INTO `status` VALUES (1,'Собран'),(2,'В сборке'),(3,'В ожидании');
/*!40000 ALTER TABLE `status` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `supplier`
--

DROP TABLE IF EXISTS `supplier`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `supplier` (
  `SupplierID` int NOT NULL,
  `SupplierName` varchar(100) NOT NULL,
  `SupplierPhone` varchar(15) NOT NULL,
  PRIMARY KEY (`SupplierID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `supplier`
--

LOCK TABLES `supplier` WRITE;
/*!40000 ALTER TABLE `supplier` DISABLE KEYS */;
INSERT INTO `supplier` VALUES (1,'ООО \"ИгрушкаОпт\"','+74951112233'),(2,'ИП Смирнова А.А.','+74952223344'),(3,'ЗАО \"ДетскийМирТрейд\"','+74953334455'),(4,'ООО \"КубикПлюс\"','+74954445566'),(5,'ООО \"МягкийДруг\"','+74955556677');
/*!40000 ALTER TABLE `supplier` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `user`
--

DROP TABLE IF EXISTS `user`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user` (
  `UserID` int NOT NULL,
  `UserFirstName` varchar(100) NOT NULL,
  `UserName` varchar(100) NOT NULL,
  `UserThreeName` varchar(100) NOT NULL,
  `UserPhone` varchar(15) NOT NULL,
  `UserDateBirthday` date NOT NULL,
  `UserLogin` varchar(100) NOT NULL,
  `UserPassword` varchar(100) NOT NULL,
  `UserRole` int NOT NULL,
  PRIMARY KEY (`UserID`),
  KEY `UserRole` (`UserRole`),
  CONSTRAINT `user_ibfk_1` FOREIGN KEY (`UserRole`) REFERENCES `role` (`RoleID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `user`
--

LOCK TABLES `user` WRITE;
/*!40000 ALTER TABLE `user` DISABLE KEYS */;
INSERT INTO `user` VALUES (1,'Иван','Иванович','Иванов','+79001112233','1985-04-12','admin','admin',1),(2,'Пётр','Петрович','Петров','+79002223344','1990-09-25','manager','manager',2),(3,'Анна','Сергеевна','Смирнова','+79003334455','1995-01-30','tovar','tovar',3),(4,'Ольга','Дмитриевна','Кузнецова','+79004445566','2000-07-15','direct','direct',4),(5,'Сергей','Александрович','Морозов','+79005556677','1988-11-03','cass','cass',5);
/*!40000 ALTER TABLE `user` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-09-26 20:40:36
