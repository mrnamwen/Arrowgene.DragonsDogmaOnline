using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Network;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class S2CInfinityDeliveryGetCurrentEventRes : ServerResponse
    {
        public override PacketId Id => PacketId.S2C_INFINITY_DELIVERY_GET_CURRENT_EVENT_RES;

        public S2CInfinityDeliveryGetCurrentEventRes()
        {
        }

        public uint EventId { get; set; }
        public bool IsActive { get; set; }
        public ulong StartTime { get; set; }
        public ulong EndTime { get; set; }

        public class Serializer : PacketEntitySerializer<S2CInfinityDeliveryGetCurrentEventRes>
        {
            public override void Write(IBuffer buffer, S2CInfinityDeliveryGetCurrentEventRes obj)
            {
                WriteServerResponse(buffer, obj);
                WriteUInt32(buffer, obj.EventId);
                WriteBool(buffer, obj.IsActive);
                WriteUInt64(buffer, obj.StartTime);
                WriteUInt64(buffer, obj.EndTime);
            }

            public override S2CInfinityDeliveryGetCurrentEventRes Read(IBuffer buffer)
            {
                S2CInfinityDeliveryGetCurrentEventRes obj = new S2CInfinityDeliveryGetCurrentEventRes();
                ReadServerResponse(buffer, obj);
                obj.EventId = ReadUInt32(buffer);
                obj.IsActive = ReadBool(buffer);
                obj.StartTime = ReadUInt64(buffer);
                obj.EndTime = ReadUInt64(buffer);
                return obj;
            }
        }
    }
}
