-- Update SPUI_GetPromotionCostDetails stored procedure to support dynamic sorting
-- Run this script to update the stored procedure in your database

USE [GoodsEnterprise]
GO

-- Drop existing procedure if it exists
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SPUI_GetPromotionCostDetails]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SPUI_GetPromotionCostDetails]
GO

-- Create updated procedure with dynamic sorting
CREATE PROCEDURE [dbo].[SPUI_GetPromotionCostDetails]  --1,  5, 'asc', 'PromotionCostID', null
 @OffsetValue int,
@PagingSize int,
@sortOrder varchar(10),
@sortColumn varchar(100),

@SearchText varchar(250),
@SearchBy varchar(50) = 'All'
AS
BEGIN
	DECLARE @FilterTotalCount INT=0
	SET @FilterTotalCount =(select COUNT(*) from (
	--select  A.ProductID,A.PromotionCost,A.PromotionCostID,A.SupplierID,C.Name[SupplierName],B.ProductName,A.StartDate,A.EndDate from PromotionCost[A] inner join Product[B] On A.ProductID=B.Id
	select  A.ProductID,A.PromotionCost,A.PromotionCostID,A.SupplierID,C.Name[SupplierName],B.ProductName,A.StartDate,A.EndDate from PromotionCost[A] inner join Product[B] On A.ProductID=B.Id
	
	inner join Supplier[C] On A.SupplierID=C.Id
	where A.IsActive=1 and A.IsDelete = 0
	AND (
		@SearchText IS NULL OR @SearchText = '' OR
		(@SearchBy = 'All' AND (B.ProductName LIKE '%' + @SearchText + '%' OR C.Name LIKE '%' + @SearchText + '%')) OR
		(@SearchBy = 'ProductName' AND B.ProductName LIKE '%' + @SearchText + '%') OR
		(@SearchBy = 'SupplierName' AND C.Name LIKE '%' + @SearchText + '%')
	)
	group by A.ProductID,A.PromotionCost,A.PromotionCostID,A.SupplierID,C.Name,B.ProductName ,A.StartDate,A.EndDate
	) M ) 


	--select distinct A.ProductID,A.PromotionCost,A.PromotionCostID,A.SupplierID,C.Name[SupplierName],B.ProductName,A.StartDate,A.EndDate ,
	select C.Name[SupplierName],B.ProductName,A.PromotionCost,CAST(A.StartDate AS DATE) AS StartDate,CAST(A.EndDate AS DATE) AS EndDate,
	CASE WHEN A.IsActive=1 THEN 'Active' ELSE 'InActive' end Status, A.PromotionCostID
	,@FilterTotalCount AS FilterTotalCount 
	from PromotionCost[A] 
	inner join Product[B] On A.ProductID=B.Id
	inner join Supplier[C] On A.SupplierID=C.Id
	where A.IsActive=1 and A.IsDelete = 0
	AND (
		@SearchText IS NULL OR @SearchText = '' OR
		(@SearchBy = 'All' AND (B.ProductName LIKE '%' + @SearchText + '%' OR C.Name LIKE '%' + @SearchText + '%')) OR
		(@SearchBy = 'ProductName' AND B.ProductName LIKE '%' + @SearchText + '%') OR
		(@SearchBy = 'SupplierName' AND C.Name LIKE '%' + @SearchText + '%')
	)
	 
	order by 
		CASE 
			WHEN @sortColumn = 'SupplierName' AND @sortOrder = 'asc' THEN C.Name
		END ASC,
		CASE 
			WHEN @sortColumn = 'SupplierName' AND @sortOrder = 'desc' THEN C.Name
		END DESC,
		CASE 
			WHEN @sortColumn = 'ProductName' AND @sortOrder = 'asc' THEN B.ProductName
		END ASC,
		CASE 
			WHEN @sortColumn = 'ProductName' AND @sortOrder = 'desc' THEN B.ProductName
		END DESC,
		CASE 
			WHEN @sortColumn = 'PromotionCost' AND @sortOrder = 'asc' THEN A.PromotionCost
		END ASC,
		CASE 
			WHEN @sortColumn = 'PromotionCost' AND @sortOrder = 'desc' THEN A.PromotionCost
		END DESC,
		CASE 
			WHEN @sortColumn = 'StartDate' AND @sortOrder = 'asc' THEN A.StartDate
		END ASC,
		CASE 
			WHEN @sortColumn = 'StartDate' AND @sortOrder = 'desc' THEN A.StartDate
		END DESC,
		CASE 
			WHEN @sortColumn = 'EndDate' AND @sortOrder = 'asc' THEN A.EndDate
		END ASC,
		CASE 
			WHEN @sortColumn = 'EndDate' AND @sortOrder = 'desc' THEN A.EndDate
		END DESC,
		CASE 
			WHEN @sortColumn NOT IN ('SupplierName', 'ProductName', 'PromotionCost', 'StartDate', 'EndDate') AND @sortOrder = 'asc' THEN COALESCE(A.ModifiedDate, A.CreatedDate)
		END ASC,
		CASE 
			WHEN @sortColumn NOT IN ('SupplierName', 'ProductName', 'PromotionCost', 'StartDate', 'EndDate') AND @sortOrder = 'desc' THEN COALESCE(A.ModifiedDate, A.CreatedDate)
		END DESC
	OFFSET @OffsetValue ROWS
	FETCH NEXT @PagingSize ROWS ONLY;

 
END
GO

-- Test the procedure with different sort columns
-- EXEC SPUI_GetPromotionCostDetails 0, 5, 'asc', 'SupplierName', '', 'All'
-- EXEC SPUI_GetPromotionCostDetails 0, 5, 'desc', 'ProductName', '', 'All'
-- EXEC SPUI_GetPromotionCostDetails 0, 5, 'asc', 'PromotionCost', '', 'All'
-- EXEC SPUI_GetPromotionCostDetails 0, 5, 'desc', 'StartDate', '', 'All'
-- EXEC SPUI_GetPromotionCostDetails 0, 5, 'asc', 'EndDate', '', 'All'
-- EXEC SPUI_GetPromotionCostDetails 0, 5, 'desc', 'DefaultSort', '', 'All' -- Should use ModifiedDate/CreatedDate default

PRINT 'SPUI_GetPromotionCostDetails stored procedure updated successfully!'
PRINT 'Supported sort columns: SupplierName, ProductName, PromotionCost, StartDate, EndDate'
PRINT 'Default sort: ModifiedDate (if not null), otherwise CreatedDate'
