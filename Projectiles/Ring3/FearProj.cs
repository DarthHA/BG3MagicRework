using BG3MagicRework.BaseType;
using BG3MagicRework.Buffs.Enemy;
using BG3MagicRework.Concentrations;
using BG3MagicRework.Spells.Ring3;
using BG3MagicRework.Static;
using BG3MagicRework.Static.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Projectiles.Ring3
{
    public class FearProj : BaseMagicProj
    {
        public List<TmpParticle> tmpParticles = new();
        public long UsedUUID = 0;
        public override int MaxHits => -1;
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 99999;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.netImportant = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 9999;
        }
        public override void AI()
        {
            if (UsedUUID == 0) UsedUUID = SomeUtils.GenerateUUID();
            Player owner = Main.player[Projectile.owner];
            if (owner.IsDead())
            {
                Projectile.Kill();
                return;
            }
            Projectile.ai[0]++;
            if (Projectile.ai[0] == 1)
            {
                for (int i = 0; i < 20; i++)
                {
                    float r = Projectile.velocity.ToRotation() + MathHelper.Pi / 3f * (Main.rand.NextFloat() * 2 - 1);
                    Vector2 MoveVel = r.ToRotationVector2() * Main.rand.Next(10, 25);
                    tmpParticles.NewParticle(Projectile.Center, MoveVel, 0.3f + Main.rand.NextFloat() * 0.3f);
                }
            }
            tmpParticles.UpdateParticle(0.93f, 0.97f);
            if (Projectile.ai[0] > 40)
            {
                Projectile.Kill();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            EasyDraw.AnotherDraw(BlendState.Additive);
            tmpParticles.DrawParticle(TextureLibrary.Extra, Color.MediumPurple, true, new Vector2(1, 1));
            tmpParticles.DrawParticle(TextureLibrary.Extra, Color.White, true, new Vector2(1, 1) * 0.75f);
            float alpha0 = 1;
            if (Projectile.ai[0] < 10)
            {
                alpha0 = MathHelper.Lerp(0, 1, Projectile.ai[0] / 10f);
            }
            else if (Projectile.ai[0] > 15)
            {
                alpha0 = MathHelper.Lerp(1, 0, (Projectile.ai[0] - 15f) / 25f);
            }
            Draw710(Projectile.Center, 150, -Projectile.ai[0] / 40f, Color.Purple * alpha0, 0);
            float k = (float)Math.Sqrt(Projectile.ai[0] / 40f);
            float dist = MathHelper.Lerp(0, 1, k) * GetAOERadius<FearSpell>() * 16 * 1.1f;
            float scale1 = MathHelper.Lerp(0, 1, k);
            float alpha1 = 1;
            if (Projectile.ai[0] > 10)
            {
                alpha1 = MathHelper.Lerp(1, 0, (Projectile.ai[0] - 10f) / 30f);
            }
            EasyDraw.AnotherDraw(BlendState.Additive);
            DrawSlash2(Projectile.Center, dist, Projectile.velocity.ToRotation(), Projectile.ai[0] / 40f, Color.Purple * alpha1);
            for (int i = 0; i <= 10; i++)
            {
                DrawSlash(Projectile.Center, dist, MathHelper.Lerp(80, 160, i / 10f), Projectile.velocity.ToRotation(), Projectile.ai[0] / 20f, Color.Lerp(Color.MediumPurple, Color.Purple, i / 10f) * alpha1 * 0.75f);
            }
            EasyDraw.AnotherDraw(BlendState.AlphaBlend);
            return false;
        }

        public void DrawSlash(Vector2 Center, float radius, float width, float rot, float progress, Color color)
        {
            List<CustomVertexInfo> bars = new();
            for (int i = 0; i <= 240; i++)
            {
                float r = rot - MathHelper.Pi / 3 + MathHelper.Pi / 3f * 2f * (i / 240f);
                float factor = 1 - (float)Math.Abs((i - 120f) / 240f) * 2;
                Vector2 Pos1 = r.ToRotationVector2() * (radius - width * (factor * 0.5f + 0.5f));
                Vector2 Pos2 = r.ToRotationVector2() * radius;
                bars.Add(new CustomVertexInfo(Center + Pos1 - Main.screenPosition, Color.White, new Vector3(0, i / 240f, 1f)));
                bars.Add(new CustomVertexInfo(Center + Pos2 - Main.screenPosition, Color.White, new Vector3(1, i / 240f, 1f)));
            }
            DrawUtils.DrawRoSVert(TextureLibrary.Perlin, bars, color, new Vector2(0.4f, 0.1f), new Vector2(1f, 0.5f), new Vector2(progress, 0), BlendState.Additive);
        }

        public void DrawSlash2(Vector2 Center, float radius, float rot, float progress, Color color)
        {
            List<CustomVertexInfo> bars = new();
            for (int i = 0; i <= 240; i++)
            {
                float r = rot - MathHelper.Pi / 3 + MathHelper.Pi / 3f * 2f * (i / 240f);
                float factor = 1 - (float)Math.Abs((i - 120f) / 240f) * 2;
                Vector2 Pos1 = r.ToRotationVector2() * 1;
                Vector2 Pos2 = r.ToRotationVector2() * radius;
                bars.Add(new CustomVertexInfo(Center + Pos1 - Main.screenPosition, Color.White, new Vector3(0, i / 240f, 1f)));
                bars.Add(new CustomVertexInfo(Center + Pos2 - Main.screenPosition, Color.White, new Vector3(1, i / 240f, 1f)));
            }
            DrawUtils.DrawRoSVert(TextureLibrary.Perlin, bars, color, new Vector2(0.4f, 0.1f), new Vector2(1f, 0.5f), new Vector2(progress, 0), BlendState.Additive);
        }

        public void Draw710(Vector2 Center, float radius, float progress, Color color, float rot = 0)
        {
            List<CustomVertexInfo> bars = new();
            for (int i = 0; i <= 240; i++)
            {
                float r = i * MathHelper.TwoPi / 240f + rot;
                Vector2 Pos1 = r.ToRotationVector2() * 1;
                Vector2 Pos2 = r.ToRotationVector2() * radius;
                bars.Add(new CustomVertexInfo(Center + Pos1 - Main.screenPosition, Color.White, new Vector3(0, i / 240f, 1f)));
                bars.Add(new CustomVertexInfo(Center + Pos2 - Main.screenPosition, Color.White, new Vector3(1, i / 240f, 1f)));
            }
            DrawUtils.DrawRoSLaser(TextureLibrary.Perlin, bars, color, 0.4f, 2f, progress, BlendState.Additive);
        }

        public override void SafeOnHit(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.HasBuff(ModContent.BuffType<FearDNDBuff>())) return;
            if (!this.DeepAddCCBuffByDC(target, ModContent.BuffType<FearDNDBuff>(), 2))
            {
                return;
            }
            Player owner = Main.player[Projectile.owner];
            if (owner.GetConcentration(UsedUUID) == -1)
            {
                BaseConcentration con = owner.GenerateConcentration<ConFear>(CurrentRing, GetTimeSpan<FearSpell>() * 60, true);
                if (con != null)
                {
                    con.projIndex = Projectile.whoAmI;
                    con.UUID = UsedUUID;
                }
            }
            int protmp = owner.NewMagicProj(target.Center, Vector2.Zero, ModContent.ProjectileType<FearEffectProj>(), CurrentRing);
            if (protmp >= 0 && protmp < 1000)
            {
                (Main.projectile[protmp].ModProjectile as BaseMagicProj).ConUUID = UsedUUID;
                (Main.projectile[protmp].ModProjectile as FearEffectProj).TargetNPC = target.whoAmI;
            }
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 v1 = Vector2.Normalize((targetHitbox.Center - projHitbox.Center).ToVector2());
            Vector2 v2 = Vector2.Normalize(Projectile.velocity);
            float a = v1.X * v2.X + v1.Y * v2.Y;
            float radius = MathHelper.Lerp(0, 1, Projectile.ai[0] / 40f) * GetAOERadius<FearSpell>() * 16f;
            return targetHitbox.Distance(Projectile.Center) < radius && a > 0.5 &&
                (CarefulSpellMM || Collision.CanHit(Projectile.Center, 1, 1, targetHitbox.TopLeft(), targetHitbox.Width, targetHitbox.Height));
        }
    }
}