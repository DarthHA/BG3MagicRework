using BG3MagicRework.BaseType;
using BG3MagicRework.Projectiles.Ring1;
using BG3MagicRework.Projectiles.VirtualEffects;
using BG3MagicRework.Spells.Reaction;
using BG3MagicRework.Spells.Ring1;
using BG3MagicRework.Static;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Systems
{
    public partial class DNDMagicPlayer : ModPlayer
    {
        /// <summary>
        /// 炼狱叱喝触发
        /// </summary>
        public bool HellishRebukeTrigger = true;
        /// <summary>
        /// 炼狱叱喝冷却
        /// </summary>
        public int HellishRebukeCooldown = 0;

        /// <summary>
        /// 护盾术触发
        /// </summary>
        public bool ShieldTrigger = true;
        /// <summary>
        /// 护盾术冷却
        /// </summary>
        public int ShieldCooldown = 0;
        /// <summary>
        /// 护盾术是否已经触发，以及触发的到底是几环
        /// </summary>
        public int ShieldRingActive = 0;


        /// <summary>
        /// 蜜蜂触发
        /// </summary>
        public bool BeeTrigger = true;
        /// <summary>
        /// 由使用法术触发，触发蜜蜂的持续时间
        /// </summary>
        public int BeeTimer = 0;

        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            if (HellishRebukeCooldown <= 0)
            {
                if (Player.HasReaction<HellishRebukeSpell>() && HellishRebukeTrigger)
                {
                    (bool careful, _, _, _, bool twinned) = Player.ActivateMetaMagic(true, false, false, false, true);
                    int target = npc.whoAmI;
                    if (!Collision.CanHit(Player.Center, 1, 1, Main.npc[target].Center, 1, 1) && !careful) target = -1;
                    if (target != -1)
                    {
                        List<int> availableRings = Player.GetAvailableRings(EverythingLibrary.GetSpell<HellishRebukeSpell>().InitialRing);
                        if (availableRings.Count > 0)
                        {
                            int consumedRing = availableRings[0];
                            bool success = EverythingLibrary.GetSpell<HellishRebukeSpell>().ReactionEffect(Player, consumedRing, target, hurtInfo.Damage, twinned ? 1 : 0, 0);
                            if (success)
                            {
                                HellishRebukeCooldown = EverythingLibrary.GetSpell<HellishRebukeSpell>().ReactionCD;
                                if (!ConsumedSpellSlot.TryAdd(consumedRing, 1))
                                {
                                    ConsumedSpellSlot[consumedRing]++;
                                }
                            }
                            else
                            {
                                if (careful) ConsumedSorceryPoint--;
                                if (twinned) ConsumedSorceryPoint--;
                            }
                        }
                        else
                        {
                            if (careful) ConsumedSorceryPoint--;
                            if (twinned) ConsumedSorceryPoint--;
                        }
                    }
                    else            //没触发就返还术法点
                    {
                        if (careful) ConsumedSorceryPoint--;
                        if (twinned) ConsumedSorceryPoint--;
                    }
                }
            }

            if (ExtraLife > 0 && AoALevel > 0)
            {
                DiceDamage damage = Player.GetDiceDamage(EverythingLibrary.GetSpell<ArmorOfAgathysSpell>().BaseDamage, 1, AoALevel, new());
                int protmp = Player.NewMagicProj(Player.Center, Vector2.Zero, ModContent.ProjectileType<ArmorOfAgathysProj>(), damage, 0, AoALevel);
                if (protmp >= 0 && protmp < 1000)
                {
                    (Main.projectile[protmp].ModProjectile as ArmorOfAgathysProj).TargetNPC = npc.whoAmI;
                }
            }

            if (ExtraLife > 0)
            {
                ExtraLife -= hurtInfo.Damage;
                if (ExtraLife < 0) ExtraLife = 0;
                if (ExtraLife == 0)
                {
                    if (AoALevel > 0)
                    {
                        AdvancedCombatText.NewText(Player.getRect(), Color.White, EverythingLibrary.GetSpell<ArmorOfAgathysSpell>().GetName(), true);
                    }
                }
            }
        }

        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
        {
            if (ShieldCooldown <= 0)
            {
                if (Player.HasReaction<ShieldSpell>() && ShieldTrigger)
                {
                    List<int> availableRings = Player.GetAvailableRings(EverythingLibrary.GetSpell<ShieldSpell>().InitialRing);
                    if (availableRings.Count > 0)
                    {
                        int consumedRing = availableRings[0];
                        bool success = EverythingLibrary.GetSpell<ShieldSpell>().ReactionEffect(Player, consumedRing, 0, 0, 0, 0);
                        if (success)
                        {
                            ShieldRingActive = consumedRing;
                            //在这里写减伤
                            modifiers.SetMaxDamage(1);
                            modifiers.Knockback *= 0;
                            modifiers.DisableDust();
                            modifiers.DisableSound();
                            ShieldCooldown = EverythingLibrary.GetSpell<ShieldSpell>().ReactionCD;
                            if (!ConsumedSpellSlot.TryAdd(consumedRing, 1))
                            {
                                ConsumedSpellSlot[consumedRing]++;
                            }
                        }
                    }
                }
            }
            else
            {
                if (ShieldRingActive > 0)
                {
                    int shield = Player.GetProj(ModContent.ProjectileType<ShieldSpellProj>());
                    if (shield != -1)
                    {
                        Main.projectile[shield].localAI[1] = 20;
                    }
                    //常态减伤
                    modifiers.Knockback *= 0;
                    modifiers.DisableDust();
                    modifiers.DisableSound();
                }
            }
            if (ExtraLife > 0)
            {
                modifiers.DisableSound();
                modifiers.DisableDust();
            }
        }

        public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
        {
            if (ShieldCooldown <= 0)
            {
                if (Player.HasReaction<ShieldSpell>() && ShieldTrigger)
                {
                    List<int> availableRings = Player.GetAvailableRings(EverythingLibrary.GetSpell<ShieldSpell>().InitialRing);
                    if (availableRings.Count > 0)
                    {
                        int consumedRing = availableRings[0];
                        bool success = EverythingLibrary.GetSpell<ShieldSpell>().ReactionEffect(Player, consumedRing, 1, 0, 0, 0);
                        if (success)
                        {
                            ShieldRingActive = consumedRing;
                            //在这里写减伤
                            modifiers.SetMaxDamage(1);
                            modifiers.Knockback *= 0;
                            modifiers.DisableDust();
                            modifiers.DisableSound();
                            ShieldCooldown = EverythingLibrary.GetSpell<ShieldSpell>().ReactionCD;
                            if (!ConsumedSpellSlot.TryAdd(consumedRing, 1))
                            {
                                ConsumedSpellSlot[consumedRing]++;
                            }
                        }
                    }
                }
            }
            else
            {
                if (ShieldRingActive > 0)
                {
                    int shield = Player.GetProj(ModContent.ProjectileType<ShieldSpellProj>());
                    if (shield != -1)
                    {
                        Main.projectile[shield].localAI[1] = 20;
                    }
                    //常态减伤
                    modifiers.Knockback *= 0;
                    modifiers.DisableDust();
                    modifiers.DisableSound();
                }
            }
            if (ExtraLife > 0)
            {
                modifiers.DisableSound();
                modifiers.DisableDust();
            }
        }

        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            if (HellishRebukeCooldown <= 0)
            {
                if (Player.HasReaction<HellishRebukeSpell>() && HellishRebukeTrigger)
                {
                    (bool careful, _, _, _, bool twinned) = Player.ActivateMetaMagic(true, false, false, false, true);
                    int target = proj.GetLastSource();
                    if (target != -1)
                    {
                        if (!Collision.CanHit(Player.Center, 1, 1, Main.npc[target].Center, 1, 1) && !careful) target = -1;
                    }
                    if (target != -1)
                    {
                        List<int> availableRings = Player.GetAvailableRings(EverythingLibrary.GetSpell<HellishRebukeSpell>().InitialRing);
                        if (availableRings.Count > 0)
                        {
                            int consumedRing = availableRings[0];
                            bool success = EverythingLibrary.GetSpell<HellishRebukeSpell>().ReactionEffect(Player, consumedRing, target, hurtInfo.Damage, twinned ? 1 : 0, 0);
                            if (success)
                            {
                                HellishRebukeCooldown = EverythingLibrary.GetSpell<HellishRebukeSpell>().ReactionCD;
                                if (!ConsumedSpellSlot.TryAdd(consumedRing, 1))
                                {
                                    ConsumedSpellSlot[consumedRing]++;
                                }
                            }
                            else
                            {
                                if (careful) ConsumedSorceryPoint--;
                                if (twinned) ConsumedSorceryPoint--;
                            }
                        }
                        else
                        {
                            if (careful) ConsumedSorceryPoint--;
                            if (twinned) ConsumedSorceryPoint--;
                        }
                    }
                    else            //没触发就返还术法点
                    {
                        if (careful) ConsumedSorceryPoint--;
                        if (twinned) ConsumedSorceryPoint--;
                    }
                }
            }

            if (ExtraLife > 0)
            {
                ExtraLife -= hurtInfo.Damage;
                if (ExtraLife < 0) ExtraLife = 0;
                if (ExtraLife == 0)
                {
                    if (AoALevel > 0)
                    {
                        AdvancedCombatText.NewText(Player.getRect(), Color.White, EverythingLibrary.GetSpell<ArmorOfAgathysSpell>().GetName(), true);
                    }
                }
            }
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (proj.ModProjectile != null && proj.ModProjectile is BaseMagicProj)
            {
                if (proj.type != ModContent.ProjectileType<LegionOfBeesProj>() && proj.type != ModContent.ProjectileType<RotateBees>())
                {
                    if (Player.HasReaction<LegionOfBeesSpell>() && BeeTrigger && BeeTimer > 0)
                    {
                        (bool careful, bool _, bool _, bool _, bool _) = Player.ActivateMetaMagic(true, false, false, false, false);       //只用来判定穿墙超魔
                        if (careful || Collision.CanHit(Player.Center, 1, 1, target.Center, 1, 1))
                        {
                            List<int> availableRings = Player.GetAvailableRings(EverythingLibrary.GetSpell<LegionOfBeesSpell>().InitialRing);
                            if (availableRings.Count > 0)
                            {
                                int consumedRing = availableRings[0];
                                bool success = EverythingLibrary.GetSpell<LegionOfBeesSpell>().ReactionEffect(Player, consumedRing, target.whoAmI, careful ? 1 : 0, 0, 0);
                                if (success)
                                {
                                    BeeTimer = 0;
                                    if (!ConsumedSpellSlot.TryAdd(consumedRing, 1))
                                    {
                                        ConsumedSpellSlot[consumedRing]++;
                                    }
                                }
                                else
                                {
                                    if (careful) ConsumedSorceryPoint--;
                                }
                            }
                            else
                            {
                                if (careful) ConsumedSorceryPoint--;
                            }
                        }
                        else            //没触发就返还术法点
                        {
                            if (careful) ConsumedSorceryPoint--;
                        }

                    }
                }
            }
        }

        public override bool ConsumableDodge(Player.HurtInfo info)
        {
            if (info.Damage > 1 && MirrorImageCount > 0)
            {
                float chance = MirrorImageCount * 0.2f;
                if (Main.rand.NextFloat() <= chance)
                {
                    MirrorImageCount--;
                    info.DustDisabled = true;
                    info.SoundDisabled = true;
                    info.Knockback *= 0;
                    Player.SetImmuneTimeForAllTypes(Player.longInvince ? 120 : 80);
                    if (Player.GetProj(ModContent.ProjectileType<MirrorShadow>()) != -1)
                    {
                        (Main.projectile[Player.GetProj(ModContent.ProjectileType<MirrorShadow>())].ModProjectile as MirrorShadow).DestroyAShadow();
                    }
                    return true;
                }
            }
            return false;
        }
    }
}
