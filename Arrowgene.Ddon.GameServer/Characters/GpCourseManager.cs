using Arrowgene.Ddon.Database.Model;
using Arrowgene.Ddon.Server;
using Arrowgene.Ddon.Shared.Entity.PacketStructure;
using Arrowgene.Ddon.Shared.Entity.Structure;
using Arrowgene.Ddon.Shared.Model;
using Arrowgene.Ddon.Shared.Model.Quest;
using Arrowgene.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Arrowgene.Ddon.GameServer.Characters
{
    public class GpCourseManager
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(GpCourseManager));

        private DdonGameServer _Server;
        private Timer _CourseTimer;
        private CourseBonus _CourseBonus;
        private Dictionary<uint, bool> _CourseIsActive;
        private Dictionary<uint, CourseBonus> _CharacterCourseBonuses;
        private readonly object _CharacterBonusLock = new object();

        public GpCourseManager(DdonGameServer server)
        {
            _Server = server;
            _CourseTimer = null;
            _CourseBonus = new CourseBonus();
            _CourseIsActive = new Dictionary<uint, bool>();
            _CharacterCourseBonuses = new Dictionary<uint, CourseBonus>();
        }

        internal class CourseBonus
        {
            public double PlayerEnemyExpBonus = 0.0;
            public double PawnEnemyExpBonus = 0.0;
            public double WorldQuestExpBonus = 0.0;
            public double EnemyPlayPointBonus = 0.0;
            public double PawnCraftBonus = 0.0;
            public uint DisablePartyExpAdjustment = 0;
            public double EnemyBloodOrbMultiplier = 0.0;
            public uint InfiniteRevive = 0;
            public uint BazaarExhibitExtend = 0;
            public ulong BazaarReExhibitShorten = 0;
            public double BoardQuestApBonus = 0.0;
            public double WorldQuestApBonus = 0.0;
            public uint AreaMasterSupply = 0;
        };

        private void ApplyCourseEffects(uint courseId, CourseBonus targetBonus)
        {
            if (!_Server.AssetRepository.GPCourseInfoAsset.Courses.TryGetValue(courseId, out var courseDescription))
            {
                Logger.Info($"Course {courseId} not found in GPCourseInfoAsset.Courses");
                return;
            }

            foreach (var effectUId in courseDescription.Effects)
            {
                if (!_Server.AssetRepository.GPCourseInfoAsset.Effects.TryGetValue(effectUId, out var effect))
                {
                    continue;
                }

                GPCourseId courseEffectId = (GPCourseId)effect.Id;
                switch (courseEffectId)
                {
                    case GPCourseId.EnemyExpUp:
                        targetBonus.PlayerEnemyExpBonus += (effect.Param0 / 100.0);
                        break;
                    case GPCourseId.PawnEnemyExpUp:
                        targetBonus.PawnEnemyExpBonus += (effect.Param0 / 100.0);
                        break;
                    case GPCourseId.WQRewardExpUp:
                        targetBonus.WorldQuestExpBonus += (effect.Param0 / 100.0);
                        break;
                    case GPCourseId.EnemyPpUp:
                        targetBonus.EnemyPlayPointBonus += (effect.Param0 / 100.0);
                        break;
                    case GPCourseId.PawnCraftExpUp:
                        targetBonus.PawnCraftBonus += (effect.Param0 / 100.0);
                        break;
                    case GPCourseId.AreaPointBQRewardUp:
                        targetBonus.BoardQuestApBonus += (effect.Param0 / 100.0);
                        break;
                    case GPCourseId.AreaPointWQRewardUp:
                        targetBonus.WorldQuestApBonus += (effect.Param0 / 100.0);
                        break;
                    case GPCourseId.DisablePartyAdjustEnemyExp:
                        targetBonus.DisablePartyExpAdjustment += 1;
                        break;
                    case GPCourseId.BloodOrbUp:
                        targetBonus.EnemyBloodOrbMultiplier += (effect.Param0 / 100.0);
                        break;
                    case GPCourseId.InfiniteRevive:
                        targetBonus.InfiniteRevive += 1;
                        break;
                    case GPCourseId.BazaarExhibitExtend:
                        targetBonus.BazaarExhibitExtend += effect.Param0;
                        break;
                    case GPCourseId.BazaarReExhibitShorten:
                        targetBonus.BazaarReExhibitShorten += effect.Param0;
                        break;
                    case GPCourseId.AreaMasterSupply:
                        targetBonus.AreaMasterSupply += 1;
                        break;
                }
            }
        }

        private void ApplyCourseEffects(uint courseId)
        {
            lock (_CourseBonus)
            {
                ApplyCourseEffects(courseId, _CourseBonus);
            }
        }

        private void RemoveCourseEffects(uint courseId, CourseBonus targetBonus)
        {
            if (!_Server.AssetRepository.GPCourseInfoAsset.Courses.TryGetValue(courseId, out var courseDescription))
            {
                return;
            }

            foreach (var effectUId in courseDescription.Effects)
            {
                if (!_Server.AssetRepository.GPCourseInfoAsset.Effects.TryGetValue(effectUId, out var effect))
                {
                    continue;
                }

                GPCourseId courseEffectId = (GPCourseId)effect.Id;
                switch (courseEffectId)
                {
                    case GPCourseId.EnemyExpUp:
                        targetBonus.PlayerEnemyExpBonus -= (effect.Param0 / 100.0);
                        break;
                    case GPCourseId.PawnEnemyExpUp:
                        targetBonus.PawnEnemyExpBonus -= (effect.Param0 / 100.0);
                        break;
                    case GPCourseId.WQRewardExpUp:
                        targetBonus.WorldQuestExpBonus -= (effect.Param0 / 100.0);
                        break;
                    case GPCourseId.EnemyPpUp:
                        targetBonus.EnemyPlayPointBonus -= (effect.Param0 / 100.0);
                        break;
                    case GPCourseId.AreaPointBQRewardUp:
                        targetBonus.BoardQuestApBonus -= (effect.Param0 / 100.0);
                        break;
                    case GPCourseId.AreaPointWQRewardUp:
                        targetBonus.WorldQuestApBonus -= (effect.Param0 / 100.0);
                        break;
                    case GPCourseId.PawnCraftExpUp:
                        targetBonus.PawnCraftBonus -= (effect.Param0 / 100.0);
                        break;
                    case GPCourseId.DisablePartyAdjustEnemyExp:
                        targetBonus.DisablePartyExpAdjustment -= 1;
                        break;
                    case GPCourseId.BloodOrbUp:
                        targetBonus.EnemyBloodOrbMultiplier -= (effect.Param0 / 100.0);
                        break;
                    case GPCourseId.InfiniteRevive:
                        targetBonus.InfiniteRevive -= 1;
                        break;
                    case GPCourseId.BazaarExhibitExtend:
                        targetBonus.BazaarExhibitExtend -= effect.Param0;
                        break;
                    case GPCourseId.BazaarReExhibitShorten:
                        targetBonus.BazaarReExhibitShorten -= effect.Param0;
                        break;
                    case GPCourseId.AreaMasterSupply:
                        targetBonus.AreaMasterSupply -= 1;
                        break;
                }
            }
        }

        private void RemoveCourseEffects(uint courseId)
        {
            lock (_CourseBonus)
            {
                RemoveCourseEffects(courseId, _CourseBonus);
            }
        }

        private static int COURSE_TIMER_TICK = 1 * 1000; // 1 second in ms

        public void EvaluateCourses()
        {
            ulong now = (ulong)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            foreach (var (id, course) in _Server.AssetRepository.GPCourseInfoAsset.ValidCourses)
            {
                if (now >= course.StartTime && now <= course.EndTime)
                {
                    _CourseIsActive[id] = true;
                    ApplyCourseEffects(id);
                }
                else
                {
                    _CourseIsActive[id] = false;
                }
            }

            _CourseTimer = new Timer(state =>
            {
                lock (_CourseIsActive)
                {
                    ulong now = (ulong)DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                    // Evaluate server-wide courses
                    foreach (var (id, course) in _Server.AssetRepository.GPCourseInfoAsset.ValidCourses)
                    {
                        if (now >= course.StartTime && now <= course.EndTime && !_CourseIsActive[id])
                        {
                            _CourseIsActive[id] = true;
                            ApplyCourseEffects(id);

                            var ntc = new S2CGPCourseStartNtc()
                            {
                                CourseID = id,
                                ExpiryTimestamp = course.EndTime,
                                AnnounceType = 0
                            };
                            foreach (var client in _Server.ClientLookup.GetAll())
                            {
                                client.Send(ntc);
                            }
                        }
                        else if (now >= course.EndTime && _CourseIsActive[id])
                        {
                            _CourseIsActive[id] = false;
                            RemoveCourseEffects(id);

                            var ntc = new S2CGpCourseEndNtc()
                            {
                                CourseID = id,
                                AnnounceType = 0
                            };
                            foreach (var client in _Server.ClientLookup.GetAll())
                            {
                                client.Send(ntc);
                            }
                        }
                    }

                    // Clean up expired character courses from database
                    _Server.Database.DeleteExpiredCharacterActiveCourses((long)now);
                }
            }, null, COURSE_TIMER_TICK, COURSE_TIMER_TICK);
        }

        /// <summary>
        /// Gets the list of available (purchased but not activated) courses for a character
        /// </summary>
        public List<CDataGPCourseAvailable> GetAvailableCourses(uint characterId)
        {
            var dbCourses = _Server.Database.SelectCharacterAvailableCourses(characterId);
            var result = new List<CDataGPCourseAvailable>();

            foreach (var dbCourse in dbCourses)
            {
                result.Add(new CDataGPCourseAvailable
                {
                    ID = (uint)dbCourse.Id,
                    CourseName = dbCourse.CourseName,
                    UseLimitTime = dbCourse.DurationSec,
                    CourseID = dbCourse.CourseId,
                    LineupID = dbCourse.LineupId,
                    BackIconID = dbCourse.BackIconId,
                    FrameIconID = dbCourse.FrameIconId
                });
            }

            return result;
        }

        /// <summary>
        /// Gets the list of available courses as CDataGPCourseInfo for the handler
        /// </summary>
        public List<CDataGPCourseInfo> GetAvailableCoursesInfo(uint characterId)
        {
            var dbCourses = _Server.Database.SelectCharacterAvailableCourses(characterId);
            var result = new List<CDataGPCourseInfo>();

            foreach (var dbCourse in dbCourses)
            {
                var courseInfo = new CDataGPCourseInfo
                {
                    CourseId = dbCourse.CourseId,
                    CourseName = dbCourse.CourseName,
                    DoubleCourseTarget = false,
                    PrioGroup = 0,
                    PrioSameTime = 0,
                    AnnounceType = 0,
                    EffectUIDs = new List<uint>()
                };

                // Get effect UIDs from the course definition
                if (_Server.AssetRepository.GPCourseInfoAsset.Courses.TryGetValue(dbCourse.CourseId, out var course))
                {
                    courseInfo.EffectUIDs = course.Effects;
                    courseInfo.DoubleCourseTarget = course.Target;
                    courseInfo.PrioGroup = (byte)course.PriorityGroup;
                    courseInfo.PrioSameTime = (byte)course.PrioritySameTime;
                    courseInfo.AnnounceType = (byte)course.AnnounceType;
                }

                result.Add(courseInfo);
            }

            return result;
        }

        /// <summary>
        /// Activates a course from the available courses list
        /// </summary>
        /// <returns>The end time of the activated course, or 0 if activation failed</returns>
        public ulong ActivateCourse(GameClient client, uint availableId)
        {
            var availableCourse = _Server.Database.SelectCharacterAvailableCourseById(availableId);
            if (availableCourse == null)
            {
                Logger.Error($"Available course with ID {availableId} not found for character {client.Character.CharacterId}");
                return 0;
            }

            if (availableCourse.CharacterId != client.Character.CharacterId)
            {
                Logger.Error($"Available course {availableId} does not belong to character {client.Character.CharacterId}");
                return 0;
            }

            long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            long endTime = now + availableCourse.DurationSec;

            // Check if this course is already active for this character
            var existingActiveCourses = _Server.Database.SelectCharacterActiveCoursesByCourseId(
                client.Character.CharacterId, availableCourse.CourseId);

            if (existingActiveCourses.Count > 0)
            {
                // Extend the existing course instead of creating a new one
                var existingCourse = existingActiveCourses[0];
                if (existingCourse.EndTime > now)
                {
                    // Course is still active, extend it
                    existingCourse.EndTime += availableCourse.DurationSec;
                    _Server.Database.UpdateCharacterActiveCourse(existingCourse);
                    endTime = existingCourse.EndTime;
                    Logger.Info($"Extended course {availableCourse.CourseId} for character {client.Character.CharacterId} until {endTime}");
                }
                else
                {
                    // Course has expired, update with new times
                    existingCourse.StartTime = now;
                    existingCourse.EndTime = endTime;
                    _Server.Database.UpdateCharacterActiveCourse(existingCourse);
                    Logger.Info($"Reactivated course {availableCourse.CourseId} for character {client.Character.CharacterId} until {endTime}");
                }
            }
            else
            {
                // Create new active course
                var activeCourse = new CharacterActiveCourse
                {
                    CharacterId = client.Character.CharacterId,
                    CourseId = availableCourse.CourseId,
                    CourseName = availableCourse.CourseName,
                    StartTime = now,
                    EndTime = endTime
                };
                _Server.Database.InsertCharacterActiveCourse(activeCourse);
                Logger.Info($"Activated course {availableCourse.CourseId} for character {client.Character.CharacterId} until {endTime}");
            }

            // Remove from available courses
            _Server.Database.DeleteCharacterAvailableCourse(availableId);

            // Apply course effects to character's bonus tracker
            LoadCharacterCourseEffects(client.Character.CharacterId);

            // Send course start notification
            var ntc = new S2CGPCourseStartNtc()
            {
                CourseID = availableCourse.CourseId,
                ExpiryTimestamp = (ulong)endTime,
                AnnounceType = 0
            };
            client.Send(ntc);

            return (ulong)endTime;
        }

        /// <summary>
        /// Gets all valid (currently active) courses for a character, combining server-wide and personal courses
        /// </summary>
        public List<CDataGPCourseValid> GetValidCoursesForCharacter(uint characterId)
        {
            var result = new List<CDataGPCourseValid>();
            long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            uint idCounter = 1;

            // Add server-wide active courses
            foreach (var (id, course) in _Server.AssetRepository.GPCourseInfoAsset.ValidCourses)
            {
                if ((ulong)now >= course.StartTime && (ulong)now <= course.EndTime)
                {
                    // Get course name from the Courses dictionary
                    string courseName = course.Comment ?? "";
                    if (_Server.AssetRepository.GPCourseInfoAsset.Courses.TryGetValue(id, out var courseInfo))
                    {
                        courseName = courseInfo.Name;
                    }

                    result.Add(new CDataGPCourseValid
                    {
                        Id = idCounter++,
                        CourseId = id,
                        NameA = courseName,
                        NameB = "",
                        StartTime = course.StartTime,
                        EndTime = course.EndTime
                    });
                }
            }

            // Add character's personal active courses
            var characterCourses = _Server.Database.SelectCharacterActiveCourses(characterId);
            foreach (var course in characterCourses)
            {
                if (course.EndTime > now)
                {
                    result.Add(new CDataGPCourseValid
                    {
                        Id = idCounter++,
                        CourseId = course.CourseId,
                        NameA = course.CourseName,
                        NameB = "(Personal)",
                        StartTime = (ulong)course.StartTime,
                        EndTime = (ulong)course.EndTime
                    });
                }
            }

            return result;
        }

        /// <summary>
        /// Loads and caches the course effects for a character on login
        /// </summary>
        public void LoadCharacterCourseEffects(uint characterId)
        {
            lock (_CharacterBonusLock)
            {
                var bonus = new CourseBonus();
                long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                var activeCourses = _Server.Database.SelectCharacterActiveCourses(characterId);
                foreach (var course in activeCourses)
                {
                    if (course.EndTime > now)
                    {
                        ApplyCourseEffects(course.CourseId, bonus);
                    }
                }

                _CharacterCourseBonuses[characterId] = bonus;
            }
        }

        /// <summary>
        /// Clears the cached course effects when a character logs out
        /// </summary>
        public void ClearCharacterCourseEffects(uint characterId)
        {
            lock (_CharacterBonusLock)
            {
                _CharacterCourseBonuses.Remove(characterId);
            }
        }

        /// <summary>
        /// Gets the combined bonus for a character (server-wide + personal)
        /// </summary>
        private CourseBonus GetCombinedBonus(uint characterId)
        {
            lock (_CharacterBonusLock)
            {
                if (_CharacterCourseBonuses.TryGetValue(characterId, out var charBonus))
                {
                    // Return combined bonus
                    lock (_CourseBonus)
                    {
                        return new CourseBonus
                        {
                            PlayerEnemyExpBonus = _CourseBonus.PlayerEnemyExpBonus + charBonus.PlayerEnemyExpBonus,
                            PawnEnemyExpBonus = _CourseBonus.PawnEnemyExpBonus + charBonus.PawnEnemyExpBonus,
                            WorldQuestExpBonus = _CourseBonus.WorldQuestExpBonus + charBonus.WorldQuestExpBonus,
                            EnemyPlayPointBonus = _CourseBonus.EnemyPlayPointBonus + charBonus.EnemyPlayPointBonus,
                            PawnCraftBonus = _CourseBonus.PawnCraftBonus + charBonus.PawnCraftBonus,
                            DisablePartyExpAdjustment = _CourseBonus.DisablePartyExpAdjustment + charBonus.DisablePartyExpAdjustment,
                            EnemyBloodOrbMultiplier = _CourseBonus.EnemyBloodOrbMultiplier + charBonus.EnemyBloodOrbMultiplier,
                            InfiniteRevive = _CourseBonus.InfiniteRevive + charBonus.InfiniteRevive,
                            BazaarExhibitExtend = _CourseBonus.BazaarExhibitExtend + charBonus.BazaarExhibitExtend,
                            BazaarReExhibitShorten = _CourseBonus.BazaarReExhibitShorten + charBonus.BazaarReExhibitShorten,
                            BoardQuestApBonus = _CourseBonus.BoardQuestApBonus + charBonus.BoardQuestApBonus,
                            WorldQuestApBonus = _CourseBonus.WorldQuestApBonus + charBonus.WorldQuestApBonus,
                            AreaMasterSupply = _CourseBonus.AreaMasterSupply + charBonus.AreaMasterSupply
                        };
                    }
                }
            }

            // No character-specific bonus, return global only
            lock (_CourseBonus)
            {
                return new CourseBonus
                {
                    PlayerEnemyExpBonus = _CourseBonus.PlayerEnemyExpBonus,
                    PawnEnemyExpBonus = _CourseBonus.PawnEnemyExpBonus,
                    WorldQuestExpBonus = _CourseBonus.WorldQuestExpBonus,
                    EnemyPlayPointBonus = _CourseBonus.EnemyPlayPointBonus,
                    PawnCraftBonus = _CourseBonus.PawnCraftBonus,
                    DisablePartyExpAdjustment = _CourseBonus.DisablePartyExpAdjustment,
                    EnemyBloodOrbMultiplier = _CourseBonus.EnemyBloodOrbMultiplier,
                    InfiniteRevive = _CourseBonus.InfiniteRevive,
                    BazaarExhibitExtend = _CourseBonus.BazaarExhibitExtend,
                    BazaarReExhibitShorten = _CourseBonus.BazaarReExhibitShorten,
                    BoardQuestApBonus = _CourseBonus.BoardQuestApBonus,
                    WorldQuestApBonus = _CourseBonus.WorldQuestApBonus,
                    AreaMasterSupply = _CourseBonus.AreaMasterSupply
                };
            }
        }

        // Character-aware bonus methods

        public double EnemyExpBonus(CharacterCommon characterCommon)
        {
            uint characterId = 0;
            if (characterCommon is Character character)
            {
                characterId = character.CharacterId;
            }
            else if (characterCommon is Pawn pawn)
            {
                characterId = pawn.CharacterId;
            }

            var bonus = GetCombinedBonus(characterId);
            if (characterCommon is Character)
            {
                return bonus.PlayerEnemyExpBonus;
            }
            else
            {
                return bonus.PawnEnemyExpBonus;
            }
        }

        public double EnemyExpBonus(uint characterId, bool isPawn)
        {
            var bonus = GetCombinedBonus(characterId);
            return isPawn ? bonus.PawnEnemyExpBonus : bonus.PlayerEnemyExpBonus;
        }

        public double QuestExpBonus(QuestType questType)
        {
            lock (_CourseBonus)
            {
                switch (questType)
                {
                    case QuestType.World:
                        return _CourseBonus.WorldQuestExpBonus;
                    default:
                        return 0;
                }
            }
        }

        public double QuestExpBonus(uint characterId, QuestType questType)
        {
            var bonus = GetCombinedBonus(characterId);
            switch (questType)
            {
                case QuestType.World:
                    return bonus.WorldQuestExpBonus;
                default:
                    return 0;
            }
        }

        public double QuestApBonus(QuestType questType)
        {
            lock (_CourseBonus)
            {
                switch (questType)
                {
                    case QuestType.World:
                        return _CourseBonus.WorldQuestApBonus;
                    case QuestType.Board:
                        return _CourseBonus.BoardQuestApBonus;
                    default:
                        return 0;
                }
            }
        }

        public double QuestApBonus(uint characterId, QuestType questType)
        {
            var bonus = GetCombinedBonus(characterId);
            switch (questType)
            {
                case QuestType.World:
                    return bonus.WorldQuestApBonus;
                case QuestType.Board:
                    return bonus.BoardQuestApBonus;
                default:
                    return 0;
            }
        }

        public double EnemyPlayPointBonus()
        {
            lock (_CourseBonus)
            {
                return _CourseBonus.EnemyPlayPointBonus;
            }
        }

        public double EnemyPlayPointBonus(uint characterId)
        {
            return GetCombinedBonus(characterId).EnemyPlayPointBonus;
        }

        public double PawnCraftBonus()
        {
            lock (_CourseBonus)
            {
                return _CourseBonus.PawnCraftBonus;
            }
        }

        public double PawnCraftBonus(uint characterId)
        {
            return GetCombinedBonus(characterId).PawnCraftBonus;
        }

        public bool DisablePartyExpAdjustment()
        {
            lock (_CourseBonus)
            {
                return _CourseBonus.DisablePartyExpAdjustment > 0;
            }
        }

        public bool DisablePartyExpAdjustment(uint characterId)
        {
            return GetCombinedBonus(characterId).DisablePartyExpAdjustment > 0;
        }

        public double EnemyBloodOrbBonus()
        {
            lock (_CourseBonus)
            {
                return _CourseBonus.EnemyBloodOrbMultiplier;
            }
        }

        public double EnemyBloodOrbBonus(uint characterId)
        {
            return GetCombinedBonus(characterId).EnemyBloodOrbMultiplier;
        }

        public bool InfiniteReviveRefresh()
        {
            lock (_CourseBonus)
            {
                return _CourseBonus.InfiniteRevive > 0;
            }
        }

        public bool InfiniteReviveRefresh(uint characterId)
        {
            return GetCombinedBonus(characterId).InfiniteRevive > 0;
        }

        public uint BazaarExhibitExtend()
        {
            lock (_CourseBonus)
            {
                return _CourseBonus.BazaarExhibitExtend;
            }
        }

        public uint BazaarExhibitExtend(uint characterId)
        {
            return GetCombinedBonus(characterId).BazaarExhibitExtend;
        }

        public ulong BazaarReExhibitShorten()
        {
            lock (_CourseBonus)
            {
                return _CourseBonus.BazaarReExhibitShorten;
            }
        }

        public ulong BazaarReExhibitShorten(uint characterId)
        {
            return GetCombinedBonus(characterId).BazaarReExhibitShorten;
        }

        public bool AreaMasterSupply()
        {
            lock (_CourseBonus)
            {
                return _CourseBonus.AreaMasterSupply > 0;
            }
        }

        public bool AreaMasterSupply(uint characterId)
        {
            return GetCombinedBonus(characterId).AreaMasterSupply > 0;
        }
    }
}
