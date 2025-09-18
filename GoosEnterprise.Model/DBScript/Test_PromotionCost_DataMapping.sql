-- =============================================
-- Test Script for PromotionCost Data Mapping
-- Author: System Optimization
-- Create date: 2025-09-18
-- Description: Test the data mapping and stored procedure with sample data
-- =============================================

USE [GoodsEnterprise]
GO

PRINT '=== TESTING PROMOTION COST DATA MAPPING ==='
PRINT ''

-- 1. First, let's check what data we have in Product and Supplier tables
PRINT '1. Checking Product table sample data...'
SELECT TOP 5 
    Id, 
    ProductName, 
    OuterEan,
    CASE 
        WHEN OuterEan IS NULL OR OuterEan = '' THEN 'MISSING'
        ELSE 'OK'
    END as OuterEanStatus
FROM Product 
ORDER BY Id;

PRINT ''
PRINT '2. Checking Supplier table sample data...'
SELECT TOP 5 
    Id, 
    Name,
    CASE 
        WHEN Name IS NULL OR Name = '' THEN 'MISSING'
        ELSE 'OK'
    END as NameStatus
FROM Supplier 
ORDER BY Id;

PRINT ''

-- 2. Create test data that matches your Excel structure
PRINT '3. Creating test UDT data based on your Excel structure...'
DECLARE @TestUDT dbo.UDTType_PromotionCost;

-- Insert test data using actual values from your tables
INSERT INTO @TestUDT (
    Product, 
    OuterBarcode, 
    PromotionCost, 
    StartDate, 
    EndDate, 
    SelloutStartDate, 
    SelloutEndDate, 
    BonusDescription, 
    SellOutDescription, 
    Supplier
)
SELECT TOP 1
    p.ProductName as Product,
    p.OuterEan as OuterBarcode,
    7.88 as PromotionCost,  -- From your Excel example
    '2025-01-01' as StartDate,
    '2025-01-31' as EndDate,
    '2025-01-01' as SelloutStartDate,
    '2025-01-31' as SelloutEndDate,
    'Bonus off Invoice 1701' as BonusDescription,  -- From your Excel
    'test' as SellOutDescription,  -- From your Excel
    s.Name as Supplier
FROM Product p
CROSS JOIN Supplier s
WHERE p.OuterEan IS NOT NULL 
  AND p.OuterEan != ''
  AND s.Name IS NOT NULL
  AND s.Name != ''
  AND s.Name = 'testsup11233';  -- From your Excel example

-- Check if we have test data
DECLARE @TestCount INT = (SELECT COUNT(*) FROM @TestUDT);
PRINT 'Test UDT contains ' + CAST(@TestCount AS VARCHAR(10)) + ' rows';

IF @TestCount > 0
BEGIN
    PRINT ''
    PRINT '4. Test UDT data:'
    SELECT * FROM @TestUDT;
    
    PRINT ''
    PRINT '5. Testing joins (this is what the SP will do):'
    SELECT 
        A.Product,
        A.OuterBarcode,
        A.PromotionCost,
        A.StartDate,
        A.EndDate,
        A.Supplier,
        B.Id as ProductID,
        B.ProductName,
        C.Id as SupplierID,
        C.Name as SupplierName,
        CASE 
            WHEN B.Id IS NULL THEN 'PRODUCT NOT FOUND'
            WHEN C.Id IS NULL THEN 'SUPPLIER NOT FOUND'
            ELSE 'READY TO INSERT'
        END as Status
    FROM @TestUDT A 
    LEFT JOIN Product B ON A.OuterBarcode = B.OuterEan
    LEFT JOIN Supplier C ON A.Supplier = C.Name;
    
    PRINT ''
    PRINT '6. Testing the stored procedure...'
    BEGIN TRY
        EXEC usp_INSERTPROMOTIONCOST @TestUDT, 1;
        PRINT 'Stored procedure executed successfully!';
    END TRY
    BEGIN CATCH
        PRINT 'ERROR in stored procedure: ' + ERROR_MESSAGE();
    END CATCH
END
ELSE
BEGIN
    PRINT 'ERROR: Could not create test data. Issues:'
    PRINT '- Check if Product table has records with valid OuterEan values'
    PRINT '- Check if Supplier table has records with valid Name values'
    PRINT '- Verify the supplier name "testsup11233" exists in your Supplier table'
    
    PRINT ''
    PRINT 'Available suppliers:'
    SELECT TOP 10 Id, Name FROM Supplier WHERE Name IS NOT NULL AND Name != '';
    
    PRINT ''
    PRINT 'Available products with OuterEan:'
    SELECT TOP 10 Id, ProductName, OuterEan FROM Product WHERE OuterEan IS NOT NULL AND OuterEan != '';
END

PRINT ''
PRINT '=== RECOMMENDATIONS FOR YOUR EXCEL FILE ==='
PRINT '1. Ensure OuterBarcode values in Excel match OuterEan values in Product table'
PRINT '2. Ensure Supplier values in Excel match Name values in Supplier table'
PRINT '3. Check that all required fields have values (Product, OuterBarcode, PromotionCost, StartDate, EndDate, Supplier)'
PRINT '4. Verify date formats are readable (DD-MM-YYYY or MM/DD/YYYY)'
PRINT '5. Ensure PromotionCost values are numeric'
