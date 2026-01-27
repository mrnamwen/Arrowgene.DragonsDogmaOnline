# Implementation Patterns for GP Shop and Gacha Systems

This document outlines the patterns found in the existing codebase for implementing new handlers, managers, packet structures, and assets.

## 1. Handler Patterns

### 1.1 Request/Response Handler Pattern (Preferred)

The `GameRequestPacketHandler<TReq, TRes>` base class is the preferred pattern for handlers that process a request and return a response.

**Location:** `/Arrowgene.Ddon.GameServer/GameRequestPacketHandler.cs`

```csharp
public class ExampleHandler : GameRequestPacketHandler<C2SExampleReq, S2CExampleRes>
{
    private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(ExampleHandler));

    public ExampleHandler(DdonGameServer server) : base(server)
    {
    }

    public override S2CExampleRes Handle(GameClient client, C2SExampleReq request)
    {
        // Business logic here
        return new S2CExampleRes()
        {
            // Response fields
        };
    }
}
```

**Example:** `/Arrowgene.Ddon.GameServer/Handler/ShopBuyShopGoodsHandler.cs`
- Uses `GameRequestPacketHandler<C2SShopBuyShopGoodsReq, S2CShopBuyShopGoodsRes>`
- Automatically handles error responses via `ResponseErrorException`
- Returns the response object directly

### 1.2 Simple Packet Handler Pattern

For handlers that need more control or don't follow request/response pattern, use `PacketHandler<GameClient>`.

**Location:** `/Arrowgene.Ddon.Server/Network/PacketHandler.cs`

```csharp
public class ExampleHandler : PacketHandler<GameClient>
{
    public override PacketId Id => PacketId.C2S_EXAMPLE_REQ;

    public ExampleHandler(DdonGameServer server) : base(server)
    {
    }

    public override void Handle(GameClient client, IPacket packet)
    {
        // Manual packet handling
        client.Send(response);
    }
}
```

**Example:** `/Arrowgene.Ddon.GameServer/Handler/GpGpCourseGetAvailableListHandler.cs`

### 1.3 Handler Registration

Handlers are registered in `DdonGameServer.LoadPacketHandler()` method:

```csharp
private void LoadPacketHandler()
{
    // ... existing handlers
    AddHandler(new GpGpShopGetItemLineupHandler(this));
    AddHandler(new GachaGachaListHandler(this));
}
```

**Location:** `/Arrowgene.Ddon.GameServer/DdonGameServer.cs:LoadPacketHandler()` (lines 175-515)

---

## 2. Packet Structure Patterns

### 2.1 Request Packet (C2S - Client to Server)

**Location:** `/Arrowgene.Ddon.Shared/Entity/PacketStructure/`

```csharp
using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Network;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class C2SGpGpShopGetItemLineupReq : IPacketStructure
    {
        public PacketId Id => PacketId.C2S_GP_GP_SHOP_GET_ITEM_LINEUP_REQ;

        public C2SGpGpShopGetItemLineupReq()
        {
            // Initialize lists/strings with defaults
        }

        public uint CategoryId { get; set; }

        public class Serializer : PacketEntitySerializer<C2SGpGpShopGetItemLineupReq>
        {
            public override void Write(IBuffer buffer, C2SGpGpShopGetItemLineupReq obj)
            {
                WriteUInt32(buffer, obj.CategoryId);
            }

            public override C2SGpGpShopGetItemLineupReq Read(IBuffer buffer)
            {
                C2SGpGpShopGetItemLineupReq obj = new C2SGpGpShopGetItemLineupReq();
                obj.CategoryId = ReadUInt32(buffer);
                return obj;
            }
        }
    }
}
```

### 2.2 Response Packet (S2C - Server to Client)

```csharp
using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Entity.Structure;
using Arrowgene.Ddon.Shared.Network;
using System.Collections.Generic;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class S2CGpGpShopGetItemLineupRes : ServerResponse
    {
        public override PacketId Id => PacketId.S2C_GP_GP_SHOP_GET_ITEM_LINEUP_RES;

        public S2CGpGpShopGetItemLineupRes()
        {
            ItemList = new List<CDataGpShopItem>();
        }

        public List<CDataGpShopItem> ItemList { get; set; }

        public class Serializer : PacketEntitySerializer<S2CGpGpShopGetItemLineupRes>
        {
            public override void Write(IBuffer buffer, S2CGpGpShopGetItemLineupRes obj)
            {
                WriteServerResponse(buffer, obj);
                WriteEntityList<CDataGpShopItem>(buffer, obj.ItemList);
            }

            public override S2CGpGpShopGetItemLineupRes Read(IBuffer buffer)
            {
                S2CGpGpShopGetItemLineupRes obj = new S2CGpGpShopGetItemLineupRes();
                ReadServerResponse(buffer, obj);
                obj.ItemList = ReadEntityList<CDataGpShopItem>(buffer);
                return obj;
            }
        }
    }
}
```

### 2.3 Data Structures (CData)

**Location:** `/Arrowgene.Ddon.Shared/Entity/Structure/`

```csharp
using Arrowgene.Buffers;

namespace Arrowgene.Ddon.Shared.Entity.Structure
{
    public class CDataGpShopItem
    {
        public CDataGpShopItem()
        {
        }

        public uint ItemId { get; set; }
        public uint Price { get; set; }
        public byte Stock { get; set; }

        public class Serializer : EntitySerializer<CDataGpShopItem>
        {
            public override void Write(IBuffer buffer, CDataGpShopItem obj)
            {
                WriteUInt32(buffer, obj.ItemId);
                WriteUInt32(buffer, obj.Price);
                WriteByte(buffer, obj.Stock);
            }

            public override CDataGpShopItem Read(IBuffer buffer)
            {
                CDataGpShopItem obj = new CDataGpShopItem();
                obj.ItemId = ReadUInt32(buffer);
                obj.Price = ReadUInt32(buffer);
                obj.Stock = ReadByte(buffer);
                return obj;
            }
        }
    }
}
```

### 2.4 Serializer Registration

All serializers must be registered in `EntitySerializer.cs`:

**Location:** `/Arrowgene.Ddon.Shared/Entity/EntitySerializer.cs`

In the static constructor, add:
```csharp
// Data structures
Create(new CDataGpShopItem.Serializer());

// Packet structures
Create(new C2SGpGpShopGetItemLineupReq.Serializer(), PacketId.C2S_GP_GP_SHOP_GET_ITEM_LINEUP_REQ);
Create(new S2CGpGpShopGetItemLineupRes.Serializer(), PacketId.S2C_GP_GP_SHOP_GET_ITEM_LINEUP_RES);
```

---

## 3. Packet IDs

**Location:** `/Arrowgene.Ddon.Shared/Network/PacketId.cs`

Existing GP Shop packet IDs (Group 28):
```csharp
// Already defined in PacketId.cs:
C2S_GP_GP_SHOP_DISPLAY_GET_TYPE_REQ      // 28, 7, 1
S2C_GP_GP_SHOP_DISPLAY_GET_TYPE_RES      // 28, 7, 2
C2S_GP_GP_SHOP_DISPLAY_GET_LINEUP_REQ    // 28, 8, 1
S2C_GP_GP_SHOP_DISPLAY_GET_LINEUP_RES    // 28, 8, 2
C2S_GP_GP_SHOP_DISPLAY_BUY_REQ           // 28, 9, 1
S2C_GP_GP_SHOP_DISPLAY_BUY_RES           // 28, 9, 2
C2S_GP_GP_SHOP_GET_COURSE_LINEUP_REQ     // 28, 10, 1
S2C_GP_GP_SHOP_GET_COURSE_LINEUP_RES     // 28, 10, 2
C2S_GP_GP_SHOP_GET_ITEM_LINEUP_REQ       // 28, 11, 1
S2C_GP_GP_SHOP_GET_ITEM_LINEUP_RES       // 28, 11, 2
C2S_GP_GP_SHOP_GET_PAWN_LINEUP_REQ       // 28, 12, 1
S2C_GP_GP_SHOP_GET_PAWN_LINEUP_RES       // 28, 12, 2
```

Existing Gacha packet IDs (Group 39):
```csharp
C2S_GACHA_GACHA_LIST_REQ    // 39, 0, 1
S2C_GACHA_GACHA_LIST_RES    // 39, 0, 2
C2S_GACHA_GACHA_BUY_REQ     // 39, 1, 1
S2C_GACHA_GACHA_BUY_RES     // 39, 1, 2
```

Box Gacha packet IDs (Group 57):
```csharp
C2S_BOX_GACHA_BOX_GACHA_LIST_REQ       // 57, 0, 1
S2C_BOX_GACHA_BOX_GACHA_LIST_RES       // 57, 0, 2
C2S_BOX_GACHA_BOX_GACHA_BUY_REQ        // 57, 1, 1
S2C_BOX_GACHA_BOX_GACHA_BUY_RES        // 57, 1, 2
C2S_BOX_GACHA_BOX_GACHA_RESET_REQ      // 57, 2, 1
S2C_BOX_GACHA_BOX_GACHA_RESET_RES      // 57, 2, 2
C2S_BOX_GACHA_BOX_GACHA_DRAW_INFO_REQ  // 57, 3, 1
S2C_BOX_GACHA_BOX_GACHA_DRAW_INFO_RES  // 57, 3, 2
```

---

## 4. Manager Patterns

### 4.1 Simple Manager (No Database)

**Example:** `/Arrowgene.Ddon.GameServer/Characters/GpCourseManager.cs`

```csharp
public class GpShopManager
{
    private DdonGameServer _Server;

    public GpShopManager(DdonGameServer server)
    {
        _Server = server;
    }

    public List<GpShopItem> GetItemLineup(uint categoryId)
    {
        // Access assets via _Server.AssetRepository
        return _Server.AssetRepository.GpShopAsset.GetItems(categoryId);
    }
}
```

### 4.2 Manager with AssetRepository Integration

**Example:** `/Arrowgene.Ddon.GameServer/Shop/ShopManager.cs`

```csharp
public class ShopManager : AssetManager<Shared.Model.Shop>
{
    protected Dictionary<uint, S2CShopGetShopGoodsListRes> Goods;

    public ShopManager(AssetRepository assetRepository, IDatabase database)
        : base(assetRepository, AssetRepository.ShopKey, database, assetRepository.ShopAsset)
    {
    }

    protected override void OnInit()
    {
        Goods = new Dictionary<uint, S2CShopGetShopGoodsListRes>();
    }

    public override void Load()
    {
        Goods.Clear();
        foreach (Shared.Model.Shop shop in this._assetList)
        {
            Goods.Add(shop.ShopId, shop.Data);
        }
    }

    public S2CShopGetShopGoodsListRes GetAssets(uint ShopId)
    {
        return Goods.GetValueOrDefault(ShopId, new S2CShopGetShopGoodsListRes());
    }
}
```

### 4.3 Manager Registration in DdonGameServer

**Location:** `/Arrowgene.Ddon.GameServer/DdonGameServer.cs` constructor

```csharp
public DdonGameServer(GameServerSetting setting, ...) : base(...)
{
    // ... other managers
    ShopManager = new ShopManager(assetRepository, database);
    GpShopManager = new GpShopManager(this);  // Add new manager
    GachaManager = new GachaManager(this);    // Add new manager
}

// Add property
public GpShopManager GpShopManager { get; }
public GachaManager GachaManager { get; }
```

---

## 5. Asset System Patterns

### 5.1 Asset Class Definition

**Location:** `/Arrowgene.Ddon.Shared/Asset/`

```csharp
using System.Collections.Generic;

namespace Arrowgene.Ddon.Shared.Asset
{
    public class GpShopAsset
    {
        public GpShopAsset()
        {
            Categories = new Dictionary<uint, GpShopCategory>();
            Items = new Dictionary<uint, GpShopItem>();
        }

        public Dictionary<uint, GpShopCategory> Categories { get; set; }
        public Dictionary<uint, GpShopItem> Items { get; set; }
    }

    public class GpShopCategory
    {
        public uint Id { get; set; }
        public string Name { get; set; }
    }

    public class GpShopItem
    {
        public uint Id { get; set; }
        public uint ItemId { get; set; }
        public uint CategoryId { get; set; }
        public uint Price { get; set; }
        public uint Stock { get; set; }
    }
}
```

### 5.2 Asset Deserializer

**Location:** `/Arrowgene.Ddon.Shared/AssetReader/`

```csharp
using System.IO;
using System.Text.Json;
using Arrowgene.Ddon.Shared.Asset;
using Arrowgene.Logging;

namespace Arrowgene.Ddon.Shared.AssetReader
{
    public class GpShopAssetDeserializer : IAssetDeserializer<GpShopAsset>
    {
        private static readonly ILogger Logger = LogProvider.Logger(typeof(GpShopAssetDeserializer));

        public GpShopAsset ReadPath(string path)
        {
            Logger.Info($"Reading {path}");

            GpShopAsset asset = new GpShopAsset();
            string json = Util.ReadAllText(path);
            JsonDocument document = JsonDocument.Parse(json);

            var categories = document.RootElement.GetProperty("categories").EnumerateArray();
            foreach (var category in categories)
            {
                GpShopCategory cat = new GpShopCategory();
                cat.Id = category.GetProperty("id").GetUInt32();
                cat.Name = category.GetProperty("name").GetString();
                asset.Categories.Add(cat.Id, cat);
            }

            var items = document.RootElement.GetProperty("items").EnumerateArray();
            foreach (var item in items)
            {
                GpShopItem shopItem = new GpShopItem();
                shopItem.Id = item.GetProperty("id").GetUInt32();
                shopItem.ItemId = item.GetProperty("item_id").GetUInt32();
                shopItem.CategoryId = item.GetProperty("category_id").GetUInt32();
                shopItem.Price = item.GetProperty("price").GetUInt32();
                shopItem.Stock = item.GetProperty("stock").GetUInt32();
                asset.Items.Add(shopItem.Id, shopItem);
            }

            return asset;
        }
    }
}
```

### 5.3 Asset Registration in AssetRepository

**Location:** `/Arrowgene.Ddon.Shared/AssetRepository.cs`

1. Add the key constant:
```csharp
public const string GpShopKey = "GpShop.json";
```

2. Add the property:
```csharp
public GpShopAsset GpShopAsset { get; private set; }
```

3. Initialize in constructor:
```csharp
GpShopAsset = new();
```

4. Register in `Initialize()`:
```csharp
RegisterAsset(value => GpShopAsset = value, GpShopKey, new GpShopAssetDeserializer());
```

### 5.4 JSON Asset File Format

**Location:** `/Arrowgene.Ddon.Shared/Files/Assets/GpShop.json`

```json
{
    "comment": "GP Shop configuration",
    "categories": [
        {
            "id": 1,
            "name": "Items"
        },
        {
            "id": 2,
            "name": "Courses"
        }
    ],
    "items": [
        {
            "id": 1,
            "item_id": 123,
            "category_id": 1,
            "price": 100,
            "stock": 255
        }
    ]
}
```

---

## 6. Error Handling Pattern

Use `ResponseErrorException` to return error codes:

```csharp
public override S2CExampleRes Handle(GameClient client, C2SExampleReq request)
{
    if (someCondition)
    {
        throw new ResponseErrorException(ErrorCode.ERROR_CODE_SHOP_LACK_MONEY);
    }

    // Normal processing
    return new S2CExampleRes();
}
```

Error codes are defined in `/Arrowgene.Ddon.Shared/Model/ErrorCode.cs`.

---

## 7. Database Transaction Pattern

For operations that modify multiple database records:

```csharp
Server.Database.ExecuteInTransaction(connection =>
{
    // All database operations here use the same connection
    var result1 = Server.ItemManager.AddItem(Server, client.Character, true, itemId, amount, connectionIn: connection);
    var result2 = Server.WalletManager.RemoveFromWallet(client.Character, WalletType.GoldenGemstones, price, connection);
});
```

---

## 8. Wallet Operations

**Location:** `/Arrowgene.Ddon.GameServer/Characters/WalletManager.cs`

```csharp
// Adding currency
CDataUpdateWalletPoint update = Server.WalletManager.AddToWallet(
    client.Character,
    WalletType.GoldenGemstones,
    amount,
    connectionIn: connection
);

// Removing currency (returns null if insufficient funds)
CDataUpdateWalletPoint update = Server.WalletManager.RemoveFromWallet(
    client.Character,
    WalletType.GoldenGemstones,
    amount,
    connectionIn: connection
);

if (update == null)
{
    throw new ResponseErrorException(ErrorCode.ERROR_CODE_SHOP_LACK_MONEY);
}
```

Wallet types are defined in `/Arrowgene.Ddon.Shared/Model/WalletType.cs`:
- `WalletType.Gold` = 1
- `WalletType.RiftPoints` = 2
- `WalletType.BloodOrbs` = 3
- `WalletType.GoldenGemstones` = 11
- etc.

---

## 9. Item Operations

**Location:** `/Arrowgene.Ddon.GameServer/Characters/ItemManager.cs`

```csharp
// Adding items
List<CDataItemUpdateResult> results = Server.ItemManager.AddItem(
    Server,
    client.Character,
    sendToItemBag: true,
    itemId,
    amount,
    connectionIn: connection
);

// Check if can add items
bool canAdd = Server.ItemManager.CanAddItem(client.Character, StorageType.ItemBagConsumable, itemId, amount);

// Consuming items
CDataItemUpdateResult result = Server.ItemManager.ConsumeItemByUId(
    Server,
    client.Character,
    StorageType.ItemBagConsumable,
    itemUID,
    amount
);
```

---

## 10. Client Notification Pattern

Send notifications to client for item/wallet updates:

```csharp
S2CItemUpdateCharacterItemNtc itemNtc = new S2CItemUpdateCharacterItemNtc()
{
    UpdateType = ItemNoticeType.ShopGoods_buy
};

itemNtc.UpdateItemList.AddRange(itemUpdateResults);
itemNtc.UpdateWalletList.Add(walletUpdateResult);

client.Send(itemNtc);
```

---

## Summary: Files to Create for GP Shop

1. **Packet Structures:**
   - `C2SGpGpShopGetItemLineupReq.cs`
   - `S2CGpGpShopGetItemLineupRes.cs`
   - `C2SGpGpShopBuyReq.cs`
   - `S2CGpGpShopBuyRes.cs`

2. **Data Structures:**
   - `CDataGpShopItem.cs`
   - `CDataGpShopCategory.cs`

3. **Handlers:**
   - `GpGpShopGetItemLineupHandler.cs`
   - `GpGpShopBuyHandler.cs`

4. **Manager:**
   - `GpShopManager.cs`

5. **Assets:**
   - `GpShopAsset.cs`
   - `GpShopAssetDeserializer.cs`
   - `GpShop.json`

6. **Modifications:**
   - `EntitySerializer.cs` - Register serializers
   - `AssetRepository.cs` - Register asset
   - `DdonGameServer.cs` - Register manager and handlers
