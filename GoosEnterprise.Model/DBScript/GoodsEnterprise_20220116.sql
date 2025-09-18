
/****** Object:  Database [GoodsEnterprise]    Script Date: 18-09-2025 10:04:05 ******/
CREATE DATABASE [GoodsEnterprise]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'GoodsEnterprise', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\GoodsEnterprise.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'GoodsEnterprise_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\GoodsEnterprise_log.ldf' , SIZE = 73728KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
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
/****** Object:  User [IIS APPPOOL\Test1]    Script Date: 18-09-2025 10:04:05 ******/
CREATE USER [IIS APPPOOL\Test1] FOR LOGIN [IIS APPPOOL\Test1] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_owner] ADD MEMBER [IIS APPPOOL\Test1]
GO
/****** Object:  UserDefinedTableType [dbo].[UDTType_PromotionCost]    Script Date: 18-09-2025 10:04:06 ******/
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
/****** Object:  Table [dbo].[Admin]    Script Date: 18-09-2025 10:04:06 ******/
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
	[ResetPasswordToken] [varchar](500) NULL,
	[ResetPasswordExpiry] [datetime] NULL,
 CONSTRAINT [PK_Admin] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BaseCost]    Script Date: 18-09-2025 10:04:06 ******/
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
/****** Object:  Table [dbo].[Brand]    Script Date: 18-09-2025 10:04:06 ******/
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
/****** Object:  Table [dbo].[Category]    Script Date: 18-09-2025 10:04:06 ******/
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
	[ImageUrl_500] [varchar](200) NULL,
	[ImageUrl_200] [varchar](200) NULL,
 CONSTRAINT [PK_Category] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Customer]    Script Date: 18-09-2025 10:04:06 ******/
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
	[CompanyEmail] [varchar](50) NULL,
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
/****** Object:  Table [dbo].[CustomerBasket]    Script Date: 18-09-2025 10:04:06 ******/
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
/****** Object:  Table [dbo].[CustomerFavourite]    Script Date: 18-09-2025 10:04:06 ******/
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
/****** Object:  Table [dbo].[Order]    Script Date: 18-09-2025 10:04:06 ******/
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
/****** Object:  Table [dbo].[OrderDetail]    Script Date: 18-09-2025 10:04:06 ******/
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
/****** Object:  Table [dbo].[Product]    Script Date: 18-09-2025 10:04:06 ******/
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
	[seebelow] [varchar](500) NULL,
	[seebelow1] [varchar](500) NULL,
	[isTaxable] [bit] NOT NULL,
 CONSTRAINT [PK_Product] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PromotionCost]    Script Date: 18-09-2025 10:04:06 ******/
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
	[IsDelete] [bit] NOT NULL,
 CONSTRAINT [PK_PromotionCost] PRIMARY KEY CLUSTERED 
(
	[PromotionCostID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Role]    Script Date: 18-09-2025 10:04:06 ******/
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
/****** Object:  Table [dbo].[Status]    Script Date: 18-09-2025 10:04:06 ******/
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
/****** Object:  Table [dbo].[SubCategory]    Script Date: 18-09-2025 10:04:06 ******/
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
/****** Object:  Table [dbo].[Supplier]    Script Date: 18-09-2025 10:04:06 ******/
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
	[IsPreferred] [bit] NULL,
	[LeadTimeDays] [int] NULL,
	[MoqCase] [varchar](100) NULL,
	[LastCost] [decimal](18, 0) NULL,
	[Incoterm] [varchar](200) NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
 CONSTRAINT [PK_Supplier] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Tax]    Script Date: 18-09-2025 10:04:06 ******/
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
ALTER TABLE [dbo].[Product] ADD  DEFAULT ((0)) FOR [isTaxable]
GO
ALTER TABLE [dbo].[PromotionCost] ADD  CONSTRAINT [DF_PromotionCost_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[PromotionCost] ADD  CONSTRAINT [DF_PromotionCost_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[PromotionCost] ADD  CONSTRAINT [DF_PromotionCost_IsDelete]  DEFAULT ((0)) FOR [IsDelete]
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
/****** Object:  StoredProcedure [dbo].[SPUI_GetHomePageBrands]    Script Date: 18-09-2025 10:04:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE    PROCEDURE [dbo].[SPUI_GetHomePageBrands]
AS
BEGIN

  SELECT prd.BrandId AS BrandId, brd.Name AS BrandName, COUNT(prd.id) AS ProductCount FROM Brand brd WITH (NOLOCK) JOIN Product prd WITH (NOLOCK) ON brd.Id = prd.BrandId
  WHERE brd.IsDelete = 0 and brd.IsActive = 1 and prd.IsDelete = 0 and prd.IsActive = 1
  GROUP BY prd.BrandId, brd.Name

END
 
GO
/****** Object:  StoredProcedure [dbo].[SPUI_GetHomePageCategories]    Script Date: 18-09-2025 10:04:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[SPUI_GetHomePageCategories]
AS
BEGIN

  SELECT prd.CategoryId AS CategoryId, cat.Name AS CategoryName, COUNT(prd.id) AS ProductCount FROM Category cat WITH (NOLOCK) JOIN Product prd  WITH (NOLOCK) ON cat.Id = prd.CategoryId
  WHERE cat.IsDelete = 0 and cat.IsActive = 1 and prd.IsDelete = 0 and prd.IsActive = 1
  GROUP BY prd.CategoryId, cat.Name

END
GO
/****** Object:  StoredProcedure [dbo].[SPUI_GetProductDetails]    Script Date: 18-09-2025 10:04:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE   PROCEDURE [dbo].[SPUI_GetProductDetails]  --0, 10, 'DESC', 'ModifiedDate', '', 'All'
                                           @OffsetValue int,
    @PagingSize int,
    @sortOrder varchar(10),
    @sortColumn varchar(100),
    @SearchText varchar(250),
    @SearchBy varchar(50) = 'All'
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Declare variables
    DECLARE @WhereCondition NVARCHAR(MAX) = ''
    DECLARE @OrderByClause NVARCHAR(50) = 'P.ModifiedDate'
    DECLARE @TotalRecords INT = 0
    
    -- Build search condition based on SearchBy parameter
    IF @SearchText IS NOT NULL AND LTRIM(RTRIM(@SearchText)) != ''
    BEGIN
        SET @SearchText = '%' + LTRIM(RTRIM(@SearchText)) + '%'
        
        IF @SearchBy = 'ProductName'
            SET @WhereCondition = 'AND P.ProductName LIKE @SearchText'
        ELSE IF @SearchBy = 'CategoryName'
            SET @WhereCondition = 'AND C.Name LIKE @SearchText'
        ELSE IF @SearchBy = 'BrandName'
            SET @WhereCondition = 'AND B.Name LIKE @SearchText'
        ELSE IF @SearchBy = 'Code'
            SET @WhereCondition = 'AND P.Code LIKE @SearchText'
        ELSE IF @SearchBy = 'OuterEan'
            SET @WhereCondition = 'AND P.OuterEAN LIKE @SearchText'
        ELSE -- 'All' - search across all fields
            SET @WhereCondition = 'AND (P.Code LIKE @SearchText OR P.ProductName LIKE @SearchText OR C.Name LIKE @SearchText OR B.Name LIKE @SearchText OR P.OuterEAN LIKE @SearchText)'
    END
    
    -- Build ORDER BY clause
    IF @sortColumn = 'code'
        SET @OrderByClause = 'P.Code'
    ELSE IF @sortColumn = 'productName'
        SET @OrderByClause = 'P.ProductName'
    ELSE IF @sortColumn = 'categoryName'
        SET @OrderByClause = 'C.Name'
    ELSE IF @sortColumn = 'brandName'
        SET @OrderByClause = 'B.Name'
    ELSE IF @sortColumn = 'outerEan'
        SET @OrderByClause = 'P.OuterEAN'
    ELSE IF @sortColumn = 'status'
        SET @OrderByClause = 'P.IsActive'
    ELSE IF @sortColumn = 'modifiedDate'
        SET @OrderByClause = 'P.ModifiedDate'
    ELSE
        SET @OrderByClause = 'P.ModifiedDate'
    
    -- Get total count for pagination
    DECLARE @CountSQL NVARCHAR(MAX) = '
        SELECT @TotalRecords = COUNT(*)
        FROM Product P
        LEFT JOIN Brand B ON P.BrandId = B.Id
        LEFT JOIN Category C ON P.CategoryId = C.Id
        WHERE P.IsDelete = 0 ' + @WhereCondition
    
    EXEC sp_executesql @CountSQL, 
        N'@SearchText NVARCHAR(255), @TotalRecords INT OUTPUT', 
        @SearchText = @SearchText, 
        @TotalRecords = @TotalRecords OUTPUT
    
    -- Main query with pagination - returns columns matching ProductList model
    DECLARE @MainSQL NVARCHAR(MAX) = '
        SELECT 
            P.Id,		
            P.Code,
            ISNULL(P.ProductName,'''') AS ProductName,
            ISNULL(C.Name, '''') AS CategoryName,
            ISNULL(B.Name, '''') AS BrandName,
            P.OuterEAN AS OuterEan,
			P.ModifiedDate AS ModifiedDate,
            CASE WHEN P.IsActive = 1 THEN ''Active'' ELSE ''InActive'' END AS Status,
            @TotalRecords AS FilterTotalCount
        FROM Product P
        LEFT JOIN Brand B ON P.BrandId = B.Id
        LEFT JOIN Category C ON P.CategoryId = C.Id
        WHERE P.IsDelete = 0 ' + @WhereCondition + '
        ORDER BY CASE WHEN ' + @OrderByClause + ' IS NULL THEN 1 ELSE 0 END ASC, ' + @OrderByClause + ' ' + ISNULL(@sortOrder, 'DESC') + '
        OFFSET @OffsetValue ROWS
        FETCH NEXT @PagingSize ROWS ONLY'
    
    EXEC sp_executesql @MainSQL,
        N'@SearchText NVARCHAR(255), @OffsetValue INT, @PagingSize INT, @TotalRecords INT',
        @SearchText = @SearchText,
        @OffsetValue = @OffsetValue,
        @PagingSize = @PagingSize,
        @TotalRecords = @TotalRecords
        
END

GO
/****** Object:  StoredProcedure [dbo].[SPUI_GetPromotionCostDetails]    Script Date: 18-09-2025 10:04:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SPUI_GetPromotionCostDetails]  --1,  5, 'asc', 'PromotionCostID', null
 @OffsetValue int,
@PagingSize int,
@sortOrder varchar(10),
@sortColumn varchar(100),

@SearchText varchar(250)
AS
BEGIN
	DECLARE @FilterTotalCount INT=0
	SET @FilterTotalCount =(select COUNT(*) from (
	--select  A.ProductID,A.PromotionCost,A.PromotionCostID,A.SupplierID,C.Name[SupplierName],B.ProductName,A.StartDate,A.EndDate from PromotionCost[A] inner join Product[B] On A.ProductID=B.Id
	select  A.ProductID,A.PromotionCost,A.PromotionCostID,A.SupplierID,C.Name[SupplierName],B.ProductName,A.StartDate,A.EndDate from PromotionCost[A] inner join Product[B] On A.ProductID=B.Id
	
	inner join Supplier[C] On A.SupplierID=C.Id
	where A.IsActive=1 and A.IsDelete = 0
	group by A.ProductID,A.PromotionCost,A.PromotionCostID,A.SupplierID,C.Name,B.ProductName ,A.StartDate,A.EndDate
	) M ) 


	--select distinct A.ProductID,A.PromotionCost,A.PromotionCostID,A.SupplierID,C.Name[SupplierName],B.ProductName,A.StartDate,A.EndDate ,
	select distinct C.Name[SupplierName],B.ProductName,A.PromotionCost,A.StartDate,A.EndDate,
	CASE WHEN A.IsActive=1 THEN 'Active' ELSE 'InActive' end Status, A.PromotionCostID
	,@FilterTotalCount AS FilterTotalCount 
	from PromotionCost[A] 
	inner join Product[B] On A.ProductID=B.Id
	inner join Supplier[C] On A.SupplierID=C.Id
	where A.IsActive=1 and A.IsDelete = 0
	 
	order by A.EndDate desc
	OFFSET ((@OffsetValue - 1) * @PagingSize) ROWS
	FETCH NEXT @PagingSize ROWS ONLY;

 
END


GO
/****** Object:  StoredProcedure [dbo].[USP_GetBrands]    Script Date: 18-09-2025 10:04:06 ******/
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
/****** Object:  StoredProcedure [dbo].[USP_GetCategories]    Script Date: 18-09-2025 10:04:06 ******/
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
/****** Object:  StoredProcedure [dbo].[USP_GetSubCategories]    Script Date: 18-09-2025 10:04:06 ******/
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
/****** Object:  StoredProcedure [dbo].[usp_INSERTPROMOTIONCOST]    Script Date: 18-09-2025 10:04:06 ******/
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
	INSERT INTO PromotionCost(ProductID,StartDate,EndDate,SelloutStartDate,SelloutEndDate,BonusDescription,SellOutDescription,Remark,PromotionCost,SupplierID,IsActive,CreatedDate,CreatedBy)
	SELECT b.Id,A.[StartDate],A.[EndDate],A.[SelloutStartDate],A.[SelloutEndDate],A.[BonusDescription],A.[SellOutDescription],'', A.PromotionCost, C.Id,1,GETDATE(),@CREATEDBY
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
