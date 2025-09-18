-- =============================================
-- Check Available Products and Suppliers
-- Author: System Optimization
-- Create date: 2025-09-19
-- Description: Check what products and suppliers are available for import validation
-- =============================================

USE [GoodsEnterprise]
GO

PRINT '=== CHECKING AVAILABLE PRODUCTS AND SUPPLIERS ==='
PRINT ''

-- 1. Check Product table
PRINT '1. Product Table Summary:'
SELECT 
    COUNT(*) as TotalProducts,
    COUNT(CASE WHEN OuterEan IS NOT NULL AND OuterEan != '' THEN 1 END) as ProductsWithBarcode,
    COUNT(CASE WHEN OuterEan IS NULL OR OuterEan = '' THEN 1 END) as ProductsWithoutBarcode
FROM Product 
WHERE IsDelete != 1 OR IsDelete IS NULL;

PRINT ''
PRINT '2. Sample Products with Barcodes (first 10):'
SELECT TOP 10 
    Id,
    ProductName,
    OuterEan,
    CASE 
        WHEN OuterEan IS NULL OR OuterEan = '' THEN 'NO BARCODE'
        ELSE 'HAS BARCODE'
    END as BarcodeStatus
FROM Product 
WHERE (IsDelete != 1 OR IsDelete IS NULL)
  AND OuterEan IS NOT NULL 
  AND OuterEan != ''
ORDER BY ProductName;

PRINT ''
PRINT '3. Check for your specific barcode from Excel:'
DECLARE @TestBarcode VARCHAR(50) = '7622210680297';
SELECT 
    Id,
    ProductName,
    OuterEan,
    'FOUND' as Status
FROM Product 
WHERE OuterEan = @TestBarcode
  AND (IsDelete != 1 OR IsDelete IS NULL);

IF @@ROWCOUNT = 0
BEGIN
    PRINT 'Product with barcode ' + @TestBarcode + ' NOT FOUND';
    PRINT 'Similar barcodes:';
    SELECT TOP 5 
        Id,
        ProductName,
        OuterEan
    FROM Product 
    WHERE OuterEan LIKE '%' + RIGHT(@TestBarcode, 5) + '%'
      AND (IsDelete != 1 OR IsDelete IS NULL);
END

PRINT ''
PRINT '4. Supplier Table Summary:'
SELECT 
    COUNT(*) as TotalSuppliers,
    COUNT(CASE WHEN Name IS NOT NULL AND Name != '' THEN 1 END) as SuppliersWithName,
    COUNT(CASE WHEN Name IS NULL OR Name = '' THEN 1 END) as SuppliersWithoutName
FROM Supplier 
WHERE IsDelete != 1 OR IsDelete IS NULL;

PRINT ''
PRINT '5. Sample Suppliers (first 10):'
SELECT TOP 10 
    Id,
    Name,
    CASE 
        WHEN Name IS NULL OR Name = '' THEN 'NO NAME'
        ELSE 'HAS NAME'
    END as NameStatus
FROM Supplier 
WHERE (IsDelete != 1 OR IsDelete IS NULL)
  AND Name IS NOT NULL 
  AND Name != ''
ORDER BY Name;

PRINT ''
PRINT '6. Check for your specific supplier from Excel:'
DECLARE @TestSupplier VARCHAR(100) = 'testsup11233';
SELECT 
    Id,
    Name,
    'FOUND' as Status
FROM Supplier 
WHERE Name = @TestSupplier
  AND (IsDelete != 1 OR IsDelete IS NULL);

IF @@ROWCOUNT = 0
BEGIN
    PRINT 'Supplier with name "' + @TestSupplier + '" NOT FOUND';
    PRINT 'Similar suppliers:';
    SELECT TOP 5 
        Id,
        Name
    FROM Supplier 
    WHERE Name LIKE '%' + @TestSupplier + '%'
      AND (IsDelete != 1 OR IsDelete IS NULL);
      
    PRINT 'All available suppliers:';
    SELECT 
        Id,
        Name
    FROM Supplier 
    WHERE (IsDelete != 1 OR IsDelete IS NULL)
      AND Name IS NOT NULL 
      AND Name != ''
    ORDER BY Name;
END

PRINT ''
PRINT '=== VALIDATION RECOMMENDATIONS ==='
PRINT '1. Ensure your Excel "Outer Barcode" values match Product.OuterEan exactly'
PRINT '2. Ensure your Excel "Supplier:" values match Supplier.Name exactly'
PRINT '3. Check for case sensitivity and extra spaces'
PRINT '4. Verify that products and suppliers are not marked as deleted (IsDelete != 1)'

PRINT ''
PRINT '=== SAMPLE EXCEL FORMAT ==='
PRINT 'Your Excel should have these columns with matching data:'
PRINT 'Product | Outer Barcode | Supplier:'
PRINT '--------|---------------|----------'

-- Show sample format with actual data
SELECT TOP 3
    p.ProductName as 'Product_Example',
    p.OuterEan as 'Outer_Barcode_Example', 
    s.Name as 'Supplier_Example'
FROM Product p
CROSS JOIN Supplier s
WHERE p.OuterEan IS NOT NULL 
  AND p.OuterEan != ''
  AND s.Name IS NOT NULL 
  AND s.Name != ''
  AND (p.IsDelete != 1 OR p.IsDelete IS NULL)
  AND (s.IsDelete != 1 OR s.IsDelete IS NULL);
