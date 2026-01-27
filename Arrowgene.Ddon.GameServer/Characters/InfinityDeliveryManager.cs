using Arrowgene.Ddon.Server;
using Arrowgene.Ddon.Server.Network;
using Arrowgene.Ddon.Shared.Asset;
using Arrowgene.Ddon.Shared.Entity.PacketStructure;
using Arrowgene.Ddon.Shared.Entity.Structure;
using Arrowgene.Ddon.Shared.Model;
using Arrowgene.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace Arrowgene.Ddon.GameServer.Characters
{
    public class InfinityDeliveryManager
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(InfinityDeliveryManager));

        private readonly DdonGameServer Server;

        public InfinityDeliveryManager(DdonGameServer server)
        {
            Server = server;
        }

        public bool IsEventEnabled()
        {
            return Server.AssetRepository.InfinityDeliveryAsset?.Enabled ?? false;
        }

        public uint GetEventId()
        {
            return Server.AssetRepository.InfinityDeliveryAsset?.EventId ?? 1;
        }

        public List<CDataInfinityDeliveryCategory> GetCategories()
        {
            var asset = Server.AssetRepository.InfinityDeliveryAsset;
            if (asset == null || asset.Categories == null)
            {
                return new List<CDataInfinityDeliveryCategory>();
            }

            return asset.Categories.Select(c => new CDataInfinityDeliveryCategory
            {
                CategoryId = c.CategoryId,
                Name = c.Name,
                Items = c.Items.Select(i => new CDataInfinityDeliveryItem
                {
                    ItemId = i.ItemId,
                    PointValue = i.PointValue
                }).ToList()
            }).ToList();
        }

        public List<CDataInfinityDeliveryBorder> GetBorders()
        {
            var asset = Server.AssetRepository.InfinityDeliveryAsset;
            if (asset == null || asset.Borders == null)
            {
                return new List<CDataInfinityDeliveryBorder>();
            }

            return asset.Borders.Select(b => new CDataInfinityDeliveryBorder
            {
                BorderId = b.BorderId,
                RequiredPoints = b.RequiredPoints,
                Rewards = b.Rewards.Select(r => new CDataRewardItem
                {
                    ItemId = (ItemId)r.ItemId,
                    Num = (ushort)r.Num
                }).ToList()
            }).ToList();
        }

        public CDataInfinityDeliveryStatus GetPlayerStatus(GameClient client, DbConnection? connectionIn = null)
        {
            uint characterId = client.Character.CharacterId;
            var status = new CDataInfinityDeliveryStatus();

            var periodStart = GetCurrentPeriodStart();

            var progressList = Server.Database.SelectInfinityDeliveryProgress(characterId, connectionIn);
            uint totalPoints = 0;
            foreach (var progress in progressList)
            {
                if (progress.PeriodStart >= periodStart)
                {
                    totalPoints += progress.TotalPoints;
                }
            }
            status.CurrentPoints = totalPoints;

            var claims = Server.Database.SelectInfinityDeliveryBorderClaimsByPeriod(characterId, periodStart, connectionIn);
            foreach (var claim in claims)
            {
                status.ReceivedBorderIds.Add(new CDataCommonU32 { Value = claim.BorderId });
            }

            return status;
        }

        public PacketQueue DeliverItems(GameClient client, uint categoryId, List<CDataItemUIDList> items, DbConnection? connectionIn = null)
        {
            PacketQueue queue = new();
            uint characterId = client.Character.CharacterId;

            var asset = Server.AssetRepository.InfinityDeliveryAsset;
            if (asset == null || !asset.Enabled)
            {
                Logger.Error("Infinity Delivery is not enabled");
                return queue;
            }

            var category = asset.Categories.FirstOrDefault(c => c.CategoryId == categoryId);
            if (category == null)
            {
                Logger.Error($"Category {categoryId} not found");
                return queue;
            }

            uint pointsEarned = 0;
            uint itemsDelivered = 0;
            S2CItemUpdateCharacterItemNtc itemNtc = new();

            Server.Database.ExecuteQuerySafe(connectionIn, connection =>
            {
                foreach (var deliverItem in items)
                {
                    var storageItem = client.Character.Storage.FindItemByUIdInStorage(ItemManager.BothStorageTypes, deliverItem.ItemUID);
                    if (storageItem == null)
                    {
                        Logger.Info($"Item {deliverItem.ItemUID} not found in storage");
                        continue;
                    }

                    var categoryItem = category.Items.FirstOrDefault(i => i.ItemId == storageItem.Item2.Item2.ItemId);
                    if (categoryItem == null)
                    {
                        Logger.Info($"Item {storageItem.Item2.Item2.ItemId} not valid for category {categoryId}");
                        continue;
                    }

                    uint quantityToDeliver = deliverItem.Num;
                    if (quantityToDeliver > storageItem.Item2.Item3)
                    {
                        quantityToDeliver = storageItem.Item2.Item3;
                    }

                    var consumeResult = Server.ItemManager.ConsumeItemByUId(Server, client.Character, storageItem.Item1, deliverItem.ItemUID, quantityToDeliver, connection);
                    if (consumeResult != null)
                    {
                        itemNtc.UpdateItemList.Add(consumeResult);
                        pointsEarned += categoryItem.PointValue * quantityToDeliver;
                        itemsDelivered += quantityToDeliver;
                    }
                }

                if (pointsEarned > 0)
                {
                    var periodStart = GetCurrentPeriodStart();
                    var existingProgress = Server.Database.SelectInfinityDeliveryProgressByCategory(characterId, categoryId, connection);

                    uint newTotalPoints = pointsEarned;
                    uint newItemsDelivered = itemsDelivered;

                    if (existingProgress.HasValue && existingProgress.Value.PeriodStart >= periodStart)
                    {
                        newTotalPoints += existingProgress.Value.TotalPoints;
                        newItemsDelivered += existingProgress.Value.ItemsDelivered;
                    }

                    Server.Database.UpsertInfinityDeliveryProgress(characterId, categoryId, newTotalPoints, newItemsDelivered, periodStart, connection);
                }
            });

            if (itemNtc.UpdateItemList.Any())
            {
                client.Enqueue(itemNtc, queue);
            }

            return queue;
        }

        public uint CalculatePointsEarned(GameClient client, uint categoryId, List<CDataItemUIDList> items)
        {
            var asset = Server.AssetRepository.InfinityDeliveryAsset;
            if (asset == null)
            {
                return 0;
            }

            var category = asset.Categories.FirstOrDefault(c => c.CategoryId == categoryId);
            if (category == null)
            {
                return 0;
            }

            uint pointsEarned = 0;
            foreach (var deliverItem in items)
            {
                var storageItem = client.Character.Storage.FindItemByUIdInStorage(ItemManager.BothStorageTypes, deliverItem.ItemUID);
                if (storageItem == null)
                {
                    continue;
                }

                var categoryItem = category.Items.FirstOrDefault(i => i.ItemId == storageItem.Item2.Item2.ItemId);
                if (categoryItem == null)
                {
                    continue;
                }

                uint quantityToDeliver = Math.Min(deliverItem.Num, storageItem.Item2.Item3);
                pointsEarned += categoryItem.PointValue * quantityToDeliver;
            }

            return pointsEarned;
        }

        public PacketQueue ClaimBorderReward(GameClient client, uint borderId, DbConnection? connectionIn = null)
        {
            PacketQueue queue = new();
            uint characterId = client.Character.CharacterId;

            var asset = Server.AssetRepository.InfinityDeliveryAsset;
            if (asset == null || !asset.Enabled)
            {
                Logger.Error("Infinity Delivery is not enabled");
                return queue;
            }

            var border = asset.Borders.FirstOrDefault(b => b.BorderId == borderId);
            if (border == null)
            {
                Logger.Error($"Border {borderId} not found");
                return queue;
            }

            Server.Database.ExecuteQuerySafe(connectionIn, connection =>
            {
                var periodStart = GetCurrentPeriodStart();
                var status = GetPlayerStatus(client, connection);

                if (status.CurrentPoints < border.RequiredPoints)
                {
                    Logger.Error($"Not enough points for border {borderId}: {status.CurrentPoints} < {border.RequiredPoints}");
                    return;
                }

                if (status.ReceivedBorderIds.Any(r => r.Value == borderId))
                {
                    Logger.Error($"Border {borderId} already claimed");
                    return;
                }

                Server.Database.InsertInfinityDeliveryBorderClaim(characterId, borderId, DateTime.UtcNow, periodStart, connection);

                S2CItemUpdateCharacterItemNtc itemNtc = new();

                foreach (var reward in border.Rewards)
                {
                    if (reward.WalletType != WalletType.None && reward.WalletAmount > 0)
                    {
                        itemNtc.UpdateWalletList.Add(Server.WalletManager.AddToWallet(client.Character, reward.WalletType, reward.WalletAmount));
                    }

                    if (reward.ItemId > 0 && reward.Num > 0)
                    {
                        itemNtc.UpdateItemList.AddRange(Server.ItemManager.AddItem(Server, client.Character, StorageType.ItemPost, reward.ItemId, reward.Num, 0, connection));
                    }
                }

                if (itemNtc.UpdateWalletList.Any() || itemNtc.UpdateItemList.Any())
                {
                    client.Enqueue(itemNtc, queue);
                }
            });

            return queue;
        }

        public bool CheckWeeklyReset(GameClient client, DbConnection? connectionIn = null)
        {
            uint characterId = client.Character.CharacterId;
            var periodStart = GetCurrentPeriodStart();

            var progressList = Server.Database.SelectInfinityDeliveryProgress(characterId, connectionIn);
            bool needsReset = progressList.Any(p => p.PeriodStart < periodStart);

            if (needsReset)
            {
                Server.Database.ExecuteQuerySafe(connectionIn, connection =>
                {
                    Server.Database.DeleteInfinityDeliveryProgress(characterId, connection);
                    Server.Database.DeleteInfinityDeliveryBorderClaims(characterId, connection);
                });

                Logger.Info($"Reset infinity delivery progress for character {characterId}");
                return true;
            }

            return false;
        }

        public (ulong StartTime, ulong EndTime) GetEventTimes()
        {
            var periodStart = GetCurrentPeriodStart();
            var periodEnd = GetNextPeriodStart();

            return (
                (ulong)new DateTimeOffset(periodStart).ToUnixTimeSeconds(),
                (ulong)new DateTimeOffset(periodEnd).ToUnixTimeSeconds()
            );
        }

        private DateTime GetCurrentPeriodStart()
        {
            var asset = Server.AssetRepository.InfinityDeliveryAsset;
            var resetDay = asset?.ResetDay ?? DayOfWeek.Monday;
            var resetHour = asset?.ResetHour ?? 5;

            TimeZoneInfo jstZone = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
            DateTime jstNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, jstZone);

            int daysBack = ((int)jstNow.DayOfWeek - (int)resetDay + 7) % 7;
            DateTime lastResetDay = jstNow.Date.AddDays(-daysBack).AddHours(resetHour);

            if (jstNow < lastResetDay)
            {
                lastResetDay = lastResetDay.AddDays(-7);
            }

            return TimeZoneInfo.ConvertTimeToUtc(lastResetDay, jstZone);
        }

        private DateTime GetNextPeriodStart()
        {
            var asset = Server.AssetRepository.InfinityDeliveryAsset;
            var resetDay = asset?.ResetDay ?? DayOfWeek.Monday;
            var resetHour = asset?.ResetHour ?? 5;

            TimeZoneInfo jstZone = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
            DateTime jstNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, jstZone);

            int daysForward = ((int)resetDay - (int)jstNow.DayOfWeek + 7) % 7;
            if (daysForward == 0 && jstNow.Hour >= resetHour)
            {
                daysForward = 7;
            }

            DateTime nextResetDay = jstNow.Date.AddDays(daysForward).AddHours(resetHour);

            return TimeZoneInfo.ConvertTimeToUtc(nextResetDay, jstZone);
        }
    }
}
