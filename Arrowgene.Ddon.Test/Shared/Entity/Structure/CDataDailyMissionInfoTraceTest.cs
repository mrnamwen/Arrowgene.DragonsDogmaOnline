using System;
using System.Collections.Generic;
using Arrowgene.Buffers;
using Arrowgene.Ddon.GameServer.Dump;
using Arrowgene.Ddon.Shared.Entity;
using Arrowgene.Ddon.Shared.Entity.Structure;
using Xunit;
using Xunit.Abstractions;

namespace Arrowgene.Ddon.Test.Shared.Entity.Structure
{
    public class CDataDailyMissionInfoTraceTest
    {
        private readonly ITestOutputHelper _output;

        public CDataDailyMissionInfoTraceTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TraceMissionBoundaries()
        {
            byte[] fullData = GameFull.data_Dump_119;
            byte[] bin = new byte[fullData.Length - 8];
            Array.Copy(fullData, 8, bin, 0, bin.Length);

            IBuffer buffer = new StreamBuffer(bin);
            buffer.SetPositionStart();

            uint listCount = buffer.ReadUInt32(Arrowgene.Buffers.Endianness.Big);
            _output.WriteLine($"List count: {listCount}");

            for (int i = 0; i < listCount && i < 3; i++)
            {
                long startPos = buffer.Position;
                _output.WriteLine($"\n=== Mission {i + 1} starts at 0x{startPos:X} ===");

                // Read each field manually and track position
                uint missionId = buffer.ReadUInt32(Arrowgene.Buffers.Endianness.Big);
                uint category = buffer.ReadUInt32(Arrowgene.Buffers.Endianness.Big);
                uint type = buffer.ReadUInt32(Arrowgene.Buffers.Endianness.Big);
                uint targetCount = buffer.ReadUInt32(Arrowgene.Buffers.Endianness.Big);
                uint currentCount = buffer.ReadUInt32(Arrowgene.Buffers.Endianness.Big);
                _output.WriteLine($"  MissionId={missionId}, Category={category}, Type={type}, Target={targetCount}, Current={currentCount}");
                _output.WriteLine($"  After core fields: 0x{buffer.Position:X}");

                // Unknown 1-4
                uint u1 = buffer.ReadUInt32(Arrowgene.Buffers.Endianness.Big);
                uint u2 = buffer.ReadUInt32(Arrowgene.Buffers.Endianness.Big);
                uint u3 = buffer.ReadUInt32(Arrowgene.Buffers.Endianness.Big);
                uint u4 = buffer.ReadUInt32(Arrowgene.Buffers.Endianness.Big);
                _output.WriteLine($"  Unknown1-4: {u1}, {u2}, {u3}, {u4}");
                _output.WriteLine($"  After Unknown1-4: 0x{buffer.Position:X}");

                // RewardList
                uint rewardCount = buffer.ReadUInt32(Arrowgene.Buffers.Endianness.Big);
                _output.WriteLine($"  RewardList count: {rewardCount}");
                for (int j = 0; j < rewardCount; j++)
                {
                    uint itemId = buffer.ReadUInt32(Arrowgene.Buffers.Endianness.Big);
                    uint num = buffer.ReadUInt32(Arrowgene.Buffers.Endianness.Big);
                    uint walletType = buffer.ReadUInt32(Arrowgene.Buffers.Endianness.Big);
                    uint walletAmount = buffer.ReadUInt32(Arrowgene.Buffers.Endianness.Big);
                    _output.WriteLine($"    Reward[{j}]: ItemId={itemId}, Num={num}, WalletType={walletType}, WalletAmount={walletAmount}");
                }
                _output.WriteLine($"  After RewardList: 0x{buffer.Position:X}");

                // SecondList
                uint secondCount = buffer.ReadUInt32(Arrowgene.Buffers.Endianness.Big);
                _output.WriteLine($"  SecondList count: {secondCount}");
                for (int j = 0; j < secondCount; j++)
                {
                    uint v1 = buffer.ReadUInt32(Arrowgene.Buffers.Endianness.Big);
                    uint v2 = buffer.ReadUInt32(Arrowgene.Buffers.Endianness.Big);
                    uint v3 = buffer.ReadUInt32(Arrowgene.Buffers.Endianness.Big);
                    uint v4 = buffer.ReadUInt32(Arrowgene.Buffers.Endianness.Big);
                    _output.WriteLine($"    Second[{j}]: {v1}, {v2}, {v3}, {v4}");
                }
                _output.WriteLine($"  After SecondList: 0x{buffer.Position:X}");

                // Unknown 5-6 + Unknown7
                uint u5 = buffer.ReadUInt32(Arrowgene.Buffers.Endianness.Big);
                uint u6 = buffer.ReadUInt32(Arrowgene.Buffers.Endianness.Big);
                ushort u7 = buffer.ReadUInt16(Arrowgene.Buffers.Endianness.Big);
                _output.WriteLine($"  Unknown5={u5}, Unknown6={u6}, Unknown7={u7}");
                _output.WriteLine($"  After Unknown5-7: 0x{buffer.Position:X}");

                // Title
                ushort titleLen = buffer.ReadUInt16(Arrowgene.Buffers.Endianness.Big);
                byte[] titleBytes = buffer.ReadBytes((int)titleLen);
                string title = System.Text.Encoding.UTF8.GetString(titleBytes);
                _output.WriteLine($"  Title ({titleLen} bytes): '{title}'");
                _output.WriteLine($"  After Title: 0x{buffer.Position:X}");

                // IconUrl
                ushort iconLen = buffer.ReadUInt16(Arrowgene.Buffers.Endianness.Big);
                byte[] iconBytes = buffer.ReadBytes((int)iconLen);
                string iconUrl = System.Text.Encoding.UTF8.GetString(iconBytes);
                _output.WriteLine($"  IconUrl ({iconLen} bytes): '{iconUrl}'");
                _output.WriteLine($"  After IconUrl: 0x{buffer.Position:X}");

                // ImageUrl
                ushort imageLen = buffer.ReadUInt16(Arrowgene.Buffers.Endianness.Big);
                byte[] imageBytes = buffer.ReadBytes((int)imageLen);
                string imageUrl = System.Text.Encoding.UTF8.GetString(imageBytes);
                _output.WriteLine($"  ImageUrl ({imageLen} bytes): '{imageUrl}'");
                _output.WriteLine($"  After ImageUrl: 0x{buffer.Position:X}");

                // Trailing fields
                uint u10 = buffer.ReadUInt32(Arrowgene.Buffers.Endianness.Big);
                byte flags = buffer.ReadByte();
                ulong startTime = buffer.ReadUInt64(Arrowgene.Buffers.Endianness.Big);
                ulong endTime = buffer.ReadUInt64(Arrowgene.Buffers.Endianness.Big);
                uint sortId = buffer.ReadUInt32(Arrowgene.Buffers.Endianness.Big);
                _output.WriteLine($"  Unknown10={u10}, Flags={flags}, StartTime={startTime}, EndTime={endTime}, SortId={sortId}");
                _output.WriteLine($"  After trailing: 0x{buffer.Position:X}");

                long endPos = buffer.Position;
                _output.WriteLine($"  Mission {i + 1} size: {endPos - startPos} bytes");

                // Show next few bytes to understand what comes next
                _output.WriteLine($"  Next 16 bytes:");
                for (int k = 0; k < 16 && buffer.Position + k < buffer.Size; k++)
                {
                    _output.WriteLine($"    0x{buffer.Position + k:X}: 0x{bin[buffer.Position + k]:X2}");
                }
            }

            _output.WriteLine($"\nFinal buffer position: 0x{buffer.Position:X}");
            _output.WriteLine($"Expected end: 0x{bin.Length:X}");
        }
    }
}
