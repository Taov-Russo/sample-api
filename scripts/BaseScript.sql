USE [master]
GO

CREATE DATABASE [SampleApiDB]
GO

USE [SampleApiDB]
GO

CREATE TABLE [User] (
	[UserId] [uniqueidentifier] NOT NULL,
	[Login] [nvarchar](50) NOT NULL,
	[PasswordHash] [varbinary](64) NOT NULL
	CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
	(
		[UserId] ASC
	)
)
GO

CREATE TABLE [Task](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[AverageTime] [integer] NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	CONSTRAINT [PK_Task] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)
)
GO

ALTER TABLE [Task] WITH CHECK ADD CONSTRAINT [FK_TaskToUser] FOREIGN KEY([UserId])
	REFERENCES [User] ([UserId])
GO

CREATE TABLE [Pipeline](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	CONSTRAINT [PK_Pipeline] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)
)
GO

CREATE TABLE [PipelineTask](
	[PipelineId] [int] NOT NULL,
	[TaskId] [int] NOT NULL
	CONSTRAINT [PK_PipelineTask] PRIMARY KEY CLUSTERED 
	(
		[PipelineId] ASC,
		[TaskId] ASC
	)
)
GO

ALTER TABLE [PipelineTask] WITH CHECK ADD CONSTRAINT [FK_PipelineTaskToPipeline] FOREIGN KEY([PipelineId])
	REFERENCES [Pipeline] ([Id])
GO

ALTER TABLE [PipelineTask] WITH CHECK ADD CONSTRAINT [FK_PipelineTaskToTask] FOREIGN KEY([TaskId])
	REFERENCES [Task] ([Id])
GO

CREATE TABLE [PipelineRunHistory](
	[PipelineId] [int] NOT NULL,
	[TotalExecutionTime] [int] NOT NULL
)
GO

ALTER TABLE [PipelineTask] WITH CHECK ADD CONSTRAINT [FK_PipelineRunHistoryToPipeline] FOREIGN KEY([PipelineId])
	REFERENCES [Pipeline] ([Id])
GO