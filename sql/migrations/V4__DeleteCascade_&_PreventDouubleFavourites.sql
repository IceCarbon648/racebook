-- 1. Find the system-generated name of the ModId foreign key
DECLARE @constraintName NVARCHAR(128);

SELECT @constraintName = fk.name
FROM sys.foreign_keys fk
INNER JOIN sys.foreign_key_columns fkc
    ON fk.object_id = fkc.constraint_object_id
INNER JOIN sys.columns c
    ON fkc.parent_object_id = c.object_id
   AND fkc.parent_column_id = c.column_id
WHERE fk.parent_object_id = OBJECT_ID('FavouriteMod')
  AND c.name = 'ModId';

-- 2. Drop it
IF @constraintName IS NOT NULL
BEGIN
    DECLARE @sql NVARCHAR(MAX) =
        N'ALTER TABLE FavouriteMod DROP CONSTRAINT ' + QUOTENAME(@constraintName);
    EXEC sp_executesql @sql;
END

-- 3. Clear out any existing duplicate pairs, keeping one of each
;WITH Duplicates AS (
    SELECT FavouriteId,
           ROW_NUMBER() OVER (PARTITION BY Uid, ModId ORDER BY FavouriteId) AS RowNum
    FROM FavouriteMod
)
DELETE FROM FavouriteMod
WHERE FavouriteId IN (SELECT FavouriteId FROM Duplicates WHERE RowNum > 1);

-- 4. Recreate the foreign key, named, with cascade
ALTER TABLE FavouriteMod
ADD CONSTRAINT FK_FavouriteMod_Mod
    FOREIGN KEY (ModId) REFERENCES Mod(ModId)
    ON DELETE CASCADE;

-- 5. Prevent the same user favouriting the same mod twice
ALTER TABLE FavouriteMod
ADD CONSTRAINT UQ_FavouriteMod_Uid_ModId UNIQUE (Uid, ModId);