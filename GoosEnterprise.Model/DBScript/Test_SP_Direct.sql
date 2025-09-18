-- =============================================
-- Direct Test of usp_INSERTPROMOTIONCOST
-- Author: System Optimization
-- Create date: 2025-09-19
-- Description: Test the stored procedure directly with hardcoded data
-- =============================================

USE [GoodsEnterprise]
GO

PRINT '=== DIRECT STORED PROCEDURE TEST ==='
PRINT ''

-- Create test data that matches your runtime values
DECLARE @TestUDT dbo.UDTType_PromotionCost;

-- Insert test data using the exact values from your runtime screenshot
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
VALUES (
    'asdsadsadsdsd',           -- Product from your runtime
    '7622210680297',           -- OuterBarcode from your runtime
    7.88,                      -- PromotionCost from your runtime
    '2025-12-12',              -- StartDate (converted from 12-12-2025)
    '2025-12-12',              -- EndDate (converted from 12-12-2025)
    '2025-12-12',              -- SelloutStartDate
    '2025-12-12',              -- SelloutEndDate
    'Bonus off Invoice 1701',  -- BonusDescription from your runtime
    'test',                    -- SellOutDescription from your runtime
    'testsup11233'             -- Supplier from your runtime
);

PRINT '1. Test data created:'
SELECT * FROM @TestUDT;

PRINT ''
PRINT '2. Checking if Product exists with this OuterBarcode:'
SELECT Id, ProductName, OuterEan 
FROM Product 
WHERE OuterEan = '7622210680297';

PRINT ''
PRINT '3. Checking if Supplier exists with this name:'
SELECT Id, Name 
FROM Supplier 
WHERE Name = 'testsup11233';

PRINT ''
PRINT '4. Testing the join that the SP will perform:'
SELECT 
    A.Product,
    A.OuterBarcode,
    A.PromotionCost,
    A.Supplier,
    B.Id as ProductID,
    C.Id as SupplierID,
    CASE 
        WHEN B.Id IS NULL THEN 'PRODUCT NOT FOUND - OuterBarcode does not match any Product.OuterEan'
        WHEN C.Id IS NULL THEN 'SUPPLIER NOT FOUND - Supplier name does not match any Supplier.Name'
        ELSE 'READY TO INSERT'
    END as Status
FROM @TestUDT A 
LEFT JOIN Product B ON A.OuterBarcode = B.OuterEan
LEFT JOIN Supplier C ON A.Supplier = C.Name;

PRINT ''
PRINT '5. Executing the stored procedure:'
BEGIN TRY
    EXEC usp_INSERTPROMOTIONCOST @TestUDT, 1;
    PRINT 'Stored procedure completed successfully!';
END TRY
BEGIN CATCH
    PRINT 'ERROR in stored procedure:';
    PRINT 'Error Number: ' + CAST(ERROR_NUMBER() AS VARCHAR(10));
    PRINT 'Error Message: ' + ERROR_MESSAGE();
    PRINT 'Error Line: ' + CAST(ERROR_LINE() AS VARCHAR(10));
END CATCH

PRINT ''
PRINT '6. Checking if any records were inserted:'
SELECT COUNT(*) as NewRecordsCount
FROM PromotionCost 
WHERE CreatedDate >= DATEADD(minute, -5, GETDATE());

IF EXISTS (SELECT 1 FROM PromotionCost WHERE CreatedDate >= DATEADD(minute, -5, GETDATE()))
BEGIN
    PRINT 'Recent PromotionCost records:'
    SELECT TOP 3 
        PromotionCostId,
        ProductID,
        SupplierID,
        PromotionCost,
        StartDate,
        EndDate,
        CreatedDate
    FROM PromotionCost 
    WHERE CreatedDate >= DATEADD(minute, -5, GETDATE())
    ORDER BY CreatedDate DESC;
END
ELSE
BEGIN
    PRINT 'No records were inserted in the last 5 minutes.';
END

PRINT ''
PRINT '=== TROUBLESHOOTING RECOMMENDATIONS ==='
PRINT 'If no records were inserted, check:'
PRINT '1. Does Product table have a record with OuterEan = ''7622210680297''?'
PRINT '2. Does Supplier table have a record with Name = ''testsup11233''?'
PRINT '3. Are there any constraints or triggers on PromotionCost table preventing inserts?'
PRINT '4. Check the stored procedure output messages above for clues'
