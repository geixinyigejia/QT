USE [QT]
GO

CREATE TABLE [dbo].[ReceivedTasks](
	[ReceivedTaskID] [int] IDENTITY(1,1) NOT NULL,
	[PublishUserName] [nvarchar](20) NULL,
	[city] [nvarchar](50) NULL,
	[links] [int] NULL,
	[wangwangxiaohao] [nvarchar](20) NULL,
	[Price] [money] NULL,
	[Point] [int] NULL,
	[Comment] [nvarchar](50) NULL,
	[ReceiverName] [nvarchar](20) NULL,
	[PublishTime] [datetime2](7) NULL,
	[CompleteStatus] [bit] NULL,
	[CompleteTime] [datetime2](7) NULL,
	) 

GO



