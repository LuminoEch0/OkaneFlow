-- =====================================================
-- Create UserPreference Table for OkaneFlow
-- Run this script in SQL Server Management Studio
-- =====================================================

-- Create the UserPreference table
CREATE TABLE [dbo].[UserPreference]
(
    [PreferenceID] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [UserID] UNIQUEIDENTIFIER NOT NULL,
    [DarkMode] BIT NOT NULL DEFAULT 0,
    [EmailNotifications] BIT NOT NULL DEFAULT 1,
    [Currency] NVARCHAR(10) NOT NULL DEFAULT 'EUR',
    [DateFormat] NVARCHAR(20) NOT NULL DEFAULT 'dd/MM/yyyy',
    
    CONSTRAINT [PK_UserPreference] PRIMARY KEY CLUSTERED ([PreferenceID] ASC)
);
GO

-- Add foreign key constraint separately
ALTER TABLE [dbo].[UserPreference]
ADD CONSTRAINT [FK_UserPreference_User] 
    FOREIGN KEY ([UserID]) REFERENCES [dbo].[User] ([UserID]) 
    ON DELETE CASCADE;
GO

-- =====================================================
-- Add LastLoginDate column to User table if missing
-- =====================================================

IF COL_LENGTH('[dbo].[User]', 'LastLoginDate') IS NULL
BEGIN
    ALTER TABLE [dbo].[User] ADD [LastLoginDate] DATETIME2 NULL;
    PRINT 'Added LastLoginDate column to User table';
END
ELSE
BEGIN
    PRINT 'LastLoginDate column already exists';
END
GO

PRINT 'Script completed successfully';
GO
