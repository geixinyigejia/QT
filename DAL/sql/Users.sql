USE [QT]
GO


CREATE TABLE [dbo].[QT_USER](
	[UserID] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [nvarchar](20) NOT NULL,
	[points] [int] NULL,
	[city] [nvarchar](100) NOT NULL,
	[QQ] [bigint] NOT NULL,
	[MobilePhone] [bigint] NULL,
	[RegistTime] [datetime2](7) NULL,
	[Checkstatus] [nvarchar](20) NOT NULL,
	[Password] [nvarchar](100) NULL,
	[LastLogonTime] [datetime2](7) NULL,
	[Acountstatus] [nvarchar](20) NOT NULL,
	[AcountStartTime] [datetime2](7) NULL,
	[UserType] [nvarchar](20) NULL
)

GO


update QT_USER set UserType=N'管理员' where UserName='admin'


