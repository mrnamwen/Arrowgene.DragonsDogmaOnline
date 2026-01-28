using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Arrowgene.Buffers;
using Arrowgene.Ddon.GameServer.Dump;
using Arrowgene.Ddon.Shared.Entity;
using Arrowgene.Ddon.Shared.Entity.Structure;
using Xunit;

namespace Arrowgene.Ddon.Test.Shared.Entity.Structure
{
    public class CDataDailyMissionInfoTest
    {
        [Fact]
        public void TestCDataDailyMissionInfoSerializer()
        {
            // Extract test data from dump (packet 72.0.2 - DailyMissionListGetRes)
            // Note: The full packet has TWO lists - MissionList (15 items) then a second list (14 items).
            // This test only verifies the MissionList portion.
            byte[] fullData = GameFull.data_Dump_119;
            // Skip first 8 bytes (packet header: 4 bytes error + 4 bytes result)
            byte[] packetData = new byte[fullData.Length - 8];
            Array.Copy(fullData, 8, packetData, 0, packetData.Length);

            IBuffer buffer;
            // Deserialize the first list
            buffer = new StreamBuffer(packetData);
            buffer.SetPositionStart();
            List<CDataDailyMissionInfo> deserialized = EntitySerializer.Get<CDataDailyMissionInfo>().ReadList(buffer);
            long bytesConsumed = buffer.Position;

            // Extract only the bytes for the first list from original
            byte[] originalListData = new byte[bytesConsumed];
            Array.Copy(packetData, 0, originalListData, 0, bytesConsumed);

            // Serialize back
            buffer = new StreamBuffer();
            EntitySerializer.Get<CDataDailyMissionInfo>().WriteList(buffer, deserialized);
            byte[] serialized = buffer.GetAllBytes();

            // Compare only the MissionList portion
            Assert.True(StructuralComparisons.StructuralEqualityComparer.Equals(originalListData, serialized),
                $"Serialization mismatch. Original size: {originalListData.Length}, Serialized size: {serialized.Length}");
        }
    }
}
