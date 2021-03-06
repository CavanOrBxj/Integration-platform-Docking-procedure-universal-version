USE [volador]
GO
/****** 对象:  Table [dbo].[EBTime_Strategy]    脚本日期: 11/01/2018 16:48:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[EBTime_Strategy](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[StartTime] [varchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[EndTime] [varchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[EvenType] [varchar](50) COLLATE Chinese_PRC_CI_AS NULL,
 CONSTRAINT [PK_EBTime_Strategy] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF