using BG3MagicRework.BaseType;
using BG3MagicRework.Static;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;

namespace BG3MagicRework.Projectiles.Ring1
{
    public class SleepProj : BaseMagicProj
    {
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
        }

        public override void AI()
        {
            Projectile.ai[1]++;

            if (Projectile.ai[1] > 40) Projectile.Kill();

        }

        public override bool PreDraw(ref Color lightColor)
        {
            float light = MathHelper.Lerp(1, 0, MathHelper.Clamp((Projectile.ai[1] - 20f) / 20f, 0f, 1f));
            float height = MathHelper.Lerp(10, 300, GetSpeed(Projectile.ai[1] / 40f));
            float radius = MathHelper.Lerp(10, 200, Projectile.ai[1] / 40f);
            float rot = GetSpeed(Projectile.ai[1] / 40f) * MathHelper.TwoPi;
            DrawSpiral(Projectile.Center + new Vector2(0, 40), 34, 100, height, Color.Purple * light * 0.5f, rot, 4);
            DrawSpiral(Projectile.Center + new Vector2(0, 40), 32, 90, height, Color.Purple * light * 0.75f, rot, 4);
            DrawSpiral(Projectile.Center + new Vector2(0, 40), 30, 80, height, Color.Purple * light, rot, 4);
            DrawSpiral(Projectile.Center + new Vector2(0, 40), 30, 60, height, Color.White * light, rot, 4);
            DrawRing(Projectile.Center, radius, 60, Color.Purple * light, rot);
            DrawRing(Projectile.Center, radius, 50, Color.White * light, rot);

            if (Projectile.ai[1] < 30)
            {
                float scaleY = 2;
                if (Projectile.ai[1] < 20)
                {
                    scaleY = MathHelper.Lerp(20, 2, Projectile.ai[1] / 20f);
                }
                float light2 = 1;
                if (Projectile.ai[1] < 5)
                {
                    light2 = MathHelper.Lerp(0, 1, Projectile.ai[1] / 5f);
                }
                else if (Projectile.ai[1] > 20)
                {
                    light2 = MathHelper.Lerp(1, 0, (Projectile.ai[1] - 20) / 10f);
                }
                Texture2D texLightField = TextureLibrary.LightField;
                Texture2D texLight = TextureLibrary.BloomFlare;
                EasyDraw.AnotherDraw(BlendState.Additive);
                Main.spriteBatch.Draw(texLightField, Projectile.Center - Main.screenPosition, null, Color.Purple * light2, MathHelper.Pi / 2f, texLightField.Size() / 2f, new Vector2(scaleY * 0.9f, 0.3f), SpriteEffects.None, 0);
                Main.spriteBatch.Draw(texLightField, Projectile.Center - Main.screenPosition, null, Color.White * light2, MathHelper.Pi / 2f, texLightField.Size() / 2f, new Vector2(scaleY * 0.7f, 0.2f), SpriteEffects.None, 0);
                Main.spriteBatch.Draw(texLight, Projectile.Center - Main.screenPosition, null, Color.Purple * light2, 0, texLight.Size() / 2f, 0.3f, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(texLight, Projectile.Center - Main.screenPosition, null, Color.White * light2, 0, texLight.Size() / 2f, 0.2f, SpriteEffects.None, 0);
                EasyDraw.AnotherDraw(BlendState.AlphaBlend);
            }

            return false;
        }

        private static void DrawRing(Vector2 Center, float radius, float width, Color color, float rotation)
        {
            Texture2D tex = TextureLibrary.Ribbon;
            List<CustomVertexInfo> bars = new();
            for (int i = 0; i < 60; i++)
            {
                float rot = MathHelper.TwoPi / 60f * i + rotation;
                Vector2 Pos1 = rot.ToRotationVector2() * (radius + width / 2f);
                Vector2 Pos2 = rot.ToRotationVector2() * (radius - width / 2f);
                Pos1.Y *= 0.3f;
                Pos2.Y *= 0.3f;
                bars.Add(new CustomVertexInfo(Center + Pos1 - Main.screenPosition, Color.White, new Vector3(1 / 60f * i, 0, 1f)));
                bars.Add(new CustomVertexInfo(Center + Pos2 - Main.screenPosition, Color.White, new Vector3(1 / 60f * i, 1, 1f)));
            }
            bars.Add(new CustomVertexInfo(Center + new Vector2(radius + width / 2f, 0) - Main.screenPosition, Color.White, new Vector3(1f, 0, 1f)));
            bars.Add(new CustomVertexInfo(Center + new Vector2(radius - width / 2f, 0) - Main.screenPosition, Color.White, new Vector3(1f, 1, 1f)));
            float len = radius * MathHelper.TwoPi;
            DrawUtils.DrawLoopTrail(tex, bars, color, 0.2f, 0, BlendState.Additive);
        }

        private static float GetSpeed(float a)
        {
            return (float)Math.Pow(a, 0.3f);
        }

        public void DrawSpiral(Vector2 Center, float radius, float width, float height, Color color, float rotation = 0, float FullCircle = 1)
        {
            Texture2D tex = TextureLibrary.Ribbon;
            List<CustomVertexInfo> bars = new();
            for (int i = 0; i < 240; i++)
            {
                float rot = MathHelper.TwoPi / 240f * i * FullCircle + rotation;
                Vector2 R = rot.ToRotationVector2();
                R.Y *= 0.5f;
                Vector2 Pos1 = Center + R * radius + new Vector2(0, -1) * width / 2f + new Vector2(0, -1) * height * i / 240f;
                Vector2 Pos2 = Center + R * radius + new Vector2(0, 1) * width / 2f + new Vector2(0, -1) * height * i / 240f;
                bars.Add(new CustomVertexInfo(Pos1 - Main.screenPosition, Color.White, new Vector3(1 / 240f * i, 0f, 1f)));
                bars.Add(new CustomVertexInfo(Pos2 - Main.screenPosition, Color.White, new Vector3(1 / 240f * i, 1f, 1f)));
            }
            bars.Add(new CustomVertexInfo(Center + rotation.ToRotationVector2() * radius + new Vector2(0, -1) * width / 2f + new Vector2(0, -1) * height - Main.screenPosition, Color.White, new Vector3(1f, 0f, 1f)));
            bars.Add(new CustomVertexInfo(Center + rotation.ToRotationVector2() * radius + new Vector2(0, 1) * width / 2f + new Vector2(0, -1) * height - Main.screenPosition, Color.White, new Vector3(1f, 1f, 1f)));
            DrawUtils.DrawRoSLaser(tex, bars, color, 0.4f, 0.2f, 0f, BlendState.Additive);
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

    }
}
