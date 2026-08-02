DROP TABLE IF EXISTS PreviewImage;

EXEC sp_rename 'dbo.Mod.FilePath', 'ModFileUrl', 'COLUMN';

ALTER TABLE dbo.Mod
ADD ImageUrl NVARCHAR(128) NOT NULL;