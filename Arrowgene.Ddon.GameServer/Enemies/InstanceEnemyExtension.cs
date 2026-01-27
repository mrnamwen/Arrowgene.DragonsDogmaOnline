using Arrowgene.Ddon.GameServer.Scripting;
using Arrowgene.Ddon.Shared.Model;

namespace Arrowgene.Ddon.GameServer.Enemies
{
    public static class InstancedEnemyExtensions
    {
        public static InstancedEnemy SetNamedEnemyParams(this InstancedEnemy enemy, uint namedParamId)
        {
            return enemy.SetNamedEnemyParams(LibDdon.Enemy.GetNamedParam(namedParamId));
        }

        /// <summary>
        /// Sets the HP rate for this enemy. Creates a custom NamedParam if needed.
        /// </summary>
        /// <param name="hpRate">HP rate as percentage (100 = normal, 200 = double HP)</param>
        public static InstancedEnemy SetHpRate(this InstancedEnemy enemy, uint hpRate)
        {
            var currentParams = enemy.NamedEnemyParams;
            var newParams = new NamedParam
            {
                Id = currentParams.Id,
                Type = currentParams.Type,
                HpRate = hpRate,
                Experience = currentParams.Experience,
                AttackBasePhys = currentParams.AttackBasePhys,
                AttackWepPhys = currentParams.AttackWepPhys,
                DefenceBasePhys = currentParams.DefenceBasePhys,
                DefenceWepPhys = currentParams.DefenceWepPhys,
                AttackBaseMagic = currentParams.AttackBaseMagic,
                AttackWepMagic = currentParams.AttackWepMagic,
                DefenceBaseMagic = currentParams.DefenceBaseMagic,
                DefenceWepMagic = currentParams.DefenceWepMagic,
                Power = currentParams.Power,
                GuardDefenceBase = currentParams.GuardDefenceBase,
                GuardDefenceWep = currentParams.GuardDefenceWep,
                ShrinkEnduranceMain = currentParams.ShrinkEnduranceMain,
                BlowEnduranceMain = currentParams.BlowEnduranceMain,
                DownEnduranceMain = currentParams.DownEnduranceMain,
                ShakeEnduranceMain = currentParams.ShakeEnduranceMain,
                HpSub = currentParams.HpSub,
                ShrinkEnduranceSub = currentParams.ShrinkEnduranceSub,
                BlowEnduranceSub = currentParams.BlowEnduranceSub,
                OcdEndurance = currentParams.OcdEndurance,
                AilmentDamage = currentParams.AilmentDamage,
            };
            return enemy.SetNamedEnemyParams(newParams);
        }

        /// <summary>
        /// Sets the attack rate for this enemy (both physical and magical).
        /// </summary>
        /// <param name="attackRate">Attack rate as percentage (100 = normal, 150 = 50% stronger)</param>
        public static InstancedEnemy SetAttackRate(this InstancedEnemy enemy, uint attackRate)
        {
            var currentParams = enemy.NamedEnemyParams;
            var newParams = new NamedParam
            {
                Id = currentParams.Id,
                Type = currentParams.Type,
                HpRate = currentParams.HpRate,
                Experience = currentParams.Experience,
                AttackBasePhys = attackRate,
                AttackWepPhys = attackRate,
                DefenceBasePhys = currentParams.DefenceBasePhys,
                DefenceWepPhys = currentParams.DefenceWepPhys,
                AttackBaseMagic = attackRate,
                AttackWepMagic = attackRate,
                DefenceBaseMagic = currentParams.DefenceBaseMagic,
                DefenceWepMagic = currentParams.DefenceWepMagic,
                Power = currentParams.Power,
                GuardDefenceBase = currentParams.GuardDefenceBase,
                GuardDefenceWep = currentParams.GuardDefenceWep,
                ShrinkEnduranceMain = currentParams.ShrinkEnduranceMain,
                BlowEnduranceMain = currentParams.BlowEnduranceMain,
                DownEnduranceMain = currentParams.DownEnduranceMain,
                ShakeEnduranceMain = currentParams.ShakeEnduranceMain,
                HpSub = currentParams.HpSub,
                ShrinkEnduranceSub = currentParams.ShrinkEnduranceSub,
                BlowEnduranceSub = currentParams.BlowEnduranceSub,
                OcdEndurance = currentParams.OcdEndurance,
                AilmentDamage = currentParams.AilmentDamage,
            };
            return enemy.SetNamedEnemyParams(newParams);
        }

        /// <summary>
        /// Sets the magic attack rate for this enemy (separate from physical).
        /// </summary>
        /// <param name="magicAttackRate">Magic attack rate as percentage (100 = normal, 150 = 50% stronger)</param>
        public static InstancedEnemy SetMagicAttackRate(this InstancedEnemy enemy, uint magicAttackRate)
        {
            var currentParams = enemy.NamedEnemyParams;
            var newParams = new NamedParam
            {
                Id = currentParams.Id,
                Type = currentParams.Type,
                HpRate = currentParams.HpRate,
                Experience = currentParams.Experience,
                AttackBasePhys = currentParams.AttackBasePhys,
                AttackWepPhys = currentParams.AttackWepPhys,
                DefenceBasePhys = currentParams.DefenceBasePhys,
                DefenceWepPhys = currentParams.DefenceWepPhys,
                AttackBaseMagic = magicAttackRate,
                AttackWepMagic = magicAttackRate,
                DefenceBaseMagic = currentParams.DefenceBaseMagic,
                DefenceWepMagic = currentParams.DefenceWepMagic,
                Power = currentParams.Power,
                GuardDefenceBase = currentParams.GuardDefenceBase,
                GuardDefenceWep = currentParams.GuardDefenceWep,
                ShrinkEnduranceMain = currentParams.ShrinkEnduranceMain,
                BlowEnduranceMain = currentParams.BlowEnduranceMain,
                DownEnduranceMain = currentParams.DownEnduranceMain,
                ShakeEnduranceMain = currentParams.ShakeEnduranceMain,
                HpSub = currentParams.HpSub,
                ShrinkEnduranceSub = currentParams.ShrinkEnduranceSub,
                BlowEnduranceSub = currentParams.BlowEnduranceSub,
                OcdEndurance = currentParams.OcdEndurance,
                AilmentDamage = currentParams.AilmentDamage,
            };
            return enemy.SetNamedEnemyParams(newParams);
        }

        /// <summary>
        /// Sets the defense rate for this enemy (both physical and magical).
        /// British spelling alias for SetDefenseRate.
        /// </summary>
        public static InstancedEnemy SetDefenceRate(this InstancedEnemy enemy, uint defenceRate)
        {
            return enemy.SetDefenseRate(defenceRate);
        }

        /// <summary>
        /// Sets the magic defense rate for this enemy (separate from physical).
        /// </summary>
        public static InstancedEnemy SetMagicDefenseRate(this InstancedEnemy enemy, uint magicDefenseRate)
        {
            var currentParams = enemy.NamedEnemyParams;
            var newParams = new NamedParam
            {
                Id = currentParams.Id,
                Type = currentParams.Type,
                HpRate = currentParams.HpRate,
                Experience = currentParams.Experience,
                AttackBasePhys = currentParams.AttackBasePhys,
                AttackWepPhys = currentParams.AttackWepPhys,
                DefenceBasePhys = currentParams.DefenceBasePhys,
                DefenceWepPhys = currentParams.DefenceWepPhys,
                AttackBaseMagic = currentParams.AttackBaseMagic,
                AttackWepMagic = currentParams.AttackWepMagic,
                DefenceBaseMagic = magicDefenseRate,
                DefenceWepMagic = magicDefenseRate,
                Power = currentParams.Power,
                GuardDefenceBase = currentParams.GuardDefenceBase,
                GuardDefenceWep = currentParams.GuardDefenceWep,
                ShrinkEnduranceMain = currentParams.ShrinkEnduranceMain,
                BlowEnduranceMain = currentParams.BlowEnduranceMain,
                DownEnduranceMain = currentParams.DownEnduranceMain,
                ShakeEnduranceMain = currentParams.ShakeEnduranceMain,
                HpSub = currentParams.HpSub,
                ShrinkEnduranceSub = currentParams.ShrinkEnduranceSub,
                BlowEnduranceSub = currentParams.BlowEnduranceSub,
                OcdEndurance = currentParams.OcdEndurance,
                AilmentDamage = currentParams.AilmentDamage,
            };
            return enemy.SetNamedEnemyParams(newParams);
        }

        /// <summary>
        /// Sets the magic defense rate (British spelling alias).
        /// </summary>
        public static InstancedEnemy SetMagicDefenceRate(this InstancedEnemy enemy, uint magicDefenceRate)
        {
            return enemy.SetMagicDefenseRate(magicDefenceRate);
        }

        /// <summary>
        /// Sets the defense rate for this enemy (both physical and magical).
        /// </summary>
        /// <param name="defenseRate">Defense rate as percentage (100 = normal, 130 = 30% tougher)</param>
        public static InstancedEnemy SetDefenseRate(this InstancedEnemy enemy, uint defenseRate)
        {
            var currentParams = enemy.NamedEnemyParams;
            var newParams = new NamedParam
            {
                Id = currentParams.Id,
                Type = currentParams.Type,
                HpRate = currentParams.HpRate,
                Experience = currentParams.Experience,
                AttackBasePhys = currentParams.AttackBasePhys,
                AttackWepPhys = currentParams.AttackWepPhys,
                DefenceBasePhys = defenseRate,
                DefenceWepPhys = defenseRate,
                AttackBaseMagic = currentParams.AttackBaseMagic,
                AttackWepMagic = currentParams.AttackWepMagic,
                DefenceBaseMagic = defenseRate,
                DefenceWepMagic = defenseRate,
                Power = currentParams.Power,
                GuardDefenceBase = currentParams.GuardDefenceBase,
                GuardDefenceWep = currentParams.GuardDefenceWep,
                ShrinkEnduranceMain = currentParams.ShrinkEnduranceMain,
                BlowEnduranceMain = currentParams.BlowEnduranceMain,
                DownEnduranceMain = currentParams.DownEnduranceMain,
                ShakeEnduranceMain = currentParams.ShakeEnduranceMain,
                HpSub = currentParams.HpSub,
                ShrinkEnduranceSub = currentParams.ShrinkEnduranceSub,
                BlowEnduranceSub = currentParams.BlowEnduranceSub,
                OcdEndurance = currentParams.OcdEndurance,
                AilmentDamage = currentParams.AilmentDamage,
            };
            return enemy.SetNamedEnemyParams(newParams);
        }
    }
}
