using System;
using System.Collections.Generic;
using System.Data.Common;
using Arrowgene.Ddon.Database.Model;
using Arrowgene.Ddon.Server;
using Arrowgene.Ddon.Shared.Asset;
using Arrowgene.Ddon.Shared.Entity.PacketStructure;
using Arrowgene.Ddon.Shared.Entity.Structure;
using Arrowgene.Ddon.Shared.Model;
using Arrowgene.Logging;

namespace Arrowgene.Ddon.GameServer.Characters
{
    public class GpShopManager
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(GpShopManager));

        // Storage expansion items: ItemId -> (StorageType, ExpansionAmount)
        private static readonly Dictionary<uint, (StorageType StorageType, ushort ExpansionAmount)> StorageExpansionItems = new()
        {
            { (uint)ItemId.ItemBagConsumableExpansion, (StorageType.ItemBagConsumable, 20) },
            { (uint)ItemId.ItemBagMaterialExpansion, (StorageType.ItemBagMaterial, 40) },
            { (uint)ItemId.ItemBagEquipmentExpansion, (StorageType.ItemBagEquipment, 40) },
            { (uint)ItemId.ItemBagJobExpansion, (StorageType.ItemBagJob, 20) },
        };

        private readonly DdonGameServer _server;

        public GpShopManager(DdonGameServer server)
        {
            _server = server;
        }

        public GpShopAsset GpShopAsset => _server.AssetRepository.GpShopAsset;

        public List<CDataGPShopLineupItem> GetItemsForCategory(uint categoryId)
        {
            return GpShopAsset.GetItemsForCategory(categoryId);
        }

        public List<CDataGPShopDisplayLineup> GetDisplayLineupForCategory(uint categoryId)
        {
            var items = GpShopAsset.GetItemsForCategory(categoryId);
            var result = new List<CDataGPShopDisplayLineup>();
            foreach (var item in items)
            {
                result.Add(new CDataGPShopDisplayLineup
                {
                    ID = item.LineupId,
                    Category = categoryId,
                    IconId = (byte)item.IconId,
                    GP = item.Price,
                    DiscountType = 0,
                    DiscountGP = 0,
                    Name = item.Name,
                    Comment = item.Description,
                    LineupID = item.LineupId,
                    ImageURL = string.Empty,
                    BackIconID = 0,
                    FrameIconID = 0,
                    BehaviorAfterBuyingType = 0,
                    Unk0 = 0
                });
            }
            return result;
        }

        public CDataGPShopLineupItem GetItem(uint lineupId)
        {
            return GpShopAsset.GetItem(lineupId);
        }

        public bool CanBuyItem(Character character, uint lineupId, uint quantity)
        {
            var item = GetItem(lineupId);
            if (item == null)
            {
                return false;
            }

            // Check if item is available (time restrictions)
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            if (item.BeginTime > 0 && now < item.BeginTime)
            {
                return false;
            }
            if (item.EndTime > 0 && now > item.EndTime)
            {
                return false;
            }

            // Check purchase limit
            if (item.PurchaseLimit > 0)
            {
                uint purchaseCount = GetCharacterPurchaseCount(character.CharacterId, lineupId);
                if (purchaseCount + quantity > item.PurchaseLimit)
                {
                    return false;
                }
            }

            // Check if player has enough GP
            uint totalPrice = item.Price * quantity;
            uint gpBalance = _server.WalletManager.GetWalletAmount(character, WalletType.GoldenGemstones);
            if (gpBalance < totalPrice)
            {
                return false;
            }

            return true;
        }

        public bool ProcessPurchase(GameClient client, uint lineupId, uint quantity, out S2CItemUpdateCharacterItemNtc itemNtc)
        {
            var ntc = new S2CItemUpdateCharacterItemNtc { UpdateType = 0 };
            itemNtc = ntc;

            var item = GetItem(lineupId);
            if (item == null)
            {
                Logger.Error($"Item with LineupId {lineupId} not found.");
                return false;
            }

            if (!CanBuyItem(client.Character, lineupId, quantity))
            {
                Logger.Info($"Player {client.Character.CharacterId} cannot buy {quantity}x item {lineupId}.");
                return false;
            }

            uint totalPrice = item.Price * quantity;

            // Deduct GP
            var walletUpdate = _server.WalletManager.RemoveFromWallet(client.Character, WalletType.GoldenGemstones, totalPrice);
            if (walletUpdate == null)
            {
                Logger.Error($"Failed to deduct {totalPrice} GP from player {client.Character.CharacterId}.");
                return false;
            }
            ntc.UpdateWalletList.Add(walletUpdate);

            // Check if this is a course ticket purchase
            if (item.CourseId > 0)
            {
                // Course ticket purchase - add to available courses instead of inventory
                _server.Database.ExecuteInTransaction(connection =>
                {
                    for (uint i = 0; i < quantity; i++)
                    {
                        var availableCourse = new CharacterAvailableCourse
                        {
                            CharacterId = client.Character.CharacterId,
                            CourseId = item.CourseId,
                            CourseName = item.Name,
                            DurationSec = item.DurationSeconds,
                            LineupId = item.LineupId,
                            BackIconId = item.BackIconId,
                            FrameIconId = item.FrameIconId,
                            PurchaseTime = DateTime.UtcNow
                        };
                        _server.Database.InsertCharacterAvailableCourse(availableCourse, connection);
                    }

                    // Track purchase for purchase limit
                    if (item.PurchaseLimit > 0)
                    {
                        IncrementCharacterPurchaseCount(client.Character.CharacterId, lineupId, quantity, connection);
                    }
                });

                Logger.Info($"Player {client.Character.CharacterId} purchased {quantity}x course ticket '{item.Name}' (CourseId: {item.CourseId}, LineupId: {lineupId}) for {totalPrice} GP.");
            }
            else if (IsStorageExpansionItem(item.ItemId))
            {
                // Storage expansion item purchase - expand the character's storage
                _server.Database.ExecuteInTransaction(connection =>
                {
                    for (uint i = 0; i < quantity; i++)
                    {
                        if (ProcessStorageExpansion(client, item.ItemId, connection, out var extendNtc))
                        {
                            // Send storage slot extension notification to client
                            client.Send(extendNtc);
                        }
                    }

                    // Track purchase for purchase limit
                    if (item.PurchaseLimit > 0)
                    {
                        IncrementCharacterPurchaseCount(client.Character.CharacterId, lineupId, quantity, connection);
                    }
                });

                Logger.Info($"Player {client.Character.CharacterId} purchased storage expansion '{item.Name}' (LineupId: {lineupId}) for {totalPrice} GP.");
            }
            else
            {
                // Regular item purchase - add to inventory
                uint totalItems = item.ItemNum * quantity;

                _server.Database.ExecuteInTransaction(connection =>
                {
                    if (_server.ItemManager.IsItemWalletPoint(item.ItemId))
                    {
                        (WalletType walletType, uint amount) = _server.ItemManager.ItemToWalletPoint(item.ItemId);
                        var result = _server.WalletManager.AddToWallet(client.Character, walletType, amount * totalItems, connectionIn: connection);
                        ntc.UpdateWalletList.Add(result);
                    }
                    else
                    {
                        var itemUpdates = _server.ItemManager.AddItem(_server, client.Character, true, item.ItemId, totalItems, connectionIn: connection);
                        ntc.UpdateItemList.AddRange(itemUpdates);
                    }

                    // Track purchase for purchase limit
                    if (item.PurchaseLimit > 0)
                    {
                        IncrementCharacterPurchaseCount(client.Character.CharacterId, lineupId, quantity, connection);
                    }
                });

                Logger.Info($"Player {client.Character.CharacterId} purchased {quantity}x '{item.Name}' (LineupId: {lineupId}) for {totalPrice} GP.");
            }

            // Record purchase history
            RecordPurchaseHistory(client.Character.CharacterId, lineupId, item.Name, totalPrice);

            return true;
        }

        private uint GetCharacterPurchaseCount(uint characterId, uint lineupId)
        {
            return _server.Database.SelectGpShopPurchaseCount(characterId, lineupId);
        }

        private void IncrementCharacterPurchaseCount(uint characterId, uint lineupId, uint amount, DbConnection connection)
        {
            uint currentCount = _server.Database.SelectGpShopPurchaseCount(characterId, lineupId, connection);
            _server.Database.UpsertGpShopPurchaseCount(characterId, lineupId, currentCount + amount, connection);
        }

        private void RecordPurchaseHistory(uint characterId, uint lineupId, string itemName, uint price)
        {
            // TODO: Implement purchase history recording in database
            // This would use the gp_shop_purchase_history table
        }

        /// <summary>
        /// Checks if the given item ID is a storage expansion item.
        /// </summary>
        public static bool IsStorageExpansionItem(uint itemId)
        {
            return StorageExpansionItems.ContainsKey(itemId);
        }

        /// <summary>
        /// Expands a character's storage by the specified amount.
        /// </summary>
        /// <param name="client">The game client</param>
        /// <param name="storageType">The storage type to expand</param>
        /// <param name="expansionAmount">Number of slots to add</param>
        /// <param name="connection">Database connection for transaction</param>
        /// <returns>The slot extension notification packet</returns>
        public S2CItemExtendItemSlotNtc ExpandStorage(GameClient client, StorageType storageType, ushort expansionAmount, DbConnection connection)
        {
            var storage = client.Character.Storage.GetStorage(storageType);
            ushort currentMax = storage.MaxSlots();
            ushort newMax = (ushort)(currentMax + expansionAmount);

            // Expand the in-memory storage by adding null slots
            for (int i = 0; i < expansionAmount; i++)
            {
                storage.Items.Add(null);
            }

            // Persist the new storage size to database
            _server.Database.UpdateStorage(client.Character.ContentCharacterId, storageType, storage, connection);

            Logger.Info($"Player {client.Character.CharacterId} expanded {storageType} from {currentMax} to {newMax} slots.");

            return new S2CItemExtendItemSlotNtc
            {
                Category = (byte)storageType,
                AddNum = expansionAmount,
                TotalNum = newMax
            };
        }

        /// <summary>
        /// Processes a storage expansion item purchase.
        /// </summary>
        public bool ProcessStorageExpansion(GameClient client, uint itemId, DbConnection connection, out S2CItemExtendItemSlotNtc extendNtc)
        {
            extendNtc = null;

            if (!StorageExpansionItems.TryGetValue(itemId, out var expansionInfo))
            {
                return false;
            }

            extendNtc = ExpandStorage(client, expansionInfo.StorageType, expansionInfo.ExpansionAmount, connection);
            return true;
        }
    }
}
