# Shop System Database Schema Design

This document describes the database schema design for the shop system, including GP purchase history, gacha pull history, and box gacha state persistence.

## Overview

The shop system requires three main tables to track:
1. **GP Purchase History** - Records of items purchased with GP (Game Points)
2. **Gacha Pull History** - Records of gacha pulls and their results
3. **Box Gacha State** - Persistence of box gacha drawn items (items that can only be drawn once)

## Table Designs

### 1. GP Purchase History Table (`ddon_gp_purchase_history`)

Tracks all GP purchases made by players.

```sql
CREATE TABLE IF NOT EXISTS "ddon_gp_purchase_history"
(
    "id"              INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
    "character_id"    INTEGER                           NOT NULL,
    "shop_type"       INTEGER                           NOT NULL,
    "lineup_id"       INTEGER                           NOT NULL,
    "item_id"         INTEGER                           NOT NULL,
    "quantity"        INTEGER                           NOT NULL,
    "gp_cost"         INTEGER                           NOT NULL,
    "purchase_time"   DATETIME                          NOT NULL,
    CONSTRAINT "fk_gp_purchase_history_character_id" FOREIGN KEY ("character_id") REFERENCES "ddon_character" ("character_id") ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS "idx_gp_purchase_history_character_id" ON "ddon_gp_purchase_history" ("character_id");
CREATE INDEX IF NOT EXISTS "idx_gp_purchase_history_purchase_time" ON "ddon_gp_purchase_history" ("purchase_time");
CREATE INDEX IF NOT EXISTS "idx_gp_purchase_history_shop_type" ON "ddon_gp_purchase_history" ("shop_type");
```

**Columns:**
| Column | Type | Description |
|--------|------|-------------|
| id | INTEGER | Auto-increment primary key |
| character_id | INTEGER | FK to ddon_character - the player who made the purchase |
| shop_type | INTEGER | The type of GP shop (main shop, event shop, etc.) |
| lineup_id | INTEGER | The lineup/slot ID in the shop |
| item_id | INTEGER | The ID of the item purchased |
| quantity | INTEGER | Number of items purchased |
| gp_cost | INTEGER | Total GP spent on this purchase |
| purchase_time | DATETIME | When the purchase was made |

**Indexes:**
- `character_id` - For player purchase history lookups
- `purchase_time` - For time-based queries and cleanup
- `shop_type` - For filtering by shop type

### 2. Gacha Pull History Table (`ddon_gacha_pull_history`)

Records every gacha pull and its results.

```sql
CREATE TABLE IF NOT EXISTS "ddon_gacha_pull_history"
(
    "id"              INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
    "character_id"    INTEGER                           NOT NULL,
    "gacha_id"        INTEGER                           NOT NULL,
    "pull_type"       INTEGER                           NOT NULL,
    "currency_type"   INTEGER                           NOT NULL,
    "currency_cost"   INTEGER                           NOT NULL,
    "pull_count"      INTEGER                           NOT NULL,
    "pull_time"       DATETIME                          NOT NULL,
    CONSTRAINT "fk_gacha_pull_history_character_id" FOREIGN KEY ("character_id") REFERENCES "ddon_character" ("character_id") ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS "ddon_gacha_pull_result"
(
    "id"              INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
    "pull_id"         INTEGER                           NOT NULL,
    "result_index"    INTEGER                           NOT NULL,
    "item_id"         INTEGER                           NOT NULL,
    "quantity"        INTEGER                           NOT NULL,
    "rarity"          INTEGER                           NOT NULL,
    "is_bonus"        BOOLEAN                           NOT NULL DEFAULT FALSE,
    CONSTRAINT "fk_gacha_pull_result_pull_id" FOREIGN KEY ("pull_id") REFERENCES "ddon_gacha_pull_history" ("id") ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS "idx_gacha_pull_history_character_id" ON "ddon_gacha_pull_history" ("character_id");
CREATE INDEX IF NOT EXISTS "idx_gacha_pull_history_gacha_id" ON "ddon_gacha_pull_history" ("gacha_id");
CREATE INDEX IF NOT EXISTS "idx_gacha_pull_history_pull_time" ON "ddon_gacha_pull_history" ("pull_time");
CREATE INDEX IF NOT EXISTS "idx_gacha_pull_result_pull_id" ON "ddon_gacha_pull_result" ("pull_id");
```

**ddon_gacha_pull_history Columns:**
| Column | Type | Description |
|--------|------|-------------|
| id | INTEGER | Auto-increment primary key |
| character_id | INTEGER | FK to ddon_character |
| gacha_id | INTEGER | The gacha banner/pool ID |
| pull_type | INTEGER | Single pull (1) or multi-pull (10, etc.) |
| currency_type | INTEGER | GP, tickets, or other currency type |
| currency_cost | INTEGER | Amount of currency spent |
| pull_count | INTEGER | Number of items received (may differ from pull_type due to bonuses) |
| pull_time | DATETIME | When the pull occurred |

**ddon_gacha_pull_result Columns:**
| Column | Type | Description |
|--------|------|-------------|
| id | INTEGER | Auto-increment primary key |
| pull_id | INTEGER | FK to ddon_gacha_pull_history |
| result_index | INTEGER | Order of the result in the pull (0-indexed) |
| item_id | INTEGER | The item received |
| quantity | INTEGER | Quantity of the item |
| rarity | INTEGER | Rarity tier of the item |
| is_bonus | BOOLEAN | Whether this was a bonus item (e.g., pity system) |

### 3. Box Gacha State Table (`ddon_box_gacha_state`)

Tracks which items have been drawn from box gacha (where items can only be obtained once).

```sql
CREATE TABLE IF NOT EXISTS "ddon_box_gacha_state"
(
    "character_id"    INTEGER NOT NULL,
    "box_gacha_id"    INTEGER NOT NULL,
    "lineup_id"       INTEGER NOT NULL,
    "drawn_time"      DATETIME NOT NULL,
    CONSTRAINT "pk_ddon_box_gacha_state" PRIMARY KEY ("character_id", "box_gacha_id", "lineup_id"),
    CONSTRAINT "fk_box_gacha_state_character_id" FOREIGN KEY ("character_id") REFERENCES "ddon_character" ("character_id") ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS "idx_box_gacha_state_character_box" ON "ddon_box_gacha_state" ("character_id", "box_gacha_id");
```

**Columns:**
| Column | Type | Description |
|--------|------|-------------|
| character_id | INTEGER | FK to ddon_character |
| box_gacha_id | INTEGER | The box gacha pool ID |
| lineup_id | INTEGER | The specific item lineup that was drawn |
| drawn_time | DATETIME | When the item was drawn |

**Design Rationale:**
- Uses composite primary key (character_id, box_gacha_id, lineup_id) to ensure each item in a box can only be drawn once per player
- Simple structure since we only need to track "drawn" state, not quantities
- drawn_time allows for debugging and potential reset functionality

## Model Classes

### GpPurchaseHistory.cs

```csharp
#nullable enable
using System;

namespace Arrowgene.Ddon.Database.Model;

public class GpPurchaseHistory
{
    public ulong Id { get; set; }
    public uint CharacterId { get; set; }
    public uint ShopType { get; set; }
    public uint LineupId { get; set; }
    public uint ItemId { get; set; }
    public uint Quantity { get; set; }
    public uint GpCost { get; set; }
    public DateTime PurchaseTime { get; set; }
}
```

### GachaPullHistory.cs

```csharp
#nullable enable
using System;
using System.Collections.Generic;

namespace Arrowgene.Ddon.Database.Model;

public class GachaPullHistory
{
    public ulong Id { get; set; }
    public uint CharacterId { get; set; }
    public uint GachaId { get; set; }
    public uint PullType { get; set; }
    public uint CurrencyType { get; set; }
    public uint CurrencyCost { get; set; }
    public uint PullCount { get; set; }
    public DateTime PullTime { get; set; }
    public List<GachaPullResult> Results { get; set; } = new();
}

public class GachaPullResult
{
    public ulong Id { get; set; }
    public ulong PullId { get; set; }
    public uint ResultIndex { get; set; }
    public uint ItemId { get; set; }
    public uint Quantity { get; set; }
    public uint Rarity { get; set; }
    public bool IsBonus { get; set; }
}
```

### BoxGachaState.cs

```csharp
#nullable enable
using System;

namespace Arrowgene.Ddon.Database.Model;

public class BoxGachaState
{
    public uint CharacterId { get; set; }
    public uint BoxGachaId { get; set; }
    public uint LineupId { get; set; }
    public DateTime DrawnTime { get; set; }
}
```

## IDatabase Interface Additions

The following methods should be added to `IDatabase.cs`:

```csharp
// GP Purchase History
long InsertGpPurchaseHistory(GpPurchaseHistory purchase, DbConnection? connectionIn = null);
List<GpPurchaseHistory> SelectGpPurchaseHistory(uint characterId, DbConnection? connectionIn = null);
List<GpPurchaseHistory> SelectGpPurchaseHistoryByShopType(uint characterId, uint shopType, DbConnection? connectionIn = null);

// Gacha Pull History
long InsertGachaPullHistory(GachaPullHistory pull, DbConnection? connectionIn = null);
long InsertGachaPullResult(GachaPullResult result, DbConnection? connectionIn = null);
List<GachaPullHistory> SelectGachaPullHistory(uint characterId, DbConnection? connectionIn = null);
List<GachaPullHistory> SelectGachaPullHistoryByGachaId(uint characterId, uint gachaId, DbConnection? connectionIn = null);
List<GachaPullResult> SelectGachaPullResults(ulong pullId, DbConnection? connectionIn = null);

// Box Gacha State
bool InsertBoxGachaState(BoxGachaState state, DbConnection? connectionIn = null);
bool DeleteBoxGachaState(uint characterId, uint boxGachaId, DbConnection? connectionIn = null);
List<BoxGachaState> SelectBoxGachaState(uint characterId, uint boxGachaId, DbConnection? connectionIn = null);
HashSet<uint> SelectBoxGachaDrawnLineups(uint characterId, uint boxGachaId, DbConnection? connectionIn = null);
bool HasBoxGachaDrawnLineup(uint characterId, uint boxGachaId, uint lineupId, DbConnection? connectionIn = null);
```

## Migration Strategy

The migration will be number 59 (following the current highest at 58).

### Migration File: `migration_shop_history.sql`

Located at: `Arrowgene.Ddon.Database/Files/Database/Script/migration_shop_history.sql`

### Migration Class: `00000059_ShopHistoryMigration.cs`

```csharp
using System.Data.Common;

namespace Arrowgene.Ddon.Database.Sql.Core.Migration
{
    public class ShopHistoryMigration(DatabaseSetting databaseSetting) : IMigrationStrategy
    {
        public uint From => 58;
        public uint To => 59;

        public bool Migrate(IDatabase db, DbConnection conn)
        {
            string adaptedSchema = DdonDatabaseBuilder.GetAdaptedSchema(databaseSetting, "Script/migration_shop_history.sql");
            db.Execute(conn, adaptedSchema, true);
            return true;
        }
    }
}
```

## Design Considerations

### Why Separate Tables for History vs State?

1. **GP Purchase History** and **Gacha Pull History** are append-only logs. They grow over time and are used for:
   - Player account history
   - Customer support investigations
   - Analytics and metrics

2. **Box Gacha State** is active game state that affects gameplay. It needs:
   - Fast lookups to check if an item is available
   - Potential resets when box refreshes

### Data Retention

Consider implementing cleanup jobs for old history data:
- GP purchases older than X days could be archived or deleted
- Gacha pull history could be summarized after a period

### Performance

The indexes are designed for common query patterns:
- Looking up a player's history
- Filtering by shop/gacha type
- Time-based queries for cleanup

### Foreign Key Relationships

All tables have ON DELETE CASCADE to ddon_character, ensuring:
- Character deletion cleans up all related shop data
- No orphaned records

## Files to Create/Modify

1. **New Files:**
   - `Arrowgene.Ddon.Database/Files/Database/Script/migration_shop_history.sql`
   - `Arrowgene.Ddon.Database/Model/GpPurchaseHistory.cs`
   - `Arrowgene.Ddon.Database/Model/GachaPullHistory.cs`
   - `Arrowgene.Ddon.Database/Model/BoxGachaState.cs`
   - `Arrowgene.Ddon.Database/Sql/Core/DdonSqlDbGpPurchaseHistory.cs`
   - `Arrowgene.Ddon.Database/Sql/Core/DdonSqlDbGachaPullHistory.cs`
   - `Arrowgene.Ddon.Database/Sql/Core/DdonSqlDbBoxGachaState.cs`
   - `Arrowgene.Ddon.Database/Sql/Core/Migration/00000059_ShopHistoryMigration.cs`

2. **Files to Modify:**
   - `Arrowgene.Ddon.Database/IDatabase.cs` - Add interface methods
   - `Arrowgene.Ddon.Database/Sql/SqlDb.cs` - Add abstract method stubs
   - `Arrowgene.Ddon.Database/Files/Database/Script/schema_sqlite.sql` - Add tables to main schema
