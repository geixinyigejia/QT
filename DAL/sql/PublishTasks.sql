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
	[ReceiverName] [nvarchar](20) default '',
	[ReceiverConfirm] [bit] default 0,
	[PublishTime] [datetime2](7) NULL,
	[CompleteStatus] [nvarchar](20),
	[CompleteTime] [datetime2](7) NULL,
	[city] [nvarchar](100) NULL,
	[IsDeleted] [bit] default 0,
	[ShuadanType][nvarchar](50) NULL,
	[payment][nvarchar](50) NULL,
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
left join QT_USER u on p.ReceiverName=u.UserName where PublishUserName='admin'and ReceiverName!=''

select * from PublishTasks

delete  from PublishTasks

update PublishTasks set CompleteStatus=N'等待完成' where PublishTaskID=37

快刷15分钟：
单  白号3   加链接不加佣金
单1-3心    5
单4心    6
单钻号    7

双链接全部在此基础上多加1元，手机全部在此基础上再多加1元
双链接全部在此基础上多加1元，手机全部在此基础上再多加1元

财富通付款 超过500元 加1块