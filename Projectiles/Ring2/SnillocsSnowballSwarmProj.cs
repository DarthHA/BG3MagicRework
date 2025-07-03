using BG3MagicRework.BaseType;
using BG3MagicRework.Static;
using BG3MagicRework.Static.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BG3MagicRework.Projectiles.Ring2
{
    public class SnillocsSnowballSwarmProj : BaseMagicProj
    {
        public List<SnowballParticle> snowballParticles = new();
        public List<TmpParticle> Particles = new();

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
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 9999;
            Projectile.netImportant = true;
        }


        public override void AI()
        {
            Projectile.ai[1]++;
            if (Projectile.ai[1] == 30)
            {
                for (int i = 0; i < 30; i++)
                {
                    Vector2 Velocity = (Main.rand.NextFloat() * MathHelper.Pi).ToRotationVector2() * Main.rand.NextFloat() * 15;
                    snowballParticles.NewParticle(Projectile.Center, Velocity, 0.08f);
                }
                for (int i = 0; i < 60; i++)
                {
                    Vector2 Velocity = ((Main.rand.NextFloat() - 1) * MathHelper.Pi).ToRotationVector2() * Main.rand.NextFloat() * 15;
                    snowballParticles.NewParticle(Projectile.Center, Velocity, 0.08f);
                }
            }
            snowballParticles.UpdateParticle(0.97f, 1f, 1f, !CarefulSpellMM, Particles);
            Particles.UpdateParticle(0.93f, 0.95f);
            if (Projectile.ai[1] > 60 && snowballParticles.Count <= 0 && Particles.Count <= 0) Projectile.Kill();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texLight = TextureLibrary.BloomFlare;
            Texture2D texHollowCircleSoftEdge = TextureLibrary.HollowCircleSoftEdge;
            EasyDraw.AnotherDraw(BlendState.Additive);
            Particles.DrawParticle(TextureLibrary.Extra, Color.White * 0.5f, false, new Vector2(1, 1));
            snowballParticles.DrawParticle(false);

            if (Projectile.ai[1] < 30)
            {
                float scale0 = 1;
                float light0 = 1;
                if (Projectile.ai[1] < 10)
                {
                    scale0 = MathHelper.Lerp(0, 1, Projectile.ai[1] / 10f);
                }
                else
                {
                    light0 = MathHelper.Lerp(1, 0, (Projectile.ai[1] - 10) / 20f);
                }
                EasyDraw.AnotherDraw(BlendState.Additive);
                Main.spriteBatch.Draw(texLight, Projectile.Center - Main.screenPosition, null, Color.Cyan * light0, 0, texLight.Size() / 2f, 0.2f * scale0, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(texLight, Projectile.Center - Main.screenPosition, null, Color.White * light0, 0, texLight.Size() / 2f, 0.15f * scale0, SpriteEffects.None, 0);
            }
            if (Projectile.ai[1] >= 30)
            {
                float scale1 = MathHelper.Lerp(0, 1, MathHelper.Clamp((Projectile.ai[1] - 30) / 15f, 0, 1));
                float light1 = MathHelper.Lerp(1, 0, MathHelper.Clamp((Projectile.ai[1] - 35) / 10f, 0, 1));
                Draw710(Projectile.Center, 300 * scale1, Color.LightCyan * light1, -Projectile.ai[1] / 12f, Projectile.ai[1] / 100f);
                EasyDraw.AnotherDraw(BlendState.Additive);
                Main.spriteBatch.Draw(texHollowCircleSoftEdge, Projectile.Center - Main.screenPosition, null, Color.LightCyan * light1, 0, texHollowCircleSoftEdge.Size() / 2f, 0.55f * scale1, SpriteEffects.None, 0);
            }
            if (Projectile.ai[1] >= 30)
            {
                float light2 = MathHelper.Lerp(1, 0, MathHelper.Clamp((Projectile.ai[1] - 30) / 15f, 0, 1));
                EasyDraw.AnotherDraw(BlendState.Additive);
                Main.spriteBatch.Draw(texLight, Projectile.Center - Main.screenPosition, null, Color.Cyan * light2, 0, texLight.Size() / 2f, 0.2f, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(texLight, Projectile.Center - Main.screenPosition, null, Color.White * light2, 0, texLight.Size() / 2f, 0.15f, SpriteEffects.None, 0);
            }

            EasyDraw.AnotherDraw(BlendState.AlphaBlend);
            return false;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public void Draw710(Vector2 Center, float radius, Color color, float progress, float rotation)
        {

            List<CustomVertexInfo> bars2 = new();
            for (int i = 0; i <= 240; i++)
            {
                float r = i * MathHelper.TwoPi / 240f + rotation;
                Vector2 Pos1 = r.ToRotationVector2() * 1;
                Vector2 Pos2 = r.ToRotationVector2() * radius;
                bars2.Add(new CustomVertexInfo(Center + Pos1 - Main.screenPosition, Color.White, new Vector3(0, i / 240f, 1f)));
                bars2.Add(new CustomVertexInfo(Center + Pos2 - Main.screenPosition, Color.White, new Vector3(1, i / 240f, 1f)));
            }
            DrawUtils.DrawRoSLaser(TextureLibrary.Perlin, bars2, color, 0.3f, 2f, progress, BlendState.Additive);
        }


        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            foreach (SnowballParticle particle in snowballParticles)
            {
                if (targetHitbox.Contains(particle.Position.ToPoint()))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
