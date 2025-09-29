-- Add Remember Me functionality columns to Admin table
-- Run this script to add the necessary columns for Remember Me functionality

USE GoodsEnterprise;
GO

-- Check if RememberMeToken column exists, if not add it
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Admin]') AND name = 'RememberMeToken')
BEGIN
    ALTER TABLE [dbo].[Admin] 
    ADD [RememberMeToken] VARCHAR(500) NULL;
    PRINT 'Added RememberMeToken column to Admin table';
END
ELSE
BEGIN
    PRINT 'RememberMeToken column already exists in Admin table';
END

-- Check if RememberMeExpiry column exists, if not add it
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Admin]') AND name = 'RememberMeExpiry')
BEGIN
    ALTER TABLE [dbo].[Admin] 
    ADD [RememberMeExpiry] DATETIME NULL;
    PRINT 'Added RememberMeExpiry column to Admin table';
END
ELSE
BEGIN
    PRINT 'RememberMeExpiry column already exists in Admin table';
END

-- Create index on RememberMeToken for better performance
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Admin]') AND name = 'IX_Admin_RememberMeToken')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Admin_RememberMeToken] ON [dbo].[Admin] ([RememberMeToken]);
    PRINT 'Created index IX_Admin_RememberMeToken on Admin table';
END
ELSE
BEGIN
    PRINT 'Index IX_Admin_RememberMeToken already exists on Admin table';
END

PRINT 'Remember Me functionality database setup completed successfully!';
GO
