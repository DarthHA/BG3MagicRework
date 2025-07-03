using BG3MagicRework.BaseType;
using BG3MagicRework.Spells.Ring2;
using BG3MagicRework.Static;
using BG3MagicRework.Static.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Projectiles.Ring2
{
    public class ShatterProj : BaseMagicProj
    {
        public List<TmpParticle> Particles = new();
        public List<SmokeParticle> smokeParticles = new();
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
                    Vector2 Vel = (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * Main.rand.Next(10, 25);
                    float scale = 0.25f + Main.rand.NextFloat() * 0.5f;
                    Particles.NewParticle(Pos, Vel, scale);
                }
                for (int i = 0; i < 35; i++)
                {
                    Vector2 Pos = Projectile.Center + (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * Main.rand.Next(1, 150);
                    Vector2 Vel = (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * Main.rand.Next(10, 60);
                    float scale = 0.25f + Main.rand.NextFloat() * 0.75f;
                    smokeParticles.NewParticle(Pos, Vel, scale, Color.White);
                }

            }
            if (Projectile.ai[0] < 10f)
                Lighting.AddLight(Projectile.Center, 5, 5, 5);
            //更新粒子
            Particles.UpdateParticle(0.9f, 0.95f);
            for (int i = 0; i < 3; i++) smokeParticles.UpdateParticle(0.75f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureLibrary.Extra;
            Texture2D texHollowCircleSoftEdge = TextureLibrary.HollowCircleSoftEdge;
            EasyDraw.AnotherDraw(BlendState.NonPremultiplied);
            smokeParticles.DrawParticle(false);
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

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return targetHitbox.Distance(Projectile.Center) < 16 * GetAOERadius<ShatterSpell>() &&
                (CarefulSpellMM || Collision.CanHit(Projectile.Center, 1, 1, targetHitbox.TopLeft(), targetHitbox.Width, targetHitbox.Height));
        }
    }
}
