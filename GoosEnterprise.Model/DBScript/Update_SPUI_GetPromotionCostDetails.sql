-- =============================================
-- Script to Update SPUI_GetPromotionCostDetails
-- Purpose: Modify StartDate and EndDate to return only date format (no time)
-- Date: 2025-09-18
-- =============================================

-- Drop the existing procedure
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SPUI_GetPromotionCostDetails]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SPUI_GetPromotionCostDetails]
GO

-- Create the updated procedure
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

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
	select distinct C.Name[SupplierName],B.ProductName,A.PromotionCost,CAST(A.StartDate AS DATE) AS StartDate,CAST(A.EndDate AS DATE) AS EndDate,
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
	 
	order by A.EndDate desc
	OFFSET @OffsetValue ROWS
	FETCH NEXT @PagingSize ROWS ONLY;

 
END
GO

-- Verify the procedure was created successfully
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SPUI_GetPromotionCostDetails]') AND type in (N'P', N'PC'))
    PRINT 'SUCCESS: SPUI_GetPromotionCostDetails procedure updated successfully!'
ELSE
    PRINT 'ERROR: Failed to update SPUI_GetPromotionCostDetails procedure!'
GO
