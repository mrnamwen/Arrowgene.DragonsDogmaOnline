/**
 * ============================================================================
 * SOLO PLAY - SEASONAL EVENT SETTINGS
 * ============================================================================
 *
 * All seasonal events are enabled year-round for solo players.
 * This allows experiencing all event content at any time.
 * ============================================================================
 */

// ============================================================================
// HALLOWEEN EVENT - YEAR-ROUND
// ============================================================================

bool EnableHalloweenEvent = true;

/** Extended to full year (Jan 1 - Dec 31) */
var HalloweenValidPeriod = LibUtils.EventTimespan("1/1", "12/31");

/** Use the 2018 version with candy collection quests */
uint HalloweenEventYear = 2018;

// ============================================================================
// CHRISTMAS EVENT - YEAR-ROUND
// ============================================================================

bool EnableChristmasEvent = true;

/** Extended to full year (Jan 1 - Dec 31) */
var ChristmasValidPeriod = LibUtils.EventTimespan("1/1", "12/31");

/** Use the 2018 version */
uint ChristmasEventYear = 2018;

// ============================================================================
// VALENTINES EVENT - YEAR-ROUND
// ============================================================================

bool EnableValentinesEvent = true;

/** Extended to full year (Jan 1 - Dec 31) */
var ValentinesValidPeriod = LibUtils.EventTimespan("1/1", "12/31");

/** Use the 2017 version */
uint ValentinesEventYear = 2017;

// ============================================================================
// SUMMER EVENT - YEAR-ROUND
// ============================================================================

bool EnableSummerEvent = true;

/** Extended to full year (Jan 1 - Dec 31) */
var SummerEventValidPeriod = LibUtils.EventTimespan("1/1", "12/31");

/** Use the 2018 Beach Festival version */
uint SummerEventYear = 2018;
