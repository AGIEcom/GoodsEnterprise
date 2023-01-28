USE [GoodsEnterprise]
GO
/****** Object:  StoredProcedure [dbo].[usp_INSERTPROMOTIONCOST]    Script Date: 16-01-2022 22:31:02 ******/
DROP PROCEDURE [dbo].[usp_INSERTPROMOTIONCOST]
GO
/****** Object:  StoredProcedure [dbo].[USP_GetSubCategories]    Script Date: 16-01-2022 22:31:02 ******/
DROP PROCEDURE [dbo].[USP_GetSubCategories]
GO
/****** Object:  StoredProcedure [dbo].[USP_GetCategories]    Script Date: 16-01-2022 22:31:02 ******/
DROP PROCEDURE [dbo].[USP_GetCategories]
GO
/****** Object:  StoredProcedure [dbo].[USP_GetBrands]    Script Date: 16-01-2022 22:31:02 ******/
DROP PROCEDURE [dbo].[USP_GetBrands]
GO
/****** Object:  StoredProcedure [dbo].[SPUI_GetPromotionCostDetails]    Script Date: 16-01-2022 22:31:02 ******/
DROP PROCEDURE [dbo].[SPUI_GetPromotionCostDetails]
GO
/****** Object:  StoredProcedure [dbo].[SPUI_GetProductDetails]    Script Date: 16-01-2022 22:31:02 ******/
DROP PROCEDURE [dbo].[SPUI_GetProductDetails]
GO
ALTER TABLE [dbo].[PromotionCost] DROP CONSTRAINT [FK_PromotionCost_Supplier]
GO
ALTER TABLE [dbo].[PromotionCost] DROP CONSTRAINT [FK_PromotionCost_PromotionCost]
GO
ALTER TABLE [dbo].[Product] DROP CONSTRAINT [FK_Product_Tax]
GO
ALTER TABLE [dbo].[OrderDetail] DROP CONSTRAINT [FK__OrderDeta__Produ__5DCAEF64]
GO
ALTER TABLE [dbo].[OrderDetail] DROP CONSTRAINT [FK__OrderDeta__Order__5CD6CB2B]
GO
ALTER TABLE [dbo].[Order] DROP CONSTRAINT [FK__Order__Status]
GO
ALTER TABLE [dbo].[Order] DROP CONSTRAINT [FK__Order__CustomerI__59FA5E80]
GO
ALTER TABLE [dbo].[CustomerFavourite] DROP CONSTRAINT [FK__CustomerF__Produ__5629CD9C]
GO
ALTER TABLE [dbo].[CustomerFavourite] DROP CONSTRAINT [FK__CustomerF__Custo__5CD6CB2B]
GO
ALTER TABLE [dbo].[CustomerBasket] DROP CONSTRAINT [FK__CustomerB__Produ__52593CB8]
GO
ALTER TABLE [dbo].[CustomerBasket] DROP CONSTRAINT [FK__CustomerB__Custo__5AEE82B9]
GO
ALTER TABLE [dbo].[Customer] DROP CONSTRAINT [FK__Customer__RoleId__59FA5E80]
GO
ALTER TABLE [dbo].[BaseCost] DROP CONSTRAINT [FK_BaseCost_Supplier]
GO
ALTER TABLE [dbo].[BaseCost] DROP CONSTRAINT [FK_BaseCost_Product]
GO
ALTER TABLE [dbo].[Admin] DROP CONSTRAINT [FK__Admin__RoleId__59063A47]
GO
ALTER TABLE [dbo].[Tax] DROP CONSTRAINT [DF_Tax_IsDelete]
GO
ALTER TABLE [dbo].[Tax] DROP CONSTRAINT [DF_Tax_IsActive]
GO
ALTER TABLE [dbo].[Supplier] DROP CONSTRAINT [DF_Supplier_IsDelete]
GO
ALTER TABLE [dbo].[Supplier] DROP CONSTRAINT [DF_Supplier_IsActive]
GO
ALTER TABLE [dbo].[SubCategory] DROP CONSTRAINT [DF_SubCategory_IsDelete]
GO
ALTER TABLE [dbo].[SubCategory] DROP CONSTRAINT [DF_SubCategory_IsActive]
GO
ALTER TABLE [dbo].[Status] DROP CONSTRAINT [DF_Status_IsDelete]
GO
ALTER TABLE [dbo].[Status] DROP CONSTRAINT [DF_Status_IsActive]
GO
ALTER TABLE [dbo].[Role] DROP CONSTRAINT [DF_Role_IsDelete]
GO
ALTER TABLE [dbo].[Role] DROP CONSTRAINT [DF_Role_IsActive]
GO
ALTER TABLE [dbo].[PromotionCost] DROP CONSTRAINT [DF_PromotionCost_CreatedDate]
GO
ALTER TABLE [dbo].[PromotionCost] DROP CONSTRAINT [DF_PromotionCost_IsActive]
GO
ALTER TABLE [dbo].[Product] DROP CONSTRAINT [Default_IsDelete]
GO
ALTER TABLE [dbo].[Product] DROP CONSTRAINT [Default_IsActive]
GO
ALTER TABLE [dbo].[OrderDetail] DROP CONSTRAINT [DF_OrderDetail_IsDelete]
GO
ALTER TABLE [dbo].[OrderDetail] DROP CONSTRAINT [DF_OrderDetail_IsActive]
GO
ALTER TABLE [dbo].[Order] DROP CONSTRAINT [DF_Order_IsDelete]
GO
ALTER TABLE [dbo].[Order] DROP CONSTRAINT [DF_Order_IsActive]
GO
ALTER TABLE [dbo].[CustomerFavourite] DROP CONSTRAINT [DF_CustomerFavourite_IsDelete]
GO
ALTER TABLE [dbo].[CustomerFavourite] DROP CONSTRAINT [DF_CustomerFavourite_IsActive]
GO
ALTER TABLE [dbo].[CustomerBasket] DROP CONSTRAINT [DF_CustomerBasket_IsDelete]
GO
ALTER TABLE [dbo].[CustomerBasket] DROP CONSTRAINT [DF_CustomerBasket_IsActive]
GO
ALTER TABLE [dbo].[Customer] DROP CONSTRAINT [DF_Customer_IsDelete]
GO
ALTER TABLE [dbo].[Customer] DROP CONSTRAINT [DF_Customer_IsActive]
GO
ALTER TABLE [dbo].[Category] DROP CONSTRAINT [DF_Category_IsDelete]
GO
ALTER TABLE [dbo].[Category] DROP CONSTRAINT [DF_Category_IsActive]
GO
ALTER TABLE [dbo].[Brand] DROP CONSTRAINT [DF_Brand_IsDelete]
GO
ALTER TABLE [dbo].[Brand] DROP CONSTRAINT [DF_Brand_IsActive]
GO
ALTER TABLE [dbo].[BaseCost] DROP CONSTRAINT [DF_BaseCost_CreatedDate]
GO
ALTER TABLE [dbo].[Admin] DROP CONSTRAINT [DF_Admin_IsDelete]
GO
ALTER TABLE [dbo].[Admin] DROP CONSTRAINT [DF_Admin_IsActive]
GO
ALTER TABLE [dbo].[Admin] DROP CONSTRAINT [Default_Admin_EmailSubscribed]
GO
/****** Object:  Table [dbo].[Tax]    Script Date: 16-01-2022 22:31:02 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Tax]') AND type in (N'U'))
DROP TABLE [dbo].[Tax]
GO
/****** Object:  Table [dbo].[Supplier]    Script Date: 16-01-2022 22:31:02 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Supplier]') AND type in (N'U'))
DROP TABLE [dbo].[Supplier]
GO
/****** Object:  Table [dbo].[SubCategory]    Script Date: 16-01-2022 22:31:02 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SubCategory]') AND type in (N'U'))
DROP TABLE [dbo].[SubCategory]
GO
/****** Object:  Table [dbo].[Status]    Script Date: 16-01-2022 22:31:02 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Status]') AND type in (N'U'))
DROP TABLE [dbo].[Status]
GO
/****** Object:  Table [dbo].[Role]    Script Date: 16-01-2022 22:31:02 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Role]') AND type in (N'U'))
DROP TABLE [dbo].[Role]
GO
/****** Object:  Table [dbo].[PromotionCost]    Script Date: 16-01-2022 22:31:02 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PromotionCost]') AND type in (N'U'))
DROP TABLE [dbo].[PromotionCost]
GO
/****** Object:  Table [dbo].[Product]    Script Date: 16-01-2022 22:31:02 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Product]') AND type in (N'U'))
DROP TABLE [dbo].[Product]
GO
/****** Object:  Table [dbo].[OrderDetail]    Script Date: 16-01-2022 22:31:02 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OrderDetail]') AND type in (N'U'))
DROP TABLE [dbo].[OrderDetail]
GO
/****** Object:  Table [dbo].[Order]    Script Date: 16-01-2022 22:31:02 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Order]') AND type in (N'U'))
DROP TABLE [dbo].[Order]
GO
/****** Object:  Table [dbo].[CustomerFavourite]    Script Date: 16-01-2022 22:31:02 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CustomerFavourite]') AND type in (N'U'))
DROP TABLE [dbo].[CustomerFavourite]
GO
/****** Object:  Table [dbo].[CustomerBasket]    Script Date: 16-01-2022 22:31:02 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CustomerBasket]') AND type in (N'U'))
DROP TABLE [dbo].[CustomerBasket]
GO
/****** Object:  Table [dbo].[Customer]    Script Date: 16-01-2022 22:31:02 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Customer]') AND type in (N'U'))
DROP TABLE [dbo].[Customer]
GO
/****** Object:  Table [dbo].[Category]    Script Date: 16-01-2022 22:31:02 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Category]') AND type in (N'U'))
DROP TABLE [dbo].[Category]
GO
/****** Object:  Table [dbo].[Brand]    Script Date: 16-01-2022 22:31:02 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Brand]') AND type in (N'U'))
DROP TABLE [dbo].[Brand]
GO
/****** Object:  Table [dbo].[BaseCost]    Script Date: 16-01-2022 22:31:02 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BaseCost]') AND type in (N'U'))
DROP TABLE [dbo].[BaseCost]
GO
/****** Object:  Table [dbo].[Admin]    Script Date: 16-01-2022 22:31:02 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Admin]') AND type in (N'U'))
DROP TABLE [dbo].[Admin]
GO
/****** Object:  UserDefinedTableType [dbo].[UDTType_PromotionCost]    Script Date: 16-01-2022 22:31:02 ******/
DROP TYPE [dbo].[UDTType_PromotionCost]
GO
USE [master]
GO
/****** Object:  Database [GoodsEnterprise]    Script Date: 16-01-2022 22:31:02 ******/
DROP DATABASE [GoodsEnterprise]
GO
/****** Object:  Database [GoodsEnterprise]    Script Date: 16-01-2022 22:31:02 ******/
CREATE DATABASE [GoodsEnterprise]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'GoodsEnterprise', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\GoodsEnterprise.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'GoodsEnterprise_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\GoodsEnterprise_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO
ALTER DATABASE [GoodsEnterprise] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [GoodsEnterprise].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [GoodsEnterprise] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [GoodsEnterprise] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [GoodsEnterprise] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [GoodsEnterprise] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [GoodsEnterprise] SET ARITHABORT OFF 
GO
ALTER DATABASE [GoodsEnterprise] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [GoodsEnterprise] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [GoodsEnterprise] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [GoodsEnterprise] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [GoodsEnterprise] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [GoodsEnterprise] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [GoodsEnterprise] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [GoodsEnterprise] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [GoodsEnterprise] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [GoodsEnterprise] SET  DISABLE_BROKER 
GO
ALTER DATABASE [GoodsEnterprise] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [GoodsEnterprise] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [GoodsEnterprise] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [GoodsEnterprise] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [GoodsEnterprise] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [GoodsEnterprise] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [GoodsEnterprise] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [GoodsEnterprise] SET RECOVERY FULL 
GO
ALTER DATABASE [GoodsEnterprise] SET  MULTI_USER 
GO
ALTER DATABASE [GoodsEnterprise] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [GoodsEnterprise] SET DB_CHAINING OFF 
GO
ALTER DATABASE [GoodsEnterprise] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [GoodsEnterprise] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [GoodsEnterprise] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [GoodsEnterprise] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'GoodsEnterprise', N'ON'
GO
ALTER DATABASE [GoodsEnterprise] SET QUERY_STORE = OFF
GO
USE [GoodsEnterprise]
GO
/****** Object:  UserDefinedTableType [dbo].[UDTType_PromotionCost]    Script Date: 16-01-2022 22:31:04 ******/
CREATE TYPE [dbo].[UDTType_PromotionCost] AS TABLE(
	[Product] [varchar](500) NULL,
	[OuterBarcode] [varchar](20) NULL,
	[PromotionCost] [numeric](18, 2) NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[SelloutStartDate] [datetime] NULL,
	[SelloutEndDate] [datetime] NULL,
	[BonusDescription] [varchar](500) NULL,
	[SellOutDescription] [varchar](500) NULL,
	[Supplier] [varchar](500) NULL
)
GO
/****** Object:  Table [dbo].[Admin]    Script Date: 16-01-2022 22:31:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Admin](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FirstName] [varchar](250) NULL,
	[LastName] [varchar](250) NULL,
	[Email] [varchar](50) NOT NULL,
	[Password] [varchar](500) NOT NULL,
	[Description] [varchar](1000) NULL,
	[RoleId] [int] NOT NULL,
	[IsEmailSubscribed] [bit] NOT NULL,
	[CreatedDate] [datetime] NULL,
	[Createdby] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	[Modifiedby] [int] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDelete] [bit] NOT NULL,
 CONSTRAINT [PK_Admin] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BaseCost]    Script Date: 16-01-2022 22:31:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BaseCost](
	[BaseCostID] [int] IDENTITY(1,1) NOT NULL,
	[ProductID] [int] NULL,
	[BaseCost] [numeric](18, 2) NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[Remark] [varchar](500) NULL,
	[SupplierID] [int] NULL,
	[IsActive] [bit] NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedBy] [int] NULL,
	[Modifiedby] [int] NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_BaseCost] PRIMARY KEY CLUSTERED 
(
	[BaseCostID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Brand]    Script Date: 16-01-2022 22:31:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Brand](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](250) NOT NULL,
	[Description] [varchar](1000) NULL,
	[ImageUrl_500] [varchar](200) NULL,
	[ImageUrl_200] [varchar](200) NULL,
	[CreatedDate] [datetime] NULL,
	[Createdby] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	[Modifiedby] [int] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDelete] [bit] NOT NULL,
 CONSTRAINT [PK_Brand] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Category]    Script Date: 16-01-2022 22:31:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Category](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](250) NOT NULL,
	[Description] [varchar](1000) NULL,
	[CreatedDate] [datetime] NULL,
	[Createdby] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	[Modifiedby] [int] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDelete] [bit] NOT NULL,
 CONSTRAINT [PK_Category] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Customer]    Script Date: 16-01-2022 22:31:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Customer](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FirstName] [varchar](250) NULL,
	[LastName] [varchar](250) NULL,
	[Address1] [varchar](250) NULL,
	[Address2] [varchar](250) NULL,
	[CompanyName] [varchar](250) NULL,
	[CompanyEmail] [varchar](250) NULL,
	[CompanyPhone] [varchar](15) NULL,
	[CompanyFax] [varchar](15) NULL,
	[ContactPerson] [varchar](250) NULL,
	[City] [varchar](50) NULL,
	[County] [varchar](50) NULL,
	[PostalCode] [varchar](15) NULL,
	[Country] [varchar](50) NULL,
	[MobilePhone] [varchar](15) NULL,
	[HomePhone] [varchar](15) NULL,
	[Email] [varchar](50) NOT NULL,
	[Password] [varchar](500) NOT NULL,
	[Description] [varchar](1000) NULL,
	[RoleId] [int] NOT NULL,
	[EmailSubscribed] [bit] NOT NULL,
	[CreatedDate] [datetime] NULL,
	[Createdby] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	[Modifiedby] [int] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDelete] [bit] NOT NULL,
	[PasswordExpiryDate] [datetime] NULL,
 CONSTRAINT [PK_Customer] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CustomerBasket]    Script Date: 16-01-2022 22:31:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CustomerBasket](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CustomerId] [int] NOT NULL,
	[ProductId] [int] NOT NULL,
	[CreatedDate] [datetime] NULL,
	[Createdby] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	[Modifiedby] [int] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDelete] [bit] NOT NULL,
 CONSTRAINT [PK_CustomerBasket] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CustomerFavourite]    Script Date: 16-01-2022 22:31:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CustomerFavourite](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CustomerId] [int] NOT NULL,
	[ProductId] [int] NOT NULL,
	[CreatedDate] [datetime] NULL,
	[Createdby] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	[Modifiedby] [int] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDelete] [bit] NOT NULL,
 CONSTRAINT [PK_CustomerFavourite] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Order]    Script Date: 16-01-2022 22:31:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Order](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StatusId] [int] NOT NULL,
	[DatePlaced] [datetime] NULL,
	[CustomerId] [int] NOT NULL,
	[Cases] [decimal](16, 2) NULL,
	[KG] [decimal](16, 2) NULL,
	[CBM] [decimal](16, 2) NULL,
	[GBP] [decimal](16, 2) NULL,
	[CreatedDate] [datetime] NULL,
	[Createdby] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	[Modifiedby] [int] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDelete] [bit] NOT NULL,
 CONSTRAINT [PK_Order] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderDetail]    Script Date: 16-01-2022 22:31:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderDetail](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OrderId] [int] NOT NULL,
	[ProductId] [int] NOT NULL,
	[TotalPrice] [decimal](16, 2) NULL,
	[IsPricingRequried] [bit] NULL,
	[CreatedDate] [datetime] NULL,
	[Createdby] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	[Modifiedby] [int] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDelete] [bit] NOT NULL,
 CONSTRAINT [PK_OrderDetail] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Product]    Script Date: 16-01-2022 22:31:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Product](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](25) NOT NULL,
	[BrandId] [int] NULL,
	[CategoryId] [int] NULL,
	[SubCategoryId] [int] NULL,
	[InnerEAN] [varchar](25) NULL,
	[OuterEAN] [varchar](25) NOT NULL,
	[UnitSize] [varchar](25) NULL,
	[UPC] [int] NULL,
	[LayerQuantity] [int] NULL,
	[PalletQuantity] [int] NULL,
	[CasePrice] [decimal](16, 2) NULL,
	[ShelfLifeInWeeks] [int] NULL,
	[PackHeight] [decimal](16, 2) NULL,
	[PackDepth] [decimal](16, 2) NULL,
	[PackWidth] [decimal](16, 2) NULL,
	[NetCaseWeightKg] [decimal](16, 2) NULL,
	[GrossCaseWeightKg] [decimal](16, 2) NULL,
	[CaseWidthMm] [decimal](16, 2) NULL,
	[CaseHeightMm] [decimal](16, 2) NULL,
	[CaseDepthMm] [decimal](16, 2) NULL,
	[PalletWeightKg] [decimal](16, 2) NULL,
	[PalletWidthMeter] [decimal](16, 2) NULL,
	[PalletHeightMeter] [decimal](16, 2) NULL,
	[PalletDepthMeter] [decimal](16, 2) NULL,
	[Image] [varchar](200) NULL,
	[CreatedDate] [datetime] NULL,
	[Createdby] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	[Modifiedby] [int] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDelete] [bit] NOT NULL,
	[SupplierId] [int] NULL,
	[ExpriyDate] [datetime] NULL,
	[ProductName] [varchar](250) NULL,
	[ProductDescription] [varchar](500) NULL,
	[TaxslabID] [int] NULL,
 CONSTRAINT [PK_Product] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PromotionCost]    Script Date: 16-01-2022 22:31:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PromotionCost](
	[PromotionCostID] [int] IDENTITY(1,1) NOT NULL,
	[ProductID] [int] NULL,
	[PromotionCost] [numeric](18, 2) NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[SelloutStartDate] [datetime] NULL,
	[SelloutEndDate] [datetime] NULL,
	[BonusDescription] [varchar](500) NULL,
	[SellOutDescription] [varchar](500) NULL,
	[Remark] [varchar](500) NULL,
	[SupplierID] [int] NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedBy] [int] NULL,
	[Modifiedby] [int] NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_PromotionCost] PRIMARY KEY CLUSTERED 
(
	[PromotionCostID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Role]    Script Date: 16-01-2022 22:31:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Role](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](250) NOT NULL,
	[CreatedDate] [datetime] NULL,
	[Createdby] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	[Modifiedby] [int] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDelete] [bit] NOT NULL,
 CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Status]    Script Date: 16-01-2022 22:31:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Status](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Value] [varchar](50) NOT NULL,
	[CreatedDate] [datetime] NULL,
	[Createdby] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	[Modifiedby] [int] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDelete] [bit] NOT NULL,
 CONSTRAINT [PK_Status] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SubCategory]    Script Date: 16-01-2022 22:31:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SubCategory](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](250) NOT NULL,
	[Description] [varchar](1000) NULL,
	[CreatedDate] [datetime] NULL,
	[Createdby] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	[Modifiedby] [int] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDelete] [bit] NOT NULL,
 CONSTRAINT [PK_SubCategory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Supplier]    Script Date: 16-01-2022 22:31:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Supplier](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](250) NOT NULL,
	[SKUCode] [varchar](20) NULL,
	[FirstName] [varchar](250) NULL,
	[LastName] [varchar](250) NULL,
	[Address1] [varchar](250) NULL,
	[Address2] [varchar](250) NULL,
	[Phone] [varchar](15) NULL,
	[Email] [varchar](50) NULL,
	[Description] [varchar](1000) NULL,
	[CreatedDate] [datetime] NULL,
	[Createdby] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	[Modifiedby] [int] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDelete] [bit] NOT NULL,
 CONSTRAINT [PK_Supplier] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Tax]    Script Date: 16-01-2022 22:31:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tax](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](250) NOT NULL,
	[Value] [decimal](16, 2) NULL,
	[Description] [varchar](1000) NULL,
	[CreatedDate] [datetime] NULL,
	[Createdby] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	[Modifiedby] [int] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDelete] [bit] NOT NULL,
 CONSTRAINT [PK_Tax] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Admin] ADD  CONSTRAINT [Default_Admin_EmailSubscribed]  DEFAULT ((0)) FOR [IsEmailSubscribed]
GO
ALTER TABLE [dbo].[Admin] ADD  CONSTRAINT [DF_Admin_IsActive]  DEFAULT ((0)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Admin] ADD  CONSTRAINT [DF_Admin_IsDelete]  DEFAULT ((0)) FOR [IsDelete]
GO
ALTER TABLE [dbo].[BaseCost] ADD  CONSTRAINT [DF_BaseCost_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[Brand] ADD  CONSTRAINT [DF_Brand_IsActive]  DEFAULT ((0)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Brand] ADD  CONSTRAINT [DF_Brand_IsDelete]  DEFAULT ((0)) FOR [IsDelete]
GO
ALTER TABLE [dbo].[Category] ADD  CONSTRAINT [DF_Category_IsActive]  DEFAULT ((0)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Category] ADD  CONSTRAINT [DF_Category_IsDelete]  DEFAULT ((0)) FOR [IsDelete]
GO
ALTER TABLE [dbo].[Customer] ADD  CONSTRAINT [DF_Customer_IsActive]  DEFAULT ((0)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Customer] ADD  CONSTRAINT [DF_Customer_IsDelete]  DEFAULT ((0)) FOR [IsDelete]
GO
ALTER TABLE [dbo].[CustomerBasket] ADD  CONSTRAINT [DF_CustomerBasket_IsActive]  DEFAULT ((0)) FOR [IsActive]
GO
ALTER TABLE [dbo].[CustomerBasket] ADD  CONSTRAINT [DF_CustomerBasket_IsDelete]  DEFAULT ((0)) FOR [IsDelete]
GO
ALTER TABLE [dbo].[CustomerFavourite] ADD  CONSTRAINT [DF_CustomerFavourite_IsActive]  DEFAULT ((0)) FOR [IsActive]
GO
ALTER TABLE [dbo].[CustomerFavourite] ADD  CONSTRAINT [DF_CustomerFavourite_IsDelete]  DEFAULT ((0)) FOR [IsDelete]
GO
ALTER TABLE [dbo].[Order] ADD  CONSTRAINT [DF_Order_IsActive]  DEFAULT ((0)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Order] ADD  CONSTRAINT [DF_Order_IsDelete]  DEFAULT ((0)) FOR [IsDelete]
GO
ALTER TABLE [dbo].[OrderDetail] ADD  CONSTRAINT [DF_OrderDetail_IsActive]  DEFAULT ((0)) FOR [IsActive]
GO
ALTER TABLE [dbo].[OrderDetail] ADD  CONSTRAINT [DF_OrderDetail_IsDelete]  DEFAULT ((0)) FOR [IsDelete]
GO
ALTER TABLE [dbo].[Product] ADD  CONSTRAINT [Default_IsActive]  DEFAULT ((0)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Product] ADD  CONSTRAINT [Default_IsDelete]  DEFAULT ((0)) FOR [IsDelete]
GO
ALTER TABLE [dbo].[PromotionCost] ADD  CONSTRAINT [DF_PromotionCost_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[PromotionCost] ADD  CONSTRAINT [DF_PromotionCost_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[Role] ADD  CONSTRAINT [DF_Role_IsActive]  DEFAULT ((0)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Role] ADD  CONSTRAINT [DF_Role_IsDelete]  DEFAULT ((0)) FOR [IsDelete]
GO
ALTER TABLE [dbo].[Status] ADD  CONSTRAINT [DF_Status_IsActive]  DEFAULT ((0)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Status] ADD  CONSTRAINT [DF_Status_IsDelete]  DEFAULT ((0)) FOR [IsDelete]
GO
ALTER TABLE [dbo].[SubCategory] ADD  CONSTRAINT [DF_SubCategory_IsActive]  DEFAULT ((0)) FOR [IsActive]
GO
ALTER TABLE [dbo].[SubCategory] ADD  CONSTRAINT [DF_SubCategory_IsDelete]  DEFAULT ((0)) FOR [IsDelete]
GO
ALTER TABLE [dbo].[Supplier] ADD  CONSTRAINT [DF_Supplier_IsActive]  DEFAULT ((0)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Supplier] ADD  CONSTRAINT [DF_Supplier_IsDelete]  DEFAULT ((0)) FOR [IsDelete]
GO
ALTER TABLE [dbo].[Tax] ADD  CONSTRAINT [DF_Tax_IsActive]  DEFAULT ((0)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Tax] ADD  CONSTRAINT [DF_Tax_IsDelete]  DEFAULT ((0)) FOR [IsDelete]
GO
ALTER TABLE [dbo].[Admin]  WITH CHECK ADD FOREIGN KEY([RoleId])
REFERENCES [dbo].[Role] ([Id])
GO
ALTER TABLE [dbo].[BaseCost]  WITH CHECK ADD  CONSTRAINT [FK_BaseCost_Product] FOREIGN KEY([ProductID])
REFERENCES [dbo].[Product] ([Id])
GO
ALTER TABLE [dbo].[BaseCost] CHECK CONSTRAINT [FK_BaseCost_Product]
GO
ALTER TABLE [dbo].[BaseCost]  WITH CHECK ADD  CONSTRAINT [FK_BaseCost_Supplier] FOREIGN KEY([SupplierID])
REFERENCES [dbo].[Supplier] ([Id])
GO
ALTER TABLE [dbo].[BaseCost] CHECK CONSTRAINT [FK_BaseCost_Supplier]
GO
ALTER TABLE [dbo].[Customer]  WITH CHECK ADD  CONSTRAINT [FK__Customer__RoleId__59FA5E80] FOREIGN KEY([RoleId])
REFERENCES [dbo].[Role] ([Id])
GO
ALTER TABLE [dbo].[Customer] CHECK CONSTRAINT [FK__Customer__RoleId__59FA5E80]
GO
ALTER TABLE [dbo].[CustomerBasket]  WITH CHECK ADD  CONSTRAINT [FK__CustomerB__Custo__5AEE82B9] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customer] ([Id])
GO
ALTER TABLE [dbo].[CustomerBasket] CHECK CONSTRAINT [FK__CustomerB__Custo__5AEE82B9]
GO
ALTER TABLE [dbo].[CustomerBasket]  WITH CHECK ADD  CONSTRAINT [FK__CustomerB__Produ__52593CB8] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([Id])
GO
ALTER TABLE [dbo].[CustomerBasket] CHECK CONSTRAINT [FK__CustomerB__Produ__52593CB8]
GO
ALTER TABLE [dbo].[CustomerFavourite]  WITH CHECK ADD  CONSTRAINT [FK__CustomerF__Custo__5CD6CB2B] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customer] ([Id])
GO
ALTER TABLE [dbo].[CustomerFavourite] CHECK CONSTRAINT [FK__CustomerF__Custo__5CD6CB2B]
GO
ALTER TABLE [dbo].[CustomerFavourite]  WITH CHECK ADD  CONSTRAINT [FK__CustomerF__Produ__5629CD9C] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([Id])
GO
ALTER TABLE [dbo].[CustomerFavourite] CHECK CONSTRAINT [FK__CustomerF__Produ__5629CD9C]
GO
ALTER TABLE [dbo].[Order]  WITH CHECK ADD  CONSTRAINT [FK__Order__CustomerI__59FA5E80] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customer] ([Id])
GO
ALTER TABLE [dbo].[Order] CHECK CONSTRAINT [FK__Order__CustomerI__59FA5E80]
GO
ALTER TABLE [dbo].[Order]  WITH CHECK ADD  CONSTRAINT [FK__Order__Status] FOREIGN KEY([StatusId])
REFERENCES [dbo].[Status] ([Id])
GO
ALTER TABLE [dbo].[Order] CHECK CONSTRAINT [FK__Order__Status]
GO
ALTER TABLE [dbo].[OrderDetail]  WITH CHECK ADD  CONSTRAINT [FK__OrderDeta__Order__5CD6CB2B] FOREIGN KEY([OrderId])
REFERENCES [dbo].[Order] ([Id])
GO
ALTER TABLE [dbo].[OrderDetail] CHECK CONSTRAINT [FK__OrderDeta__Order__5CD6CB2B]
GO
ALTER TABLE [dbo].[OrderDetail]  WITH CHECK ADD  CONSTRAINT [FK__OrderDeta__Produ__5DCAEF64] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([Id])
GO
ALTER TABLE [dbo].[OrderDetail] CHECK CONSTRAINT [FK__OrderDeta__Produ__5DCAEF64]
GO
ALTER TABLE [dbo].[Product]  WITH CHECK ADD  CONSTRAINT [FK_Product_Tax] FOREIGN KEY([TaxslabID])
REFERENCES [dbo].[Tax] ([Id])
GO
ALTER TABLE [dbo].[Product] CHECK CONSTRAINT [FK_Product_Tax]
GO
ALTER TABLE [dbo].[PromotionCost]  WITH CHECK ADD  CONSTRAINT [FK_PromotionCost_PromotionCost] FOREIGN KEY([ProductID])
REFERENCES [dbo].[Product] ([Id])
GO
ALTER TABLE [dbo].[PromotionCost] CHECK CONSTRAINT [FK_PromotionCost_PromotionCost]
GO
ALTER TABLE [dbo].[PromotionCost]  WITH CHECK ADD  CONSTRAINT [FK_PromotionCost_Supplier] FOREIGN KEY([SupplierID])
REFERENCES [dbo].[Supplier] ([Id])
GO
ALTER TABLE [dbo].[PromotionCost] CHECK CONSTRAINT [FK_PromotionCost_Supplier]
GO
/****** Object:  StoredProcedure [dbo].[SPUI_GetProductDetails]    Script Date: 16-01-2022 22:31:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE   PROCEDURE [dbo].[SPUI_GetProductDetails]
 @OffsetValue int,
@PagingSize int,
@sortOrder varchar(10),
@sortColumn varchar(100),

@SearchText varchar(250)
 
AS
BEGIN
	DECLARE @FilterTotalCount INT=0
	SET @FilterTotalCount =(SELECT COUNT(*)
	FROM Product[A] INNER JOIN Brand[B] ON A.BrandId=B.Id
	INNER JOIN Category[C] ON A.CategoryId=C.Id
	LEFT JOIN SubCategory[D] ON A.SubCategoryId=D.Id
	WHERE A.IsActive=1 AND B.IsActive=1 AND C.IsActive=1 AND ISNULL(D.IsActive,1)=1)

	SELECT a.Code,A.OuterEAN, CASE WHEN A.IsActive=1 THEN 'Active' ELSE 'InActive' end Status,A.Id
--	,A.ProductName,
--	A.ProductDescription,A.BrandId,A.CategoryId,A.SubCategoryId,InnerEAN,UnitSize,UPC,LayerQuantity,PalletQuantity,CasePrice,
--ShelfLifeInWeeks,PackHeight,PackDepth,PackWidth,NetCaseWeightKg,GrossCaseWeightKg,CaseWidthMm,CaseHeightMm,CaseDepthMm
--,PalletWeightKg,PalletWidthMeter,PalletHeightMeter,PalletDepthMeter,Image,A.CreatedDate,A.Createdby,A.ExpriyDate
,@FilterTotalCount AS FilterTotalCount
--,A.IsActive,A.IsDelete,A.Modifiedby,A.ModifiedDate,A.SupplierId
	FROM Product[A] INNER JOIN Brand[B] ON A.BrandId=B.Id
	INNER JOIN Category[C] ON A.CategoryId=C.Id
	LEFT JOIN SubCategory[D] ON A.SubCategoryId=D.Id
	WHERE A.IsActive=1 AND B.IsActive=1 AND C.IsActive=1 AND ISNULL(D.IsActive,1)=1
	ORDER BY A.Id
	OFFSET ((@OffsetValue - 1) * @PagingSize) ROWS
	FETCH NEXT @PagingSize ROWS ONLY;
END
GO
/****** Object:  StoredProcedure [dbo].[SPUI_GetPromotionCostDetails]    Script Date: 16-01-2022 22:31:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SPUI_GetPromotionCostDetails]
 @OffsetValue int,
@PagingSize int,
@sortOrder varchar(10),
@sortColumn varchar(100),

@SearchText varchar(250)
AS
BEGIN
	DECLARE @FilterTotalCount INT=0
	SET @FilterTotalCount =(select COUNT(*) from (
	select  A.ProductID,A.PromotionCost,A.PromotionCostID,A.SupplierID,C.Name[SupplierName],B.ProductName,A.StartDate,A.EndDate from PromotionCost[A] inner join Product[B] On A.ProductID=B.Id
	inner join Supplier[C] On A.SupplierID=C.Id
	where A.IsActive=1
	group by A.ProductID,A.PromotionCost,A.PromotionCostID,A.SupplierID,C.Name,B.ProductName ,A.StartDate,A.EndDate
	) M ) 


	select distinct A.ProductID,A.PromotionCost,A.PromotionCostID,A.SupplierID,C.Name[SupplierName],B.ProductName,A.StartDate,A.EndDate ,
	CASE WHEN A.IsActive=1 THEN 'Active' ELSE 'InActive' end Status
	,@FilterTotalCount AS FilterTotalCount 
	from PromotionCost[A] 
	inner join Product[B] On A.ProductID=B.Id
	inner join Supplier[C] On A.SupplierID=C.Id
	where A.IsActive=1
	 
	order by A.EndDate desc
	OFFSET ((@OffsetValue - 1) * @PagingSize) ROWS
	FETCH NEXT @PagingSize ROWS ONLY;

 
END
GO
/****** Object:  StoredProcedure [dbo].[USP_GetBrands]    Script Date: 16-01-2022 22:31:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[USP_GetBrands] 
@PageNumber INT = 1,
@PageSize INT = 5,
@OrderByDescending BIT = 0,
@OrderBy VARCHAR(250) = 'NAME',
@SearchBy VARCHAR(250) = NULL,
@TotalRecords INT = 0 OUT
AS
BEGIN

SELECT * FROM Brand WITH (NOLOCK)
WHERE (IsDelete IS NULL OR IsDelete = 0) AND
((@SearchBy IS NULL OR @SearchBy = '') OR [Name] LIKE @SearchBy + '%')
ORDER BY --CreatedDate

CASE WHEN @OrderBy = 'NAME' AND @OrderByDescending = 0 THEN [Name] END ASC,
CASE WHEN @OrderBy = 'NAME' AND @OrderByDescending = 1 THEN [Name] END DESC,
CASE WHEN @OrderBy = 'DATE' AND @OrderByDescending = 0 THEN CreatedDate END ASC,
CASE WHEN @OrderBy = 'DATE' AND @OrderByDescending = 1 THEN CreatedDate END DESC

 -- CASE 
	--WHEN @OrderByDescending = 0 THEN		
	--	CASE
	--		WHEN @OrderBy = 'Name' THEN [Name]
	--		WHEN @OrderBy = 'Date' THEN CreatedDate
	--	END
	--END ASC,
	--CASE 
	--WHEN @OrderByDescending = 1 THEN	
	--	CASE
	--		WHEN @OrderBy = 'Name' THEN [Name]
	--		WHEN @OrderBy = 'Date' THEN CreatedDate
	--    END 
	--END DESC
OFFSET @PageSize * (@PageNumber - 1) ROW
FETCH NEXT @PageSize ROWS ONLY

SELECT @TotalRecords = COUNT(Id) FROM Brand WITH (NOLOCK)
	WHERE (IsDelete IS NULL OR IsDelete = 0) AND
	((@SearchBy IS NULL OR @SearchBy = '') OR [Name] LIKE @SearchBy + '%')

END

GO
/****** Object:  StoredProcedure [dbo].[USP_GetCategories]    Script Date: 16-01-2022 22:31:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[USP_GetCategories] 
@PageNumber INT = 1,
@PageSize INT = 5,
@OrderByDescending BIT = 0,
@OrderBy VARCHAR(250) = 'NAME',
@SearchBy VARCHAR(250) = NULL,
@TotalRecords INT = 0 OUT
AS
BEGIN

SELECT * FROM Category WITH (NOLOCK)
WHERE (IsDelete IS NULL OR IsDelete = 0) AND
((@SearchBy IS NULL OR @SearchBy = '') OR [Name] LIKE @SearchBy + '%')
ORDER BY --CreatedDate

CASE WHEN @OrderBy = 'NAME' AND @OrderByDescending = 0 THEN [Name] END ASC,
CASE WHEN @OrderBy = 'NAME' AND @OrderByDescending = 1 THEN [Name] END DESC,
CASE WHEN @OrderBy = 'DATE' AND @OrderByDescending = 0 THEN CreatedDate END ASC,
CASE WHEN @OrderBy = 'DATE' AND @OrderByDescending = 1 THEN CreatedDate END DESC

 -- CASE 
	--WHEN @OrderByDescending = 0 THEN		
	--	CASE
	--		WHEN @OrderBy = 'Name' THEN [Name]
	--		WHEN @OrderBy = 'Date' THEN CreatedDate
	--	END
	--END ASC,
	--CASE 
	--WHEN @OrderByDescending = 1 THEN	
	--	CASE
	--		WHEN @OrderBy = 'Name' THEN [Name]
	--		WHEN @OrderBy = 'Date' THEN CreatedDate
	--    END 
	--END DESC
OFFSET @PageSize * (@PageNumber - 1) ROW
FETCH NEXT @PageSize ROWS ONLY

SELECT @TotalRecords = COUNT(Id) FROM Category WITH (NOLOCK)
	WHERE (IsDelete IS NULL OR IsDelete = 0) AND
	((@SearchBy IS NULL OR @SearchBy = '') OR [Name] LIKE @SearchBy + '%')

END

GO
/****** Object:  StoredProcedure [dbo].[USP_GetSubCategories]    Script Date: 16-01-2022 22:31:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[USP_GetSubCategories] 
@PageNumber INT = 1,
@PageSize INT = 5,
@OrderByDescending BIT = 0,
@OrderBy VARCHAR(250) = 'NAME',
@SearchBy VARCHAR(250) = NULL,
@TotalRecords INT = 0 OUT
AS
BEGIN

SELECT * FROM SubCategory WITH (NOLOCK)
WHERE (IsDelete IS NULL OR IsDelete = 0) AND
((@SearchBy IS NULL OR @SearchBy = '') OR [Name] LIKE @SearchBy + '%')
ORDER BY --CreatedDate

CASE WHEN @OrderBy = 'NAME' AND @OrderByDescending = 0 THEN [Name] END ASC,
CASE WHEN @OrderBy = 'NAME' AND @OrderByDescending = 1 THEN [Name] END DESC,
CASE WHEN @OrderBy = 'DATE' AND @OrderByDescending = 0 THEN CreatedDate END ASC,
CASE WHEN @OrderBy = 'DATE' AND @OrderByDescending = 1 THEN CreatedDate END DESC

 -- CASE 
	--WHEN @OrderByDescending = 0 THEN		
	--	CASE
	--		WHEN @OrderBy = 'Name' THEN [Name]
	--		WHEN @OrderBy = 'Date' THEN CreatedDate
	--	END
	--END ASC,
	--CASE 
	--WHEN @OrderByDescending = 1 THEN	
	--	CASE
	--		WHEN @OrderBy = 'Name' THEN [Name]
	--		WHEN @OrderBy = 'Date' THEN CreatedDate
	--    END 
	--END DESC
OFFSET @PageSize * (@PageNumber - 1) ROW
FETCH NEXT @PageSize ROWS ONLY

SELECT @TotalRecords = COUNT(Id) FROM SubCategory WITH (NOLOCK)
	WHERE (IsDelete IS NULL OR IsDelete = 0) AND
	((@SearchBy IS NULL OR @SearchBy = '') OR [Name] LIKE @SearchBy + '%')

END

GO
/****** Object:  StoredProcedure [dbo].[usp_INSERTPROMOTIONCOST]    Script Date: 16-01-2022 22:31:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE   PROCEDURE [dbo].[usp_INSERTPROMOTIONCOST]
	@UDTType_PromotionCost as dbo.UDTType_PromotionCost READONLY,
	@CREATEDBY INT
AS
BEGIN
	INSERT INTO PromotionCost(ProductID,PromotionCost,StartDate,EndDate,SelloutStartDate,SelloutEndDate,BonusDescription,SellOutDescription,Remark,SupplierID,IsActive,CreatedDate,CreatedBy)
	SELECT b.Id,A.PromotionCost,[StartDate],[EndDate],[SelloutStartDate],[SelloutEndDate],[BonusDescription],[SellOutDescription],'' ,C.Id,1,GETDATE(),@CREATEDBY
	FROM @UDTType_PromotionCost[A] INNER JOIN Product[B] ON A.OuterBarcode=b.OuterEAN
	INNER JOIN Supplier[C] ON A.Supplier=C.Name
	--WHERE PromotionCost.ProductID NOT IN 
	select 1
END
GO
USE [master]
GO
ALTER DATABASE [GoodsEnterprise] SET  READ_WRITE 
GO
