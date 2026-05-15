IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'racebook')
BEGIN
  CREATE DATABASE racebook;
END
GO