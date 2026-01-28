using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Arrowgene.Buffers;
using Arrowgene.Ddon.GameServer.Dump;
using Arrowgene.Ddon.Shared.Entity;
using Arrowgene.Ddon.Shared.Entity.Structure;
using Xunit;
using Xunit.Abstractions;

namespace Arrowgene.Ddon.Test.Shared.Entity.Structure
{
    public class CDataDailyMissionInfoCompareTest
    {
        private readonly ITestOutputHelper _output;

        public CDataDailyMissionInfoCompareTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void CompareOriginalAndSerialized()
        {
            byte[] fullData = GameFull.data_Dump_119;
            byte[] bin = new byte[fullData.Length - 8];
            Array.Copy(fullData, 8, bin, 0, bin.Length);

            // Deserialize
            IBuffer buffer = new StreamBuffer(bin);
            buffer.SetPositionStart();
            List<CDataDailyMissionInfo> deserialized = EntitySerializer.Get<CDataDailyMissionInfo>().ReadList(buffer);
            _output.WriteLine($"Deserialized {deserialized.Count} missions");
            _output.WriteLine($"Buffer position after deserialize: 0x{buffer.Position:X} (expected 0x{bin.Length:X})");
            _output.WriteLine($"Bytes remaining: {bin.Length - buffer.Position}");

            // Dump first mission fields
            var m = deserialized[0];
            _output.WriteLine($"\n--- First Mission Details ---");
            _output.WriteLine($"MissionId: {m.MissionId}");
            _output.WriteLine($"Category: {m.Category}");
            _output.WriteLine($"Type: {m.Type}");
            _output.WriteLine($"TargetCount: {m.TargetCount}");
            _output.WriteLine($"CurrentCount: {m.CurrentCount}");
            _output.WriteLine($"Unknown1-4: {m.Unknown1}, {m.Unknown2}, {m.Unknown3}, {m.Unknown4}");
            _output.WriteLine($"RewardList count: {m.RewardList.Count}");
            if (m.RewardList.Count > 0)
            {
                var r = m.RewardList[0];
                _output.WriteLine($"  Reward[0]: ItemId={r.ItemId}, Num={r.Num}, WalletType={r.WalletType}, WalletAmount={r.WalletAmount}");
            }
            _output.WriteLine($"SecondList count: {m.SecondList.Count}");
            _output.WriteLine($"Unknown5-6: {m.Unknown5}, {m.Unknown6}");
            _output.WriteLine($"Title: '{m.Title}'");
            _output.WriteLine($"IconUrl: '{m.IconUrl}'");
            _output.WriteLine($"ImageUrl: '{m.ImageUrl}'");
            _output.WriteLine($"Unknown10: {m.Unknown10}");
            _output.WriteLine($"Flags: {m.Flags} (IsComplete={m.IsComplete}, IsReceived={m.IsReceived})");
            _output.WriteLine($"StartTime: {m.StartTime}");
            _output.WriteLine($"EndTime: {m.EndTime}");
            _output.WriteLine($"SortId: {m.SortId}");

            // Serialize
            buffer = new StreamBuffer();
            EntitySerializer.Get<CDataDailyMissionInfo>().WriteList(buffer, deserialized);
            byte[] serialized = buffer.GetAllBytes();

            _output.WriteLine($"\nOriginal size: {bin.Length}, Serialized size: {serialized.Length}");
            _output.WriteLine($"Diff: {serialized.Length - bin.Length} bytes");
        }
    }
}
