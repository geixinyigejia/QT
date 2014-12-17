USE [QT]
GO

CREATE TABLE [dbo].[PublishTasks](
	[PublishTaskID] [int] IDENTITY(1,1) NOT NULL,
	[PublishUserName] [nvarchar](20) NULL,
	[links] [nvarchar](50) NULL,
	[wangwangxiaohao] [nvarchar](20) NULL,
	[TaskPrice] [money] NULL,
	[charges] [int] NULL,
	[Comment] [nvarchar](50) NULL,
	[TaskType][nvarchar](50) NULL,
	[ReceiverName] [nvarchar](20) NULL,
	[PublishTime] [datetime2](7) NULL,
	[CompleteStatus] [nvarchar](20),
	[CompleteTime] [datetime2](7) NULL,
	[city] [nvarchar](100) NULL,

	)
GO



use QT; 
insert PublishTasks(PublishUserName,links,wangwangxiaohao,TaskPrice,charges,Comment,city,PublishTime,CompleteTime,TaskType ) 
values('admin',N'单链接',N'默认','123','0',N'测试使用',N'北京市北京市','2014/12/17 12:28:25','2014/12/17 12:28:25',N'电脑');

select PublishTasks.PublishUserName,count(PublishTaskID) as PublishTaskCount
from dbo.PublishTasks 
where PublishUserName='admin' and YEAR(PublishTime)=YEAR(GETDATE()) and MONTH(PublishTime)=MONTH(GETDATE()) 
group by PublishTasks.PublishUserName,YEAR(PublishTime),MONTH(PublishTime)

select p.ReceiverName,u.city,p.links,p.wangwangxiaohao,p.TaskPrice,p.TaskType,p.Comment,p.charges from PublishTasks p 
left join QT_USER u on p.ReceiverName=u.UserName where PublishUserName='admin'and ReceiverName is not null 