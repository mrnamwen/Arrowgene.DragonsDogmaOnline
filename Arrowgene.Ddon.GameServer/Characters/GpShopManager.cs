using System;
using System.Collections.Generic;
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
            uint totalItems = item.ItemNum * quantity;

            // Deduct GP
            var walletUpdate = _server.WalletManager.RemoveFromWallet(client.Character, WalletType.GoldenGemstones, totalPrice);
            if (walletUpdate == null)
            {
                Logger.Error($"Failed to deduct {totalPrice} GP from player {client.Character.CharacterId}.");
                return false;
            }
            ntc.UpdateWalletList.Add(walletUpdate);

            // Add items to inventory
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

            // Record purchase history
            RecordPurchaseHistory(client.Character.CharacterId, lineupId, item.Name, totalPrice);

            Logger.Info($"Player {client.Character.CharacterId} purchased {quantity}x '{item.Name}' (LineupId: {lineupId}) for {totalPrice} GP.");

            return true;
        }

        private uint GetCharacterPurchaseCount(uint characterId, uint lineupId)
        {
            // TODO: Implement database query for purchase count tracking
            // For now, return 0 (no limit enforcement until DB is implemented)
            return 0;
        }

        private void IncrementCharacterPurchaseCount(uint characterId, uint lineupId, uint amount, object connection)
        {
            // TODO: Implement database update for purchase count tracking
        }

        private void RecordPurchaseHistory(uint characterId, uint lineupId, string itemName, uint price)
        {
            // TODO: Implement purchase history recording in database
            // This would use the gp_shop_purchase_history table
        }
    }
}
