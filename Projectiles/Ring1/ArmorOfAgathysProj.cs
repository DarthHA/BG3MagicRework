using BG3MagicRework.BaseType;
using BG3MagicRework.Static;
using BG3MagicRework.Static.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;

namespace BG3MagicRework.Projectiles.Ring1
{
    public class ArmorOfAgathysProj : BaseMagicProj
    {
        public override int MaxHits => 1;
        public int TargetNPC = -1;
        public List<SmokeParticle> smokeParticles = new();
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
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 9999;
            Projectile.hide = true;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (owner.IsDead())
            {
                Projectile.Kill();
                return;
            }
            Projectile.Center = owner.Center;

            Projectile.ai[0]++;

            if (Projectile.ai[0] == 1)
            {
                for (int i = 0; i < 15; i++)
                {
                    Vector2 ShootVel = (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * Main.rand.NextFloat() * 8 + owner.velocity;
                    smokeParticles.NewParticle(Projectile.Center, ShootVel, Main.rand.NextFloat() * 0.3f + 0.3f, Color.LightBlue);
                }
            }
            smokeParticles.UpdateParticle(0.93f);

            if (Projectile.ai[0] > 60) Projectile.Kill();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            EasyDraw.AnotherDraw(BlendState.NonPremultiplied);
            smokeParticles.DrawParticle(false);

            if (Projectile.ai[0] <= 25)
            {
                float light0 = 1;
                if (Projectile.ai[0] < 5)
                {
                    light0 = MathHelper.Lerp(0, 1, Projectile.ai[0] / 5f);
                }
                else
                {
                    light0 = MathHelper.Lerp(1, 0, (Projectile.ai[0] - 5) / 20f);
                }
                Texture2D tex2 = TextureLibrary.BloomFlare;
                EasyDraw.AnotherDraw(BlendState.Additive);
                Main.spriteBatch.Draw(tex2, Projectile.Center - Main.screenPosition, null, Color.Cyan * light0, 0, tex2.Size() / 2f, 0.2f, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(tex2, Projectile.Center - Main.screenPosition, null, Color.White * light0, 0, tex2.Size() / 2f, 0.15f, SpriteEffects.None, 0);
            }

            if (Projectile.ai[0] <= 35)
            {
                EasyDraw.AnotherDraw(BlendState.Additive);
                Texture2D tex3 = TextureLibrary.SnowFlake;
                float alpha0 = 1;
                float radius0 = 1;
                if (Projectile.ai[0] < 20)
                {
                    radius0 = MathHelper.Lerp(0, 1, Projectile.ai[0] / 20f);
                }
                else
                {
                    alpha0 = MathHelper.Lerp(1, 0, (Projectile.ai[0] - 20) / 15f);
                }
                DrawUtils.DrawGradientCircle(tex3, Projectile.Center, Color.Cyan * alpha0, 0, 1f, BlendState.Additive, radius0, 0.5f);
            }

            EasyDraw.AnotherDraw(BlendState.Additive);
            float radius = 250;
            float scale = MathHelper.Lerp(0, 1, MathHelper.Clamp(Projectile.ai[0] / 20, 0, 1));
            float alpha = MathHelper.Lerp(1, 0, MathHelper.Clamp((Projectile.ai[0] - 10) / 10, 0, 1));
            Draw710(Projectile.Center, radius * scale, -Projectile.ai[0] / 200f, Color.Cyan * alpha * 0.8f, Projectile.ai[0] / 200f);
            Draw710(Projectile.Center, radius * scale, -Projectile.ai[0] / 200f, Color.White * alpha * 0.8f, Projectile.ai[0] / 200f + 0.5f);

            EasyDraw.AnotherDraw(BlendState.AlphaBlend);
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return true;
        }
        public override bool? SafeCanHitNPC(NPC target)
        {
            if (TargetNPC == -1 || TargetNPC != target.whoAmI) return false;
            return null;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
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
            DrawUtils.DrawRoSLaser(TextureLibrary.Perlin, bars, color, 0.2f, 0.6f, progress, BlendState.Additive);
        }
    }
}
