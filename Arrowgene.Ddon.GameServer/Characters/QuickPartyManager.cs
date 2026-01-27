using Arrowgene.Ddon.GameServer.Party;
using Arrowgene.Ddon.GameServer.Utils;
using Arrowgene.Ddon.Server;
using Arrowgene.Ddon.Shared.Entity.PacketStructure;
using Arrowgene.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Arrowgene.Ddon.GameServer.Characters
{
    public class QuickPartyManager
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(QuickPartyManager));

        public static readonly uint QUICK_PARTY_TIMEOUT_SECONDS = 1800; // 30 minutes
        public static readonly uint QUICK_PARTY_READY_TIMEOUT_SECONDS = 60; // 60 seconds to confirm
        public static readonly uint DEFAULT_MIN_MEMBERS = 2;
        public static readonly uint DEFAULT_MAX_MEMBERS = 4;

        private readonly DdonGameServer _Server;
        private readonly object _Lock = new object();

        // Content ID -> Queue of waiting registrations
        private readonly Dictionary<uint, List<QuickPartyRegistration>> _ContentQueues;
        // Character ID -> Registration
        private readonly Dictionary<uint, QuickPartyRegistration> _CharacterRegistrations;
        // Registration ID -> Registration
        private readonly Dictionary<uint, QuickPartyRegistration> _Registrations;
        // Match ID -> Match data
        private readonly Dictionary<uint, QuickPartyMatch> _Matches;
        // Character ID -> Match ID
        private readonly Dictionary<uint, uint> _CharacterMatches;

        private readonly UniqueIdPool _RegistrationIdPool;
        private readonly UniqueIdPool _MatchIdPool;

        public class QuickPartyRegistration
        {
            public uint RegistrationId { get; set; }
            public uint CharacterId { get; set; }
            public uint ContentId { get; set; }
            public uint QuestScheduleId { get; set; }
            public DateTime RegisteredAt { get; set; }
            public uint TimeoutTimerId { get; set; }
        }

        public class QuickPartyMatch
        {
            public uint MatchId { get; set; }
            public uint ContentId { get; set; }
            public uint QuestScheduleId { get; set; }
            public List<uint> MemberCharacterIds { get; set; } = new List<uint>();
            public Dictionary<uint, bool> MemberReadyState { get; set; } = new Dictionary<uint, bool>();
            public uint ReadyTimerId { get; set; }
            public DateTime MatchedAt { get; set; }
        }

        public QuickPartyManager(DdonGameServer server)
        {
            _Server = server;
            _ContentQueues = new Dictionary<uint, List<QuickPartyRegistration>>();
            _CharacterRegistrations = new Dictionary<uint, QuickPartyRegistration>();
            _Registrations = new Dictionary<uint, QuickPartyRegistration>();
            _Matches = new Dictionary<uint, QuickPartyMatch>();
            _CharacterMatches = new Dictionary<uint, uint>();
            _RegistrationIdPool = new UniqueIdPool(1);
            _MatchIdPool = new UniqueIdPool(1);
        }

        /// <summary>
        /// Register a character for quick party matchmaking for a specific content
        /// </summary>
        public QuickPartyRegistration RegisterForContent(uint characterId, uint contentId)
        {
            lock (_Lock)
            {
                // Check if already registered
                if (_CharacterRegistrations.ContainsKey(characterId))
                {
                    Logger.Error($"Character {characterId} already registered for quick party");
                    return null;
                }

                // Check if already in a match
                if (_CharacterMatches.ContainsKey(characterId))
                {
                    Logger.Error($"Character {characterId} already in a quick party match");
                    return null;
                }

                var registration = new QuickPartyRegistration
                {
                    RegistrationId = _RegistrationIdPool.GenerateId(),
                    CharacterId = characterId,
                    ContentId = contentId,
                    QuestScheduleId = 0,
                    RegisteredAt = DateTime.UtcNow
                };

                // Add to tracking dictionaries
                _Registrations[registration.RegistrationId] = registration;
                _CharacterRegistrations[characterId] = registration;

                // Add to content queue
                if (!_ContentQueues.ContainsKey(contentId))
                {
                    _ContentQueues[contentId] = new List<QuickPartyRegistration>();
                }
                _ContentQueues[contentId].Add(registration);

                // Start timeout timer
                registration.TimeoutTimerId = _Server.TimerManager.CreateTimer(QUICK_PARTY_TIMEOUT_SECONDS, () =>
                {
                    HandleRegistrationTimeout(registration.RegistrationId);
                });
                _Server.TimerManager.StartTimer(registration.TimeoutTimerId);

                Logger.Info($"Character {characterId} registered for quick party content {contentId}, RegistrationId={registration.RegistrationId}");

                // Try to find a match
                TryMatchContent(contentId);

                return registration;
            }
        }

        /// <summary>
        /// Register a character for quick party matchmaking for a specific quest
        /// </summary>
        public QuickPartyRegistration RegisterForQuest(uint characterId, uint questScheduleId)
        {
            lock (_Lock)
            {
                // Check if already registered
                if (_CharacterRegistrations.ContainsKey(characterId))
                {
                    Logger.Error($"Character {characterId} already registered for quick party");
                    return null;
                }

                // Check if already in a match
                if (_CharacterMatches.ContainsKey(characterId))
                {
                    Logger.Error($"Character {characterId} already in a quick party match");
                    return null;
                }

                // Use quest schedule ID as the content ID for matching purposes
                uint contentId = questScheduleId;

                var registration = new QuickPartyRegistration
                {
                    RegistrationId = _RegistrationIdPool.GenerateId(),
                    CharacterId = characterId,
                    ContentId = contentId,
                    QuestScheduleId = questScheduleId,
                    RegisteredAt = DateTime.UtcNow
                };

                // Add to tracking dictionaries
                _Registrations[registration.RegistrationId] = registration;
                _CharacterRegistrations[characterId] = registration;

                // Add to content queue
                if (!_ContentQueues.ContainsKey(contentId))
                {
                    _ContentQueues[contentId] = new List<QuickPartyRegistration>();
                }
                _ContentQueues[contentId].Add(registration);

                // Start timeout timer
                registration.TimeoutTimerId = _Server.TimerManager.CreateTimer(QUICK_PARTY_TIMEOUT_SECONDS, () =>
                {
                    HandleRegistrationTimeout(registration.RegistrationId);
                });
                _Server.TimerManager.StartTimer(registration.TimeoutTimerId);

                Logger.Info($"Character {characterId} registered for quick party quest {questScheduleId}, RegistrationId={registration.RegistrationId}");

                // Try to find a match
                TryMatchContent(contentId);

                return registration;
            }
        }

        /// <summary>
        /// Cancel a character's quick party registration
        /// </summary>
        public bool CancelRegistration(uint characterId)
        {
            lock (_Lock)
            {
                if (!_CharacterRegistrations.TryGetValue(characterId, out var registration))
                {
                    return false;
                }

                RemoveRegistration(registration);
                Logger.Info($"Character {characterId} canceled quick party registration");
                return true;
            }
        }

        /// <summary>
        /// Handle match entry confirmation from a player
        /// </summary>
        public bool ConfirmEntry(uint characterId, bool isEntry)
        {
            lock (_Lock)
            {
                if (!_CharacterMatches.TryGetValue(characterId, out var matchId))
                {
                    Logger.Error($"Character {characterId} not in a quick party match");
                    return false;
                }

                if (!_Matches.TryGetValue(matchId, out var match))
                {
                    Logger.Error($"Match {matchId} not found");
                    return false;
                }

                if (isEntry)
                {
                    match.MemberReadyState[characterId] = true;
                    Logger.Info($"Character {characterId} confirmed entry for match {matchId}");

                    // Check if all players are ready
                    if (AllMembersReady(match))
                    {
                        CreatePartyFromMatch(match);
                    }
                }
                else
                {
                    // Player declined - cancel the match for everyone
                    CancelMatch(matchId, 0); // 0 = user canceled
                }

                return true;
            }
        }

        /// <summary>
        /// Get registration for a character
        /// </summary>
        public QuickPartyRegistration GetRegistration(uint characterId)
        {
            lock (_Lock)
            {
                return _CharacterRegistrations.TryGetValue(characterId, out var reg) ? reg : null;
            }
        }

        /// <summary>
        /// Get match for a character
        /// </summary>
        public QuickPartyMatch GetMatch(uint characterId)
        {
            lock (_Lock)
            {
                if (_CharacterMatches.TryGetValue(characterId, out var matchId))
                {
                    return _Matches.TryGetValue(matchId, out var match) ? match : null;
                }
                return null;
            }
        }

        /// <summary>
        /// Check if character is registered or in a match
        /// </summary>
        public bool IsCharacterInQueue(uint characterId)
        {
            lock (_Lock)
            {
                return _CharacterRegistrations.ContainsKey(characterId) || _CharacterMatches.ContainsKey(characterId);
            }
        }

        private void TryMatchContent(uint contentId)
        {
            if (!_ContentQueues.TryGetValue(contentId, out var queue) || queue.Count < DEFAULT_MIN_MEMBERS)
            {
                return;
            }

            // Get up to max members for a match
            var matchMembers = queue.Take((int)DEFAULT_MAX_MEMBERS).ToList();

            if (matchMembers.Count < DEFAULT_MIN_MEMBERS)
            {
                return;
            }

            // Create the match
            var match = new QuickPartyMatch
            {
                MatchId = _MatchIdPool.GenerateId(),
                ContentId = contentId,
                QuestScheduleId = matchMembers[0].QuestScheduleId,
                MatchedAt = DateTime.UtcNow
            };

            foreach (var reg in matchMembers)
            {
                match.MemberCharacterIds.Add(reg.CharacterId);
                match.MemberReadyState[reg.CharacterId] = false;
                _CharacterMatches[reg.CharacterId] = match.MatchId;

                // Remove from queue and registration tracking
                RemoveRegistration(reg, removeFromCharacterTracking: false);
            }

            _Matches[match.MatchId] = match;

            // Start ready-up timer
            match.ReadyTimerId = _Server.TimerManager.CreateTimer(QUICK_PARTY_READY_TIMEOUT_SECONDS, () =>
            {
                HandleReadyTimeout(match.MatchId);
            });
            _Server.TimerManager.StartTimer(match.ReadyTimerId);

            Logger.Info($"Created quick party match {match.MatchId} for content {contentId} with {match.MemberCharacterIds.Count} members");

            // Notify all matched players
            NotifyMatchFound(match);
        }

        private void NotifyMatchFound(QuickPartyMatch match)
        {
            foreach (var characterId in match.MemberCharacterIds)
            {
                var client = _Server.ClientLookup.GetClientByCharacterId(characterId);
                if (client != null)
                {
                    client.Send(new S2CQuickPartyRegisterNtc
                    {
                        ContentId = match.ContentId,
                        MemberCount = (uint)match.MemberCharacterIds.Count
                    });
                }
            }
        }

        private bool AllMembersReady(QuickPartyMatch match)
        {
            return match.MemberReadyState.Values.All(ready => ready);
        }

        private void CreatePartyFromMatch(QuickPartyMatch match)
        {
            // Cancel the ready timer
            if (match.ReadyTimerId != 0)
            {
                _Server.TimerManager.CancelTimer(match.ReadyTimerId);
                match.ReadyTimerId = 0;
            }

            // Get the first member as party leader
            var leaderCharacterId = match.MemberCharacterIds[0];
            var leaderClient = _Server.ClientLookup.GetClientByCharacterId(leaderCharacterId);

            if (leaderClient == null)
            {
                Logger.Error($"Leader client not found for match {match.MatchId}");
                CancelMatch(match.MatchId, 2); // 2 = error
                return;
            }

            // Create the party (boardId 0 for quick party)
            var partyGroup = _Server.PartyManager.NewParty(0);
            if (partyGroup == null)
            {
                Logger.Error($"Failed to create party for match {match.MatchId}");
                CancelMatch(match.MatchId, 2); // 2 = error
                return;
            }

            // Add the leader as host
            partyGroup.AddHost(leaderClient);

            // Add other members to the party
            foreach (var characterId in match.MemberCharacterIds.Skip(1))
            {
                var memberClient = _Server.ClientLookup.GetClientByCharacterId(characterId);
                if (memberClient != null)
                {
                    partyGroup.ForceAccept(memberClient);
                }
            }

            Logger.Info($"Created party {partyGroup.Id} from quick party match {match.MatchId}");

            // Notify all members that the party is ready
            foreach (var characterId in match.MemberCharacterIds)
            {
                var client = _Server.ClientLookup.GetClientByCharacterId(characterId);
                if (client != null)
                {
                    client.Send(new S2CQuickPartyReadyNtc
                    {
                        PartyId = partyGroup.Id
                    });
                }

                // Clean up character match tracking
                _CharacterMatches.Remove(characterId);
            }

            // Clean up match
            _Matches.Remove(match.MatchId);
            _MatchIdPool.ReclaimId(match.MatchId);
        }

        private void CancelMatch(uint matchId, uint reason)
        {
            if (!_Matches.TryGetValue(matchId, out var match))
            {
                return;
            }

            // Cancel the ready timer
            if (match.ReadyTimerId != 0)
            {
                _Server.TimerManager.CancelTimer(match.ReadyTimerId);
            }

            // Notify all members
            foreach (var characterId in match.MemberCharacterIds)
            {
                var client = _Server.ClientLookup.GetClientByCharacterId(characterId);
                if (client != null)
                {
                    client.Send(new S2CQuickPartyCancelNtc
                    {
                        CancelReason = reason
                    });
                }

                _CharacterMatches.Remove(characterId);
            }

            _Matches.Remove(matchId);
            _MatchIdPool.ReclaimId(matchId);

            Logger.Info($"Canceled quick party match {matchId}, reason={reason}");
        }

        private void HandleRegistrationTimeout(uint registrationId)
        {
            lock (_Lock)
            {
                if (!_Registrations.TryGetValue(registrationId, out var registration))
                {
                    return;
                }

                var client = _Server.ClientLookup.GetClientByCharacterId(registration.CharacterId);
                if (client != null)
                {
                    client.Send(new S2CQuickPartyCancelNtc
                    {
                        CancelReason = 1 // 1 = timeout
                    });
                }

                RemoveRegistration(registration);
                Logger.Info($"Quick party registration {registrationId} timed out");
            }
        }

        private void HandleReadyTimeout(uint matchId)
        {
            lock (_Lock)
            {
                CancelMatch(matchId, 1); // 1 = timeout
            }
        }

        private void RemoveRegistration(QuickPartyRegistration registration, bool removeFromCharacterTracking = true)
        {
            // Cancel timeout timer
            if (registration.TimeoutTimerId != 0)
            {
                _Server.TimerManager.CancelTimer(registration.TimeoutTimerId);
            }

            // Remove from content queue
            if (_ContentQueues.TryGetValue(registration.ContentId, out var queue))
            {
                queue.Remove(registration);
                if (queue.Count == 0)
                {
                    _ContentQueues.Remove(registration.ContentId);
                }
            }

            // Remove from tracking dictionaries
            _Registrations.Remove(registration.RegistrationId);
            if (removeFromCharacterTracking)
            {
                _CharacterRegistrations.Remove(registration.CharacterId);
            }

            _RegistrationIdPool.ReclaimId(registration.RegistrationId);
        }

        /// <summary>
        /// Clean up when a character disconnects
        /// </summary>
        public void HandleCharacterDisconnect(uint characterId)
        {
            lock (_Lock)
            {
                // Cancel any registration
                if (_CharacterRegistrations.TryGetValue(characterId, out var registration))
                {
                    RemoveRegistration(registration);
                }

                // Handle if in a match
                if (_CharacterMatches.TryGetValue(characterId, out var matchId))
                {
                    CancelMatch(matchId, 2); // 2 = error (player disconnected)
                }
            }
        }
    }
}
