
USE [GoodsEnterprise]
GO
/****** Object:  Table [dbo].[Admin]    Script Date: 29-08-2021 18:23:25 ******/
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
/****** Object:  Table [dbo].[Brand]    Script Date: 29-08-2021 18:23:25 ******/
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
/****** Object:  Table [dbo].[Category]    Script Date: 29-08-2021 18:23:25 ******/
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
	[BrandId] [int] NOT NULL,
 CONSTRAINT [PK_Category] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Customer]    Script Date: 29-08-2021 18:23:25 ******/
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
/****** Object:  Table [dbo].[CustomerBasket]    Script Date: 29-08-2021 18:23:25 ******/
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
/****** Object:  Table [dbo].[CustomerFavourite]    Script Date: 29-08-2021 18:23:25 ******/
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
/****** Object:  Table [dbo].[Order]    Script Date: 29-08-2021 18:23:25 ******/
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
/****** Object:  Table [dbo].[OrderDetail]    Script Date: 29-08-2021 18:23:25 ******/
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
/****** Object:  Table [dbo].[Product]    Script Date: 29-08-2021 18:23:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Product](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](25) NOT NULL,
	[BrandId] [int] NOT NULL,
	[CategoryId] [int] NOT NULL,
	[SubCategoryId] [int] NOT NULL,
	[InnerEAN] [varchar](25) NULL,
	[OuterEAN] [varchar](25) NULL,
	[PackSize] [varchar](25) NULL,
	[UPC] [int] NULL,
	[LayerQuantity] [int] NULL,
	[PalletQuantity] [int] NULL,
	[Height] [decimal](16, 2) NULL,
	[Weight] [decimal](16, 2) NULL,
	[Width] [decimal](16, 2) NULL,
	[NetWeight] [decimal](16, 2) NULL,
	[Depth] [decimal](16, 2) NULL,
	[Image] [varchar](200) NULL,
	[CreatedDate] [datetime] NULL,
	[Createdby] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	[Modifiedby] [int] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDelete] [bit] NOT NULL,
 CONSTRAINT [PK_Product] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Role]    Script Date: 29-08-2021 18:23:25 ******/
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
/****** Object:  Table [dbo].[Status]    Script Date: 29-08-2021 18:23:25 ******/
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
/****** Object:  Table [dbo].[SubCategory]    Script Date: 29-08-2021 18:23:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SubCategory](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CategoryId] [int] NOT NULL,
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
/****** Object:  Table [dbo].[Supplier]    Script Date: 29-08-2021 18:23:25 ******/
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
/****** Object:  Table [dbo].[Tax]    Script Date: 29-08-2021 18:23:25 ******/
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
ALTER TABLE [dbo].[Order] ADD  CONSTRAINT [DF_Order_IsActive]  DEFAULT ((0)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Order] ADD  CONSTRAINT [DF_Order_IsDelete]  DEFAULT ((0)) FOR [IsDelete]
GO
ALTER TABLE [dbo].[Product] ADD  CONSTRAINT [Default_IsActive]  DEFAULT ((0)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Product] ADD  CONSTRAINT [Default_IsDelete]  DEFAULT ((0)) FOR [IsDelete]
GO
ALTER TABLE [dbo].[Status] ADD  CONSTRAINT [DF_Status_IsActive]  DEFAULT ((0)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Status] ADD  CONSTRAINT [DF_Status_IsDelete]  DEFAULT ((0)) FOR [IsDelete]
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
ALTER TABLE [dbo].[Category]  WITH CHECK ADD  CONSTRAINT [FK_Brand_ID] FOREIGN KEY([BrandId])
REFERENCES [dbo].[Brand] ([Id])
GO
ALTER TABLE [dbo].[Category] CHECK CONSTRAINT [FK_Brand_ID]
GO
ALTER TABLE [dbo].[Customer]  WITH CHECK ADD FOREIGN KEY([RoleId])
REFERENCES [dbo].[Role] ([Id])
GO
ALTER TABLE [dbo].[CustomerBasket]  WITH CHECK ADD FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customer] ([Id])
GO
ALTER TABLE [dbo].[CustomerBasket]  WITH CHECK ADD  CONSTRAINT [FK__CustomerB__Produ__52593CB8] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([Id])
GO
ALTER TABLE [dbo].[CustomerBasket] CHECK CONSTRAINT [FK__CustomerB__Produ__52593CB8]
GO
ALTER TABLE [dbo].[CustomerFavourite]  WITH CHECK ADD FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customer] ([Id])
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
ALTER TABLE [dbo].[Product]  WITH CHECK ADD  CONSTRAINT [FK__Product__BrandId__403A8C7D] FOREIGN KEY([BrandId])
REFERENCES [dbo].[Brand] ([Id])
GO
ALTER TABLE [dbo].[Product] CHECK CONSTRAINT [FK__Product__BrandId__403A8C7D]
GO
ALTER TABLE [dbo].[Product]  WITH CHECK ADD  CONSTRAINT [FK__Product__Categor__412EB0B6] FOREIGN KEY([CategoryId])
REFERENCES [dbo].[Category] ([Id])
GO
ALTER TABLE [dbo].[Product] CHECK CONSTRAINT [FK__Product__Categor__412EB0B6]
GO
ALTER TABLE [dbo].[Product]  WITH CHECK ADD  CONSTRAINT [FK__Product__SubCate__4222D4EF] FOREIGN KEY([SubCategoryId])
REFERENCES [dbo].[SubCategory] ([Id])
GO
ALTER TABLE [dbo].[Product] CHECK CONSTRAINT [FK__Product__SubCate__4222D4EF]
GO
/****** Object:  StoredProcedure [dbo].[USP_GetBrands]    Script Date: 29-08-2021 18:23:25 ******/
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
/****** Object:  StoredProcedure [dbo].[USP_GetCategories]    Script Date: 29-08-2021 18:23:25 ******/
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
/****** Object:  StoredProcedure [dbo].[USP_GetSubCategories]    Script Date: 29-08-2021 18:23:25 ******/
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
USE [master]
GO
ALTER DATABASE [GoodsEnterprise] SET  READ_WRITE 
GO
