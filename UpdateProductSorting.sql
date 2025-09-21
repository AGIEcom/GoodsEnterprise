-- Update SPUI_GetProductDetails stored procedure to support default ModifiedDate/CreatedDate sorting
-- Run this script to update the stored procedure in your database

USE [GoodsEnterprise]
GO

-- Drop existing procedure if it exists
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SPUI_GetProductDetails]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SPUI_GetProductDetails]
GO

-- Create updated procedure with default ModifiedDate/CreatedDate sorting and removed ModifiedDate column
CREATE PROCEDURE [dbo].[SPUI_GetProductDetails]  --0, 10, 'DESC', 'DefaultSort', '', 'All'
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
    DECLARE @OrderByClause NVARCHAR(50) = 'COALESCE(P.ModifiedDate, P.CreatedDate)'
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
    ELSE -- Default sort: ModifiedDate if not null, otherwise CreatedDate
        SET @OrderByClause = 'COALESCE(P.ModifiedDate, P.CreatedDate)'
    
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
    
    -- Main query with pagination - returns columns matching ProductList model (ModifiedDate column removed)
    DECLARE @MainSQL NVARCHAR(MAX) = '
        SELECT 
            P.Id,		
            P.Code,
            ISNULL(P.ProductName,'''') AS ProductName,
            ISNULL(C.Name, '''') AS CategoryName,
            ISNULL(B.Name, '''') AS BrandName,
            P.OuterEAN AS OuterEan,
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

-- Test the procedure with different sort columns
-- EXEC SPUI_GetProductDetails 0, 5, 'asc', 'code', '', 'All'
-- EXEC SPUI_GetProductDetails 0, 5, 'desc', 'productName', '', 'All'
-- EXEC SPUI_GetProductDetails 0, 5, 'asc', 'categoryName', '', 'All'
-- EXEC SPUI_GetProductDetails 0, 5, 'desc', 'DefaultSort', '', 'All' -- Should use ModifiedDate/CreatedDate default

PRINT 'SPUI_GetProductDetails stored procedure updated successfully!'
PRINT 'ModifiedDate column removed from SELECT list'
PRINT 'Supported sort columns: code, productName, categoryName, brandName, outerEan, status'
PRINT 'Default sort: ModifiedDate (if not null), otherwise CreatedDate DESC'
