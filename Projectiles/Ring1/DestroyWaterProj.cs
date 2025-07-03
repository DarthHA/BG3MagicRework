using BG3MagicRework.BaseType;
using BG3MagicRework.Spells.Ring1;
using BG3MagicRework.Static;
using BG3MagicRework.Static.Particles;
using BG3MagicRework.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace BG3MagicRework.Projectiles.Ring1
{
    public class DestroyWaterProj : BaseMagicProj
    {
        public override int MaxHits => -1;
        public List<DripParticle> dripParticles = new();
        public Dictionary<int, int> TargetAndTimer = new();
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 9999;
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 99999;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 9999;
        }
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (owner.IsDead())
            {
                Projectile.Kill();
                return;
            }
            Projectile.ai[0]++;

            if (Projectile.ai[0] > 15 && Projectile.ai[0] < 120)
            {
                List<int> ShouldRemove = new();
                foreach (int t in TargetAndTimer.Keys)
                {
                    NPC target = Main.npc[t];
                    if (!target.CanBeChasedBy() && !target.immortal)
                    {
                        ShouldRemove.Add(t);
                        continue;
                    }

                    float count = target.width * target.height / 10000f;
                    if (count < 1)
                    {
                        if (count < 0.05f) count = 0.05f;
                        if (Main.rand.NextFloat() < count) count = 1f;
                    }
                    for (int i = 0; i < count; i++)
                    {
                        Vector2 Pos = target.position + new Vector2(Main.rand.Next(target.width), Main.rand.Next(target.height));
                        Vector2 Vel = new Vector2(0, -1) * Main.rand.Next(20, 25);
                        dripParticles.NewParticle(Pos, Vel, Main.rand.NextFloat() + 1f);
                    }

                    TargetAndTimer[t]--;
                    if (TargetAndTimer[t] <= 0)
                    {
                        ShouldRemove.Add(t);
                        continue;
                    }
                }
                foreach (int t in ShouldRemove)
                {
                    TargetAndTimer.Remove(t);
                }
            }

            if (Projectile.ai[0] < 180)
            {
                SomeUtils.AddLightLine(Projectile.Center, Projectile.Center - new Vector2(0, 1200), Color.White, 12);
            }

            if (Projectile.ai[0] > 180)
            {
                Projectile.Kill();
            }

            //更新雨点
            dripParticles.UpdateParticle(Projectile.Center, false);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            dripParticles.DrawParticle();

            if (Projectile.ai[0] < 30)
            {
                float scale = MathHelper.Lerp(0, 1, Math.Clamp(Projectile.ai[0] / 5f, 0, 1));
                float light = 1f;
                if (Projectile.ai[0] > 20)
                {
                    light = MathHelper.Lerp(1, 0, (Projectile.ai[0] - 20) / 10f);
                }
                Vector2 TopPos = Projectile.Center - new Vector2(0, 1500);
                Vector2 unitY = new Vector2(1, 0);
                List<CustomVertexInfo> bars0 = new()
                {
                    new CustomVertexInfo(TopPos + unitY * GetAOERadius<CreateWaterSpell>() * 16 * (1 + CurrentRing) * 0.4f * scale - Main.screenPosition, Color.White, new Vector3(0, 0f, 1)),
                    new CustomVertexInfo(TopPos - unitY * GetAOERadius<CreateWaterSpell>() * 16 * (1 + CurrentRing) * 0.4f * scale - Main.screenPosition, Color.White, new Vector3(0, 1f, 1)),
                    new CustomVertexInfo(Projectile.Center + unitY * GetAOERadius <CreateWaterSpell>() * 16 * (1 + CurrentRing) * scale - Main.screenPosition, Color.White, new Vector3(1, 0f, 1)),
                    new CustomVertexInfo(Projectile.Center - unitY * GetAOERadius <CreateWaterSpell>() * 16 * (1 + CurrentRing) * scale - Main.screenPosition, Color.White, new Vector3(1, 1f, 1)),
                };
                DrawUtils.DrawRoSLaser(Terraria.GameContent.TextureAssets.MagicPixel.Value, bars0, Color.Cyan * 0.5f * light, 0.4f, 1f, 0, BlendState.Additive);
            }
            return false;
        }

        public override void SafeOnHit(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.GetGlobalNPC<EnemyStatusSystem>().FireAndColdStatus == FireAndColdStatusID.Wet)
            {
                target.DeepClearWetAndChilled(0);
                if (!TargetAndTimer.TryAdd(target.whoAmI, 30))
                {
                    TargetAndTimer[target.whoAmI] = 30;
                }
            }
        }

        public override bool? SafeCanHitNPC(NPC target)
        {
            if (Projectile.ai[0] > 15 && Projectile.ai[0] < 120)
            {
                return null;
            }
            return false;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            int X = (int)(Projectile.Center.X - GetAOERadius<CreateWaterSpell>() * 16 * CurrentRing);
            int Y = (int)(Main.screenPosition.Y - 30);
            int Width = (int)(GetAOERadius<CreateWaterSpell>() * 16 * CurrentRing * 2);
            int Height = Main.screenHeight + 60;
            Rectangle rect = new(X, Y, Width, Height);
            return targetHitbox.Intersects(rect) &&
                (CarefulSpellMM || Collision.CanHit(Projectile.Center, 1, 1, targetHitbox.TopLeft(), targetHitbox.Width, targetHitbox.Height));
        }

        public static Vector2 GetTileBlockedEndPos(Vector2 start, Vector2 end)
        {
            bool HitTile = true;
            float maxLen = end.Distance(start);
            if (maxLen <= 16) return end;
            float currentLen;
            float result = maxLen;
            for (currentLen = 16; currentLen <= maxLen; currentLen += 16)
            {
                Vector2 pos = start + Vector2.Normalize(end - start) * currentLen;
                if (!Collision.SolidCollision(pos, 1, 1))        //无物块
                {
                    HitTile = false;
                }
                else
                {
                    if (!HitTile)
                    {
                        result = currentLen;
                        break;
                    }
                }
            }
            if (HitTile) return start;
            return start + Vector2.Normalize(end - start) * result;
        }
    }
}
