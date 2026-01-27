using System;
using System.Collections.Generic;
using Arrowgene.Ddon.Server;
using Arrowgene.Ddon.Shared.Entity.PacketStructure;
using Arrowgene.Ddon.Shared.Entity.Structure;
using Arrowgene.Ddon.Shared.Model;
using Arrowgene.Logging;

namespace Arrowgene.Ddon.GameServer.Characters
{
    public class NpcBazaarManager
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(NpcBazaarManager));

        /// <summary>
        /// Reserved character ID for NPC Bazaar listings.
        /// </summary>
        public const uint NpcCharacterId = 0;

        /// <summary>
        /// Offset added to item IDs to create unique NPC bazaar_id values.
        /// This ensures NPC bazaar IDs don't conflict with database-generated player bazaar IDs.
        /// </summary>
        public const ulong NpcBazaarIdOffset = 10_000_000_000;

        private readonly DdonGameServer Server;

        public NpcBazaarManager(DdonGameServer server)
        {
            Server = server;
        }

        /// <summary>
        /// Checks if a bazaar_id belongs to an NPC listing.
        /// </summary>
        public bool IsNpcBazaarId(ulong bazaarId)
        {
            return bazaarId >= NpcBazaarIdOffset;
        }

        /// <summary>
        /// Extracts the item ID from an NPC bazaar_id.
        /// </summary>
        public uint GetItemIdFromNpcBazaarId(ulong bazaarId)
        {
            if (!IsNpcBazaarId(bazaarId))
            {
                throw new ArgumentException($"BazaarId {bazaarId} is not an NPC bazaar ID.");
            }
            return (uint)(bazaarId - NpcBazaarIdOffset);
        }

        /// <summary>
        /// Creates an NPC bazaar_id from an item ID.
        /// </summary>
        public ulong CreateNpcBazaarId(uint itemId)
        {
            return NpcBazaarIdOffset + itemId;
        }

        /// <summary>
        /// Checks if an item is eligible for NPC Bazaar trading.
        /// </summary>
        public bool IsItemEligible(uint itemId)
        {
            if (!Server.AssetRepository.ClientItemInfos.ContainsKey(itemId))
            {
                return false;
            }

            var itemInfo = Server.AssetRepository.ClientItemInfos[itemId];

            // Check if item has a price
            if (itemInfo.Price == 0)
            {
                return false;
            }

            // Check if category is excluded
            var npcBazaarAsset = Server.AssetRepository.NpcBazaarAsset;
            if (npcBazaarAsset.ExcludedCategories.Contains(itemInfo.Category))
            {
                return false;
            }

            // Check if specific item is excluded
            if (npcBazaarAsset.ExcludedItemIds.Contains(itemId))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Calculates the price a player pays to buy an item from the NPC Bazaar.
        /// </summary>
        public uint CalculateNpcBuyPrice(uint itemId)
        {
            if (!Server.AssetRepository.ClientItemInfos.ContainsKey(itemId))
            {
                return 0;
            }

            var itemInfo = Server.AssetRepository.ClientItemInfos[itemId];
            var npcBazaarAsset = Server.AssetRepository.NpcBazaarAsset;

            double multiplier = Server.GameSettings.GameServerSettings.NpcBazaarBuyMultiplier;

            // Check for item-specific override
            if (npcBazaarAsset.ItemOverrides.TryGetValue(itemId, out var itemOverride) && itemOverride.BuyMultiplier.HasValue)
            {
                multiplier = itemOverride.BuyMultiplier.Value;
            }

            return (uint)Math.Max(1, itemInfo.Price * multiplier);
        }

        /// <summary>
        /// Calculates the price the NPC pays a player when selling an item.
        /// </summary>
        public uint CalculateNpcSellPrice(uint itemId)
        {
            if (!Server.AssetRepository.ClientItemInfos.ContainsKey(itemId))
            {
                return 0;
            }

            var itemInfo = Server.AssetRepository.ClientItemInfos[itemId];
            var npcBazaarAsset = Server.AssetRepository.NpcBazaarAsset;

            double multiplier = Server.GameSettings.GameServerSettings.NpcBazaarSellMultiplier;

            // Check for item-specific override
            if (npcBazaarAsset.ItemOverrides.TryGetValue(itemId, out var itemOverride) && itemOverride.SellMultiplier.HasValue)
            {
                multiplier = itemOverride.SellMultiplier.Value;
            }

            return (uint)Math.Max(1, itemInfo.Price * multiplier);
        }

        /// <summary>
        /// Generates an NPC exhibition for a specific item.
        /// Returns null if the item is not eligible or NPC Bazaar is disabled.
        /// </summary>
        public BazaarExhibition GetNpcExhibitionForItem(uint itemId)
        {
            if (!Server.GameSettings.GameServerSettings.EnableNpcBazaar)
            {
                return null;
            }

            if (!IsItemEligible(itemId))
            {
                return null;
            }

            uint price = CalculateNpcBuyPrice(itemId);
            ushort quantity = (ushort)Server.GameSettings.GameServerSettings.NpcBazaarItemQuantity;

            var exhibition = new BazaarExhibition
            {
                CharacterId = NpcCharacterId,
                Info = new CDataBazaarCharacterInfo
                {
                    ItemInfo = new CDataBazaarItemInfo
                    {
                        BazaarId = CreateNpcBazaarId(itemId),
                        Sequence = 0,
                        ItemBaseInfo = new CDataBazaarItemBaseInfo
                        {
                            ItemId = itemId,
                            Num = quantity,
                            Price = price
                        },
                        ExhibitionTime = DateTimeOffset.UtcNow
                    },
                    State = BazaarExhibitionState.OnSale,
                    Proceeds = 0, // NPC doesn't need proceeds
                    Expire = DateTimeOffset.MaxValue // NPC listings never expire
                }
            };

            return exhibition;
        }

        /// <summary>
        /// Processes a purchase from the NPC Bazaar.
        /// </summary>
        public void ProcessNpcPurchase(GameClient client, ulong bazaarId, List<CDataItemStorageIndicateNum> itemStorageIndicateNumList)
        {
            if (!IsNpcBazaarId(bazaarId))
            {
                throw new ResponseErrorException(ErrorCode.ERROR_CODE_BAZAAR_INTERNAL_ERROR, "Not an NPC bazaar ID.");
            }

            uint itemId = GetItemIdFromNpcBazaarId(bazaarId);

            if (!IsItemEligible(itemId))
            {
                throw new ResponseErrorException(ErrorCode.ERROR_CODE_BAZAAR_INTERNAL_ERROR, "Item not eligible for NPC Bazaar.");
            }

            uint pricePerItem = CalculateNpcBuyPrice(itemId);
            uint totalItemAmount = 0;
            foreach (var storage in itemStorageIndicateNumList)
            {
                totalItemAmount += storage.ItemNum;
            }
            uint totalPrice = pricePerItem * totalItemAmount;

            S2CItemUpdateCharacterItemNtc updateCharacterItemNtc = new S2CItemUpdateCharacterItemNtc();
            updateCharacterItemNtc.UpdateType = 0;

            // Add items to inventory
            foreach (CDataItemStorageIndicateNum itemStorageIndicateNum in itemStorageIndicateNumList)
            {
                bool sendToItemBag;
                switch (itemStorageIndicateNum.StorageType)
                {
                    case 19:
                        sendToItemBag = true;
                        break;
                    case 20:
                        sendToItemBag = false;
                        break;
                    default:
                        throw new ResponseErrorException(ErrorCode.ERROR_CODE_BAZAAR_INTERNAL_ERROR, $"Unexpected destination when buying goods: {itemStorageIndicateNum.StorageType}");
                }
                List<CDataItemUpdateResult> itemUpdateResult = Server.ItemManager.AddItem(Server, client.Character, sendToItemBag, itemId, itemStorageIndicateNum.ItemNum);
                updateCharacterItemNtc.UpdateItemList.AddRange(itemUpdateResult);
            }

            // Deduct gold
            CDataUpdateWalletPoint updateWalletPoint = Server.WalletManager.RemoveFromWallet(client.Character, WalletType.Gold, totalPrice);
            if (updateWalletPoint is null)
            {
                throw new ResponseErrorException(ErrorCode.ERROR_CODE_BAZAAR_INTERNAL_ERROR, "Insufficient funds.");
            }

            updateCharacterItemNtc.UpdateWalletList.Add(updateWalletPoint);

            // No notification to seller for NPC purchases
            // No database update needed for NPC purchases

            client.Send(updateCharacterItemNtc);

            Logger.Info($"Player {client.Character.CharacterId} purchased {totalItemAmount}x item {itemId} from NPC Bazaar for {totalPrice} gold.");
        }

        /// <summary>
        /// Processes a sale to the NPC Bazaar.
        /// </summary>
        public S2CItemUpdateCharacterItemNtc ProcessNpcSale(GameClient client, StorageType storageType, string itemUID, ushort quantity)
        {
            // Find the item first to get its ItemId and validate
            var foundItem = client.Character.Storage.GetStorage(storageType).FindItemByUId(itemUID);
            if (foundItem == null)
            {
                throw new ResponseErrorException(ErrorCode.ERROR_CODE_ITEM_NOT_FOUND, $"Item {itemUID} not found in storage.");
            }

            (ushort slotNo, Item item, uint itemNum) = foundItem;
            uint itemId = item.ItemId;

            if (!IsItemEligible(itemId))
            {
                throw new ResponseErrorException(ErrorCode.ERROR_CODE_BAZAAR_INTERNAL_ERROR, "Item not eligible for NPC Bazaar sale.");
            }

            if (itemNum < quantity)
            {
                throw new ResponseErrorException(ErrorCode.ERROR_CODE_ITEM_NUM_SHORT, "Not enough items to sell.");
            }

            uint pricePerItem = CalculateNpcSellPrice(itemId);
            uint totalPrice = pricePerItem * quantity;

            S2CItemUpdateCharacterItemNtc updateCharacterItemNtc = new S2CItemUpdateCharacterItemNtc();
            updateCharacterItemNtc.UpdateType = 0;

            // Remove item from inventory
            CDataItemUpdateResult itemUpdateResult = Server.ItemManager.ConsumeItemByUId(Server, client.Character, storageType, itemUID, quantity);
            if (itemUpdateResult == null)
            {
                throw new ResponseErrorException(ErrorCode.ERROR_CODE_ITEM_NOT_FOUND, $"Failed to consume item {itemUID}.");
            }
            updateCharacterItemNtc.UpdateItemList.Add(itemUpdateResult);

            // Add gold
            CDataUpdateWalletPoint updateWalletPoint = Server.WalletManager.AddToWallet(client.Character, WalletType.Gold, totalPrice);
            updateCharacterItemNtc.UpdateWalletList.Add(updateWalletPoint);

            Logger.Info($"Player {client.Character.CharacterId} sold {quantity}x item {itemId} to NPC Bazaar for {totalPrice} gold.");

            return updateCharacterItemNtc;
        }
    }
}
