USE [master]
GO
CREATE LOGIN [sample_api] WITH PASSWORD=N'sample_api', DEFAULT_DATABASE=[master], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO
USE [SampleApiDB]
GO
CREATE USER [sample_api] FOR LOGIN [sample_api]
GO
USE [SampleApiDB]
GO
ALTER ROLE [db_owner] ADD MEMBER [sample_api]
GO
