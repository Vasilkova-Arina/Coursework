CREATE DATABASE  IF NOT EXISTS `toy_shop` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `toy_shop`;
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
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-09-26 20:37:16
