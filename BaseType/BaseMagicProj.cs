using BG3MagicRework.Buffs;
using BG3MagicRework.Buffs.Enemy;
using BG3MagicRework.Static;
using BG3MagicRework.Systems;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.BaseType
{
    public abstract class BaseMagicProj : ModProjectile
    {
        public override string Texture => "BG3MagicRework/Images/PlaceHolder";
        /// <summary>
        /// 该法术弹幕的UUID
        /// </summary>
        public long ConUUID = 0;
        /// <summary>
        /// 该法术弹幕环数
        /// </summary>
        public int CurrentRing = 0;
        /// <summary>
        /// 用于用弹幕触发专注时给予持续时间
        /// </summary>

        /// <summary>
        /// 远距法术超魔
        /// </summary>
        public bool DistantSpellMM = false;
        /// <summary>
        /// 延长法术超魔
        /// </summary>
        public bool ExtendedSpellMM = false;
        /// <summary>
        /// 谨慎法术超魔
        /// </summary>
        public bool CarefulSpellMM = false;
        /// <summary>
        /// 升阶法术超魔
        /// </summary>
        public bool HeightenedSpellMM = false;
        /// <summary>
        /// 孪生法术超魔
        /// </summary>
        public bool TwinnedSpellMM = false;

        /// <summary>
        /// 豁免难度
        /// </summary>
        public int DifficultyClass = -1;

        /// <summary>
        /// 该法术弹幕的主伤害
        /// </summary>
        public DiceDamage diceDamage = new();
        /// <summary>
        /// 该法术弹幕的副伤害
        /// </summary>
        public DiceDamage extraDiceDamage = new();

        /// <summary>
        /// 该法术可命中敌人次数,-1为无限命中
        /// </summary>
        public virtual int MaxHits => -1;

        private int numOfHit = 0;

        /// <summary>
        /// 该法术移动的距离
        /// </summary>
        protected float TravelDistance = 0;

        public override void PostAI()
        {
            if (ShouldUpdatePosition())
            {
                TravelDistance += Projectile.velocity.Length();
            }
        }
        public sealed override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.DamageVariationScale *= 0;
            modifiers.DefenseEffectiveness *= 0;
            DiceDamage diceUsed = diceDamage;
            Dictionary<DamageElement, float> Resistance = target.GetResistance();
            float damageModifier = 1f;
            if (Main.player[Projectile.owner].HasBuff(ModContent.BuffType<BlindedDNDBuff_Player>()))
            {
                damageModifier *= 0.5f;
            }
            SafeModifyHit(target, ref modifiers, ref diceUsed, ref damageModifier, ref Resistance);
            if (target.GetGlobalNPC<EnemyStatusSystem>().FireAndColdStatus == FireAndColdStatusID.Burning) diceUsed = diceUsed.FireAdd1D4Fire();
            if (Main.player[Projectile.owner].HasBuff(ModContent.BuffType<ArcaneHungerBuff>())) damageModifier *= 0.1f;
            int damage;
            bool crit = false;
            if (TwinnedSpellMM || target.HasBuff(ModContent.BuffType<GuidingBoltBuff>()) || target.HasBuff(ModContent.BuffType<SleepDNDBuff>()) || target.HasBuff(ModContent.BuffType<FaerieFireBuff>()))
            {
                int d1 = diceUsed.Damage(out bool crit1, Resistance);
                int d2 = diceUsed.Damage(out bool crit2, Resistance);
                damage = (int)(Math.Max(d1, d2) * damageModifier);
                crit = crit1 || crit2;
            }
            else
            {
                damage = (int)(diceUsed.Damage(out crit, Resistance) * damageModifier);
            }
            modifiers.ModifyHitInfo += (ref NPC.HitInfo info) =>
            {
                info.Damage = damage;
            };
            if (ModContent.GetInstance<BG3Config>().ShowCombatInfo)
            {
                if (!diceUsed.NoDamage())
                    Main.NewText(string.Format(LangLibrary.XDealXDamageToX,
                        Main.player[Projectile.owner].name,
                        Lang.GetNPCName(target.type),
                        diceUsed.ShowCalc(),
                        damageModifier,
                        damage
                        ));
            }
            if (crit)         //大成功变为重击（暴击）特效
            {
                modifiers.SetCrit();
            }
            else
            {
                modifiers.DisableCrit();
            }

            //在这里造成伤害效果，比如打油上火
            if (diceUsed.HasElement(DamageElement.Fire) && target.HasBuff(ModContent.BuffType<GreaseBuff>()))
            {
                target.DeepAddBurning(CurrentRing == 0 ? CombatStat.BurningTimeCantrip : CombatStat.BurningTime);
            }
            //冰冻效果被特殊伤害类型解除
            if (!diceUsed.NoDamage() && target.GetGlobalNPC<EnemyStatusSystem>().FireAndColdStatus == FireAndColdStatusID.Frozen)
            {
                if (diceUsed.HasElement(DamageElement.Force) || diceUsed.HasElement(DamageElement.None) || diceUsed.HasElement(DamageElement.Thunder))
                {
                    target.DeepClearFrozen(0);
                }
            }

        }

        public virtual void SafeModifyHit(NPC target, ref NPC.HitModifiers modifiers, ref DiceDamage diceUsed, ref float damageModifier, ref Dictionary<DamageElement, float> resistance)
        {

        }

        public sealed override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            numOfHit++;
            SafeOnHit(target, hit, damageDone);
        }

        public sealed override bool? CanHitNPC(NPC target)
        {
            if (numOfHit >= MaxHits && MaxHits != -1) return false;
            return SafeCanHitNPC(target);
        }

        public virtual bool? SafeCanHitNPC(NPC target)
        {
            return null;
        }

        public virtual void SafeOnHit(NPC target, NPC.HitInfo hit, int damageDone)
        {

        }

        public int GetSpellRange(string spellName)
        {
            int a = DistantSpellMM ? 2 : 1;
            return EverythingLibrary.spells[spellName].SpellRange * a;
        }

        public int GetSpellRange<T>() where T : BaseSpell
        {
            int a = DistantSpellMM ? 2 : 1;
            return EverythingLibrary.GetSpell<T>().SpellRange * a;
        }

        public int GetAOERadius<T>() where T : BaseSpell
        {
            return EverythingLibrary.GetSpell<T>().AOERadius;
        }

        public int GetTimeSpan<T>() where T : BaseSpell
        {
            int a = ExtendedSpellMM ? 2 : 1;
            return EverythingLibrary.GetSpell<T>().TimeSpan * a;
        }

    }
}
