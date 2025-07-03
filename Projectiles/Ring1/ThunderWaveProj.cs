using BG3MagicRework.BaseType;
using BG3MagicRework.Spells.Ring1;
using BG3MagicRework.Static;
using BG3MagicRework.Static.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Projectiles.Ring1
{
    /// <summary>
    /// TODO 把烟雾弹幕改成粒子
    /// </summary>
    public class ThunderWaveProj : BaseMagicProj
    {
        public List<TmpParticle> Particles = new();
        public List<SmokeParticle> smokeParticles1 = new();
        public List<SmokeParticle> smokeParticles2 = new();
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
            Player owner = Main.player[Projectile.owner];
            if (owner.IsDead())
            {
                Projectile.Kill();
                return;
            }

            Projectile.ai[0]++;
            if (Projectile.ai[0] > 60) Projectile.Kill();
            if (Projectile.ai[0] == 1)
            {
                for (int i = 0; i < 25; i++)
                {
                    Vector2 Pos = Projectile.Center + (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * Main.rand.Next(1, 250);
                    Vector2 Vel = Vector2.Normalize(Projectile.velocity) * Main.rand.Next(10, 40);
                    float scale = 0.25f + Main.rand.NextFloat() * 0.5f;
                    Particles.NewParticle(Pos, Vel, scale);
                }
                for (int i = 0; i < 30; i++)     //直吹灰烟
                {
                    Vector2 Pos = Projectile.Center + (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * Main.rand.Next(1, 250);
                    Vector2 Vel = Vector2.Normalize(Projectile.velocity) * Main.rand.Next(10, 20);
                    float scale = 0.25f + Main.rand.NextFloat() * 0.75f;
                    smokeParticles1.NewParticle(Pos, Vel, scale, Color.White * 0.75f);
                }
                for (int i = 0; i < 20; i++)     //周围吹的白烟
                {
                    Vector2 Pos = Projectile.Center + (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * Main.rand.Next(1, 150);
                    Vector2 Vel = (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * Main.rand.Next(10, 25);
                    float scale = 0.25f + Main.rand.NextFloat() * 0.75f;
                    smokeParticles2.NewParticle(Pos, Vel, scale, Color.White);
                }

            }
            if (Projectile.ai[0] < 10f)
                Lighting.AddLight(Projectile.Center, 5, 5, 5);
            //更新粒子
            Particles.UpdateParticle(0.9f, 0.95f);
            for (int i = 0; i < 2; i++) smokeParticles1.UpdateParticle();
            for (int i = 0; i < 3; i++) smokeParticles2.UpdateParticle(0.65f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureLibrary.Extra;
            Texture2D texHollowCircleSoftEdge = TextureLibrary.HollowCircleSoftEdge;
            EasyDraw.AnotherDraw(BlendState.NonPremultiplied);
            smokeParticles1.DrawParticle(false);
            smokeParticles2.DrawParticle(false);
            EasyDraw.AnotherDraw(BlendState.Additive);
            if (Projectile.ai[0] <= 10)
            {
                float scale = MathHelper.Lerp(0, 1, Projectile.ai[0] / 10f);
                float light = MathHelper.Lerp(1, 0, Projectile.ai[0] / 10f);
                Main.spriteBatch.Draw(texHollowCircleSoftEdge, Projectile.Center - Main.screenPosition, null, Color.White * light, 0, texHollowCircleSoftEdge.Size() / 2f, scale, SpriteEffects.None, 0);
            }
            Particles.DrawParticle(tex, Color.White, false, new Vector2(1, 1));
            EasyDraw.AnotherDraw(BlendState.AlphaBlend);
            return false;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override bool? SafeCanHitNPC(NPC target)
        {
            if (Projectile.ai[0] >= 5) return false;
            return null;
        }

        public override void SafeOnHit(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.knockBackResist > 0)
            {
                target.velocity = Projectile.velocity * 50 * target.knockBackResist;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 v1 = Vector2.Normalize((targetHitbox.Center - projHitbox.Center).ToVector2());
            Vector2 v2 = Vector2.Normalize(Projectile.velocity);
            float a = v1.X * v2.X + v1.Y * v2.Y;
            return targetHitbox.Distance(Projectile.Center) < GetAOERadius<ThunderWaveSpell>() * 16f && a > 0 &&
                (CarefulSpellMM || Collision.CanHit(Projectile.Center, 1, 1, targetHitbox.TopLeft(), targetHitbox.Width, targetHitbox.Height));
        }
    }
}