-- Script to create SPUI_GetProductDetails stored procedure aligned with Product.cshtml

USE [GoodsEnterprise]
GO

-- Drop existing stored procedure
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'SPUI_GetProductDetails')
    DROP PROCEDURE [dbo].[SPUI_GetProductDetails]
GO

-- Create stored procedure that matches ProductList model exactly
CREATE PROCEDURE [dbo].[SPUI_GetProductDetails]
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

PRINT 'SPUI_GetProductDetails stored procedure created successfully!'
PRINT 'Columns returned: Id, Code, ProductName, CategoryName, BrandName, OuterEan, Status, FilterTotalCount'
