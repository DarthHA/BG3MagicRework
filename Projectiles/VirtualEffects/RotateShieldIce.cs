using BG3MagicRework.Static;
using BG3MagicRework.Static.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;

namespace BG3MagicRework.Projectiles.VirtualEffects
{
    public class RotateShieldIce : BaseDrawOrbit
    {
        public List<SmokeParticle> smokeParticles = new();

        public override void SafeAI()
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
                    smokeParticles.NewParticle(Projectile.Center, ShootVel, Main.rand.NextFloat() * 0.25f + 0.25f, Color.LightBlue);
                }
            }
            smokeParticles.UpdateParticle(0.93f);
            if (Projectile.ai[0] > 60) Projectile.Kill();
        }

        public override void DrawFront(Color lightColor)
        {
            Texture2D tex = TextureLibrary.EnergyShield;
            float scale = 0.75f;
            if (Projectile.ai[0] < 15) scale = MathHelper.Lerp(0, 0.75f, Projectile.ai[0] / 15f);
            float dist = 60 * scale;
            float light = 1;
            if (Projectile.ai[0] > 30) light = MathHelper.Lerp(1, 0, (Projectile.ai[0] - 30f) / 30f);
            for (float r0 = 0; r0 < MathHelper.TwoPi; r0 += MathHelper.TwoPi / 3f)
            {
                float r = Projectile.ai[0] * MathHelper.TwoPi / 120f + r0;
                if (IsFront(r))
                {
                    Vector2 unitY = (r - MathHelper.Pi / 2f).ToRotationVector2();
                    Vector2 d1 = r.ToRotationVector2() * dist + unitY * tex.Width * scale;
                    Vector2 d2 = r.ToRotationVector2() * dist - unitY * tex.Width * scale;
                    d1.Y *= 0.4f;
                    d2.Y *= 0.4f;
                    List<CustomVertexInfo> vertexInfos = new()
                    {
                    new CustomVertexInfo(Projectile.Center + d1 - new Vector2(0, tex.Height * scale) - Main.screenPosition, Color.White, new Vector3(0, 0f, 1)),
                    new CustomVertexInfo(Projectile.Center + d1 + new Vector2(0, tex.Height * scale) - Main.screenPosition, Color.White, new Vector3(0, 1f, 1)),
                    new CustomVertexInfo(Projectile.Center + d2 - new Vector2(0, tex.Height * scale) - Main.screenPosition, Color.White, new Vector3(1, 0f, 1)),
                    new CustomVertexInfo(Projectile.Center + d2 + new Vector2(0, tex.Height * scale) - Main.screenPosition, Color.White, new Vector3(1, 1f, 1))
                    };
                    DrawUtils.DrawTrail(tex, vertexInfos, Color.LightCyan * 0.85f * light, BlendState.Additive);
                }
            }
        }

        public override void DrawBehind(Color lightColor)
        {
            EasyDraw.AnotherDraw(BlendState.NonPremultiplied);
            smokeParticles.DrawParticle(false);

            Texture2D tex = TextureLibrary.EnergyShield;
            float scale = 0.75f;
            if (Projectile.ai[0] < 15) scale = MathHelper.Lerp(0, 0.75f, Projectile.ai[0] / 15f);
            float dist = 60 * scale;
            float light = 1;
            if (Projectile.ai[0] > 30) light = MathHelper.Lerp(1, 0, (Projectile.ai[0] - 30f) / 30f);
            for (float r0 = 0; r0 < MathHelper.TwoPi; r0 += MathHelper.TwoPi / 3f)
            {
                float r = Projectile.ai[0] * MathHelper.TwoPi / 120f + r0;
                if (!IsFront(r))
                {
                    Vector2 unitY = (r - MathHelper.Pi / 2f).ToRotationVector2();
                    Vector2 d1 = r.ToRotationVector2() * dist + unitY * tex.Width * scale;
                    Vector2 d2 = r.ToRotationVector2() * dist - unitY * tex.Width * scale;
                    d1.Y *= 0.4f;
                    d2.Y *= 0.4f;
                    List<CustomVertexInfo> vertexInfos = new()
                    {
                    new CustomVertexInfo(Projectile.Center + d1 - new Vector2(0, tex.Height * scale) - Main.screenPosition, Color.White, new Vector3(0, 0f, 1)),
                    new CustomVertexInfo(Projectile.Center + d1 + new Vector2(0, tex.Height * scale) - Main.screenPosition, Color.White, new Vector3(0, 1f, 1)),
                    new CustomVertexInfo(Projectile.Center + d2 - new Vector2(0, tex.Height * scale) - Main.screenPosition, Color.White, new Vector3(1, 0f, 1)),
                    new CustomVertexInfo(Projectile.Center + d2 + new Vector2(0, tex.Height * scale) - Main.screenPosition, Color.White, new Vector3(1, 1f, 1))
                    };
                    DrawUtils.DrawTrail(tex, vertexInfos, Color.LightCyan * 0.85f * light, BlendState.Additive);
                }
            }

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
                Main.spriteBatch.Draw(tex2, Projectile.Center - Main.screenPosition, null, Color.Cyan * light0, 0, tex2.Size() / 2f, 0.15f, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(tex2, Projectile.Center - Main.screenPosition, null, Color.White * light0, 0, tex2.Size() / 2f, 0.12f, SpriteEffects.None, 0);
            }

            if (Projectile.ai[0] <= 45)
            {
                EasyDraw.AnotherDraw(BlendState.Additive);
                Texture2D tex3 = TextureLibrary.SnowFlake;
                float alpha0 = 1;
                float radius0 = 1;
                if (Projectile.ai[0] < 30)
                {
                    radius0 = MathHelper.Lerp(0, 1, Projectile.ai[0] / 30f);
                }
                else
                {
                    alpha0 = MathHelper.Lerp(1, 0, (Projectile.ai[0] - 30) / 15f);
                }
                DrawUtils.DrawGradientCircle(tex3, Projectile.Center, Color.Cyan * alpha0, 0, 1f, BlendState.Additive, radius0, 0.5f);
            }

            EasyDraw.AnotherDraw(BlendState.AlphaBlend);
        }


    }
}
