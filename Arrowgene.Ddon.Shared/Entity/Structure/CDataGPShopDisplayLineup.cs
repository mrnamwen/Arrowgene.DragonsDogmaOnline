using Arrowgene.Buffers;

namespace Arrowgene.Ddon.Shared.Entity.Structure
{
    /// <summary>
    /// Structure for GP Shop display lineup items.
    /// Based on PC client packet capture analysis (v03040008).
    /// Note: PC client has ImageURL and Unk0 fields that PS4 client does not have.
    /// </summary>
    public class CDataGPShopDisplayLineup
    {
        public uint ID { get; set; }
        public uint Category { get; set; }
        public byte IconId { get; set; }
        public uint GP { get; set; }
        public uint DiscountType { get; set; }
        public uint DiscountGP { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }
        public uint LineupID { get; set; }
        public string ImageURL { get; set; }
        public uint BackIconID { get; set; }
        public uint FrameIconID { get; set; }
        public uint BehaviorAfterBuyingType { get; set; }
        public uint Unk0 { get; set; }

        public CDataGPShopDisplayLineup()
        {
            Name = string.Empty;
            Comment = string.Empty;
            ImageURL = string.Empty;
        }

        public class Serializer : EntitySerializer<CDataGPShopDisplayLineup>
        {
            public override void Write(IBuffer buffer, CDataGPShopDisplayLineup obj)
            {
                WriteUInt32(buffer, obj.ID);
                WriteUInt32(buffer, obj.Category);
                WriteByte(buffer, obj.IconId);
                WriteUInt32(buffer, obj.GP);
                WriteUInt32(buffer, obj.DiscountType);
                WriteUInt32(buffer, obj.DiscountGP);
                WriteMtString(buffer, obj.Name);
                WriteMtString(buffer, obj.Comment);
                WriteUInt32(buffer, obj.LineupID);
                WriteMtString(buffer, obj.ImageURL);
                WriteUInt32(buffer, obj.BackIconID);
                WriteUInt32(buffer, obj.FrameIconID);
                WriteUInt32(buffer, obj.BehaviorAfterBuyingType);
                WriteUInt32(buffer, obj.Unk0);
            }

            public override CDataGPShopDisplayLineup Read(IBuffer buffer)
            {
                CDataGPShopDisplayLineup obj = new CDataGPShopDisplayLineup
                {
                    ID = ReadUInt32(buffer),
                    Category = ReadUInt32(buffer),
                    IconId = ReadByte(buffer),
                    GP = ReadUInt32(buffer),
                    DiscountType = ReadUInt32(buffer),
                    DiscountGP = ReadUInt32(buffer),
                    Name = ReadMtString(buffer),
                    Comment = ReadMtString(buffer),
                    LineupID = ReadUInt32(buffer),
                    ImageURL = ReadMtString(buffer),
                    BackIconID = ReadUInt32(buffer),
                    FrameIconID = ReadUInt32(buffer),
                    BehaviorAfterBuyingType = ReadUInt32(buffer),
                    Unk0 = ReadUInt32(buffer)
                };
                return obj;
            }
        }
    }
}
