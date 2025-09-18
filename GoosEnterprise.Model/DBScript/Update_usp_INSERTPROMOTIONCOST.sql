-- =============================================
-- Script: Update usp_INSERTPROMOTIONCOST stored procedure
-- Author: System Optimization
-- Create date: 2025-09-18
-- Description: Fix issues in promotion cost insertion procedure
-- =============================================

USE [GoodsEnterprise]
GO

-- Drop existing procedure if it exists
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_INSERTPROMOTIONCOST]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_INSERTPROMOTIONCOST]
GO

-- Create updated procedure
CREATE PROCEDURE [dbo].[usp_INSERTPROMOTIONCOST]
	@UDTType_PromotionCost as dbo.UDTType_PromotionCost READONLY,
	@CREATEDBY INT
AS
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @InsertedCount INT = 0;
	DECLARE @ErrorCount INT = 0;
	
	BEGIN TRY
		-- Log input data for debugging
		DECLARE @InputCount INT = (SELECT COUNT(*) FROM @UDTType_PromotionCost);
		PRINT 'Input UDT contains ' + CAST(@InputCount AS VARCHAR(10)) + ' rows';
		
		-- Check for matching products and suppliers
		DECLARE @ProductMatches INT = (
			SELECT COUNT(*) 
			FROM @UDTType_PromotionCost A 
			INNER JOIN Product B ON A.OuterBarcode = B.OuterEan
		);
		
		DECLARE @SupplierMatches INT = (
			SELECT COUNT(*) 
			FROM @UDTType_PromotionCost A 
			INNER JOIN Supplier C ON A.Supplier = C.Name
		);
		
		PRINT 'Product matches: ' + CAST(@ProductMatches AS VARCHAR(10));
		PRINT 'Supplier matches: ' + CAST(@SupplierMatches AS VARCHAR(10));
		
		-- Show sample data from UDT
		SELECT TOP 3 
			Product, OuterBarcode, PromotionCost, StartDate, EndDate, Supplier
		FROM @UDTType_PromotionCost;
		
		-- Insert promotion costs with proper joins and error handling
		INSERT INTO PromotionCost(
			ProductID, StartDate, EndDate, SelloutStartDate, SelloutEndDate, 
			BonusDescription, SellOutDescription, Remark, PromotionCost, 
			SupplierID, IsActive, CreatedDate, CreatedBy
		)
		SELECT 
			B.Id as ProductID,
			A.StartDate,
			A.EndDate,
			A.SelloutStartDate,
			A.SelloutEndDate,
			A.BonusDescription,
			A.SellOutDescription,
			ISNULL(A.Product, '') as Remark,
			A.PromotionCost,
			C.Id as SupplierID,
			1 as IsActive,
			GETDATE() as CreatedDate,
			@CREATEDBY as CreatedBy
		FROM @UDTType_PromotionCost A 
		INNER JOIN Product B ON A.OuterBarcode = B.OuterEan
		INNER JOIN Supplier C ON A.Supplier = C.Name
		WHERE A.PromotionCost IS NOT NULL 
		  AND A.StartDate IS NOT NULL 
		  AND A.EndDate IS NOT NULL
		  AND B.Id IS NOT NULL 
		  AND C.Id IS NOT NULL;
		
		SET @InsertedCount = @@ROWCOUNT;
		
		-- Log successful insertion
		PRINT 'Successfully inserted ' + CAST(@InsertedCount AS VARCHAR(10)) + ' promotion cost records.';
		
		-- Return success status
		SELECT @InsertedCount as InsertedRecords, 0 as ErrorCount, 'Success' as Status;
		
	END TRY
	BEGIN CATCH
		SET @ErrorCount = 1;
		
		-- Log error details
		DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
		DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
		DECLARE @ErrorState INT = ERROR_STATE();
		
		PRINT 'Error in usp_INSERTPROMOTIONCOST: ' + @ErrorMessage;
		
		-- Return error status
		SELECT 0 as InsertedRecords, @ErrorCount as ErrorCount, @ErrorMessage as Status;
		
		-- Re-throw the error for proper handling
		RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
	END CATCH
END
GO

-- Test the procedure with sample data (optional)
/*
DECLARE @TestData dbo.UDTType_PromotionCost;

-- Insert test data (replace with actual values from your database)
INSERT INTO @TestData (Product, OuterBarcode, PromotionCost, StartDate, EndDate, SelloutStartDate, SelloutEndDate, BonusDescription, SellOutDescription, Supplier)
VALUES ('Test Product', '1234567890123', 10.50, '2025-01-01', '2025-01-31', '2025-01-01', '2025-01-31', 'Test Bonus', 'Test Sellout', 'Test Supplier');

-- Execute the procedure
EXEC usp_INSERTPROMOTIONCOST @TestData, 1;
*/

PRINT 'Stored procedure usp_INSERTPROMOTIONCOST updated successfully!'
