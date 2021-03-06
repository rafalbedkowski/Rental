USE [master]
GO
/****** Object:  Database [RentalDatabase]    Script Date: 11.08.2020 16:34:21 ******/

GO
ALTER DATABASE [RentalDatabase] SET COMPATIBILITY_LEVEL = 140
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [RentalDatabase].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [RentalDatabase] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [RentalDatabase] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [RentalDatabase] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [RentalDatabase] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [RentalDatabase] SET ARITHABORT OFF 
GO
ALTER DATABASE [RentalDatabase] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [RentalDatabase] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [RentalDatabase] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [RentalDatabase] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [RentalDatabase] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [RentalDatabase] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [RentalDatabase] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [RentalDatabase] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [RentalDatabase] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [RentalDatabase] SET  ENABLE_BROKER 
GO
ALTER DATABASE [RentalDatabase] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [RentalDatabase] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [RentalDatabase] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [RentalDatabase] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [RentalDatabase] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [RentalDatabase] SET READ_COMMITTED_SNAPSHOT ON 
GO
ALTER DATABASE [RentalDatabase] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [RentalDatabase] SET RECOVERY FULL 
GO
ALTER DATABASE [RentalDatabase] SET  MULTI_USER 
GO
ALTER DATABASE [RentalDatabase] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [RentalDatabase] SET DB_CHAINING OFF 
GO
ALTER DATABASE [RentalDatabase] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [RentalDatabase] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [RentalDatabase] SET DELAYED_DURABILITY = DISABLED 
GO
EXEC sys.sp_db_vardecimal_storage_format N'RentalDatabase', N'ON'
GO
ALTER DATABASE [RentalDatabase] SET QUERY_STORE = OFF
GO
USE [RentalDatabase]
GO
/****** Object:  Table [dbo].[Companies]    Script Date: 11.08.2020 16:34:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Companies](
	[CompanyId] [uniqueidentifier] NOT NULL,
	[CompanyName] [nvarchar](50) NOT NULL,
	[PostCode] [nvarchar](max) NULL,
	[City] [nvarchar](max) NULL,
	[Address] [nvarchar](max) NULL,
	[Email] [nvarchar](max) NULL,
	[Phone] [nvarchar](max) NULL,
	[CompanyType] [int] NOT NULL,
 CONSTRAINT [PK_dbo.Companies] PRIMARY KEY CLUSTERED 
(
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Tools]    Script Date: 11.08.2020 16:34:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tools](
	[ToolId] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](50) NULL,
	[Sn] [nvarchar](max) NULL,
	[PurchaseDate] [datetime] NOT NULL,
	[PurchasesValue] [decimal](18, 2) NOT NULL,
	[DocumentNumber] [nvarchar](max) NULL,
	[Warranty] [int] NOT NULL,
	[RentalPrice] [decimal](18, 2) NOT NULL,
	[Destroyed] [bit] NOT NULL,
	[DestroyedDate] [datetime] NULL,
	[Lost] [bit] NOT NULL,
	[LostDate] [datetime] NULL,
	[Description] [nvarchar](max) NULL,
	[DestroyedCustomer_UserId] [uniqueidentifier] NULL,
	[LostCustomer_UserId] [uniqueidentifier] NULL,
	[Producer_CompanyId] [uniqueidentifier] NULL,
 CONSTRAINT [PK_dbo.Tools] PRIMARY KEY CLUSTERED 
(
	[ToolId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Transactions]    Script Date: 11.08.2020 16:34:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Transactions](
	[TransactionId] [uniqueidentifier] NOT NULL,
	[TransactionType] [int] NOT NULL,
	[TransactionDate] [datetime] NOT NULL,
	[TransactionNumber] [int] NOT NULL,
	[PriceForRent] [decimal](18, 2) NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
	[AppUser_UserId] [uniqueidentifier] NOT NULL,
	[Customer_UserId] [uniqueidentifier] NOT NULL,
	[Tool_ToolId] [uniqueidentifier] NOT NULL,
	[User_UserId] [uniqueidentifier] NULL,
 CONSTRAINT [PK_dbo.Transactions] PRIMARY KEY CLUSTERED 
(
	[TransactionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 11.08.2020 16:34:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[UserId] [uniqueidentifier] NOT NULL,
	[FirstName] [nvarchar](50) NOT NULL,
	[LastName] [nvarchar](50) NULL,
	[Phone] [nvarchar](max) NULL,
	[Email] [nvarchar](max) NULL,
	[LoginName] [nvarchar](50) NULL,
	[Password] [nvarchar](50) NULL,
	[UserType] [int] NOT NULL,
	[Company_CompanyId] [uniqueidentifier] NULL,
 CONSTRAINT [PK_dbo.Users] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Index [IX_DestroyedCustomer_UserId]    Script Date: 11.08.2020 16:34:22 ******/
CREATE NONCLUSTERED INDEX [IX_DestroyedCustomer_UserId] ON [dbo].[Tools]
(
	[DestroyedCustomer_UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_LostCustomer_UserId]    Script Date: 11.08.2020 16:34:22 ******/
CREATE NONCLUSTERED INDEX [IX_LostCustomer_UserId] ON [dbo].[Tools]
(
	[LostCustomer_UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Producer_CompanyId]    Script Date: 11.08.2020 16:34:22 ******/
CREATE NONCLUSTERED INDEX [IX_Producer_CompanyId] ON [dbo].[Tools]
(
	[Producer_CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_AppUser_UserId]    Script Date: 11.08.2020 16:34:22 ******/
CREATE NONCLUSTERED INDEX [IX_AppUser_UserId] ON [dbo].[Transactions]
(
	[AppUser_UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Customer_UserId]    Script Date: 11.08.2020 16:34:22 ******/
CREATE NONCLUSTERED INDEX [IX_Customer_UserId] ON [dbo].[Transactions]
(
	[Customer_UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Tool_ToolId]    Script Date: 11.08.2020 16:34:22 ******/
CREATE NONCLUSTERED INDEX [IX_Tool_ToolId] ON [dbo].[Transactions]
(
	[Tool_ToolId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_User_UserId]    Script Date: 11.08.2020 16:34:22 ******/
CREATE NONCLUSTERED INDEX [IX_User_UserId] ON [dbo].[Transactions]
(
	[User_UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Company_CompanyId]    Script Date: 11.08.2020 16:34:22 ******/
CREATE NONCLUSTERED INDEX [IX_Company_CompanyId] ON [dbo].[Users]
(
	[Company_CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Tools]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Tools_dbo.Companies_Producer_CompanyId] FOREIGN KEY([Producer_CompanyId])
REFERENCES [dbo].[Companies] ([CompanyId])
GO
ALTER TABLE [dbo].[Tools] CHECK CONSTRAINT [FK_dbo.Tools_dbo.Companies_Producer_CompanyId]
GO
ALTER TABLE [dbo].[Tools]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Tools_dbo.Users_DestroyedCustomer_UserId] FOREIGN KEY([DestroyedCustomer_UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[Tools] CHECK CONSTRAINT [FK_dbo.Tools_dbo.Users_DestroyedCustomer_UserId]
GO
ALTER TABLE [dbo].[Tools]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Tools_dbo.Users_LostCustomer_UserId] FOREIGN KEY([LostCustomer_UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[Tools] CHECK CONSTRAINT [FK_dbo.Tools_dbo.Users_LostCustomer_UserId]
GO
ALTER TABLE [dbo].[Transactions]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Transactions_dbo.Tools_Tool_ToolId] FOREIGN KEY([Tool_ToolId])
REFERENCES [dbo].[Tools] ([ToolId])
GO
ALTER TABLE [dbo].[Transactions] CHECK CONSTRAINT [FK_dbo.Transactions_dbo.Tools_Tool_ToolId]
GO
ALTER TABLE [dbo].[Transactions]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Transactions_dbo.Users_AppUser_UserId] FOREIGN KEY([AppUser_UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[Transactions] CHECK CONSTRAINT [FK_dbo.Transactions_dbo.Users_AppUser_UserId]
GO
ALTER TABLE [dbo].[Transactions]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Transactions_dbo.Users_Customer_UserId] FOREIGN KEY([Customer_UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[Transactions] CHECK CONSTRAINT [FK_dbo.Transactions_dbo.Users_Customer_UserId]
GO
ALTER TABLE [dbo].[Transactions]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Transactions_dbo.Users_User_UserId] FOREIGN KEY([User_UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[Transactions] CHECK CONSTRAINT [FK_dbo.Transactions_dbo.Users_User_UserId]
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Users_dbo.Companies_Company_CompanyId] FOREIGN KEY([Company_CompanyId])
REFERENCES [dbo].[Companies] ([CompanyId])
GO
ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [FK_dbo.Users_dbo.Companies_Company_CompanyId]
GO
USE [master]
GO
ALTER DATABASE [RentalDatabase] SET  READ_WRITE 
GO
