
/****** Object:  Database [@@database_name@@]    Script Date: 11/10/2016 8:45:36 AM ******/
CREATE DATABASE [@@database_name@@]
 CONTAINMENT = NONE
GO

ALTER DATABASE [@@database_name@@] SET COMPATIBILITY_LEVEL = 110
GO

IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [@@database_name@@].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [@@database_name@@] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [@@database_name@@] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [@@database_name@@] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [@@database_name@@] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [@@database_name@@] SET ARITHABORT OFF 
GO
ALTER DATABASE [@@database_name@@] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [@@database_name@@] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [@@database_name@@] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [@@database_name@@] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [@@database_name@@] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [@@database_name@@] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [@@database_name@@] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [@@database_name@@] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [@@database_name@@] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [@@database_name@@] SET  DISABLE_BROKER 
GO
ALTER DATABASE [@@database_name@@] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [@@database_name@@] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [@@database_name@@] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [@@database_name@@] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [@@database_name@@] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [@@database_name@@] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [@@database_name@@] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [@@database_name@@] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [@@database_name@@] SET  MULTI_USER 
GO
ALTER DATABASE [@@database_name@@] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [@@database_name@@] SET DB_CHAINING OFF 
GO
ALTER DATABASE [@@database_name@@] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO

USE [@@database_name@@]
GO

CREATE LOGIN @@database_username@@ 
	WITH PASSWORD = '@@database_password@@', DEFAULT_DATABASE = @@database_name@@,
	CHECK_EXPIRATION =OFF,  
	CHECK_POLICY = OFF;  
GO  


/****** Object:  User [@@database_name@@]    Script Date: 11/10/2016 8:45:37 AM ******/
CREATE USER [@@database_username@@] FOR LOGIN [@@database_password@@] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_owner] ADD MEMBER [@@database_name@@]
GO
ALTER ROLE [db_ddladmin] ADD MEMBER [@@database_name@@]
GO
ALTER ROLE [db_datareader] ADD MEMBER [@@database_name@@]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [@@database_name@@]
GO

/****** Object:  Table [dbo].[ckbx_Credential]    Script Date: 11/10/2016 8:45:37 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
USE [master]
GO
ALTER DATABASE [@@database_name@@] SET  READ_WRITE 
GO
