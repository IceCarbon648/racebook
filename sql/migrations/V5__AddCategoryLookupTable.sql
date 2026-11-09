CREATE TABLE Category (
    Name VARCHAR(16) PRIMARY KEY,
    DisplayOrder INT NOT NULL DEFAULT 0
);

INSERT INTO Category (Name, DisplayOrder) VALUES
    ('ENVIRONMENT', 1), ('VEHICLE', 2), ('PACK', 3), ('UI', 4), ('MISC', 5);

ALTER TABLE Mod
ADD CONSTRAINT FK_Mod_Category
    FOREIGN KEY (Type) REFERENCES Category(Name);