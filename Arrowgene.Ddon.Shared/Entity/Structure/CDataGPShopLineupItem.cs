using Arrowgene.Buffers;

namespace Arrowgene.Ddon.Shared.Entity.Structure
{
    public class CDataGPShopLineupItem
    {
        public uint LineupId { get; set; }
        public uint ItemId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public uint Price { get; set; }
        public uint BasePrice { get; set; }
        public uint ItemNum { get; set; }
        public uint PurchaseLimit { get; set; }
        public uint PurchaseCount { get; set; }
        public long BeginTime { get; set; }
        public long EndTime { get; set; }
        public uint CategoryId { get; set; }
        public uint IconId { get; set; }
        public byte SortOrder { get; set; }

        // Course ticket fields (for purchasable courses)
        public uint CourseId { get; set; }         // 0 = regular item, >0 = course ticket
        public uint DurationSeconds { get; set; }  // Duration in seconds when activated
        public uint BackIconId { get; set; }
        public uint FrameIconId { get; set; }

        public CDataGPShopLineupItem()
        {
            Name = string.Empty;
            Description = string.Empty;
        }

        public class Serializer : EntitySerializer<CDataGPShopLineupItem>
        {
            public override void Write(IBuffer buffer, CDataGPShopLineupItem obj)
            {
                WriteUInt32(buffer, obj.LineupId);
                WriteUInt32(buffer, obj.ItemId);
                WriteMtString(buffer, obj.Name);
                WriteMtString(buffer, obj.Description);
                WriteUInt32(buffer, obj.Price);
                WriteUInt32(buffer, obj.BasePrice);
                WriteUInt32(buffer, obj.ItemNum);
                WriteUInt32(buffer, obj.PurchaseLimit);
                WriteUInt32(buffer, obj.PurchaseCount);
                WriteInt64(buffer, obj.BeginTime);
                WriteInt64(buffer, obj.EndTime);
                WriteUInt32(buffer, obj.CategoryId);
                WriteUInt32(buffer, obj.IconId);
                WriteByte(buffer, obj.SortOrder);
            }

            public override CDataGPShopLineupItem Read(IBuffer buffer)
            {
                CDataGPShopLineupItem obj = new CDataGPShopLineupItem
                {
                    LineupId = ReadUInt32(buffer),
                    ItemId = ReadUInt32(buffer),
                    Name = ReadMtString(buffer),
                    Description = ReadMtString(buffer),
                    Price = ReadUInt32(buffer),
                    BasePrice = ReadUInt32(buffer),
                    ItemNum = ReadUInt32(buffer),
                    PurchaseLimit = ReadUInt32(buffer),
                    PurchaseCount = ReadUInt32(buffer),
                    BeginTime = ReadInt64(buffer),
                    EndTime = ReadInt64(buffer),
                    CategoryId = ReadUInt32(buffer),
                    IconId = ReadUInt32(buffer),
                    SortOrder = ReadByte(buffer)
                };
                return obj;
            }
        }
    }
}
