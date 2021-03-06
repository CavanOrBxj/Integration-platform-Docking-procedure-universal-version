USE [volador]
GO
/****** 对象:  Table [dbo].[CheckEBMData]    脚本日期: 11/01/2018 16:42:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CheckEBMData](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[EBDID] [varchar](255) COLLATE Chinese_PRC_CI_AS NULL,
	[EBDDID] [varchar](255) COLLATE Chinese_PRC_CI_AS NULL,
	[CodeA] [varchar](255) COLLATE Chinese_PRC_CI_AS NULL,
	[NameA] [varchar](255) COLLATE Chinese_PRC_CI_AS NULL,
	[EBMID] [varchar](255) COLLATE Chinese_PRC_CI_AS NULL,
	[SentTime] [varchar](255) COLLATE Chinese_PRC_CI_AS NULL,
	[EBMStartTime] [varchar](255) COLLATE Chinese_PRC_CI_AS NULL,
	[EBMEndTime] [varchar](255) COLLATE Chinese_PRC_CI_AS NULL,
	[EBMTitle] [varchar](255) COLLATE Chinese_PRC_CI_AS NULL,
	[EBMType] [varchar](255) COLLATE Chinese_PRC_CI_AS NULL,
	[EBMDesc] [varchar](1000) COLLATE Chinese_PRC_CI_AS NULL,
	[EBMCode] [varchar](255) COLLATE Chinese_PRC_CI_AS NULL,
	[EBMUrl] [varchar](500) COLLATE Chinese_PRC_CI_AS NULL,
	[CheckStatus] [varchar](255) COLLATE Chinese_PRC_CI_AS NULL,
 CONSTRAINT [PK_CheckEBMData] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF