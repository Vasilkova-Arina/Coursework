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
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-09-26 20:37:16
