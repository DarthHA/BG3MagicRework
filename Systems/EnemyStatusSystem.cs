using BG3MagicRework.BaseType;
using BG3MagicRework.Buffs.Enemy;
using BG3MagicRework.Projectiles.VirtualEffects;
using BG3MagicRework.Static;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Systems
{
    public class EnemyStatusSystem : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        /// <summary>
        /// 行动速度
        /// </summary>
        public float SpeedFactor = 1f;
        /// <summary>
        /// AI速度
        /// </summary>
        public float AIFactor = 1f;

        /// <summary>
        /// 累计行动数，用于非整数AI速度
        /// </summary>
        public float accumulatedActions = 0;

        public Vector2? SavedVelocity = null;

        public FireAndColdStatusID FireAndColdStatus = FireAndColdStatusID.None;

        public int FireAndColdStatusTime = 0;

        public Dictionary<DamageElement, float> InitialResistance = new();

        public override bool PreAI(NPC npc)
        {
            if (accumulatedActions >= 1f)
            {
                if (SavedVelocity.HasValue)
                {
                    npc.velocity = SavedVelocity.Value;
                    SavedVelocity = null;
                }
            }
            else
            {
                if (SavedVelocity == null)
                {
                    SavedVelocity = npc.velocity;
                    npc.velocity = Vector2.Zero;
                }
                return false;
            }
            return true;
        }

        public override void ModifyHitNPC(NPC npc, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (npc.HasBuff<BlindedDNDBuff>())
            {
                modifiers.FinalDamage *= 0.5f;
            }
        }

        public override void ModifyHitPlayer(NPC npc, Player target, ref Player.HurtModifiers modifiers)
        {
            if (npc.HasBuff<BlindedDNDBuff>())
            {
                modifiers.FinalDamage *= 0.5f;
            }
        }

        public override void OnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            if (damageDone > 1)
            {
                npc.DeepClearCCBuff(ModContent.BuffType<SleepDNDBuff>());
            }
        }

        public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (damageDone > 1)
            {
                npc.DeepClearCCBuff(ModContent.BuffType<SleepDNDBuff>());
            }
        }

        /// <summary>
        /// 更新环境buff
        /// </summary>
        /// <param name="npc"></param>
        /// <param name="gnpc"></param>
        public static void UpdateFireAndColdStatus(NPC npc, EnemyStatusSystem gnpc)
        {
            if (gnpc.FireAndColdStatusTime > 0) gnpc.FireAndColdStatusTime--;

            if (gnpc.FireAndColdStatusTime == 0)      //持续时间结束后清除所有效果
            {
                switch (gnpc.FireAndColdStatus)
                {
                    case FireAndColdStatusID.Burning:
                        AdvancedCombatText.NewText(npc.getRect(), Color.White, Lang.GetBuffName(ModContent.BuffType<BurningDNDBuff>()), true);
                        break;
                    case FireAndColdStatusID.Wet:
                        AdvancedCombatText.NewText(npc.getRect(), Color.White, Lang.GetBuffName(ModContent.BuffType<WetDNDBuff>()), true);
                        break;
                    case FireAndColdStatusID.Chilled:
                        AdvancedCombatText.NewText(npc.getRect(), Color.White, Lang.GetBuffName(ModContent.BuffType<ChilledDNDBuff>()), true);
                        break;
                    case FireAndColdStatusID.Frozen:
                        AdvancedCombatText.NewText(npc.getRect(), Color.White, Lang.GetBuffName(ModContent.BuffType<FrozenDNDBuff>()), true);
                        break;
                }
                gnpc.FireAndColdStatus = FireAndColdStatusID.None;
            }
            if (SomeUtils.WaterCollision(npc) && !SomeUtils.LavaCollision(npc))           //进水湿润且灭火
            {
                npc.DeepAddWet(60, false);
            }
            if (SomeUtils.LavaCollision(npc) && !SomeUtils.WaterCollision(npc))           //进岩浆灭水灭冰
            {
                npc.DeepClearWetAndChilled(0);
            }
            if (npc.HasBuff(ModContent.BuffType<GreaseBuff>()))
            {
                npc.oiled = true;
            }

            //上特效buff
            if (gnpc.FireAndColdStatus == FireAndColdStatusID.Chilled)
            {
                npc.buffImmune[ModContent.BuffType<ChilledDNDBuff>()] = false;
                npc.AddBuff(ModContent.BuffType<ChilledDNDBuff>(), 2);
            }
            if (gnpc.FireAndColdStatus == FireAndColdStatusID.Wet)
            {
                npc.buffImmune[ModContent.BuffType<WetDNDBuff>()] = false;
                npc.AddBuff(ModContent.BuffType<WetDNDBuff>(), 2);
            }
            if (gnpc.FireAndColdStatus == FireAndColdStatusID.Burning)
            {
                npc.buffImmune[ModContent.BuffType<BurningDNDBuff>()] = false;
                npc.AddBuff(ModContent.BuffType<BurningDNDBuff>(), 2);
            }
        }


        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (FireAndColdStatus == FireAndColdStatusID.Frozen)
            {
                EasyDraw.AnotherDraw(SpriteSortMode.Immediate);
                Color color = Lighting.GetColor((int)(npc.Center.X / 16f), (int)(npc.Center.Y / 16f), Color.White);
                EffectLibrary.FrozenEffect.Parameters["uOpacity"].SetValue(npc.Opacity);
                EffectLibrary.FrozenEffect.Parameters["uTime"].SetValue(0f);
                EffectLibrary.FrozenEffect.Parameters["uIntensity"].SetValue(1f);
                EffectLibrary.FrozenEffect.CurrentTechnique.Passes[0].Apply();
                EffectLibrary.FrozenEffect.Parameters["lightColor"].SetValue(color.ToVector4());
                Main.spriteBatch.GraphicsDevice.Textures[1] = TextureLibrary.Crystal;
                EffectLibrary.FrozenEffect.CurrentTechnique.Passes[0].Apply();
            }
            return true;
        }

        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (FireAndColdStatus == FireAndColdStatusID.Frozen)
            {
                EasyDraw.AnotherDraw(SpriteSortMode.Deferred);
            }
        }


        /// <summary>
        /// 修改移动速度和AI速度的钩子
        /// </summary>
        /// <param name="npc"></param>
        /// <param name="gnpc"></param>
        public static void UpdateNPCSpeedAndAIModifier(NPC npc, EnemyStatusSystem gnpc)
        {
            if (npc.HasBuff(ModContent.BuffType<SleepDNDBuff>()))
            {
                gnpc.AIFactor = 0f;
            }
            if (gnpc.FireAndColdStatus == FireAndColdStatusID.Frozen)
            {
                gnpc.AIFactor = 0f;
            }
            if (gnpc.FireAndColdStatus == FireAndColdStatusID.Chilled)      //两种寒冷减速不叠加
            {
                gnpc.SpeedFactor *= 0.5f;
            }
            else if (npc.HasBuff(ModContent.BuffType<IceSlowBuff>()))
            {
                gnpc.SpeedFactor *= 0.75f;
            }
            if (npc.HasBuff(ModContent.BuffType<SpiritGuardiansSlowBuff>()))
            {
                gnpc.SpeedFactor *= 0.5f;
            }
            if (npc.HasBuff<DisadvantageTerrainBuff2>())
            {
                gnpc.SpeedFactor *= 0.25f;
            }
            else if (npc.HasBuff<DisadvantageTerrainBuff>())
            {
                gnpc.SpeedFactor *= 0.5f;
            }
                if (npc.HasBuff(ModContent.BuffType<FearDNDBuff>()))
            {
                gnpc.SpeedFactor *= 0.001f;
            }
        }

        /// <summary>
        /// 重置AI速度和移动速度的钩子
        /// </summary>
        /// <param name="npc"></param>
        /// <param name="gnpc"></param>
        public static void ResetNPCSpeedAndAIModifier(NPC npc, EnemyStatusSystem gnpc)
        {
            gnpc.SpeedFactor = 1;
            gnpc.AIFactor = 1;
        }

        #region 修改AI和移动速度


        public override void Load()
        {
            IL_NPC.UpdateNPC_Inner += IL_NPC_UpdateNPC_Inner;
            On_NPC.UpdateNPC_Inner += On_NPC_UpdateNPC_Inner;
            On_NPC.UpdateCollision += On_NPC_UpdateCollision;
            On_NPC.FindFrame += On_NPC_FindFrame;
        }

        private void On_NPC_FindFrame(On_NPC.orig_FindFrame orig, NPC self)
        {
            if (self.TryGetGlobalNPC(out EnemyStatusSystem gnpc))
            {
                if (gnpc.accumulatedActions < 1)
                {
                    return;
                }
            }
            orig.Invoke(self);
        }

        public override void Unload()
        {
            IL_NPC.UpdateNPC_Inner -= IL_NPC_UpdateNPC_Inner;
            On_NPC.UpdateNPC_Inner -= On_NPC_UpdateNPC_Inner;
            On_NPC.UpdateCollision -= On_NPC_UpdateCollision;
            On_NPC.FindFrame -= On_NPC_FindFrame;
        }
        private static void On_NPC_UpdateNPC_Inner(On_NPC.orig_UpdateNPC_Inner orig, NPC self, int i)
        {
            if (self.TryGetGlobalNPC(out EnemyStatusSystem gnpc))
            {
                UpdateNPCSpeedAndAIModifier(self, gnpc);
                gnpc.accumulatedActions += gnpc.AIFactor;
                while (true)
                {
                    if (gnpc.accumulatedActions >= 2)
                    {
                        ExtraUpdate(self);
                        gnpc.accumulatedActions--;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            orig.Invoke(self, i);
            if (self.TryGetGlobalNPC(out EnemyStatusSystem gnpc2))
            {
                if (gnpc2.accumulatedActions >= 1) gnpc.accumulatedActions--;
                UpdateFireAndColdStatus(self, gnpc2);
                ResetNPCSpeedAndAIModifier(self, gnpc2);
            }
        }
        private static void On_NPC_UpdateCollision(On_NPC.orig_UpdateCollision orig, NPC self)
        {
            if (self.TryGetGlobalNPC(out EnemyStatusSystem result))
            {
                Vector2 velo = self.velocity;
                self.velocity *= result.SpeedFactor;
                Vector2 oldvelo = self.velocity;
                orig.Invoke(self);
                if (Math.Abs(self.velocity.X) >= Math.Abs(oldvelo.X))
                {
                    self.velocity.X = velo.X;
                }
                if (Math.Abs(self.velocity.Y) >= Math.Abs(oldvelo.Y))
                {
                    self.velocity.Y = velo.Y;
                }
                return;
            }
            orig.Invoke(self);
        }

        private static void IL_NPC_UpdateNPC_Inner(ILContext il)
        {
            ILCursor cursor = new(il);
            FieldInfo fieldInfo = typeof(Entity).GetField("velocity", BindingFlags.Instance | BindingFlags.Public);
            MethodInfo methodBase = typeof(Vector2).GetMethod("op_Addition", BindingFlags.Static | BindingFlags.Public);
            if (!cursor.TryGotoNext(
            [
        (Instruction i) => ILPatternMatchingExt.MatchLdfld(i, fieldInfo) && ILPatternMatchingExt.MatchCall(i.Next, methodBase)
            ]))
            {
                throw new Exception("IL Error");
            }
            ILCursor ilcursor = cursor;
            int index = ilcursor.Index;
            ilcursor.Index = index + 1;
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.EmitDelegate(delegate (Vector2 defvelo, NPC npc)
            {
                if (npc.TryGetGlobalNPC(out EnemyStatusSystem result))
                {
                    return defvelo * result.SpeedFactor;
                }
                return defvelo;
            });
        }

        private static void ExtraUpdate(NPC npc)
        {
            if (!npc.active)
            {
                return;
            }
            NPCLoader.ResetEffects(npc);
            if (npc.life <= 0)
            {
                npc.active = false;
                npc.justHit = false;
                return;
            }
            npc.AI();
            npc.FindFrame();
            npc.CheckActive();
            npc.justHit = false;
        }
        #endregion
    }
}

