-- =============================================
-- Diagnostic Script for PromotionCost Import Issues
-- Author: System Optimization
-- Create date: 2025-09-18
-- Description: Help diagnose why promotion cost data is not importing
-- =============================================

USE [GoodsEnterprise]
GO

PRINT '=== PROMOTION COST IMPORT DIAGNOSTICS ==='
PRINT ''

-- 1. Check if UDT exists
PRINT '1. Checking UDTType_PromotionCost structure...'
SELECT 
    c.name AS ColumnName,
    t.name AS DataType,
    c.max_length,
    c.precision,
    c.scale,
    c.is_nullable
FROM sys.table_types tt
INNER JOIN sys.columns c ON tt.type_table_object_id = c.object_id
INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
WHERE tt.name = 'UDTType_PromotionCost'
ORDER BY c.column_id;

PRINT ''

-- 2. Check if stored procedure exists
PRINT '2. Checking stored procedure...'
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_INSERTPROMOTIONCOST]') AND type in (N'P', N'PC'))
    PRINT 'Stored procedure usp_INSERTPROMOTIONCOST EXISTS'
ELSE
    PRINT 'ERROR: Stored procedure usp_INSERTPROMOTIONCOST DOES NOT EXIST'

PRINT ''

-- 3. Check PromotionCost table structure
PRINT '3. Checking PromotionCost table structure...'
SELECT 
    c.COLUMN_NAME,
    c.DATA_TYPE,
    c.IS_NULLABLE,
    c.CHARACTER_MAXIMUM_LENGTH,
    c.NUMERIC_PRECISION,
    c.NUMERIC_SCALE
FROM INFORMATION_SCHEMA.COLUMNS c
WHERE c.TABLE_NAME = 'PromotionCost'
ORDER BY c.ORDINAL_POSITION;

PRINT ''

-- 4. Check Product table for OuterEan column
PRINT '4. Checking Product table for OuterEan column...'
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Product' AND COLUMN_NAME = 'OuterEan')
    PRINT 'Product.OuterEan column EXISTS'
ELSE
    PRINT 'ERROR: Product.OuterEan column DOES NOT EXIST'

-- Show sample Product data
PRINT 'Sample Product data (first 5 rows):'
SELECT TOP 5 Id, ProductName, OuterEan FROM Product WHERE OuterEan IS NOT NULL;

PRINT ''

-- 5. Check Supplier table
PRINT '5. Checking Supplier table...'
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Supplier')
    PRINT 'Supplier table EXISTS'
ELSE
    PRINT 'ERROR: Supplier table DOES NOT EXIST'

-- Show sample Supplier data
PRINT 'Sample Supplier data (first 5 rows):'
SELECT TOP 5 Id, Name FROM Supplier;

PRINT ''

-- 6. Check for existing PromotionCost data
PRINT '6. Checking existing PromotionCost data...'
DECLARE @ExistingCount INT = (SELECT COUNT(*) FROM PromotionCost);
PRINT 'Existing PromotionCost records: ' + CAST(@ExistingCount AS VARCHAR(10));

IF @ExistingCount > 0
BEGIN
    PRINT 'Sample PromotionCost data (first 3 rows):'
    SELECT TOP 3 
        PromotionCostId,
        ProductID,
        SupplierID,
        PromotionCost,
        StartDate,
        EndDate,
        CreatedDate
    FROM PromotionCost
    ORDER BY CreatedDate DESC;
END

PRINT ''

-- 7. Test UDT with sample data
PRINT '7. Testing UDT with sample data...'
DECLARE @TestUDT dbo.UDTType_PromotionCost;

-- Try to insert sample data (you may need to adjust these values based on your actual data)
INSERT INTO @TestUDT (Product, OuterBarcode, PromotionCost, StartDate, EndDate, SelloutStartDate, SelloutEndDate, BonusDescription, SellOutDescription, Supplier)
SELECT TOP 1
    p.ProductName,
    p.OuterEan,
    10.50,
    GETDATE(),
    DATEADD(day, 30, GETDATE()),
    GETDATE(),
    DATEADD(day, 30, GETDATE()),
    'Test Bonus',
    'Test Sellout',
    s.Name
FROM Product p
CROSS JOIN Supplier s
WHERE p.OuterEan IS NOT NULL 
  AND p.OuterEan != ''
  AND s.Name IS NOT NULL
  AND s.Name != '';

DECLARE @TestRowCount INT = (SELECT COUNT(*) FROM @TestUDT);
PRINT 'Test UDT populated with ' + CAST(@TestRowCount AS VARCHAR(10)) + ' rows';

IF @TestRowCount > 0
BEGIN
    PRINT 'Test UDT data:'
    SELECT * FROM @TestUDT;
    
    -- Test the join conditions
    PRINT ''
    PRINT 'Testing join conditions:'
    SELECT 
        A.Product,
        A.OuterBarcode,
        A.Supplier,
        B.Id as ProductID,
        B.ProductName,
        C.Id as SupplierID,
        C.Name as SupplierName
    FROM @TestUDT A 
    LEFT JOIN Product B ON A.OuterBarcode = B.OuterEan
    LEFT JOIN Supplier C ON A.Supplier = C.Name;
END
ELSE
BEGIN
    PRINT 'WARNING: Could not create test data. Check if Product and Supplier tables have valid data.';
END

PRINT ''
PRINT '=== END DIAGNOSTICS ==='

-- Recommendations
PRINT ''
PRINT '=== RECOMMENDATIONS ==='
PRINT '1. Run the Update_usp_INSERTPROMOTIONCOST.sql script to fix the stored procedure'
PRINT '2. Ensure your Excel file has the correct column headers:'
PRINT '   - Product, Outer Barcode, W/sale Nett Cost, Start, End, Sellout Start, Sellout End, Bonus Description, Sell Out Description, Supplier:'
PRINT '3. Verify that OuterBarcode values in Excel match OuterEan values in Product table'
PRINT '4. Verify that Supplier values in Excel match Name values in Supplier table'
PRINT '5. Check application logs for detailed error messages during import'
