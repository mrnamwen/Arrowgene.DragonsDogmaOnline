using System;
using Arrowgene.Buffers;
using Arrowgene.Ddon.GameServer.Dump;
using Xunit;
using Xunit.Abstractions;

namespace Arrowgene.Ddon.Test.Shared.Entity.Structure
{
    public class CDataDailyMissionInfoFullTraceTest
    {
        private readonly ITestOutputHelper _output;

        public CDataDailyMissionInfoFullTraceTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TraceAllMissions()
        {
            byte[] fullData = GameFull.data_Dump_119;
            byte[] bin = new byte[fullData.Length - 8];
            Array.Copy(fullData, 8, bin, 0, bin.Length);

            IBuffer buffer = new StreamBuffer(bin);
            buffer.SetPositionStart();

            uint listCount = buffer.ReadUInt32(Arrowgene.Buffers.Endianness.Big);
            _output.WriteLine($"List count: {listCount}");
            _output.WriteLine($"Total data size: {bin.Length} bytes");

            for (int i = 0; i < listCount; i++)
            {
                long startPos = buffer.Position;

                // Read core fields
                uint missionId = buffer.ReadUInt32(Arrowgene.Buffers.Endianness.Big);
                buffer.ReadUInt32(Arrowgene.Buffers.Endianness.Big); // category
                buffer.ReadUInt32(Arrowgene.Buffers.Endianness.Big); // type
                buffer.ReadUInt32(Arrowgene.Buffers.Endianness.Big); // targetCount
                buffer.ReadUInt32(Arrowgene.Buffers.Endianness.Big); // currentCount

                // Unknown 1-4
                for (int j = 0; j < 4; j++) buffer.ReadUInt32(Arrowgene.Buffers.Endianness.Big);

                // RewardList
                uint rewardCount = buffer.ReadUInt32(Arrowgene.Buffers.Endianness.Big);
                for (int j = 0; j < rewardCount; j++)
                {
                    buffer.ReadUInt32(Arrowgene.Buffers.Endianness.Big);
                    buffer.ReadUInt32(Arrowgene.Buffers.Endianness.Big);
                    buffer.ReadUInt32(Arrowgene.Buffers.Endianness.Big);
                    buffer.ReadUInt32(Arrowgene.Buffers.Endianness.Big);
                }

                // SecondList
                uint secondCount = buffer.ReadUInt32(Arrowgene.Buffers.Endianness.Big);
                for (int j = 0; j < secondCount; j++)
                {
                    buffer.ReadUInt32(Arrowgene.Buffers.Endianness.Big);
                    buffer.ReadUInt32(Arrowgene.Buffers.Endianness.Big);
                    buffer.ReadUInt32(Arrowgene.Buffers.Endianness.Big);
                    buffer.ReadUInt32(Arrowgene.Buffers.Endianness.Big);
                }

                // Unknown 5-7
                buffer.ReadUInt32(Arrowgene.Buffers.Endianness.Big);
                buffer.ReadUInt32(Arrowgene.Buffers.Endianness.Big);
                buffer.ReadUInt16(Arrowgene.Buffers.Endianness.Big);

                // Title
                ushort titleLen = buffer.ReadUInt16(Arrowgene.Buffers.Endianness.Big);
                buffer.ReadBytes((int)titleLen);

                // IconUrl
                ushort iconLen = buffer.ReadUInt16(Arrowgene.Buffers.Endianness.Big);
                buffer.ReadBytes((int)iconLen);

                // ImageUrl
                ushort imageLen = buffer.ReadUInt16(Arrowgene.Buffers.Endianness.Big);
                buffer.ReadBytes((int)imageLen);

                // Trailing fields
                buffer.ReadUInt32(Arrowgene.Buffers.Endianness.Big);
                buffer.ReadByte();
                buffer.ReadUInt64(Arrowgene.Buffers.Endianness.Big);
                buffer.ReadUInt64(Arrowgene.Buffers.Endianness.Big);
                buffer.ReadUInt32(Arrowgene.Buffers.Endianness.Big);

                long endPos = buffer.Position;
                long size = endPos - startPos;
                _output.WriteLine($"Mission {i + 1}: MissionId={missionId}, start=0x{startPos:X}, end=0x{endPos:X}, size={size}");
            }

            _output.WriteLine($"\nAfter all missions: 0x{buffer.Position:X}");
            _output.WriteLine($"Remaining bytes: {bin.Length - buffer.Position}");

            // Show what's after the missions
            if (buffer.Position < bin.Length)
            {
                _output.WriteLine($"\nData after missions (first 64 bytes):");
                int bytesToShow = Math.Min(64, (int)(bin.Length - buffer.Position));
                for (int i = 0; i < bytesToShow; i++)
                {
                    _output.WriteLine($"  0x{buffer.Position + i:X}: 0x{bin[buffer.Position + i]:X2}");
                }
            }
        }
    }
}
