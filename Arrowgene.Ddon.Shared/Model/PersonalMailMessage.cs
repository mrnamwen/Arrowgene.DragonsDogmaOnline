using Arrowgene.Ddon.Shared.Entity.Structure;
using System;

namespace Arrowgene.Ddon.Shared.Model
{
    public class PersonalMailMessage
    {
        public PersonalMailMessage()
        {
            Title = String.Empty;
            Body = String.Empty;
        }

        public ulong MessageId { get; set; }
        public uint RecipientCharacterId { get; set; }
        public uint SenderCharacterId { get; set; }
        public string SenderName { get; set; } = String.Empty;
        public string SenderClanName { get; set; } = String.Empty;
        public MailState MessageState { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public ulong SendDate { get; set; }

        public CDataMailInfo ToCDataMailInfo()
        {
            return new CDataMailInfo()
            {
                Id = MessageId,
                State = MessageState,
                BaseInfo = new CDataCommunityCharacterBaseInfo()
                {
                    CharacterId = SenderCharacterId,
                    CharacterName = new CDataCharacterName()
                    {
                        FirstName = SenderName
                    },
                    ClanName = SenderClanName
                },
                SenderName = SenderName,
                MailText = Title,
                SenderDate = SendDate,
                ItemState = 0
            };
        }
    }
}
