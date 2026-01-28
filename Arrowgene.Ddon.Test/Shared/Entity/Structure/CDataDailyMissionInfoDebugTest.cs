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
    public class CDataDailyMissionInfoDebugTest
    {
        private readonly ITestOutputHelper _output;

        public CDataDailyMissionInfoDebugTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void DebugFirstMission()
        {
            // Extract test data from dump first
            string testFile = TestUtils.GetTestPath("CDataDailyMissionInfoTestData.bin");
            ExtractTestDataFromDump(testFile);

            byte[] bin = File.ReadAllBytes(testFile);
            _output.WriteLine($"File size: {bin.Length}");

            IBuffer buffer = new StreamBuffer(bin);
            buffer.SetPositionStart();

            // Manual parse to determine structure
            _output.WriteLine("\n--- Manual Parse of First Mission ---");

            // List count
            uint listCount = buffer.ReadUInt32(Arrowgene.Buffers.Endianness.Big);
            _output.WriteLine($"List count: {listCount}, pos: 0x{buffer.Position:X}");

            // First mission
            _output.WriteLine($"\n--- Mission 1 starts at 0x{buffer.Position:X} ---");

            uint missionId = buffer.ReadUInt32(Arrowgene.Buffers.Endianness.Big);
            _output.WriteLine($"MissionId: {missionId}, pos: 0x{buffer.Position:X}");

            uint category = buffer.ReadUInt32(Arrowgene.Buffers.Endianness.Big);
            _output.WriteLine($"Category: {category}, pos: 0x{buffer.Position:X}");

            uint type = buffer.ReadUInt32(Arrowgene.Buffers.Endianness.Big);
            _output.WriteLine($"Type: {type}, pos: 0x{buffer.Position:X}");

            uint targetCount = buffer.ReadUInt32(Arrowgene.Buffers.Endianness.Big);
            _output.WriteLine($"TargetCount: {targetCount}, pos: 0x{buffer.Position:X}");

            uint currentCount = buffer.ReadUInt32(Arrowgene.Buffers.Endianness.Big);
            _output.WriteLine($"CurrentCount: {currentCount}, pos: 0x{buffer.Position:X}");

            // Unknown1-4
            for (int i = 1; i <= 4; i++)
            {
                uint unk = buffer.ReadUInt32(Arrowgene.Buffers.Endianness.Big);
                _output.WriteLine($"Unknown{i}: {unk}, pos: 0x{buffer.Position:X}");
            }

            // RewardList
            uint rewardCount = buffer.ReadUInt32(Arrowgene.Buffers.Endianness.Big);
            _output.WriteLine($"RewardList count: {rewardCount}, pos: 0x{buffer.Position:X}");
            for (int i = 0; i < rewardCount; i++)
            {
                uint itemId = buffer.ReadUInt32(Arrowgene.Buffers.Endianness.Big);
                uint num = buffer.ReadUInt32(Arrowgene.Buffers.Endianness.Big);
                uint walletType = buffer.ReadUInt32(Arrowgene.Buffers.Endianness.Big);
                uint walletAmount = buffer.ReadUInt32(Arrowgene.Buffers.Endianness.Big);
                _output.WriteLine($"  Reward {i+1}: ItemId={itemId}, Num={num}, WalletType={walletType}, WalletAmount={walletAmount}, pos: 0x{buffer.Position:X}");
            }

            // SecondList
            uint secondCount = buffer.ReadUInt32(Arrowgene.Buffers.Endianness.Big);
            _output.WriteLine($"SecondList count: {secondCount}, pos: 0x{buffer.Position:X}");
            for (int i = 0; i < secondCount; i++)
            {
                uint v1 = buffer.ReadUInt32(Arrowgene.Buffers.Endianness.Big);
                uint v2 = buffer.ReadUInt32(Arrowgene.Buffers.Endianness.Big);
                uint v3 = buffer.ReadUInt32(Arrowgene.Buffers.Endianness.Big);
                uint v4 = buffer.ReadUInt32(Arrowgene.Buffers.Endianness.Big);
                _output.WriteLine($"  Second {i+1}: {v1}, {v2}, {v3}, {v4}, pos: 0x{buffer.Position:X}");
            }

            // Now we need to figure out what comes before strings
            // Show next 40 bytes raw
            _output.WriteLine($"\nNext 40 bytes (should contain unknown fields and start of Title):");
            long startPos = buffer.Position;
            for (int i = 0; i < 40 && buffer.Position < buffer.Size; i++)
            {
                byte b = buffer.ReadByte();
                _output.WriteLine($"  0x{startPos + i:X}: 0x{b:X2}");
            }
        }

        private void ExtractTestDataFromDump(string testFile)
        {
            // Get the dump data (packet 72.0.2 - DailyMissionListGetRes)
            byte[] fullData = GameFull.data_Dump_119;
            _output.WriteLine($"Full dump size: {fullData.Length}");

            // Skip first 8 bytes (packet header: 4 bytes error + 4 bytes result)
            byte[] missionData = new byte[fullData.Length - 8];
            Array.Copy(fullData, 8, missionData, 0, missionData.Length);
            _output.WriteLine($"Mission data size: {missionData.Length}");

            // Write to test file
            File.WriteAllBytes(testFile, missionData);
            _output.WriteLine($"Wrote {missionData.Length} bytes to {testFile}");

            // Show first bytes for verification
            System.Text.StringBuilder sb = new System.Text.StringBuilder("First 20 bytes: ");
            for (int i = 0; i < 20 && i < missionData.Length; i++)
            {
                sb.Append($"0x{missionData[i]:X2} ");
            }
            _output.WriteLine(sb.ToString());
        }
    }
}
