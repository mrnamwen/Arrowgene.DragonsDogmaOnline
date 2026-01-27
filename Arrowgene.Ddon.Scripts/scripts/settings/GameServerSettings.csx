/**
 * ============================================================================
 * SOLO PLAY CONFIGURATION
 * ============================================================================
 *
 * This configuration optimizes the game for single-player / solo play:
 * - Significantly reduced grind through increased XP and reward multipliers
 * - All premium features enabled for free (via GpCourseInfo.json Course 15)
 * - Faster crafting and cultivation times
 * - Free cosmetic services (beauty parlor, reincarnation)
 * - NPC Bazaar enabled for solo trading
 * - Pawns receive quest rewards
 * - Reduced/eliminated tool breakage
 *
 * Note: This is designed for a relaxed solo experience. Adjust values as needed.
 * ============================================================================
 */

// ============================================================================
// EXPERIENCE & PROGRESSION MULTIPLIERS
// ============================================================================
// These dramatically reduce the grind. Retail DDON was notoriously grindy.
// A 3x multiplier roughly brings it in line with single-player RPG pacing.

/** Global modifier for enemy EXP - 3x default */
double EnemyExpModifier = 3.0;

/** Global modifier for BBM (Bitterblack Maze) enemy EXP - 3x default */
double BBMEnemyExpModifier = 3.0;

/** Global modifier for quest EXP rewards - 3x default */
double QuestExpModifier = 3.0;

/** Global modifier for Job Points - 3x to keep skill progression in sync */
double JpModifier = 3.0;

/** Global modifier for Play Points - 2x default */
double PpModifier = 2.0;

/** Global modifier for Area Points - 2x default */
double ApModifier = 2.0;

// ============================================================================
// CURRENCY MULTIPLIERS
// ============================================================================

/** Global modifier for Gold drops/rewards - 2x default */
double GoldModifier = 2.0;

/** Global modifier for Rift Points - 2x default */
double RiftModifier = 2.0;

/** Global modifier for Blood Orbs - 2x default */
double BoModifier = 2.0;

/** Global modifier for High Orbs - 2x default */
double HoModifier = 2.0;

// ============================================================================
// CRAFTING & TIME GATING
// ============================================================================
// Significantly reduce crafting times for solo play convenience

/** Crafting speed factor - 0.25 = 4x faster crafting */
double AdditionalProductionSpeedFactor = 0.25;

/** Mandragora crafting speed - 0.1 = 10x faster */
double MandragoraCraftSpeedFactor = 0.1;

/** Mandragora cultivation speed - 0.1 = 10x faster (1.8 hours instead of 18) */
double MandragoraCultivationSpeedFactor = 0.1;

/** Maximum items to recycle/disassemble before reset - increased from 10 */
byte CraftItemRecycleMax = 99;

/** Maximum consumables craftable in one batch - increased from 10 */
byte CraftConsumableProductionTimesMax = 99;

/** Chance of crafting great success - 25% instead of 10% */
int CraftGreatSuccessBaseOdds = 25;

// ============================================================================
// COSMETIC SERVICES - FREE
// ============================================================================

/** Beauty Parlor GG cost - FREE */
uint BeautyParlorGGPrice = 0;

/** Beauty Parlor Silver Ticket cost - FREE */
uint BeautyParlorSTPrice = 0;

/** Reincarnation GG cost - FREE */
uint ReincarnationGGPrice = 0;

/** Mandragora cultivation skip cost - FREE */
uint MandragoraCultivationSkipGGCost = 0;

/** BBM reset GG cost - FREE */
uint BBMResetGGCost = 0;

/** Crafting recycle reset cost - FREE */
byte CraftItemRecycleResetGGCost = 0;

// ============================================================================
// PAWN & PARTY SETTINGS
// ============================================================================

/** Allow pawns to skip job training requirements */
bool PawnSkipJobTraining = true;

/** Main pawns receive quest rewards (originally false in retail) */
bool EnableMainPartyPawnsQuestRewards = true;

/** Rental pawn adventure charges - increased from 10 */
byte RentalPawnAdventureCount = 99;

/** Rental pawn craft charges - increased from 10 */
byte RentalPawnCraftCount = 99;

/** Rental points to JP conversion rate - 1:1 instead of 10:1 */
uint RentalPointConversionRate = 1;

// ============================================================================
// BAZAAR & TRADING
// ============================================================================
// NPC Bazaar already enabled by default for solo trading

/** Default bazaar exhibit slots for new characters - increased from 5 */
uint DefaultMaxBazaarExhibits = 20;

/** Bazaar max price per item - increased substantially */
uint BazaarExhibitionMaxPrice = 99999999;

/** No bazaar cooldown - instant re-listing */
ulong BazaarCooldownTimeSeconds = 0;

// ============================================================================
// QUALITY OF LIFE
// ============================================================================

/** Enable autoloot for enemy drops (already enabled by default) */
bool EnableAutoloot = true;

/** Send crafting materials directly to storage when autolooting */
bool AutolootMaterialsToStorage = true;

/** Autoloot items from treasure chests and gathering points */
bool AutolootGatheringItems = true;

/** Chat type for storage autoloot notifications
 *  3 = System (white), 9 = ManagementGuideC (gold), 11 = ManagementAlertC (red) */
byte AutolootNotificationChatType = 9; // Gold

/** Enable tool-based gathering drops (chests, mining, etc.) */
bool EnableToolGatheringDrops = true;

/** Default warp favorites - increased from 5 */
uint DefaultWarpFavorites = 20;

/** Maximum ordered quests - increased from 20 */
byte QuestOrderMax = 50;

/** Reward box slots - increased from 100 */
byte RewardBoxMax = 200;

/** Lantern burn time - 1 hour instead of 25 minutes */
uint LanternBurnTimeInSeconds = 3600;

/** Epitaph weekly reward restriction - disabled for solo */
bool EnableEpitaphWeeklyRewards = false;

/** BBM weekly reset tickets - increased */
uint BBMWeeklyResetTickets = 10;

/** BBM weekly GG resets - unlimited */
uint BBMWeeklyGGResets = 99;

// ============================================================================
// GATHERING TOOL DURABILITY
// ============================================================================
// Reduce or eliminate tool breakage for convenience

var ToolBreakChance = new Dictionary<ItemId, double>
{
    [ItemId.Pickaxe] = 0.05,           // 5% instead of 30%
    [ItemId.EnhancedPickaxe] = 0.02,   // 2% instead of 20%
    [ItemId.ArtisansPickaxe] = 0.0,    // Never breaks
    [ItemId.LumberKnife] = 0.05,
    [ItemId.EnhancedLumberKnife] = 0.02,
    [ItemId.ArtisansLumberKnife] = 0.0,
    [ItemId.Lockpick] = 0.05,
    [ItemId.EnhancedLockpick] = 0.02,
    [ItemId.AllPurposeLockpick] = 0.0,
};

// ============================================================================
// MISCELLANEOUS
// ============================================================================

/** Enable high orb conversion - allows BO to HO conversion */
bool EnableHighOrbConversion = true;

/** High orb conversion rate - 50 BO per HO instead of 100 */
uint HighOrbConversionRate = 50;

/** Area boss AP reward - doubled */
uint AreaBossApReward = 1000;

/** Login announcement message */
string LoginAnnouncementMessage = "Welcome to DDON Solo Mode! All premium features are enabled. Enjoy your adventure!";
