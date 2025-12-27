-- Create Subscription Table
CREATE TABLE [dbo].[Subscription](
	[SubscriptionID] [uniqueidentifier] NOT NULL,
	[AccountID] [uniqueidentifier] NOT NULL,
	[CategoryID] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[Frequency] [nvarchar](50) NOT NULL,
	[StartDate] [datetime2](7) NOT NULL,
	[Description] [nvarchar](255) NULL,
 CONSTRAINT [PK_Subscription] PRIMARY KEY CLUSTERED 
(
	[SubscriptionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

-- Create Debt Table
CREATE TABLE [dbo].[Debt](
	[DebtID] [uniqueidentifier] NOT NULL,
	[AccountID] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[InitialAmount] [decimal](18, 2) NOT NULL,
	[RemainingAmount] [decimal](18, 2) NOT NULL,
	[InterestRate] [decimal](5, 2) NOT NULL,
	[DueDate] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_Debt] PRIMARY KEY CLUSTERED 
(
	[DebtID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

-- Add Foreign Key Constraints
-- Fixed: Changed ON DELETE CASCADE to ON DELETE NO ACTION for Category to prevent multiple cascade paths
ALTER TABLE [dbo].[Subscription]  WITH CHECK ADD  CONSTRAINT [FK_Subscription_BankAccount] FOREIGN KEY([AccountID])
REFERENCES [dbo].[BankAccount] ([AccountID])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[Subscription] CHECK CONSTRAINT [FK_Subscription_BankAccount]
GO

-- Note: We use NO ACTION here because BankAccount deletion already cascades to Subscription (via AccountID)
-- and BankAccount->Category->Subscription creates a second cycle if this were CASCADE.
ALTER TABLE [dbo].[Subscription]  WITH CHECK ADD  CONSTRAINT [FK_Subscription_Category] FOREIGN KEY([CategoryID])
REFERENCES [dbo].[Category] ([CategoryID])
ON DELETE NO ACTION
GO

ALTER TABLE [dbo].[Subscription] CHECK CONSTRAINT [FK_Subscription_Category]
GO

ALTER TABLE [dbo].[Debt]  WITH CHECK ADD  CONSTRAINT [FK_Debt_BankAccount] FOREIGN KEY([AccountID])
REFERENCES [dbo].[BankAccount] ([AccountID])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[Debt] CHECK CONSTRAINT [FK_Debt_BankAccount]
GO
