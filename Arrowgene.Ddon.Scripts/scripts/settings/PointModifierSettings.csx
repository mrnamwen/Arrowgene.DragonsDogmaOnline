/**
 * ============================================================================
 * SOLO PLAY - POINT MODIFIER SETTINGS
 * ============================================================================
 *
 * Disables EXP penalties that exist for multiplayer balancing.
 * In solo play, these penalties are unnecessary and frustrating.
 * ============================================================================
 */

// ============================================================================
// PARTY EXP ADJUSTMENTS - DISABLED
// ============================================================================

/**
 * Disable party-based EXP penalties.
 * Originally penalized parties with large level gaps.
 * Not needed for solo play.
 */
bool EnableAdjustPartyEnemyExp = false;

/**
 * Disable target level EXP penalties.
 * Originally penalized killing enemies much lower level than you.
 * Disabled for solo convenience.
 */
bool EnableAdjustTargetLvEnemyExp = false;

/**
 * Player's own pawns excluded from EXP penalty calculations.
 * (Keep enabled as a safety)
 */
bool DisableExpCorrectionForMyPawn = true;

// ============================================================================
// PAWN CATCH-UP MECHANICS - ENHANCED
// ============================================================================

/**
 * Enable pawn catch-up bonus for underleveled pawns.
 */
bool EnablePawnCatchup = true;

/**
 * Pawn catch-up multiplier - 3x instead of 1.5x
 * Helps pawns level faster when behind.
 */
double PawnCatchupMultiplier = 3.0;

/**
 * Level difference required for catch-up - reduced from 5 to 3
 * Catch-up kicks in sooner.
 */
uint PawnCatchupLvDiff = 3;
