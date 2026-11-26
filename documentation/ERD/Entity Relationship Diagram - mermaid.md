# Entity Relationship Diagram

Mermaid:

```
erDiagram
    User ||--o{ Mod : uploads
    User ||--o{ FavouriteMod : saves
    Category ||--o{ Mod : classifies
    Mod ||--o{ FavouriteMod : "is saved as"

    User {
        uniqueidentifier Uid PK
        nvarchar Email
        nvarchar Username
        varchar PasswordHash
    }

    Category {
        varchar Name PK
        int DisplayOrder
    }

    Mod {
        uniqueidentifier ModId PK
        uniqueidentifier Uid FK
        varchar Type FK
        nvarchar Title
        nvarchar Description
        date UploadDate
        date EditDate
        nvarchar ModFileUrl
        nvarchar ImageUrl
    }

    FavouriteMod {
        uniqueidentifier FavouriteId PK
        uniqueidentifier Uid FK
        uniqueidentifier ModId FK
    
```
